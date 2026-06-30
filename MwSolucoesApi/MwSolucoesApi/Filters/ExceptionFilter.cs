using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MwSolucoes.Communication.Responses;
using MwSolucoes.Exception.ExceptionBase;
using MwSolucoes.Exception.ResouceErrors;

namespace MwSolucoes.Api.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;
        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is MwSolucoesException)
            {
                HandleProjectException(context);
            }
            else
            {
                ThrowUnknownError(context);
            }
        }
        private void HandleProjectException(ExceptionContext context)
        {
            var exception = context.Exception as MwSolucoesException;
            var errorResponse = new ResponseError(exception!.GetErrors());

            context.HttpContext.Response.StatusCode = exception.StatusCode;
            context.Result = new ObjectResult(errorResponse);
            _logger.LogError(context.Exception, $"An error occurred: {context.Exception.Message}");
        }
        private void ThrowUnknownError(ExceptionContext context)
        {
            var errorResponse = new ResponseError(ResourceErrorMessages.INTERNAL_ERROR);
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Result = new ObjectResult(errorResponse);
            _logger.LogError(context.Exception, $"An unknown error occurred: {context.Exception.Message}");
        }
    }
}
