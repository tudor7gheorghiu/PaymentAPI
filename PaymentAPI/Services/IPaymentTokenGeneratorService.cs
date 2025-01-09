using PaymentAPI.Models;

namespace PaymentAPI.Services
{
    public interface IPaymentTokenGeneratorService
    {
        public Task<Dictionary<string, dynamic>> SendPostRequestAsync(PaymentInfo paymentInfo);
    }
}
