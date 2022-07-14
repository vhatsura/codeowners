# CodeOwners Parser

![GitHub Actions Badge](https://github.com/vhatsura/codeowners-parser/actions/workflows/continuous.integration.yml/badge.svg)
[![NuGet Badge](https://buildstats.info/nuget/CodeOwnersParser)](https://www.nuget.org/packages/CodeOwnersParser/)

## Installation

```powershell
Install-Package CodeOwnersParser
```

## Usage

```csharp
using CodeOwners;

CodeOwnersParser.Parse("*       @global-owner1 @global-owner2");
```

## Roadmap

* [X] Parse method
* [X] Benchmark
* [ ] IsValid method
* [ ] GitLab support
* [ ] Continuous Integration
