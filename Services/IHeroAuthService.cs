using BusinessObject.DTOs.Login;
using Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IHeroAuthService
    {
        Task<ServiceResult<TokenResponseDto>> LoginAsync(HeroLoginDto dto);
        Task<ServiceResult<TokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto dto);
    }
}
