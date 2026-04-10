using System.Net;

namespace MwSolucoes.Exception.ExceptionBase
{
    public class UnprocessableEntityException : MwSolucoesException
    {
        private readonly string _errorMessage;
        public override int StatusCode => (int)HttpStatusCode.UnprocessableEntity;

        public UnprocessableEntityException(string errors) : base(string.Empty)
        {
            _errorMessage = errors;
        }
        public override List<string> GetErrors()
        {
            return [_errorMessage];
        }
    }
}
