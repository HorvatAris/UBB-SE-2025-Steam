using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SteamWebApi.Controllers
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
        public IActionResult GetWallet(int userId)
        {
            var balance = walletService.GetBalance();
            var points = walletService.GetPoints();

            var wallet = new
            {
                UserId = userId,
                Balance = balance,
                Points = points
            };

            return Ok(wallet);
        }

        [HttpPost("create/{userId}")]
        public IActionResult CreateWallet(int userId)
        {
            walletService.CreateWallet(userId);
            return Ok();
        }

        [HttpPost("add-money")]
        public IActionResult AddMoney([FromBody] AddMoneyRequest request)
        {
            walletService.AddMoney(request.Amount);
            return Ok();
        }

        [HttpPost("credit-points/{userId}")]
        public IActionResult CreditPoints(int userId, [FromBody] CreditPointsRequest request)
        {
            walletService.CreditPoints(userId, request.NumberOfPoints);
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