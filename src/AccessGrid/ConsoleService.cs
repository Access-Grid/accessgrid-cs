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

        internal ConsoleService(IApiService apiService)
        {
            _apiService = apiService;
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
    }
}