using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// 服务器监控服务实现
/// </summary>
public class ServerMonitorService : ApplicationService, IServerMonitorService
{
    private static readonly DateTime ProcessStartTime = Process.GetCurrentProcess().StartTime;

    public Task<ServerInfoDto> GetServerInfoAsync()
    {
        var info = new ServerInfoDto
        {
            Cpu = GetCpuInfo(),
            Memory = GetMemoryInfo(),
            Disks = GetDiskInfo(),
            System = GetSystemInfo()
        };

        return Task.FromResult(info);
    }

    /// <summary>
    /// 获取CPU信息
    /// </summary>
    private CpuInfo GetCpuInfo()
    {
        var cpuInfo = new CpuInfo
        {
            Cores = Environment.ProcessorCount,
            Name = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") ?? "Unknown"
        };

        // 获取CPU使用率（简化版本）
        try
        {
            using var process = Process.GetCurrentProcess();
            cpuInfo.UsagePercent = Math.Round(process.TotalProcessorTime.TotalMilliseconds / (DateTime.Now - process.StartTime).TotalMilliseconds * 100 / Environment.ProcessorCount, 2);
        }
        catch
        {
            cpuInfo.UsagePercent = 0;
        }

        return cpuInfo;
    }

    /// <summary>
    /// 获取内存信息
    /// </summary>
    private MemoryInfo GetMemoryInfo()
    {
        var memInfo = new MemoryInfo();

        try
        {
            using var process = Process.GetCurrentProcess();
            var usedMemory = process.WorkingSet64 / 1024 / 1024; // 转换为MB

            // 在Windows上可以获取总物理内存（简化版）
            var gcMemoryInfo = GC.GetGCMemoryInfo();
            var totalMemory = gcMemoryInfo.TotalAvailableMemoryBytes / 1024 / 1024;

            memInfo.Used = usedMemory;
            memInfo.Total = totalMemory;
            memInfo.Free = totalMemory - usedMemory;
            memInfo.UsagePercent = totalMemory > 0 ? Math.Round((double)usedMemory / totalMemory * 100, 2) : 0;
        }
        catch
        {
            memInfo.Total = 0;
            memInfo.Used = 0;
            memInfo.Free = 0;
            memInfo.UsagePercent = 0;
        }

        return memInfo;
    }

    /// <summary>
    /// 获取磁盘信息
    /// </summary>
    private List<DiskInfo> GetDiskInfo()
    {
        var disks = new List<DiskInfo>();

        try
        {
            var drives = DriveInfo.GetDrives().Where(d => d.IsReady);
            
            foreach (var drive in drives)
            {
                var totalGB = drive.TotalSize / 1024 / 1024 / 1024;
                var freeGB = drive.AvailableFreeSpace / 1024 / 1024 / 1024;
                var usedGB = totalGB - freeGB;

                disks.Add(new DiskInfo
                {
                    Name = drive.Name,
                    MountPoint = drive.RootDirectory.FullName,
                    Total = totalGB,
                    Free = freeGB,
                    Used = usedGB,
                    UsagePercent = totalGB > 0 ? Math.Round((double)usedGB / totalGB * 100, 2) : 0
                });
            }
        }
        catch
        {
            // 忽略错误
        }

        return disks;
    }

    /// <summary>
    /// 获取系统信息
    /// </summary>
    private SystemInfo GetSystemInfo()
    {
        return new SystemInfo
        {
            Os = RuntimeInformation.OSDescription,
            Architecture = RuntimeInformation.OSArchitecture.ToString(),
            HostName = Environment.MachineName,
            FrameworkVersion = RuntimeInformation.FrameworkDescription,
            StartTime = ProcessStartTime,
            RunTime = (long)(DateTime.Now - ProcessStartTime).TotalSeconds
        };
    }
}

