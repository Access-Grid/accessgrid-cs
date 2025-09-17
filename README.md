# AccessGrid C# SDK

Official C# SDK for interacting with the AccessGrid API.

## Installation

```
Install-Package accessgrid -Version 1.0.0
```

## Authentication

The SDK uses a dual authentication mechanism:

1. A static account ID sent in the `X-ACCT-ID` header
2. A shared secret scheme to authenticate every API request with a signed payload in the `X-PAYLOAD-SIG` header

You can find both keys in your AccessGrid console on the API keys page.

## Usage

### Initializing the Client

```csharp
using AccessGrid;
using System;

// Get credentials from environment variables
var accountId = Environment.GetEnvironmentVariable("ACCESSGRID_ACCOUNT_ID");
var secretKey = Environment.GetEnvironmentVariable("ACCESSGRID_SECRET_KEY");

// Initialize the client (default HTTP client)
using var client = new AccessGridClient(accountId, secretKey);

// Or with custom HTTP client for testing/mocking
var httpClient = new HttpClientWrapper(new HttpClient());
using var client = new AccessGridClient(accountId, secretKey, httpClient);
```

### Listing NFC Keys

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task ListCardsAsync()
{
    var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
    var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

    using var client = new AccessGridClient(accountId, secretKey);

    // Get filtered keys by template
    var templateKeys = await client.AccessCards.ListAsync(new ListKeysRequest
    {
        TemplateId = "0xd3adb00b5"
    });

    // Get filtered keys by state
    var activeKeys = await client.AccessCards.ListAsync(new ListKeysRequest
    {
        State = "active"
    });

    // Print keys
    foreach (var key in templateKeys)
    {
        Console.WriteLine($"Key ID: {key.Id}, Name: {key.FullName}, State: {key.State}");
    }
}
```

### Issuing an NFC Key

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task ProvisionCardAsync()
{
    var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
    var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

    using var client = new AccessGridClient(accountId, secretKey);

    var card = await client.AccessCards.ProvisionAsync(new ProvisionCardRequest
    {
        CardTemplateId = "0xd3adb00b5",
        EmployeeId = "123456789",
        TagId = "DDEADB33FB00B5",
        AllowOnMultipleDevices = true,
        FullName = "Employee name",
        Email = "employee@yourwebsite.com",
        PhoneNumber = "+19547212241",
        Classification = "full_time",
        StartDate = DateTime.UtcNow,
        ExpirationDate = DateTime.Parse("2025-02-22T21:04:03.664Z").ToUniversalTime(),
        EmployeePhoto = "[image_in_base64_encoded_format]"
    });

    Console.WriteLine($"Install URL: {card.Url}");
}
```

### Updating an NFC Key

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task UpdateCardAsync()
{
   var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
   var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

   using var client = new AccessGridClient(accountId, secretKey);

   await client.AccessCards.UpdateAsync(new UpdateCardRequest
   {
       CardId = "0xc4rd1d",
       EmployeeId = "987654321",
       FullName = "Updated Employee Name",
       Classification = "contractor",
       ExpirationDate = DateTime.UtcNow.AddMonths(3),
       EmployeePhoto = "[image_in_base64_encoded_format]"
   });

   Console.WriteLine("Card updated successfully");
}
```

### Suspending an NFC Key

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task SuspendCardAsync()
{
   var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
   var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

   using var client = new AccessGridClient(accountId, secretKey);

   await client.AccessCards.SuspendAsync("0xc4rd1d");

   Console.WriteLine("Card suspended successfully");
}
```

### Resuming an NFC Key

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task ResumeCardAsync()
{
   var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
   var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

   using var client = new AccessGridClient(accountId, secretKey);

   await client.AccessCards.ResumeAsync("0xc4rd1d");

   Console.WriteLine("Card resumed successfully");
}
```

### Unlinking an NFC Key

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task UnlinkCardAsync()
{
   var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
   var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

   using var client = new AccessGridClient(accountId, secretKey);

   await client.AccessCards.UnlinkAsync("0xc4rd1d");

   Console.WriteLine("Card unlinked successfully");
}
```

## Enterprise Features

### Creating a Card Template

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task CreateTemplateAsync()
{
   var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
   var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

   using var client = new AccessGridClient(accountId, secretKey);

   var template = await client.Console.CreateTemplateAsync(new CreateTemplateRequest
   {
       Name = "Employee NFC key",
       Platform = "apple",
       UseCase = "employee_badge",
       Protocol = "desfire",
       AllowOnMultipleDevices = true,
       WatchCount = 2,
       IPhoneCount = 3,
       Design = new TemplateDesign
       {
           BackgroundColor = "#FFFFFF",
           LabelColor = "#000000",
           LabelSecondaryColor = "#333333",
           BackgroundImage = "[image_in_base64_encoded_format]",
           LogoImage = "[image_in_base64_encoded_format]",
           IconImage = "[image_in_base64_encoded_format]"
       },
       SupportInfo = new SupportInfo
       {
           SupportUrl = "https://help.yourcompany.com",
           SupportPhoneNumber = "+1-555-123-4567",
           SupportEmail = "support@yourcompany.com",
           PrivacyPolicyUrl = "https://yourcompany.com/privacy",
           TermsAndConditionsUrl = "https://yourcompany.com/terms"
       }
   });

   Console.WriteLine($"Template created successfully: {template.Id}");
}
```

### Updating a Card Template

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task UpdateTemplateAsync()
{
   var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
   var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

   using var client = new AccessGridClient(accountId, secretKey);

   var template = await client.Console.UpdateTemplateAsync(
     new UpdateTemplateRequest
     {
         CardTemplateId = "0xd3adb00b5",
         Name = "Updated Employee NFC key",
         AllowOnMultipleDevices = true,
         WatchCount = 2,
         IPhoneCount = 3,
         SupportInfo = new SupportInfo
         {
             SupportUrl = "https://help.yourcompany.com",
             SupportPhoneNumber = "+1-555-123-4567",
             SupportEmail = "support@yourcompany.com",
             PrivacyPolicyUrl = "https://yourcompany.com/privacy",
             TermsAndConditionsUrl = "https://yourcompany.com/terms"
         }
     }
   );

   Console.WriteLine($"Template updated successfully: {template.Id}");
}
```

### Reading a Card Template

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task ReadTemplateAsync()
{
   var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
   var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

   using var client = new AccessGridClient(accountId, secretKey);

   var template = await client.Console.ReadTemplateAsync("0xd3adb00b5");

   Console.WriteLine($"Template ID: {template.Id}");
   Console.WriteLine($"Name: {template.Name}");
   Console.WriteLine($"Platform: {template.Platform}");
   Console.WriteLine($"Protocol: {template.Protocol}");
   Console.WriteLine($"Multi-device: {template.AllowOnMultipleDevices}");
}
```

### Reading Event Logs

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task GetEventLogAsync()
{
   var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
   var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

   using var client = new AccessGridClient(accountId, secretKey);

   var events = await client.Console.EventLogAsync(
       "0xd3adb00b5",
       new EventLogFilters
       {
           Device = "mobile",
           StartDate = DateTime.UtcNow.AddDays(-30),
           EndDate = DateTime.UtcNow,
           EventType = "install"
       });

   foreach (var evt in events)
   {
       Console.WriteLine($"Event: {evt.Type} at {evt.Timestamp} by {evt.UserId}");
   }
}
```

## Testing and Mocking

The AccessGrid SDK is designed to be testable with support for dependency injection and mocking. This allows you to write unit tests without making actual API calls.

### Setting Up Test Dependencies

For testing, you'll need to install a mocking framework. We recommend Moq with either xUnit or NUnit:

```
Install-Package Moq
Install-Package Microsoft.NET.Test.Sdk

# For xUnit
Install-Package xunit
Install-Package xunit.runner.visualstudio

# For NUnit
Install-Package NUnit
Install-Package NUnit3TestAdapter
```

### Mocking the API Service

The SDK provides the `IApiService` interface which can be easily mocked for testing:

```csharp
using AccessGrid;
using Moq;
using System.Threading.Tasks;
using Xunit;

public class AccessCardsServiceTests
{
    [Fact]
    public async Task IssueAsync_ShouldReturnAccessCard_WhenRequestIsValid()
    {
        // Arrange
        var mockApiService = new Mock<IApiService>();
        var expectedCard = new AccessCard
        {
            Id = "test-card-id",
            FullName = "Test User",
            State = "active"
        };

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
        Assert.Equal(expectedCard.Id, result.Id);
        Assert.Equal(expectedCard.FullName, result.FullName);
        mockApiService.Verify(x => x.PostAsync<AccessCard>("/v1/key-cards", request), Times.Once);
    }
}
```

### Mocking HTTP Client Operations

For more granular control, you can mock the HTTP client wrapper. Here's an example using NUnit:

```csharp
using AccessGrid;
using Moq;
using NUnit;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
```

### Integration Testing with Dependency Injection

You can also use dependency injection containers for integration testing:

```csharp
using AccessGrid;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading.Tasks;
using Xunit;

public class IntegrationTests
{
    [Fact]
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
        Assert.NotNull(result);
        mockApiService.Verify(x => x.GetAsync<KeysListResponse>("/v1/key-cards", It.IsAny<Dictionary<string, string>>()), Times.Once);
    }
}
```

### Testing Best Practices

1. **Mock at the right level**: Use `IApiService` for testing business logic, `IHttpClientWrapper` for testing HTTP-specific behavior
2. **Verify interactions**: Use `Verify()` to ensure the correct API endpoints are called
3. **Test error scenarios**: Mock HTTP errors and API exceptions to test error handling
4. **Use realistic test data**: Create test data that matches real API responses

## License

MIT