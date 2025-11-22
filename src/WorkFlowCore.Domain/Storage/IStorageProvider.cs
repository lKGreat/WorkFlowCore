using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WorkFlowCore.Domain.Storage;

/// <summary>
/// 存储提供者抽象接口
/// </summary>
public interface IStorageProvider
{
    /// <summary>
    /// 上传文件分片
    /// </summary>
    /// <param name="stream">文件流</param>
    /// <param name="path">存储路径</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实际存储的路径</returns>
    Task<string> UploadChunkAsync(Stream stream, string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// 合并文件分片
    /// </summary>
    /// <param name="chunkPaths">分片路径列表</param>
    /// <param name="finalPath">最终文件路径</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>合并后的文件路径</returns>
    Task<string> MergeChunksAsync(string[] chunkPaths, string finalPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// 生成临时访问URL（预签名URL）
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="expireMinutes">过期时间（分钟）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>临时访问URL</returns>
    Task<string> GeneratePresignedUrlAsync(string path, int expireMinutes = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task DeleteAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取文件流
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>文件流</returns>
    Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查文件是否存在
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取文件大小
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>文件大小（字节）</returns>
    Task<long> GetFileSizeAsync(string path, CancellationToken cancellationToken = default);
}

