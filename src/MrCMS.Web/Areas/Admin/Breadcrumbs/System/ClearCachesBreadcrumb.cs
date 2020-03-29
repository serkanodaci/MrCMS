﻿using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.System
{
    public class ClearCachesBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override int Order => 14;
        public override string Controller => "ClearCaches";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}