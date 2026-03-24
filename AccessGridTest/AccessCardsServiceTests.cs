namespace AccessGridTest;

using AccessGrid;
using Moq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

[TestFixture]
public class AccessCardsServiceTests
{
    private Mock<IHttpClientWrapper> _mockHttpClient;
    private AccessGridClient _httpClient;

    [SetUp]
    public void SetUp()
    {
        _mockHttpClient = new Mock<IHttpClientWrapper>();
        _httpClient = new AccessGridClient("test_account", "test_secret", _mockHttpClient.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
    }

    private void StubHttpResponse(string json, HttpStatusCode status = HttpStatusCode.OK)
    {
        _mockHttpClient
            .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new HttpResponseMessage(status)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
    }
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

    [Test]
    public async Task DeleteAsync_ShouldPostToCorrectEndpoint()
    {
        // Arrange
        var mockApiService = new Mock<IApiService>();
        var expectedCard = new AccessCard("card-789", null, null);

        mockApiService
            .Setup(x => x.PostAsync<AccessCard>("/v1/key-cards/card-789/delete", null))
            .ReturnsAsync(expectedCard);

        var service = new AccessCardsService(mockApiService.Object);

        // Act
        var result = await service.DeleteAsync("card-789");

        // Assert
        Assert.That(result.Id, Is.EqualTo("card-789"));
        mockApiService.Verify(x => x.PostAsync<AccessCard>("/v1/key-cards/card-789/delete", null), Times.Once);
    }

    [Test]
    public async Task ProvisionAsync_ShouldDelegateToIssueAsync()
    {
        // Arrange
        var mockApiService = new Mock<IApiService>();
        var expectedCard = new AccessCard("card-new", "https://example.com/install", AccessPassState.Active);

        mockApiService
            .Setup(x => x.PostAsync<AccessCard>("/v1/key-cards", It.IsAny<ProvisionCardRequest>()))
            .ReturnsAsync(expectedCard);

        var service = new AccessCardsService(mockApiService.Object);
        var request = new ProvisionCardRequest
        {
            CardTemplateId = "template-123",
            FullName = "Test User"
        };

        // Act
        var result = await service.ProvisionAsync(request);

        // Assert
        Assert.That(result.Id, Is.EqualTo("card-new"));
        mockApiService.Verify(x => x.PostAsync<AccessCard>("/v1/key-cards", request), Times.Once);
    }

    #region Temporary Pass

    [Test]
    public async Task GetAsync_DeserializesTemporaryField_WhenTrue()
    {
        var json = """
        {
            "id": "card-tmp-1",
            "install_url": "https://example.com/install",
            "state": "active",
            "temporary": true,
            "expiration_date": "2025-03-01T00:00:00Z"
        }
        """;
        StubHttpResponse(json);

        var result = await _httpClient.AccessCards.GetAsync("card-tmp-1");

        Assert.That(result.Temporary, Is.EqualTo(true));
        Assert.That(result.Id, Is.EqualTo("card-tmp-1"));
    }

    [Test]
    public async Task GetAsync_DeserializesTemporaryField_WhenFalse()
    {
        var json = """
        {
            "id": "card-reg-1",
            "install_url": "https://example.com/install",
            "state": "active",
            "temporary": false
        }
        """;
        StubHttpResponse(json);

        var result = await _httpClient.AccessCards.GetAsync("card-reg-1");

        Assert.That(result.Temporary, Is.EqualTo(false));
    }

    [Test]
    public async Task GetAsync_TemporaryIsNull_WhenAbsentFromResponse()
    {
        var json = """
        {
            "id": "card-old-1",
            "install_url": "https://example.com/install",
            "state": "active"
        }
        """;
        StubHttpResponse(json);

        var result = await _httpClient.AccessCards.GetAsync("card-old-1");

        Assert.That(result.Temporary, Is.Null);
    }

    [Test]
    public async Task IssueAsync_CanSetTemporaryOnRequest()
    {
        var json = """
        {
            "id": "card-tmp-new",
            "install_url": "https://example.com/install",
            "state": "active",
            "temporary": true
        }
        """;
        StubHttpResponse(json);

        var request = new ProvisionCardRequest
        {
            CardTemplateId = "template-123",
            FullName = "Temp User",
            Temporary = true
        };

        var result = await _httpClient.AccessCards.IssueAsync(request);

        Assert.That(result.Temporary, Is.EqualTo(true));

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Post &&
            req.RequestUri!.ToString().Contains("/v1/key-cards")
        )), Times.Once);
    }

    #endregion
}