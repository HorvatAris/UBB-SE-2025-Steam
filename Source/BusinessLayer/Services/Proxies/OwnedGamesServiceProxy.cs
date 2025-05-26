using System;
using System.Collections.Generic;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Services.Proxies
{
    public class OwnedGamesServiceProxy : ServiceProxy, IOwnedGamesService
    {
        public OwnedGamesServiceProxy(string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
        }

        public List<OwnedGame> GetAllOwnedGames(int userIdentifier)
        {
            try
            {
                return GetAsync<List<OwnedGame>>($"OwnedGames/{userIdentifier}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve owned games", ex);
            }
        }

        public OwnedGame GetOwnedGameByIdentifier(int gameIdentifier, int userIdentifier)
        {
            try
            {
                return GetAsync<OwnedGame>($"OwnedGames/{userIdentifier}/game/{gameIdentifier}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve owned game", ex);
            }
        }

        public void RemoveOwnedGame(int gameIdentifier, int userIdentifier)
        {
            try
            {
                DeleteAsync<object>($"OwnedGames/{userIdentifier}/game/{gameIdentifier}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to remove owned game", ex);
            }
        }
    }
}