using System.Linq.Expressions;

namespace WorkFlowCore.Infrastructure.Repositories;

/// <summary>
/// 仓储接口
/// </summary>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    Task<TEntity?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有实体
    /// </summary>
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件查询
    /// </summary>
    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加实体
    /// </summary>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新实体
    /// </summary>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除实体
    /// </summary>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 保存更改
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

