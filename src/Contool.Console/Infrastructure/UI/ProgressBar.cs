using Spectre.Console;

namespace Contool.Console.Infrastructure.UI;

internal static class ProgressBar
{
    public static Progress GetInstance()
    {
        return AnsiConsole.Progress()
            .AutoRefresh(true)
            .AutoClear(false)
            .HideCompleted(false)
            .Columns(
            [
                new ProgressBarColumn
                {
                    CompletedStyle = Styles.Alert,
                    FinishedStyle = Styles.Alert,
                    IndeterminateStyle = Styles.Dim,
                    RemainingStyle = Styles.Dim,
                },
                new PercentageColumn
                {
                    CompletedStyle = Styles.Alert,
                    Style = Styles.Normal,
                },
                new SpinnerColumn
                {
                    Style = Styles.AlertAccent,
                    //CompletedText = "✔",
                },
                new TaskDescriptionColumn
                {
                    Alignment = Justify.Left,
                },
                new ProgressBarRemainingTimeColumn(),
            ]);
    }
}