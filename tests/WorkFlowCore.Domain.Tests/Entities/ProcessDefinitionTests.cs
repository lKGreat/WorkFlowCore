using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Domain.Tests.Entities;

/// <summary>
/// ProcessDefinition 实体单元测试
/// </summary>
public class ProcessDefinitionTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        var id = 1L;
        var tenantId = Guid.NewGuid();
        var name = "请假流程";
        var key = "leave_process";
        var content = "{\"nodes\":[]}";

        // Act
        var processDefinition = new ProcessDefinition(id, tenantId, name, key, content);

        // Assert
        Assert.Equal(id, processDefinition.Id);
        Assert.Equal(tenantId, processDefinition.TenantId);
        Assert.Equal(name, processDefinition.Name);
        Assert.Equal(key, processDefinition.Key);
        Assert.Equal(content, processDefinition.Content);
        Assert.Equal(1, processDefinition.Version);
        Assert.True(processDefinition.IsEnabled);
        Assert.Equal("JSON", processDefinition.ContentFormat);
    }

    [Fact]
    public void Version_ShouldDefaultToOne()
    {
        // Arrange & Act
        var processDefinition = new ProcessDefinition(1L, null, "测试流程", "test", "{}");

        // Assert
        Assert.Equal(1, processDefinition.Version);
    }

    [Fact]
    public void IsEnabled_ShouldDefaultToTrue()
    {
        // Arrange & Act
        var processDefinition = new ProcessDefinition(1L, null, "测试流程", "test", "{}");

        // Assert
        Assert.True(processDefinition.IsEnabled);
    }

    [Fact]
    public void ContentFormat_ShouldDefaultToJSON()
    {
        // Arrange & Act
        var processDefinition = new ProcessDefinition(1L, null, "测试流程", "test", "{}");

        // Assert
        Assert.Equal("JSON", processDefinition.ContentFormat);
    }

    [Fact]
    public void CanSetDescription()
    {
        // Arrange
        var processDefinition = new ProcessDefinition(1L, null, "测试流程", "test", "{}");
        var description = "这是一个测试流程";

        // Act
        processDefinition.Description = description;

        // Assert
        Assert.Equal(description, processDefinition.Description);
    }

    [Fact]
    public void CanDisableProcess()
    {
        // Arrange
        var processDefinition = new ProcessDefinition(1L, null, "测试流程", "test", "{}");

        // Act
        processDefinition.IsEnabled = false;

        // Assert
        Assert.False(processDefinition.IsEnabled);
    }

    [Fact]
    public void CanIncrementVersion()
    {
        // Arrange
        var processDefinition = new ProcessDefinition(1L, null, "测试流程", "test", "{}");

        // Act
        processDefinition.Version = 2;

        // Assert
        Assert.Equal(2, processDefinition.Version);
    }

    [Fact]
    public void CanSetContentFormatToBPMN()
    {
        // Arrange
        var processDefinition = new ProcessDefinition(1L, null, "测试流程", "test", "<bpmn></bpmn>");

        // Act
        processDefinition.ContentFormat = "BPMN";

        // Assert
        Assert.Equal("BPMN", processDefinition.ContentFormat);
    }

    [Fact]
    public void TenantId_CanBeNull()
    {
        // Arrange & Act
        var processDefinition = new ProcessDefinition(1L, null, "测试流程", "test", "{}");

        // Assert
        Assert.Null(processDefinition.TenantId);
    }

    [Fact]
    public void Name_ShouldNotBeEmpty()
    {
        // Arrange
        var processDefinition = new ProcessDefinition(1L, null, "测试流程", "test", "{}");

        // Assert
        Assert.NotEmpty(processDefinition.Name);
    }

    [Fact]
    public void Key_ShouldBeUnique()
    {
        // Arrange
        var key = "unique_process_key";
        var processDefinition1 = new ProcessDefinition(1L, null, "流程1", key, "{}");
        var processDefinition2 = new ProcessDefinition(2L, null, "流程2", key, "{}");

        // Assert
        Assert.Equal(processDefinition1.Key, processDefinition2.Key);
        Assert.NotEqual(processDefinition1.Id, processDefinition2.Id);
    }
}

