using System.Net;

namespace MwSolucoes.Exception.ExceptionBase
{
    public class RequestConflictException : MwSolucoesException
    {
        private readonly string _errorMessage;
        public override int StatusCode => (int)HttpStatusCode.Conflict;
        public RequestConflictException(string message) : base(message)
        {
            _errorMessage = message;
        }
        public override List<string> GetErrors()
        {
            return [_errorMessage];
        }
    }
}
