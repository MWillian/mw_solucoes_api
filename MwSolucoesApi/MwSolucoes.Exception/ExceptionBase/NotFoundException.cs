using System.Net;

namespace MwSolucoes.Exception.ExceptionBase
{
    public class NotFoundException : MwSolucoesException
    {
        private readonly string _errorMessage;
        public override int StatusCode => (int)HttpStatusCode.NotFound;
        public NotFoundException(string errorMessage) : base(errorMessage)
        {
            _errorMessage = errorMessage;
        }
        public override List<string> GetErrors()
        {
            return [_errorMessage];
        }
    }
}
