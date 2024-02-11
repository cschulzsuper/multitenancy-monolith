using ChristianSchulz.MultitenancyMonolith.Shared.QuerySearch;
using System.Linq;
using Xunit;

namespace SearchQueryParserTests;

public class SearchQueryParserTests
{
    [Fact]
    public void Parse_ReturnsWord_IfSearchQueryHasWord()
    {
        // Arrange
        var searchQuery = "word";

        // Act
        var terms = SearchQueryParser.Parse(searchQuery)
            .ToList();

        // Assert
        Assert.Collection(terms,
            x =>
            {
                Assert.Equal(string.Empty, x.Key);
                Assert.Collection(x.Value.Order(),
                    y => Assert.Equal("word", y));
            });
    }

    [Fact]
    public void Parse_ReturnsWords_IfSearchQueryHasWords()
    {
        // Arrange
        var searchQuery = "word1 word2";

        // Act
        var terms = SearchQueryParser.Parse(searchQuery)
            .ToList();

        // Assert
        Assert.Collection(terms,
            x =>
            {
                Assert.Equal(string.Empty, x.Key);
                Assert.Collection(x.Value.Order(), 
                    y => Assert.Equal("word1", y),
                    y => Assert.Equal("word2", y));
            });
    }

    [Fact]
    public void Parse_ReturnsPhrase_IfSearchQueryHasPhrase()
    {
        // Arrange
        var searchQuery = "\"the phrase\"";

        // Act
        var terms = SearchQueryParser.Parse(searchQuery)
            .ToList();

        // Assert
        Assert.Collection(terms,
            x =>
            {
                Assert.Equal(string.Empty, x.Key);
                Assert.Collection(x.Value.Order(),
                    y => Assert.Equal("the phrase", y));
            });
    }

    [Fact]
    public void Parse_ReturnsPhrases_IfSearchQueryHasPhrases()
    {
        // Arrange
        var searchQuery = "\"the phrase1\" \"the phrase2\"";

        // Act
        var terms = SearchQueryParser.Parse(searchQuery)
            .ToList();

        // Assert
        Assert.Collection(terms,
            x =>
            {
                Assert.Equal(string.Empty, x.Key);
                Assert.Collection(x.Value.Order(),
                    y => Assert.Equal("the phrase1", y),
                    y => Assert.Equal("the phrase2", y));
            });
    }

    [Fact]
    public void Parse_ReturnsParameterWithWord_IfSearchQueryHasParameterWithWord()
    {
        // Arrange
        var searchQuery = "parameter:word";

        // Act
        var terms = SearchQueryParser.Parse(searchQuery)
            .ToList();

        // Assert
        Assert.Collection(terms,
            x =>
            {
                Assert.Equal("parameter", x.Key);
                Assert.Collection(x.Value.Order(),
                    y => Assert.Equal("word", y));
            });
    }

    [Fact]
    public void Parse_ReturnsParametersWithWords_IfSearchQueryHasParametersWithWords()
    {
        // Arrange
        var searchQuery = "parameter1:word1 parameter2:word2";

        // Act
        var terms = SearchQueryParser.Parse(searchQuery)
            .ToList();

        // Assert
        Assert.Collection(terms.OrderBy(x => x.Key),
            x =>
            {
                Assert.Equal("parameter1", x.Key);
                Assert.Collection(x.Value.Order(),
                    y => Assert.Equal("word1", y));
            },
            x =>
            {
                Assert.Equal("parameter2", x.Key);
                Assert.Collection(x.Value.Order(),
                    y => Assert.Equal("word2", y));
            });
    }

    [Fact]
    public void Parse_ReturnsParameterWithPhrase_IfSearchQueryHasParameterWithPhrase()
    {
        // Arrange
        var searchQuery = "parameter:\"the phrase\"";

        // Act
        var terms = SearchQueryParser.Parse(searchQuery)
            .ToList();

        // Assert
        Assert.Collection(terms,
            x =>
            {
                Assert.Equal("parameter", x.Key);
                Assert.Collection(x.Value.Order(),
                    y => Assert.Equal("the phrase", y));
            });
    }

    [Fact]
    public void Parse_ReturnsParameterWithMultipePhrases_IfSearchQueryHasParameterWithPhrasesMultipleTimes()
    {
        // Arrange
        var searchQuery = "parameter:\"the phrase1\" parameter:\"the phrase2\"";

        // Act
        var terms = SearchQueryParser.Parse(searchQuery)
            .ToList();

        // Assert
        Assert.Collection(terms.OrderBy(x => x.Key),
            x =>
            {
                Assert.Equal("parameter", x.Key);
                Assert.Collection(x.Value.Order(),
                    y => Assert.Equal("the phrase1", y),
                    y => Assert.Equal("the phrase2", y));
            });
    }

    [Fact]
    public void Parse_ReturnsParametersWithPhrases_IfSearchQueryHasParametersWithPhrases()
    {
        // Arrange
        var searchQuery = "parameter1:\"the phrase1\" parameter2:\"the phrase2\"";

        // Act
        var terms = SearchQueryParser.Parse(searchQuery)
            .ToList();

        // Assert
        Assert.Collection(terms.OrderBy(x => x.Key),
            x =>
            {
                Assert.Equal("parameter1", x.Key);
                Assert.Collection(x.Value.Order(),
                    y => Assert.Equal("the phrase1", y));
            }, 
            x =>
            {
                Assert.Equal("parameter2", x.Key);
                Assert.Collection(x.Value.Order(),
                    y => Assert.Equal("the phrase2", y));
            });
    }

    [Fact]
    public void Parse_ReturnsTerms_IfSearchQueryHasTerms()
    {
        // Arrange
        var searchQuery = "word \"the phrase1\" parameter:\"the phrase2\"";

        // Act
        var terms = SearchQueryParser.Parse(searchQuery)
            .ToList();

        // Assert
        Assert.Collection(terms.OrderBy(x => x.Key),
            x =>
            {
                Assert.Equal(string.Empty, x.Key);
                Assert.Collection(x.Value.Order(),
                    y => Assert.Equal("the phrase1", y),
                    y => Assert.Equal("word", y));
            },
            x =>
            {
                Assert.Equal("parameter", x.Key);
                Assert.Collection(x.Value.Order(),
                    y => Assert.Equal("the phrase2", y));
            });
    }
}