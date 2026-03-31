using MwSolucoes.Application.Mappers;
using MwSolucoes.Application.UseCases.User.RegisterUser;
using MwSolucoes.Communication.Requests;
using MwSolucoes.Communication.Responses.User;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Domain.Security;
using MwSolucoes.Exception.ExceptionBase;
using MwSolucoes.Exception.ResouceErrors.UseCaseErrorMessages;

namespace MwSolucoes.Application.UseCases.User.Register
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordEncrypter _passwordEncrypter;
        public RegisterUserUseCase(IUserRepository userRepository, IPasswordEncrypter passwordEncrypter)
        {
            _userRepository = userRepository;
            _passwordEncrypter = passwordEncrypter;
        }
        public async Task<ResponseRegisterUser> Execute(RequestRegisterUser request)
        {
            await ValidateRequest(request);

            var passwordHash = _passwordEncrypter.Encrypt(request.Password);
            var user = UserMapper.ToUser(request,passwordHash);

            await _userRepository.Add(user);

            var response = UserMapper.ToResponseRegisterUser(user);

            return response;
        }
        public async Task ValidateRequest(RequestRegisterUser request)
        {
            RegisterUserValidator.Validate(request);
            await ValidateExistingUser(request);
                
        }
        private async Task ValidateExistingUser(RequestRegisterUser request)
        {
            var existingUserByEmail = await _userRepository.ExistUserWithEmail(request.Email);
            if (existingUserByEmail == true) throw new RequestConflictException(RegisterUserErrorMessages.EMAIL_ALREADY_REGISTERED);
            var existingUserByPhone = await _userRepository.ExistUserWithPhoneNumber(request.PhoneNumber);
            if (existingUserByPhone) throw new RequestConflictException(RegisterUserErrorMessages.PHONE_NUMBER_ALREADY_REGISTERED);
        }
    }
}
