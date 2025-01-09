using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Models;
using PaymentAPI.Services;

namespace PaymentAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentGeneratorController : ControllerBase
    {
        private readonly IPaymentTokenGeneratorService _paymentTokenGeneratorService;
        private readonly ILogger<PaymentGeneratorController> _logger;

        public PaymentGeneratorController(IPaymentTokenGeneratorService paymentTokenGeneratorService,
                                          ILogger<PaymentGeneratorController> logger)
        {
            _paymentTokenGeneratorService = paymentTokenGeneratorService;
            _logger = logger;
        }

        [HttpPost(Name = "CreatePaymentToken")]
        public async Task<IActionResult> GetPaymentToken(PaymentInfo paymentInfo)
        {
            // 3RD party API does not require any validation; using validation for future developemnt
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await _paymentTokenGeneratorService.SendPostRequestAsync(paymentInfo);
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { error = ex.Message});
            }
        }
    }
}
