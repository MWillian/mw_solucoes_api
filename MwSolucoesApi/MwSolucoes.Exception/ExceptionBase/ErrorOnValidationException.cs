using System.Net;

namespace MwSolucoes.Exception.ExceptionBase
{
    public class ErrorOnValidationException : MwSolucoesException
    {
        private readonly List<string> _errorMessage = new List<string>();
        public override int StatusCode => (int)HttpStatusCode.BadRequest;

        public ErrorOnValidationException(List<string> errors) : base(string.Empty)
        {
            _errorMessage = errors;
        }
        public override List<string> GetErrors()
        {
            return _errorMessage;
        }
    }
}
