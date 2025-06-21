using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contool.Core.Infrastructure.Contentful.Utils;

namespace Contool.Core.Tests.Unit.Infrastructure.Contentful.Utils;

public class ContentTypeEqualityComparerTests
{
    private readonly ContentTypeEqualityComparer _sut = new();

    [Fact]
    public void GivenTwoIdenticalContentTypes_WhenCompared_ThenTheyAreEqual()
    {
        // Arrange
        var contentType1 = CreateContentType("BlogPost", "A blog post", "title", CreateFields("title", "body"));
        var contentType2 = CreateContentType("BlogPost", "A blog post", "title", CreateFields("title", "body"));

        // Act
        var actual = _sut.Equals(contentType1, contentType2);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void GivenContentTypesWithDifferentNames_WhenCompared_ThenTheyAreNotEqual()
    {
        // Arrange
        var contentType1 = CreateContentType("BlogPost", "A blog post", "title", CreateFields("title", "body"));
        var contentType2 = CreateContentType("Article", "A blog post", "title", CreateFields("title", "body"));

        // Act
        var actual = _sut.Equals(contentType1, contentType2);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void GivenContentTypesWithDifferentDescriptions_WhenCompared_ThenTheyAreNotEqual()
    {
        // Arrange
        var contentType1 = CreateContentType("BlogPost", "A blog post", "title", CreateFields("title", "body"));
        var contentType2 = CreateContentType("BlogPost", "An article", "title", CreateFields("title", "body"));

        // Act
        var actual = _sut.Equals(contentType1, contentType2);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void GivenContentTypesWithDifferentDisplayFields_WhenCompared_ThenTheyAreNotEqual()
    {
        // Arrange
        var contentType1 = CreateContentType("BlogPost", "A blog post", "title", CreateFields("title", "body"));
        var contentType2 = CreateContentType("BlogPost", "A blog post", "headline", CreateFields("title", "body"));

        // Act
        var actual = _sut.Equals(contentType1, contentType2);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void GivenContentTypesWithDifferentFields_WhenCompared_ThenTheyAreNotEqual()
    {
        // Arrange
        var contentType1 = CreateContentType("BlogPost", "A blog post", "title", CreateFields("title", "body"));
        var contentType2 = CreateContentType("BlogPost", "A blog post", "title", CreateFields("headline"));

        // Act
        var actual = _sut.Equals(contentType1, contentType2);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void GivenContentTypesWithDifferentFieldValidators_WhenCompared_ThenTheyAreNotEqual()
    {
        // Arrange
        var contentType1 = CreateContentType("BlogPost", "A blog post", "title", CreateFieldsWithValidators("title", "body"));
        var contentType2 = CreateContentType("BlogPost", "A blog post", "title", CreateFieldsWithValidators("title", "headline"));

        // Act
        var actual = _sut.Equals(contentType1, contentType2);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void GivenNullContentTypes_WhenCompared_ThenTheyAreEqual()
    {
        // Arrange
        ContentType? contentType1 = null;
        ContentType? contentType2 = null;

        // Act
        var actual = _sut.Equals(contentType1, contentType2);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void GivenOneNullContentType_WhenCompared_ThenTheyAreNotEqual()
    {
        // Arrange
        var contentType1 = CreateContentType("BlogPost", "A blog post", "title", CreateFields("title", "body"));
        ContentType? contentType2 = null;

        // Act
        var actual = _sut.Equals(contentType1, contentType2);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void GivenContentType_WhenGetHashCodeIsCalled_ThenHashCodeIsConsistent()
    {
        // Arrange
        var contentType = CreateContentType("BlogPost", "A blog post", "title", CreateFields("title", "body"));

        // Act
        var hashCode1 = _sut.GetHashCode(contentType);
        var hashCode2 = _sut.GetHashCode(contentType);

        // Assert
        Assert.Equal(hashCode1, hashCode2);
    }

    private static ContentType CreateContentType(string name, string description, string displayField, List<Field> fields)
    {
        return new ContentType
        {
            Name = name,
            Description = description,
            DisplayField = displayField,
            Fields = fields
        };
    }

    private static List<Field> CreateFields(params string[] fieldNames)
    {
        var fields = new List<Field>();
        foreach (var fieldName in fieldNames)
        {
            fields.Add(new Field
            {
                Id = fieldName,
                Name = fieldName,
                Type = "Text",
                LinkType = null,
                Required = true,
                Disabled = false,
                Omitted = false,
                Localized = false,
                Validations = new List<IFieldValidator>()
            });
        }
        return fields;
    }

    private static List<Field> CreateFieldsWithValidators(params string[] fieldNames)
    {
        var fields = new List<Field>();
        foreach (var fieldName in fieldNames)
        {
            fields.Add(new Field
            {
                Id = fieldName,
                Name = fieldName,
                Type = "Text",
                LinkType = null,
                Required = true,
                Disabled = false,
                Omitted = false,
                Localized = false,
                Validations =
                [
                    new RegexValidator(expression: "^[a-zA-Z0-9]*$", flags: null) // Example validator
                ]
            });
        }
        return fields;
    }
}