using WorkFlowCore.Engine.Models;

namespace WorkFlowCore.Engine;

/// <summary>
/// 流程定义解析器接口
/// </summary>
public interface IProcessDefinitionParser
{
    /// <summary>
    /// 解析流程定义
    /// </summary>
    /// <param name="content">流程定义内容（BPMN XML 或 JSON）</param>
    /// <param name="format">内容格式</param>
    /// <returns>流程定义模型</returns>
    ProcessDefinitionModel Parse(string content, string format);

    /// <summary>
    /// 验证流程定义
    /// </summary>
    /// <param name="model">流程定义模型</param>
    /// <returns>验证结果</returns>
    ValidationResult Validate(ProcessDefinitionModel model);
}

