using MwSolucoes.Application.Interfaces;
using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Requests.User;
using MwSolucoes.Communication.Responses.User;
using MwSolucoes.Domain.CepValidation;
using MwSolucoes.Domain.Communication;
using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Domain.Security.Cryptography;
using MwSolucoes.Domain.Security.Tokens;
using MwSolucoes.Domain.ValueObjects;
using MwSolucoes.Exception.ExceptionBase;
using MwSolucoes.Exception.ResouceErrors.UseCaseErrorMessages;
using System.ComponentModel.DataAnnotations;


namespace MwSolucoes.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordEncrypter _passwordEncrypter;
        private readonly ICepValidator _cepValidator;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IEmailService _emailService;

        public UserService(IUserRepository userRepository, IPasswordEncrypter passwordEncrypter, ICepValidator cepValidator, ITokenGenerator tokenGenerator, IRefreshTokenRepository refreshTokenRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _passwordEncrypter = passwordEncrypter;
            _cepValidator = cepValidator;
            _tokenGenerator = tokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
            _emailService = emailService;
        }

        //Main methods
        public async Task<ResponseRegisterUser> RegisterUser(RequestRegisterUser request)
        {
            await ValidateExistingUser(request);
            await ValidateCep(request.Cep);
            var passwordHash = _passwordEncrypter.Encrypt(request.Password);

            var user = UserMapper.ToUser(request, passwordHash);

            await _userRepository.Add(user);

            string placeholderConfirmationLink = "https://example.com/confirm?token=some-token";
            await _emailService.SendWelcomeConfirmationAsync(user.Email, user.Name, placeholderConfirmationLink);
            var accessToken = _tokenGenerator.GenerateToken(user);
            var refreshTokenString = _tokenGenerator.GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken(refreshTokenString, DateTime.UtcNow.AddDays(7), user.Id);
            await _refreshTokenRepository.Add(refreshTokenEntity);

            return UserMapper.ToResponseRegisterUser(user, accessToken, refreshTokenString);
        }

        public async Task<ResponseUpdateUser> UpdateUser(Guid id, RequestUpdateUser request)
        {
            var user = await _userRepository.GetById(id);
            if (user is null) throw new NotFoundException("Usuário não encontrado.");

            await ValidateExistingUser(request, id);
            await ValidateCep(request.Cep);
            ValidateId(user.Id);

            UpdateUserFields(user, request);
            await _userRepository.Update(user);
            return UserMapper.ToResponseUpdateUser(user);
        }

        public async Task<Communication.Responses.PagedResult<ResponseGetUser>> GetUserList(UserFilters filters)
        {
            var repositoryFilters = UserMapper.MapToDomainFilters(filters);

            var users = await _userRepository.GetAll(repositoryFilters);
            var result = UserMapper.ToResponseGetUsers(users);
            return result;
        }

        public async Task DeactivateUser(Guid id)
        {
            ValidateId(id);
            var user = await _userRepository.GetById(id) ?? throw new NotFoundException(GetUsersErrorMessages.USER_NOT_FOUND);
            user.Deactivate(user);
            await _userRepository.Update(user);
            return;
        }

        public async Task ActivateUser(Guid id)
        {
            ValidateId(id);
            var user = await _userRepository.GetById(id) ?? throw new NotFoundException(GetUsersErrorMessages.USER_NOT_FOUND);
            user.Activate(user);
            await _userRepository.Update(user);
            return;
        }

        // Helper Methods
        private void UpdateUserFields(Domain.Entities.User user, RequestUpdateUser request)
        {
            var address = new Address(request.Logradouro, request.Numero, request.Bairro, request.Cidade, request.Estado, request.Cep);
            user.UpdateUser(request.Name, request.Email, request.PhoneNumber, request.Cpf, 0, 0, address);
        }

        private async Task ValidateExistingUser(IUniqueUserData request, Guid? currentUserId = null)
        {
            if (await _userRepository.ExistUserWithEmail(request.Email, currentUserId))
                throw new RequestConflictException("Email já cadastrado.");
            if (await _userRepository.ExistUserWithPhoneNumber(new PhoneNumber(request.PhoneNumber).Number, currentUserId))
                throw new RequestConflictException("Telefone já cadastrado.");
            if (await _userRepository.ExistUserByCpf(new Cpf(request.Cpf).Number, currentUserId))
                throw new RequestConflictException("CPF já cadastrado.");
        }
        private async Task ValidateCep(string cep)
        {
            var cepValidationResult = await _cepValidator.IsValidCepAsync(cep);
            if (cepValidationResult == false) throw new RequestConflictException(RegisterUserErrorMessages.INVALID_CEP);
        }

        private void ValidateId(Guid id)
        {
            if (id == Guid.Empty) throw new ValidationException(GetUsersErrorMessages.INVALID_USER_ID);
        }

        public async Task<ResponseGetUser> GetUserById(Guid id)
        {
            var user = await _userRepository.GetById(id) ?? throw new NotFoundException(GetUsersErrorMessages.USER_NOT_FOUND);
            return UserMapper.ToResponseGetUser(user);
        }
    }
}