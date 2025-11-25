using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Common;
using WorkFlowCore.Domain.Data;
using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// 岗位服务实现
/// </summary>
public class PostService : ApplicationService, IPostService
{
    private readonly IRepository<Post, long> _postRepository;

    public PostService(IRepository<Post, long> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<PagedResponse<PostDto>> GetPagedListAsync(PostQueryDto query)
    {
        var queryable = await _postRepository.GetQueryableAsync();

        // 条件过滤
        if (!string.IsNullOrEmpty(query.PostCode))
        {
            queryable = queryable.Where(p => p.PostCode.Contains(query.PostCode));
        }
        if (!string.IsNullOrEmpty(query.PostName))
        {
            queryable = queryable.Where(p => p.PostName.Contains(query.PostName));
        }
        if (!string.IsNullOrEmpty(query.Status))
        {
            queryable = queryable.Where(p => p.Status == query.Status);
        }

        // 排序
        queryable = queryable.OrderBy(p => p.PostSort);

        // 计算总数
        var totalCount = queryable.Count();

        // 分页
        var posts = queryable
            .Skip(query.Skip)
            .Take(query.Take)
            .ToList();

        var dtos = posts.Select(MapToDto).ToList();

        return new PagedResponse<PostDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<List<PostDto>> GetAllAsync()
    {
        var posts = await _postRepository.GetListAsync();
        return posts.OrderBy(p => p.PostSort).Select(MapToDto).ToList();
    }

    public async Task<PostDto?> GetByIdAsync(long id)
    {
        var post = await _postRepository.FindAsync(id);
        return post == null ? null : MapToDto(post);
    }

    public async Task<PostDto> CreateAsync(PostDto dto)
    {
        var post = new Post(SnowflakeIdGenerator.NextId(), CurrentTenant.Id, dto.PostCode, dto.PostName)
        {
            PostSort = dto.PostSort,
            Status = dto.Status,
            Remark = dto.Remark
        };

        await _postRepository.InsertAsync(post);
        return MapToDto(post);
    }

    public async Task UpdateAsync(PostDto dto)
    {
        var post = await _postRepository.GetAsync(dto.Id);

        post.PostCode = dto.PostCode;
        post.PostName = dto.PostName;
        post.PostSort = dto.PostSort;
        post.Status = dto.Status;
        post.Remark = dto.Remark;

        await _postRepository.UpdateAsync(post);
    }

    public async Task DeleteAsync(long[] ids)
    {
        // TODO: 检查岗位是否被用户使用
        // var hasUsers = await _userPostRepository.AnyAsync(up => ids.Contains(up.PostId));
        // if (hasUsers)
        // {
        //     throw new UserFriendlyException("该岗位下存在用户，无法删除");
        // }

        await _postRepository.DeleteManyAsync(ids);
    }

    public async Task<bool> CheckPostNameUniqueAsync(string postName, long? excludeId = null)
    {
        var posts = await _postRepository.GetListAsync();
        var exists = posts.Any(p => 
            p.PostName == postName && 
            (excludeId == null || p.Id != excludeId.Value));
        return !exists;
    }

    public async Task<bool> CheckPostCodeUniqueAsync(string postCode, long? excludeId = null)
    {
        var posts = await _postRepository.GetListAsync();
        var exists = posts.Any(p => 
            p.PostCode == postCode && 
            (excludeId == null || p.Id != excludeId.Value));
        return !exists;
    }

    /// <summary>
    /// 映射到DTO
    /// </summary>
    private PostDto MapToDto(Post post)
    {
        return new PostDto
        {
            Id = post.Id,
            PostCode = post.PostCode,
            PostName = post.PostName,
            PostSort = post.PostSort,
            Status = post.Status,
            Remark = post.Remark,
            CreationTime = post.CreationTime
        };
    }
}

