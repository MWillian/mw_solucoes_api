using System.Net;

namespace MwSolucoes.Exception.ExceptionBase
{
    public class DomainException : MwSolucoesException
    {
        private readonly string _errorMessage;
        public override int StatusCode => (int)HttpStatusCode.UnprocessableEntity;
        public DomainException(string errorMessage) : base(string.Empty)
        {
            _errorMessage = errorMessage;
        }
        public override List<string> GetErrors()
        {
            return [_errorMessage];
        }
    }
}
