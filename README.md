# AccessGrid C# SDK

Official C# SDK for interacting with the AccessGrid API.

## Installation

```
Install-Package accessgrid -Version 1.4.0
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

    // Get filtered keys by state
    var activeKeys = await client.AccessCards.ListAsync(new ListKeysRequest
    {
        TemplateId = "05d3adb00b5",
        State = "active"
    });

    // Print keys
    foreach (var key in activeKeys)
    {
        Console.WriteLine(key);
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
        CardTemplateId = "05d3adb00b5",
        EmployeeId = "123456789",
        TagId = "DDEADB33FB00B5",
        FullName = "Employee name",
        Email = "employee@yourwebsite.com",
        PhoneNumber = "+19547212241",
        Classification = "full_time",
        StartDate = DateTime.UtcNow,
        ExpirationDate = DateTime.Parse("2025-02-22T21:04:03.664Z").ToUniversalTime(),
        CardNumber = "1252",
        EmployeePhoto = "[image_in_base64_encoded_format]"
    });

    Console.WriteLine($"Install URL: {card.Url}");
}
```

### Getting an NFC Key

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task GetCardAsync()
{
    var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
    var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

    using var client = new AccessGridClient(accountId, secretKey);

    var card = await client.AccessCards.GetAsync("0xc4rd1d");

    Console.WriteLine($"Card ID: {card.Id}");
    Console.WriteLine($"State: {card.State}");
    Console.WriteLine($"Full Name: {card.FullName}");
    Console.WriteLine($"Install URL: {card.InstallUrl}");
    Console.WriteLine($"Expiration Date: {card.ExpirationDate}");
    Console.WriteLine($"Card Number: {card.CardNumber}");
    Console.WriteLine($"Site Code: {card.SiteCode}");
    Console.WriteLine($"Devices: {card.Devices.Count}");
    Console.WriteLine($"Metadata: {card.Metadata}");
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

   await client.AccessCards.UpdateAsync("OtrysOXjeXSIyd1", new UpdateCardRequest
   {
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

   await client.AccessCards.SuspendAsync("OtrysOXjeXSIyd1");

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

   await client.AccessCards.ResumeAsync("OtrysOXjeXSIyd1");

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

   await client.AccessCards.UnlinkAsync("OtrysOXjeXSIyd1");

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
       Name = "Employee Access Pass",
       Platform = "apple",
       UseCase = "employee_badge",
       Protocol = "desfire",
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
         Name = "Updated Employee Access Pass",
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
         TermsAndConditionsUrl = "https://yourcompany.com/terms"
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

### Listing Pass Template Pairs

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task ListPassTemplatePairsAsync()
{
   var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
   var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

   using var client = new AccessGridClient(accountId, secretKey);

   // List first page with default page size (50)
   var result = await client.Console.ListPassTemplatePairsAsync();

   foreach (var pair in result.PassTemplatePairs)
   {
       Console.WriteLine($"Pair: {pair.Name} ({pair.Id})");
       Console.WriteLine($"  iOS: {pair.IosTemplate?.Name ?? "none"}");
       Console.WriteLine($"  Android: {pair.AndroidTemplate?.Name ?? "none"}");
   }

   Console.WriteLine($"Page {result.Pagination.CurrentPage} of {result.Pagination.TotalPages}");

   // Or with pagination
   var page2 = await client.Console.ListPassTemplatePairsAsync(page: 2, perPage: 10);
}
```

### Getting Ledger Items

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task GetLedgerItemsAsync()
{
   var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
   var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

   using var client = new AccessGridClient(accountId, secretKey);

   // Get all ledger items
   var result = await client.Console.GetLedgerItemsAsync();

   foreach (var item in result.LedgerItems)
   {
       Console.WriteLine($"{item.CreatedAt} | {item.Kind} | {item.Amount} | {item.ExId}");

       if (item.AccessPass != null)
       {
           Console.WriteLine($"  Pass: {item.AccessPass.FullName} ({item.AccessPass.State})");

           if (item.AccessPass.PassTemplate != null)
               Console.WriteLine($"  Template: {item.AccessPass.PassTemplate.Name}");
       }
   }

   Console.WriteLine($"Page {result.Pagination.CurrentPage} of {result.Pagination.TotalPages}");

   // With date filters and pagination
   var filtered = await client.Console.GetLedgerItemsAsync(
       startDate: DateTime.UtcNow.AddDays(-30),
       endDate: DateTime.UtcNow,
       page: 1,
       perPage: 25
   );
}
```

### iOS In-App Provisioning Preflight

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task GenerateProvisioningCredentialsAsync()
{
    var client = new AccessGridClient(
        Environment.GetEnvironmentVariable("ACCOUNT_ID"),
        Environment.GetEnvironmentVariable("SECRET_KEY")
    );

    var response = await client.Console.IosPreflightAsync(
        cardTemplateId: "0xt3mp14t3-3x1d",
        accessPassExId: "0xp455-3x1d"
    );

    Console.WriteLine($"Provisioning Credential ID: {response.ProvisioningCredentialIdentifier}");
    Console.WriteLine($"Sharing Instance ID: {response.SharingInstanceIdentifier}");
    Console.WriteLine($"Card Template ID: {response.CardTemplateIdentifier}");
    Console.WriteLine($"Environment ID: {response.EnvironmentIdentifier}");
}
```

### Landing Pages

#### List Landing Pages

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task ListLandingPagesAsync()
{
    var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
    var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

    using var client = new AccessGridClient(accountId, secretKey);

    var landingPages = await client.Console.ListLandingPagesAsync();

    foreach (var page in landingPages)
    {
        Console.WriteLine($"ID: {page.Id}, Name: {page.Name}, Kind: {page.Kind}");
        Console.WriteLine($"  Password Protected: {page.PasswordProtected}");
        if (page.LogoUrl != null)
            Console.WriteLine($"  Logo URL: {page.LogoUrl}");
    }
}
```

#### Create a Landing Page

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task CreateLandingPageAsync()
{
    var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
    var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

    using var client = new AccessGridClient(accountId, secretKey);

    var landingPage = await client.Console.CreateLandingPageAsync(new CreateLandingPageRequest
    {
        Name = "Miami Office Access Pass",
        Kind = "universal",
        AdditionalText = "Welcome to the Miami Office",
        BgColor = "#f1f5f9",
        AllowImmediateDownload = true
    });

    Console.WriteLine($"Landing page created: {landingPage.Id}");
    Console.WriteLine($"Name: {landingPage.Name}, Kind: {landingPage.Kind}");
}
```

#### Update a Landing Page

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task UpdateLandingPageAsync()
{
    var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
    var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

    using var client = new AccessGridClient(accountId, secretKey);

    var landingPage = await client.Console.UpdateLandingPageAsync("0xlandingpage1d", new UpdateLandingPageRequest
    {
        Name = "Updated Miami Office Access Pass",
        AdditionalText = "Welcome! Tap below to get your access pass.",
        BgColor = "#e2e8f0"
    });

    Console.WriteLine($"Landing page updated: {landingPage.Id}");
    Console.WriteLine($"Name: {landingPage.Name}");
}
```

### Credential Profiles

#### List Credential Profiles

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task ListProfilesAsync()
{
    var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
    var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

    using var client = new AccessGridClient(accountId, secretKey);

    var profiles = await client.Console.CredentialProfiles.ListAsync();

    foreach (var profile in profiles)
    {
        Console.WriteLine($"ID: {profile.Id}, Name: {profile.Name}, AID: {profile.Aid}");
    }
}
```

#### Create a Credential Profile

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task CreateProfileAsync()
{
    var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
    var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

    using var client = new AccessGridClient(accountId, secretKey);

    var profile = await client.Console.CredentialProfiles.CreateAsync(new CreateCredentialProfileRequest
    {
        Name = "Main Office Profile",
        AppName = "KEY-ID-main",
        Keys = new[]
        {
            new KeyParam { Value = "your_32_char_hex_master_key_here" },
            new KeyParam { Value = "your_32_char_hex__read_key__here" }
        }
    });

    Console.WriteLine($"Profile created: {profile.Id}");
    Console.WriteLine($"AID: {profile.Aid}");
}
```

### Webhooks

```csharp
using AccessGrid;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public async Task ManageWebhooksAsync()
{
    var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
    var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

    using var client = new AccessGridClient(accountId, secretKey);

    // List webhooks
    var webhooks = await client.Console.Webhooks.ListAsync();
    foreach (var wh in webhooks.Webhooks)
    {
        Console.WriteLine($"Webhook: {wh.Name} ({wh.Id})");
    }

    // Create a webhook
    var newWebhook = await client.Console.Webhooks.CreateAsync(new CreateWebhookRequest
    {
        Name = "My Webhook",
        Url = "https://example.com/webhook",
        SubscribedEvents = new List<string> { "ag.access_pass.issued", "ag.access_pass.activated" }
    });
    Console.WriteLine($"Created webhook: {newWebhook.Id}");
    Console.WriteLine($"Private key: {newWebhook.PrivateKey}");

    // Delete a webhook
    await client.Console.Webhooks.DeleteAsync(newWebhook.Id);
}
```

### HID Organizations

#### Create an HID org

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task CreateOrgAsync()
{
    var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
    var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

    var client = new AccessGridClient(accountId, secretKey);

    var org = await client.Console.HID.Orgs.CreateAsync(new CreateHIDOrgRequest
    {
        Name = "My Org",
        FullAddress = "1 Main St, NY NY",
        Phone = "+1-555-0000",
        FirstName = "Ada",
        LastName = "Lovelace"
    });

    Console.WriteLine($"Created org: {org.Name} (ID: {org.Id})");
    Console.WriteLine($"Slug: {org.Slug}");
}
```

#### List HID orgs

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task ListOrgsAsync()
{
    var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
    var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

    var client = new AccessGridClient(accountId, secretKey);

    var orgs = await client.Console.HID.Orgs.ListAsync();

    foreach (var org in orgs)
    {
        Console.WriteLine($"Org ID: {org.Id}, Name: {org.Name}, Slug: {org.Slug}");
    }
}
```

#### Activate an HID org

```csharp
using AccessGrid;
using System;
using System.Threading.Tasks;

public async Task CompleteRegistrationAsync()
{
    var accountId = Environment.GetEnvironmentVariable("ACCOUNT_ID");
    var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

    var client = new AccessGridClient(accountId, secretKey);

    var result = await client.Console.HID.Orgs.ActivateAsync(new CompleteHIDOrgRequest
    {
        Email = "admin@example.com",
        Password = "hid-password-123"
    });

    Console.WriteLine($"Completed registration for org: {result.Name}");
    Console.WriteLine($"Status: {result.Status}");
}
```

## Testing Your Application Code

When building applications that use the AccessGrid SDK, you'll want to test your own business logic without making actual API calls. Here are examples of how to test your application code that calls the AccessGrid library.

### Testing Dependencies

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

### Example 1: Testing Employee Onboarding Service (XUnit)

Let's say you have an employee onboarding service in your application:

```csharp
// Your application service that uses AccessGrid
public class EmployeeOnboardingService
{
    private readonly IAccessGridClient _accessGridClient;
    private readonly ILogger<EmployeeOnboardingService> _logger;

    public EmployeeOnboardingService(IAccessGridClient accessGridClient, ILogger<EmployeeOnboardingService> logger)
    {
        _accessGridClient = accessGridClient;
        _logger = logger;
    }

    public async Task<OnboardingResult> OnboardEmployeeAsync(Employee employee)
    {
        try
        {
            // Your business logic here
            var provisionRequest = new ProvisionCardRequest
            {
                CardTemplateId = "your-template-id",
                EmployeeId = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                Classification = employee.Department == "Security" ? "security_personnel" : "employee",
                StartDate = employee.StartDate,
                ExpirationDate = employee.StartDate.AddYears(1)
            };

            var card = await _accessGridClient.AccessCards.ProvisionAsync(provisionRequest);

            _logger.LogInformation("Access card provisioned for employee {employee.Id}: {card.Id}");
            
            return new OnboardingResult
            {
                Success = true,
                CardId = card.Id,
                InstallUrl = card.Url,
                EmployeeId = employee.Id
            };
        }
        catch (AccessGridException ex)
        {
            _logger.LogError($"Failed to provision access card for employee {employee.Id}", ex);
            return new OnboardingResult { Success = false, ErrorMessage = ex.Message };
        }
    }
}

// Your test class
public class EmployeeOnboardingServiceTests
{
    [Fact]
    public async Task OnboardEmployeeAsync_ShouldReturnSuccess_WhenProvisioningSucceeds()
    {
        // Arrange
        var mockClient = new Mock<IAccessGridClient>();
        var mockApiService = new Mock<IApiService>();
        var mockLogger = new Mock<ILogger<EmployeeOnboardingService>>();

        var employee = new Employee
        {
            Id = "EMP123",
            FullName = "John Smith",
            Email = "john.smith@company.com",
            Department = "Engineering",
            StartDate = DateTime.UtcNow
        };

        var expectedCard = new AccessCard
        {
            Id = "card-123",
            FullName = "John Smith",
            State = "active",
            Url = "https://install.accessgrid.com/card-123"
        };

        mockApiService
            .Setup(x => x.PostAsync<AccessCard>("/v1/key-cards", It.IsAny<ProvisionCardRequest>()))
            .ReturnsAsync(expectedCard);

        var accessCardsService = new AccessCardsService(mockApiService.Object);
        mockClient.SetupGet(x => x.AccessCards).Returns(accessCardsService);

        var service = new EmployeeOnboardingService(mockClient.Object, mockLogger.Object);

        // Act
        var result = await service.OnboardEmployeeAsync(employee);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("card-123", result.CardId);
        Assert.Equal("https://install.accessgrid.com/card-123", result.InstallUrl);
        Assert.Equal("EMP123", result.EmployeeId);

        // Verify the correct request was made
        mockApiService.Verify(x => x.PostAsync<AccessCard>("/v1/key-cards", It.Is<ProvisionCardRequest>(req =>
            req.EmployeeId == "EMP123" &&
            req.FullName == "John Smith" &&
            req.Classification == "employee")), Times.Once);
    }

    [Fact]
    public async Task OnboardEmployeeAsync_ShouldReturnFailure_WhenAccessGridThrowsException()
    {
        // Arrange
        var mockClient = new Mock<IAccessGridClient>();
        var mockApiService = new Mock<IApiService>();
        var mockLogger = new Mock<ILogger<EmployeeOnboardingService>>();

        var employee = new Employee { Id = "EMP123", FullName = "John Smith", Email = "john@company.com" };

        mockApiService
            .Setup(x => x.PostAsync<AccessCard>("/v1/key-cards", It.IsAny<ProvisionCardRequest>()))
            .ThrowsAsync(new AccessGridException("API rate limit exceeded"));

        var accessCardsService = new AccessCardsService(mockApiService.Object);
        mockClient.SetupGet(x => x.AccessCards).Returns(accessCardsService);

        var service = new EmployeeOnboardingService(mockClient.Object, mockLogger.Object);

        // Act
        var result = await service.OnboardEmployeeAsync(employee);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("API rate limit exceeded", result.ErrorMessage);
        Assert.Null(result.CardId);
    }
}
```

### Example 2: Testing Minimal API Web Application (NUnit)

Here's how you might test a modern .NET 8 minimal API application using NUnit:

```csharp
// Your minimal API application (Program.cs)
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register AccessGrid client
builder.Services.AddSingleton<IAccessGridClient>(provider =>
{
    var accountId = builder.Configuration["AccessGrid:AccountId"];
    var secretKey = builder.Configuration["AccessGrid:SecretKey"];
    return new AccessGridClient(accountId, secretKey);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Minimal API endpoints
app.MapGet("/api/accesscards", async (string templateId, IAccessGridClient accessGridClient) =>
{
    var cards = await accessGridClient.AccessCards.ListAsync(new ListKeysRequest
    {
        TemplateId = templateId,
        State = "active"
    });

    return Results.Ok(cards);
})
.WithName("GetAccessCards")
.WithOpenApi();

app.MapPost("/api/accesscards/{cardId}/suspend", async (string cardId, IAccessGridClient accessGridClient) =>
{
    var suspendedCard = await accessGridClient.AccessCards.SuspendAsync(cardId);
    return Results.Ok(suspendedCard);
})
.WithName("SuspendCard")
.WithOpenApi();

app.Run();

// Your test class using NUnit
[TestFixture]
public class AccessCardsApiTests
{
    private WebApplication _app = null!;
    private HttpClient _client = null!;
    private Mock<IAccessGridClient> _mockAccessGridClient = null!;
    private Mock<IApiService> _mockApiService = null!;

    [SetUp]
    public void Setup()
    {
        _mockAccessGridClient = new Mock<IAccessGridClient>();
        _mockApiService = new Mock<IApiService>();

        var builder = WebApplication.CreateBuilder();
        builder.Services.AddEndpointsApiExplorer();

        // Override the AccessGrid client with our mock
        builder.Services.AddSingleton(_mockAccessGridClient.Object);

        _app = builder.Build();

        // Configure the same endpoints as your main application
        _app.MapGet("/api/accesscards", async (string templateId, IAccessGridClient accessGridClient) =>
        {
            var cards = await accessGridClient.AccessCards.ListAsync(new ListKeysRequest
            {
                TemplateId = templateId,
                State = "active"
            });
            return Results.Ok(cards);
        });

        _app.MapPost("/api/accesscards/{cardId}/suspend", async (string cardId, IAccessGridClient accessGridClient) =>
        {
            var suspendedCard = await accessGridClient.AccessCards.SuspendAsync(cardId);
            return Results.Ok(suspendedCard);
        });

        _client = new HttpClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client?.Dispose();
        _app?.DisposeAsync();
    }

    [Test]
    public async Task GetAccessCards_ShouldReturnOkWithCards_WhenCardsExist()
    {
        // Arrange
        var expectedCards = new List<AccessCard>
        {
            new() { Id = "card1", FullName = "John Smith", State = "active" },
            new() { Id = "card2", FullName = "Jane Doe", State = "active" }
        };

        var keysListResponse = new KeysListResponse { Keys = expectedCards };
        _mockApiService
            .Setup(x => x.GetAsync<KeysListResponse>("/v1/key-cards", It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(keysListResponse);

        var accessCardsService = new AccessCardsService(_mockApiService.Object);
        _mockAccessGridClient.SetupGet(x => x.AccessCards).Returns(accessCardsService);

        // Act
        await _app.StartAsync();
        var response = await _client.GetAsync($"{_app.Urls.First()}/api/accesscards?templateId=test-template");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var content = await response.Content.ReadAsStringAsync();
        var returnedCards = JsonSerializer.Deserialize<List<AccessCard>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.That(returnedCards, Is.Not.Null);
        Assert.That(returnedCards.Count, Is.EqualTo(2));
        Assert.That(returnedCards[0].FullName, Is.EqualTo("John Smith"));

        _mockApiService.Verify(x => x.GetAsync<KeysListResponse>("/v1/key-cards", It.IsAny<Dictionary<string, string>>()), Times.Once);
    }

    [Test]
    public async Task SuspendCard_ShouldReturnOkWithSuspendedCard_WhenSuspensionSucceeds()
    {
        // Arrange
        var expectedCard = new AccessCard { Id = "card123", State = "suspended" };

        _mockApiService
            .Setup(x => x.PostAsync<AccessCard>("/v1/key-cards/card123/suspend", null))
            .ReturnsAsync(expectedCard);

        var accessCardsService = new AccessCardsService(_mockApiService.Object);
        _mockAccessGridClient.SetupGet(x => x.AccessCards).Returns(accessCardsService);

        // Act
        await _app.StartAsync();
        var response = await _client.PostAsync($"{_app.Urls.First()}/api/accesscards/card123/suspend", null);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AccessCard>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo("card123"));
        Assert.That(result.State, Is.EqualTo("suspended"));

        _mockApiService.Verify(x => x.PostAsync<AccessCard>("/v1/key-cards/card123/suspend", null), Times.Once);
    }
}

## Feature Matrix

| Endpoint | Method | Supported |
|---|---|:---:|
| POST /v1/key-cards | `AccessCards.ProvisionAsync()` | Y |
| GET /v1/key-cards/{id} | `AccessCards.GetAsync()` | Y |
| PATCH /v1/key-cards/{id} | `AccessCards.UpdateAsync()` | Y |
| GET /v1/key-cards | `AccessCards.ListAsync()` | Y |
| POST /v1/key-cards/{id}/suspend | `AccessCards.SuspendAsync()` | Y |
| POST /v1/key-cards/{id}/resume | `AccessCards.ResumeAsync()` | Y |
| POST /v1/key-cards/{id}/unlink | `AccessCards.UnlinkAsync()` | Y |
| POST /v1/key-cards/{id}/delete | `AccessCards.DeleteAsync()` | Y |
| POST /v1/console/card-templates | `Console.CreateTemplateAsync()` | Y |
| PUT /v1/console/card-templates/{id} | `Console.UpdateTemplateAsync()` | Y |
| GET /v1/console/card-templates/{id} | `Console.ReadTemplateAsync()` | Y |
| GET /v1/console/card-templates/{id}/logs | `Console.EventLogAsync()` | Y |
| GET /v1/console/pass-template-pairs | `Console.ListPassTemplatePairsAsync()` | Y |
| POST /v1/console/card-templates/{id}/ios_preflight | `Console.IosPreflightAsync()` | Y |
| GET /v1/console/ledger-items | `Console.GetLedgerItemsAsync()` | Y |
| GET /v1/console/landing-pages | `Console.ListLandingPagesAsync()` | Y |
| POST /v1/console/landing-pages | `Console.CreateLandingPageAsync()` | Y |
| PUT /v1/console/landing-pages/{id} | `Console.UpdateLandingPageAsync()` | Y |
| GET /v1/console/credential-profiles | `Console.CredentialProfiles.ListAsync()` | Y |
| POST /v1/console/credential-profiles | `Console.CredentialProfiles.CreateAsync()` | Y |
| GET /v1/console/webhooks | `Console.Webhooks.ListAsync()` | Y |
| POST /v1/console/webhooks | `Console.Webhooks.CreateAsync()` | Y |
| DELETE /v1/console/webhooks/{id} | `Console.Webhooks.DeleteAsync()` | Y |
| POST /v1/console/hid/orgs | `Console.HID.Orgs.CreateAsync()` | Y |
| POST /v1/console/hid/orgs/activate | `Console.HID.Orgs.ActivateAsync()` | Y |
| GET /v1/console/hid/orgs | `Console.HID.Orgs.ListAsync()` | Y |

## License

MIT
