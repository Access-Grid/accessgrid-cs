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

            _logger.LogInformation("Access card provisioned for employee {EmployeeId}: {CardId}", employee.Id, card.Id);

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
            _logger.LogError(ex, "Failed to provision access card for employee {EmployeeId}", employee.Id);
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
        var mockAccessCards = new Mock<AccessCardsService>();
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

        mockAccessCards
            .Setup(x => x.ProvisionAsync(It.IsAny<ProvisionCardRequest>()))
            .ReturnsAsync(expectedCard);

        mockClient.SetupGet(x => x.AccessCards).Returns(mockAccessCards.Object);

        var service = new EmployeeOnboardingService(mockClient.Object, mockLogger.Object);

        // Act
        var result = await service.OnboardEmployeeAsync(employee);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("card-123", result.CardId);
        Assert.Equal("https://install.accessgrid.com/card-123", result.InstallUrl);
        Assert.Equal("EMP123", result.EmployeeId);

        // Verify the correct request was made
        mockAccessCards.Verify(x => x.ProvisionAsync(It.Is<ProvisionCardRequest>(req =>
            req.EmployeeId == "EMP123" &&
            req.FullName == "John Smith" &&
            req.Classification == "employee")), Times.Once);
    }

    [Fact]
    public async Task OnboardEmployeeAsync_ShouldReturnFailure_WhenAccessGridThrowsException()
    {
        // Arrange
        var mockClient = new Mock<IAccessGridClient>();
        var mockAccessCards = new Mock<AccessCardsService>();
        var mockLogger = new Mock<ILogger<EmployeeOnboardingService>>();

        var employee = new Employee { Id = "EMP123", FullName = "John Smith", Email = "john@company.com" };

        mockAccessCards
            .Setup(x => x.ProvisionAsync(It.IsAny<ProvisionCardRequest>()))
            .ThrowsAsync(new AccessGridException("API rate limit exceeded"));

        mockClient.SetupGet(x => x.AccessCards).Returns(mockAccessCards.Object);

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

### Example 2: Testing Card Management Controller (NUnit)

Here's how you might test an ASP.NET Core controller that manages access cards:

```csharp
// Your controller that uses AccessGrid
[ApiController]
[Route("api/[controller]")]
public class AccessCardsController : ControllerBase
{
    private readonly IAccessGridClient _accessGridClient;
    private readonly ILogger<AccessCardsController> _logger;

    public AccessCardsController(IAccessGridClient accessGridClient, ILogger<AccessCardsController> logger)
    {
        _accessGridClient = accessGridClient;
        _logger = logger;
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetEmployeeCards(string employeeId)
    {
        try
        {
            var cards = await _accessGridClient.AccessCards.ListAsync(new ListKeysRequest
            {
                TemplateId = "your-template-id",
                State = "active"
            });

            var employeeCards = cards.Where(c => c.EmployeeId == employeeId).ToList();

            if (!employeeCards.Any())
            {
                return NotFound($"No active cards found for employee {employeeId}");
            }

            return Ok(employeeCards.Select(c => new
            {
                c.Id,
                c.FullName,
                c.State,
                c.ExpirationDate
            }));
        }
        catch (AccessGridException ex)
        {
            _logger.LogError(ex, "Failed to retrieve cards for employee {EmployeeId}", employeeId);
            return StatusCode(500, "Failed to retrieve access cards");
        }
    }

    [HttpPost("{cardId}/suspend")]
    public async Task<IActionResult> SuspendCard(string cardId)
    {
        try
        {
            var suspendedCard = await _accessGridClient.AccessCards.SuspendAsync(cardId);
            _logger.LogInformation("Card {CardId} suspended successfully", cardId);

            return Ok(new { cardId = suspendedCard.Id, state = suspendedCard.State });
        }
        catch (AccessGridException ex)
        {
            _logger.LogError(ex, "Failed to suspend card {CardId}", cardId);
            return BadRequest($"Failed to suspend card: {ex.Message}");
        }
    }
}

// Your test class
[TestFixture]
public class AccessCardsControllerTests
{
    private Mock<IAccessGridClient> _mockClient;
    private Mock<AccessCardsService> _mockAccessCards;
    private Mock<ILogger<AccessCardsController>> _mockLogger;
    private AccessCardsController _controller;

    [SetUp]
    public void Setup()
    {
        _mockClient = new Mock<IAccessGridClient>();
        _mockAccessCards = new Mock<AccessCardsService>();
        _mockLogger = new Mock<ILogger<AccessCardsController>>();

        _mockClient.SetupGet(x => x.AccessCards).Returns(_mockAccessCards.Object);
        _controller = new AccessCardsController(_mockClient.Object, _mockLogger.Object);
    }

    [Test]
    public async Task GetEmployeeCards_ShouldReturnOk_WhenCardsExist()
    {
        // Arrange
        var cards = new List<AccessCard>
        {
            new AccessCard { Id = "card1", EmployeeId = "EMP123", FullName = "John Smith", State = "active" },
            new AccessCard { Id = "card2", EmployeeId = "EMP456", FullName = "Jane Doe", State = "active" },
            new AccessCard { Id = "card3", EmployeeId = "EMP123", FullName = "John Smith", State = "active" }
        };

        _mockAccessCards
            .Setup(x => x.ListAsync(It.IsAny<ListKeysRequest>()))
            .ReturnsAsync(cards);

        // Act
        var result = await _controller.GetEmployeeCards("EMP123");

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        var returnedCards = okResult.Value as IEnumerable<object>;
        Assert.That(returnedCards.Count(), Is.EqualTo(2)); // Should return 2 cards for EMP123
    }

    [Test]
    public async Task GetEmployeeCards_ShouldReturnNotFound_WhenNoCardsExist()
    {
        // Arrange
        _mockAccessCards
            .Setup(x => x.ListAsync(It.IsAny<ListKeysRequest>()))
            .ReturnsAsync(new List<AccessCard>());

        // Act
        var result = await _controller.GetEmployeeCards("EMP999");

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = (NotFoundObjectResult)result;
        Assert.That(notFoundResult.Value, Is.EqualTo("No active cards found for employee EMP999"));
    }

    [Test]
    public async Task SuspendCard_ShouldReturnOk_WhenSuspensionSucceeds()
    {
        // Arrange
        var suspendedCard = new AccessCard { Id = "card123", State = "suspended" };

        _mockAccessCards
            .Setup(x => x.SuspendAsync("card123"))
            .ReturnsAsync(suspendedCard);

        // Act
        var result = await _controller.SuspendCard("card123");

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;

        // Verify the correct method was called
        _mockAccessCards.Verify(x => x.SuspendAsync("card123"), Times.Once);
    }

    [Test]
    public async Task SuspendCard_ShouldReturnBadRequest_WhenAccessGridThrowsException()
    {
        // Arrange
        _mockAccessCards
            .Setup(x => x.SuspendAsync("invalid-card"))
            .ThrowsAsync(new AccessGridException("Card not found"));

        // Act
        var result = await _controller.SuspendCard("invalid-card");

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = (BadRequestObjectResult)result;
        Assert.That(badRequestResult.Value, Is.EqualTo("Failed to suspend card: Card not found"));
    }
}

## License

MIT