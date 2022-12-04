namespace Extensions.Raw.Api.ApiModels
{
  public class QueryParameters
  {
  
    public QueryParameters(int pageNumber = 1, int pageSize = 10, string orderBy = "Id", bool ascending = true)
    {
      PageNumber = pageNumber;
      PageSize = pageSize;
      OrderBy = orderBy;
      Ascending = ascending;
    }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string OrderBy { get; set; } = string.Empty;
    public bool Ascending { get; set; } = true;
  }
}