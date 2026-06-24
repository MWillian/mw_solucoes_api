using MwSolucoes.Application.Interfaces;
using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Requests.User;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Communication.Responses.User;
using MwSolucoes.Domain.CepValidation;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Domain.Security.Cryptography;
using MwSolucoes.Domain.Security.Tokens;
using MwSolucoes.Domain.ValueObjects;
using MwSolucoes.Exception.ExceptionBase;
using MwSolucoes.Exception.ResouceErrors.UseCaseErrorMessages;
using System.ComponentModel.DataAnnotations;
using DomainUserFilters = MwSolucoes.Domain.Repositories.Filters.UserFilters;


namespace MwSolucoes.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordEncrypter _passwordEncrypter;
        private readonly ICepValidator _cepValidator;
        private readonly ITokenGenerator _tokenGenerator;
        public UserService(IUserRepository userRepository, IPasswordEncrypter passwordEncrypter, ICepValidator cepValidator, ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _passwordEncrypter = passwordEncrypter;
            _cepValidator = cepValidator;
            _tokenGenerator = tokenGenerator;
        }

        //Main methods
        public async Task<ResponseRegisterUser> RegisterUser(RequestRegisterUser request)
        {
            await ValidateExistingUser(request);
            await ValidateCep(request.Cep);
            var passwordHash = _passwordEncrypter.Encrypt(request.Password);

            var user = UserMapper.ToUser(request, passwordHash);

            await _userRepository.Add(user);

            return UserMapper.ToResponseRegisterUser(user, _tokenGenerator);
        }

        public async Task<ResponseUpdateUser> UpdateUser(Guid id, RequestUpdateUser request)
        {
            var user = await _userRepository.GetById(id);
            if (user is null) throw new NotFoundException("Usuário não encontrado.");
            await ValidateExistingUser(request);    
            await ValidateCep(request.Cep);
            UpdateUserFields(user, request);
            await _userRepository.Update(user);
            return UserMapper.ToResponseUpdateUser(user);
        }

        public async Task<PagedResult<ResponseGetUser>> GetUserList(UserFilters filters)
        {
            var repositoryFilters = MapToDomainFilters(filters);

            var users = await _userRepository.GetAll(repositoryFilters);
            var result = UserMapper.ToResponseGetUsers(users);
            return result;
        }

        // Aux Methods
        private void UpdateUserFields(Domain.Entities.User user, RequestUpdateUser request)
        {
            var address = new Address(request.Logradouro, request.Numero, request.Bairro, request.Cidade, request.Estado, request.Cep);
            user.UpdateUser(request.Name, request.Email, request.PhoneNumber, request.Cpf, 0, 0, address);
        }

        private async Task ValidateExistingUser(IUniqueUserData request)
        {
            var existingUserByEmail = await _userRepository.ExistUserWithEmail(request.Email);
            if (existingUserByEmail == true) throw new RequestConflictException(RegisterUserErrorMessages.EMAIL_ALREADY_REGISTERED);

            var normalizedPhoneNumber = new PhoneNumber(request.PhoneNumber).Number;
            var existingUserByPhone = await _userRepository.ExistUserWithPhoneNumber(normalizedPhoneNumber);
            if (existingUserByPhone) throw new RequestConflictException(RegisterUserErrorMessages.PHONE_NUMBER_ALREADY_REGISTERED);

            var normalizedCpf = new Cpf(request.Cpf);
            var existingUserByCpf = await _userRepository.ExistUserByCpf(normalizedCpf.Number);
            if (existingUserByCpf) throw new RequestConflictException("Cpf já cadastrado no sistema.");
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
        private DomainUserFilters MapToDomainFilters(UserFilters? filters)
        {
            if (filters is null)
            {
                return new DomainUserFilters
                {
                    Page = 1,
                    PageSize = 20,
                    SortBy = "name",
                    SortDirection = "asc"
                };
            }

            return new DomainUserFilters
            {
                Name = filters.Name,
                Email = filters.Email,
                IsActive = filters.IsActive,
                Page = filters.Page <= 0 ? 1 : filters.Page,
                PageSize = filters.PageSize <= 0 ? 20 : Math.Min(filters.PageSize, 100),
                SortBy = string.IsNullOrWhiteSpace(filters.SortBy) ? "name" : filters.SortBy,
                SortDirection = string.IsNullOrWhiteSpace(filters.SortDirection) ? "asc" : filters.SortDirection
            };
        }
    }
}