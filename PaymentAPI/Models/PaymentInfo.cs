namespace PaymentAPI.Models
{
    public class PaymentInfo
    {
        public double Amount { get; set; }
        public string? Currency { get; set; }
        public string? PaymentType { get; set; }
    }
}
