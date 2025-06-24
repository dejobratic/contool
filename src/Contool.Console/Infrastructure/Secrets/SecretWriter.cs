using Contentful.Core.Configuration;
using Contool.Core.Infrastructure.Extensions;
using Microsoft.AspNetCore.DataProtection;
using System.Text;

namespace Contool.Console.Infrastructure.Secrets;

public static class SecretWriter
{
    public static string GetSecretsFilePath()
    {
        var docPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var folder = Path.Combine(docPath, ".contool");


        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "secrets.enc");
    }

    public static void Save(ContentfulOptions secrets, IDataProtector protector)
    {
        var json = secrets.SerializeToJsonString();

        var bytes = Encoding.UTF8.GetBytes(json);

        var encrypted = protector.Protect(bytes);

        File.WriteAllBytes(GetSecretsFilePath(), encrypted);
    }

    public static void Clear()
    {
        var path = GetSecretsFilePath();

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}