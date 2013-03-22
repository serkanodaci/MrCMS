﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class SearchPageController : MrCMSAppUIController<CoreApp>
    {
        private readonly IDocumentService _documentService;

        public SearchPageController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        public ActionResult Show(SearchPage page)
        {
            int pageVal;
            ViewData["term"] = Request["q"];
            int? pageNum = string.IsNullOrWhiteSpace(Request["p"])
                               ? null
                               : int.TryParse(Request["p"], out pageVal)
                                     ? pageVal
                                     : (int?)null;
            ViewData["searchResults"] = _documentService.SiteSearch(Request["q"], pageNum);

            return View(page);
        }

        public ActionResult Post(SearchPage page, FormCollection form)
        {
            return Redirect(page.LiveUrlSegment + string.Format("?q={0}", form["term"]));
        }
    }
}
