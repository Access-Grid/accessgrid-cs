using System.Net;
using System.Text;
using AccessGrid;
using NUnit.Framework;
using Moq;

namespace AccessGridTest;

[TestFixture]
public class ConsoleServiceTests
{
    private Mock<IHttpClientWrapper> _mockHttpClient;
    private AccessGridClient _client;

    [SetUp]
    public void SetUp()
    {
        _mockHttpClient = new Mock<IHttpClientWrapper>();
        _client = new AccessGridClient("test_account", "test_secret", _mockHttpClient.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _client?.Dispose();
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
    public async Task ListPassTemplatePairsAsync_ReturnsPassTemplatePairs()
    {
        // Same fixture as Ruby console_spec.rb #list_pass_template_pairs
        var json = """
        {
            "pass_template_pairs": [
                {
                    "id": "pair_1",
                    "name": "Employee Badge Pair",
                    "created_at": "2025-01-01T00:00:00Z",
                    "ios_template": { "id": "tmpl_ios_1", "name": "iOS Badge", "platform": "apple" },
                    "android_template": { "id": "tmpl_android_1", "name": "Android Badge", "platform": "android" }
                },
                {
                    "id": "pair_2",
                    "name": "Contractor Badge Pair",
                    "created_at": "2025-01-02T00:00:00Z",
                    "ios_template": { "id": "tmpl_ios_2", "name": "iOS Contractor", "platform": "apple" },
                    "android_template": null
                }
            ],
            "pagination": {
                "current_page": 1,
                "per_page": 50,
                "total_pages": 1,
                "total_count": 2
            }
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.ListPassTemplatePairsAsync();

        Assert.That(result.PassTemplatePairs, Has.Count.EqualTo(2));

        var first = result.PassTemplatePairs[0];
        Assert.That(first.Id, Is.EqualTo("pair_1"));
        Assert.That(first.Name, Is.EqualTo("Employee Badge Pair"));
        Assert.That(first.CreatedAt, Is.EqualTo("2025-01-01T00:00:00Z"));
        Assert.That(first.IosTemplate.Id, Is.EqualTo("tmpl_ios_1"));
        Assert.That(first.IosTemplate.Platform, Is.EqualTo("apple"));
        Assert.That(first.AndroidTemplate.Id, Is.EqualTo("tmpl_android_1"));
        Assert.That(first.AndroidTemplate.Platform, Is.EqualTo("android"));

        var second = result.PassTemplatePairs[1];
        Assert.That(second.Id, Is.EqualTo("pair_2"));
        Assert.That(second.AndroidTemplate, Is.Null);
        Assert.That(second.IosTemplate, Is.Not.Null);

        Assert.That(result.Pagination.CurrentPage, Is.EqualTo(1));
        Assert.That(result.Pagination.PerPage, Is.EqualTo(50));
        Assert.That(result.Pagination.TotalPages, Is.EqualTo(1));
        Assert.That(result.Pagination.TotalCount, Is.EqualTo(2));
    }

    [Test]
    public async Task ListPassTemplatePairsAsync_PassesPaginationParams()
    {
        var json = """
        {
            "pass_template_pairs": [],
            "pagination": { "current_page": 2, "per_page": 10, "total_pages": 3, "total_count": 25 }
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.ListPassTemplatePairsAsync(page: 2, perPage: 10);

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Get &&
            req.RequestUri!.ToString().Contains("page=2") &&
            req.RequestUri!.ToString().Contains("per_page=10")
        )), Times.Once);

        Assert.That(result.Pagination.CurrentPage, Is.EqualTo(2));
        Assert.That(result.Pagination.PerPage, Is.EqualTo(10));
    }

    [Test]
    public async Task ListPassTemplatePairsAsync_HandlesEmptyResponse()
    {
        var json = """
        {
            "pass_template_pairs": [],
            "pagination": { "current_page": 1, "per_page": 50, "total_pages": 0, "total_count": 0 }
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.ListPassTemplatePairsAsync();

        Assert.That(result.PassTemplatePairs, Is.Empty);
        Assert.That(result.Pagination.TotalCount, Is.EqualTo(0));
    }

    [Test]
    public async Task ListPassTemplatePairsAsync_SetsAuthHeaders()
    {
        var json = """
        {
            "pass_template_pairs": [],
            "pagination": { "current_page": 1, "per_page": 50, "total_pages": 0, "total_count": 0 }
        }
        """;
        StubHttpResponse(json);

        await _client.Console.ListPassTemplatePairsAsync();

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Headers.Contains("X-ACCT-ID") &&
            req.Headers.GetValues("X-ACCT-ID")!.First() == "test_account" &&
            req.Headers.Contains("X-PAYLOAD-SIG")
        )), Times.Once);
    }
}
