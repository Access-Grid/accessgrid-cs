using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccessGrid
{
    /// <summary>
    /// Service for managing console operations (enterprise features)
    /// </summary>
    public class ConsoleService
    {
        private readonly IApiService _apiService;

        /// <summary>
        /// HID-related services
        /// </summary>
        public HIDService HID { get; }

        /// <summary>
        /// Webhook management services
        /// </summary>
        public WebhooksService Webhooks { get; }

        /// <summary>
        /// Credential profile management services
        /// </summary>
        public CredentialProfilesService CredentialProfiles { get; }

        internal ConsoleService(IApiService apiService)
        {
            _apiService = apiService;
            HID = new HIDService(apiService);
            Webhooks = new WebhooksService(apiService);
            CredentialProfiles = new CredentialProfilesService(apiService);
        }

        /// <summary>
        /// Creates a new card template (enterprise only)
        /// </summary>
        /// <param name="request">Template creation details</param>
        /// <returns>Newly created Template</returns>
        public async Task<Template> CreateTemplateAsync(CreateTemplateRequest request)
        {
            var response = await _apiService.PostAsync<Template>("/v1/console/card-templates", request);
            return response;
        }

        /// <summary>
        /// Updates an existing card template (enterprise only)
        /// </summary>
        /// <param name="request">Template update details including CardTemplateId</param>
        /// <returns>Updated Template</returns>
        public async Task<Template> UpdateTemplateAsync(UpdateTemplateRequest request)
        {
            var response = await _apiService.PutAsync<Template>($"/v1/console/card-templates/{request.CardTemplateId}", request);
            return response;
        }

        /// <summary>
        /// Gets details of a card template (enterprise only)
        /// </summary>
        /// <param name="templateId">Unique identifier for the card template to look up</param>
        /// <returns>Template details</returns>
        public async Task<Template> ReadTemplateAsync(string templateId)
        {
            var response = await _apiService.GetAsync<Template>($"/v1/console/card-templates/{templateId}");
            return response;
        }

        /// <summary>
        /// Gets event logs for a card template (enterprise only)
        /// </summary>
        /// <param name="templateId">Unique identifier for the card template to look up</param>
        /// <param name="filters">Optional filters to reduce result size of event logs</param>
        /// <returns>List of event log entries</returns>
        public async Task<List<EventLogEntry>> EventLogAsync(string templateId, EventLogFilters filters = null)
        {
            var queryParams = new Dictionary<string, string>();
            
            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.Device))
                    queryParams.Add("device", filters.Device);
                
                if (filters.StartDate.HasValue)
                    queryParams.Add("start_date", filters.StartDate.Value.ToString("o"));
                
                if (filters.EndDate.HasValue)
                    queryParams.Add("end_date", filters.EndDate.Value.ToString("o"));
                
                if (!string.IsNullOrEmpty(filters.EventType))
                    queryParams.Add("event_type", filters.EventType);
            }

            var response = await _apiService.GetAsync<EventLogResponse>($"/v1/console/card-templates/{templateId}/logs", queryParams);
            return response?.Events ?? new List<EventLogEntry>();
        }

        /// <summary>
        /// Lists pass template pairs (enterprise only)
        /// </summary>
        /// <param name="page">Page number (defaults to 1 on the server)</param>
        /// <param name="perPage">Items per page, max 100 (defaults to 50 on the server)</param>
        /// <returns>Pass template pairs with pagination info</returns>
        public async Task<PassTemplatePairsResponse> ListPassTemplatePairsAsync(int? page = null, int? perPage = null)
        {
            var queryParams = new Dictionary<string, string>();

            if (page.HasValue)
                queryParams.Add("page", page.Value.ToString());

            if (perPage.HasValue)
                queryParams.Add("per_page", perPage.Value.ToString());

            var response = await _apiService.GetAsync<PassTemplatePairsResponse>("/v1/console/pass-template-pairs", queryParams);
            return response ?? new PassTemplatePairsResponse();
        }

        /// <summary>
        /// Gets ledger/billing items (enterprise only)
        /// </summary>
        /// <param name="page">Page number (defaults to 1 on the server)</param>
        /// <param name="perPage">Items per page, max 100 (defaults to 50 on the server)</param>
        /// <param name="startDate">Filter items created on or after this date (ISO8601)</param>
        /// <param name="endDate">Filter items created on or before this date (ISO8601)</param>
        /// <returns>Ledger items with pagination info</returns>
        public async Task<LedgerItemsResponse> GetLedgerItemsAsync(int? page = null, int? perPage = null, System.DateTime? startDate = null, System.DateTime? endDate = null)
        {
            var queryParams = new Dictionary<string, string>();

            if (page.HasValue)
                queryParams.Add("page", page.Value.ToString());

            if (perPage.HasValue)
                queryParams.Add("per_page", perPage.Value.ToString());

            if (startDate.HasValue)
                queryParams.Add("start_date", startDate.Value.ToString("o"));

            if (endDate.HasValue)
                queryParams.Add("end_date", endDate.Value.ToString("o"));

            var response = await _apiService.GetAsync<LedgerItemsResponse>("/v1/console/ledger-items", queryParams);
            return response ?? new LedgerItemsResponse();
        }

        /// <summary>
        /// Lists all landing pages
        /// </summary>
        /// <returns>List of landing pages</returns>
        public async Task<List<LandingPage>> ListLandingPagesAsync()
        {
            var response = await _apiService.GetAsync<List<LandingPage>>("/v1/console/landing-pages");
            return response ?? new List<LandingPage>();
        }

        /// <summary>
        /// Creates a new landing page
        /// </summary>
        /// <param name="request">Landing page creation details</param>
        /// <returns>Newly created landing page</returns>
        public async Task<LandingPage> CreateLandingPageAsync(CreateLandingPageRequest request)
        {
            var response = await _apiService.PostAsync<LandingPage>("/v1/console/landing-pages", request);
            return response;
        }

        /// <summary>
        /// Updates an existing landing page
        /// </summary>
        /// <param name="landingPageId">ID of the landing page to update</param>
        /// <param name="request">Landing page update details</param>
        /// <returns>Updated landing page</returns>
        public async Task<LandingPage> UpdateLandingPageAsync(string landingPageId, UpdateLandingPageRequest request)
        {
            var response = await _apiService.PutAsync<LandingPage>($"/v1/console/landing-pages/{landingPageId}", request);
            return response;
        }

        /// <summary>
        /// Retrieves iOS In-App Provisioning identifiers for a card template and access pass
        /// </summary>
        /// <param name="cardTemplateId">The card template ID</param>
        /// <param name="accessPassExId">The access pass external ID</param>
        /// <returns>iOS preflight identifiers</returns>
        public async Task<IosPreflightResponse> IosPreflightAsync(string cardTemplateId, string accessPassExId)
        {
            var body = new { access_pass_ex_id = accessPassExId };
            var response = await _apiService.PostAsync<IosPreflightResponse>($"/v1/console/card-templates/{cardTemplateId}/ios_preflight", body);
            return response;
        }
    }

    /// <summary>
    /// Service for managing webhooks
    /// </summary>
    public class WebhooksService
    {
        private readonly IApiService _apiService;

        internal WebhooksService(IApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Lists all webhooks
        /// </summary>
        public async Task<WebhooksResponse> ListAsync(int? page = null, int? perPage = null)
        {
            var queryParams = new Dictionary<string, string>();

            if (page.HasValue)
                queryParams.Add("page", page.Value.ToString());

            if (perPage.HasValue)
                queryParams.Add("per_page", perPage.Value.ToString());

            var response = await _apiService.GetAsync<WebhooksResponse>("/v1/console/webhooks", queryParams);
            return response ?? new WebhooksResponse();
        }

        /// <summary>
        /// Creates a new webhook
        /// </summary>
        public async Task<Webhook> CreateAsync(CreateWebhookRequest request)
        {
            if (string.IsNullOrEmpty(request.AuthMethod))
                request.AuthMethod = "bearer_token";

            var response = await _apiService.PostAsync<Webhook>("/v1/console/webhooks", request);
            return response;
        }

        /// <summary>
        /// Deletes a webhook by ID
        /// </summary>
        public async Task DeleteAsync(string webhookId)
        {
            await _apiService.DeleteAsync($"/v1/console/webhooks/{webhookId}");
        }
    }

    /// <summary>
    /// Service providing access to HID-related services
    /// </summary>
    public class HIDService
    {
        /// <summary>
        /// HID organization management
        /// </summary>
        public HIDOrgsService Orgs { get; }

        internal HIDService(IApiService apiService)
        {
            Orgs = new HIDOrgsService(apiService);
        }
    }

    /// <summary>
    /// Service for managing credential profiles
    /// </summary>
    public class CredentialProfilesService
    {
        private readonly IApiService _apiService;

        internal CredentialProfilesService(IApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Lists all credential profiles
        /// </summary>
        public async Task<List<CredentialProfile>> ListAsync()
        {
            var response = await _apiService.GetAsync<List<CredentialProfile>>("/v1/console/credential-profiles");
            return response ?? new List<CredentialProfile>();
        }

        /// <summary>
        /// Creates a new credential profile
        /// </summary>
        public async Task<CredentialProfile> CreateAsync(CreateCredentialProfileRequest request)
        {
            var response = await _apiService.PostAsync<CredentialProfile>("/v1/console/credential-profiles", request);
            return response;
        }
    }

    /// <summary>
    /// Service for managing HID organizations
    /// </summary>
    public class HIDOrgsService
    {
        private readonly IApiService _apiService;

        internal HIDOrgsService(IApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Creates a new HID organization
        /// </summary>
        public async Task<HIDOrg> CreateAsync(CreateHIDOrgRequest request)
        {
            var response = await _apiService.PostAsync<HIDOrg>("/v1/console/hid/orgs", request);
            return response;
        }

        /// <summary>
        /// Lists all HID organizations
        /// </summary>
        public async Task<List<HIDOrg>> ListAsync()
        {
            var response = await _apiService.GetAsync<List<HIDOrg>>("/v1/console/hid/orgs");
            return response ?? new List<HIDOrg>();
        }

        /// <summary>
        /// Completes HID org registration with credentials
        /// </summary>
        public async Task<HIDOrg> ActivateAsync(CompleteHIDOrgRequest request)
        {
            var response = await _apiService.PostAsync<HIDOrg>("/v1/console/hid/orgs/activate", request);
            return response;
        }
    }
}