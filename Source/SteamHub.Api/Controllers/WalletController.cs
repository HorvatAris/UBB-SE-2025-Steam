
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services.Interfaces;

namespace SreamHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService walletService;

        public WalletController(IWalletService walletService)
        {
            this.walletService = walletService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWallet([FromRoute]int userId)
        {
            var balance = await walletService.GetBalance(userId);
            var points = await walletService.GetPoints(userId);

            var wallet = new
            {
                UserId = userId,
                Balance = balance,
                Points = points
            };

            return Ok(wallet);
        }

        [HttpPost("create/{userId}")]
        public async Task<IActionResult> CreateWallet([FromRoute]int userId)
        {
            await walletService.CreateWallet(userId);
            return Ok();
        }

        [HttpPost("add-money")]
        public async Task<IActionResult> AddMoney([FromBody] AddMoneyRequest request)
        {
            await walletService.AddMoney(request.Amount, request.UserId);
            return Ok();
        }

        [HttpPost("credit-points/{userId}")]
        public async Task<IActionResult> CreditPoints(int userId, [FromBody] CreditPointsRequest request)
        {
            await walletService.CreditPoints(userId, request.NumberOfPoints);
            return Ok();
        }
    }

    public class AddMoneyRequest
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
    }

    public class CreditPointsRequest
    {
        public int NumberOfPoints { get; set; }
    }
}