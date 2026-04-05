using MwSolucoes.Application.Mappers;
using MwSolucoes.Application.UseCases.User.RegisterUser;
using MwSolucoes.Communication.Requests;
using MwSolucoes.Communication.Responses.User;
using MwSolucoes.Domain.CepValidation;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Domain.Security.Cryptography;
using MwSolucoes.Domain.Security.Tokens;
using MwSolucoes.Domain.ValueObjects;
using MwSolucoes.Exception.ExceptionBase;
using MwSolucoes.Exception.ResouceErrors.UseCaseErrorMessages;

namespace MwSolucoes.Application.UseCases.User.Register
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordEncrypter _passwordEncrypter;
        private readonly ICepValidator _cepValidator;
        private readonly ITokenGenerator _tokenGenerator;

        public RegisterUserUseCase(IUserRepository userRepository, IPasswordEncrypter passwordEncrypter, ICepValidator cepValidator, ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _passwordEncrypter = passwordEncrypter;
            _cepValidator = cepValidator;
            _tokenGenerator = tokenGenerator;
        }
        public async Task<ResponseRegisterUser> Execute(RequestRegisterUser request)
        {
            await ValidateExistingUser(request);
            await ValidateCep(request.Cep);
            var passwordHash = _passwordEncrypter.Encrypt(request.Password);

            var user = UserMapper.ToUser(request, passwordHash);

            await _userRepository.Add(user);

            return UserMapper.ToResponseRegisterUser(user, _tokenGenerator);

        }
        private async Task ValidateExistingUser(RequestRegisterUser request)
        {
            var existingUserByEmail = await _userRepository.ExistUserWithEmail(request.Email);
            if (existingUserByEmail == true) throw new RequestConflictException(RegisterUserErrorMessages.EMAIL_ALREADY_REGISTERED);

            var normalizedPhoneNumber = new PhoneNumber(request.PhoneNumber).Number;
            var existingUserByPhone = await _userRepository.ExistUserWithPhoneNumber(normalizedPhoneNumber);
            if (existingUserByPhone) throw new RequestConflictException(RegisterUserErrorMessages.PHONE_NUMBER_ALREADY_REGISTERED);
        }
        private async Task ValidateCep(string cep)
        {
            var cepValidationResult = await _cepValidator.IsValidCepAsync(cep);
            if (cepValidationResult == false) throw new RequestConflictException(RegisterUserErrorMessages.INVALID_CEP);
        }
    }
}
