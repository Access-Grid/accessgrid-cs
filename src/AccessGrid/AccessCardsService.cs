using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace AccessGrid
{
    /// <summary>
    /// Service for managing access cards
    /// </summary>
    public class AccessCardsService
    {
        private readonly IApiService _apiService;

        public AccessCardsService(IApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Issues a new access card or unified access pass (for template pairs)
        /// </summary>
        /// <param name="request">Card provision details</param>
        /// <returns>AccessCard for single card, or UnifiedAccessPass for template pairs</returns>
        public async Task<Union> IssueAsync(ProvisionCardRequest request)
        {
            var json = await _apiService.PostAsync<string>("/v1/key-cards", request);
            var card = JsonSerializer.Deserialize<AccessCard>(json);

            if (card.Details != null)
                return JsonSerializer.Deserialize<UnifiedAccessPass>(json);

            return card;
        }

        /// <summary>
        /// Alias for IssueAsync to maintain backward compatibility
        /// </summary>
        /// <param name="request">Card provision details</param>
        /// <returns>AccessCard for single card, or UnifiedAccessPass for template pairs</returns>
        public async Task<Union> ProvisionAsync(ProvisionCardRequest request)
        {
            return await IssueAsync(request);
        }

        /// <summary>
        /// Gets details about a specific access card
        /// </summary>
        /// <param name="cardId">Unique identifier of the NFC key to retrieve</param>
        /// <returns>AccessCard details</returns>
        public async Task<AccessCard> GetAsync(string cardId)
        {
            var response = await _apiService.GetAsync<AccessCard>($"/v1/key-cards/{cardId}", null);
            return response;
        }

        /// <summary>
        /// Updates an existing access card
        /// </summary>
        /// <param name="cardId">Unique identifier of the NFC key to update</param>
        /// <param name="request">Card update details</param>
        /// <returns>Updated AccessCard</returns>
        public async Task<AccessCard> UpdateAsync(string cardId, UpdateCardRequest request)
        {
            var response = await _apiService.PatchAsync<AccessCard>($"/v1/key-cards/{cardId}", request);
            return response;
        }

        /// <summary>
        /// Lists NFC keys provisioned for a particular card template
        /// </summary>
        /// <param name="request">List keys request parameters</param>
        /// <returns>List of AccessCard objects</returns>
        public async Task<List<AccessCard>> ListAsync(ListKeysRequest request)
        {
            var queryParams = new Dictionary<string, string>();
            
            if (!string.IsNullOrEmpty(request.TemplateId))
                queryParams.Add("template_id", request.TemplateId);
            
            if (!string.IsNullOrEmpty(request.State))
                queryParams.Add("state", request.State);

            var response = await _apiService.GetAsync<KeysListResponse>("/v1/key-cards", queryParams);
            return response?.Keys ?? new List<AccessCard>();
        }

        private async Task<AccessCard> ManageAsync(string cardId, string action)
        {
            var response = await _apiService.PostAsync<AccessCard>($"/v1/key-cards/{cardId}/{action}", null);
            return response;
        }

        /// <summary>
        /// Suspends an access card
        /// </summary>
        /// <param name="cardId">Unique identifier of the NFC key to suspend</param>
        /// <returns>Suspended AccessCard</returns>
        public async Task<AccessCard> SuspendAsync(string cardId)
        {
            return await ManageAsync(cardId, "suspend");
        }

        /// <summary>
        /// Resumes a suspended access card
        /// </summary>
        /// <param name="cardId">Unique identifier of the NFC key to resume</param>
        /// <returns>Resumed AccessCard</returns>
        public async Task<AccessCard> ResumeAsync(string cardId)
        {
            return await ManageAsync(cardId, "resume");
        }

        /// <summary>
        /// Unlinks an access card from its current holder
        /// </summary>
        /// <param name="cardId">Unique identifier of the NFC key to unlink</param>
        /// <returns>Unlinked AccessCard</returns>
        public async Task<AccessCard> UnlinkAsync(string cardId)
        {
            return await ManageAsync(cardId, "unlink");
        }

        /// <summary>
        /// Deletes an access card
        /// </summary>
        /// <param name="cardId">Unique identifier of the NFC key to delete</param>
        /// <returns>Deleted AccessCard</returns>
        public async Task<AccessCard> DeleteAsync(string cardId)
        {
            return await ManageAsync(cardId, "delete");
        }
    }
}