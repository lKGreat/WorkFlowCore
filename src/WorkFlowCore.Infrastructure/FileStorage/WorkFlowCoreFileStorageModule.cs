using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Aliyun;
using Volo.Abp.BlobStoring.Aws;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.Modularity;
using WorkFlowCore.Infrastructure.FileStorage.Containers;

namespace WorkFlowCore.Infrastructure.FileStorage;

/// <summary>
/// 文件存储 ABP 模块
/// </summary>
[DependsOn(
    typeof(AbpBlobStoringModule),
    typeof(AbpBlobStoringAliyunModule),
    typeof(AbpBlobStoringAwsModule),
    typeof(AbpBlobStoringFileSystemModule))]
public class WorkFlowCoreFileStorageModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var fileStorageSection = configuration.GetSection("FileStorage");

        context.Services.Configure<FileStorageOptions>(fileStorageSection);

        Configure<AbpBlobStoringOptions>(options =>
        {
            ConfigureLocal(options, fileStorageSection);
            ConfigureAliyun(options, fileStorageSection);
            ConfigureAws(options, fileStorageSection);
        });
    }

    private static void ConfigureLocal(AbpBlobStoringOptions options, IConfigurationSection section)
    {
        var basePath = section.GetValue<string>("Local:RootPath");
        options.Containers.Configure<LocalFileBlobContainer>(container =>
        {
            container.IsMultiTenant = true;
            container.UseFileSystem(fs =>
            {
                fs.BasePath = string.IsNullOrWhiteSpace(basePath)
                    ? Path.Combine(AppContext.BaseDirectory, "files")
                    : Path.IsPathRooted(basePath)
                        ? basePath!
                        : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, basePath!));
                fs.CreateOnInit = true;
            });
        });
    }

    private static void ConfigureAliyun(AbpBlobStoringOptions options, IConfigurationSection section)
    {
        options.Containers.Configure<AliyunFileBlobContainer>(container =>
        {
            container.IsMultiTenant = true;
            container.UseAliyun(aliyun =>
            {
                aliyun.Endpoint = section.GetValue<string>("Aliyun:Endpoint") ?? string.Empty;
                aliyun.AccessKeyId = section.GetValue<string>("Aliyun:AccessKeyId") ?? string.Empty;
                aliyun.AccessKeySecret = section.GetValue<string>("Aliyun:AccessKeySecret") ?? string.Empty;
                aliyun.ContainerName = section.GetValue<string>("Aliyun:BucketName") ?? string.Empty;
                aliyun.CreateContainerIfNotExists = true;
            });
        });
    }

    private static void ConfigureAws(AbpBlobStoringOptions options, IConfigurationSection section)
    {
        options.Containers.Configure<AwsFileBlobContainer>(container =>
        {
            container.IsMultiTenant = true;
            container.UseAws(aws =>
            {
                aws.Region = section.GetValue<string>("Aws:Region") ?? string.Empty;
                aws.AccessKeyId = section.GetValue<string>("Aws:AccessKeyId") ?? string.Empty;
                aws.SecretAccessKey = section.GetValue<string>("Aws:AccessKeySecret") ?? string.Empty;
                aws.BucketName = section.GetValue<string>("Aws:BucketName") ?? string.Empty;
                aws.CreateBucketIfNotExists = true;
            });
        });
    }
}

