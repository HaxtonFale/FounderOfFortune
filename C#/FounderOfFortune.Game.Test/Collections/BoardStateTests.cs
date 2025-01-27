using System.Collections.Immutable;
using FounderOfFortune.Game.Collections;
using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Test.Collections;

public class BoardStateTests
{
    private static ImmutableList<TableauStack> EmptyStacks =>
        Enumerable.Repeat(new TableauStack(), 11).ToImmutableList();
    private static BoardState EmptyBoard => new(EmptyStacks);
    public static TheoryData<Card> DefaultPromotions
    {
        get
        {
            var data = new TheoryData<Card>
            {
                new MajorArcana(0),
                new MajorArcana(21),
                new MinorArcana(Suit.Coins, 2),
                new MinorArcana(Suit.Goblets, 2),
                new MinorArcana(Suit.Swords, 2),
                new MinorArcana(Suit.Wands, 2)
            };
            return data;
        }
    }

    public static TheoryData<int> StackNumbers
    {
        get
        {
            var data = new TheoryData<int>();
            for (var i = 0; i < 11; i++)
            {
                data.Add(i);
            }

            return data;
        }
    }

    private static TableauStack SourceStack =>
        new(new CardSequence(new MajorArcana(5), 5).Concat(new CardSequence(new MinorArcana(Suit.Goblets, 5),
            7)).ToImmutableList());

    private static TableauStack InvalidDestinationStack =>
        new(new CardSequence(new MinorArcana(Suit.Swords, 10),
            8).ToImmutableList());

    private static TableauStack ValidDestinationStack =>
        new(new CardSequence(new MinorArcana(Suit.Goblets, 10),
            8).ToImmutableList());

    [Theory]
    [MemberData(nameof(DefaultPromotions))]
    public void BoardPromotesAutomatically(Card card)
    {
        // Arrange
        var initialStack = new TableauStack().PlaceCard(card);
        var stacks = Enumerable.Repeat(new TableauStack(), 10).Prepend(initialStack).ToImmutableList();

        // Act
        var board = new BoardState(stacks);

        // Assert
        board.TableauStacks.Should().AllSatisfy(s => s.Cards.Should().BeEmpty());
        (card is MajorArcana).Should()
            .Imply(board.MajorArcanaStacks.Left == card as MajorArcana || board.MajorArcanaStacks.Right == card as MajorArcana);
        (card is MinorArcana).Should().Imply(board.MinorArcanaStacks.Stacks.Select(s =>s.TopCard).Contains(card));
    }

    [Fact]
    public void PromotionStartsFromFreeCell()
    {
        // Arrange
        var majorStacks = new MajorArcanaStacks(new MajorArcana(11), new MajorArcana(14));
        var initialStack = new TableauStack().PlaceCard(new MajorArcana(13));
        var stacks = Enumerable.Repeat(new TableauStack(), 10).Prepend(initialStack).ToImmutableList();

        // Act
        var board = new BoardState(majorStacks, new MinorArcanaStacks(), new MajorArcana(12), stacks);

        // Assert
        board.FreeCell.Should().BeNull("the card in the free cell should have been promoted");
        board.TableauStacks.Should().AllSatisfy(s => s.Cards.Should().BeEmpty());
        board.MajorArcanaStacks.Should()
            .BeEquivalentTo(new MajorArcanaStacks(new MajorArcana(13), new MajorArcana(13)),
                "XIII from the tableau was promoted last so it is on top of the stacks");
    }

    [Theory]
    [InlineData(10)]
    [InlineData(12)]
    public void InvalidInitialTableauStackCountThrows(int count)
    {
        // Arrange
        var stacks = Enumerable.Repeat(new TableauStack(), count).ToImmutableList();
        var act = () => new BoardState(stacks);

        // Act & Assert
        var ex = act.Should().Throw<ArgumentException>().Which;
        ex.Message.Should().StartWith("Must have exactly 11 tableau stacks");
        ex.ParamName.Should().Be("tableauStacks");
    }

    #region MoveSingleCard

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public void InvalidSingleCardMoveSourceThrows(int source)
    {
        // Arrange
        var act = () => EmptyBoard.MoveSingleCard(source, 0);

        // Act & Assert
        var ex = act.Should().Throw<ArgumentOutOfRangeException>().Which;
        ex.Message.Should().StartWith("Source stack must be between 0 and 10");
        ex.ParamName.Should().Be(nameof(source));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public void InvalidSingleCardMoveDestinationThrows(int destination)
    {
        // Arrange
        var act = () => EmptyBoard.MoveSingleCard(0, destination);

        // Act & Assert
        var ex = act.Should().Throw<ArgumentOutOfRangeException>().Which;
        ex.Message.Should().StartWith("Destination stack must be between 0 and 10");
        ex.ParamName.Should().Be(nameof(destination));
    }

    [Theory]
    [MemberData(nameof(StackNumbers))]
    public void MovingSingleCardFromEmptyStackThrows(int source)
    {
        // Arrange
        var destination = 10 - source;
        var act = () => EmptyBoard.MoveSingleCard(source, destination);

        // Act & Assert
        act.Should().Throw<InvalidOperationException>().Which.Message.Should()
            .StartWith("Cannot move cards from an empty stack");
    }

    [Fact]
    public void InvalidSingleCardMoveThrows()
    {
        // Arrange
        var stacks = Enumerable.Repeat(new TableauStack(), 9).Append(SourceStack).Append(InvalidDestinationStack).ToImmutableList();
        var board = new BoardState(stacks);
        var act = () => board.MoveSingleCard(9, 10);

        // Act & Assert
        act.Should().Throw<InvalidOperationException>().Which.Message.Should()
            .StartWith("Cannot move cards from stack 9 to stack 10");
    }

    [Fact]
    public void MovesSingleCardCorrectly()
    {
        // Arrange
        var stacks = Enumerable.Repeat(new TableauStack(), 9).Append(SourceStack).Append(ValidDestinationStack).ToImmutableList();
        var board = new BoardState(stacks);

        // Act
        var newBoard = board.MoveSingleCard(9, 10);

        // Assert
        newBoard.TableauStacks[9].TopCard.Should().Be(new MinorArcana(Suit.Goblets, 6));
        newBoard.TableauStacks[10].TopCard.Should().Be(new MinorArcana(Suit.Goblets, 7));
    }

    #endregion

    #region MoveCardSequence

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public void InvalidCardSequenceSourceThrows(int source)
    {
        // Arrange
        var stacks = Enumerable.Repeat(new TableauStack(), 11).ToImmutableList();
        var board = new BoardState(stacks);
        var act = () => board.MoveCardSequence(source, 0);

        // Act & Assert
        var ex = act.Should().Throw<ArgumentOutOfRangeException>().Which;
        ex.Message.Should().StartWith("Source stack must be between 0 and 10");
        ex.ParamName.Should().Be(nameof(source));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public void InvalidCardSequenceDestinationThrows(int destination)
    {
        // Arrange
        var stacks = Enumerable.Repeat(new TableauStack(), 11).ToImmutableList();
        var board = new BoardState(stacks);
        var act = () => board.MoveCardSequence(0, destination);

        // Act & Assert
        var ex = act.Should().Throw<ArgumentOutOfRangeException>().Which;
        ex.Message.Should().StartWith("Destination stack must be between 0 and 10");
        ex.ParamName.Should().Be(nameof(destination));
    }

    [Theory]
    [MemberData(nameof(StackNumbers))]
    public void MovingCardSequenceFromEmptyStackThrows(int source)
    {
        // Arrange
        var destination = 10 - source;
        var act = () => EmptyBoard.MoveCardSequence(source, destination);

        // Act & Assert
        act.Should().Throw<InvalidOperationException>().Which.Message.Should()
            .StartWith("Cannot move cards from an empty stack");
    }

    [Fact]
    public void InvalidCardSequenceMoveThrows()
    {
        // Arrange
        var stacks = Enumerable.Repeat(new TableauStack(), 9).Append(SourceStack).Append(InvalidDestinationStack).ToImmutableList();
        var board = new BoardState(stacks);
        var act = () => board.MoveCardSequence(9, 10);

        // Act & Assert
        act.Should().Throw<InvalidOperationException>().Which.Message.Should()
            .StartWith("Cannot move cards from stack 9 to stack 10");
    }

    [Fact]
    public void MovesCardSequenceCorrectly()
    {
        // Arrange
        var stacks = Enumerable.Repeat(new TableauStack(), 9).Append(SourceStack).Append(ValidDestinationStack).ToImmutableList();
        var board = new BoardState(stacks);

        // Act
        var newBoard = board.MoveCardSequence(9, 10);

        // Assert
        newBoard.TableauStacks[9].TopCard.Should().Be(new MajorArcana(5));
        newBoard.TableauStacks[10].TopCard.Should().Be(new MinorArcana(Suit.Goblets, 5));
    }

    #endregion

    #region StoreCard

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public void InvalidStoreCardSourceThrows(int source)
    {
        // Arrange
        var act = () => EmptyBoard.StoreCard(source);

        // Act & Assert
        var ex = act.Should().Throw<ArgumentOutOfRangeException>().Which;
        ex.Message.Should().StartWith("Source stack must be between 0 and 10");
        ex.ParamName.Should().Be(nameof(source));
    }

    [Fact]
    public void StoringCardWhenFreeCellOccupiedThrows()
    {
        // Arrange
        var board = new BoardState(new MajorArcanaStacks(), new MinorArcanaStacks(), new MajorArcana(3), EmptyStacks);
        var act = () => board.StoreCard(3);

        // Act & Assert
        act.Should().Throw<InvalidOperationException>().Which.Message.Should().StartWith("Cannot store a card when free cell already occupied");
    }

    [Fact]
    public void StoresCardCorrectly()
    {
        // Arrange
        var card = new MajorArcana(5);
        const int source = 3;
        var stack = new TableauStack(ImmutableList<Card>.Empty.Add(card));
        var allStacks = EmptyStacks.SetItem(source, stack);
        var board = new BoardState(allStacks);

        // Act
        var newBoard = board.StoreCard(source);

        // Assert
        newBoard.FreeCell.Should().Be(card);
        newBoard.TableauStacks[source].TopCard.Should().NotBe(card);
    }

    #endregion

    #region RetrieveCard

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public void InvalidRetrieveCardDestinationThrows(int destination)
    {
        // Arrange
        var act = () => EmptyBoard.RetrieveCard(destination);

        // Act & Assert
        var ex = act.Should().Throw<ArgumentOutOfRangeException>().Which;
        ex.Message.Should().StartWith("Destination stack must be between 0 and 10");
        ex.ParamName.Should().Be(nameof(destination));
    }

    [Fact]
    public void RetrievingFromEmptyFreeCellThrows()
    {
        // Arrange
        var act = () => EmptyBoard.RetrieveCard(3);

        // Act & Assert
        act.Should().Throw<InvalidOperationException>().Which.Message.Should().StartWith("Cannot retrieve a card when free cell is empty");
    }

    [Fact]
    public void RetrievingCardToInvalidStackThrows()
    {
        // Arrange
        var storedCard = new MinorArcana(Suit.Coins, 5);
        var stackCard = new MinorArcana(Suit.Swords, 4);
        const int destination = 3;
        var stack = new TableauStack(ImmutableList<Card>.Empty.Add(stackCard));
        var allStacks = EmptyStacks.SetItem(destination, stack);
        var board = new BoardState(new MajorArcanaStacks(), new MinorArcanaStacks(), storedCard, allStacks);
        var act = () => board.RetrieveCard(destination);

        // Act & Assert
        act.Should().Throw<InvalidOperationException>().Which.Message.Should().StartWith($"Cannot place a card on top of stack {destination}");
    }

    [Fact]
    public void RetrievesOntoEmptyStack()
    {
        // Arrange
        var card = new MinorArcana(Suit.Coins, 5);
        const int destination = 3;
        var board = new BoardState(new MajorArcanaStacks(), new MinorArcanaStacks(), card, EmptyStacks);

        // Act
        var newBoard = board.RetrieveCard(destination);

        // Assert
        newBoard.FreeCell.Should().BeNull();
        newBoard.TableauStacks[destination].TopCard.Should().Be(card);
    }

    [Fact]
    public void RetrievesOntoValidStack()
    {
        // Arrange
        var storedCard = new MinorArcana(Suit.Coins, 5);
        var stackCard = new MinorArcana(Suit.Coins, 4);
        const int destination = 3;
        var stack = new TableauStack(ImmutableList<Card>.Empty.Add(stackCard));
        var allStacks = EmptyStacks.SetItem(destination, stack);
        var board = new BoardState(new MajorArcanaStacks(), new MinorArcanaStacks(), storedCard, allStacks);

        // Act
        var newBoard = board.RetrieveCard(destination);

        // Assert
        newBoard.FreeCell.Should().BeNull();
        newBoard.TableauStacks[destination].TopCard.Should().Be(storedCard);
    }

    #endregion
}