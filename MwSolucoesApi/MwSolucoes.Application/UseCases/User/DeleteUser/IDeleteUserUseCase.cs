namespace MwSolucoes.Application.UseCases.User.DeleteUser
{
    public interface IDeleteUserUseCase
    {
        Task Execute(Guid id);
    }
}
