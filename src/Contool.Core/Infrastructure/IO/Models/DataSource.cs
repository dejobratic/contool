﻿namespace Contool.Core.Infrastructure.IO.Models;

public abstract class DataSource(string name)
{
    public static readonly DataSource Csv = new FileDataSource("CSV", ".csv");
    public static readonly DataSource Json = new FileDataSource("JSON", ".json");
    public static readonly DataSource Excel = new FileDataSource("EXCEL", ".xlsx");
    //public static readonly DataSource Database = new DatabaseDataSource("Database");
    private static readonly DataSource[] All = [Csv, Json, Excel];

    public string Name { get; } = name;

    public static DataSource From(string value)
    {
        return All.FirstOrDefault(ds =>
            ds.Name.Equals(value, StringComparison.OrdinalIgnoreCase) ||
            ds is FileDataSource file && file.Extension.Equals(value, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"Unsupported data source: {value}");
    }

    public override string ToString() => Name;
}