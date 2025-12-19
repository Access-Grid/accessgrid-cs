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
        var jsonResponse = @"{""id"":""test-card-id"",""full_name"":""Test User"",""state"":""active""}";

        mockApiService
            .Setup(x => x.PostAsync<string>("/v1/key-cards", It.IsAny<ProvisionCardRequest>()))
            .ReturnsAsync(jsonResponse);

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
        Assert.That(result, Is.InstanceOf<AccessCard>());
        var card = (AccessCard)result;
        Assert.That(card.Id, Is.EqualTo("test-card-id"));
        Assert.That(card.FullName, Is.EqualTo("Test User"));
        mockApiService.Verify(x => x.PostAsync<string>("/v1/key-cards", request), Times.Once);
    }
}