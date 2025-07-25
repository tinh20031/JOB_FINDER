using JOB_FINDER_API.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace JOB_FINDER_API.Services
{
    public interface IPaymentService
    {
        Task<(bool Success, string CheckoutUrl, string OrderCode, string ErrorMessage)> CreatePaymentRequest(
            User user, SubscriptionType package);
        Task<(bool Success, string Status, string ErrorMessage)> CheckPaymentStatus(string orderCode);
        bool VerifyWebhookSignature(string payload, string signature);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _apiKey;
        private readonly string _checksumKey;
        private readonly string _payOsApiUrl = "https://api-business.payos.vn";
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IConfiguration configuration,
            HttpClient httpClient,
            ILogger<PaymentService> logger)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;

            // Initialize PayOS configuration
            _clientId = _configuration["PayOS:ClientId"];
            _apiKey = _configuration["PayOS:ApiKey"];
            _checksumKey = _configuration["PayOS:ChecksumKey"];

            // Configure HttpClient for PayOS API calls
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("x-client-id", _clientId);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        }

        public async Task<(bool Success, string CheckoutUrl, string OrderCode, string ErrorMessage)> CreatePaymentRequest(
            User user, SubscriptionType package)
        {
            try
            {
                var orderCode = $"SUB-{Guid.NewGuid():N}";

                var paymentRequest = new
                {
                    orderCode = orderCode,
                    amount = (int)(package.Price * 100), // Convert to smallest currency unit
                    description = $"Payment for {package.Name} subscription package",
                    buyerName = user.FullName,
                    buyerEmail = user.Email,
                    buyerPhone = user.Phone,
                    cancelUrl = $"{_configuration["AppSettings:ClientUrl"]}/payment/cancel",
                    returnUrl = $"{_configuration["AppSettings:ClientUrl"]}/payment/success?orderCode={orderCode}",
                    expiredAt = DateTime.UtcNow.AddHours(24)
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(paymentRequest),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(
                    $"{_payOsApiUrl}/v2/payment-requests", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"PayOS payment creation failed: {errorContent}");
                    return (false, null, null, $"Payment creation failed: {response.StatusCode}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var paymentResponse = JsonSerializer.Deserialize<PayOsPaymentResponse>(
                    responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (paymentResponse?.Success != true || paymentResponse.Data == null)
                {
                    return (false, null, null, "Invalid response from payment provider");
                }

                return (true, paymentResponse.Data.CheckoutUrl, paymentResponse.Data.OrderCode, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating payment: {ex.Message}");
                return (false, null, null, $"Error creating payment: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Status, string ErrorMessage)> CheckPaymentStatus(string orderCode)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_payOsApiUrl}/v2/payment-requests/{orderCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"PayOS status check failed: {errorContent}");
                    return (false, null, $"Payment status check failed: {response.StatusCode}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var paymentStatus = JsonSerializer.Deserialize<PayOsPaymentStatusResponse>(
                    responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (paymentStatus?.Success != true || paymentStatus.Data == null)
                {
                    return (false, null, "Invalid response from payment provider");
                }

                return (true, paymentStatus.Data.Status, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking payment status: {ex.Message}");
                return (false, null, $"Error checking payment status: {ex.Message}");
            }
        }

        public bool VerifyWebhookSignature(string payload, string signature)
        {
            try
            {
                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_checksumKey)))
                {
                    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                    var computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

                    return computedSignature == signature.ToLower();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error verifying webhook signature: {ex.Message}");
                return false;
            }
        }
    }

    // Response models for PayOS API
    public class PayOsPaymentResponse
    {
        public bool Success { get; set; }
        public PayOsPaymentData Data { get; set; }
    }

    public class PayOsPaymentData
    {
        public string OrderCode { get; set; }
        public string Status { get; set; }
        public string CheckoutUrl { get; set; }
    }

    public class PayOsPaymentStatusResponse
    {
        public bool Success { get; set; }
        public PayOsPaymentStatusData Data { get; set; }
    }

    public class PayOsPaymentStatusData
    {
        public string OrderCode { get; set; }
        public string Status { get; set; }
        public int Amount { get; set; }
    }
}