//using System.Net;
//using Microsoft.AspNetCore.Mvc;

//namespace CleanProject3.Web.Helpers;

//public class OkResultWithCaching<T> : OkNegotiatedContentResult<T>
//{
//  public OkResultWithCaching(T content, ApiController controller)
//      : base(content, controller) { }

//  public OkResultWithCaching(T content, IContentNegotiator contentNegotiator, HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters)
//      : base(content, contentNegotiator, request, formatters) { }

//  public CacheControlHeaderValue CacheControlHeader { get; set; }
//  public EntityTagHeaderValue ETag { get; set; }

//  public override async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
//  {
//    HttpResponseMessage response;
//    try
//    {
//      response = await base.ExecuteAsync(cancellationToken);
//      response.Headers.CacheControl = this.CacheControlHeader;
//      response.Headers.ETag = ETag;
//    }
//    catch (OperationCanceledException)
//    {
//      response = new HttpResponseMessage(HttpStatusCode.Conflict) { ReasonPhrase = "Operation was cancelled" };
//    }
//    return response;
//  }
//}
