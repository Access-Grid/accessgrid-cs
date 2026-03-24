namespace AccessGridTest;

using System.Text.Json;
using System.Text.Json.Serialization;
using AccessGrid;
using NUnit.Framework;

[TestFixture]
public class AccessPassEventTests
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
    public void Deserialize_ShouldIncludeAppleUserId_WhenPresent()
    {
        var json = """
        {
            "id": "ap_123",
            "card_template_id": "ct_456",
            "state": "active",
            "full_name": "Jane Doe",
            "apple_user_id": "001234.abc567def890.1234",
            "card_templates": [],
            "devices": []
        }
        """;

        var evt = JsonSerializer.Deserialize<AccessPassEvent>(json, _jsonOptions);

        Assert.That(evt.AppleUserId, Is.EqualTo("001234.abc567def890.1234"));
    }

    [Test]
    public void Deserialize_ShouldHaveNullAppleUserId_WhenAbsent()
    {
        var json = """
        {
            "id": "ap_123",
            "card_template_id": "ct_456",
            "state": "active",
            "card_templates": [],
            "devices": []
        }
        """;

        var evt = JsonSerializer.Deserialize<AccessPassEvent>(json, _jsonOptions);

        Assert.That(evt.AppleUserId, Is.Null);
    }

    [Test]
    public void Deserialize_ShouldHaveNullAppleUserId_WhenExplicitlyNull()
    {
        var json = """
        {
            "id": "ap_123",
            "card_template_id": "ct_456",
            "state": "active",
            "apple_user_id": null,
            "card_templates": [],
            "devices": []
        }
        """;

        var evt = JsonSerializer.Deserialize<AccessPassEvent>(json, _jsonOptions);

        Assert.That(evt.AppleUserId, Is.Null);
    }
}
