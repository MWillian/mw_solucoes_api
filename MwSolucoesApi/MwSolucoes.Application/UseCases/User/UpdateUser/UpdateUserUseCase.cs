using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Requests;
using MwSolucoes.Communication.Responses.User;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;
using MwSolucoes.Exception.ResouceErrors.UseCaseErrorMessages;

namespace MwSolucoes.Application.UseCases.User.UpdateUser
{
    public class UpdateUserUseCase : IUpdateUserUseCase
    {
        private readonly IUserRepository _userRepository;
        public UpdateUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<ResponseUpdateUser> Execute(Guid id, RequestUpdateUser request)
        {
            var user = await _userRepository.GetById(id);
            if (user is null) throw new NotFoundException("Usuário não encontrado.");
            var updatedUser = await _userRepository.Update(user);
            return UserMapper.ToResponseUpdateUser();
        }
    }
}
