using System;
using Xunit;
using System.Linq;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using BusinessLayer.Exceptions;
using System.Linq;

namespace BusinessLayer.Tests.Repositories
{
    public class WalletRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly WalletRepository _repository;

        public WalletRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _repository = new WalletRepository(_context);
        }

        private void SeedWallet(int userId = 1, decimal balance = 100, int points = 50)
        {
            _context.Wallets.Add(new Wallet { UserId = userId, Balance = balance, Points = points });
            _context.SaveChanges();
        }

        [Fact]
        public void GetWallet_Succeeds_WhenValidInput()
        {
            SeedWallet();
            var result = _repository.GetWallet(1);
            Assert.Equal(1, result.WalletId);
        }

        [Fact]
        public void GetWallet_ThrowsException_WhenInvalidInput()
        {
            Assert.Throws<RepositoryException>(() => _repository.GetWallet(999));
        }

        [Fact]
        public void GetWalletIdByUserId_Succeeds_WhenValidInput()
        {
            SeedWallet();
            var id = _repository.GetWalletIdByUserId(1);
            Assert.Equal(1, id);
        }

        [Fact]
        public void GetWalletIdByUserId_ThrowsException_WhenInvalidInput()
        {
            Assert.Throws<RepositoryException>(() => _repository.GetWalletIdByUserId(999));
        }

        [Fact]
        public void AddMoneyToWallet_Succeeds_WhenValidInput()
        {
            SeedWallet();
            _repository.AddMoneyToWallet(50, 1);
            Assert.Equal(150, _context.Wallets.First().Balance);
        }

        [Fact]
        public void AddMoneyToWallet_ThrowsException_WhenInvalidInput()
        {
            Assert.Throws<RepositoryException>(() => _repository.AddMoneyToWallet(50, 999));
        }

        [Fact]
        public void AddPointsToWallet_Succeeds_WhenValidInput()
        {
            SeedWallet();
            _repository.AddPointsToWallet(20, 1);
            Assert.Equal(70, _context.Wallets.First().Points);
        }

        [Fact]
        public void AddPointsToWallet_ThrowsException_WhenInvalidInput()
        {
            Assert.Throws<RepositoryException>(() => _repository.AddPointsToWallet(10, 999));
        }

        [Fact]
        public void GetMoneyFromWallet_Succeeds_WhenValidInput()
        {
            SeedWallet();
            var money = _repository.GetMoneyFromWallet(1);
            Assert.Equal(100, money);
        }

        [Fact]
        public void GetMoneyFromWallet_ThrowsException_WhenInvalidInput()
        {
            Assert.Throws<RepositoryException>(() => _repository.GetMoneyFromWallet(999));
        }

        [Fact]
        public void GetPointsFromWallet_Succeeds_WhenValidInput()
        {
            SeedWallet();
            var points = _repository.GetPointsFromWallet(1);
            Assert.Equal(50, points);
        }

        [Fact]
        public void GetPointsFromWallet_ThrowsException_WhenInvalidInput()
        {
            Assert.Throws<RepositoryException>(() => _repository.GetPointsFromWallet(999));
        }

        [Fact]
        public void BuyWithMoney_Succeeds_WhenValidInput()
        {
            SeedWallet();
            _repository.BuyWithMoney(30, 1);
            Assert.Equal(70, _context.Wallets.First().Balance);
        }

        [Fact]
        public void BuyWithMoney_ThrowsException_WhenInvalidInput()
        {
            Assert.Throws<RepositoryException>(() => _repository.BuyWithMoney(10, 999));
        }

        [Fact]
        public void BuyWithPoints_Succeeds_WhenValidInput()
        {
            SeedWallet();
            _repository.BuyWithPoints(10, 1);
            Assert.Equal(40, _context.Wallets.First().Points);
        }

        [Fact]
        public void BuyWithPoints_ThrowsException_WhenInvalidInput()
        {
            Assert.Throws<RepositoryException>(() => _repository.BuyWithPoints(10, 999));
        }

        [Fact]
        public void AddNewWallet_Succeeds_WhenCalled()
        {
            _repository.AddNewWallet(2);
            Assert.Single(_context.Wallets.Where(w => w.UserId == 2));
        }

        [Fact]
        public void RemoveWallet_Succeeds_WhenWalletExists()
        {
            SeedWallet();
            _repository.RemoveWallet(1);
            Assert.Empty(_context.Wallets);
        }

        [Fact]
        public void RemoveWallet_DoesNothing_WhenWalletDoesNotExist()
        {
            _repository.RemoveWallet(999);
            Assert.Empty(_context.Wallets);
        }

        [Fact]
        public void WinPoints_Succeeds_WhenValidInput()
        {
            SeedWallet();
            _repository.WinPoints(10, 1);
            Assert.Equal(60, _context.Wallets.First().Points);
        }

        [Fact]
        public void WinPoints_ThrowsException_WhenInvalidInput()
        {
            Assert.Throws<RepositoryException>(() => _repository.WinPoints(10, 999));
        }
    }
}