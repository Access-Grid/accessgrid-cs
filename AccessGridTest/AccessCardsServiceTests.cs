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
}