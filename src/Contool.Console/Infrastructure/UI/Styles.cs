using Spectre.Console;

namespace Contool.Console.Infrastructure.UI;

public static class Styles
{
    // Section Headings
    public static readonly Style Heading = new(Color.Chartreuse3, null, Decoration.Bold);
    public static readonly Style SubHeading = new(Color.SteelBlue3, null, Decoration.Bold);

    // Body Text
    public static readonly Style Normal = new(Color.Grey78);
    public static readonly Style Dim = new(Color.Grey39);

    // Alerts and Highlights
    public static readonly Style Alert = new(Color.Orange1);
    public static readonly Style AlertAccent = new(Color.LightGoldenrod2, null, Decoration.Bold);
    public static readonly Style Highlight = new(Color.LightGoldenrod3, null, Decoration.Italic);

    // Input Prompts
    public static readonly Style Input = new(Color.PaleTurquoise1, Color.Grey11);
}

