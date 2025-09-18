namespace AccessGridTest;

using AccessGrid;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading.Tasks;
using NUnit.Framework;

[TestFixture]
public class IntegrationTests
{
    [Test]
    public async Task ServiceIntegration_ShouldWork_WithDependencyInjection()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockApiService = new Mock<IApiService>();

        // Configure mock behavior
        mockApiService
            .Setup(x => x.GetAsync<KeysListResponse>("/v1/key-cards", It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(new KeysListResponse { Keys = new List<AccessCard>() });

        // Register services
        services.AddSingleton(mockApiService.Object);
        services.AddTransient<AccessCardsService>();

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var cardsService = serviceProvider.GetRequiredService<AccessCardsService>();
        var result = await cardsService.ListAsync(new ListKeysRequest());

        // Assert
        Assert.That(result, Is.Not.Null);
        mockApiService.Verify(x => x.GetAsync<KeysListResponse>("/v1/key-cards", It.IsAny<Dictionary<string, string>>()), Times.Once);
    }
}