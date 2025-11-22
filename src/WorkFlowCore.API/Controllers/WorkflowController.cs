using Microsoft.AspNetCore.Mvc;
using global::WorkflowCore.Interface;
using WorkFlowCore.Application.Common;
using WorkFlowCore.Application.DTOs;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 工作流控制器
/// </summary>
public class WorkflowController : BaseController
{
    private readonly IWorkflowHost _workflowHost;
    private readonly IPersistenceProvider _persistenceProvider;

    public WorkflowController(IWorkflowHost workflowHost, IPersistenceProvider persistenceProvider)
    {
        _workflowHost = workflowHost;
        _persistenceProvider = persistenceProvider;
    }

    /// <summary>
    /// 启动工作流
    /// </summary>
    [HttpPost("start")]
    public async Task<ActionResult<ApiResponse<string>>> StartWorkflow([FromBody] StartWorkflowRequest request)
    {
        try
        {
            var workflowId = await _workflowHost.StartWorkflow(
                request.WorkflowId,
                request.Version,
                request.Data,
                request.Reference
            );

            return ApiResponse<string>.Ok(workflowId, "工作流已启动").ToActionResult();
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.Fail($"启动失败: {ex.Message}").ToActionResult();
        }
    }

    /// <summary>
    /// 完成审批任务
    /// </summary>
    [HttpPost("complete-task")]
    public async Task<ActionResult<ApiResponse>> CompleteTask([FromBody] CompleteTaskRequest request)
    {
        try
        {
            // 发布事件到工作流，更新步骤的持久化数据
            await _workflowHost.PublishEvent("approval-" + request.StepId, request.WorkflowId, new
            {
                request.Approved,
                request.Comment
            });

            return ApiResponse.Ok("任务已完成").ToActionResult();
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail($"完成任务失败: {ex.Message}").ToActionResult();
        }
    }

    /// <summary>
    /// 获取工作流实例信息
    /// </summary>
    [HttpGet("instance/{id}")]
    public async Task<ActionResult<ApiResponse<WorkflowInstanceDto>>> GetInstance(string id)
    {
        try
        {
            var instance = await _persistenceProvider.GetWorkflowInstance(id);
            
            if (instance == null)
            {
                return ApiResponse<WorkflowInstanceDto>.Fail("工作流实例不存在", ErrorCodes.NotFound).ToActionResult();
            }

            var dto = new WorkflowInstanceDto
            {
                Id = instance.Id,
                WorkflowDefinitionId = instance.WorkflowDefinitionId,
                Version = instance.Version,
                Reference = instance.Reference,
                Status = instance.Status.ToString(),
                CreateTime = instance.CreateTime,
                CompleteTime = instance.CompleteTime
            };

            return ApiResponse<WorkflowInstanceDto>.Ok(dto).ToActionResult();
        }
        catch (Exception ex)
        {
            return ApiResponse<WorkflowInstanceDto>.Fail($"查询失败: {ex.Message}").ToActionResult();
        }
    }

    /// <summary>
    /// 暂停工作流
    /// </summary>
    [HttpPost("suspend/{id}")]
    public async Task<ActionResult<ApiResponse>> SuspendWorkflow(string id)
    {
        try
        {
            var result = await _workflowHost.SuspendWorkflow(id);
            
            if (result)
            {
                return ApiResponse.Ok("工作流已暂停").ToActionResult();
            }
            
            return ApiResponse.Fail("暂停失败").ToActionResult();
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail($"暂停失败: {ex.Message}").ToActionResult();
        }
    }

    /// <summary>
    /// 恢复工作流
    /// </summary>
    [HttpPost("resume/{id}")]
    public async Task<ActionResult<ApiResponse>> ResumeWorkflow(string id)
    {
        try
        {
            var result = await _workflowHost.ResumeWorkflow(id);
            
            if (result)
            {
                return ApiResponse.Ok("工作流已恢复").ToActionResult();
            }
            
            return ApiResponse.Fail("恢复失败").ToActionResult();
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail($"恢复失败: {ex.Message}").ToActionResult();
        }
    }

    /// <summary>
    /// 终止工作流
    /// </summary>
    [HttpPost("terminate/{id}")]
    public async Task<ActionResult<ApiResponse>> TerminateWorkflow(string id)
    {
        try
        {
            var result = await _workflowHost.TerminateWorkflow(id);
            
            if (result)
            {
                return ApiResponse.Ok("工作流已终止").ToActionResult();
            }
            
            return ApiResponse.Fail("终止失败").ToActionResult();
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail($"终止失败: {ex.Message}").ToActionResult();
        }
    }
}

