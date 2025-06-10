using Contool.Contentful.Models;

namespace Contool.Models;

internal class Content
{
    public ContentFieldName[] Headings { get; private set; } = [];

    private readonly List<dynamic> _rows = [];
    public IReadOnlyList<dynamic> Rows => _rows.AsReadOnly();

    public void SetHeadings(string[] headings)
    {
        if (headings is null || headings.Length == 0)
            throw new ArgumentException("Headings cannot be null or empty.", nameof(headings));

        Headings = [.. headings.Select(x => new ContentFieldName(x))];
    }

    public void AddRow(dynamic row)
    {
        if (row is null)
            throw new ArgumentNullException(nameof(row), "Row cannot be null.");

        _rows.Add(row);
    }
}