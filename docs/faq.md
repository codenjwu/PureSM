# FAQ & Troubleshooting

Q: Why am I seeing warnings about `net5.0` or `net7.0` being out of support?
A: The library is multi-targeted for compatibility. Those warnings are informational — upgrade the target if you require a supported runtime.

Q: I get build errors about `ImplicitUsings` or `Dictionary.AsReadOnly()` on older targets.
A: The project conditionally enables `ImplicitUsings` for targets other than `net5.0`. A compatibility shim (ReadOnlyDictionary wrapper) is used where APIs differ across frameworks.

Q: Context.GetItem<T> requires reference types — how do I store ints or structs?
A: Store value types as `object` and cast when retrieving (examples use this pattern). Eg `SetItem("count", (object)0);` and `(int)context.GetItem<object>("count");`.

Q: How do I stop an example that runs indefinitely?
A: Examples are designed to finish; the Traffic Light example completes after configured cycles. If you run a modified example that loops, press Ctrl+C to terminate.

Q: Where are the tests?
A: Unit tests are under `tests/PureSM.Tests`. Run them with `dotnet test` in that folder.

Still stuck?
- Open an issue or share the error output; I can help debug specific failures.