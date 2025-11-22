using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;

namespace WorkFlowCore.API.Controllers;

/// <summary>
/// 文件存储提供者控制器
/// </summary>
[ApiController]
[Route("api/file-storage-providers")]
[Authorize]
public class FileStorageProviderController : AbpControllerBase
{
    private readonly IFileStorageProviderService _providerService;

    public FileStorageProviderController(IFileStorageProviderService providerService)
    {
        _providerService = providerService;
    }

    [HttpGet]
    public Task<List<FileStorageProviderDto>> GetAll(CancellationToken cancellationToken)
    {
        return _providerService.GetAllAsync(cancellationToken);
    }

    [HttpGet("enabled")]
    public Task<List<FileStorageProviderDto>> GetEnabled(CancellationToken cancellationToken)
    {
        return _providerService.GetEnabledAsync(cancellationToken);
    }

    [HttpGet("{id}")]
    public Task<FileStorageProviderDto?> GetById(long id, CancellationToken cancellationToken)
    {
        return _providerService.GetByIdAsync(id, cancellationToken);
    }

    [HttpPost]
    public Task<FileStorageProviderDto> Create([FromBody] CreateFileStorageProviderRequest request, CancellationToken cancellationToken)
    {
        return _providerService.CreateAsync(request, cancellationToken);
    }

    [HttpPut("{id}")]
    public Task<FileStorageProviderDto> Update(long id, [FromBody] UpdateFileStorageProviderRequest request, CancellationToken cancellationToken)
    {
        return _providerService.UpdateAsync(id, request, cancellationToken);
    }

    [HttpDelete("{id}")]
    public Task Delete(long id, CancellationToken cancellationToken)
    {
        return _providerService.DeleteAsync(id, cancellationToken);
    }

    [HttpPost("{id}/test")]
    public Task<bool> TestConnection(long id, CancellationToken cancellationToken)
    {
        return _providerService.TestConnectionAsync(id, cancellationToken);
    }
}

