using Moq;
using WorkFlowCore.Engine;
using global::WorkflowCore.Interface;
using global::WorkflowCore.Models;

namespace WorkFlowCore.Engine.Tests;

/// <summary>
/// WorkflowEngineService 集成测试
/// </summary>
public class WorkflowEngineServiceTests
{
    private readonly Mock<IWorkflowHost> _mockWorkflowHost;
    private readonly Mock<IWorkflowController> _mockWorkflowController;
    private readonly WorkflowEngineService _workflowEngineService;

    public WorkflowEngineServiceTests()
    {
        _mockWorkflowHost = new Mock<IWorkflowHost>();
        _mockWorkflowController = new Mock<IWorkflowController>();
        _workflowEngineService = new WorkflowEngineService(
            _mockWorkflowHost.Object,
            _mockWorkflowController.Object);
    }

    [Fact]
    public async Task StartProcessAsync_ShouldReturnInstanceId()
    {
        // Arrange
        var definitionId = "SimpleApproval";
        var variables = new Dictionary<string, object>
        {
            { "title", "请假申请" },
            { "approverId", Guid.NewGuid() }
        };
        var expectedInstanceId = Guid.NewGuid().ToString();

        _mockWorkflowHost
            .Setup(x => x.StartWorkflow<object>(
                definitionId,
                It.IsAny<int?>(),
                It.IsAny<object>(),
                It.IsAny<string>()))
            .ReturnsAsync(expectedInstanceId);

        // Act
        var instanceId = await _workflowEngineService.StartProcessAsync(
            definitionId,
            variables);

        // Assert
        Assert.Equal(expectedInstanceId, instanceId);
        _mockWorkflowHost.Verify(
            x => x.StartWorkflow<object>(
                definitionId,
                It.IsAny<int?>(),
                It.IsAny<object>(),
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task StartProcessAsync_WithEmptyDefinitionId_ShouldThrowException()
    {
        // Arrange
        var variables = new Dictionary<string, object>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _workflowEngineService.StartProcessAsync(
                string.Empty,
                variables));
    }

    [Fact]
    public async Task StartProcessAsync_WithNullDefinitionId_ShouldThrowException()
    {
        // Arrange
        var variables = new Dictionary<string, object>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _workflowEngineService.StartProcessAsync(
                null!,
                variables));
    }

    [Fact]
    public async Task StartProcessAsync_WhenWorkflowNotFound_ShouldThrowException()
    {
        // Arrange
        var definitionId = "NonExistentWorkflow";
        var variables = new Dictionary<string, object>();

        _mockWorkflowHost
            .Setup(x => x.StartWorkflow<object>(
                definitionId,
                It.IsAny<int?>(),
                It.IsAny<object>(),
                It.IsAny<string>()))
            .ReturnsAsync(string.Empty);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _workflowEngineService.StartProcessAsync(
                definitionId,
                variables));
    }

    [Fact]
    public async Task CompleteTaskAsync_ShouldPublishEvent()
    {
        // Arrange
        var taskId = Guid.NewGuid().ToString();
        var variables = new Dictionary<string, object>
        {
            { "Approved", true },
            { "Comment", "同意" }
        };

        // Act
        await _workflowEngineService.CompleteTaskAsync(taskId, variables);

        // Assert
        _mockWorkflowController.Verify(
            x => x.PublishEvent(
                $"TaskCompleted_{taskId}",
                taskId,
                variables,
                It.IsAny<DateTime>()),
            Times.Once);
    }

    [Fact]
    public async Task CompleteTaskAsync_WithEmptyTaskId_ShouldThrowException()
    {
        // Arrange
        var variables = new Dictionary<string, object>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _workflowEngineService.CompleteTaskAsync(
                string.Empty,
                variables));
    }

    [Fact]
    public async Task CompleteTaskAsync_WithNullTaskId_ShouldThrowException()
    {
        // Arrange
        var variables = new Dictionary<string, object>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _workflowEngineService.CompleteTaskAsync(
                null!,
                variables));
    }

    [Fact]
    public async Task TerminateProcessAsync_ShouldCallTerminateWorkflow()
    {
        // Arrange
        var instanceId = Guid.NewGuid().ToString();
        var reason = "用户取消";

        _mockWorkflowHost
            .Setup(x => x.TerminateWorkflow(instanceId))
            .ReturnsAsync(true);

        // Act
        await _workflowEngineService.TerminateProcessAsync(instanceId, reason);

        // Assert
        _mockWorkflowHost.Verify(
            x => x.TerminateWorkflow(instanceId),
            Times.Once);
    }

    [Fact]
    public async Task TerminateProcessAsync_WithEmptyInstanceId_ShouldThrowException()
    {
        // Arrange
        var reason = "测试";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _workflowEngineService.TerminateProcessAsync(
                string.Empty,
                reason));
    }

    [Fact]
    public async Task TerminateProcessAsync_WithNullInstanceId_ShouldThrowException()
    {
        // Arrange
        var reason = "测试";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _workflowEngineService.TerminateProcessAsync(
                null!,
                reason));
    }

    [Fact]
    public async Task TerminateProcessAsync_WhenInstanceNotFound_ShouldThrowException()
    {
        // Arrange
        var instanceId = Guid.NewGuid().ToString();
        var reason = "测试";

        _mockWorkflowHost
            .Setup(x => x.TerminateWorkflow(instanceId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _workflowEngineService.TerminateProcessAsync(
                instanceId,
                reason));
    }

    [Fact]
    public async Task StartProcessAsync_WithEmptyVariables_ShouldSucceed()
    {
        // Arrange
        var definitionId = "SimpleApproval";
        var variables = new Dictionary<string, object>();
        var expectedInstanceId = Guid.NewGuid().ToString();

        _mockWorkflowHost
            .Setup(x => x.StartWorkflow<object>(
                definitionId,
                It.IsAny<int?>(),
                It.IsAny<object>(),
                It.IsAny<string>()))
            .ReturnsAsync(expectedInstanceId);

        // Act
        var instanceId = await _workflowEngineService.StartProcessAsync(
            definitionId,
            variables);

        // Assert
        Assert.Equal(expectedInstanceId, instanceId);
    }

    [Fact]
    public async Task CompleteTaskAsync_WithEmptyVariables_ShouldSucceed()
    {
        // Arrange
        var taskId = Guid.NewGuid().ToString();
        var variables = new Dictionary<string, object>();

        // Act
        await _workflowEngineService.CompleteTaskAsync(taskId, variables);

        // Assert
        _mockWorkflowController.Verify(
            x => x.PublishEvent(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<DateTime>()),
            Times.Once);
    }
}

