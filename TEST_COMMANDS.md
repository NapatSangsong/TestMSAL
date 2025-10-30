# Quick Test Commands Reference

## Local Testing

### Basic Commands

```bash
# Run all tests
dotnet test

# Run tests with verbose output
dotnet test --verbosity detailed

# Run tests quietly (only summary)
dotnet test --verbosity quiet

# Run tests and keep previous results
dotnet test --no-build
```

### Targeted Testing

```bash
# Run specific test class
dotnet test --filter "AuthenticationServiceTests"

# Run specific test method
dotnet test --filter "FullyQualifiedName~ApiServiceTests.CallApiAsync_WithNullAccessToken_ThrowsArgumentException"

# Run tests matching a pattern
dotnet test --filter "Name~ServiceTests"

# Run tests by category (trait)
dotnet test --filter "Category=Unit"
```

### Code Coverage

```bash
# Generate coverage report
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover

# Generate coverage with exclusions
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover /p:Exclude="[HelloWorldApp.Tests]*"

# Generate multiple coverage formats
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura /p:CoverageFormat=opencover
```

### Performance Testing

```bash
# Run tests and measure time
dotnet test --logger "console;verbosity=minimal"

# Run single test class (faster)
dotnet test --filter "ApplicationTests"

# Parallel test execution
dotnet test --parallel
```

### Build Variants

```bash
# Debug build
dotnet test --configuration Debug

# Release build
dotnet test --configuration Release

# Clean build
dotnet clean && dotnet build && dotnet test

# No restore (assume packages downloaded)
dotnet test --no-restore
```

## Pre-commit Testing

### Setup/Teardown

```bash
# Install pre-commit hook
./.githooks/setup-hooks.sh

# Verify hook is installed
git config core.hooksPath

# Make hook executable
chmod +x .githooks/pre-commit
```

### Bypass Hooks (Not Recommended)

```bash
# Commit without running tests
git commit --no-verify

# Push without tests on remote
git push --no-verify
```

## CI/CD Commands

### Local Simulation

```bash
# Simulate GitHub Actions build
dotnet build --configuration Release

# Simulate GitHub Actions test
dotnet test --no-build --configuration Release

# Simulate coverage job
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### GitHub Actions

```bash
# View workflow status
gh workflow view build-and-test

# View latest run
gh run view --repo [owner]/[repo]

# Download artifacts
gh run download --repo [owner]/[repo]

# Re-run failed tests
gh run rerun --repo [owner]/[repo]
```

## Filtering Syntax

### Filter by Test Class

```bash
# Exact class name
--filter "ClassName=HelloWorldApp.Tests.Services.ApiServiceTests"

# Name contains
--filter "Name~ApiServiceTests"

# Full name contains
--filter "FullyQualifiedName~Services.ApiService"
```

### Filter by Test Method

```bash
# Method name contains
--filter "Name~WithNullAccessToken"

# Exact full name
--filter "FullyQualifiedName~CallApiAsync_WithNullAccessToken_ThrowsArgumentException"

# Multiple conditions
--filter "ClassName~Services&Name~Constructor"
```

### Combine Filters

```bash
# OR operator
--filter "TestClass=Class1|TestClass=Class2"

# AND operator
--filter "ClassName~Services&Name~async"

# Complex filter
--filter "(ClassName~Services&Name~Null)|(ClassName~Models)"
```

## Output Examples

### Test Results

```
Passed!  - Failed: 0, Passed: 40, Skipped: 0, Total: 40, Duration: 44 ms
```

### With Failures

```
Failed!  - Failed: 1, Passed: 39, Skipped: 0, Total: 40
FAILED HelloWorldApp.Tests.Services.ApiServiceTests.CallApiAsync_WithNullAccessToken_ThrowsArgumentException
```

### Coverage Report

```
Generating coverage report...
| Module | Lines | Branches | Methods | Types |
|--------|-------|----------|---------|-------|
| HelloWorldApp | 95% | 88% | 100% | 100% |
```

## Troubleshooting Commands

### Reset Test State

```bash
# Clean all build artifacts
dotnet clean

# Remove package cache
rm -rf ~/.nuget/packages

# Full restore
dotnet restore --no-cache
```

### Verify Environment

```bash
# Check .NET version
dotnet --version

# List installed SDKs
dotnet --list-sdks

# Check project references
dotnet list reference

# Check NuGet packages
dotnet list package

# Check outdated packages
dotnet list package --outdated
```

### Debug Tests

```bash
# Run with detailed diagnostics
dotnet test --diag logfile.txt

# Run single test with full output
dotnet test --filter "TestMethod" --logger "console;verbosity=detailed"

# Run with test adapter diagnostics
dotnet test --logger "console;verbosity=diagnostic"
```

## Useful Aliases

Add to your shell profile (~/.bashrc, ~/.zshrc, etc.):

```bash
# Test aliases
alias test='dotnet test'
alias test-debug='dotnet test --configuration Debug'
alias test-release='dotnet test --configuration Release'
alias test-coverage='dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover'
alias test-watch='dotnet watch test'
alias test-quiet='dotnet test --verbosity quiet'
alias test-verbose='dotnet test --verbosity detailed'

# Build aliases
alias build='dotnet build'
alias build-release='dotnet build --configuration Release'
alias clean='dotnet clean'

# Combined operations
alias rebuild='dotnet clean && dotnet build'
alias retest='dotnet clean && dotnet test'
alias rebuild-test='dotnet clean && dotnet build && dotnet test'
```

Usage:
```bash
# Use aliases instead
test                 # instead of dotnet test
test-coverage       # instead of dotnet test /p:CollectCoverage=true...
rebuild-test        # instead of full command chain
```

## Pro Tips

### Run Tests on File Changes
```bash
# Requires dotnet watch
dotnet watch test
```

### Parallel Execution
```bash
# Run multiple tests in parallel (faster)
dotnet test --parallel
```

### Fail on First Error
```bash
# Stop at first test failure
dotnet test --logger "console;verbosity=normal" -- RunConfiguration.StopOnFirstFailure=true
```

### Log Test Details
```bash
# Capture detailed test logs
dotnet test --logger "trx;LogFileName=test-results.trx"
dotnet test --logger "html;LogFileName=test-results.html"
```

### Filter by Traits
```bash
# If tests use [Trait] attributes
dotnet test --filter "Category=UnitTest"
dotnet test --filter "Priority=High"
```

## Common Issues & Solutions

### Issue: Tests fail locally but pass in CI
```bash
# Solution: Clean and rebuild
dotnet clean
dotnet build --configuration Release
dotnet test --no-build
```

### Issue: Pre-commit hook not running
```bash
# Solution: Reinstall hook
./.githooks/setup-hooks.sh
chmod +x .githooks/pre-commit
```

### Issue: Tests hang/timeout
```bash
# Solution: Add timeout parameter
dotnet test --logger "console" -- RunConfiguration.TestSessionTimeout=60000
```

### Issue: Coverage report not generated
```bash
# Solution: Install coverage tools
dotnet tool install -g dotnet-reportgenerator-globaltool
```

## Performance Benchmarks

Expected timings (on modern machine):

| Operation | Time |
|-----------|------|
| Full test run (40 tests) | ~44 ms |
| Single test class | ~5-10 ms |
| With coverage enabled | ~100-200 ms |
| Full build + test | ~15 seconds |
| Pre-commit hook (all) | ~1-2 seconds |
| GitHub Actions workflow | ~2-3 minutes |

## Resources

- [xUnit Documentation](https://xunit.net/docs/running-tests)
- [Moq Usage](https://github.com/moq/moq4/wiki/Quickstart)
- [dotnet test command](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-test)
- [Test Filtering](https://docs.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests)
