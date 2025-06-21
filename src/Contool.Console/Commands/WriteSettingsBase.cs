using Spectre.Console.Cli;
using System.ComponentModel;

namespace Contool.Console.Commands;

public class WriteSettingsBase : SettingsBase
{
    [CommandOption("-a|--apply")]
    [Description("Whether to perform the operation (omit for dry run).")]
    public bool Apply { get; init; }
}