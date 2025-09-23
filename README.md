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

## License

MIT