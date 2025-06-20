using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Utils;

namespace Contool.Core.Tests.Unit.Infrastructure.Contentful.Utils;

public class LocaleEqualityComparerTests
{
    private readonly LocaleEqualityComparer _sut = new();

    [Fact]
    public void GivenTwoIdenticalLocales_WhenCompared_ThenTheyAreEqual()
    {
        // Arrange
        var locale1 = CreateLocale();
        var locale2 = CreateLocale();

        // Act
        var actual = _sut.Equals(locale1, locale2);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void GivenLocalesWithDifferentCodes_WhenCompared_ThenTheyAreNotEqual()
    {
        // Arrange
        var locale1 = CreateLocale(code: "en-US");
        var locale2 = CreateLocale(code: "fr-FR");

        // Act
        var actual = _sut.Equals(locale1, locale2);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void GivenLocalesWithDifferentNames_WhenCompared_ThenTheyAreNotEqual()
    {
        // Arrange
        var locale1 = CreateLocale(name: "English (United States)");
        var locale2 = CreateLocale(name: "French (France)");

        // Act
        var actual = _sut.Equals(locale1, locale2);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void GivenLocalesWithDifferentFallbackCodes_WhenCompared_ThenTheyAreNotEqual()
    {
        // Arrange
        var locale1 = CreateLocale(fallbackCode: "en");
        var locale2 = CreateLocale(fallbackCode: "fr");

        // Act
        var actual = _sut.Equals(locale1, locale2);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void GivenLocalesWithDifferentDefaultValues_WhenCompared_ThenTheyAreNotEqual()
    {
        // Arrange
        var locale1 = CreateLocale(isDefault: true);
        var locale2 = CreateLocale(isDefault: false);

        // Act
        var actual = _sut.Equals(locale1, locale2);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void GivenLocalesWithDifferentOptionalValues_WhenCompared_ThenTheyAreNotEqual()
    {
        // Arrange
        var locale1 = CreateLocale(isOptional: true);
        var locale2 = CreateLocale(isOptional: false);

        // Act
        var actual = _sut.Equals(locale1, locale2);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void GivenNullLocales_WhenCompared_ThenTheyAreEqual()
    {
        // Arrange
        Locale? locale1 = null;
        Locale? locale2 = null;

        // Act
        var actual = _sut.Equals(locale1, locale2);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void GivenOneNullLocale_WhenCompared_ThenTheyAreNotEqual()
    {
        // Arrange
        var locale1 = CreateLocale();

        Locale? locale2 = null;

        // Act
        var actual = _sut.Equals(locale1, locale2);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void GivenLocale_WhenGetHashCodeIsCalled_ThenHashCodeIsConsistent()
    {
        // Arrange
        var locale = CreateLocale();

        // Act
        var hashCode1 = _sut.GetHashCode(locale);
        var hashCode2 = _sut.GetHashCode(locale);

        // Assert
        Assert.Equal(hashCode1, hashCode2);
    }

    private static Locale CreateLocale(
        string code = "en-US",
        string name = "English (United States)",
        string fallbackCode = "en",
        bool isDefault = true,
        bool isOptional = false)
    {
        return new Locale
        {
            Code = code,
            Name = name,
            FallbackCode = fallbackCode,
            Default = isDefault,
            Optional = isOptional
        };
    }
}
