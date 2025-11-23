using Volo.Abp.Application.Services;
using WorkFlowCore.Application.DTOs.User;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 应用用户服务接口
/// </summary>
public interface IAppUserService : IApplicationService
{
    /// <summary>
    /// 分页查询用户
    /// </summary>
    Task<PagedResponse<UserListDto>> GetPagedAsync(UserPagedRequest request);

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    Task<UserListDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建用户
    /// </summary>
    Task<UserListDto> CreateAsync(CreateUserInput input);

    /// <summary>
    /// 更新用户
    /// </summary>
    Task UpdateAsync(UpdateUserInput input);

    /// <summary>
    /// 删除用户(批量)
    /// </summary>
    Task DeleteAsync(List<Guid> ids);

    /// <summary>
    /// 重置密码
    /// </summary>
    Task ResetPasswordAsync(ResetPasswordInput input);

    /// <summary>
    /// 更改状态
    /// </summary>
    Task ChangeStatusAsync(ChangeStatusInput input);
}

