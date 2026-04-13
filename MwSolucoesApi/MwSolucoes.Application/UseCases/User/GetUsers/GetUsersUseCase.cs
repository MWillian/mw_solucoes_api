using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Requests.User;
using MwSolucoes.Communication.Responses.User;
using MwSolucoes.Domain.Repositories;
using DomainUserFilters = MwSolucoes.Domain.Repositories.Filters.UserFilters;

namespace MwSolucoes.Application.UseCases.User.GetUsers
{
    public class GetUsersUseCase : IGetUsersUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetUsersUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Communication.Responses.PagedResult<ResponseGetUser>> Execute(UserFilters filters)
        {
            filters ??= new UserFilters();

            filters.Page = filters.Page <= 0 ? 1 : filters.Page;
            filters.PageSize = filters.PageSize <= 0 ? 20 : Math.Min(filters.PageSize, 100);
            filters.SortBy = string.IsNullOrWhiteSpace(filters.SortBy) ? "name" : filters.SortBy;
            filters.SortDirection = string.IsNullOrWhiteSpace(filters.SortDirection) ? "asc" : filters.SortDirection;

            var repositoryFilters = new DomainUserFilters
            {
                Name = filters.Name,
                Email = filters.Email,
                IsActive = filters.IsActive,
                Page = filters.Page,
                PageSize = filters.PageSize,
                SortBy = filters.SortBy,
                SortDirection = filters.SortDirection
            };

            var users = await _userRepository.GetAll(repositoryFilters);
            var result = UserMapper.ToResponseGetUsers(users);
            return result;
        }
    }
}