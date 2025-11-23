using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.API.Filters;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 菜单管理控制器
/// </summary>
[Authorize]
[Route("api/[controller]")]
public class MenuController : BaseController
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// 获取菜单列表
    /// </summary>
    [HttpGet("list")]
    [OperationLog("查询菜单", "QUERY")]
    public async Task<ActionResult<ApiResponse<List<MenuDto>>>> GetList()
    {
        var menus = await _menuService.GetListAsync();
        return ApiResponse<List<MenuDto>>.Ok(menus).ToActionResult();
    }

    /// <summary>
    /// 获取菜单树
    /// </summary>
    [HttpGet("treelist")]
    [OperationLog("查询菜单树", "QUERY")]
    public async Task<ActionResult<ApiResponse<List<MenuDto>>>> GetTree()
    {
        var tree = await _menuService.GetTreeAsync();
        return ApiResponse<List<MenuDto>>.Ok(tree).ToActionResult();
    }

    /// <summary>
    /// 获取菜单详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<MenuDto>>> GetMenu(long id)
    {
        var menus = await _menuService.GetListAsync();
        var menu = menus.FirstOrDefault(m => m.MenuId == id);
        
        if (menu == null)
        {
            return ApiResponse<MenuDto>.Fail("菜单不存在").ToActionResult();
        }

        return ApiResponse<MenuDto>.Ok(menu).ToActionResult();
    }

    /// <summary>
    /// 创建菜单
    /// </summary>
    [HttpPut("add")]
    [OperationLog("新增菜单", "INSERT")]
    public async Task<ActionResult<ApiResponse<MenuDto>>> Create([FromBody] MenuDto dto)
    {
        var menu = await _menuService.CreateAsync(dto);
        return ApiResponse<MenuDto>.Ok(menu, "创建成功").ToActionResult();
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
    [HttpPost("edit")]
    [OperationLog("修改菜单", "UPDATE")]
    public async Task<ActionResult<ApiResponse<object?>>> Update([FromBody] MenuDto dto)
    {
        await _menuService.UpdateAsync(dto);
        return ApiResponse<object?>.Ok(null, "更新成功").ToActionResult();
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    [HttpDelete("{id}")]
    [OperationLog("删除菜单", "DELETE")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(long id)
    {
        await _menuService.DeleteAsync(id);
        return ApiResponse<object?>.Ok(null, "删除成功").ToActionResult();
    }

    /// <summary>
    /// 获取菜单下拉树
    /// </summary>
    [HttpGet("treeSelect")]
    public async Task<ActionResult<ApiResponse<List<MenuDto>>>> TreeSelect()
    {
        var tree = await _menuService.GetTreeAsync();
        return ApiResponse<List<MenuDto>>.Ok(tree).ToActionResult();
    }

    /// <summary>
    /// 根据角色ID查询菜单
    /// </summary>
    [HttpGet("roleMenuTreeselect/{roleId}")]
    public async Task<ActionResult<ApiResponse<object>>> RoleMenuTreeSelect(Guid roleId)
    {
        // TODO: 实现根据角色ID查询已分配的菜单
        var tree = await _menuService.GetTreeAsync();
        return ApiResponse<object>.Ok(new { menus = tree, checkedKeys = new long[] { } }).ToActionResult();
    }
}

