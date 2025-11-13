using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TC_Backend.DTOs;
using TC_Backend.Exceptions;
using TC_Backend.Helpers;
using TC_Backend.Interfaces;
using TC_Backend.Models;
using TC_Backend.Services.Interfaces;

namespace TC_Backend.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IRoleRepo _roleRepo;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly UserMapping _userMapping;
        private readonly ILogger<UserService> _logger;
        private readonly JwtSettings _jwtSettings;

        public UserService(UserManager<User> userManager, IRoleRepo roleRepo, ICloudinaryService cloudinaryService,
            UserMapping userMapping, ILogger<UserService> logger, IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _roleRepo = roleRepo;
            _cloudinaryService = cloudinaryService;
            _userMapping = userMapping;
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<bool> UserSignupAsync(UserSignupDTO signupDTO)
        {
            return await SignupUserWithRoleAsync(signupDTO, "Investor");
        }

        public async Task<bool> AdminSignupAsync(UserSignupDTO signupDTO)
        {
            return await SignupUserWithRoleAsync(signupDTO, "Admin");
        }

        private async Task<bool> SignupUserWithRoleAsync(UserSignupDTO signupDTO, string roleName)
        {
            var existingUser = await _userManager.FindByEmailAsync(signupDTO.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("User with email {Email} already exists.", signupDTO.Email);
                throw new UserAlreadyExistsException($"User with email {signupDTO.Email} already exists.");
            }

            var userEntity = _userMapping.MapSignupDtoToUser(signupDTO);
            if (userEntity == null)
            {
                _logger.LogError("Failed to map UserSignupDTO to User entity");
                throw new UserCreationException("Mapping failed.");
            }

            userEntity.Id = Guid.NewGuid().ToString();

            var roles = await _roleRepo.GetAllRoles();
            var selectedRole = roles.FirstOrDefault(r => r.RoleName.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            if (selectedRole == null)
            {
                _logger.LogError("Role '{RoleName}' not found in the database.", roleName);
                throw new RoleNotFoundException($"Role '{roleName}' not found.");
            }

            userEntity.RoleID = selectedRole.RoleId;

            if (signupDTO.ProfilePicture != null && signupDTO.ProfilePicture.Length > 0)
            {
                var uploadPic = await _cloudinaryService.UploadProfileImg(signupDTO.ProfilePicture, userEntity.Id);
                userEntity.ProfilePictureUrl = uploadPic.Url;
                userEntity.ProfilePicturePublicId = uploadPic.PublicId;
            }

            userEntity.IsActive = true;

            var result = await _userManager.CreateAsync(userEntity, signupDTO.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to create user with role {RoleName}: {Errors}", roleName, errors);
                throw new UserCreationException(errors);
            }

            _logger.LogInformation("User signup successful for email {Email} with role {RoleName}", signupDTO.Email, roleName);
            return true;
        }


        public async Task<string> UserLoginAsync(UserLoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
            {
                _logger.LogWarning("Login failed for email {Email}", loginDTO.Email);
                throw new UnauthorizedAccessException("Invalid login attempt.");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login attempt for inactive user {Email}", loginDTO.Email);
                throw new UnauthorizedAccessException("User account is inactive.");
            }

            var role = await _roleRepo.GetRoleById(user.RoleID);
            if (role == null)
            {
                _logger.LogError("Role with ID {RoleID} not found for user {Email}", user.RoleID, loginDTO.Email);
                throw new RoleNotFoundException("User role not found.");
            }

            user.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Login successful for email {Email}", loginDTO.Email);
            return GenerateJwtToken(user, role.RoleName);
        }

        private string GenerateJwtToken(User user, string roleName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(ClaimTypes.Role, roleName),
                new("RoleID", user.RoleID.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email!)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiryInHours),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<UserProfileDTO?> GetUserProfileAsync(Guid userId)
        {
            try
            {
                _logger.LogDebug("Getting user profile for userId: {UserId}", userId);

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", userId);
                    return null;
                }

                var profileDto = _userMapping.MapUserToProfileDto(user);
                if (profileDto == null)
                {
                    _logger.LogError("Failed to map user to profile DTO for userId: {UserId}", userId);
                    return null;
                }

                // Get role name
                var role = await _roleRepo.GetRoleById(user.RoleID);
                if (role != null)
                {
                    profileDto.RoleName = role.RoleName;
                }

                _logger.LogInformation("Successfully retrieved user profile for userId: {UserId}", userId);
                return profileDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile for userId: {UserId}", userId);
                return null;
            }
        }
    }
}
