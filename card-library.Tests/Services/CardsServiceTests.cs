using Moq;
using card_library.Core.Application.Models;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Application.Repository.Contracts;
using card_library.Core.Application.Services;
using card_library.Core.Application.Services.Contracts;

namespace card_library.Tests.Services;

public class CardsServiceTests
{
    private readonly Mock<ICardRepository> _cardRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IFileStorageService> _fileStorageMock;
    private readonly CardsService _sut;

    public CardsServiceTests()
    {
        _cardRepoMock = new Mock<ICardRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _fileStorageMock = new Mock<IFileStorageService>();

        _sut = new CardsService(
            _cardRepoMock.Object,
            _unitOfWorkMock.Object,
            _fileStorageMock.Object
        );
    }

    // ── GetCardById ─────────────────────────────────────────────

    [Fact]
    public async Task GetCardById_WhenCardExists_ReturnsOkWithCardResponse()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var card = new Card
        {
            Id = cardId,
            Name = "Fire Dragon",
            HexCardColor = "FF0000",
            ImageRefUrl = "cards/dragon.png"
        };

        _cardRepoMock
            .Setup(r => r.GetById(cardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(card);

        _fileStorageMock
            .Setup(f => f.GetPresignedUrlAsync("cards/dragon.png", It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://s3.example.com/presigned-url");

        // Act
        var result = await _sut.GetCardById(cardId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(cardId, result.Value.CardId);
        Assert.Equal("Fire Dragon", result.Value.CardName);
        Assert.Equal("FF0000", result.Value.HexColor);
        Assert.Equal("https://s3.example.com/presigned-url", result.Value.PublicImgUrl);
    }

    [Fact]
    public async Task GetCardById_WhenCardNotFound_ReturnsFailResult()
    {
        // Arrange
        _cardRepoMock
            .Setup(r => r.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Card?)null);

        // Act
        var result = await _sut.GetCardById(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Failed to get card", result.Error);
    }

    [Fact]
    public async Task GetCardById_WhenImageRefUrlIsEmpty_ReturnsEmptyPublicImgUrl()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var card = new Card
        {
            Id = cardId,
            Name = "Text Only Card",
            ImageRefUrl = string.Empty
        };

        _cardRepoMock
            .Setup(r => r.GetById(cardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(card);

        // Act
        var result = await _sut.GetCardById(cardId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(string.Empty, result.Value!.PublicImgUrl);
        _fileStorageMock.Verify(
            f => f.GetPresignedUrlAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ── GetCardsByName ──────────────────────────────────────────

    [Fact]
    public async Task GetCardsByName_WhenCardsExist_ReturnsOkWithList()
    {
        // Arrange
        var cards = new List<Card>
        {
            new Card { Id = Guid.NewGuid(), Name = "Goblin", ImageRefUrl = "" },
            new Card { Id = Guid.NewGuid(), Name = "Goblin", ImageRefUrl = "" }
        };

        _cardRepoMock
            .Setup(r => r.GetListByName("Goblin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cards);

        // Act
        var result = await _sut.GetCardsByName("Goblin", CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Value!.Count);
    }

    [Fact]
    public async Task GetCardsByName_WhenRepoReturnsNull_ReturnsFailResult()
    {
        // Arrange
        _cardRepoMock
            .Setup(r => r.GetListByName(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<Card>?)null!);

        // Act
        var result = await _sut.GetCardsByName("Missing", CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Failed to get card", result.Error);
    }

    // ── UploadImage ─────────────────────────────────────────────

    [Fact]
    public async Task UploadImage_WhenCardExists_SavesKeyAndReturnsOk()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var card = new Card { Id = cardId, Name = "Dragon" };

        _cardRepoMock
            .Setup(r => r.GetById(cardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(card);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.UploadImage(cardId, "cards/new-image.png", CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("cards/new-image.png", result.Value);
        Assert.Equal("cards/new-image.png", card.ImageRefUrl);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadImage_WhenCardNotFound_ReturnsFailResult()
    {
        // Arrange
        _cardRepoMock
            .Setup(r => r.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Card?)null);

        // Act
        var result = await _sut.UploadImage(Guid.NewGuid(), "cards/img.png", CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Card not found.", result.Error);
    }

    // ── DeleteImage ─────────────────────────────────────────────

    [Fact]
    public async Task DeleteImage_WhenCardHasImage_ClearsUrlAndReturnsKey()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var card = new Card { Id = cardId, ImageRefUrl = "cards/old.png" };

        _cardRepoMock
            .Setup(r => r.GetById(cardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(card);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.DeleteImage(cardId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("cards/old.png", result.Value);
        Assert.Equal(string.Empty, card.ImageRefUrl);
    }

    [Fact]
    public async Task DeleteImage_WhenCardNotFound_ReturnsFailResult()
    {
        // Arrange
        _cardRepoMock
            .Setup(r => r.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Card?)null);

        // Act
        var result = await _sut.DeleteImage(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Card not found.", result.Error);
    }

    [Fact]
    public async Task DeleteImage_WhenCardHasNoImage_ReturnsFailResult()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var card = new Card { Id = cardId, ImageRefUrl = string.Empty };

        _cardRepoMock
            .Setup(r => r.GetById(cardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(card);

        // Act
        var result = await _sut.DeleteImage(cardId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Card has no image to delete.", result.Error);
    }
}
