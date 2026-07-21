# 01-upgrade-projects: Update project targets and test dependencies

This task updates the library and test project to target .NET 8, refreshes package references where needed, and resolves any compile or test issues that appear after the target framework change. The scope covers the main library project and the xUnit test project, including the deprecated xUnit package so the solution builds and tests successfully on the new target framework.

**Done when**: Both projects target `net8.0`, the solution restores successfully, the build succeeds with no errors, and the test project passes.
