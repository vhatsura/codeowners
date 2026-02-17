# Project: CodeOwners

.NET 8.0 library for parsing and serializing CODEOWNERS files.

## Build & Test

- `dotnet build --configuration Release` - build the project
- `dotnet test tests/CodeOwners.Tests/` - run tests

## Project Structure

- `src/CodeOwners/` - main library (CodeOwnersSerializer, CodeOwnersEntry record)
- `tests/CodeOwners.Tests/` - xUnit 3 tests with Shouldly assertions
- Snapshot testing via Snapshooter.Xunit3, snapshots in `tests/CodeOwners.Tests/__snapshots__/`

## CI/CD

- Super-linter v8 with config in `.github/linters/`
- Zizmor config: `.github/linters/zizmor.yaml`
- JSCPD config: `.github/linters/.jscpd.json`
- NuGet publishing uses OIDC trusted publishing (`nuget/login@v1`)

## Code Style

- Use collection expressions (`["a", "b"]`) over `new List<string> { "a", "b" }` where possible
- XML doc comments required on public members (CS1591 is enabled)
