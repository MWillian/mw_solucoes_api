using MwSolucoes.Communication.Requests;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Domain.Security.Cryptography;
using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Application.UseCases.Auth.UpdatePassword
{
    public class UpdatePasswordUseCase : IUpdatePasswordUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordEncrypter _passwordEncrypter;

        public UpdatePasswordUseCase(IUserRepository userRepository, IPasswordEncrypter passwordEncrypter)
        {
            _userRepository = userRepository;
            _passwordEncrypter = passwordEncrypter;
        }
        public async Task Execute(Guid userId, RequestUpdatePassword request)
        {
            var user = await _userRepository.GetById(userId) ?? throw new NotFoundException("Usuário não encontrado.");
            var currentPasswordHash = _passwordEncrypter.Verify(request.CurrentPassword, user.PasswordHash);
            if (!currentPasswordHash)
                throw new UnauthorizedException("Senha incorreta.");
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
