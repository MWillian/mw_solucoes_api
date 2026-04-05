using MwSolucoes.Application.Mappers;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;
using MwSolucoes.Exception.ResouceErrors.UseCaseErrorMessages;
using System.ComponentModel.DataAnnotations;

namespace MwSolucoes.Application.UseCases.User.DeleteUser
{
    public class DeleteUserUseCase : IDeleteUserUseCase
    {
        private readonly IUserRepository _userRepository;
        public DeleteUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task Execute(Guid id)
        {
            ValidateId(id);
            var user = await _userRepository.GetById(id) ?? throw new NotFoundException(GetUsersErrorMessages.USER_NOT_FOUND);
            user.Deactivate(user);
            await _userRepository.Update(user);
            return;
        }
        private void ValidateId(Guid id)
        {
            if (id == Guid.Empty) throw new ValidationException(GetUsersErrorMessages.INVALID_USER_ID);
        }
    }
}
