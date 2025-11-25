using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 代码生成表信息DTO
/// </summary>
public class GenTableDto
{
    /// <summary>
    /// 表名
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// 表注释
    /// </summary>
    public string? TableComment { get; set; }

    /// <summary>
    /// 实体类名
    /// </summary>
    public string? ClassName { get; set; }

    /// <summary>
    /// 模块名
    /// </summary>
    public string ModuleName { get; set; } = "System";

    /// <summary>
    /// 业务名
    /// </summary>
    public string BusinessName { get; set; } = string.Empty;

    /// <summary>
    /// 功能名
    /// </summary>
    public string FunctionName { get; set; } = string.Empty;

    /// <summary>
    /// 作者
    /// </summary>
    public string Author { get; set; } = "Admin";

    /// <summary>
    /// 表列信息
    /// </summary>
    public List<GenTableColumnDto> Columns { get; set; } = new();
}

/// <summary>
/// 表列信息DTO
/// </summary>
public class GenTableColumnDto
{
    /// <summary>
    /// 列名
    /// </summary>
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// 列注释
    /// </summary>
    public string? ColumnComment { get; set; }

    /// <summary>
    /// 列类型
    /// </summary>
    public string ColumnType { get; set; } = string.Empty;

    /// <summary>
    /// C#类型
    /// </summary>
    public string CsharpType { get; set; } = string.Empty;

    /// <summary>
    /// C#字段名
    /// </summary>
    public string CsharpField { get; set; } = string.Empty;

    /// <summary>
    /// 是否主键
    /// </summary>
    public bool IsPk { get; set; }

    /// <summary>
    /// 是否自增
    /// </summary>
    public bool IsIncrement { get; set; }

    /// <summary>
    /// 是否必填
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// 是否为插入字段
    /// </summary>
    public bool IsInsert { get; set; } = true;

    /// <summary>
    /// 是否编辑字段
    /// </summary>
    public bool IsEdit { get; set; } = true;

    /// <summary>
    /// 是否列表字段
    /// </summary>
    public bool IsList { get; set; } = true;

    /// <summary>
    /// 是否查询字段
    /// </summary>
    public bool IsQuery { get; set; }

    /// <summary>
    /// 查询方式（EQ,NE,GT,LT,LIKE等）
    /// </summary>
    public string QueryType { get; set; } = "EQ";

    /// <summary>
    /// 显示类型（input,textarea,select,datetime等）
    /// </summary>
    public string HtmlType { get; set; } = "input";
}

/// <summary>
/// 数据库表查询DTO
/// </summary>
public class DbTableQueryDto : PagedRequest
{
    /// <summary>
    /// 表名
    /// </summary>
    public string? TableName { get; set; }

    /// <summary>
    /// 表注释
    /// </summary>
    public string? TableComment { get; set; }
}

