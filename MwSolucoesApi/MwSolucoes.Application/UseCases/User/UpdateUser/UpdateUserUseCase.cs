using MwSolucoes.Application.Mappers;
using MwSolucoes.Communication.Requests;
using MwSolucoes.Communication.Responses.User;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Exception.ExceptionBase;

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
            UpdateUserFields(user, request);
            await _userRepository.Update(user);
            return UserMapper.ToResponseUpdateUser(user);
        }

        private void UpdateUserFields(Domain.Entities.User user, RequestUpdateUser request)
        {
            var address = new Domain.ValueObjects.Address(request.Logradouro, request.Numero, request.Bairro, request.Cidade, request.Estado, request.Cep);
            user.UpdateUser(request.Name, request.Email, request.PhoneNumber, request.Cpf, request.Role, request.AccessLevel, address);
        }
    }
}
