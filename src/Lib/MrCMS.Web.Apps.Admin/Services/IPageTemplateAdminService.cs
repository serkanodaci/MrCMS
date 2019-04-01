using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IPageTemplateAdminService
    {
        IPagedList<PageTemplate> Search(PageTemplateSearchQuery query);
        void Add(AddPageTemplateModel model);
        UpdatePageTemplateModel GetEditModel(int id);
        void Update(UpdatePageTemplateModel template);

        List<SelectListItem> GetPageTypeOptions();
        List<SelectListItem> GetLayoutOptions();
        List<SelectListItem> GetUrlGeneratorOptions(string typeName);
    }
}