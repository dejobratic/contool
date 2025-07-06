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
- [Command Output Format](#command-output-format)
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
git clone https://github.com/dejobratic/contool.git
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
git clone https://github.com/dejobratic/contool.git
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

## Command Output Format

### Standard Output
All commands display their options and provide profiling information:

```
Command          : command name
Options                  
  --option-name : value
  --another-option : value

[Command output content]

Profiling
  Execution Time    : 0h 0m 1s
  Peak Memory Usage : 15.88 MB
```

### Dry Run Mode
All write commands (content: upload, delete, publish, unpublish and type: clone, delete) support dry run mode by default. To actually execute the operation, use the `--apply` or `-a` flag:

```bash
# Preview changes (safe)
contool content delete -c blogPost

# Execute the operation
contool content delete -c blogPost --apply
```

When in dry run mode, commands display:
```
DRY RUN MODE - Use --apply|-a to execute operations.
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
- `-s, --space-id <ID>` - Override default space
- `-e, --environment-id <ID>` - Override default environment

**Examples:**
```bash
# Download blog posts to Excel
contool content download -c blogPost -o ./exports -f EXCEL

# Download authors to CSV
contool content download -c author -o ./data -f CSV

# Download categories to JSON
contool content download -c category -o ./backup -f JSON

# Download from specific environment
contool content download -c blogPost -o ./staging-data -f CSV -e development
```

**Output:**
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
- `-p, --publish` - Upload entries as published (omit for draft)
- `-a, --apply` - Apply changes (omit for dry run)
- `-s, --space-id <ID>` - Override default space
- `-e, --environment-id <ID>` - Override default environment

**Examples:**
```bash
# Preview upload (dry run)
contool content upload -c blogPost -i ./data.csv

# Upload and apply changes
contool content upload -c blogPost -i ./data.csv -a

# Upload, apply, and publish
contool content upload -c blogPost -i ./data.xlsx -a -p

# Upload from JSON
contool content upload -c author -i ./authors.json -a
```

**Note:** Upload functionality is currently under development.

#### `contool content delete`
Delete all entries of a specific content type.

**Syntax:**
```bash
contool content delete -c <CONTENT_TYPE> [options]
```

**Required Options:**
- `-c, --content-type-id <ID>` - Content type to delete

**Optional Options:**
- `-i, --include-archived` - Include archived entries (omit to exclude)
- `-a, --apply` - Apply changes (omit for dry run)
- `-s, --space-id <ID>` - Override default space
- `-e, --environment-id <ID>` - Override default environment

**Examples:**
```bash
# Preview delete (dry run)
contool content delete -c blogPost

# Delete and apply changes
contool content delete -c blogPost -a

# Delete including archived entries
contool content delete -c blogPost -i -a
```

**Output:**
```
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
# Preview publish (dry run)
contool content publish -c blogPost

# Publish and apply
contool content publish -c blogPost -a
```

**Output:**
```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 100%   Publishing

Summary
  Total Processed : 45
  Success Rate    : 100.0%

Operations
  PUBLISH  : 45 succeeded 0 failed
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
# Preview unpublish (dry run)
contool content unpublish -c author

# Unpublish and apply
contool content unpublish -c author -a
```

**Output:**
```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 100%   Unpublishing

Summary
  Total Processed : 45
  Success Rate    : 100.0%

Operations
  READ      : 45 succeeded 0 failed
  UNPUBLISH : 45 succeeded 0 failed
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
contool type clone -c blogPost -t staging -a

# Clone, apply, and publish
contool type clone -c blogPost -t production -p -a

# Clone from specific source environment
contool type clone -c blogPost -t production -e development -a
```

**Output (when locales differ between environments):**
```
Error: Locales in source and target environments are not equivalent.
```

**Output (when content types differ between environments):**
```
Error: Content types 'category' in source and target environments are not equivalent.
```

**Output (successful cloning):**
```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 100%   Cloning

Summary
  Total Processed : 25
  Success Rate    : 100.0%

Operations
  CLONE : 25 succeeded 0 failed
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
- `-f, --force` - Force deletion even if content type contains entries
- `-a, --apply` - Apply changes (omit for dry run)
- `-s, --space-id <ID>` - Override default space
- `-e, --environment-id <ID>` - Override default environment

**Examples:**
```bash
# Preview delete (shows validation errors if entries exist)
contool type delete -c category

# Force delete with all entries
contool type delete -c category -f -a
```

**Output (when content type has entries):**
```
Error: Content type with ID 'category' cannot be deleted because it contains 
entries. Use the force option to delete it anyway.
```

**Output (successful deletion):**
```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 100%   Deleting

Summary
  Total Processed : 25
  Success Rate    : 100.0%

Operations
  DELETE : 25 succeeded 0 failed
```

## Configuration

### Credential Storage
Contool securely stores your API credentials using ASP.NET Core Data Protection:

- **Windows:** `%APPDATA%\Contool\`
- **macOS:** `~/.contool/`
- **Linux:** `~/.contool/`

Credentials are encrypted and tied to your user account for security.

## Advanced Usage

### Batch Processing
Contool automatically handles large datasets with efficient batch processing:

- **Upload/Download:** Processes in configurable batches
- **Delete operations:** Batches of 50 entries
- **Progress tracking:** Real-time progress bars with time estimates
- **Memory efficient:** Streams large files without loading everything into memory

### File Format Support
Contool supports multiple data formats with a specific field naming convention for localized content and system fields.

#### Field Naming Convention
- **Content fields**: `fieldId.locale` (e.g., `title.en-US`, `body.es-ES`)
- **Array fields**: `fieldId.locale[]` (e.g., `tags.en-US[]`, `categories.de-DE[]`)
- **System fields**: `sys.FieldName` (e.g., `sys.Id`, `sys.CreatedAt`)

#### System Fields (auto-populated on download)
All entries include Contentful system metadata:
- `sys.Id` - Entry ID
- `sys.Type` - Entry type ("Entry")
- `sys.ContentType` - Content type ID
- `sys.Space` - Space ID
- `sys.Environment` - Environment ID
- `sys.Version` - Entry version
- `sys.PublishedVersion` - Published version
- `sys.ArchivedVersion` - Archived version
- `sys.CreatedAt` - Creation timestamp
- `sys.UpdatedAt` - Last update timestamp
- `sys.PublishedAt` - Publication timestamp
- `sys.FirstPublishedAt` - First publication timestamp
- `sys.ArchivedAt` - Archival timestamp

#### CSV Format
```csv
sys.Id,title.en-US,slug.en-US,body.en-US,publishedAt.en-US,tags.en-US[],sys.CreatedAt
entry1,My First Post,my-first-post,"This is the content...",2024-01-15T10:30:00Z,"tech|blog|contentful",2024-01-10T08:30:00Z
entry2,Second Post,second-post,"More content here...",2024-01-16T14:22:00Z,"tutorial|guide",2024-01-12T10:15:00Z
```

**Array Handling**: Use pipe `|` or comma `,` to separate array values. For reference arrays, use the entry IDs.

#### Excel Format
- Supports `.xlsx` files with the same column naming convention
- First row must contain field names (e.g., `title.en-US`, `sys.Id`)
- Automatic data type detection based on Contentful field types
- Array fields use pipe `|` separation
- Handles rich text and references

#### JSON Format
```json
[
  {
    "sys.Id": "entry1",
    "sys.Type": "Entry", 
    "sys.ContentType": "blogPost",
    "sys.CreatedAt": "2024-01-10T08:30:00Z",
    "sys.UpdatedAt": "2024-01-15T10:30:00Z",
    "title.en-US": "My First Post",
    "slug.en-US": "my-first-post", 
    "body.en-US": "This is the content...",
    "publishedAt.en-US": "2024-01-15T10:30:00Z",
    "tags.en-US[]": "tech|blog|contentful"
  }
]
```

#### Supported Field Types
- **Symbol/Text**: Plain text values
- **RichText**: Rich text (stored as structured data)
- **Integer/Number**: Numeric values
- **Date**: ISO 8601 formatted dates
- **Boolean**: true/false values
- **Link**: References to other entries or assets (by ID)
- **Array**: Multiple values using pipe `|` or comma `,` separation
- **Object**: Complex structured data

### Complex Workflows

#### Environment Migration
```bash
# Step 1: Download from production
contool content download -c blogPost -o ./migration -e production -f JSON

# Step 2: Upload to staging
contool content upload -c blogPost -i ./migration/blogPost_*.json -e staging -a

# Step 3: Verify and publish
contool info -e staging
contool content publish -c blogPost -e staging -a
```

#### Content Type Cloning with Selective Publishing
```bash
# Clone content type structure and entries
contool type clone -c blogPost -t staging -a

# Selectively publish specific entries (manual process)
# Then bulk publish remaining drafts
contool content publish -c blogPost -e staging -a
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
contool info -e development
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

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/Contool.Core.Tests.Unit/
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## Contact & Support

- **GitHub Issues:** [Report bugs and request features](https://github.com/dejobratic/contool/issues)
- **Documentation:** [Full documentation](https://github.com/dejobratic/contool/wiki)
- **Contentful Community:** [Get help with Contentful](https://www.contentfulcommunity.com/)

---