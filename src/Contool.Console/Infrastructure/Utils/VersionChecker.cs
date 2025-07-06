﻿using System.Reflection;

namespace Contool.Console.Infrastructure.Utils;

public static class VersionChecker
{
    public static async Task GetLatestReleaseVersion()
    {
        try
        {
            var installedVersion = GetInstalledCliVersion();

            using var request = new HttpRequestMessage(HttpMethod.Get, AppInfo.ReleasePage);
            request.Headers.Add("Accept", "text/html");

            using var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });

            using var response = await client.SendAsync(request);

            using var content = response.Content;

            if (response.StatusCode != System.Net.HttpStatusCode.Found)
            {
                return;
            }

            var latestVersion = response.Headers.Location?.Segments.LastOrDefault();

            if (latestVersion is null)
            {
                return;
            }

            if (latestVersion.FirstOrDefault() == 'v')
            {
                latestVersion = latestVersion[1..]; // remove the 'v' prefix. equivalent to `latest.Substring(1, latest.Length - 1)`
            }

            var installedVersionNo = Convert.ToInt32(installedVersion.Replace(".", ""));

            var latestVersionNo = Convert.ToInt32(latestVersion.Replace(".", ""));

            if (installedVersionNo < latestVersionNo && installedVersionNo > 100)
            {
                /*
                var cw = new ConsoleWriter(AnsiConsole.Console);

                cw.WriteDim("");
                cw.WriteBlankLine();
                cw.WriteAlert($"This version of '{AppInfo.Name}' ({installedVersion}) is older than that of the latest version ({latestVersion}). Update the tool for the latest features and bug fixes:");
                cw.WriteBlankLine();
                cw.WriteAlertAccent($"dotnet tool update -g {AppInfo.Name}");
                cw.WriteBlankLine();
                */
            }
        }
        catch (Exception)
        {
            // fail silently
        }
    }

    public static string GetInstalledCliVersion()
    {
        var installedVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString();

        installedVersion = installedVersion[0..^2];

        return installedVersion;
    }
}