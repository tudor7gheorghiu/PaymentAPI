using System.Text.Json;
using System.Text;
using PaymentAPI.Models;
using Microsoft.Extensions.Options;
using PaymentAPI.Controllers;

namespace PaymentAPI.Services
{
    public class PaymentTokenGeneratorService : IPaymentTokenGeneratorService
    {
        private readonly TotalProcessingOptions _tpOptions;
        private readonly ILogger<PaymentTokenGeneratorService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public PaymentTokenGeneratorService(IOptions<TotalProcessingOptions> tpOptions,
                                            ILogger<PaymentTokenGeneratorService> logger,
                                            IHttpClientFactory httpClientFactory) 
        { 
            _tpOptions = tpOptions.Value;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Dictionary<string, dynamic>> SendPostRequestAsync(PaymentInfo paymentInfo)
        {
            // Return dynamic response to capture different formats 3rd pary API responses
            var responseData = new Dictionary<string, dynamic>();
            
            if (!ValidateConfiguration())
            {
                responseData.Add("Error Message", "Missing configuration");
                _logger.LogError("Missing configuration");
            }

            var requestData = new Dictionary<string, string?>
            {
                { "entityId", _tpOptions.EntityId },
                { "amount", paymentInfo.Amount.ToString() },
                { "currency", paymentInfo.Currency },
                { "paymentType", paymentInfo.PaymentType },
                { "integrity", "true" }
            };

            using (var client = _httpClientFactory.CreateClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("Authorization", _tpOptions.BearerToken);
                    var content = new FormUrlEncodedContent(requestData);

                    var response = await client.PostAsync(_tpOptions.Url, content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError("Request unsuccessful " + responseString);
                    }

                    responseData = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(responseString) ?? new Dictionary<string, dynamic>();
                }
                catch (Exception ex)
                {
                    responseData.Add("Exception", ex.Message);
                    _logger.LogError(ex.Message);
                }
            }

            return responseData;
        }


        // Ensure configuration is present
        private bool ValidateConfiguration() 
        {
            if (string.IsNullOrWhiteSpace(_tpOptions.Url) || 
                string.IsNullOrWhiteSpace(_tpOptions.BearerToken) ||
                string.IsNullOrWhiteSpace(_tpOptions.EntityId))
            {
                return false;
            }    
            return true;
        }
    }
}
