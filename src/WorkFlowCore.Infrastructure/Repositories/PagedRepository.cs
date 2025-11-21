using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Infrastructure.Data;

namespace WorkFlowCore.Infrastructure.Repositories;

/// <summary>
/// 支持分页的仓储实现
/// </summary>
public class PagedRepository<TEntity> : Repository<TEntity>, IPagedRepository<TEntity> where TEntity : class
{
    public PagedRepository(WorkFlowDbContext context) : base(context)
    {
    }

    public virtual async Task<PagedResponse<TEntity>> GetPagedAsync(
        int pageIndex,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool descending = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        // 应用过滤条件
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        // 获取总数
        var totalCount = await query.CountAsync(cancellationToken);

        // 应用排序
        if (orderBy != null)
        {
            query = descending
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);
        }

        // 应用分页
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResponse<TEntity>.Create(items, totalCount, pageIndex, pageSize);
    }

    public virtual async Task<PagedResponse<TResult>> GetPagedAsync<TResult>(
        int pageIndex,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool descending = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        // 应用过滤条件
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        // 获取总数
        var totalCount = await query.CountAsync(cancellationToken);

        // 应用排序
        if (orderBy != null)
        {
            query = descending
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);
        }

        // 应用分页和投影
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(selector)
            .ToListAsync(cancellationToken);

        return PagedResponse<TResult>.Create(items, totalCount, pageIndex, pageSize);
    }
}

