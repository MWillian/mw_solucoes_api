using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MwSolucoes.Application.Interfaces;
using MwSolucoes.Application.Services;
using MwSolucoes.Communication.Requests.Auth;
using MwSolucoes.Communication.Requests.Login;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.Login;

namespace MwSolucoes.Api.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : MainController
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [EnableRateLimiting("auth")]
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] RequestResetPasswordDto request)
        {
            await _authService.ResetPasswordAsync(request);
            return Ok(new { Message = "Sua senha foi redefinida com sucesso! Você já pode fazer login." });
        }

        [EnableRateLimiting("auth")]
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login([FromBody] RequestLogin request)
        {
            var response = await _authService.LoginAsync(request);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", response.RefreshToken, cookieOptions);
            return Ok(new
            {
                response.Name,
                response.Token,
                response.RefreshToken
            });

        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseLogin), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Logout()
        {
            var token = GetCookieToken();
            if (token is null)
            {
                return Unauthorized();
            }
            await _authService.LogoutAsync(token);
            Response.Cookies.Delete("refreshToken");
            return NoContent();
        }

        [EnableRateLimiting("auth")]
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {
            var token = GetCookieToken();
            if (token is null)
            {
                return BadRequest();
            }
            var response = await _authService.TokenRefresh(token);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", response.RefreshToken, cookieOptions);
            return Ok(new
            {
                response.Token
            });
        }

        [EnableRateLimiting("auth")]
        [HttpPut("update-password/me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePassword([FromBody] RequestUpdatePassword request)
        {
            var userId = GetUserId();
            await _authService.UpdatePasswordMeAsync(userId, request);

            return NoContent();
        }
        private string? GetCookieToken()
        {
            if (Request.Cookies.TryGetValue("refreshToken", out var cookieToken))
            {
                return cookieToken;
            }
            return null;
        }

        [EnableRateLimiting("auth")]
        [HttpPut("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] RequestForgotPassword request)
        {
            await _authService.ForgotPasswordAsync(request.Email);
            return Ok(new { Message = "Se o e-mail estiver cadastrado, você receberá um link de redefinição em breve." });
        }
    }
}
