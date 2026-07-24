# Performance Benchmarks

## Overview
KObjectMapper caches property reflection results using `ConcurrentDictionary<Type, PropertyInfo[]>` to minimize overhead on warm paths.

## Cold-Start vs Warm-Path

| Scenario | Description |
|---|---|
| Cold start | First mapping call for a type pair; reflection runs and result is cached |
| Warm path | Subsequent calls reuse cached `PropertyInfo[]`; no reflection overhead |

## Thread Safety
- All internal caches use `ConcurrentDictionary` — safe for concurrent reads and writes.
- No shared mutable state exists outside of thread-safe collections.
- Parallel mapping tests verify correctness under high concurrency (50+ concurrent tasks).

## Benchmark Guidance
To measure performance, use [BenchmarkDotNet](https://benchmarkdotnet.org/):
1. Add a `benchmarks\KObjectMapper.Benchmarks` project referencing `KObjectMapper`.
2. Create a benchmark class with `[Benchmark]` methods for cold-start and warm-path scenarios.
3. Run with `dotnet run -c Release`.

## Known Characteristics
- Warm-path mapping is allocation-light; only target property writes allocate.
- Cold-start incurs one-time reflection cost per type, amortized over subsequent calls.
