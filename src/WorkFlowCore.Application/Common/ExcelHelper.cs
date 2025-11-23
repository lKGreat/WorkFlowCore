using MiniExcelLibs;

namespace WorkFlowCore.Application.Common;

/// <summary>
/// Excel导入导出工具类
/// </summary>
public static class ExcelHelper
{
    /// <summary>
    /// 导出数据到Excel
    /// </summary>
    public static async Task<byte[]> ExportAsync<T>(IEnumerable<T> data) where T : class
    {
        var stream = new MemoryStream();
        await stream.SaveAsAsync(data);
        return stream.ToArray();
    }

    /// <summary>
    /// 从Excel导入数据
    /// </summary>
    public static async Task<List<T>> ImportAsync<T>(Stream stream) where T : class, new()
    {
        var rows = await stream.QueryAsync<T>();
        return rows.ToList();
    }
}

