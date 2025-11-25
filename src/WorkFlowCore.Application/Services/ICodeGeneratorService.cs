using Volo.Abp.Application.Services;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 代码生成器服务接口（简化版，基础框架）
/// </summary>
public interface ICodeGeneratorService : IApplicationService
{
    /// <summary>
    /// 获取数据库表列表
    /// </summary>
    Task<PagedResponse<DbTableInfo>> GetDbTablesAsync(DbTableQueryDto query);

    /// <summary>
    /// 获取表详细信息
    /// </summary>
    Task<GenTableDto?> GetTableInfoAsync(string tableName);

    /// <summary>
    /// 预览生成代码
    /// </summary>
    Task<Dictionary<string, string>> PreviewCodeAsync(string tableName);

    /// <summary>
    /// 生成代码（返回文件内容）
    /// </summary>
    Task<Dictionary<string, string>> GenerateCodeAsync(string tableName);
}

/// <summary>
/// 数据库表信息
/// </summary>
public class DbTableInfo
{
    public string TableName { get; set; } = string.Empty;
    public string? TableComment { get; set; }
    public DateTime? CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
}

