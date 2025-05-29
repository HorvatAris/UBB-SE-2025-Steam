using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Models.Item;
using SteamHub.Web.ViewModels;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;

namespace SteamHub.Web.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly IInventoryService inventoryService;
        private readonly IUserService userService;

        public InventoryController(IInventoryService inventoryService, IUserService userService)
        {
            this.inventoryService = inventoryService;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? selectedUserId, int? selectedGameId, string searchText)
        {
            var model = new InventoryViewModel();

            try
            {
                // Get all users
                var currentUser = await userService.GetCurrentUserAsync();
                if (currentUser == null)
                {
                    model.StatusMessage = "No users found.";
                    return View(model);
                }

                // Get current user or default to first user
                var currentUserId = selectedUserId ?? currentUser?.UserId ?? 0;

                // Get inventory items
                var allItems = await inventoryService.GetUserInventoryAsync(currentUserId);
                
                // Get available games
                var availableGames = await inventoryService.GetAvailableGamesAsync(allItems, currentUserId);
                if (availableGames == null || !availableGames.Any())
                {
                    availableGames = new List<Game> { new Game { GameId = 0, GameTitle = "All Games" } };
                }

                // Ensure we have at least one user
                var availableUsers = new List<IUserDetails> { currentUser };
                if (availableUsers == null || !availableUsers.Any())
                {
                    model.StatusMessage = "No users available.";
                    return View(model);
                }

                // Get selected game if any
                Game selectedGame = null;
                if (selectedGameId.HasValue && selectedGameId.Value > 0)
                {
                    selectedGame = availableGames.FirstOrDefault(g => g.GameId == selectedGameId.Value);
                }

                // Get filtered items
                var filteredItems = await inventoryService.GetUserFilteredInventoryAsync(
                    currentUserId,
                    selectedGame,
                    searchText
                );

                // Update model with retrieved data
                model.SelectedUserId = currentUserId;
                model.SelectedGameId = selectedGameId ?? 0;
                model.SearchText = searchText ?? string.Empty;
                model.InventoryItems = filteredItems ?? new List<Item>();
                model.AvailableGames = availableGames;
                model.AvailableUsers = availableUsers;
            }
            catch (Exception ex)
            {
                model.StatusMessage = $"An error occurred: {ex.Message}";
            }

            // Set status message from TempData if it exists
            if (TempData["StatusMessage"] != null)
            {
                model.StatusMessage = TempData["StatusMessage"].ToString();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sell(int itemId, int selectedUserId, int? selectedGameId, string searchText)
        {
            try
            {
                var item = (await inventoryService.GetUserInventoryAsync(selectedUserId))
                    .FirstOrDefault(i => i.ItemId == itemId);

                if (item != null && !item.IsListed)
                {
                    await inventoryService.SellItemAsync(item, selectedUserId);
                    TempData["StatusMessage"] = $"Item '{item.ItemName}' was successfully listed for sale.";
                }
                else
                {
                    TempData["StatusMessage"] = "Item could not be found or is already listed.";
                }
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] = $"An error occurred while trying to sell the item: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new { selectedUserId, selectedGameId, searchText });
        }
    }
}