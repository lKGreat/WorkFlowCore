using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkFlowCore.API.Filters;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 岗位管理控制器
/// </summary>
[Authorize]
[Route("api/system/post")]
public class PostController : BaseController
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    /// <summary>
    /// 获取岗位列表（分页）
    /// </summary>
    [HttpGet("list")]
    [ActionPermissionFilter(Permission = "system:post:list")]
    [OperationLog("查询岗位", "QUERY")]
    public async Task<ActionResult<ApiResponse<PagedResponse<PostDto>>>> GetList([FromQuery] PostQueryDto query)
    {
        var result = await _postService.GetPagedListAsync(query);
        return ApiResponse<PagedResponse<PostDto>>.Ok(result).ToActionResult();
    }

    /// <summary>
    /// 获取岗位详情
    /// </summary>
    [HttpGet("{id}")]
    [ActionPermissionFilter(Permission = "system:post:query")]
    public async Task<ActionResult<ApiResponse<PostDto>>> GetById(long id)
    {
        var post = await _postService.GetByIdAsync(id);
        if (post == null)
        {
            return ApiResponse<PostDto>.Fail("岗位不存在").ToActionResult();
        }
        return ApiResponse<PostDto>.Ok(post).ToActionResult();
    }

    /// <summary>
    /// 新增岗位
    /// </summary>
    [HttpPost]
    [ActionPermissionFilter(Permission = "system:post:add")]
    [OperationLog("新增岗位", "INSERT")]
    public async Task<ActionResult<ApiResponse<PostDto>>> Create([FromBody] PostDto dto)
    {
        // 检查岗位名称是否唯一
        if (!await _postService.CheckPostNameUniqueAsync(dto.PostName))
        {
            return ApiResponse<PostDto>.Fail($"新增岗位'{dto.PostName}'失败，岗位名称已存在").ToActionResult();
        }

        // 检查岗位编码是否唯一
        if (!await _postService.CheckPostCodeUniqueAsync(dto.PostCode))
        {
            return ApiResponse<PostDto>.Fail($"新增岗位'{dto.PostName}'失败，岗位编码已存在").ToActionResult();
        }

        var result = await _postService.CreateAsync(dto);
        return ApiResponse<PostDto>.Ok(result, "创建成功").ToActionResult();
    }

    /// <summary>
    /// 修改岗位
    /// </summary>
    [HttpPut]
    [ActionPermissionFilter(Permission = "system:post:edit")]
    [OperationLog("修改岗位", "UPDATE")]
    public async Task<ActionResult<ApiResponse<object?>>> Update([FromBody] PostDto dto)
    {
        // 检查岗位名称是否唯一
        if (!await _postService.CheckPostNameUniqueAsync(dto.PostName, dto.Id))
        {
            return ApiResponse<object?>.Fail($"修改岗位'{dto.PostName}'失败，岗位名称已存在").ToActionResult();
        }

        // 检查岗位编码是否唯一
        if (!await _postService.CheckPostCodeUniqueAsync(dto.PostCode, dto.Id))
        {
            return ApiResponse<object?>.Fail($"修改岗位'{dto.PostName}'失败，岗位编码已存在").ToActionResult();
        }

        await _postService.UpdateAsync(dto);
        return ApiResponse<object?>.Ok(null, "更新成功").ToActionResult();
    }

    /// <summary>
    /// 删除岗位
    /// </summary>
    [HttpDelete("{ids}")]
    [ActionPermissionFilter(Permission = "system:post:remove")]
    [OperationLog("删除岗位", "DELETE")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(string ids)
    {
        // 解析ID数组
        var idArray = ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToArray();

        await _postService.DeleteAsync(idArray);
        return ApiResponse<object?>.Ok(null, "删除成功").ToActionResult();
    }

    /// <summary>
    /// 获取岗位下拉选择列表
    /// </summary>
    [HttpGet("optionselect")]
    public async Task<ActionResult<ApiResponse<List<PostDto>>>> GetOptionSelect()
    {
        var posts = await _postService.GetAllAsync();
        return ApiResponse<List<PostDto>>.Ok(posts).ToActionResult();
    }
}

