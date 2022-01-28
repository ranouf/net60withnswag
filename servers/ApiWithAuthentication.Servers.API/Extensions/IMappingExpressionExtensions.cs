using SK.Entities;
using ApiWithAuthentication.Domains.Core.Identity.Entities;
using ApiWithAuthentication.Servers.API.Controllers.Dtos.Entities;
using AutoMapper;

namespace ApiWithAuthentication.Servers.API.Extensions
{
    public static class IMappingExpressionExtensions
    {
        public static IMappingExpression<TSource, TDestination> AddCreatedBy<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mapping)
            where TSource : ICreationAudited<User>
            where TDestination : ICreationAuditedDto
        {
            return mapping.ForMember(
                dest => dest.CreatedBy,
                opts => opts.MapFrom(src => src.CreatedByUser.FullName)
            );
        }

        public static IMappingExpression<TSource, TDestination> AddUpdatedBy<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mapping)
            where TSource : IUpdateAudited<User>
            where TDestination : IUpdateAuditedDto
        {
            return mapping.ForMember(
                dest => dest.UpdatedBy,
                opts => opts.MapFrom(src => src.UpdatedByUser.FullName)
            );
        }

        public static IMappingExpression<TSource, TDestination> AddAuditedBy<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mapping)
            where TSource : IAudited<User>
            where TDestination : IAuditedDto
        {
            return mapping
                .AddCreatedBy()
                .AddUpdatedBy();
        }

        public static IMappingExpression<TSource, TDestination> AddDeletedBy<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mapping)
            where TSource : IDeleteAudited<User>
            where TDestination : IDeleteAuditedDto
        {
            return mapping.ForMember(
                dest => dest.DeletedBy,
                opts => opts.MapFrom(src => src.DeletedByUser.FullName)
            );
        }

        public static IMappingExpression<TSource, TDestination> AddFullAuditedBy<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mapping)
            where TSource : IAudited<User>, IDeleteAudited<User>
            where TDestination : IAuditedDto, IDeleteAuditedDto
        {
            return mapping
                .AddAuditedBy()
                .AddDeletedBy();
        }
    }
}
