using Contool.Core.Infrastructure.Contentful.Utils;
using System.Text.RegularExpressions;

namespace Contool.Core.Tests.Unit.Infrastructure.Contentful.Utils;

public class ContentfulIdGeneratorTests
{
    [Fact]
    public void GivenNewId_WhenCalled_ThenIdStartsWithPrefix()
    {
        // Arrange
        const string expectedPrefix = "contool-";

        // Act
        var actual = ContentfulIdGenerator.NewId();

        // Assert
        Assert.StartsWith(expectedPrefix, actual);
    }

    [Fact]
    public void GivenNewId_WhenCalled_ThenIdHasCorrectLength()
    {
        // Arrange
        const int expectedLength = 30; // "contool-" (8 chars) + 22 chars

        // Act
        var actual = ContentfulIdGenerator.NewId();

        // Assert
        Assert.Equal(expectedLength, actual.Length);
    }

    [Fact]
    public void GivenNewId_WhenCalled_ThenIdContainsOnlyValidCharacters()
    {
        // Arrange
        const string validCharSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var regex = new Regex($"^[{Regex.Escape(validCharSet)}]+$");

        // Act
        var actual = ContentfulIdGenerator.NewId()[8..]; // Exclude "contool-"

        // Assert
        Assert.Matches(regex, actual);
    }

    [Fact]
    public void GivenNewId_WhenCalledMultipleTimes_ThenIdsAreUnique()
    {
        // Arrange
        const int numberOfIds = 1000;

        // Act
        var ids = Enumerable.Range(0, numberOfIds)
            .Select(_ => ContentfulIdGenerator.NewId())
            .ToList();

        // Assert
        Assert.Equal(numberOfIds, ids.Distinct().Count());
    }
}
