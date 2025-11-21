using System.Linq.Expressions;
using WorkFlowCore.Application.Common;

namespace WorkFlowCore.Infrastructure.Repositories;

/// <summary>
/// 支持分页的仓储接口
/// </summary>
public interface IPagedRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResponse<TEntity>> GetPagedAsync(
        int pageIndex,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool descending = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 分页查询（带投影）
    /// </summary>
    Task<PagedResponse<TResult>> GetPagedAsync<TResult>(
        int pageIndex,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool descending = false,
        CancellationToken cancellationToken = default);
}

