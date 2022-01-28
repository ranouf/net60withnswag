namespace ApiWithAuthentication.Servers.API.Controllers.Dtos.Paging
{
    public class PagedRequestDto : IDto
    {
        public string Filter { get; set; }
        public int? MaxResultCount { get; set; }
        public int? SkipCount { get; set; }
    }
}
