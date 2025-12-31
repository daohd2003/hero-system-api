using BusinessObject.DTOs.Login;
using Repositories;
using Services.Common;
using System.Security.Claims;

namespace Services
{
    public class HeroAuthService : IHeroAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceHelper _serviceHelper;

        public HeroAuthService(ITokenService tokenService, IUnitOfWork unitOfWork, IServiceHelper serviceHelper)
        {
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _serviceHelper = serviceHelper;
        }

        public async Task<ServiceResult<TokenResponseDto>> LoginAsync(HeroLoginDto dto)
        {
            try
            {
                // Tìm Hero theo tên (Bạn cần thêm hàm này vào Repo nhé)
                // Hoặc dùng: var hero = (await _unitOfWork.Heros.GetAllAsync()).FirstOrDefault(h => h.Name == dto.HeroName);
                var hero = await _unitOfWork.Heroes.GetHeroByName(dto.HeroName);

                // Kiểm tra Password (Ở đây mình so sánh chuỗi thô cho đơn giản, thực tế nên Hash)
                if (hero == null || hero.PasswordHash != dto.HeroPassword)
                {
                    return _serviceHelper.HandleBadRequest<TokenResponseDto>("Sai tên Hero hoặc mật khẩu!");
                }

                // Tạo Token mới
                var accessToken = _tokenService.GenerateAccessToken(hero);
                var refreshToken = _tokenService.GenerateRefreshToken();

                // Cập nhật Refresh Token xuống DB
                hero.RefreshToken = refreshToken;
                hero.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Hạn 7 ngày

                _unitOfWork.Heroes.Update(hero);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<TokenResponseDto>.Ok(new TokenResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                return _serviceHelper.HandleError<TokenResponseDto>(ex);
            }
        }

        public async Task<ServiceResult<TokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto dto)
        {
            try
            {
                // Giải mã token cũ để lấy HeroId
                var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);
                var heroIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (heroIdStr == null) return _serviceHelper.HandleBadRequest<TokenResponseDto>("Invalid Token");

                var hero = await _unitOfWork.Heroes.GetByIdAsync(Guid.Parse(heroIdStr));

                // Kiểm tra tính hợp lệ: Token gửi lên phải trùng với DB và chưa hết hạn
                if (hero == null || hero.RefreshToken != dto.RefreshToken || hero.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    return _serviceHelper.HandleBadRequest<TokenResponseDto>("Refresh Token không hợp lệ hoặc đã hết hạn. Vui lòng đăng nhập lại.");
                }

                // --- QUAN TRỌNG: XOAY VÒNG TOKEN (Revoke cái cũ, cấp cái mới) ---
                var newAccessToken = _tokenService.GenerateAccessToken(hero);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                // Lưu cái mới đè lên cái cũ
                hero.RefreshToken = newRefreshToken;
                hero.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                _unitOfWork.Heroes.Update(hero);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<TokenResponseDto>.Ok(new TokenResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }
            catch (Exception ex)
            {
                return _serviceHelper.HandleError<TokenResponseDto>(ex);
            }
        }
    }
}
