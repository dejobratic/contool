using Contentful.Core.Configuration;
using Contool.Core.Infrastructure.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Contool.Console.Infrastructure.Secrets;

public class EncryptedSecretsConfigurationProvider(
    string filePath,
    IDataProtector protector) : ConfigurationProvider
{
    public override void Load()
    {
        if (!File.Exists(filePath))
            return;

        try
        {
            var encrypted = File.ReadAllBytes(filePath);

            var decrypted = protector.Unprotect(encrypted);

            var json = Encoding.UTF8.GetString(decrypted);

            var dict = json.DeserializeFromJsonString<Dictionary<string, string>>() ?? [];

            foreach (var kv in dict) // TODO: this needs to be refactored
                Data[$"{nameof(ContentfulOptions)}:{kv.Key}"] = kv.Value;
        }
        catch
        {
            // Silently fail
        }
    }
}