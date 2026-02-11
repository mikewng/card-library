using Moq;
using card_library.Core.Application.Models;
using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Repository.Contracts;
using card_library.Core.Application.Services;
using card_library.Core.Application.Services.Contracts;

namespace card_library.Tests.Services;

public class TextIconServiceTests
{
    private readonly Mock<ITextIconRepository> _textIconRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IFileStorageService> _fileStorageMock;
    private readonly TextIconService _sut;

    public TextIconServiceTests()
    {
        _textIconRepoMock = new Mock<ITextIconRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _fileStorageMock = new Mock<IFileStorageService>();

        _sut = new TextIconService(
            _textIconRepoMock.Object,
            _unitOfWorkMock.Object,
            _fileStorageMock.Object
        );
    }

    // ── CreateIcon ────────────────────────────────────────────────

    [Fact]
    public async Task CreateIcon_WhenNameIsUnique_ReturnsOkWithResponse()
    {
        // Arrange
        var request = new NewTextIconRequest { Name = "fire" };

        _textIconRepoMock
            .Setup(r => r.GetByName("fire", It.IsAny<CancellationToken>()))
            .ReturnsAsync((TextIcon?)null);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.CreateIcon(request, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("fire", result.Value.Name);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        _textIconRepoMock.Verify(r => r.AddAsync(It.IsAny<TextIcon>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateIcon_WhenNameAlreadyExists_ReturnsFailResult()
    {
        // Arrange
        var request = new NewTextIconRequest { Name = "fire" };
        var existing = new TextIcon { Id = Guid.NewGuid(), Name = "fire" };

        _textIconRepoMock
            .Setup(r => r.GetByName("fire", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _sut.CreateIcon(request, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("already exists", result.Error);
        _textIconRepoMock.Verify(r => r.AddAsync(It.IsAny<TextIcon>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ── GetAllIcons ───────────────────────────────────────────────

    [Fact]
    public async Task GetAllIcons_ReturnsAllIconsWithPresignedUrls()
    {
        // Arrange
        var icons = new List<TextIcon>
        {
            new TextIcon { Id = Guid.NewGuid(), Name = "fire", ImageRefUrl = "icons/fire.png" },
            new TextIcon { Id = Guid.NewGuid(), Name = "ice", ImageRefUrl = "icons/ice.png" }
        };

        _textIconRepoMock
            .Setup(r => r.GetAll(It.IsAny<CancellationToken>()))
            .ReturnsAsync(icons);

        _fileStorageMock
            .Setup(f => f.GetPresignedUrlAsync("icons/fire.png", It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://s3.example.com/fire-presigned");

        _fileStorageMock
            .Setup(f => f.GetPresignedUrlAsync("icons/ice.png", It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://s3.example.com/ice-presigned");

        // Act
        var result = await _sut.GetAllIcons(CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Value!.Count);
        Assert.Equal("https://s3.example.com/fire-presigned", result.Value[0].PublicImgUrl);
        Assert.Equal("https://s3.example.com/ice-presigned", result.Value[1].PublicImgUrl);
    }

    [Fact]
    public async Task GetAllIcons_WhenEmpty_ReturnsEmptyList()
    {
        // Arrange
        _textIconRepoMock
            .Setup(r => r.GetAll(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TextIcon>());

        // Act
        var result = await _sut.GetAllIcons(CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Empty(result.Value!);
    }

    // ── GetIconById ───────────────────────────────────────────────

    [Fact]
    public async Task GetIconById_WhenIconExists_ReturnsOkWithPresignedUrl()
    {
        // Arrange
        var iconId = Guid.NewGuid();
        var icon = new TextIcon { Id = iconId, Name = "fire", ImageRefUrl = "icons/fire.png" };

        _textIconRepoMock
            .Setup(r => r.GetById(iconId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(icon);

        _fileStorageMock
            .Setup(f => f.GetPresignedUrlAsync("icons/fire.png", It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://s3.example.com/fire-presigned");

        // Act
        var result = await _sut.GetIconById(iconId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(iconId, result.Value!.Id);
        Assert.Equal("fire", result.Value.Name);
        Assert.Equal("https://s3.example.com/fire-presigned", result.Value.PublicImgUrl);
    }

    [Fact]
    public async Task GetIconById_WhenIconNotFound_ReturnsFailResult()
    {
        // Arrange
        _textIconRepoMock
            .Setup(r => r.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TextIcon?)null);

        // Act
        var result = await _sut.GetIconById(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Icon not found.", result.Error);
    }

    [Fact]
    public async Task GetIconById_WhenNoImage_ReturnsEmptyPublicImgUrl()
    {
        // Arrange
        var iconId = Guid.NewGuid();
        var icon = new TextIcon { Id = iconId, Name = "fire", ImageRefUrl = string.Empty };

        _textIconRepoMock
            .Setup(r => r.GetById(iconId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(icon);

        // Act
        var result = await _sut.GetIconById(iconId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(string.Empty, result.Value!.PublicImgUrl);
        _fileStorageMock.Verify(
            f => f.GetPresignedUrlAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ── UploadImage ───────────────────────────────────────────────

    [Fact]
    public async Task UploadImage_WhenIconExists_SavesKeyAndReturnsOk()
    {
        // Arrange
        var iconId = Guid.NewGuid();
        var icon = new TextIcon { Id = iconId, Name = "fire" };

        _textIconRepoMock
            .Setup(r => r.GetById(iconId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(icon);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.UploadImage(iconId, "icons/images/fire.png", CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("icons/images/fire.png", result.Value);
        Assert.Equal("icons/images/fire.png", icon.ImageRefUrl);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadImage_WhenIconNotFound_ReturnsFailResult()
    {
        // Arrange
        _textIconRepoMock
            .Setup(r => r.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TextIcon?)null);

        // Act
        var result = await _sut.UploadImage(Guid.NewGuid(), "icons/img.png", CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Icon not found.", result.Error);
    }

    // ── DeleteIcon ────────────────────────────────────────────────

    [Fact]
    public async Task DeleteIcon_WhenIconExists_DeletesAndReturnsS3Key()
    {
        // Arrange
        var iconId = Guid.NewGuid();
        var icon = new TextIcon { Id = iconId, Name = "fire", ImageRefUrl = "icons/fire.png" };

        _textIconRepoMock
            .Setup(r => r.GetById(iconId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(icon);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.DeleteIcon(iconId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("icons/fire.png", result.Value);
        _textIconRepoMock.Verify(r => r.Delete(icon), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteIcon_WhenIconNotFound_ReturnsFailResult()
    {
        // Arrange
        _textIconRepoMock
            .Setup(r => r.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TextIcon?)null);

        // Act
        var result = await _sut.DeleteIcon(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Icon not found.", result.Error);
        _textIconRepoMock.Verify(r => r.Delete(It.IsAny<TextIcon>()), Times.Never);
    }

    [Fact]
    public async Task DeleteIcon_WhenIconHasNoImage_ReturnsEmptyS3Key()
    {
        // Arrange
        var iconId = Guid.NewGuid();
        var icon = new TextIcon { Id = iconId, Name = "fire", ImageRefUrl = string.Empty };

        _textIconRepoMock
            .Setup(r => r.GetById(iconId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(icon);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.DeleteIcon(iconId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(string.Empty, result.Value);
        _textIconRepoMock.Verify(r => r.Delete(icon), Times.Once);
    }
}
