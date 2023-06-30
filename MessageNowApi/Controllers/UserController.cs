using MessageNowApi.Dtos;
using MessageNowApi.Models;
using MessageNowApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MessageNowApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly SignInManager<MessageNowUser> _signInManager;
        private readonly UserManager<MessageNowUser> _userManager;
        private readonly ITokenService _tokenService;
        public UserController(SignInManager<MessageNowUser> signInManager, UserManager<MessageNowUser> userManager, ITokenService tokenService) 
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (await _userManager.FindByNameAsync(registerDto.Username) != null)
            {
                var error = new ErrorDto
                {
                    Code = "UsernameTaken",
                    Description = "Username is taken."
                };
                return BadRequest(error);
            }
            var user = new MessageNowUser { UserName = registerDto.Username, Email = registerDto.Email };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors.First());
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if ((await _signInManager.PasswordSignInAsync(loginDto.Username, loginDto.Password, true, false)).Succeeded)
            {
                var bearerToken = await _tokenService.CreateToken(loginDto.Username);
                var refreshToken = await _tokenService.CreateRefreshToken(loginDto.Username);
                var tokensDto = new TokensDto() { 
                    RefreshToken = refreshToken,
                    BearerToken = bearerToken
                };
                return Ok(tokensDto);
            }
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var token = await _tokenService.CreateRefreshToken(refreshTokenDto.RefreshToken);
            if(token == null)
            {
                return Unauthorized();
            }
            var bearerTokenDto = new BearerTokenDto() { 
                BearerToken = token 
            };
            return Ok(bearerTokenDto);
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> LogOut([FromBody] RefreshTokenDto refreshTokenDto)
        {
            await _tokenService.DeactiveRefreshToken(refreshTokenDto.RefreshToken);
            return Ok();
        }

    }
}
