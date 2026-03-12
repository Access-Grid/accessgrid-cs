using System.Net;
using System.Text;
using AccessGrid;
using NUnit.Framework;
using Moq;

namespace AccessGridTest;

[TestFixture]
public class AccessGridClientTests
{
    [Test]
    public async Task GetAsync_ShouldDeserializeResponse_WhenApiReturnsValidJson()
    {
        // Arrange
        var mockHttpClient = new Mock<IHttpClientWrapper>();
        var jsonResponse = """{"id": "test-id", "name": "Test"}""";
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        };

        mockHttpClient
            .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(httpResponse);

        var client = new AccessGridClient("test-account", "test-secret", mockHttpClient.Object);

        // Act
        var result = await client.GetAsync<TestModel>("/test-endpoint");

        // Assert
        Assert.That(result.Id, Is.EqualTo("test-id"));
        Assert.That(result.Name, Is.EqualTo("Test"));
    }

    [Test]
    public void Constructor_ShouldThrow_WhenAccountIdIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => new AccessGridClient("", "test-secret"));
    }

    [Test]
    public void Constructor_ShouldThrow_WhenSecretKeyIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => new AccessGridClient("test-account", ""));
    }

    [Test]
    public void Constructor_ShouldThrow_WhenAccountIdIsNull()
    {
        Assert.Throws<ArgumentException>(() => new AccessGridClient(null, "test-secret"));
    }

    [Test]
    public void Request_ShouldThrowAuthenticationException_On401()
    {
        // Arrange
        var mockHttpClient = new Mock<IHttpClientWrapper>();
        mockHttpClient
            .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("Unauthorized", Encoding.UTF8, "application/json")
            });

        var client = new AccessGridClient("test-account", "test-secret", mockHttpClient.Object);

        // Act & Assert
        Assert.ThrowsAsync<AuthenticationException>(() =>
            client.GetAsync<TestModel>("/test-endpoint"));
    }

    [Test]
    public void Request_ShouldThrowAccessGridException_On402()
    {
        // Arrange
        var mockHttpClient = new Mock<IHttpClientWrapper>();
        mockHttpClient
            .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.PaymentRequired)
            {
                Content = new StringContent("Payment required", Encoding.UTF8, "application/json")
            });

        var client = new AccessGridClient("test-account", "test-secret", mockHttpClient.Object);

        // Act & Assert
        var ex = Assert.ThrowsAsync<AccessGridException>(() =>
            client.GetAsync<TestModel>("/test-endpoint"));
        Assert.That(ex.Message, Is.EqualTo("Insufficient account balance"));
    }

    [Test]
    public void Request_ShouldThrowAccessGridException_OnOtherErrors()
    {
        // Arrange
        var mockHttpClient = new Mock<IHttpClientWrapper>();
        mockHttpClient
            .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("{\"message\": \"Invalid input\"}", Encoding.UTF8, "application/json")
            });

        var client = new AccessGridClient("test-account", "test-secret", mockHttpClient.Object);

        // Act & Assert
        Assert.ThrowsAsync<AccessGridException>(() =>
            client.GetAsync<TestModel>("/test-endpoint"));
    }

    public class TestModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}