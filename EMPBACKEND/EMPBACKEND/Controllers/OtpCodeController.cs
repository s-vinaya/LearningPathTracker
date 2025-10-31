using Microsoft.AspNetCore.Mvc;
using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace EMPBACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OtpCodeController : ControllerBase
    {
        private readonly IOtpCodeService _service;

        public OtpCodeController(IOtpCodeService service)
        {
            _service = service;
        }

        [HttpPost("generate")]
        public async Task<ActionResult<string>> GenerateOtp([FromBody] string email)
        {
            var otp = await _service.GenerateOtpAsync(email);
            return Ok(new { otp });
        }

        [HttpPost("verify")]
        public async Task<ActionResult> VerifyOtp(VerifyOtpDto verifyDto)
        {
            var isValid = await _service.VerifyAsync(verifyDto.Email, verifyDto.OtpCode);
            return isValid ? Ok(new { message = "OTP verified successfully" }) : BadRequest(new { message = "Invalid or expired OTP" });
        }

        [HttpPost]
        public async Task<ActionResult<OtpCodeDto>> Create(CreateOtpCodeDto createDto)
        {
            var otpCode = await _service.CreateAsync(createDto);
            return Ok(otpCode);
        }

        [HttpGet("{email}/{code}")]
        public async Task<ActionResult<OtpCodeDto>> GetByEmailAndCode(string email, string code)
        {
            var otpCode = await _service.GetByEmailAndCodeAsync(email, code);
            return otpCode == null ? NotFound() : Ok(otpCode);
        }

        [HttpDelete("expired")]
        public async Task<ActionResult> DeleteExpired()
        {
            var result = await _service.DeleteExpiredAsync();
            return result ? Ok(new { message = "Expired OTP codes deleted" }) : Ok(new { message = "No expired OTP codes found" });
        }

        [HttpPut("{id}/mark-used")]
        public async Task<ActionResult> MarkAsUsed(int id)
        {
            var result = await _service.MarkAsUsedAsync(id);
            return result ? Ok() : NotFound();
        }
    }
}