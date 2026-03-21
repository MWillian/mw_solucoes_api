namespace MwSolucoes.Exception.ExceptionBase
{
    public abstract class MwSolucoesException : SystemException
    {
        protected MwSolucoesException(string message) : base(message) { }
        public abstract int StatusCode { get; }
        public abstract List<string> GetErrors();
    }
}
