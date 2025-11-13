using TC_Backend.DTOs;

namespace TC_Backend.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> UserSignupAsync(UserSignupDTO signupDTO);
        Task<bool> AdminSignupAsync(UserSignupDTO signupDTO);
        Task<string> UserLoginAsync(UserLoginDTO loginDTO);
        Task<UserProfileDTO?> GetUserProfileAsync(Guid userId);
    }
}