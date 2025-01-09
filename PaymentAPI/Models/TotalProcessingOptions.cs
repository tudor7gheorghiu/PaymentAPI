namespace PaymentAPI.Models
{
    public class TotalProcessingOptions
    {
        public string EntityId { get; set; } = string.Empty;
        public string BearerToken { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty; 
    }
}
