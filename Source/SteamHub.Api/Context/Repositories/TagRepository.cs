﻿using Microsoft.EntityFrameworkCore;
using SteamHub.ApiContract.Models.Tag;
using SteamHub.ApiContract.Repositories;
using Tag = SteamHub.Api.Entities.Tag;


namespace SteamHub.Api.Context.Repositories;

public class TagRepository : ITagRepository
{
	private readonly DataContext _context;

	public TagRepository(DataContext context)
	{
		_context = context;
	}

	public async Task<CreateTagResponse> CreateTagAsync(CreateTagRequest request)
	{
		var isDuplicate = await _context.Tags
			.AnyAsync(tag => tag.TagName == request.TagName);

		if (isDuplicate)
		{
			throw new ArgumentException($"Tag with name {request.TagName} already exists");
		}

		var newTag = new Tag
		{
			TagName = request.TagName
		};

		_context.Add(newTag);

		await _context.SaveChangesAsync();

		return new CreateTagResponse
		{
			TagId = newTag.TagId
		};
	}

	public async Task<TagNameOnlyResponse?> GetTagByIdAsync(int tagId)
	{
		var foundTag = await _context.Tags
			.Where(tag => tag.TagId == tagId)
			.Select(tag => new TagNameOnlyResponse
			{
				TagName = tag.TagName
			})
			.SingleOrDefaultAsync();

		return foundTag;
	}

	public async Task<GetTagsResponse> GetAllTagsAsync()
	{
		var tags = await _context.Tags
			.Select(tag => new TagSummaryResponse
			{
				TagId = tag.TagId,
				TagName = tag.TagName
			})
			.ToListAsync();

		return new GetTagsResponse
		{
			Tags = tags
		};
	}

	public async Task UpdateTagAsync(int tagId, UpdateTagRequest request)
	{
		var foundTag = await _context.Tags
			.Where(tag => tag.TagId == tagId)
			.SingleOrDefaultAsync();

		if (foundTag is null)
		{
			throw new ArgumentException($"Tag with id {tagId} was not found");
		}

		foundTag.TagName = request.TagName;
		await _context.SaveChangesAsync();
	}

	public async Task DeleteTagAsync(int tagId)
	{
		var foundTag = await _context.Tags
			.Where(tag => tag.TagId == tagId)
			.SingleOrDefaultAsync();

		if (foundTag is null)
		{
			throw new ArgumentException($"Tag with id {tagId} was not found");
		}

		_context.Tags.Remove(foundTag);

		await _context.SaveChangesAsync();
	}
}
