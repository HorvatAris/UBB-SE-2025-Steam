using SteamHub.ApiContract.Models.Collections;

namespace SteamHub.Api.Entities;

public class CollectionGame
{
    public int CollectionId { get; set; }

    public int GameId { get; set; }

    public Collection Collection { get; set; }
    public OwnedGame OwnedGame { get; set; }
}