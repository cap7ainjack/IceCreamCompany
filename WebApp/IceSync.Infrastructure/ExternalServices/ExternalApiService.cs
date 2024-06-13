using Microsoft.Extensions.Options;
using IceSync.Application.Models.Config;
using IceSync.Application.Models.Identity;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using IceSync.Application.Services.External;
using Microsoft.Extensions.Caching.Memory;
using IceSync.Application.Models.ExternalApi;

namespace IceSync.Application.Integrations
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ExternalApiSettings _settings;
        private readonly IMemoryCache _cache;

        public ExternalApiService(HttpClient httpClient, IMemoryCache cache, IOptions<ExternalApiSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _cache = cache;
        }

        public async Task<string> GetTokenAsync()
        {
            if (!_cache.TryGetValue("externalApiToken", out string token))
            {
                var tokenRequest = new
                {
                    apiCompanyId = _settings.ApiCompanyId,
                    apiUserId = _settings.ApiUserId,
                    apiUserSecret = _settings.ApiUserSecret
                };

                var tokenResponse = await _httpClient.PostAsJsonAsync($"{_settings.BaseUrl}/v2/authenticate", tokenRequest);

                tokenResponse.EnsureSuccessStatusCode();
                var tokenObject = await tokenResponse.Content.ReadFromJsonAsync<ExternalApiTokenResponse>();
                token = tokenObject.AccessToken;
                var tokenExpiration = DateTime.UtcNow.AddSeconds(tokenObject.ExpiresIn - 120);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = tokenExpiration
                };

                _cache.Set("externalApiToken", token, cacheEntryOptions);
            }

            return token;
        }

        public async Task<IEnumerable<ExternalApiWorkflow>> GetWorkflowsAsync()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync());
                var response = await _httpClient.GetAsync(_settings.BaseUrl + "/workflows");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<IEnumerable<ExternalApiWorkflow>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // log fir the background service
                throw;
            }
        }

        public async Task<bool> RunWorkflowAsync(int workflowId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync());
                var response = await _httpClient.PostAsync($"{_settings.BaseUrl}/workflows/{workflowId}/run", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // log fir the background service
                throw;
            }
        }
    }
}