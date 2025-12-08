using learning_path_tracker.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace learning_path_tracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CertificatesController : ControllerBase
{
    private readonly ICertificateService _certificateService;

    public CertificatesController(ICertificateService certificateService)
    {
        _certificateService = certificateService;
    }

    [HttpGet("verify/{certificateId}")]
    public async Task<IActionResult> VerifyCertificate(string certificateId)
    {
        var certificate = await _certificateService.VerifyCertificateAsync(certificateId);
        
        if (certificate == null)
            return NotFound(new { message = "Certificate not found or invalid" });

        return Ok(certificate);
    }
}
