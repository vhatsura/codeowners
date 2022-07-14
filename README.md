# CodeOwners Parser

![GitHub Actions Badge](https://github.com/vhatsura/codeowners-parser/actions/workflows/continuous.integration.yml/badge.svg)
[![NuGet Badge](https://buildstats.info/nuget/CodeOwnersParser)](https://www.nuget.org/packages/CodeOwnersParser/)

## Installation

```shell
Install-Package CodeOwnersParser
```

## Usage

```csharp
CodeOwnersParser.Parse("*       @global-owner1 @global-owner2");
```

## Benchmark results

```text
BenchmarkDotNet=v0.13.1, OS=macOS Monterey 12.4 (21F79) [Darwin 21.5.0]
Apple M1 Max, 1 CPU, 10 logical and 10 physical cores
.NET SDK=6.0.201
[Host]     : .NET 6.0.3 (6.0.322.12309), Arm64 RyuJIT
DefaultJob : .NET 6.0.3 (6.0.322.12309), Arm64 RyuJIT
```

|    Method |       Mean |    Error |   StdDev |  Gen 0 | Allocated |
|---------- |-----------:|---------:|---------:|-------:|----------:|
|   OneLine |   290.3 ns |  1.07 ns |  0.95 ns | 0.2294 |     480 B |
| MultiLine | 9,115.7 ns | 23.67 ns | 20.99 ns | 1.4038 |   2,944 B |

```text
Mean      : Arithmetic mean of all measurements
Error     : Half of 99.9% confidence interval
StdDev    : Standard deviation of all measurements
Gen 0     : GC Generation 0 collects per 1000 operations
Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
1 ns      : 1 Nanosecond (0.000000001 sec)
```

## Roadmap

* [X] Parse method
* [X] Benchmark
* [ ] IsValid method
* [ ] GitLab support
* [ ] Continuous Integration
