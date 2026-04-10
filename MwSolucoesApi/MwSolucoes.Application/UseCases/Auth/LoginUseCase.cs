using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Requests;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Domain.Security.Cryptography;
using MwSolucoes.Domain.Security.Tokens;
using MwSolucoes.Exception.ExceptionBase;

namespace MwSolucoes.Application.UseCases.Auth
{
    public class LoginUseCase : ILoginUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordEncrypter _passwordEncrypter;
        private readonly ITokenGenerator _tokenGenerator;

        public LoginUseCase(IUserRepository userRepository, IPasswordEncrypter passwordEncrypter, ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _passwordEncrypter = passwordEncrypter;
            _tokenGenerator = tokenGenerator;
        }
        public async Task<ResponseLogin> Execute(RequestLogin request)
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
    }
}
