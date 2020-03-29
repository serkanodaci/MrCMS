using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MrCMS.Data;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class InPageAdminService : IInPageAdminService
    {
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly ILogger<InPageAdminService> _logger;

        public InPageAdminService(
            IStringResourceProvider stringResourceProvider,
            IRepositoryResolver repositoryResolver, ILogger<InPageAdminService> logger)
        {
            _stringResourceProvider = stringResourceProvider;
            _repositoryResolver = repositoryResolver;
            _logger = logger;
        }

        public async Task<SaveResult> SaveContent(UpdatePropertyData updatePropertyData)
        {
            HashSet<Type> types = TypeHelper.GetAllConcreteTypesAssignableFrom<SystemEntity>();
            Type entityType = types.FirstOrDefault(t => t.Name == updatePropertyData.Type);
            if (entityType == null)
                return new SaveResult(false, string.Format(_stringResourceProvider.GetValue("Admin Inline Editing Save Entity Not Found", "Could not find entity type '{0}'"), updatePropertyData.Type));
            var entity = await GetEntity(entityType, updatePropertyData.Id);
            if (entity == null)
                return new SaveResult(false,
                    string.Format(_stringResourceProvider.GetValue("Admin InlineEditing Save Not Found", "Could not find entity of type '{0}' with id {1}"), updatePropertyData.Type,
                        updatePropertyData.Id));
            PropertyInfo propertyInfo =
                entityType.GetProperties().FirstOrDefault(info => info.Name == updatePropertyData.Property);
            if (propertyInfo == null)
                return new SaveResult(false,
                    string.Format(_stringResourceProvider.GetValue("Admin InlineEditing Save NotFound", "Could not find entity of type '{0}' with id {1}"), updatePropertyData.Type,
                        updatePropertyData.Id));
            if (propertyInfo.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(StringLengthAttribute)) is StringLengthAttribute stringLengthAttribute && updatePropertyData.Content.Length > stringLengthAttribute.MaximumLength)
            {
                return new SaveResult
                {
                    success = false,
                    message = string.Format(_stringResourceProvider.GetValue("Admin InlineEditing Save MaxLength", "Could not save property. The maximum length for this field is {0} characters."), stringLengthAttribute.MaximumLength)
                };
            }
            if (string.IsNullOrWhiteSpace(updatePropertyData.Content) &&
                propertyInfo.GetCustomAttributes(typeof(RequiredAttribute), false).Any())
            {
                return new SaveResult(false,
                    string.Format(_stringResourceProvider.GetValue("Admin InlineEditing Save Required", "Could not edit '{0}' as it is required"), updatePropertyData.Property));
            }

            try
            {
                propertyInfo.SetValue(entity, updatePropertyData.Content, null);
                var globalMethod = typeof(InPageAdminService).GetMethod(nameof(UpdateEntity));

                await (Task)globalMethod.MakeGenericMethod(entityType).Invoke(this, new[] { entity });
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, exception.Message);

                return new SaveResult(false, string.Format(_stringResourceProvider.GetValue("Admin InlineEditing Save Error", "Could not save to database '{0}' due to unknown error. Please check log."), updatePropertyData.Property));
            }

            return new SaveResult();
        }

        private Task<object> GetEntity(Type entityType, int id)
        {
            var globalMethod = typeof(InPageAdminService).GetMethod(nameof(GetEntityObject), new[] { typeof(int) });

            return globalMethod.MakeGenericMethod(entityType).InvokeAsync(this, id);
        }
        public async Task<T> GetEntityObject<T>(int id) where T : class, IHaveId
        {
            return await _repositoryResolver.GetGlobalRepository<T>().Load(id);
        }
        public async Task UpdateEntity<T>(T entity) where T : class, IHaveId
        {
            await _repositoryResolver.GetGlobalRepository<T>().Update(entity);
        }

        public async Task<ContentInfo> GetContent(GetPropertyData getPropertyData)
        {
            HashSet<Type> types = TypeHelper.GetAllConcreteTypesAssignableFrom<SystemEntity>();
            Type entityType = types.FirstOrDefault(t => t.Name == getPropertyData.Type);
            if (entityType == null)
                return new ContentInfo();
            var entity = await GetEntity(entityType, getPropertyData.Id);
            if (entity == null)
                return new ContentInfo();
            PropertyInfo propertyInfo =
                entityType.GetProperties().FirstOrDefault(info => info.Name == getPropertyData.Property);
            if (propertyInfo == null)
                return new ContentInfo();
            return new ContentInfo
            {
                Content = Convert.ToString(propertyInfo.GetValue(entity, null)),
                Entity = entity
            };
        }
    }
}