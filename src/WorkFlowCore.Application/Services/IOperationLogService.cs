using Volo.Abp.Application.Services;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Services;

public interface IOperationLogService : IApplicationService
{
    Task<PagedResponse<OperationLogDto>> GetPagedAsync(OperationLogPagedRequest request);
    Task<OperationLogDto?> GetByIdAsync(long id);
    Task DeleteAsync(List<long> ids);
    Task CleanAsync();
}

public class OperationLogDto
{
    public long OperLogId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public string RequestMethod { get; set; } = string.Empty;
    public string RequestUrl { get; set; } = string.Empty;
    public string? OperatorName { get; set; }
    public string? OperatorIp { get; set; }
    public int ExecutionTime { get; set; }
    public string Status { get; set; } = "0";
    public string? ErrorMsg { get; set; }
    public DateTime CreationTime { get; set; }
}

public class OperationLogPagedRequest : PagedRequest
{
    public string? Title { get; set; }
    public string? BusinessType { get; set; }
    public string? Status { get; set; }
    public string? OperatorName { get; set; }
    public DateTime? BeginTime { get; set; }
    public DateTime? EndTime { get; set; }
}

