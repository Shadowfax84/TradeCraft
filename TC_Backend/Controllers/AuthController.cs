using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using TC_Backend.DTOs;
using TC_Backend.Services.Interfaces;

namespace TC_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("signup")]
        public async Task<ActionResult> Signup([FromForm] UserSignupDTO signupDto)
        {
            if (signupDto == null)
            {
                _logger.LogError("Signup DTO is null or not bound correctly.");
                throw new ArgumentNullException(nameof(signupDto), "Signup data is required.");
            }

            _logger.LogInformation("Signup request received for email {Email}", signupDto.Email);

            await _userService.UserSignupAsync(signupDto);

            _logger.LogInformation("User signup successful for email {Email}", signupDto.Email);
            return Ok(new { Message = "User signed up successfully." });
        }

        [HttpPost("admin/signup")]
        public async Task<ActionResult> AdminSignup([FromForm] UserSignupDTO signupDto)
        {
            if (signupDto == null)
            {
                _logger.LogError("Admin signup DTO is null or not bound correctly.");
                throw new ArgumentNullException(nameof(signupDto), "Admin signup data is required.");
            }

            _logger.LogInformation("Admin signup request received for email {Email}", signupDto.Email);

            await _userService.AdminSignupAsync(signupDto);

            _logger.LogInformation("Admin signup successful for email {Email}", signupDto.Email);
            return Ok(new { Message = "Admin account created successfully." });
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserLoginDTO loginDto)
        {
            _logger.LogInformation("Login attempt for email {Email}", loginDto.Email);

            var token = await _userService.UserLoginAsync(loginDto);

            _logger.LogInformation("Login successful for email {Email}", loginDto.Email);
            return Ok(new { Token = token });
        }

        [HttpGet("profile/{userId}")]
        public async Task<ActionResult<UserProfileDTO>> GetProfile(Guid userId)
        {
            _logger.LogInformation("Profile request for userId: {UserId}", userId);

            var profile = await _userService.GetUserProfileAsync(userId);
            if (profile == null)
            {
                _logger.LogWarning("Profile not found for userId: {UserId}", userId);
                return NotFound("User profile not found.");
            }

            _logger.LogInformation("Profile retrieved successfully for userId: {UserId}", userId);
            return Ok(profile);
        }
    }
}
