namespace WorkFlowCore.Application.Permissions;

/// <summary>
/// WorkFlowCore 权限常量定义
/// </summary>
public static class WorkFlowCorePermissions
{
    public const string GroupName = "WorkFlowCore";

    /// <summary>
    /// 用户管理权限
    /// </summary>
    public static class Users
    {
        public const string Default = GroupName + ".Users";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string ManageRoles = Default + ".ManageRoles";
        public const string ResetPassword = Default + ".ResetPassword";
        public const string Export = Default + ".Export";
        public const string Import = Default + ".Import";
    }

    /// <summary>
    /// 角色管理权限
    /// </summary>
    public static class Roles
    {
        public const string Default = GroupName + ".Roles";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string ManagePermissions = Default + ".ManagePermissions";
    }

    /// <summary>
    /// 菜单管理权限
    /// </summary>
    public static class Menus
    {
        public const string Default = GroupName + ".Menus";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    /// <summary>
    /// 部门管理权限
    /// </summary>
    public static class Departments
    {
        public const string Default = GroupName + ".Departments";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    /// <summary>
    /// 字典管理权限
    /// </summary>
    public static class Dicts
    {
        public const string Default = GroupName + ".Dicts";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    /// <summary>
    /// 系统配置权限
    /// </summary>
    public static class Configs
    {
        public const string Default = GroupName + ".Configs";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    /// <summary>
    /// 日志管理权限
    /// </summary>
    public static class Logs
    {
        public const string Default = GroupName + ".Logs";
        public const string View = Default + ".View";
        public const string Delete = Default + ".Delete";
        public const string Export = Default + ".Export";
    }

    /// <summary>
    /// 流程定义权限
    /// </summary>
    public static class ProcessDefinitions
    {
        public const string Default = GroupName + ".ProcessDefinitions";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Deploy = Default + ".Deploy";
    }

    /// <summary>
    /// 流程实例权限
    /// </summary>
    public static class ProcessInstances
    {
        public const string Default = GroupName + ".ProcessInstances";
        public const string Start = Default + ".Start";
        public const string View = Default + ".View";
        public const string Cancel = Default + ".Cancel";
    }

    /// <summary>
    /// 任务实例权限
    /// </summary>
    public static class TaskInstances
    {
        public const string Default = GroupName + ".TaskInstances";
        public const string View = Default + ".View";
        public const string Approve = Default + ".Approve";
        public const string Reject = Default + ".Reject";
    }
}

