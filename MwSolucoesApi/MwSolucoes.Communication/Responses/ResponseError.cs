namespace MwSolucoes.Communication.Responses
{
    public class ResponseError
    {
        public List<string> ErrorsMessages { get; set; }
        public ResponseError(string errorMessage)
        {
            ErrorsMessages = new List<string>() { errorMessage };
        }
        public ResponseError(List<string> errorMessages)
        {
            ErrorsMessages = errorMessages;
        }
    }
}
