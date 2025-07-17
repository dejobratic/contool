using Contentful.Core.Models;
using Contentful.Core.Models.Management;

namespace Contool.Core.Tests.Unit.Helpers;

public class UserBuilder
{
    private string _id = "user-123";
    private string _firstName = "John";
    private string _lastName = "Doe";
    private string _email = "john.doe@example.com";
    private string _avatarUrl = "https://example.com/avatar.jpg";
    private bool _activated = true;
    private bool _confirmed = true;

    public UserBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public UserBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public UserBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserBuilder WithAvatarUrl(string avatarUrl)
    {
        _avatarUrl = avatarUrl;
        return this;
    }

    public UserBuilder WithActivated(bool activated)
    {
        _activated = activated;
        return this;
    }

    public UserBuilder WithConfirmed(bool confirmed)
    {
        _confirmed = confirmed;
        return this;
    }

    public User Build()
    {
        return new User
        {
            SystemProperties = new SystemProperties
            {
                Id = _id,
                Type = "User",
                Version = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            FirstName = _firstName,
            LastName = _lastName,
            Email = _email,
            AvatarUrl = _avatarUrl,
            Activated = _activated,
            Confirmed = _confirmed
        };
    }

    public static UserBuilder Create() => new();

    public static User CreateDefault() => new UserBuilder().Build();

    public static User CreateJohnDoe() => new UserBuilder().Build();

    public static User CreateJaneSmith() => 
        new UserBuilder()
            .WithId("user-456")
            .WithFirstName("Jane")
            .WithLastName("Smith")
            .WithEmail("jane.smith@example.com")
            .Build();

    public static User CreateBobJohnson() => 
        new UserBuilder()
            .WithId("user-789")
            .WithFirstName("Bob")
            .WithLastName("Johnson")
            .WithEmail("bob.johnson@example.com")
            .Build();

    public static List<User> CreateMultiple() =>
    [
        CreateJohnDoe(),
        CreateJaneSmith(),
        CreateBobJohnson()
    ];
}