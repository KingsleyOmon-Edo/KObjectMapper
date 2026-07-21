# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NETCoreApp,Version=v8.0.

## Table of Contents

- [Executive Summary](#executive-Summary)
  - [Highlevel Metrics](#highlevel-metrics)
  - [Projects Compatibility](#projects-compatibility)
  - [Package Compatibility](#package-compatibility)
  - [API Compatibility](#api-compatibility)
  - [Binding Redirect Configuration](#binding-redirect-configuration)
- [Aggregate NuGet packages details](#aggregate-nuget-packages-details)
- [Top API Migration Challenges](#top-api-migration-challenges)
  - [Technologies and Features](#technologies-and-features)
  - [Most Frequent API Issues](#most-frequent-api-issues)
- [Projects Relationship Graph](#projects-relationship-graph)
- [Project Details](#project-details)

  - [src\KObjectMapper\KObjectMapper.csproj](#srckobjectmapperkobjectmappercsproj)
  - [tests\KObjectMapperTests\KObjectMapperTests.csproj](#testskobjectmappertestskobjectmappertestscsproj)


## Executive Summary

### Highlevel Metrics

| Metric | Count | Status |
| :--- | :---: | :--- |
| Total Projects | 2 | All require upgrade |
| Total NuGet Packages | 4 | 1 need upgrade |
| Total Code Files | 35 |  |
| Total Code Files with Incidents | 2 |  |
| Total Lines of Code | 2034 |  |
| Total Number of Issues | 3 |  |
| Estimated LOC to modify | 0+ | at least 0.0% of codebase |

### Projects Compatibility

| Project | Target Framework | Difficulty | Package Issues | API Issues | Binding Issues | Est. LOC Impact | Description |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| [src\KObjectMapper\KObjectMapper.csproj](#srckobjectmapperkobjectmappercsproj) | net6.0 | 🟢 Low | 0 | 0 | 0 |  | ClassLibrary, Sdk Style = True |
| [tests\KObjectMapperTests\KObjectMapperTests.csproj](#testskobjectmappertestskobjectmappertestscsproj) | net6.0 | 🟢 Low | 1 | 0 | 0 |  | DotNetCoreApp, Sdk Style = True |

### Package Compatibility

| Status | Count | Percentage |
| :--- | :---: | :---: |
| ✅ Compatible | 3 | 75.0% |
| ⚠️ Incompatible | 1 | 25.0% |
| 🔄 Upgrade Recommended | 0 | 0.0% |
| ***Total NuGet Packages*** | ***4*** | ***100%*** |

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 1394 |  |
| ***Total APIs Analyzed*** | ***1394*** |  |

## Aggregate NuGet packages details

| Package | Current Version | Suggested Version | Projects | Description |
| :--- | :---: | :---: | :--- | :--- |
| FluentAssertions | 6.8.0 |  | [KObjectMapperTests.csproj](#testskobjectmappertestskobjectmappertestscsproj) | ✅Compatible |
| Microsoft.NET.Test.Sdk | 17.1.0 |  | [KObjectMapperTests.csproj](#testskobjectmappertestskobjectmappertestscsproj) | ✅Compatible |
| xunit | 2.4.1 |  | [KObjectMapperTests.csproj](#testskobjectmappertestskobjectmappertestscsproj) | ⚠️NuGet package is deprecated |
| xunit.runner.visualstudio | 2.4.3 |  | [KObjectMapperTests.csproj](#testskobjectmappertestskobjectmappertestscsproj) | ✅Compatible |

## Top API Migration Challenges

### Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |

### Most Frequent API Issues

| API | Count | Percentage | Category |
| :--- | :---: | :---: | :--- |

## Projects Relationship Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>📦&nbsp;KObjectMapper.csproj</b><br/><small>net6.0</small>"]
    P2["<b>📦&nbsp;KObjectMapperTests.csproj</b><br/><small>net6.0</small>"]
    P2 --> P1
    click P1 "#srckobjectmapperkobjectmappercsproj"
    click P2 "#testskobjectmappertestskobjectmappertestscsproj"

```

## Project Details

<a id="srckobjectmapperkobjectmappercsproj"></a>
### src\KObjectMapper\KObjectMapper.csproj

#### Project Info

- **Current Target Framework:** net6.0
- **Proposed Target Framework:** net8.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 0
- **Dependants**: 1
- **Number of Files**: 10
- **Number of Files with Incidents**: 1
- **Lines of Code**: 747
- **Estimated LOC to modify**: 0+ (at least 0.0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (1)"]
        P2["<b>📦&nbsp;KObjectMapperTests.csproj</b><br/><small>net6.0</small>"]
        click P2 "#testskobjectmappertestskobjectmappertestscsproj"
    end
    subgraph current["KObjectMapper.csproj"]
        MAIN["<b>📦&nbsp;KObjectMapper.csproj</b><br/><small>net6.0</small>"]
        click MAIN "#srckobjectmapperkobjectmappercsproj"
    end
    P2 --> MAIN

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 307 |  |
| ***Total APIs Analyzed*** | ***307*** |  |

<a id="testskobjectmappertestskobjectmappertestscsproj"></a>
### tests\KObjectMapperTests\KObjectMapperTests.csproj

#### Project Info

- **Current Target Framework:** net6.0
- **Proposed Target Framework:** net8.0
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 1
- **Dependants**: 0
- **Number of Files**: 28
- **Number of Files with Incidents**: 1
- **Lines of Code**: 1287
- **Estimated LOC to modify**: 0+ (at least 0.0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["KObjectMapperTests.csproj"]
        MAIN["<b>📦&nbsp;KObjectMapperTests.csproj</b><br/><small>net6.0</small>"]
        click MAIN "#testskobjectmappertestskobjectmappertestscsproj"
    end
    subgraph downstream["Dependencies (1"]
        P1["<b>📦&nbsp;KObjectMapper.csproj</b><br/><small>net6.0</small>"]
        click P1 "#srckobjectmapperkobjectmappercsproj"
    end
    MAIN --> P1

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 1087 |  |
| ***Total APIs Analyzed*** | ***1087*** |  |

