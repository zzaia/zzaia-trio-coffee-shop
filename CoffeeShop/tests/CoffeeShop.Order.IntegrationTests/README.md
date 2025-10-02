# Integration Tests

These tests make real HTTP calls to external services and are skipped by default.

## Prerequisites

- Redis running on localhost:6379
- Access to https://challenge.trio.dev API

## Running Integration Tests

```bash
# Run all tests including integration tests
dotnet test --filter "Category=Integration"

# Run without integration tests (default)
dotnet test --filter "Category!=Integration"
```

## Unskip Tests

Remove the Skip parameter from [Fact(Skip = "...")] to run the tests with real API calls.
