using System.Net;

namespace MwSolucoes.Exception.ExceptionBase
{
    public class UnauthorizedException : MwSolucoesException
    {
        private readonly string _errorMessage;
        public override int StatusCode => (int)HttpStatusCode.Unauthorized;
        public UnauthorizedException(string errorMessage) : base(errorMessage)
        {
            _errorMessage = errorMessage;
        }
        public override List<string> GetErrors()
        {
            return [_errorMessage];
        }
    }
}
