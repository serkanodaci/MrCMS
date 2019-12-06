﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Infrastructure.Models.Tabs;

namespace MrCMS.Web.Apps.Admin.Models.Forms
{
    public class FormGDPRSettingsTab : AdminTab<Form>
    {
        public override int Order
        {
            get { return 400; }
        }

        public override Type ParentType
        {
            get { return null; }
        }

        public override Type ModelType => typeof(FormGDPRTabViewModel);

        public override string TabHtmlId
        {
            get { return "form-gdpr-tab"; }
        }

        public override Task RenderTabPane(IHtmlHelper html, IMapper mapper, Form form)
        {
            return html.RenderPartialAsync("GDPR", mapper.Map<FormGDPRTabViewModel>(form));
        }

        public override string Name(IServiceProvider serviceProvider, Form entity)
        {
            return "GDPR";
        }

        public override bool ShouldShow(IServiceProvider serviceProvider, Form entity)
        {
            return true;
        }
    }
}