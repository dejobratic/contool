using Spectre.Console;
using Spectre.Console.Rendering;

namespace Contool.Console.Infrastructure.UI.Models;

public sealed class ProgressBarRemainingTimeColumn : ProgressColumn
{
    public override IRenderable Render(RenderOptions options, ProgressTask task, TimeSpan deltaTime)
    {
        var remaining = task.RemainingTime;

        if (remaining is null || task.Value == task.MaxValue)
            return new Markup("");

        if (remaining.Value.TotalHours > 99)
            return new Markup("**:**:**");

        return new Text($@"{remaining.Value:hh\:mm\:ss}", Styles.Dim);
    }

    public override int? GetColumnWidth(RenderOptions options)
        => 8;
}