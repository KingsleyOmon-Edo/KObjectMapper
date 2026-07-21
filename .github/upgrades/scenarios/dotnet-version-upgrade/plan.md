# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade the solution from .NET 6 to .NET 8 LTS.
**Scope**: Two SDK-style projects, a class library and its xUnit test project, with a straightforward dependency graph and minimal breaking-change risk.

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 2 projects, both on .NET 6, a simple dependency structure, and low upgrade complexity.

## Tasks

### 01-upgrade-projects: Update project targets and test dependencies

This task updates the library and test project to target .NET 8, refreshes package references where needed, and resolves any compile or test issues that appear after the target framework change. The scope covers the main library project and the xUnit test project, including the deprecated xUnit package so the solution builds and tests successfully on the new target framework.

**Done when**: Both projects target `net8.0`, the solution restores successfully, the build succeeds with no errors, and the test project passes.
