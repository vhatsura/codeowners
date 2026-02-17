using Shouldly;
using Snapshooter.Xunit3;
using Xunit;

namespace CodeOwners.Tests;

public class CodeOwnersSerializerTests
{
    public static IEnumerable<object[]> CorrectData
    {
        get
        {
            yield return [string.Empty, new List<CodeOwnersEntry>()];

            yield return
            [
                "*       @global-owner1 @global-owner2",
                new List<CodeOwnersEntry> { new("*", ["@global-owner1", "@global-owner2"]) }
            ];

            yield return
            [
                "*.js    @js-owner", new List<CodeOwnersEntry> { new("*.js", ["@js-owner"]) }
            ];

            yield return
            [
                "*.go docs@example.com", new List<CodeOwnersEntry> { new("*.go", ["docs@example.com"]) }
            ];

            yield return
            [
                "*.txt @octo-org/octocats",
                new List<CodeOwnersEntry> { new("*.txt", ["@octo-org/octocats"]) }
            ];

            yield return
            [
                "/build/logs/ @doctocat", new List<CodeOwnersEntry> { new("/build/logs/", ["@doctocat"]) }
            ];

            yield return
            [
                "docs/*  docs@example.com",
                new List<CodeOwnersEntry> { new("docs/*", ["docs@example.com"]) }
            ];

            yield return
            [
                "apps/ @octocat", new List<CodeOwnersEntry> { new("apps/", ["@octocat"]) }
            ];

            yield return
            [
                "/scripts/ @doctocat @octocat",
                new List<CodeOwnersEntry> { new("/scripts/", ["@doctocat", "@octocat"]) }
            ];

            yield return
            [
                "/apps/github", new List<CodeOwnersEntry> { new("/apps/github", Array.Empty<string>()) }
            ];
        }
    }

    [Theory]
    [MemberData(nameof(CorrectData))]
    public void DeserializeShouldDeserializeContentCorrectly(string content,
        IEnumerable<CodeOwnersEntry> expectedResult)
    {
        // Arrange + Act
        var result = CodeOwnersSerializer.Deserialize(content);

        // Assert
        result.ShouldBeEquivalentTo(expectedResult);
    }

    [Fact]
    public void DeserializeShouldDeserializeMultiLineContentCorrectly()
    {
        // Arrange
        var content = @"# This is a comment.
# Each line is a file pattern followed by one or more owners.

# These owners will be the default owners for everything in
# the repo. Unless a later match takes precedence,
# @global-owner1 and @global-owner2 will be requested for
# review when someone opens a pull request.
*       @global-owner1 @global-owner2

# Order is important; the last matching pattern takes the most
# precedence. When someone opens a pull request that only
# modifies JS files, only @js-owner and not the global
# owner(s) will be requested for a review.
*.js    @js-owner

# You can also use email addresses if you prefer. They'll be
# used to look up users just like we do for commit author
# emails.
*.go docs@example.com

# Teams can be specified as code owners as well. Teams should
# be identified in the format @org/team-name. Teams must have
# explicit write access to the repository. In this example,
# the octocats team in the octo-org organization owns all .txt files.
*.txt @octo-org/octocats

# In this example, @doctocat owns any files in the build/logs
# directory at the root of the repository and any of its
# subdirectories.
/build/logs/ @doctocat

# The `docs/*` pattern will match files like
# `docs/getting-started.md` but not further nested files like
# `docs/build-app/troubleshooting.md`.
docs/*  docs@example.com

# In this example, @octocat owns any file in an apps directory
# anywhere in your repository.
apps/ @octocat

# In this example, @doctocat owns any file in the `/docs`
# directory in the root of your repository and any of its
# subdirectories.
/docs/ @doctocat

# In this example, any change inside the `/scripts` directory
# will require approval from @doctocat or @octocat.
/scripts/ @doctocat @octocat

# In this example, @octocat owns any file in the `/apps`
# directory in the root of your repository except for the `/apps/github`
# subdirectory, as its owners are left empty.
/apps/ @octocat
/apps/github";

        // Act
        var result = CodeOwnersSerializer.Deserialize(content);

        // Assert
        result.ShouldBeEquivalentTo(new List<CodeOwnersEntry>
        {
            new("*", new List<string> { "@global-owner1", "@global-owner2" }),
            new("*.js", new List<string> { "@js-owner" }),
            new("*.go", new List<string> { "docs@example.com" }),
            new("*.txt", new List<string> { "@octo-org/octocats" }),
            new("/build/logs/", new List<string> { "@doctocat" }),
            new("docs/*", new List<string> { "docs@example.com" }),
            new("apps/", new List<string> { "@octocat" }),
            new("/docs/", new List<string> { "@doctocat" }),
            new("/scripts/", new List<string> { "@doctocat", "@octocat" }),
            new("/apps/", new List<string> { "@octocat" }),
            new("/apps/github", new List<string>())
        });
    }


    [Fact]
    public void SerializeShouldSerializeOneLineContentCorrectly()
    {
        // Arrange
        var codeOwners =
            new List<CodeOwnersEntry> { new("*", new List<string> { "@global-owner1", "@global-owner2" }) };

        // Act
        var result = CodeOwnersSerializer.Serialize(codeOwners);

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.MatchSnapshot();
    }

    [Fact]
    public void SerializeShouldSerializeMultiLineContentCorrectly()
    {
        // Arrange
        var codeOwners =
            new List<CodeOwnersEntry>
            {
                new("*", new List<string> { "@global-owner1", "@global-owner2" }),
                new("*.js", new List<string> { "@js-owner" }),
                new("*.go", new List<string> { "docs@example.com" }),
            };

        // Act
        var result = CodeOwnersSerializer.Serialize(codeOwners);

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.MatchSnapshot();
    }
}
