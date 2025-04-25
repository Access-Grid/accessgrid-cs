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

// Initialize the client
var client = new AccessGridClient(accountId, secretKey);
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

    var client = new AccessGridClient(accountId, secretKey);

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

    var client = new AccessGridClient(accountId, secretKey);

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

   var client = new AccessGridClient(accountId, secretKey);

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

   var client = new AccessGridClient(accountId, secretKey);

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

   var client = new AccessGridClient(accountId, secretKey);

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

   var client = new AccessGridClient(accountId, secretKey);

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

   var client = new AccessGridClient(accountId, secretKey);

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

   var client = new AccessGridClient(accountId, secretKey);

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

   var client = new AccessGridClient(accountId, secretKey);

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

   var client = new AccessGridClient(accountId, secretKey);

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

## License

MIT