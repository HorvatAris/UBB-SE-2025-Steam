using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Models.ItemTradeDetails;
using SteamHub.ApiContract.Repositories;
namespace SteamHub.Api.Context.Repositories;
public class ItemTradeDetailRepository : IItemTradeDetailRepository
{
    private readonly DataContext context;

    public ItemTradeDetailRepository(DataContext context)
    {
        this.context = context;
    }

    public async Task<GetItemTradeDetailsResponse?> GetItemTradeDetailsAsync()
    {
        var tradeDetails = await context.ItemTradeDetails
            .Select(details => new ItemTradeDetailResponse
            {
                TradeId = details.TradeId,
                ItemId = details.ItemId,
                IsSourceUserItem = details.IsSourceUserItem
            })
            .ToListAsync();

        return new GetItemTradeDetailsResponse
        {
            ItemTradeDetails = tradeDetails
        };
    }

    public async Task<ItemTradeDetailResponse?> GetItemTradeDetailAsync(int tradeId, int itemId)
    {
        var result = await context.ItemTradeDetails
            .Where(details => details.TradeId == tradeId && details.ItemId == itemId)
            .Select(details => new ItemTradeDetailResponse
            {
                TradeId = details.TradeId,
                ItemId = details.ItemId,
                IsSourceUserItem = details.IsSourceUserItem
            })
            .SingleOrDefaultAsync();

        return result;
    }

    public async Task<CreateItemTradeDetailResponse> CreateItemTradeDetailAsync(CreateItemTradeDetailRequest request)
    {
        var newDetail = new ItemTradeDetail
        {
            TradeId = request.TradeId,
            ItemId = request.ItemId,
            IsSourceUserItem = request.IsSourceUserItem
        };

        await context.ItemTradeDetails.AddAsync(newDetail);
        await context.SaveChangesAsync();

        return new CreateItemTradeDetailResponse
        {
            TradeId = newDetail.TradeId,
            ItemId = newDetail.ItemId
        };
    }

    public async Task DeleteItemTradeDetailAsync(int tradeId, int itemId)
    {
        var detail = await context.ItemTradeDetails.FindAsync(tradeId, itemId);
        if (detail == null)
        {
            throw new Exception("ItemTradeDetail not found");
        }

        context.ItemTradeDetails.Remove(detail);
        await context.SaveChangesAsync();
    }
}