using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WorkFlowCore.Domain.Identity;
using WorkFlowCore.Infrastructure.Data;

namespace WorkFlowCore.Infrastructure.Repositories;

/// <summary>
/// AppUser 仓储实现
/// </summary>
public class AppUserRepository : EfCoreRepository<WorkFlowDbContext, AppUser, Guid>
{
    public AppUserRepository(IDbContextProvider<WorkFlowDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}


