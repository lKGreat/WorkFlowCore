using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace WorkFlowCore.Application.Permissions;

/// <summary>
/// WorkFlowCore 权限定义提供器
/// </summary>
public class WorkFlowCorePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var workflowGroup = context.AddGroup(
            WorkFlowCorePermissions.GroupName,
            L("Permission:WorkFlowCore"));

        // 用户管理权限
        var usersPermission = workflowGroup.AddPermission(
            WorkFlowCorePermissions.Users.Default,
            L("Permission:Users"));
        usersPermission.AddChild(WorkFlowCorePermissions.Users.Create, L("Permission:Users.Create"));
        usersPermission.AddChild(WorkFlowCorePermissions.Users.Edit, L("Permission:Users.Edit"));
        usersPermission.AddChild(WorkFlowCorePermissions.Users.Delete, L("Permission:Users.Delete"));
        usersPermission.AddChild(WorkFlowCorePermissions.Users.ManageRoles, L("Permission:Users.ManageRoles"));
        usersPermission.AddChild(WorkFlowCorePermissions.Users.ResetPassword, L("Permission:Users.ResetPassword"));
        usersPermission.AddChild(WorkFlowCorePermissions.Users.Export, L("Permission:Users.Export"));
        usersPermission.AddChild(WorkFlowCorePermissions.Users.Import, L("Permission:Users.Import"));

        // 角色管理权限
        var rolesPermission = workflowGroup.AddPermission(
            WorkFlowCorePermissions.Roles.Default,
            L("Permission:Roles"));
        rolesPermission.AddChild(WorkFlowCorePermissions.Roles.Create, L("Permission:Roles.Create"));
        rolesPermission.AddChild(WorkFlowCorePermissions.Roles.Edit, L("Permission:Roles.Edit"));
        rolesPermission.AddChild(WorkFlowCorePermissions.Roles.Delete, L("Permission:Roles.Delete"));
        rolesPermission.AddChild(WorkFlowCorePermissions.Roles.ManagePermissions, L("Permission:Roles.ManagePermissions"));

        // 菜单管理权限
        var menusPermission = workflowGroup.AddPermission(
            WorkFlowCorePermissions.Menus.Default,
            L("Permission:Menus"));
        menusPermission.AddChild(WorkFlowCorePermissions.Menus.Create, L("Permission:Menus.Create"));
        menusPermission.AddChild(WorkFlowCorePermissions.Menus.Edit, L("Permission:Menus.Edit"));
        menusPermission.AddChild(WorkFlowCorePermissions.Menus.Delete, L("Permission:Menus.Delete"));

        // 部门管理权限
        var deptsPermission = workflowGroup.AddPermission(
            WorkFlowCorePermissions.Departments.Default,
            L("Permission:Departments"));
        deptsPermission.AddChild(WorkFlowCorePermissions.Departments.Create, L("Permission:Departments.Create"));
        deptsPermission.AddChild(WorkFlowCorePermissions.Departments.Edit, L("Permission:Departments.Edit"));
        deptsPermission.AddChild(WorkFlowCorePermissions.Departments.Delete, L("Permission:Departments.Delete"));

        // 字典管理权限
        var dictsPermission = workflowGroup.AddPermission(
            WorkFlowCorePermissions.Dicts.Default,
            L("Permission:Dicts"));
        dictsPermission.AddChild(WorkFlowCorePermissions.Dicts.Create, L("Permission:Dicts.Create"));
        dictsPermission.AddChild(WorkFlowCorePermissions.Dicts.Edit, L("Permission:Dicts.Edit"));
        dictsPermission.AddChild(WorkFlowCorePermissions.Dicts.Delete, L("Permission:Dicts.Delete"));

        // 系统配置权限
        var configsPermission = workflowGroup.AddPermission(
            WorkFlowCorePermissions.Configs.Default,
            L("Permission:Configs"));
        configsPermission.AddChild(WorkFlowCorePermissions.Configs.Create, L("Permission:Configs.Create"));
        configsPermission.AddChild(WorkFlowCorePermissions.Configs.Edit, L("Permission:Configs.Edit"));
        configsPermission.AddChild(WorkFlowCorePermissions.Configs.Delete, L("Permission:Configs.Delete"));

        // 日志管理权限
        var logsPermission = workflowGroup.AddPermission(
            WorkFlowCorePermissions.Logs.Default,
            L("Permission:Logs"));
        logsPermission.AddChild(WorkFlowCorePermissions.Logs.View, L("Permission:Logs.View"));
        logsPermission.AddChild(WorkFlowCorePermissions.Logs.Delete, L("Permission:Logs.Delete"));
        logsPermission.AddChild(WorkFlowCorePermissions.Logs.Export, L("Permission:Logs.Export"));

        // 流程定义权限
        var processDefsPermission = workflowGroup.AddPermission(
            WorkFlowCorePermissions.ProcessDefinitions.Default,
            L("Permission:ProcessDefinitions"));
        processDefsPermission.AddChild(WorkFlowCorePermissions.ProcessDefinitions.Create, L("Permission:ProcessDefinitions.Create"));
        processDefsPermission.AddChild(WorkFlowCorePermissions.ProcessDefinitions.Edit, L("Permission:ProcessDefinitions.Edit"));
        processDefsPermission.AddChild(WorkFlowCorePermissions.ProcessDefinitions.Delete, L("Permission:ProcessDefinitions.Delete"));
        processDefsPermission.AddChild(WorkFlowCorePermissions.ProcessDefinitions.Deploy, L("Permission:ProcessDefinitions.Deploy"));

        // 流程实例权限
        var processInstsPermission = workflowGroup.AddPermission(
            WorkFlowCorePermissions.ProcessInstances.Default,
            L("Permission:ProcessInstances"));
        processInstsPermission.AddChild(WorkFlowCorePermissions.ProcessInstances.Start, L("Permission:ProcessInstances.Start"));
        processInstsPermission.AddChild(WorkFlowCorePermissions.ProcessInstances.View, L("Permission:ProcessInstances.View"));
        processInstsPermission.AddChild(WorkFlowCorePermissions.ProcessInstances.Cancel, L("Permission:ProcessInstances.Cancel"));

        // 任务实例权限
        var taskInstsPermission = workflowGroup.AddPermission(
            WorkFlowCorePermissions.TaskInstances.Default,
            L("Permission:TaskInstances"));
        taskInstsPermission.AddChild(WorkFlowCorePermissions.TaskInstances.View, L("Permission:TaskInstances.View"));
        taskInstsPermission.AddChild(WorkFlowCorePermissions.TaskInstances.Approve, L("Permission:TaskInstances.Approve"));
        taskInstsPermission.AddChild(WorkFlowCorePermissions.TaskInstances.Reject, L("Permission:TaskInstances.Reject"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WorkFlowCoreApplicationModule>(name);
    }
}

