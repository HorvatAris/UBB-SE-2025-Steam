﻿using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Context;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Models.PointShopItem;
using SteamHub.ApiContract.Repositories;
using PointShopItem = SteamHub.Api.Entities.PointShopItem;

namespace SteamHub.Api.Context.Repositories;

public class PointShopItemRepository : IPointShopItemRepository
{
    private readonly DataContext context;

    public PointShopItemRepository(DataContext context)
    {
        this.context = context;
    }
    public async Task<GetPointShopItemsResponse?> GetPointShopItemsAsync()
    {
        var pointShopItems = await context.PointShopItems
            .Select(pointShopItem => new PointShopItemResponse
            {
                PointShopItemId = pointShopItem.PointShopItemId,
                Name = pointShopItem.Name,
                Description = pointShopItem.Description,
                ImagePath = pointShopItem.ImagePath,
                PointPrice = pointShopItem.PointPrice,
                ItemType = pointShopItem.ItemType
            })
            .ToListAsync();

        return new GetPointShopItemsResponse
        {
            PointShopItems = pointShopItems
        };
    }

    public async Task<PointShopItemResponse?> GetPointShopItemByIdAsync(int id)
    {
        var result = await context.PointShopItems
            .Where(item => item.PointShopItemId == id)
            .Select(pointShopItem => new PointShopItemResponse
            {
                PointShopItemId = pointShopItem.PointShopItemId,
                Name = pointShopItem.Name,
                Description = pointShopItem.Description,
                ImagePath = pointShopItem.ImagePath,
                PointPrice = pointShopItem.PointPrice,
                ItemType = pointShopItem.ItemType
            })
            .SingleOrDefaultAsync();

        return result;
    }

    public async Task UpdatePointShopItemAsync(int itemId, UpdatePointShopItemRequest request)
    {
        var existingPointShopItem = await context.PointShopItems.FindAsync(itemId);
        if (existingPointShopItem == null)
        {
            throw new Exception("PointShopItem not found");
        }

        existingPointShopItem.Name = request.Name;
        existingPointShopItem.Description = request.Description;
        existingPointShopItem.ImagePath = request.ImagePath;
        existingPointShopItem.PointPrice = request.PointPrice;
        existingPointShopItem.ItemType = request.ItemType;

        await context.SaveChangesAsync();
    }

    public async Task<CreatePointShopItemResponse> CreatePointShopItemAsync(CreatePointShopItemRequest request)
    {
        PointShopItem newPointShopItem = new PointShopItem
        {
            Name = request.Name,
            Description = request.Description,
            ImagePath = request.ImagePath,
            PointPrice = request.PointPrice,
            ItemType = request.ItemType
        };

        await context.PointShopItems.AddAsync(newPointShopItem);

        await context.SaveChangesAsync();

        return new CreatePointShopItemResponse
        {
            PointShopItemId = newPointShopItem.PointShopItemId
        };
    }

    public async Task DeletePointShopItemAsync(int id)
    {
        var pointShopItem = await context.PointShopItems.FindAsync(id);
        if (pointShopItem == null)
        {
            throw new Exception("PointShopItem not found");
        }
        context.PointShopItems.Remove(pointShopItem);
        await context.SaveChangesAsync();
    }

}