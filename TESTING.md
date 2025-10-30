# Testing Guide

This document describes the testing infrastructure and automated testing setup for the HelloWorldApp project.

## Overview

The project includes comprehensive unit tests with automated CI/CD pipeline integration using GitHub Actions.

- **Unit Tests**: 40 tests using xUnit and Moq
- **Test Framework**: xUnit 2.8.0
- **Mocking Library**: Moq 4.20.70
- **CI/CD**: GitHub Actions

## Local Testing

### Running Tests Locally

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity detailed

# Run specific test class
dotnet test --filter "HelloWorldApp.Tests.Services.AuthenticationServiceTests"

# Run tests with code coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Test Project Structure

```
HelloWorldApp.Tests/
├── Fixtures/
│   └── ServiceFixture.cs              # Common test fixtures
├── Services/
│   ├── AuthenticationServiceTests.cs  # 5 tests
│   └── ApiServiceTests.cs             # 10 tests
├── Models/
│   └── AppSettingsTests.cs            # 6 tests
├── Exceptions/
│   └── ExceptionTests.cs              # 11 tests
├── ApplicationTests.cs                # 8 tests
└── Usings.cs
```

### Test Coverage Summary

| Component | Tests | Coverage |
|-----------|-------|----------|
| AuthenticationService | 5 | Constructor validation, initialization |
| ApiService | 10 | Token validation, error handling |
| Application | 8 | Workflow orchestration, DI validation |
| Models | 6 | Configuration model validation |
| Exceptions | 11 | Exception creation and chaining |
| **Total** | **40** | **100% of public APIs** |

## Git Hooks (Pre-commit Testing)

### Setup

Run the setup script to enable pre-commit hooks:

```bash
./.githooks/setup-hooks.sh
```

Or manually configure:

```bash
git config core.hooksPath .githooks
chmod +x .githooks/pre-commit
```

### What It Does

The pre-commit hook automatically runs all unit tests before each commit:

- ✅ Compiles the code
- ✅ Runs all 40 unit tests
- ✅ Aborts commit if tests fail
- ✅ Shows test results

### Usage

**Normal commit (tests run automatically):**
```bash
git commit -m "Your commit message"
```

If tests fail, fix them and try again.

**Bypass hooks (not recommended):**
```bash
git commit --no-verify -m "Your commit message"
```

## Continuous Integration (GitHub Actions)

### Workflows

#### 1. Build and Test Workflow
**File**: `.github/workflows/build-and-test.yml`

Triggers on:
- Push to `main` or `develop` branches
- Pull requests to `main` or `develop` branches

**Jobs**:
1. **Build and Test** (Ubuntu latest)
   - Checkout code
   - Setup .NET 9.0
   - Restore dependencies
   - Build solution (Release config)
   - Run all tests
   - Upload test results as artifacts
   - Publish test results to PR

2. **Code Coverage** (Ubuntu latest)
   - Run tests with code coverage enabled
   - Generate OpenCover XML report
   - Upload to Codecov

### Viewing Results

#### GitHub Actions Dashboard
1. Go to your repository
2. Click "Actions" tab
3. Select "Build and Test" workflow
4. View latest run details

#### Pull Request Comments
- Test results are automatically posted as comments on PRs
- Shows pass/fail status and statistics

#### Codecov Integration
- Coverage reports uploaded to Codecov
- View at: `https://app.codecov.io/gh/[owner]/[repo]`

## Automated Testing Configuration

### Status Checks
The workflow is configured to:
- ✅ Run on every push to main/develop
- ✅ Run on every pull request
- ✅ Block PR merge if tests fail (when configured in repo settings)
- ✅ Generate test artifacts
- ✅ Publish coverage metrics

### Environment
- **OS**: Ubuntu Latest
- **.NET Version**: 9.0.x
- **Build Configuration**: Release
- **Test Framework**: xUnit

## Test Execution Flow

```
┌─────────────────────────────────────────┐
│   Developer commits code                │
└─────────────────────┬───────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────┐
│   Pre-commit hook runs (local)          │
│   - Runs all 40 unit tests              │
│   - Aborts if tests fail                │
└─────────────────────┬───────────────────┘
                      │
              (if passing)
                      │
                      ▼
┌─────────────────────────────────────────┐
│   Push to GitHub                        │
└─────────────────────┬───────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────┐
│   GitHub Actions triggered              │
│   - Build job                           │
│   - Test job                            │
│   - Coverage job                        │
└─────────────────────┬───────────────────┘
                      │
              (if all pass)
                      │
                      ▼
┌─────────────────────────────────────────┐
│   ✅ Tests passed                        │
│   - PR mergeable                        │
│   - Coverage report available           │
└─────────────────────────────────────────┘
```

## Writing Tests

### Test Structure (AAA Pattern)

```csharp
[Fact]
public void MethodName_Scenario_ExpectedOutcome()
{
    // Arrange - Setup test data and mocks
    var service = new MyService(mockDependency.Object);

    // Act - Execute the code under test
    var result = service.DoSomething();

    // Assert - Verify the outcome
    Assert.Equal(expected, result);
}
```

### Using Theory Tests

```csharp
[Theory]
[InlineData(input1, expected1)]
[InlineData(input2, expected2)]
public void MethodName_WithVariousInputs(string input, string expected)
{
    // Test with multiple data sets
    Assert.Equal(expected, ProcessInput(input));
}
```

### Mocking Services

```csharp
// Create mock
var mockService = new Mock<IMyService>();
mockService
    .Setup(x => x.Method(It.IsAny<string>()))
    .ReturnsAsync("result");

// Verify method was called
mockService.Verify(x => x.Method("test"), Times.Once);
```

## Troubleshooting

### Tests Fail Locally But Pass on CI

1. Clean and rebuild:
   ```bash
   dotnet clean
   dotnet build
   dotnet test
   ```

2. Check .NET version:
   ```bash
   dotnet --version
   ```

3. Check dependencies:
   ```bash
   dotnet list package
   ```

### Pre-commit Hook Not Working

1. Verify hook is executable:
   ```bash
   ls -la .githooks/pre-commit
   ```

2. Reconfigure git:
   ```bash
   git config core.hooksPath .githooks
   ```

3. Check hook file format (no Windows line endings):
   ```bash
   file .githooks/pre-commit
   ```

### Test Results Not Publishing

- Ensure `EnricoMi/publish-unit-test-result-action@v2` has correct permissions
- Check that test results XML is being generated
- Verify workflow has `write` permissions

## Best Practices

✅ **Do**:
- Run tests locally before pushing
- Write tests for new features
- Keep tests independent and isolated
- Use meaningful test names
- Mock external dependencies
- Maintain green test suite

❌ **Don't**:
- Commit failing tests
- Use `--no-verify` without reason
- Create test interdependencies
- Test implementation details
- Mock classes under test

## Continuous Improvement

### Metrics Tracked
- ✅ Total test count (currently: 40)
- ✅ Test pass rate (target: 100%)
- ✅ Code coverage (target: >80%)
- ✅ Execution time (target: <60s)
- ✅ CI/CD success rate

### Adding New Tests

When adding new features:
1. Write unit tests first (TDD approach)
2. Implement feature
3. Run tests: `dotnet test`
4. Commit changes
5. Push to GitHub
6. Verify CI/CD passes

## Support and Documentation

- **Test Framework**: https://xunit.net/
- **Moq Documentation**: https://github.com/moq/moq4
- **GitHub Actions**: https://docs.github.com/en/actions
- **Code Coverage**: https://codecov.io/

## Status Badge

Add this to your README.md to display CI status:

```markdown
[![Build and Test](https://github.com/[owner]/[repo]/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/[owner]/[repo]/actions/workflows/build-and-test.yml)
```
