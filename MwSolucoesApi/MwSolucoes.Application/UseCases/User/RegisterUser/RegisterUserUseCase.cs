using MwSolucoes.Communication.Responses.User;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;
using MwSolucoes.Exception.ResouceErrors.UseCaseErrorMessages;

namespace MwSolucoes.Application.UseCases.Usuario.CriarUsuario
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private readonly IUserRepository _userRepository;
        public RegisterUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<RegisterUserResponse> Execute(RequestRegisterUser request)
        {
            Validate(request);
            // implementar hash da senha
            // chamar repositório pra salvar
        }
        public async void Validate(RequestRegisterUser request)
        {
            var existingUserByEmail = await _userRepository.ExistUserWithEmail(request.Email);
            if (existingUserByEmail == true) throw new RequestConflictException(RegisterUserErrorMessages.EMAIL_ALREADY_REGISTERED);
            var existingUserByPhone = await _userRepository.ExistUserWithPhoneNumber(request.PhoneNumber);
            if (existingUserByPhone) throw new RequestConflictException(RegisterUserErrorMessages.PHONE_NUMBER_ALREADY_REGISTERED);

        }
    }
}
