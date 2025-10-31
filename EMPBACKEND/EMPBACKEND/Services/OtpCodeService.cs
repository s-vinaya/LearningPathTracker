using EMPBACKEND.DTOs;
using EMPBACKEND.Interfaces.Services;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;
<<<<<<< Updated upstream
=======
using AutoMapper;
>>>>>>> Stashed changes

namespace EMPBACKEND.Services
{
    public class OtpCodeService : IOtpCodeService
    {
        private readonly IOtpCodeRepository _repository;
<<<<<<< Updated upstream

        public OtpCodeService(IOtpCodeRepository repository)
        {
            _repository = repository;
=======
        private readonly IMapper _mapper;

        public OtpCodeService(IOtpCodeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
>>>>>>> Stashed changes
        }

        public async Task<IEnumerable<OtpCodeDto>> GetAllAsync()
        {
            var otpCodes = await _repository.GetAllAsync();
            return otpCodes.Select(MapToDto);
        }

        public async Task<OtpCodeDto?> GetByIdAsync(int id)
        {
            var otpCode = await _repository.GetByIdAsync(id);
            return otpCode != null ? MapToDto(otpCode) : null;
        }

        public async Task<OtpCodeDto?> GetByEmailAndCodeAsync(string email, string code)
        {
            var otpCode = await _repository.GetByEmailAndCodeAsync(email, code);
            return otpCode != null ? MapToDto(otpCode) : null;
        }

        public async Task<OtpCodeDto> CreateAsync(CreateOtpCodeDto createDto)
        {
            var otpCode = new OtpCode
            {
                Email = createDto.Email,
                Code = createDto.Code,
                ExpiryTime = createDto.ExpiryTime,
                IsUsed = false
            };

            var created = await _repository.CreateAsync(otpCode);
            var result = await _repository.GetByIdAsync(created.Id);
            return MapToDto(result!);
        }

        public async Task<OtpCodeDto> UpdateAsync(int id, UpdateOtpCodeDto updateDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) throw new ArgumentException("OTP code not found");

            existing.IsUsed = updateDto.IsUsed;

            await _repository.UpdateAsync(existing);
            var updated = await _repository.GetByIdAsync(id);
            return MapToDto(updated!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<OtpCodeDto> GenerateOtpAsync(string email)
        {
            var code = new Random().Next(100000, 999999).ToString();
            var createDto = new CreateOtpCodeDto
            {
                Email = email,
                Code = code,
                ExpiryTime = DateTime.UtcNow.AddMinutes(10)
            };
            return await CreateAsync(createDto);
        }

        public async Task<bool> VerifyAsync(string email, string code)
        {
            var otpCode = await _repository.GetByEmailAndCodeAsync(email, code);
            return otpCode != null && otpCode.ExpiryTime > DateTime.UtcNow && !otpCode.IsUsed;
        }

        public async Task<bool> MarkAsUsedAsync(int id)
        {
            var updateDto = new UpdateOtpCodeDto { IsUsed = true };
            await UpdateAsync(id, updateDto);
            return true;
        }

        public async Task<bool> DeleteExpiredAsync()
        {
            var expiredCodes = await _repository.GetAllAsync();
            var expired = expiredCodes.Where(o => o.ExpiryTime < DateTime.UtcNow);
            foreach (var code in expired)
            {
                await _repository.DeleteAsync(code.Id);
            }
            return true;
        }

        private static OtpCodeDto MapToDto(OtpCode otpCode)
        {
            return new OtpCodeDto
            {
                Id = otpCode.Id,
                Email = otpCode.Email,
                Code = otpCode.Code,
                ExpiryTime = otpCode.ExpiryTime,
                IsUsed = otpCode.IsUsed
            };
        }
    }
<<<<<<< Updated upstream
}
=======
}
>>>>>>> Stashed changes
