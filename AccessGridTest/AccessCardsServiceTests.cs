namespace AccessGridTest;

using AccessGrid;
using Moq;
using System.Threading.Tasks;
using NUnit.Framework;

[TestFixture]
public class AccessCardsServiceTests
{
    [Test]
    public async Task IssueAsync_ShouldReturnAccessCard_WhenRequestIsValid()
    {
        // Arrange
        var mockApiService = new Mock<IApiService>();
        var expectedCard = new AccessCard("test-card-id", "Test User", AccessPassState.Active);

        mockApiService
            .Setup(x => x.PostAsync<AccessCard>("/v1/key-cards", It.IsAny<ProvisionCardRequest>()))
            .ReturnsAsync(expectedCard);

        var service = new AccessCardsService(mockApiService.Object);
        var request = new ProvisionCardRequest
        {
            CardTemplateId = "template-123",
            EmployeeId = "emp-456",
            FullName = "Test User"
        };

        // Act
        var result = await service.IssueAsync(request);

        // Assert
        Assert.That(result.Id, Is.EqualTo(expectedCard.Id));
        Assert.That(result.FullName, Is.EqualTo(expectedCard.FullName));
        mockApiService.Verify(x => x.PostAsync<AccessCard>("/v1/key-cards", request), Times.Once);
    }

    [Test]
    public async Task GetAsync_ShouldReturnAccessCard_WhenCardExists()
    {
        // Arrange
        var mockApiService = new Mock<IApiService>();
        var expectedCard = new AccessCard("card-123", "https://example.com/install", AccessPassState.Active);

        mockApiService
            .Setup(x => x.GetAsync<AccessCard>("/v1/key-cards/card-123", null))
            .ReturnsAsync(expectedCard);

        var service = new AccessCardsService(mockApiService.Object);

        // Act
        var result = await service.GetAsync("card-123");

        // Assert
        Assert.That(result.Id, Is.EqualTo("card-123"));
        Assert.That(result.State, Is.EqualTo(AccessPassState.Active));
        mockApiService.Verify(x => x.GetAsync<AccessCard>("/v1/key-cards/card-123", null), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_ShouldPatchAndReturnAccessCard()
    {
        // Arrange
        var mockApiService = new Mock<IApiService>();
        var expectedCard = new AccessCard("card-456", "https://example.com/install", AccessPassState.Active);

        mockApiService
            .Setup(x => x.PatchAsync<AccessCard>("/v1/key-cards/card-456", It.IsAny<UpdateCardRequest>()))
            .ReturnsAsync(expectedCard);

        var service = new AccessCardsService(mockApiService.Object);
        var request = new UpdateCardRequest
        {
            FullName = "Updated Name",
            Classification = "contractor"
        };

        // Act
        var result = await service.UpdateAsync("card-456", request);

        // Assert
        Assert.That(result.Id, Is.EqualTo("card-456"));
        mockApiService.Verify(x => x.PatchAsync<AccessCard>("/v1/key-cards/card-456", request), Times.Once);
    }

    [Test]
    public async Task SuspendAsync_ShouldPostToCorrectEndpoint()
    {
        // Arrange
        var mockApiService = new Mock<IApiService>();
        var expectedCard = new AccessCard("card-789", null, AccessPassState.Suspended);

        mockApiService
            .Setup(x => x.PostAsync<AccessCard>("/v1/key-cards/card-789/suspend", null))
            .ReturnsAsync(expectedCard);

        var service = new AccessCardsService(mockApiService.Object);

        // Act
        var result = await service.SuspendAsync("card-789");

        // Assert
        Assert.That(result.State, Is.EqualTo(AccessPassState.Suspended));
        mockApiService.Verify(x => x.PostAsync<AccessCard>("/v1/key-cards/card-789/suspend", null), Times.Once);
    }

    [Test]
    public async Task ResumeAsync_ShouldPostToCorrectEndpoint()
    {
        // Arrange
        var mockApiService = new Mock<IApiService>();
        var expectedCard = new AccessCard("card-789", null, AccessPassState.Active);

        mockApiService
            .Setup(x => x.PostAsync<AccessCard>("/v1/key-cards/card-789/resume", null))
            .ReturnsAsync(expectedCard);

        var service = new AccessCardsService(mockApiService.Object);

        // Act
        var result = await service.ResumeAsync("card-789");

        // Assert
        Assert.That(result.State, Is.EqualTo(AccessPassState.Active));
        mockApiService.Verify(x => x.PostAsync<AccessCard>("/v1/key-cards/card-789/resume", null), Times.Once);
    }

    [Test]
    public async Task UnlinkAsync_ShouldPostToCorrectEndpoint()
    {
        // Arrange
        var mockApiService = new Mock<IApiService>();
        var expectedCard = new AccessCard("card-789", null, AccessPassState.Unlink);

        mockApiService
            .Setup(x => x.PostAsync<AccessCard>("/v1/key-cards/card-789/unlink", null))
            .ReturnsAsync(expectedCard);

        var service = new AccessCardsService(mockApiService.Object);

        // Act
        var result = await service.UnlinkAsync("card-789");

        // Assert
        Assert.That(result.State, Is.EqualTo(AccessPassState.Unlink));
        mockApiService.Verify(x => x.PostAsync<AccessCard>("/v1/key-cards/card-789/unlink", null), Times.Once);
    }
}