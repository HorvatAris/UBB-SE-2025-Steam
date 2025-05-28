﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Tag;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;
using System.Collections.ObjectModel;

namespace SteamHub.Web.Controllers
{
    [Authorize(Roles = "Developer")]
    public class DeveloperController : Controller
    {
        private readonly IDeveloperService developerService;
        private IUserDetails user;

        public DeveloperController(IDeveloperService developerService)
        {
            this.developerService = developerService;
            this.user = developerService.GetCurrentUser();
        }
        // GET: Developer/MyGames
        public async Task<IActionResult> MyGames()
        {
            var games = await developerService.GetDeveloperGamesAsync(this.user.UserId);
            return View(games); // View expects a list of Game
        }
        // GET: /Developer/UnvalidatedGames
        public async Task<IActionResult> UnvalidatedGames()
        {
            var games = await developerService.GetUnvalidatedAsync(this.user.UserId);
            return View(games); // View expects a list of Game
        }
        public async Task<IActionResult> Create()
        {
            return View(new CreateGameViewModel());
        }

        // POST: /Developer/Create
        [HttpPost]
        public async Task<IActionResult> Create(CreateGameViewModel game_view_model)
        {
            if (!ModelState.IsValid)
            {
                //game_view_model.AllTags = (await developerService.GetAllTagsAsync()).ToList();
                return View(game_view_model);
            }

            var game = await developerService.CreateValidatedGameAsync(
                game_view_model.GameId,
                game_view_model.Name,
                game_view_model.Price,
                game_view_model.Description,
                game_view_model.ImageUrl,
                game_view_model.TrailerUrl,
                game_view_model.GameplayUrl,
                game_view_model.MinimumRequirement,
                game_view_model.RecommendedRequirement,
                game_view_model.Discount,
                new List<Tag> { new Tag { TagId = 500, Tag_name = "bagpl" } },
                this.developerService.GetCurrentUser().UserId

            );

            return RedirectToAction("MyGames");
        }


        //Fix for CS1503: Convert the List<Game> to ObservableCollection<Game> before passing it to the method.
        public async Task<IActionResult> Edit(int id)
        {
            var allGames = new ObservableCollection<Game>((await developerService.GetDeveloperGamesAsync(this.user.UserId)).ToList());
            var game = developerService.FindGameInObservableCollectionById(id, allGames);
            if (game == null) return NotFound();


            var tags = (await developerService.GetAllTagsAsync()).ToList();
            var selectedTags = (await developerService.GetGameTagsAsync(id))
                   .Select(t => t.TagId)
                   .ToList();

            var game_view_model = new EditGameViewModel
            {
                GameId = game.GameId.ToString(),
                Name = game.GameTitle,
                Price = game.Price.ToString(),
                Description = game.GameDescription,
                ImageUrl = game.ImagePath,
                TrailerUrl = game.TrailerPath,
                GameplayUrl = game.GameplayPath,
                MinimumRequirement = game.MinimumRequirements,
                RecommendedRequirement = game.RecommendedRequirements,
                Discount = game.Discount.ToString(),
                AllTags = tags,
                SelectedTags = selectedTags
            };

            return View(game_view_model);
        }

        //// POST: /Developer/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(EditGameViewModel game_view_model)
        {
            if (!ModelState.IsValid)
            {
                game_view_model.AllTags = (await developerService.GetAllTagsAsync()).ToList(); // Explicit conversion to List<Tag>

                return View(game_view_model);
            }
            var allTags = await developerService.GetAllTagsAsync();
            // Fix for CS1503: Convert the string GameId to an integer before passing it to the method.
            var selectedTags = game_view_model.SelectedTags;
            var selectedTagObjects = allTags
        .Where(tag => selectedTags.Contains(tag.TagId))  // Find tags that match the selected tag IDs
        .ToList();

            var game = developerService.ValidateInputForAddingAGame(
                game_view_model.GameId,
                game_view_model.Name,
                game_view_model.Price,
                game_view_model.Description,
                game_view_model.ImageUrl,
                game_view_model.TrailerUrl,
                game_view_model.GameplayUrl,
                game_view_model.MinimumRequirement,
                game_view_model.RecommendedRequirement,
                game_view_model.Discount,
                selectedTagObjects
            );

            await developerService.UpdateGameWithTagsAsync(game, selectedTagObjects, this.user.UserId);
            return RedirectToAction("MyGames");
        }

        // POST: /Developer/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var developerGames = new ObservableCollection<Game>((await developerService.GetDeveloperGamesAsync(this.user.UserId)).ToList());
            await developerService.DeleteGameAsync(id, developerGames);
            return RedirectToAction("MyGames");
        }
        [HttpGet]
        public async Task<JsonResult> GetGameOwnerCount(int id)
        {
            var count = await developerService.GetGameOwnerCountAsync(id);
            return Json(count);
        }


        // POST: /Developer/Validate/5
        [HttpPost]
        public async Task<IActionResult> Validate(int id)
        {
            await developerService.ValidateGameAsync(id);
            return RedirectToAction("UnvalidatedGames");
        }

        // GET: /Developer/Reject/5
        public async Task<IActionResult> Reject(int id)
        {
            var game_view_model = new RejectGameViewModel { GameId = id };
            return View(game_view_model);
        }

        // Fix for CS1503: Convert the List<Game> to ObservableCollection<Game> before passing it to the method.
        [HttpPost]
        public async Task<IActionResult> Reject(RejectGameViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.RejectionMessage))
            {
                await developerService.RejectGameWithMessageAsync(model.GameId, model.RejectionMessage);
            }
            else
            {
                var unvalidatedGames = new ObservableCollection<Game>((await developerService.GetUnvalidatedAsync(this.user.UserId)).ToList());
                await developerService.RejectGameAndRemoveFromUnvalidatedAsync(model.GameId, unvalidatedGames);
            }

            return RedirectToAction("UnvalidatedGames");
        }

        // GET: /Developer/GameTags/5
        public async Task<IActionResult> GameTags(int id)
        {
            var tags = await developerService.GetGameTagsAsync(id);
            return Json(tags);
        }
        // GET: /Developer/RejectionMessage/5
        public async Task<IActionResult> RejectionMessage(int id)
        {
            try
            {
                string rejection_message = await developerService.GetRejectionMessageAsync(id);

                if (string.IsNullOrWhiteSpace(rejection_message))
                {
                    //TempData["Error"] = "No rejection rejection_message found.";
                    //return RedirectToAction("MyGames"); // or wherever makes sense
                    rejection_message = "No rejection rejection_message available for this game.";
                }

                var model = new RejectionMessageViewModel
                {
                    GameId = id,
                    Message = rejection_message
                };

                return View(model);
            }
            catch (Exception exception)
            {
                TempData["Error"] = $"Failed to get rejection rejection_message: {exception.Message}";
                return RedirectToAction("MyGames");
            }
        }




        public IActionResult Index()
        {
            return View();
        }
    }
}
