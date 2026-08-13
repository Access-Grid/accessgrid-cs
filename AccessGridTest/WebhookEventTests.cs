namespace AccessGridTest;

using System.Text.Json;
using System.Text.Json.Serialization;
using AccessGrid;
using NUnit.Framework;

[TestFixture]
public class WebhookEventTests
{
    private JsonSerializerOptions _jsonOptions;

    [SetUp]
    public void SetUp()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    [Test]
    public void Deserialize_LandingPageEvent()
    {
        var json = """
        {
            "landing_page_id": "lp_123"
        }
        """;

        var evt = JsonSerializer.Deserialize<LandingPageEvent>(json, _jsonOptions)!;

        Assert.That(evt.Id, Is.EqualTo("lp_123"));
    }

    [Test]
    public void Deserialize_CredentialProfileEvent()
    {
        var json = """
        {
            "credential_profile_id": "cp_123"
        }
        """;

        var evt = JsonSerializer.Deserialize<CredentialProfileEvent>(json, _jsonOptions)!;

        Assert.That(evt.Id, Is.EqualTo("cp_123"));
    }

    [Test]
    public void Deserialize_HIDOrgEvent()
    {
        var json = """
        {
            "account_org_id": "org_123",
            "slug": "acme-corp",
            "name": "Acme Corp"
        }
        """;

        var evt = JsonSerializer.Deserialize<HIDOrgEvent>(json, _jsonOptions)!;

        Assert.Multiple(() =>
        {
            Assert.That(evt.Id, Is.EqualTo("org_123"));
            Assert.That(evt.Slug, Is.EqualTo("acme-corp"));
            Assert.That(evt.Name, Is.EqualTo("Acme Corp"));
        });
    }

    [Test]
    public void Deserialize_AccountBalanceEvent()
    {
        var json = """
        {
            "account_id": "acct_123",
            "organization_name": "Acme Corp",
            "current_balance": 50.0,
            "threshold": 100.0,
            "amount_below_threshold": 50.0
        }
        """;

        var evt = JsonSerializer.Deserialize<AccountBalanceEvent>(json, _jsonOptions)!;

        Assert.Multiple(() =>
        {
            Assert.That(evt.Id, Is.EqualTo("acct_123"));
            Assert.That(evt.OrganizationName, Is.EqualTo("Acme Corp"));
            Assert.That(evt.CurrentBalance, Is.EqualTo(50.0m));
            Assert.That(evt.Threshold, Is.EqualTo(100.0m));
            Assert.That(evt.AmountBelowThreshold, Is.EqualTo(50.0m));
        });
    }

    [Test]
    public void Deserialize_WebhookEvent()
    {
        var json = """
        {
            "webhook_id": "wh_123",
            "webhook_name": "Production endpoint",
            "cert_expires_at": "2026-09-11T00:00:00Z",
            "days_until_expiration": 30
        }
        """;

        var evt = JsonSerializer.Deserialize<WebhookEvent>(json, _jsonOptions)!;

        Assert.Multiple(() =>
        {
            Assert.That(evt.Id, Is.EqualTo("wh_123"));
            Assert.That(evt.Name, Is.EqualTo("Production endpoint"));
            Assert.That(evt.CertExpiresAt, Is.EqualTo(DateTimeOffset.Parse("2026-09-11T00:00:00Z")));
            Assert.That(evt.DaysUntilExpiration, Is.EqualTo(30));
        });
    }

    [Test]
    public void Deserialize_CardTemplatePairEvent()
    {
        var json = """
        {
            "card_template_pair_id": "ctp_123",
            "name": "Corporate Badge",
            "ios_template": {
                "id": "ct_ios",
                "name": "Corporate Badge iOS",
                "platform": "apple",
                "protocol": "desfire"
            },
            "android_template": {
                "id": "ct_android",
                "name": "Corporate Badge Android",
                "platform": "android",
                "protocol": "smart_tap"
            }
        }
        """;

        var evt = JsonSerializer.Deserialize<CardTemplatePairEvent>(json, _jsonOptions)!;

        Assert.Multiple(() =>
        {
            Assert.That(evt.Id, Is.EqualTo("ctp_123"));
            Assert.That(evt.Name, Is.EqualTo("Corporate Badge"));
            Assert.That(evt.IosTemplate.Id, Is.EqualTo("ct_ios"));
            Assert.That(evt.IosTemplate.Platform, Is.EqualTo(Platform.Apple));
            Assert.That(evt.IosTemplate.Protocol, Is.EqualTo(Protocol.DESFire));
            Assert.That(evt.AndroidTemplate.Id, Is.EqualTo("ct_android"));
            Assert.That(evt.AndroidTemplate.Platform, Is.EqualTo(Platform.Android));
            Assert.That(evt.AndroidTemplate.Protocol, Is.EqualTo(Protocol.SmartTap));
        });
    }

    [Test]
    public void Deserialize_OptionalFieldsAreNull_WhenAbsent()
    {
        var json = """
        {
            "account_org_id": "org_123"
        }
        """;

        var evt = JsonSerializer.Deserialize<HIDOrgEvent>(json, _jsonOptions)!;

        Assert.Multiple(() =>
        {
            Assert.That(evt.Id, Is.EqualTo("org_123"));
            Assert.That(evt.Slug, Is.Null);
            Assert.That(evt.Name, Is.Null);
        });
    }
}
