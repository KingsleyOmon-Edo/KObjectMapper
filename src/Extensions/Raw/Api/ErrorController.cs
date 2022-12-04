//using Microsoft.AspNetCore.Diagnostics;
//using Microsoft.AspNetCore.Mvc;

//namespace CleanProject3.Web.Api;

//[ApiController]
//public class ErrorController : BaseApiController
//{
//    [HttpGet("/error")]
//    public IActionResult Error()
//    {
//        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
//        var stackTrace = context?.Error.StackTrace;
//        var message = context?.Error.Message;

//        return Problem();
//    }
//}
