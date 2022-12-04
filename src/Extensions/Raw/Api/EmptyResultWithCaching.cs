//using System.Net;

//namespace CleanProject3.Web.Helpers;

//public class EmptyResultWithCaching : IHttpActionResult
//{
//  public CacheControlHeaderValue CacheControlHeader { get; set; }
//  public EntityTagHeaderValue ETag { get; set; }
//  public HttpStatusCode StatusCode { get; set; }
//  public Uri Location { get; set; }

//  public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
//  {
//    HttpResponseMessage response = new HttpResponseMessage(StatusCode);
//    response.Headers.CacheControl = this.CacheControlHeader;
//    response.Headers.ETag = this.ETag;
//    response.Headers.Location = this.Location;
//    return response;
//  }
//}
