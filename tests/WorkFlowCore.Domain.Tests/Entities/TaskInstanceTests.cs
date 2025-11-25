using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Domain.Tests.Entities;

/// <summary>
/// TaskInstance 实体单元测试
/// </summary>
public class TaskInstanceTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        var id = 1L;
        var tenantId = Guid.NewGuid();
        var processInstanceId = 100L;
        var name = "经理审批";
        var nodeId = "approval_node_1";

        // Act
        var taskInstance = new TaskInstance(id, tenantId, processInstanceId, name, nodeId);

        // Assert
        Assert.Equal(id, taskInstance.Id);
        Assert.Equal(tenantId, taskInstance.TenantId);
        Assert.Equal(processInstanceId, taskInstance.ProcessInstanceId);
        Assert.Equal(name, taskInstance.Name);
        Assert.Equal(nodeId, taskInstance.NodeId);
        Assert.Equal("UserTask", taskInstance.TaskType);
        Assert.Equal("Pending", taskInstance.Status);
        Assert.Equal(0, taskInstance.Priority);
    }

    [Fact]
    public void TaskType_ShouldDefaultToUserTask()
    {
        // Arrange & Act
        var taskInstance = new TaskInstance(1L, null, 100L, "测试任务", "node1");

        // Assert
        Assert.Equal("UserTask", taskInstance.TaskType);
    }

    [Fact]
    public void Status_ShouldDefaultToPending()
    {
        // Arrange & Act
        var taskInstance = new TaskInstance(1L, null, 100L, "测试任务", "node1");

        // Assert
        Assert.Equal("Pending", taskInstance.Status);
    }

    [Fact]
    public void Priority_ShouldDefaultToZero()
    {
        // Arrange & Act
        var taskInstance = new TaskInstance(1L, null, 100L, "测试任务", "node1");

        // Assert
        Assert.Equal(0, taskInstance.Priority);
    }

    [Fact]
    public void CanAssignTaskToUser()
    {
        // Arrange
        var taskInstance = new TaskInstance(1L, null, 100L, "测试任务", "node1");
        var assigneeId = 1001L;

        // Act
        taskInstance.AssigneeId = assigneeId;

        // Assert
        Assert.Equal(assigneeId, taskInstance.AssigneeId);
    }

    [Fact]
    public void CanCompleteTask()
    {
        // Arrange
        var taskInstance = new TaskInstance(1L, null, 100L, "测试任务", "node1");
        var completeTime = DateTime.UtcNow;

        // Act
        taskInstance.Status = "Completed";
        taskInstance.CompleteTime = completeTime;

        // Assert
        Assert.Equal("Completed", taskInstance.Status);
        Assert.NotNull(taskInstance.CompleteTime);
        Assert.Equal(completeTime, taskInstance.CompleteTime);
    }

    [Fact]
    public void CanSetDueDate()
    {
        // Arrange
        var taskInstance = new TaskInstance(1L, null, 100L, "测试任务", "node1");
        var dueDate = DateTime.UtcNow.AddDays(3);

        // Act
        taskInstance.DueDate = dueDate;

        // Assert
        Assert.Equal(dueDate, taskInstance.DueDate);
    }

    [Fact]
    public void CanSetComment()
    {
        // Arrange
        var taskInstance = new TaskInstance(1L, null, 100L, "测试任务", "node1");
        var comment = "同意该申请";

        // Act
        taskInstance.Comment = comment;

        // Assert
        Assert.Equal(comment, taskInstance.Comment);
    }

    [Fact]
    public void CanSetCandidateUsers()
    {
        // Arrange
        var taskInstance = new TaskInstance(1L, null, 100L, "测试任务", "node1");
        var candidateUsers = "[1001, 1002, 1003]";

        // Act
        taskInstance.CandidateUsers = candidateUsers;

        // Assert
        Assert.Equal(candidateUsers, taskInstance.CandidateUsers);
    }

    [Fact]
    public void CanSetCandidateGroups()
    {
        // Arrange
        var taskInstance = new TaskInstance(1L, null, 100L, "测试任务", "node1");
        var candidateGroups = "[\"manager\", \"supervisor\"]";

        // Act
        taskInstance.CandidateGroups = candidateGroups;

        // Assert
        Assert.Equal(candidateGroups, taskInstance.CandidateGroups);
    }

    [Fact]
    public void CanSetVariables()
    {
        // Arrange
        var taskInstance = new TaskInstance(1L, null, 100L, "测试任务", "node1");
        var variables = "{\"leaveType\":\"年假\",\"days\":3}";

        // Act
        taskInstance.Variables = variables;

        // Assert
        Assert.Equal(variables, taskInstance.Variables);
    }

    [Fact]
    public void CanSetPriority()
    {
        // Arrange
        var taskInstance = new TaskInstance(1L, null, 100L, "测试任务", "node1");

        // Act
        taskInstance.Priority = 5;

        // Assert
        Assert.Equal(5, taskInstance.Priority);
    }

    [Fact]
    public void TaskInstance_CanCheckIfOverdue()
    {
        // Arrange
        var taskInstance = new TaskInstance(1L, null, 100L, "测试任务", "node1");
        taskInstance.DueDate = DateTime.UtcNow.AddDays(-1); // 昨天到期

        // Act
        var isOverdue = taskInstance.DueDate < DateTime.UtcNow && taskInstance.Status == "Pending";

        // Assert
        Assert.True(isOverdue);
    }

    [Fact]
    public void TaskInstance_NotOverdueIfCompleted()
    {
        // Arrange
        var taskInstance = new TaskInstance(1L, null, 100L, "测试任务", "node1");
        taskInstance.DueDate = DateTime.UtcNow.AddDays(-1);
        taskInstance.Status = "Completed";
        taskInstance.CompleteTime = DateTime.UtcNow;

        // Act
        var isOverdue = taskInstance.DueDate < DateTime.UtcNow && taskInstance.Status == "Pending";

        // Assert
        Assert.False(isOverdue);
    }

    [Fact]
    public void TenantId_CanBeNull()
    {
        // Arrange & Act
        var taskInstance = new TaskInstance(1L, null, 100L, "测试任务", "node1");

        // Assert
        Assert.Null(taskInstance.TenantId);
    }

    [Fact]
    public void CanChangeTaskType()
    {
        // Arrange
        var taskInstance = new TaskInstance(1L, null, 100L, "测试任务", "node1");

        // Act
        taskInstance.TaskType = "SystemTask";

        // Assert
        Assert.Equal("SystemTask", taskInstance.TaskType);
    }
}

