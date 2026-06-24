using MwSolucoes.Application.Interfaces;
using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Requests.Auth;
using MwSolucoes.Communication.Requests.Login;
using MwSolucoes.Communication.Responses.Login;
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
        public AuthService(IUserRepository userRepository, IPasswordEncrypter passwordEncrypter, ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _passwordEncrypter = passwordEncrypter;
            _tokenGenerator = tokenGenerator;
        }
        public async Task<ResponseLogin> LoginAsync(RequestLogin request)
        {
            var user = await _userRepository.GetByEmail(request.Email) ?? throw new NotFoundException("Usuário/Senha incorreta.");
            var isActiveUser = await _userRepository.IsActive(user.Id);
            if (!isActiveUser)
                throw new UnprocessableEntityException("Usuário inativo. Entre em contato com o administrador do sistema.");
            var passwordMatch = _passwordEncrypter.Verify(request.Password, user.PasswordHash);
            if (!passwordMatch) throw new InvalidLoginException("Usuário/Senha incorreta.");
            var response = UserMapper.ToResponseLogin(user, _tokenGenerator);
            return response;
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
            UpdateUserPassword(user, newPasswordHash);
            await _userRepository.Update(user);
        }
        private void UpdateUserPassword(Domain.Entities.User user, string newPasswordHash)
        {
            user.UpdatePassword(newPasswordHash);
        }
    }
}
