namespace WorkFlowCore.Engine.Models;

/// <summary>
/// 验证结果
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// 是否有效
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// 错误信息列表
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// 创建成功的验证结果
    /// </summary>
    public static ValidationResult Success() => new() { IsValid = true };

    /// <summary>
    /// 创建失败的验证结果
    /// </summary>
    public static ValidationResult Failure(params string[] errors) => new()
    {
        IsValid = false,
        Errors = errors.ToList()
    };
}

