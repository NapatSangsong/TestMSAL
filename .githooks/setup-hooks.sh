#!/bin/bash
# Setup script to install git hooks

echo "📦 Setting up git hooks..."

# Configure git to use the .githooks directory
git config core.hooksPath .githooks

# Make hooks executable
chmod +x .githooks/pre-commit

echo "✅ Git hooks installed successfully!"
echo ""
echo "Installed hooks:"
echo "  - pre-commit: Runs tests before each commit"
echo ""
echo "To disable a hook temporarily, use:"
echo "  git commit --no-verify"
