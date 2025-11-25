using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Common;
using WorkFlowCore.Domain.Data;
using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// 定时任务服务实现（简化版，不包含Quartz集成）
/// </summary>
public class TaskService : ApplicationService, ITaskService
{
    private readonly IRepository<SysTask, long> _taskRepository;
    private readonly IRepository<SysTaskLog, long> _taskLogRepository;

    public TaskService(
        IRepository<SysTask, long> taskRepository,
        IRepository<SysTaskLog, long> taskLogRepository)
    {
        _taskRepository = taskRepository;
        _taskLogRepository = taskLogRepository;
    }

    public async Task<PagedResponse<TaskDto>> GetPagedListAsync(TaskQueryDto query)
    {
        var queryable = await _taskRepository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(query.TaskName))
        {
            queryable = queryable.Where(t => t.TaskName.Contains(query.TaskName));
        }
        if (!string.IsNullOrEmpty(query.TaskGroup))
        {
            queryable = queryable.Where(t => t.TaskGroup == query.TaskGroup);
        }
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(t => t.Status == query.Status.Value);
        }

        queryable = queryable.OrderBy(t => t.CreationTime);

        var totalCount = queryable.Count();
        var tasks = queryable.Skip(query.Skip).Take(query.Take).ToList();
        var dtos = tasks.Select(MapToDto).ToList();

        return new PagedResponse<TaskDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<TaskDto?> GetByIdAsync(long id)
    {
        var task = await _taskRepository.FindAsync(id);
        return task == null ? null : MapToDto(task);
    }

    public async Task<TaskDto> CreateAsync(TaskDto dto)
    {
        var task = new SysTask(SnowflakeIdGenerator.NextId(), CurrentTenant.Id, dto.TaskName)
        {
            TaskGroup = dto.TaskGroup,
            InvokeTarget = dto.InvokeTarget,
            CronExpression = dto.CronExpression,
            Status = dto.Status,
            Concurrent = dto.Concurrent,
            MisfirePolicy = dto.MisfirePolicy,
            Remark = dto.Remark
        };

        await _taskRepository.InsertAsync(task);
        return MapToDto(task);
    }

    public async Task UpdateAsync(TaskDto dto)
    {
        var task = await _taskRepository.GetAsync(dto.Id);

        task.TaskName = dto.TaskName;
        task.TaskGroup = dto.TaskGroup;
        task.InvokeTarget = dto.InvokeTarget;
        task.CronExpression = dto.CronExpression;
        task.Status = dto.Status;
        task.Concurrent = dto.Concurrent;
        task.MisfirePolicy = dto.MisfirePolicy;
        task.Remark = dto.Remark;

        await _taskRepository.UpdateAsync(task);
    }

    public async Task DeleteAsync(long[] ids)
    {
        await _taskRepository.DeleteManyAsync(ids);
        // 删除相关日志
        await _taskLogRepository.DeleteAsync(log => ids.Contains(log.TaskId));
    }

    public async Task<PagedResponse<TaskLogDto>> GetLogPagedListAsync(TaskLogQueryDto query)
    {
        var queryable = await _taskLogRepository.GetQueryableAsync();

        if (query.TaskId.HasValue)
        {
            queryable = queryable.Where(l => l.TaskId == query.TaskId.Value);
        }
        if (!string.IsNullOrEmpty(query.TaskName))
        {
            queryable = queryable.Where(l => l.TaskName.Contains(query.TaskName));
        }
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(l => l.Status == query.Status.Value);
        }
        if (query.BeginTime.HasValue)
        {
            queryable = queryable.Where(l => l.CreationTime >= query.BeginTime.Value);
        }
        if (query.EndTime.HasValue)
        {
            queryable = queryable.Where(l => l.CreationTime <= query.EndTime.Value);
        }

        queryable = queryable.OrderByDescending(l => l.CreationTime);

        var totalCount = queryable.Count();
        var logs = queryable.Skip(query.Skip).Take(query.Take).ToList();
        var dtos = logs.Select(MapLogToDto).ToList();

        return new PagedResponse<TaskLogDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task ClearLogsAsync(long taskId)
    {
        await _taskLogRepository.DeleteAsync(log => log.TaskId == taskId);
    }

    private TaskDto MapToDto(SysTask task)
    {
        return new TaskDto
        {
            Id = task.Id,
            TaskName = task.TaskName,
            TaskGroup = task.TaskGroup,
            InvokeTarget = task.InvokeTarget,
            CronExpression = task.CronExpression,
            Status = task.Status,
            Concurrent = task.Concurrent,
            MisfirePolicy = task.MisfirePolicy,
            Remark = task.Remark,
            CreationTime = task.CreationTime
        };
    }

    private TaskLogDto MapLogToDto(SysTaskLog log)
    {
        return new TaskLogDto
        {
            Id = log.Id,
            TaskId = log.TaskId,
            TaskName = log.TaskName,
            TaskGroup = log.TaskGroup,
            InvokeTarget = log.InvokeTarget,
            Status = log.Status,
            LogInfo = log.LogInfo,
            Exception = log.Exception,
            StartTime = log.StartTime,
            EndTime = log.EndTime,
            Duration = log.Duration,
            CreationTime = log.CreationTime
        };
    }
}

