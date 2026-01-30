# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-01-29

### Added
- Initial release of PureSM
- Lightweight state machine implementation with fluent builder API
- Async-friendly state lifecycle hooks (Entry/Action/Exit)
- Support for conditional transitions
- Context object for passing data between states
- Terminal (end) states support
- Multi-target support for .NET 5.0, 6.0, 7.0, 8.0, and 9.0
- Comprehensive unit tests
- Example projects:
  - TrafficLightExample - Simple state machine demonstration
  - OrderProcessingExample - Conditional transitions example
  - CrawlerExample - Iterative workflow pattern

[1.0.0]: https://github.com/codenjwu/PureSM/releases/tag/v1.0.0
