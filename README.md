# Contool

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()
[![License](https://img.shields.io/badge/license-MIT-blue.svg)]()

**Contool** is a powerful .NET CLI tool for managing Contentful content and content types across environments. It provides bulk operations, data migration capabilities, and automated workflows for Contentful CMS management.

## Table of Contents

- [Project Overview](#project-overview)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Getting Started](#getting-started)
- [Usage Guide](#usage-guide)
    - [Authentication Commands](#authentication-commands)
    - [Content Management Commands](#content-management-commands)
    - [Content Type Management Commands](#content-type-management-commands)
- [Configuration](#configuration)
- [Advanced Usage](#advanced-usage)
- [Troubleshooting](#troubleshooting)
- [Development](#development)
- [License](#license)

## Project Overview

Contool simplifies Contentful management by automating common operations that would otherwise require manual work through the Contentful web interface or custom API integrations. It's designed for content managers, developers, and DevOps teams who need to:

- **Migrate content** between environments (development → staging → production)
- **Bulk manage entries** (upload, download, publish, unpublish, delete)
- **Clone content types** with their entries across environments
- **Export/import data** in multiple formats (CSV, Excel, JSON)
- **Automate workflows** with dry-run capabilities and progress tracking

**Target Framework:** .NET 8.0

## Prerequisites

- **.NET 8.0 SDK** or later ([Download here](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Contentful account** with Management API access
- **Management API token** from your Contentful space

## Installation

### Option 1: Install as Global Tool (Recommended)

```bash
# Install from source (current method)
git clone https://github.com/yourusername/contool.git
cd contool

# Use the pack and install script (recommended for development)
./scripts/pack_and_install.sh        # On Linux/macOS
.\scripts\pack_and_install.ps1       # On Windows PowerShell

# Or manually:
dotnet pack src/Contool.Console/Contool.Console.csproj -c Release
dotnet tool install --global --add-source ./src/Contool.Console/bin/Release contool
```

### Option 2: Build from Source

```bash
# Clone the repository
git clone https://github.com/yourusername/contool.git
cd contool

# Build the solution
dotnet build

# Run directly
dotnet run --project src/Contool.Console -- [command] [options]
```

### Option 3: Install from NuGet (Coming Soon)

```bash
# Once published to NuGet
dotnet tool install --global contool
```

## Getting Started

1. **Configure your Contentful profile:**
   ```bash
   contool login
   ```
   You'll be prompted to enter your Management API token and select a space/environment.

2. **Verify your configuration:**
   ```bash
   contool info
   ```

3. **Start managing your content:**
   ```bash
   # Download entries to Excel
   contool content download -c blogPost -o ./exports -f EXCEL
   
   # Upload entries from CSV (dry run)
   contool content upload -c blogPost -i ./data.csv
   
   # Apply the upload
   contool content upload -c blogPost -i ./data.csv --apply
   ```

## Usage Guide

### Authentication Commands

#### `contool login`
Configure Contentful API credentials and default space/environment.

**Syntax:**
```bash
contool login [options]
```

**Options:**
- `--management-token <TOKEN>` - Management API token (optional, will prompt if not provided)
- `--delivery-token <TOKEN>` - Delivery API token (optional)
- `--preview-token <TOKEN>` - Preview API token (optional)
- `--space-id <ID>` - Contentful space ID
- `--environment-id <ID>` - Environment ID

**Examples:**
```bash
# Interactive login (recommended)
contool login

# Login with token
contool login --management-token CFPAT-abc123... --space-id myspace --environment-id production

# Login with all tokens
contool login \
  --management-token CFPAT-abc123... \
  --delivery-token abc123... \
  --preview-token abc123... \
  --space-id myspace \
  --environment-id production
```

**Expected Output:**
```
You are logged in.
```

#### `contool logout`
Remove stored Contentful credentials.

**Syntax:**
```bash
contool logout
```

**Example:**
```bash
contool logout
```

**Expected Output:**
```
You are logged out.
```

#### `contool info`
Display current Contentful space and environment information.

**Syntax:**
```bash
contool info [options]
```

**Options:**
- `--space-id <ID>` - Override default space
- `--environment-id <ID>` - Override default environment

**Example:**
```bash
contool info
```

**Expected Output:**
```
Contentful
  Space    : My Blog Space (production)
  Env      : production
  User     : john.doe@example.com (7H3jFvrKlLS3kt2vyGlZ7C)

Content Types (3)
  BlogPost             : blogPost (45)
  Author               : author (12)
  Category             : category (8)

Locales (2)
  English (United States) : en-US (default)
  Spanish (Spain)         : es-ES
```

### Content Management Commands

#### `contool content download`
Export content entries to files in various formats.

**Syntax:**
```bash
contool content download -c <CONTENT_TYPE> -o <OUTPUT_PATH> -f <FORMAT> [options]
```

**Required Options:**
- `-c, --content-type-id <ID>` - Content type to download
- `-o, --output-path <PATH>` - Output directory path
- `-f, --output-format <FORMAT>` - Output format (EXCEL, CSV, JSON)

**Optional Options:**
- `--space-id <ID>` - Override default space
- `--environment-id <ID>` - Override default environment

**Examples:**
```bash
# Download blog posts to Excel
contool content download -c blogPost -o ./exports -f EXCEL

# Download authors to CSV
contool content download -c author -o ./data -f CSV

# Download categories to JSON
contool content download -c category -o ./backup -f JSON

# Download from specific environment
contool content download -c blogPost -o ./staging-data -f CSV --environment-id development
```

**Expected Output:**
```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 100%   Downloading

Summary
  Total Processed : 45
  Success Rate    : 100.0%

Operations
  READ     : 45 succeeded 0 failed
```

#### `contool content upload`
Import content entries from files.

**Syntax:**
```bash
contool content upload -c <CONTENT_TYPE> -i <INPUT_PATH> [options]
```

**Required Options:**
- `-c, --content-type-id <ID>` - Target content type
- `-i, --input-path <PATH>` - Input file path

**Optional Options:**
- `--publish` - Upload entries as published (omit for draft)
- `--apply` - Apply changes (omit for dry run)
- `--space-id <ID>` - Override default space
- `--environment-id <ID>` - Override default environment

**Examples:**
```bash
# Dry run upload (safe preview)
contool content upload -c blogPost -i ./data.csv

# Upload and apply changes
contool content upload -c blogPost -i ./data.csv --apply

# Upload, apply, and publish
contool content upload -c blogPost -i ./data.xlsx --apply --publish

# Upload from JSON
contool content upload -c author -i ./authors.json --apply
```

**Expected Output (Dry Run):**
```
DRY RUN MODE - Use --apply|-a to execute operations.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 100%   Uploading

Summary
  Total Processed : 23
  Success Rate    : 100.0%

Operations
  Upload  : 18 succeeded 0 failed
  Upload  : 5 succeeded 0 failed
  Publish : 23 succeeded 0 failed
```

**Note:** Upload functionality is currently under development.

**Expected Output (Apply):**
```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 100%   Uploading  

Summary
  Total Processed : 23
  Success Rate    : 100.0%

Operations
  Upload  : 18 succeeded 0 failed
  Upload  : 5 succeeded 0 failed
  Publish : 23 succeeded 0 failed
```

#### `contool content delete`
Delete all entries of a specific content type.

**Syntax:**
```bash
contool content delete -c <CONTENT_TYPE> [options]
```

**Required Options:**
- `-c, --content-type-id <ID>` - Content type to delete

**Optional Options:**
- `--include-archived` - Include archived entries (omit to exclude)
- `--apply` - Apply changes (omit for dry run)
- `--space-id <ID>` - Override default space
- `--environment-id <ID>` - Override default environment

**Examples:**
```bash
# Dry run delete
contool content delete -c oldContentType

# Delete and apply changes
contool content delete -c oldContentType --apply

# Delete including archived entries
contool content delete -c oldContentType --include-archived --apply
```

**Expected Output (Dry Run):**
```
DRY RUN MODE - Use --apply|-a to execute operations.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 100%   Deleting  

Summary
  Total Processed : 45
  Success Rate    : 100.0%

Operations
  READ      : 45 succeeded 0 failed
  UNPUBLISH : 45 succeeded 0 failed
  DELETE    : 45 succeeded 0 failed
```

#### `contool content publish`
Publish all draft entries of a content type.

**Syntax:**
```bash
contool content publish -c <CONTENT_TYPE> [options]
```

**Required Options:**
- `-c, --content-type-id <ID>` - Content type to publish

**Optional Options:**
- `-a, --apply` - Apply changes (omit for dry run)
- `-s, --space-id <ID>` - Override default space
- `-e, --environment-id <ID>` - Override default environment

**Examples:**
```bash
# Dry run publish
contool content publish -c blogPost

# Publish and apply
contool content publish -c blogPost --apply
```

**Expected Output (Dry Run):**
```
DRY RUN MODE - Use --apply|-a to execute operations.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 100%   Publishing  

Summary
  Total Processed : 45
  Success Rate    : 100.0%

Operations
  READ     : 45 succeeded 0 failed

Profiling
  Execution Time    : 0h 0m 0s
  Peak Memory Usage : 16.47 MB
```

#### `contool content unpublish`
Unpublish all published entries of a content type.

**Syntax:**
```bash
contool content unpublish -c <CONTENT_TYPE> [options]
```

**Required Options:**
- `-c, --content-type-id <ID>` - Content type to unpublish

**Optional Options:**
- `-a, --apply` - Apply changes (omit for dry run)
- `-s, --space-id <ID>` - Override default space
- `-e, --environment-id <ID>` - Override default environment

**Examples:**
```bash
# Dry run unpublish
contool content unpublish -c draftContent

# Unpublish and apply
contool content unpublish -c draftContent --apply
```

**Expected Output (Dry Run):**
```
DRY RUN MODE - Use --apply|-a to execute operations.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 100%   Unpublishing  

Summary
  Total Processed : 45
  Success Rate    : 100.0%

Operations
  READ      : 45 succeeded 0 failed
  UNPUBLISH : 45 succeeded 0 failed

Profiling
  Execution Time    : 0h 0m 0s
  Peak Memory Usage : 16.28 MB
```

### Content Type Management Commands

#### `contool type clone`
Clone content type and entries between environments.

**Syntax:**
```bash
contool type clone -c <CONTENT_TYPE> -t <TARGET_ENVIRONMENT> [options]
```

**Required Options:**
- `-c, --content-type-id <ID>` - Content type to clone
- `-t, --target-environment-id <ID>` - Destination environment

**Optional Options:**
- `-p, --publish` - Publish cloned entries
- `-a, --apply` - Apply changes (omit for dry run)
- `-s, --space-id <ID>` - Override default space
- `-e, --environment-id <ID>` - Override source environment

**Examples:**
```bash
# Dry run clone from production to staging
contool type clone -c blogPost -t staging

# Clone and apply changes
contool type clone -c blogPost -t staging --apply

# Clone, apply, and publish
contool type clone -c blogPost -t production --publish --apply

# Clone from specific source environment
contool type clone -c blogPost -t production --environment-id development --apply
```

**Expected Output (Dry Run):**
```
DRY RUN MODE - Use --apply|-a to execute operations.
```

**Note:** This error occurs when the target environment doesn't exist. Ensure the target environment is created in Contentful before cloning.

#### `contool type delete`
Delete content type and all its entries.

**Syntax:**
```bash
contool type delete -c <CONTENT_TYPE> [options]
```

**Required Options:**
- `-c, --content-type <ID>` - Content type to delete

**Optional Options:**
- `--force` - Force deletion even with existing entries
- `--apply` - Apply changes (omit for dry run)
- `--space-id <ID>` - Override default space
- `--environment-id <ID>` - Override default environment

**Examples:**
```bash
# Dry run delete (will show validation errors if entries exist)
contool type delete -c oldContentType

# Force delete with all entries
contool type delete -c oldContentType --force --apply
```

**Expected Output (Dry Run):**
```
DRY RUN MODE - Use --apply|-a to execute operations.
```

**Force Delete Example:**
```bash
# Force delete content type with entries
contool type delete -c oldContentType --force --apply
```

## Configuration

### Credential Storage
Contool securely stores your API credentials using ASP.NET Core Data Protection:

- **Windows:** `%APPDATA%\Contool\`
- **macOS:** `~/.contool/`
- **Linux:** `~/.contool/`

Credentials are encrypted and tied to your user account for security.

### User Secrets (Development)
For development, you can use .NET User Secrets:

```bash
# Set user secrets
dotnet user-secrets set "Contentful:ManagementToken" "CFPAT-your-token-here" --project src/Contool.Console
dotnet user-secrets set "Contentful:SpaceId" "your-space-id" --project src/Contool.Console
dotnet user-secrets set "Contentful:EnvironmentId" "production" --project src/Contool.Console
```

### Environment Variables
You can also use environment variables:

```bash
export CONTENTFUL_MANAGEMENT_TOKEN="CFPAT-your-token-here"
export CONTENTFUL_SPACE_ID="your-space-id"
export CONTENTFUL_ENVIRONMENT_ID="production"
```

## Advanced Usage

### Dry Run Mode
All write operations support dry run mode for safe testing:

```bash
# Preview changes without applying them
contool content upload -c blogPost -i ./data.csv
contool content delete -c oldContent
contool type clone -c blogPost -t staging

# Apply changes after reviewing
contool content upload -c blogPost -i ./data.csv --apply
```

### Batch Processing
Contool automatically handles large datasets with efficient batch processing:

- **Upload/Download:** Processes in configurable batches
- **Delete operations:** Batches of 50 entries
- **Progress tracking:** Real-time progress bars with time estimates
- **Memory efficient:** Streams large files without loading everything into memory

### File Format Support
Support for multiple data formats:

#### CSV Format
```csv
id,title,slug,body,publishedAt
entry1,My First Post,my-first-post,"This is the content...",2024-01-15T10:30:00Z
entry2,Second Post,second-post,"More content here...",2024-01-16T14:22:00Z
```

#### Excel Format
- Supports `.xlsx` files
- First row contains field names
- Automatic data type detection
- Handles rich text and references

#### JSON Format
```json
[
  {
    "id": "entry1",
    "fields": {
      "title": {"en-US": "My First Post"},
      "slug": {"en-US": "my-first-post"},
      "body": {"en-US": "This is the content..."},
      "publishedAt": {"en-US": "2024-01-15T10:30:00Z"}
    }
  }
]
```

### Complex Workflows

#### Environment Migration
```bash
# Step 1: Download from production
contool content download -c blogPost -o ./migration --environment-id production -f JSON

# Step 2: Upload to staging
contool content upload -c blogPost -i ./migration/blogPost_*.json --environment-id staging --apply

# Step 3: Verify and publish
contool info --environment-id staging
contool content publish -c blogPost --environment-id staging --apply
```

#### Content Type Cloning with Selective Publishing
```bash
# Clone content type structure and entries
contool type clone -c blogPost -t staging --apply

# Selectively publish specific entries (manual process)
# Then bulk publish remaining drafts
contool content publish -c blogPost --environment-id staging --apply
```

## Troubleshooting

### Common Issues

#### Authentication Errors
```
Please log in before running this command. Run 'contool login' and follow the instructions.
```
**Solution:** Verify your token has the correct permissions and isn't expired.

```bash
# Re-authenticate
contool logout
contool login
```

#### Space/Environment Not Found
```
Please log in before running this command. Run 'contool login' and follow the instructions.
```
**Solution:** Check space ID and ensure your token has access to the space.

#### Content Type Validation Errors
```
Content type 'blogPost' not found in environment 'staging'
```
**Solution:** Verify the content type exists in the target environment.

```bash
# Check available content types
contool info --environment-id development
```

#### File Format Issues
```
Unable to parse CSV file - missing required columns
```
**Solution:** Ensure your CSV/Excel file has the correct column headers matching field IDs.

#### Rate Limiting
```
Rate limited - retrying in 5 seconds...
```
**Solution:** Contool automatically handles rate limiting with exponential backoff. For persistent issues, consider using smaller batch sizes.

### Getting Help

```bash
# Show available commands
contool --help

# Show command-specific help
contool content --help
contool content upload --help
```

### Debug Mode
Set environment variable for detailed logging:

```bash
export CONTOOL_DEBUG=true
contool [command]
```

## Development

### Building the Project

```bash
# Clone repository
git clone https://github.com/yourusername/contool.git
cd contool

# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Pack and install for local development
./scripts/pack_and_install.sh        # Linux/macOS
.\scripts\pack_and_install.ps1       # Windows PowerShell
```

### Project Structure

```
contool/
├── src/
│   ├── Contool.Console/          # CLI interface and commands
│   └── Contool.Core/             # Core business logic and services
├── tests/
│   └── Contool.Core.Tests.Unit/  # Unit tests
├── scripts/
│   ├── pack_and_install.sh       # Linux/macOS pack and install script
│   └── pack_and_install.ps1      # Windows PowerShell pack and install script
├── README.md
└── Contool.sln
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/Contool.Core.Tests.Unit/
```

### Contributing Guidelines

1. **Fork** the repository
2. **Create** a feature branch: `git checkout -b feature/amazing-feature`
3. **Write tests** for your changes
4. **Ensure** all tests pass: `dotnet test`
5. **Commit** your changes: `git commit -m 'Add amazing feature'`
6. **Push** to the branch: `git push origin feature/amazing-feature`
7. **Open** a Pull Request

### Code Style
- Follow C# coding conventions
- Use nullable reference types
- Ensure warnings are treated as errors
- Write comprehensive unit tests
- Document public APIs

### Architecture Notes
The project follows Clean Architecture principles:

- **Console Layer:** CLI commands and user interface
- **Core Layer:** Business logic, domain models, and services
- **Infrastructure:** External integrations (Contentful API, file I/O)

Key patterns used:
- Command Pattern for CLI operations
- Dependency Injection for IoC
- Decorator Pattern for cross-cutting concerns
- Factory Pattern for service creation

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## Contact & Support

- **GitHub Issues:** [Report bugs and request features](https://github.com/dejobratic/contool/issues)
- **Documentation:** [Full documentation](https://github.com/dejobratic/contool/wiki)
- **Contentful Community:** [Get help with Contentful](https://www.contentfulcommunity.com/)

---