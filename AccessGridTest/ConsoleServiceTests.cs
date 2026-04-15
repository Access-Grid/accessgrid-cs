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
            "card_template_pairs": [
                {
                    "id": "pair_1",
                    "ex_id": "pair_1",
                    "name": "Employee Badge Pair",
                    "created_at": "2025-01-01T00:00:00Z",
                    "ios_template": { "id": "tmpl_ios_1", "ex_id": "tmpl_ios_1", "name": "iOS Badge", "platform": "apple" },
                    "android_template": { "id": "tmpl_android_1", "ex_id": "tmpl_android_1", "name": "Android Badge", "platform": "android" }
                },
                {
                    "id": "pair_2",
                    "ex_id": "pair_2",
                    "name": "Contractor Badge Pair",
                    "created_at": "2025-01-02T00:00:00Z",
                    "ios_template": { "id": "tmpl_ios_2", "ex_id": "tmpl_ios_2", "name": "iOS Contractor", "platform": "apple" },
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
        Assert.That(first.ExId, Is.EqualTo("pair_1"));
        Assert.That(first.Name, Is.EqualTo("Employee Badge Pair"));
        Assert.That(first.CreatedAt, Is.EqualTo(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
        Assert.That(first.IosTemplate.Id, Is.EqualTo("tmpl_ios_1"));
        Assert.That(first.IosTemplate.ExId, Is.EqualTo("tmpl_ios_1"));
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
            "card_template_pairs": [],
            "pagination": { "current_page": 2, "per_page": 10, "total_pages": 3, "total_count": 25 }
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.ListPassTemplatePairsAsync(page: 2, perPage: 10);

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Get &&
            req.RequestUri!.ToString().Contains("/v1/console/card-template-pairs") &&
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
            "card_template_pairs": [],
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
            "card_template_pairs": [],
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

    [Test]
    public async Task CreatePassTemplatePairAsync_PostsAndReturnsPair()
    {
        var json = """
        {
            "id": "pair_new",
            "ex_id": "pair_new",
            "name": "New Badge Pair",
            "created_at": "2025-04-15T12:00:00Z",
            "ios_template": { "id": "tmpl_ios", "ex_id": "tmpl_ios", "name": "iOS Badge", "platform": "apple" },
            "android_template": { "id": "tmpl_android", "ex_id": "tmpl_android", "name": "Android Badge", "platform": "android" }
        }
        """;
        StubHttpResponse(json, HttpStatusCode.Created);

        var result = await _client.Console.CreatePassTemplatePairAsync(new CreatePassTemplatePairRequest
        {
            Name = "New Badge Pair",
            AppleCardTemplateId = "tmpl_ios",
            GoogleCardTemplateId = "tmpl_android"
        });

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo("pair_new"));
        Assert.That(result.ExId, Is.EqualTo("pair_new"));
        Assert.That(result.Name, Is.EqualTo("New Badge Pair"));
        Assert.That(result.IosTemplate.Platform, Is.EqualTo("apple"));
        Assert.That(result.AndroidTemplate.Platform, Is.EqualTo("android"));

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Post &&
            req.RequestUri!.ToString().Contains("/v1/console/card-template-pairs")
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

    #region IosPreflightAsync

    [Test]
    public async Task IosPreflightAsync_ShouldReturnPreflightIdentifiers()
    {
        var json = """
        {
            "provisioningCredentialIdentifier": "prov-cred-123",
            "sharingInstanceIdentifier": "sharing-456",
            "cardTemplateIdentifier": "tmpl-789",
            "environmentIdentifier": "env-abc"
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.IosPreflightAsync("tmpl-789", "pass-456");

        Assert.That(result.ProvisioningCredentialIdentifier, Is.EqualTo("prov-cred-123"));
        Assert.That(result.SharingInstanceIdentifier, Is.EqualTo("sharing-456"));
        Assert.That(result.CardTemplateIdentifier, Is.EqualTo("tmpl-789"));
        Assert.That(result.EnvironmentIdentifier, Is.EqualTo("env-abc"));

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Post &&
            req.RequestUri!.ToString().Contains("/v1/console/card-templates/tmpl-789/ios_preflight")
        )), Times.Once);
    }

    #endregion

    #region WebhooksService

    [Test]
    public async Task WebhooksListAsync_ShouldReturnWebhooks()
    {
        var json = """
        {
            "webhooks": [
                {
                    "id": "wh_1",
                    "name": "My Webhook",
                    "url": "https://example.com/webhook",
                    "auth_method": "bearer_token",
                    "subscribed_events": ["ag.access_pass.issued"],
                    "created_at": "2025-03-01T00:00:00Z"
                }
            ],
            "pagination": { "current_page": 1, "per_page": 50, "total_pages": 1, "total_count": 1 }
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.Webhooks.ListAsync();

        Assert.That(result.Webhooks, Has.Count.EqualTo(1));
        Assert.That(result.Webhooks[0].Id, Is.EqualTo("wh_1"));
        Assert.That(result.Webhooks[0].Name, Is.EqualTo("My Webhook"));
        Assert.That(result.Webhooks[0].Url, Is.EqualTo("https://example.com/webhook"));
        Assert.That(result.Webhooks[0].AuthMethod, Is.EqualTo("bearer_token"));
        Assert.That(result.Webhooks[0].SubscribedEvents, Has.Count.EqualTo(1));
        Assert.That(result.Pagination.TotalCount, Is.EqualTo(1));
    }

    [Test]
    public async Task WebhooksCreateAsync_ShouldCreateWebhook()
    {
        var json = """
        {
            "id": "wh_new",
            "name": "New Webhook",
            "url": "https://example.com/hook",
            "auth_method": "bearer_token",
            "subscribed_events": ["ag.access_pass.issued"],
            "created_at": "2025-03-01T00:00:00Z",
            "private_key": "secret-key-123"
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.Webhooks.CreateAsync(new CreateWebhookRequest
        {
            Name = "New Webhook",
            Url = "https://example.com/hook",
            SubscribedEvents = new List<string> { "ag.access_pass.issued" }
        });

        Assert.That(result.Id, Is.EqualTo("wh_new"));
        Assert.That(result.PrivateKey, Is.EqualTo("secret-key-123"));

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Post &&
            req.RequestUri!.ToString().Contains("/v1/console/webhooks")
        )), Times.Once);
    }

    [Test]
    public async Task WebhooksDeleteAsync_ShouldDeleteWebhook()
    {
        StubHttpResponse("{}");

        await _client.Console.Webhooks.DeleteAsync("wh_123");

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Delete &&
            req.RequestUri!.ToString().Contains("/v1/console/webhooks/wh_123")
        )), Times.Once);
    }

    #endregion

    #region HIDOrgsService

    [Test]
    public async Task HIDOrgsCreateAsync_ShouldCreateOrg()
    {
        var json = """
        {
            "id": "org_1",
            "name": "My Org",
            "slug": "my-org",
            "first_name": "Ada",
            "last_name": "Lovelace",
            "phone": "+1-555-0000",
            "full_address": "1 Main St, NY NY",
            "status": "pending",
            "created_at": "2025-03-01T00:00:00Z"
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.HID.Orgs.CreateAsync(new CreateHIDOrgRequest
        {
            Name = "My Org",
            FullAddress = "1 Main St, NY NY",
            Phone = "+1-555-0000",
            FirstName = "Ada",
            LastName = "Lovelace"
        });

        Assert.That(result.Id, Is.EqualTo("org_1"));
        Assert.That(result.Name, Is.EqualTo("My Org"));
        Assert.That(result.Slug, Is.EqualTo("my-org"));
        Assert.That(result.FirstName, Is.EqualTo("Ada"));
        Assert.That(result.Status, Is.EqualTo("pending"));

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Post &&
            req.RequestUri!.ToString().Contains("/v1/console/hid/orgs")
        )), Times.Once);
    }

    [Test]
    public async Task HIDOrgsListAsync_ShouldReturnOrgs()
    {
        var json = """
        [
            {
                "id": "org_1",
                "name": "Org One",
                "slug": "org-one",
                "first_name": "Ada",
                "last_name": "Lovelace",
                "phone": "+1-555-0000",
                "full_address": "1 Main St",
                "status": "active",
                "created_at": "2025-03-01T00:00:00Z"
            },
            {
                "id": "org_2",
                "name": "Org Two",
                "slug": "org-two",
                "first_name": "Bob",
                "last_name": "Smith",
                "phone": "+1-555-1111",
                "full_address": "2 Main St",
                "status": "pending",
                "created_at": "2025-03-02T00:00:00Z"
            }
        ]
        """;
        StubHttpResponse(json);

        var result = await _client.Console.HID.Orgs.ListAsync();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Id, Is.EqualTo("org_1"));
        Assert.That(result[0].Name, Is.EqualTo("Org One"));
        Assert.That(result[1].Id, Is.EqualTo("org_2"));
        Assert.That(result[1].Status, Is.EqualTo("pending"));
    }

    [Test]
    public async Task HIDOrgsListAsync_ShouldHandleEmptyList()
    {
        StubHttpResponse("[]");

        var result = await _client.Console.HID.Orgs.ListAsync();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task HIDOrgsActivateAsync_ShouldCompleteRegistration()
    {
        var json = """
        {
            "id": "org_1",
            "name": "My Org",
            "slug": "my-org",
            "first_name": "Ada",
            "last_name": "Lovelace",
            "phone": "+1-555-0000",
            "full_address": "1 Main St",
            "status": "active",
            "created_at": "2025-03-01T00:00:00Z"
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.HID.Orgs.ActivateAsync(new CompleteHIDOrgRequest
        {
            Email = "admin@example.com",
            Password = "hid-password-123"
        });

        Assert.That(result.Id, Is.EqualTo("org_1"));
        Assert.That(result.Name, Is.EqualTo("My Org"));
        Assert.That(result.Status, Is.EqualTo("active"));

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Post &&
            req.RequestUri!.ToString().Contains("/v1/console/hid/orgs/activate")
        )), Times.Once);
    }

    #endregion

    #region LandingPages

    [Test]
    public async Task ListLandingPagesAsync_ShouldReturnLandingPages()
    {
        var json = """
        [
            {
                "id": "lp_1",
                "name": "Miami Office",
                "created_at": "2025-03-01T00:00:00Z",
                "kind": "universal",
                "password_protected": false,
                "logo_url": "https://example.com/logo.png"
            },
            {
                "id": "lp_2",
                "name": "NYC Office",
                "created_at": "2025-03-02T00:00:00Z",
                "kind": "universal",
                "password_protected": true,
                "logo_url": null
            }
        ]
        """;
        StubHttpResponse(json);

        var result = await _client.Console.ListLandingPagesAsync();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Id, Is.EqualTo("lp_1"));
        Assert.That(result[0].Name, Is.EqualTo("Miami Office"));
        Assert.That(result[0].Kind, Is.EqualTo("universal"));
        Assert.That(result[0].PasswordProtected, Is.False);
        Assert.That(result[0].LogoUrl, Is.EqualTo("https://example.com/logo.png"));
        Assert.That(result[1].PasswordProtected, Is.True);
        Assert.That(result[1].LogoUrl, Is.Null);

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Get &&
            req.RequestUri!.ToString().Contains("/v1/console/landing-pages")
        )), Times.Once);
    }

    [Test]
    public async Task ListLandingPagesAsync_ShouldHandleEmptyList()
    {
        StubHttpResponse("[]");

        var result = await _client.Console.ListLandingPagesAsync();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task CreateLandingPageAsync_ShouldCreateLandingPage()
    {
        var json = """
        {
            "id": "lp_new",
            "name": "Miami Office Access Pass",
            "created_at": "2025-03-01T00:00:00Z",
            "kind": "universal",
            "password_protected": false,
            "logo_url": null
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.CreateLandingPageAsync(new CreateLandingPageRequest
        {
            Name = "Miami Office Access Pass",
            Kind = "universal",
            AdditionalText = "Welcome to the Miami Office",
            BgColor = "#f1f5f9",
            AllowImmediateDownload = true
        });

        Assert.That(result.Id, Is.EqualTo("lp_new"));
        Assert.That(result.Name, Is.EqualTo("Miami Office Access Pass"));
        Assert.That(result.Kind, Is.EqualTo("universal"));

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Post &&
            req.RequestUri!.ToString().Contains("/v1/console/landing-pages")
        )), Times.Once);
    }

    [Test]
    public async Task UpdateLandingPageAsync_ShouldUpdateLandingPage()
    {
        var json = """
        {
            "id": "lp_1",
            "name": "Updated Miami Office",
            "created_at": "2025-03-01T00:00:00Z",
            "kind": "universal",
            "password_protected": false,
            "logo_url": null
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.UpdateLandingPageAsync("lp_1", new UpdateLandingPageRequest
        {
            Name = "Updated Miami Office",
            AdditionalText = "Welcome! Tap below to get your access pass.",
            BgColor = "#e2e8f0"
        });

        Assert.That(result.Id, Is.EqualTo("lp_1"));
        Assert.That(result.Name, Is.EqualTo("Updated Miami Office"));

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Put &&
            req.RequestUri!.ToString().Contains("/v1/console/landing-pages/lp_1")
        )), Times.Once);
    }

    #endregion

    #region CredentialProfilesService

    [Test]
    public async Task CredentialProfilesListAsync_ShouldReturnProfiles()
    {
        var json = """
        [
            {
                "id": "cp_1",
                "aid": "F0394148",
                "name": "Main Office Profile",
                "apple_id": "apple-123",
                "created_at": "2025-03-01T00:00:00Z",
                "card_storage": "2K",
                "keys": [
                    { "ex_id": "key_1", "label": "Master Key", "keys_diversified": true, "source_key_index": 0 }
                ],
                "files": [
                    { "ex_id": "file_1", "file_type": "Standard", "file_size": 32, "communication_settings": "Full", "read_rights": "key_1", "write_rights": "key_1", "read_write_rights": "key_1", "change_rights": "key_1" }
                ]
            }
        ]
        """;
        StubHttpResponse(json);

        var result = await _client.Console.CredentialProfiles.ListAsync();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo("cp_1"));
        Assert.That(result[0].Aid, Is.EqualTo("F0394148"));
        Assert.That(result[0].Name, Is.EqualTo("Main Office Profile"));
        Assert.That(result[0].AppleId, Is.EqualTo("apple-123"));
        Assert.That(result[0].CardStorage, Is.EqualTo("2K"));
        Assert.That(result[0].Keys, Has.Count.EqualTo(1));
        Assert.That(result[0].Keys[0].ExId, Is.EqualTo("key_1"));
        Assert.That(result[0].Keys[0].KeysDiversified, Is.True);
        Assert.That(result[0].Files, Has.Count.EqualTo(1));
        Assert.That(result[0].Files[0].FileType, Is.EqualTo("Standard"));

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Get &&
            req.RequestUri!.ToString().Contains("/v1/console/credential-profiles")
        )), Times.Once);
    }

    [Test]
    public async Task CredentialProfilesListAsync_ShouldHandleEmptyList()
    {
        StubHttpResponse("[]");

        var result = await _client.Console.CredentialProfiles.ListAsync();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task CredentialProfilesCreateAsync_ShouldCreateProfile()
    {
        var json = """
        {
            "id": "cp_new",
            "aid": "F0394148",
            "name": "Main Office Profile",
            "apple_id": null,
            "created_at": "2025-03-01T00:00:00Z",
            "card_storage": "2K",
            "keys": [
                { "ex_id": "key_1", "label": "Master Key", "keys_diversified": false, "source_key_index": null },
                { "ex_id": "key_2", "label": "Read Key", "keys_diversified": false, "source_key_index": null }
            ],
            "files": []
        }
        """;
        StubHttpResponse(json);

        var result = await _client.Console.CredentialProfiles.CreateAsync(new CreateCredentialProfileRequest
        {
            Name = "Main Office Profile",
            AppName = "KEY-ID-main",
            Keys = new[]
            {
                new KeyParam { Value = "your_32_char_hex_master_key_here" },
                new KeyParam { Value = "your_32_char_hex__read_key__here" }
            }
        });

        Assert.That(result.Id, Is.EqualTo("cp_new"));
        Assert.That(result.Aid, Is.EqualTo("F0394148"));
        Assert.That(result.Name, Is.EqualTo("Main Office Profile"));
        Assert.That(result.Keys, Has.Count.EqualTo(2));

        _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(req =>
            req.Method == HttpMethod.Post &&
            req.RequestUri!.ToString().Contains("/v1/console/credential-profiles")
        )), Times.Once);
    }

    #endregion

    #region AccessCard New Fields

    [Test]
    public async Task AccessCard_ShouldDeserializeNewFields()
    {
        var json = """
        {
            "id": "card_1",
            "install_url": "https://install.example.com/card_1",
            "state": "active",
            "full_name": "Jane Doe",
            "organization_name": "Acme Corp",
            "title": "Engineering Manager",
            "department": "Engineering",
            "location": "San Francisco",
            "site_name": "HQ Building A",
            "workstation": "4F-207",
            "mail_stop": "MS-401",
            "company_address": "123 Main St, San Francisco, CA 94105"
        }
        """;
        StubHttpResponse(json);

        var result = await _client.AccessCards.GetAsync("card_1");

        Assert.That(result.OrganizationName, Is.EqualTo("Acme Corp"));
        Assert.That(result.Title, Is.EqualTo("Engineering Manager"));
        Assert.That(result.Department, Is.EqualTo("Engineering"));
        Assert.That(result.Location, Is.EqualTo("San Francisco"));
        Assert.That(result.SiteName, Is.EqualTo("HQ Building A"));
        Assert.That(result.Workstation, Is.EqualTo("4F-207"));
        Assert.That(result.MailStop, Is.EqualTo("MS-401"));
        Assert.That(result.CompanyAddress, Is.EqualTo("123 Main St, San Francisco, CA 94105"));
    }

    [Test]
    public async Task AccessCard_NewFieldsAreNullWhenAbsent()
    {
        var json = """
        {
            "id": "card_2",
            "install_url": "https://install.example.com/card_2",
            "state": "active",
            "full_name": "John Smith"
        }
        """;
        StubHttpResponse(json);

        var result = await _client.AccessCards.GetAsync("card_2");

        Assert.That(result.OrganizationName, Is.Null);
        Assert.That(result.Department, Is.Null);
        Assert.That(result.Location, Is.Null);
        Assert.That(result.SiteName, Is.Null);
        Assert.That(result.Workstation, Is.Null);
        Assert.That(result.MailStop, Is.Null);
        Assert.That(result.CompanyAddress, Is.Null);
    }

    #endregion
}
