using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccessGrid
{
    /// <summary>
    /// Service for managing access cards
    /// </summary>
    public class AccessCardsService
    {
        private readonly AccessGridClient _client;

        internal AccessCardsService(AccessGridClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Issues a new access card
        /// </summary>
        /// <param name="request">Card provision details</param>
        /// <returns>Newly created AccessCard</returns>
        public async Task<AccessCard> IssueAsync(ProvisionCardRequest request)
        {
            var response = await _client.PostAsync<AccessCard>("/v1/key-cards", request);
            return response;
        }

        /// <summary>
        /// Alias for IssueAsync to maintain backward compatibility
        /// </summary>
        /// <param name="request">Card provision details</param>
        /// <returns>Newly created AccessCard</returns>
        public async Task<AccessCard> ProvisionAsync(ProvisionCardRequest request)
        {
            return await IssueAsync(request);
        }

        /// <summary>
        /// Updates an existing access card
        /// </summary>
        /// <param name="request">Card update details including CardId</param>
        /// <returns>Updated AccessCard</returns>
        public async Task<AccessCard> UpdateAsync(UpdateCardRequest request)
        {
            var response = await _client.PatchAsync<AccessCard>($"/v1/key-cards/{request.CardId}", request);
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

            var response = await _client.GetAsync<KeysListResponse>("/v1/key-cards", queryParams);
            return response?.Keys ?? new List<AccessCard>();
        }

        private async Task<AccessCard> ManageAsync(string cardId, string action)
        {
            var response = await _client.PostAsync<AccessCard>($"/v1/key-cards/{cardId}/{action}", null);
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