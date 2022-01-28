namespace ApiWithAuthentication.Servers.API.Controllers.Dtos.Entities
{
    public interface ICreationAuditedDto
    {
        string CreatedBy { get; set; }
    }

    public interface IUpdateAuditedDto
    {
        string UpdatedBy { get; set; }
    }

    public interface IAuditedDto : ICreationAuditedDto, IUpdateAuditedDto
    {
    }

    public interface IDeleteAuditedDto
    {
        string DeletedBy { get; set; }
    }
}
