using System;
using System.Threading.Tasks;
using AccessGrid;

namespace AccessGridExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Get credentials from environment variables
            var accountId = Environment.GetEnvironmentVariable("ACCESSGRID_ACCOUNT_ID");
            var secretKey = Environment.GetEnvironmentVariable("ACCESSGRID_SECRET_KEY");

            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(secretKey))
            {
                Console.WriteLine("Please set ACCESSGRID_ACCOUNT_ID and ACCESSGRID_SECRET_KEY environment variables");
                return;
            }

            // Initialize the client
            using var client = new AccessGridClient(accountId, secretKey);

            try
            {
                Console.WriteLine("AccessGrid Client Test");
                Console.WriteLine("API Credentials:");
                Console.WriteLine($"  Account ID: {accountId}");
                Console.WriteLine($"  Secret Key: {secretKey.Substring(0, 3)}...{secretKey.Substring(secretKey.Length - 3)}");
                
                // Example 1: List cards
                Console.WriteLine("\nListing access cards...");
                var cards = await client.AccessCards.ListAsync(new ListKeysRequest
                {
                    TemplateId = "573087dd976",
                    // No state filter to get all cards
                });

                Console.WriteLine($"Found {cards.Count} active cards");
                foreach (var card in cards)
                {
                    Console.WriteLine($"Card ID: {card.Id}, Name: {card.FullName}, State: {card.State}");
                }

                // Example 2: Provision a new card
                Console.WriteLine("\nProvisioning a new card...");
                var newCard = await client.AccessCards.ProvisionAsync(new ProvisionCardRequest
                {
                    CardTemplateId = "573087dd976",
                    EmployeeId = "101010101",
                    CardNumber = "42069",
                    SiteCode = "42",
                    FullName = "John Doe",
                    Email = "ab@accessgrid.com",
                    PhoneNumber = "+17869064810",
                    Classification = "Employee",
                    Title = "Developer",
                    StartDate = DateTime.UtcNow,
                    ExpirationDate = DateTime.UtcNow.AddYears(1)
                });

                Console.WriteLine($"Card provisioned successfully. Install URL: {newCard.Url}");

                // // Example 3: Update a card
                // Console.WriteLine("\nUpdating a card...");
                // var updatedCard = await client.AccessCards.UpdateAsync(new UpdateCardRequest
                // {
                //     CardId = newCard.Id,
                //     FullName = "John Smith",
                //     ExpirationDate = DateTime.UtcNow.AddYears(2)
                // });

                // Console.WriteLine($"Card updated successfully. New name: {updatedCard.FullName}");

                // // Example 4: Suspend a card
                // Console.WriteLine("\nSuspending a card...");
                // var suspendedCard = await client.AccessCards.SuspendAsync(newCard.Id);
                // Console.WriteLine($"Card suspended successfully. State: {suspendedCard.State}");

                // // Example 5: Resume a card
                // Console.WriteLine("\nResuming a card...");
                // var resumedCard = await client.AccessCards.ResumeAsync(newCard.Id);
                // Console.WriteLine($"Card resumed successfully. State: {resumedCard.State}");

                // // Example 6: Enterprise example - Read template (for enterprise customers)
                // Console.WriteLine("\nReading template info...");
                // try
                // {
                //     var template = await client.Console.ReadTemplateAsync("your_template_id");
                //     Console.WriteLine($"Template: {template.Name}, Platform: {template.Platform}");
                // }
                // catch (AccessGridException ex)
                // {
                //     Console.WriteLine($"Enterprise operation failed: {ex.Message}");
                // }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}