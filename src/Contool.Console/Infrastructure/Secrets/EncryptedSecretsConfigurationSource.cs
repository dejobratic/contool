using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

namespace Contool.Console.Infrastructure.Secrets;

public class EncryptedSecretsConfigurationSource(
    string filePath,
    IDataProtector protector) : IConfigurationSource
{
    public string FilePath { get; } = filePath;

    public IDataProtector Protector { get; } = protector;

    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new EncryptedSecretsConfigurationProvider(FilePath, Protector);
}
