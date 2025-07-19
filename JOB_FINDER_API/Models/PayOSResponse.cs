using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models.Payment
{
    public class PayOSResponse
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("desc")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public PayOSData? Data { get; set; }
    }

    public class PayOSData
    {
        [JsonPropertyName("paymentLinkId")]
        public string PaymentLinkId { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("checkoutUrl")]
        public string CheckoutUrl { get; set; } = string.Empty;

        [JsonPropertyName("qrCode")]
        public string QrCode { get; set; } = string.Empty;
    }
}