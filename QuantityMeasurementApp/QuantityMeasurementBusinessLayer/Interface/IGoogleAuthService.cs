using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementBusinessLayer.Interface
{
    /// <summary>UC19: Google OAuth2 authentication — validate Google token, find/create user, issue our JWT.</summary>
    public interface IGoogleAuthService
    {
        Task<AuthResponseDto> GoogleLoginAsync(GoogleAuthRequestDto request);
    }
}
