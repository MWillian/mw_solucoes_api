using MwSolucoes.Application.Interfaces;
using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Requests.Auth;
using MwSolucoes.Communication.Requests.Login;
using MwSolucoes.Communication.Responses.Login;
using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Domain.Security.Cryptography;
using MwSolucoes.Domain.Security.Tokens;
using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordEncrypter _passwordEncrypter;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public AuthService(IUserRepository userRepository, IPasswordEncrypter passwordEncrypter, ITokenGenerator tokenGenerator, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _passwordEncrypter = passwordEncrypter;
            _tokenGenerator = tokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task<ResponseLogin> LoginAsync(RequestLogin request)
        {
            var user = await _userRepository.GetByEmail(request.Email) ?? throw new InvalidLoginException("Usuário/Senha incorreta.");
            if (!user.IsActive)
                throw new UnprocessableEntityException("Usuário inativo. Entre em contato com o administrador do sistema.");
            var passwordMatch = _passwordEncrypter.Verify(request.Password, user.PasswordHash);
            if (!passwordMatch) throw new InvalidLoginException("Usuário/Senha incorreta.");

            var accessToken = _tokenGenerator.GenerateToken(user);
            var refreshTokenString = _tokenGenerator.GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken(refreshTokenString, DateTime.UtcNow.AddDays(7), user.Id);
            await _refreshTokenRepository.Add(refreshTokenEntity);

            var response = UserMapper.ToResponseLogin(user, accessToken, refreshTokenString);
            return response;
        }
        public async Task LogoutAsync(string cookieToken)
        {
            var refreshToken = await _refreshTokenRepository.GetByToken(cookieToken) ?? throw new NotFoundException("Token de atualização não encontrado.");
            if (refreshToken.IsExpired || !refreshToken.IsActive)
            {
                throw new UnprocessableEntityException("Sessão expirada ou inativa.");
            }
            refreshToken.Revoke();
            await _refreshTokenRepository.Update(refreshToken);
        }

        public async Task<ResponseLogin> TokenRefresh(string cookieToken)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenWithUser(cookieToken)
                ?? throw new UnauthorizedException("Token de atualização inválido.");

            if (refreshToken.IsRevoked)
            {
                await _refreshTokenRepository.RevokeAllByUserId(refreshToken.UserId);
                throw new UnauthorizedException("Tentativa de reutilização de token detectada. Todas as sessões foram encerradas por segurança.");
            }

            if (refreshToken.IsExpired)
            {
                throw new UnauthorizedException("Sessão expirada. Faça login novamente.");
            }

            refreshToken.Revoke();
            await _refreshTokenRepository.Update(refreshToken);

            var user = refreshToken.User;
            var newAccessToken = _tokenGenerator.GenerateToken(user);
            var newRefreshTokenString = _tokenGenerator.GenerateRefreshToken();

            var newRefreshTokenEntity = new RefreshToken(
                newRefreshTokenString,
                DateTime.UtcNow.AddDays(7),
                user.Id
            );
            await _refreshTokenRepository.Add(newRefreshTokenEntity);

            return UserMapper.ToResponseLogin(user, newAccessToken, newRefreshTokenString);
        }

        public async Task UpdatePasswordAsync(Guid userId, RequestUpdatePassword request)
        {
            var user = await _userRepository.GetById(userId) ?? throw new NotFoundException("Usuário não encontrado.");
            var currentPasswordHash = _passwordEncrypter.Verify(request.CurrentPassword, user.PasswordHash);
            if (!currentPasswordHash)
                throw new UnauthorizedException("Senha incorreta.");

            if (request.NewPassword != request.ConfirmNewPassword)
                throw new UnprocessableEntityException("A nova senha e a confirmação da nova senha não coincidem.");

            if (request.CurrentPassword == request.NewPassword)
                throw new UnprocessableEntityException("A nova senha deve ser diferente da senha atual.");

            var newPasswordHash = _passwordEncrypter.Encrypt(request.NewPassword);
            user.UpdatePassword(newPasswordHash);
            await _userRepository.Update(user);
            await _refreshTokenRepository.RevokeAllByUserId(user.Id);
        }

    }
}
