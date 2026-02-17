# CodeOwners

![GitHub Actions Badge](https://github.com/vhatsura/codeowners/actions/workflows/continuous.integration.yml/badge.svg)
[![NuGet Badge](https://buildstats.info/nuget/CodeOwners)](https://www.nuget.org/packages/CodeOwners/)

## Installation

```powershell
Install-Package CodeOwnersParser
```

## Usage

```csharp
using CodeOwners;

IEnumerable<CodeOwnersEntry> entries = CodeOwnersSerializer.Deserialize("*       @global-owner1 @global-owner2");

string content = CodeOwnersSerializer.Serialize(entries);
```

## Roadmap

- [ ] IsValid method
- [ ] GitLab support
