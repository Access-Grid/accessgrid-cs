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

    public class TestModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}