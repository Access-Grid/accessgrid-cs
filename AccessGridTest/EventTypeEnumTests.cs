namespace AccessGridTest;

using System.Text.Json;
using System.Text.Json.Serialization;
using AccessGrid;
using NUnit.Framework;

/// <summary>
/// Pins every event-type enum member to the exact event name the server sends.
/// The expected strings below are the contract — if one of these fails, either the
/// enum drifted or the server renamed an event. Do not "fix" a failure by editing
/// the expectation without checking what the server actually emits.
/// </summary>
[TestFixture]
public class EventTypeEnumTests
{
    private JsonSerializerOptions _jsonOptions;

    [SetUp]
    public void SetUp()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };
    }

    [TestCase(AccessPassEventType.Issued, "ag.access_pass.issued")]
    [TestCase(AccessPassEventType.Viewed, "ag.access_pass.viewed")]
    [TestCase(AccessPassEventType.Updated, "ag.access_pass.updated")]
    [TestCase(AccessPassEventType.Suspended, "ag.access_pass.suspended")]
    [TestCase(AccessPassEventType.Resumed, "ag.access_pass.resumed")]
    [TestCase(AccessPassEventType.Unlinked, "ag.access_pass.unlinked")]
    [TestCase(AccessPassEventType.Deleted, "ag.access_pass.deleted")]
    [TestCase(AccessPassEventType.DeviceAdded, "ag.access_pass.devices.added")]
    [TestCase(AccessPassEventType.DeviceRemoved, "ag.access_pass.devices.removed")]
    [TestCase(AccessPassEventType.Expired, "ag.access_pass.expired")]
    [TestCase(AccessPassEventType.Failed, "ag.access_pass.failed")]
    [TestCase(AccessPassEventType.Activated, "ag.access_pass.activated")]
    [TestCase(AccessPassEventType.Renewed, "ag.access_pass.renewed")]
    [TestCase(AccessPassEventType.DeviceSuspended, "ag.access_pass.devices.suspended")]
    [TestCase(AccessPassEventType.DeviceResumed, "ag.access_pass.devices.resumed")]
    public void AccessPassEventType_MatchesWireName(AccessPassEventType value, string expected)
        => AssertRoundTrip(value, expected);

    [TestCase(CardTemplateEventType.Created, "ag.card_template.created")]
    [TestCase(CardTemplateEventType.Updated, "ag.card_template.updated")]
    [TestCase(CardTemplateEventType.RequestPublishing, "ag.card_template.requested_publishing")]
    [TestCase(CardTemplateEventType.Published, "ag.card_template.published")]
    [TestCase(CardTemplateEventType.Deleted, "ag.card_template.deleted")]
    public void CardTemplateEventType_MatchesWireName(CardTemplateEventType value, string expected)
        => AssertRoundTrip(value, expected);

    [TestCase(LandingPageEventType.Created, "ag.landing_page.created")]
    [TestCase(LandingPageEventType.Updated, "ag.landing_page.updated")]
    [TestCase(LandingPageEventType.AttachedToTemplate, "ag.landing_page.attached_to_template")]
    public void LandingPageEventType_MatchesWireName(LandingPageEventType value, string expected)
        => AssertRoundTrip(value, expected);

    [TestCase(CredentialProfileEventType.Created, "ag.credential_profile.created")]
    [TestCase(CredentialProfileEventType.Deleted, "ag.credential_profile.deleted")]
    [TestCase(CredentialProfileEventType.AttachedToTemplate, "ag.credential_profile.attached_to_template")]
    public void CredentialProfileEventType_MatchesWireName(CredentialProfileEventType value, string expected)
        => AssertRoundTrip(value, expected);

    [TestCase(HIDOrgEventType.Created, "ag.hid_org.created")]
    [TestCase(HIDOrgEventType.Activated, "ag.hid_org.activated")]
    public void HIDOrgEventType_MatchesWireName(HIDOrgEventType value, string expected)
        => AssertRoundTrip(value, expected);

    [TestCase(AccountBalanceEventType.Low, "ag.account_balance.low")]
    public void AccountBalanceEventType_MatchesWireName(AccountBalanceEventType value, string expected)
        => AssertRoundTrip(value, expected);

    [TestCase(WebhookEventType.CertExpiring, "ag.webhook.cert_expiring")]
    public void WebhookEventType_MatchesWireName(WebhookEventType value, string expected)
        => AssertRoundTrip(value, expected);

    [TestCase(CardTemplatePairEventType.Created, "ag.card_template_pair.created")]
    public void CardTemplatePairEventType_MatchesWireName(CardTemplatePairEventType value, string expected)
        => AssertRoundTrip(value, expected);

    /// <summary>
    /// Guards the tables above — a member added without a matching test case fails here.
    /// </summary>
    [TestCase(typeof(AccessPassEventType), 15)]
    [TestCase(typeof(CardTemplateEventType), 5)]
    [TestCase(typeof(LandingPageEventType), 3)]
    [TestCase(typeof(CredentialProfileEventType), 3)]
    [TestCase(typeof(HIDOrgEventType), 2)]
    [TestCase(typeof(AccountBalanceEventType), 1)]
    [TestCase(typeof(WebhookEventType), 1)]
    [TestCase(typeof(CardTemplatePairEventType), 1)]
    public void EventTypeEnum_HasExpectedMemberCount(Type enumType, int expected)
        => Assert.That(Enum.GetValues(enumType), Has.Length.EqualTo(expected));

    private void AssertRoundTrip<T>(T value, string expected) where T : struct, Enum
    {
        var json = JsonSerializer.Serialize(value, _jsonOptions);

        Assert.Multiple(() =>
        {
            Assert.That(json, Is.EqualTo($"\"{expected}\""));
            Assert.That(JsonSerializer.Deserialize<T>(json, _jsonOptions), Is.EqualTo(value));
        });
    }
}
