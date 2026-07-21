# .NET Version Upgrade

## Strategy
**Selected**: All-At-Once
**Rationale**: 2 projects, both on .NET 6, a simple dependency structure, and low upgrade complexity.

### Execution Constraints
- Update the library and test project together in a single upgrade pass.
- Validate restore, build, and tests after the framework and package updates complete.
- Keep the change focused on the .NET 8 target framework bump and package compatibility fixes.

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net8.0
- **Commit Strategy**: Single Commit at End

## Source Control
- **Source Branch**: main
- **Working Branch**: upgrade-dotnet-8
- **Branch Sync**: Auto (Merge)
