using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Responses.User;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;
using System.ComponentModel.DataAnnotations;
using MwSolucoes.Exception.ResouceErrors.UseCaseErrorMessages;

namespace MwSolucoes.Application.UseCases.User.GetUser
{
    public class GetUserByIdUseCase : IGetUserByIdUseCase
    {
        private readonly IUserRepository _userRepository;
        public GetUserByIdUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<ResponseGetUser> Execute(Guid id)
        {
            ValidateId(id);
            var user = await _userRepository.GetById(id) ?? throw new NotFoundException(GetUsersErrorMessages.USER_NOT_FOUND);
            return UserMapper.ToResponseGetUser(user);
        }
        private void ValidateId(Guid id)
        {
            if (id == Guid.Empty) throw new ValidationException(GetUsersErrorMessages.INVALID_USER_ID);
        }
    }
}
