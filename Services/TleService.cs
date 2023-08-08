using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MyConsoleApp.Consts;
using MyConsoleApp.Helpers;
using MyConsoleApp.Interfaces;
using MyConsoleApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConsoleApp.Services
{
    public class TleService : ITleService
    {
        private string GetTlesUrl => $"{ApiUrl.Url}/tle";

        private readonly int _pageSize = 100;
        private readonly ILogger<TleService> _logger;

        public TleService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TleService>();
        }
        public async Task<List<Tle>> GetAllTlesAsync()
        {
            var parameters = new Parameters() { PageSize = _pageSize };
            try
            {
                var apiUrl = BuildUrlWithQuery(parameters);
                List<Tle> allTles = new List<Tle>();

                using (var client = new HttpClient())
                {
                    _logger.LogInformation("Starting to get all tles....");
                    TleResponse firstResponse = await GetTleResponseAsync(client, apiUrl);

                    var uriForLastItem = new Uri(firstResponse.View.Last);
                    var queryDictionaryForLast = QueryHelpers.ParseQuery(uriForLastItem.Query);

                    int totalItems = int.Parse(queryDictionaryForLast["page"]);
                    int currentPage = firstResponse.Parameters.Page;
                    allTles.AddRange(firstResponse.Member);

                    int totalPages = totalItems;
                    _logger.LogInformation($"Loading {currentPage}/{totalPages}...\n");

                    var tleResponses = new List<TleResponse>();
                    for (currentPage = currentPage + 1; currentPage <= totalPages; currentPage++)
                    {
                        var uriBuilder = new UriBuilder(apiUrl);
                        var query = QueryHelpers.ParseQuery(uriBuilder.Query);
                        parameters.Page = currentPage;
                        uriBuilder.Query = BuildUrlWithQuery(parameters);
                        string nextPageUrl = uriBuilder.ToString();

                        _logger.LogInformation($"Loading {currentPage}/{totalPages}...\n");
                        var responce = await GetTleResponseAsync(client, nextPageUrl);
                        tleResponses.Add(responce);
                    }

                    foreach (var tleResponse in tleResponses)
                    {
                        List<Tle> currentTles = tleResponse.Member;
                        allTles.AddRange(currentTles);
                    }
                }
                return allTles;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<TleResponse> GetTleResponseAsync(HttpClient client, string apiUrl)
        {
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                if(response.StatusCode == System.Net.HttpStatusCode.TooManyRequests 
                    && response.Headers.TryGetValues("X-RateLimit-Retry-After", out var retryAfterValues))
                {
                    _logger.LogWarning("Got 429: TooManyRequests\n" +
                        "Trying to wait till next retry");
                    if(DateTimeOffset.TryParse(retryAfterValues.First(), out DateTimeOffset retryAfter)) 
                    {
                        TimeSpan timeUntilRetry = retryAfter - DateTimeOffset.UtcNow;

                        int millisecondsUntilRetry = (int)timeUntilRetry.TotalMilliseconds;
                        _logger.LogWarning($"Waiting another {(int)timeUntilRetry.TotalMinutes} minutes to retry.....");
                        await Task.Delay(millisecondsUntilRetry + 1000);
                        var tles = await GetTleResponseAsync(client, apiUrl);
                        return tles;
                    }
                }
                throw new Exception($"Status code is not successfull with apiUrl: {apiUrl} " +
                    $"With response: {response}");
            }
            string responseBody = await response.Content.ReadAsStringAsync();
            TleResponse tleResponse = JsonConvert.DeserializeObject<TleResponse>(responseBody);
            if (tleResponse is null)
                return new TleResponse();
            return tleResponse;
        }

        public void SaveTlesToFile(List<Tle> tles, string filePath)
        {
            _logger.LogInformation($"Starting to save tles into file....");
            var count = 1;
            string extension = Path.GetExtension(filePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            var directory = Path.GetDirectoryName(filePath);
            while (File.Exists(filePath))
            {
                filePath = Path.Combine(directory, $"{fileNameWithoutExtension}_{count}{extension}");
                count++;
            };
            FileHelper.SaveObjectToFile(tles, filePath);
        }

        public List<Tle> GetTlesFromFile(string filePath)
        {
            var fileTles = FileHelper.GetObjectFromFile<List<Tle>>(filePath);
            return fileTles;
        }

        public bool CheckIfSatelliteExistsInFile(string filePath, long satelliteId)
        {
            if (!File.Exists(filePath))
                return false;

            List<Tle> objects = FileHelper.GetObjectFromFile<List<Tle>>(filePath);
            return objects.Any(obj => obj.SatelliteId == satelliteId);
        }

        private string BuildUrlWithQuery(Parameters parameters)
        {
            var uriBuilder = new UriBuilder(GetTlesUrl);
            uriBuilder.Query = $"page-size={parameters.PageSize}&page={parameters.Page}&search={parameters.Search}&sort={parameters.Sort}&sort-dir={parameters.SortDir}";
            var apiUrl = uriBuilder.ToString();
            return apiUrl;
        }

    }
}
