using MwSolucoes.Communication.Requests.User;
using MwSolucoes.Communication.Responses.Login;
using MwSolucoes.Communication.Responses.User;
using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Security.Tokens;
using MwSolucoes.Domain.ValueObjects;
using DomainUserFilters = MwSolucoes.Domain.Repositories.Filters.UserFilters;

namespace MwSolucoes.Application.Mappers
{
    public static class UserMapper
    {
        public static ResponseRegisterUser ToResponseRegisterUser(User user, string accessToken, string refreshToken)
        {
            return new ResponseRegisterUser
            {
                Name = user.Name,
                Token = accessToken,
                RefreshToken = refreshToken
            };
        }
        public static ResponseLogin ToResponseLogin(User user, string accessToken, string refreshToken)
        {
            return new ResponseLogin
            {
                Name = user.Name,
                Token = accessToken,
                RefreshToken = refreshToken
            };
        }
        public static User ToUser(RequestRegisterUser request, string passwordHash)
        {
            var address = new Address(request.Logradouro, request.Numero, request.Bairro, request.Cidade, request.Estado, request.Cep);
            return new User(request.Name, request.Email, passwordHash, request.PhoneNumber, request.Cpf, 0, 0, address);
        }
        public static ResponseGetUser ToResponseGetUser(User user)
        {
            return new ResponseGetUser
            {
                Id = user.Id,
                Name = user.Name,
                Address = user.Address,
                Cpf = user.CPF,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }
        public static Communication.Responses.PagedResult<ResponseGetUser> ToResponseGetUsers(PagedResult<User> users)
        {
            var responseItems = users.Items.Select(ToResponseGetUser).ToList();
            Communication.Responses.PagedResult<ResponseGetUser> result = new(responseItems, users.TotalCount, users.CurrentPage, users.PageSize);
            return result;
        }

        public static ResponseUpdateUser ToResponseUpdateUser(User user)
        {
            return new ResponseUpdateUser
            {
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Cpf = user.CPF,
                Logradouro = user.Address.Logradouro,
                Numero = user.Address.Numero,
                Bairro = user.Address.Bairro,
                Cidade = user.Address.Cidade,
                Estado = user.Address.Estado,
                Cep = user.Address.Cep,
            };
        }
        public static DomainUserFilters MapToDomainFilters(UserFilters? filters)
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
