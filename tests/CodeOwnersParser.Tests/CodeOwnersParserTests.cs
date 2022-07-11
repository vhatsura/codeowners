using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace CodeOwnersParser.Tests;

public class CodeOwnersParserTests
{
    public static IEnumerable<object[]> CorrectData
    {
        get
        {
            yield return new object[]
            {
                "*       @global-owner1 @global-owner2",
                new List<(string Pattern, IEnumerable<string> Owners)>
                {
                    ("*", new[] { "@global-owner1", "@global-owner2" })
                }
            };

            yield return new object[]
            {
                "*.js    @js-owner",
                new List<(string Pattern, IEnumerable<string> Owners)> { ("*.js", new[] { "@js-owner" }) }
            };

            yield return new object[]
            {
                "*.go docs@example.com",
                new List<(string Pattern, IEnumerable<string> Owners)> { ("*.go", new[] { "docs@example.com" }) }
            };

            yield return new object[]
            {
                "*.txt @octo-org/octocats",
                new List<(string Pattern, IEnumerable<string> Owners)> { ("*.txt", new[] { "@octo-org/octocats" }) }
            };

            yield return new object[]
            {
                "/build/logs/ @doctocat",
                new List<(string Pattern, IEnumerable<string> Owners)> { ("/build/logs/", new[] { "@doctocat" }) }
            };

            yield return new object[]
            {
                "docs/*  docs@example.com",
                new List<(string Pattern, IEnumerable<string> Owners)> { ("docs/*", new[] { "docs@example.com" }) }
            };

            yield return new object[]
            {
                "apps/ @octocat",
                new List<(string Pattern, IEnumerable<string> Owners)> { ("apps/", new[] { "@octocat" }) }
            };

            yield return new object[]
            {
                "/scripts/ @doctocat @octocat",
                new List<(string Pattern, IEnumerable<string> Owners)>
                {
                    ("/scripts/", new[] { "@doctocat", "@octocat" })
                }
            };

            yield return new object[]
            {
                "/apps/github",
                new List<(string Pattern, IEnumerable<string> Owners)> { ("/apps/github", Array.Empty<string>()) }
            };
        }
    }

    [Theory]
    [MemberData(nameof(CorrectData))]
    public void Parse_ShouldParseContentCorrectly(string content,
        IEnumerable<(string Pattern, IEnumerable<string> Owners)> expectedResult)
    {
        // Arrange
        var parser = new CodeOwnersParser();

        // Act
        var result = parser.Parse(content);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void Parse_ShouldParseMultiLineContentCorrectly()
    {
        // Arrange
        var parser = new CodeOwnersParser();
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
        var result = parser.Parse(content);

        // Assert
        result.Should().BeEquivalentTo(new List<(string Pattern, IEnumerable<string> Owners)>
        {
            ("*", new List<string> { "@global-owner1", "@global-owner2" }),
            ("*.js", new List<string> { "@js-owner" }),
            ("*.go", new List<string> { "docs@example.com" }),
            ("*.txt", new List<string> { "@octo-org/octocats" }),
            ("/build/logs/", new List<string> { "@doctocat" }),
            ("docs/*", new List<string> { "docs@example.com" }),
            ("apps/", new List<string> { "@octocat" }),
            ("/docs/", new List<string> { "@doctocat" }),
            ("/scripts/", new List<string> { "@doctocat", "@octocat" }),
            ("/apps/", new List<string> { "@octocat" }),
            ("/apps/github", new List<string>())
        });
    }
}
