using TC_Backend.DTOs;
using TC_Backend.Models;

namespace TC_Backend.Helpers
{
    public class UserMapping
    {
        private readonly ILogger<UserMapping> _logger;

        public UserMapping(ILogger<UserMapping> logger)
        {
            _logger = logger;
        }

        /// Maps UserSignupDTO to User entity. 
        /// Note: RoleID, LastLogin, IsActive, ProfilePictureUrl, ProfilePicturePublicId,Password, 
        /// and all Identity fields should be set by the caller/service.
        public User? MapSignupDtoToUser(UserSignupDTO? dto)
        {
            try
            {
                _logger.LogDebug("Starting mapping from UserSignupDTO to User entity");

                if (dto == null)
                {
                    _logger.LogWarning("UserSignupDTO is null, returning null");
                    return null;
                }

                var user = new User
                {
                    UserName = dto.Username,
                    Email = dto.Email
                };

                _logger.LogDebug("Successfully mapped UserSignupDTO to User entity");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping UserSignupDTO to User entity");
                return null;
            }
        }

        /// <summary>
        /// Maps User entity back to UserSignupDTO.
        /// Note: Password cannot be retrieved from User (it's hashed), ProfilePicture (IFormFile) cannot be reconstructed.
        /// This method is mainly for reference/completeness.
        /// </summary>
        public UserSignupDTO? MapUserToSignupDto(User? user)
        {
            try
            {
                _logger.LogDebug("Starting mapping from User entity to UserSignupDTO");

                if (user == null)
                {
                    _logger.LogWarning("User entity is null, returning null");
                    return null;
                }

                var dto = new UserSignupDTO
                {
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Password = string.Empty, // Cannot retrieve password from hashed value
                    ProfilePicture = null // Cannot reconstruct IFormFile from URL
                };

                _logger.LogDebug("Successfully mapped User entity to UserSignupDTO");
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping User entity to UserSignupDTO");
                return null;
            }
        }

        /// <summary>
        /// Note: UserLoginDTO is used only for authentication and doesn't map to User entity directly.
        /// Login credentials are validated against the User entity via Identity UserManager.
        /// This method is provided for completeness but typically won't be used.
        /// </summary>
        public User? MapLoginDtoToUser(UserLoginDTO? dto)
        {
            try
            {
                _logger.LogDebug("Starting mapping from UserLoginDTO to User entity");

                if (dto == null)
                {
                    _logger.LogWarning("UserLoginDTO is null, returning null");
                    return null;
                }

                // LoginDTO only has Email and Password, not enough to create a full User entity
                // This mapping is typically not used; authentication is handled by Identity UserManager
                var user = new User
                {
                    Email = dto.Email
                    // UserName, RoleID, and other fields cannot be determined from LoginDTO alone
                };

                _logger.LogDebug("Mapped UserLoginDTO to partial User entity (Email only)");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping UserLoginDTO to User entity");
                return null;
            }
        }

        /// <summary>
        /// Maps User entity to UserProfileDTO.
        /// Note: RoleName is ignored as it's not available in User entity (only RoleID).
        /// </summary>
        public UserProfileDTO? MapUserToProfileDto(User? user)
        {
            try
            {
                _logger.LogDebug("Starting mapping from User entity to UserProfileDTO");

                if (user == null)
                {
                    _logger.LogWarning("User entity is null, returning null");
                    return null;
                }

                var dto = new UserProfileDTO
                {
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    LastLogin = user.LastLogin,
                    ProfilePictureUrl = user.ProfilePictureUrl
                    // RoleName is ignored - will be set by service layer
                };

                _logger.LogDebug("Successfully mapped User entity to UserProfileDTO");
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping User entity to UserProfileDTO");
                return null;
            }
        }

        /// <summary>
        /// Maps UserProfileDTO to User entity.
        /// Note: RoleName is ignored as User entity uses RoleID.
        /// </summary>
        public User? MapProfileDtoToUser(UserProfileDTO? dto)
        {
            try
            {
                _logger.LogDebug("Starting mapping from UserProfileDTO to User entity");

                if (dto == null)
                {
                    _logger.LogWarning("UserProfileDTO is null, returning null");
                    return null;
                }

                var user = new User
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    LastLogin = dto.LastLogin ?? DateTime.UtcNow,
                    ProfilePictureUrl = dto.ProfilePictureUrl
                    // RoleName is ignored - RoleID should be set separately
                };

                _logger.LogDebug("Successfully mapped UserProfileDTO to User entity");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping UserProfileDTO to User entity");
                return null;
            }
        }
    }
}
