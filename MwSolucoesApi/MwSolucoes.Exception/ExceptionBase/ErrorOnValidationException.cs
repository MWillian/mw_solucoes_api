using System.Net;

namespace MwSolucoes.Exception.ExceptionBase
{
    public class ErrorOnValidationException : MwSolucoesException
    {
        private readonly string _errorMessage = string.Empty;
        public override int StatusCode => (int)HttpStatusCode.BadRequest;

        public ErrorOnValidationException(string errors) : base(string.Empty)
        {
            _errorMessage = errors;
        }
        public override List<string> GetErrors()
        {
            return [_errorMessage];
        }
    }
}
