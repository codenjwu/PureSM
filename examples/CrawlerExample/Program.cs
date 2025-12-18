using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PureSM;

namespace CrawlerExample
{
    // Web Crawler example - demonstrating a crawler state machine
    public class Program
    {
        static async Task Main()
        {
            Console.WriteLine("=== Web Crawler State Machine Example ===\n");

            // Create the context for crawler state
            var context = new Context();
            var urlQueue = new Queue<string>();
            urlQueue.Enqueue("https://example.com");
            urlQueue.Enqueue("https://example.com/page1");
            urlQueue.Enqueue("https://example.com/page2");
            var crawledUrls = new HashSet<string>();
            
            context.SetItem("url_queue", (object)urlQueue);
            context.SetItem("crawled_urls", (object)crawledUrls);
            context.SetItem("current_url", (object)"");
            context.SetItem("links_found", (object)0);

            // Create states
            var idleState = new IdleState(context);
            var fetchState = new FetchState(context);
            var parseState = new ParseState(context);
            var extractState = new ExtractState(context);
            var completeState = new CompleteState(context);

            // Condition: Check if there are more URLs to crawl
            Func<Context, State, Task<bool>> hasMoreUrls = async (ctx, state) =>
            {
                var queue = (Queue<string>)ctx.GetItem<object>("url_queue");
                return await Task.FromResult(queue.Count > 0);
            };

            // Condition: Check if crawling should continue
            Func<Context, State, Task<bool>> shouldContinue = async (ctx, state) =>
            {
                return await hasMoreUrls(ctx, state);
            };

            // Transitions from Idle
            var idleToFetch = new Transition(
                async (ctx, state) => await hasMoreUrls(ctx, state),
                new System.Collections.Generic.List<State> { fetchState },
                null
            );

            var idleToComplete = new Transition(
                async (ctx, state) => !(await hasMoreUrls(ctx, state)),
                new System.Collections.Generic.List<State> { completeState },
                null
            );

            // Transitions from Fetch
            var fetchToParse = new Transition(
                async (ctx, state) => await Task.FromResult(true),
                new System.Collections.Generic.List<State> { parseState },
                null
            );

            // Transitions from Parse
            var parseToExtract = new Transition(
                async (ctx, state) => await Task.FromResult(true),
                new System.Collections.Generic.List<State> { extractState },
                null
            );

            // Transitions from Extract - either continue or complete
            var extractToIdle = new Transition(
                async (ctx, state) => await shouldContinue(ctx, state),
                new System.Collections.Generic.List<State> { idleState },
                null
            );

            var extractToComplete = new Transition(
                async (ctx, state) => !(await shouldContinue(ctx, state)),
                new System.Collections.Generic.List<State> { completeState },
                null
            );

            // Add transitions to states
            idleState.AddTransition(idleToFetch);
            idleState.AddTransition(idleToComplete);
            fetchState.AddTransition(fetchToParse);
            parseState.AddTransition(parseToExtract);
            extractState.AddTransition(extractToIdle);
            extractState.AddTransition(extractToComplete);

            // Build the state machine
            var stateMachine = new StateMachineBuilder()
                .AddInitialState(idleState)
                .AddState(fetchState)
                .AddState(parseState)
                .AddState(extractState)
                .AddState(completeState)
                .WithContext(context)
                .Build();

            // Run the crawler
            Console.WriteLine("Starting web crawler...\n");
            await stateMachine.StartAsync();

            Console.WriteLine("\n=== Crawler execution complete ===");
        }
    }

    // Idle State - Checks if there are URLs to crawl
    public class IdleState : State
    {
        public IdleState(Context context) : base(context, false) { }

        public override Task<State> Entry()
        {
            var crawledUrls = (HashSet<string>)Context.GetItem<object>("crawled_urls");
            var linksFound = (int)Context.GetItem<object>("links_found");
            
            if (crawledUrls.Count > 0)
            {
                Console.WriteLine($"\n⏸️  IDLE - Resuming from queue...");
                Console.WriteLine($"   URLs crawled so far: {crawledUrls.Count}, Links found: {linksFound}");
            }
            else
            {
                Console.WriteLine($"\n⏸️  IDLE - Starting crawler...");
            }
            
            return Task.FromResult<State>(this);
        }

        public override Task<State> Action()
        {
            return Task.FromResult<State>(this);
        }

        public override Task<State> Exit()
        {
            return Task.FromResult<State>(this);
        }
    }

    // Fetch State - Retrieves URL content
    public class FetchState : State
    {
        public FetchState(Context context) : base(context, false) { }

        public override Task<State> Entry()
        {
            Console.WriteLine("\n📡 FETCHING - Retrieving page content...");
            return Task.FromResult<State>(this);
        }

        public override Task<State> Action()
        {
            var urlQueue = (Queue<string>)Context.GetItem<object>("url_queue");
            
            if (urlQueue.Count > 0)
            {
                var url = urlQueue.Dequeue();
                Context.SetItem("current_url", (object)url);
                
                Console.WriteLine($"   URL: {url}");
                Console.WriteLine($"   Status: 200 OK");
                Console.WriteLine($"   Size: {new Random().Next(5, 50)} KB");
            }
            
            return Task.FromResult<State>(this);
        }

        public override Task<State> Exit()
        {
            Console.WriteLine("   ✓ Content retrieved successfully");
            return Task.FromResult<State>(this);
        }
    }

    // Parse State - Analyzes page content
    public class ParseState : State
    {
        public ParseState(Context context) : base(context, false) { }

        public override Task<State> Entry()
        {
            Console.WriteLine("\n📖 PARSING - Analyzing content...");
            return Task.FromResult<State>(this);
        }

        public override Task<State> Action()
        {
            var currentUrl = (string)Context.GetItem<object>("current_url");
            Console.WriteLine($"   Parsing: {currentUrl}");
            Console.WriteLine($"   Content type: text/html");
            Console.WriteLine($"   Encoding: UTF-8");
            
            return Task.FromResult<State>(this);
        }

        public override Task<State> Exit()
        {
            Console.WriteLine("   ✓ Content parsed successfully");
            return Task.FromResult<State>(this);
        }
    }

    // Extract State - Extracts links from content
    public class ExtractState : State
    {
        public ExtractState(Context context) : base(context, false) { }

        public override Task<State> Entry()
        {
            Console.WriteLine("\n🔗 EXTRACTING - Finding links...");
            return Task.FromResult<State>(this);
        }

        public override Task<State> Action()
        {
            var urlQueue = (Queue<string>)Context.GetItem<object>("url_queue");
            var crawledUrls = (HashSet<string>)Context.GetItem<object>("crawled_urls");
            var currentUrl = (string)Context.GetItem<object>("current_url");
            
            // Mark URL as crawled
            crawledUrls.Add(currentUrl);
            
            // Simulate extracting 2-4 new links
            int newLinks = new Random().Next(2, 5);
            Console.WriteLine($"   Found {newLinks} new links");
            
            for (int i = 0; i < newLinks && urlQueue.Count < 10; i++)
            {
                string newUrl = $"https://example.com/page{urlQueue.Count + i}";
                if (!crawledUrls.Contains(newUrl))
                {
                    urlQueue.Enqueue(newUrl);
                    Console.WriteLine($"   → Added to queue: {newUrl}");
                }
            }
            
            var linksFound = (int)Context.GetItem<object>("links_found");
            Context.SetItem("links_found", (object)(linksFound + newLinks));
            
            return Task.FromResult<State>(this);
        }

        public override Task<State> Exit()
        {
            var urlQueue = (Queue<string>)Context.GetItem<object>("url_queue");
            if (urlQueue.Count > 0)
            {
                Console.WriteLine($"   ✓ Links extracted. Queue size: {urlQueue.Count}");
            }
            else
            {
                Console.WriteLine($"   ✓ Links extracted. Queue empty.");
            }
            
            return Task.FromResult<State>(this);
        }
    }

    // Complete State - Final state
    public class CompleteState : State
    {
        public CompleteState(Context context) : base(context, isEndState: true) { }

        public override Task<State> Entry()
        {
            Console.WriteLine("\n✅ COMPLETE - Crawling finished!");
            
            var crawledUrls = (HashSet<string>)Context.GetItem<object>("crawled_urls");
            var linksFound = (int)Context.GetItem<object>("links_found");
            
            Console.WriteLine($"   Total URLs crawled: {crawledUrls.Count}");
            Console.WriteLine($"   Total links found: {linksFound}");
            Console.WriteLine($"\n   Crawled URLs:");
            foreach (var url in crawledUrls)
            {
                Console.WriteLine($"     • {url}");
            }
            
            return Task.FromResult<State>(this);
        }

        public override Task<State> Action()
        {
            Console.WriteLine("\n   Generating crawl report...");
            Console.WriteLine("   Report saved to crawler_report.txt");
            return Task.FromResult<State>(this);
        }

        public override Task<State> Exit()
        {
            return Task.FromResult<State>(this);
        }
    }
}
