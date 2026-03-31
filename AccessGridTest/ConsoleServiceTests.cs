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
        Assert.That(first.CreatedAt, Is.EqualTo(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
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

    #region CreateTemplateAsync

    [Test]
    public async Task CreateTemplateAsync_ShouldPostAndReturnTemplate()
    {
        var json = """
        {
            "id": "tmpl-new",
            "name": "Employee NFC Key",
            "platform": "apple",
            "use_case": "employee_badge",
            "protocol": "desfire",
            "created_at": "2025-03-01T00:00:00Z",
            "issued_keys_count": 0,
            "active_keys_count": 0,
            "metadata": { "version": "2.1" }
        }
        """;
        StubHttpResponse(json);

        var request = new CreateTemplateRequest
        {
            Name = "Employee NFC Key",
            Platform = Platform.Apple,
            UseCase = "employee_badge",
            Protocol = Protocol.DESFire,
            AllowOnMultipleDevices = true,
            WatchCount = 2,
            IPhoneCount = 3,
            BackgroundColor = "#FFFFFF",
            LabelColor = "#000000",
            LabelSecondaryColor = "#333333",
            SupportUrl = "https://help.yourcompany.com",
            SupportPhoneNumber = "+1-555-123-4567",
            SupportEmail = "support@yourcompany.com",
            PrivacyPolicyUrl = "https://yourcompany.com/privacy",
            TermsAndConditionsUrl = "https://yourcompany.com/terms",
            Metadata = new Dictionary<string, object>
            {
                ["version"] = "2.1",
                ["approval_status"] = "approved"
            }
        };

        var result = await _client.Console.CreateTemplateAsync(request);

        Assert.That(result.Id, Is.EqualTo("tmpl-new"));
        Assert.That(result.Name, Is.EqualTo("Employee NFC Key"));
        Assert.That(result.Platform, Is.EqualTo("apple"));
        Assert.That(result.UseCase, Is.EqualTo("employee_badge"));
        Assert.That(result.Protocol, Is.EqualTo("desfire"));
        Assert.That(result.IssuedKeysCount, Is.EqualTo(0));

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Post &&
            req.RequestUri!.ToString().Contains("/v1/console/card-templates")
        )), Times.Once);
    }

    [Test]
    public async Task CreateTemplateAsync_SendsFlatDesignAndSupportParams()
    {
        var json = """
        {
            "id": "tmpl-flat",
            "name": "Flat Params Template",
            "platform": "apple",
            "use_case": "employee_badge",
            "protocol": "desfire",
            "metadata": { "version": "2.1" }
        }
        """;

        string capturedBody = null;
        _mockHttpClient
            .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
            .Returns<HttpRequestMessage>(async req =>
            {
                if (req.Content != null)
                    capturedBody = await req.Content.ReadAsStringAsync();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            });

        var request = new CreateTemplateRequest
        {
            Name = "Flat Params Template",
            Platform = Platform.Apple,
            UseCase = "employee_badge",
            Protocol = Protocol.DESFire,
            BackgroundColor = "#FFFFFF",
            SupportUrl = "https://help.yourcompany.com",
            Metadata = new Dictionary<string, object> { ["version"] = "2.1" }
        };

        await _client.Console.CreateTemplateAsync(request);

        Assert.That(capturedBody, Is.Not.Null);
        // Flat params should appear at root level, not nested under design/support_info
        Assert.That(capturedBody, Does.Contain("background_color"));
        Assert.That(capturedBody, Does.Contain("support_url"));
        Assert.That(capturedBody, Does.Not.Contain("\"design\""));
        Assert.That(capturedBody, Does.Not.Contain("\"support_info\""));
    }

    #endregion

    #region UpdateTemplateAsync

    [Test]
    public async Task UpdateTemplateAsync_ShouldPutAndReturnTemplate()
    {
        var json = """
        {
            "id": "tmpl-123",
            "name": "Updated Badge",
            "platform": "apple",
            "use_case": "employee_badge",
            "protocol": "desfire"
        }
        """;
        StubHttpResponse(json);

        var request = new UpdateTemplateRequest
        {
            CardTemplateId = "tmpl-123",
            Name = "Updated Badge",
            AllowOnMultipleDevices = false,
            WatchCount = 1,
            IPhoneCount = 2,
            BackgroundColor = "#FFFFFF",
            LabelColor = "#000000",
            LabelSecondaryColor = "#333333",
            SupportUrl = "https://help.yourcompany.com",
            SupportPhoneNumber = "+1-555-123-4567",
            SupportEmail = "support@yourcompany.com",
            PrivacyPolicyUrl = "https://yourcompany.com/privacy",
            TermsAndConditionsUrl = "https://yourcompany.com/terms"
        };

        var result = await _client.Console.UpdateTemplateAsync(request);

        Assert.That(result.Id, Is.EqualTo("tmpl-123"));
        Assert.That(result.Name, Is.EqualTo("Updated Badge"));

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Put &&
            req.RequestUri!.ToString().Contains("/v1/console/card-templates/tmpl-123")
        )), Times.Once);
    }

    [Test]
    public async Task UpdateTemplateAsync_SendsFlatDesignAndSupportParams()
    {
        var json = """{ "id": "tmpl-123", "name": "Test" }""";

        string capturedBody = null;
        _mockHttpClient
            .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
            .Returns<HttpRequestMessage>(async req =>
            {
                if (req.Content != null)
                    capturedBody = await req.Content.ReadAsStringAsync();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            });

        var request = new UpdateTemplateRequest
        {
            CardTemplateId = "tmpl-123",
            Name = "Test",
            BackgroundColor = "#FFFFFF",
            SupportUrl = "https://help.yourcompany.com"
        };

        await _client.Console.UpdateTemplateAsync(request);

        Assert.That(capturedBody, Is.Not.Null);
        Assert.That(capturedBody, Does.Contain("background_color"));
        Assert.That(capturedBody, Does.Contain("support_url"));
        Assert.That(capturedBody, Does.Not.Contain("\"design\""));
        Assert.That(capturedBody, Does.Not.Contain("\"support_info\""));
    }

    #endregion

    #region ReadTemplateAsync

    [Test]
    public async Task ReadTemplateAsync_ShouldGetAndReturnTemplate()
    {
        var json = """
        {
            "id": "tmpl-456",
            "name": "Visitor Pass",
            "platform": "google",
            "use_case": "employee_badge",
            "protocol": "desfire",
            "created_at": "2025-01-15T00:00:00Z",
            "last_published_at": "2025-02-01T00:00:00Z",
            "issued_keys_count": 42,
            "active_keys_count": 38
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.ReadTemplateAsync("tmpl-456");

        Assert.That(result.Id, Is.EqualTo("tmpl-456"));
        Assert.That(result.Name, Is.EqualTo("Visitor Pass"));
        Assert.That(result.Platform, Is.EqualTo("google"));
        Assert.That(result.CreatedAt, Is.EqualTo("2025-01-15T00:00:00Z"));
        Assert.That(result.LastPublishedAt, Is.EqualTo("2025-02-01T00:00:00Z"));
        Assert.That(result.IssuedKeysCount, Is.EqualTo(42));
        Assert.That(result.ActiveKeysCount, Is.EqualTo(38));

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Get &&
            req.RequestUri!.ToString().Contains("/v1/console/card-templates/tmpl-456")
        )), Times.Once);
    }

    #endregion

    #region EventLogAsync

    [Test]
    public async Task EventLogAsync_ShouldReturnEventLogEntries()
    {
        var json = """
        {
            "events": [
                { "type": "install", "timestamp": "2025-03-01T10:00:00Z", "user_id": "user-1" },
                { "type": "suspend", "timestamp": "2025-03-02T14:00:00Z", "user_id": "user-2" }
            ]
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.EventLogAsync("tmpl-789");

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Type, Is.EqualTo("install"));
        Assert.That(result[0].UserId, Is.EqualTo("user-1"));
        Assert.That(result[1].Type, Is.EqualTo("suspend"));

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Get &&
            req.RequestUri!.ToString().Contains("/v1/console/card-templates/tmpl-789/logs")
        )), Times.Once);
    }

    [Test]
    public async Task EventLogAsync_ShouldPassFilters()
    {
        var json = """
        {
            "events": []
        }
        """;
        StubHttpResponse(json);

        var filters = new EventLogFilters
        {
            Device = "mobile",
            StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2025, 6, 30, 0, 0, 0, DateTimeKind.Utc),
            EventType = "install"
        };

        await _client.Console.EventLogAsync("tmpl-789", filters);

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Get &&
            req.RequestUri!.ToString().Contains("device=mobile") &&
            req.RequestUri!.ToString().Contains("start_date=") &&
            req.RequestUri!.ToString().Contains("end_date=") &&
            req.RequestUri!.ToString().Contains("event_type=install")
        )), Times.Once);
    }

    [Test]
    public async Task EventLogAsync_ShouldHandleEmptyResponse()
    {
        var json = """
        {
            "events": []
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.EventLogAsync("tmpl-789");

        Assert.That(result, Is.Empty);
    }

    #endregion

    #region GetLedgerItemsAsync

    [Test]
    public async Task GetLedgerItemsAsync_ReturnsLedgerItemsWithNestedObjects()
    {
        var json = """
        {
            "ledger_items": [
                {
                    "created_at": "2025-06-15T14:30:00Z",
                    "amount": -1.50,
                    "id": "li_abc123",
                    "kind": "access_pass_debit",
                    "metadata": {
                        "access_pass_ex_id": "ap_xyz",
                        "pass_template_ex_id": "pt_456"
                    },
                    "access_pass": {
                        "id": "ap_xyz",
                        "full_name": "Jane Doe",
                        "state": "active",
                        "metadata": { "department": "Engineering" },
                        "unified_access_pass_ex_id": "uap_789",
                        "pass_template": {
                            "id": "pt_456",
                            "name": "Employee Badge",
                            "protocol": "desfire",
                            "platform": "apple",
                            "use_case": "employee_badge"
                        }
                    }
                }
            ],
            "pagination": {
                "current_page": 1,
                "per_page": 50,
                "total_pages": 1,
                "total_count": 1
            }
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.GetLedgerItemsAsync();

        Assert.That(result.LedgerItems, Has.Count.EqualTo(1));

        var item = result.LedgerItems[0];
        Assert.That(item.CreatedAt, Is.EqualTo(new DateTime(2025, 6, 15, 14, 30, 0, DateTimeKind.Utc)));
        Assert.That(item.Amount, Is.EqualTo(-1.50m));
        Assert.That(item.Id, Is.EqualTo("li_abc123"));
        Assert.That(item.Kind, Is.EqualTo("access_pass_debit"));
        Assert.That(item.Metadata["access_pass_ex_id"].ToString(), Is.EqualTo("ap_xyz"));

        var ap = item.AccessPass;
        Assert.That(ap, Is.Not.Null);
        Assert.That(ap!.Id, Is.EqualTo("ap_xyz"));
        Assert.That(ap.FullName, Is.EqualTo("Jane Doe"));
        Assert.That(ap.State, Is.EqualTo("active"));
        Assert.That(ap.UnifiedAccessPassExId, Is.EqualTo("uap_789"));

        var pt = ap.PassTemplate;
        Assert.That(pt, Is.Not.Null);
        Assert.That(pt!.Id, Is.EqualTo("pt_456"));
        Assert.That(pt.Name, Is.EqualTo("Employee Badge"));
        Assert.That(pt.Protocol, Is.EqualTo("desfire"));
        Assert.That(pt.Platform, Is.EqualTo("apple"));
        Assert.That(pt.UseCase, Is.EqualTo("employee_badge"));

        Assert.That(result.Pagination.TotalCount, Is.EqualTo(1));
    }

    [Test]
    public async Task GetLedgerItemsAsync_PassesDateFilters()
    {
        var json = """
        {
            "ledger_items": [],
            "pagination": { "current_page": 1, "per_page": 50, "total_pages": 0, "total_count": 0 }
        }
        """;
        StubHttpResponse(json);

        var startDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2025, 6, 30, 23, 59, 59, DateTimeKind.Utc);

        await _client.Console.GetLedgerItemsAsync(startDate: startDate, endDate: endDate, page: 2, perPage: 25);

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Get &&
            req.RequestUri!.ToString().Contains("start_date=") &&
            req.RequestUri!.ToString().Contains("end_date=") &&
            req.RequestUri!.ToString().Contains("page=2") &&
            req.RequestUri!.ToString().Contains("per_page=25")
        )), Times.Once);
    }

    [Test]
    public async Task GetLedgerItemsAsync_HandlesNullAccessPass()
    {
        var json = """
        {
            "ledger_items": [
                {
                    "created_at": "2025-03-01T00:00:00Z",
                    "amount": 100.00,
                    "id": "li_credit_1",
                    "kind": "credit",
                    "metadata": {},
                    "access_pass": null
                }
            ],
            "pagination": { "current_page": 1, "per_page": 50, "total_pages": 1, "total_count": 1 }
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.GetLedgerItemsAsync();

        var item = result.LedgerItems[0];
        Assert.That(item.Kind, Is.EqualTo("credit"));
        Assert.That(item.Amount, Is.EqualTo(100.00m));
        Assert.That(item.AccessPass, Is.Null);
    }

    [Test]
    public async Task GetLedgerItemsAsync_HandlesNullPassTemplate()
    {
        var json = """
        {
            "ledger_items": [
                {
                    "created_at": "2025-04-10T12:00:00Z",
                    "amount": -2.00,
                    "id": "li_no_tmpl",
                    "kind": "access_pass_debit",
                    "metadata": { "access_pass_ex_id": "ap_solo" },
                    "access_pass": {
                        "id": "ap_solo",
                        "full_name": "Bob Smith",
                        "state": "suspended",
                        "metadata": {},
                        "unified_access_pass_ex_id": null,
                        "pass_template": null
                    }
                }
            ],
            "pagination": { "current_page": 1, "per_page": 50, "total_pages": 1, "total_count": 1 }
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.GetLedgerItemsAsync();

        var ap = result.LedgerItems[0].AccessPass;
        Assert.That(ap, Is.Not.Null);
        Assert.That(ap!.FullName, Is.EqualTo("Bob Smith"));
        Assert.That(ap.State, Is.EqualTo("suspended"));
        Assert.That(ap.UnifiedAccessPassExId, Is.Null);
        Assert.That(ap.PassTemplate, Is.Null);
    }

    [Test]
    public async Task GetLedgerItemsAsync_HandlesEmptyResponse()
    {
        var json = """
        {
            "ledger_items": [],
            "pagination": { "current_page": 1, "per_page": 50, "total_pages": 0, "total_count": 0 }
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.GetLedgerItemsAsync();

        Assert.That(result.LedgerItems, Is.Empty);
        Assert.That(result.Pagination.TotalCount, Is.EqualTo(0));
    }

    [Test]
    public async Task GetLedgerItemsAsync_DeserializesTemporaryPassOnAccessPass()
    {
        var json = """
        {
            "ledger_items": [
                {
                    "created_at": "2025-06-15T14:30:00Z",
                    "amount": -0.50,
                    "id": "li_tmp_1",
                    "kind": "temporary_pass_debit",
                    "metadata": {
                        "access_pass_ex_id": "ap_tmp",
                        "temporary": true
                    },
                    "access_pass": {
                        "id": "ap_tmp",
                        "full_name": "Temp Visitor",
                        "state": "active",
                        "temporary": true,
                        "metadata": { "department": "Lobby" },
                        "unified_access_pass_ex_id": "uap_tmp",
                        "pass_template": {
                            "id": "pt_789",
                            "name": "Visitor Badge",
                            "protocol": "desfire",
                            "platform": "apple",
                            "use_case": "employee_badge"
                        }
                    }
                }
            ],
            "pagination": {
                "current_page": 1,
                "per_page": 50,
                "total_pages": 1,
                "total_count": 1
            }
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.GetLedgerItemsAsync();

        var ap = result.LedgerItems[0].AccessPass;
        Assert.That(ap, Is.Not.Null);
        Assert.That(ap!.Temporary, Is.EqualTo(true));
        Assert.That(ap.FullName, Is.EqualTo("Temp Visitor"));
    }

    [Test]
    public async Task GetLedgerItemsAsync_TemporaryIsNull_WhenAbsentFromAccessPass()
    {
        var json = """
        {
            "ledger_items": [
                {
                    "created_at": "2025-06-15T14:30:00Z",
                    "amount": -1.50,
                    "id": "li_reg_1",
                    "kind": "access_pass_debit",
                    "metadata": {},
                    "access_pass": {
                        "id": "ap_reg",
                        "full_name": "Regular User",
                        "state": "active",
                        "metadata": {},
                        "unified_access_pass_ex_id": null
                    }
                }
            ],
            "pagination": { "current_page": 1, "per_page": 50, "total_pages": 1, "total_count": 1 }
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.GetLedgerItemsAsync();

        var ap = result.LedgerItems[0].AccessPass;
        Assert.That(ap, Is.Not.Null);
        Assert.That(ap!.Temporary, Is.Null);
    }

    #endregion
}
