using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models.Payment
{
    public class PayOSRequest
    {
        [JsonPropertyName("orderCode")]
        public string OrderCode { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("buyerName")]
        public string BuyerName { get; set; } = string.Empty;

        [JsonPropertyName("buyerEmail")]
        public string BuyerEmail { get; set; } = string.Empty;

        [JsonPropertyName("buyerPhone")]
        public string BuyerPhone { get; set; } = string.Empty;

        [JsonPropertyName("cancelUrl")]
        public string CancelUrl { get; set; } = string.Empty;

        [JsonPropertyName("returnUrl")]
        public string ReturnUrl { get; set; } = string.Empty;

        [JsonPropertyName("signature")]
        public string Signature { get; set; } = string.Empty;

        [JsonPropertyName("expiredAt")]
        public long ExpiredAt { get; set; }
    }
}