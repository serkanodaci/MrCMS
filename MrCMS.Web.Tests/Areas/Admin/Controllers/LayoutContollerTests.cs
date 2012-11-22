﻿using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Models;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class LayoutContollerTests
    {
        [Fact]
        public void LayoutController_AddGet_ShouldReturnAddPageModel()
        {
            var documentService = A.Fake<IDocumentService>();
            var layoutController = new LayoutController(documentService) { IsAjaxRequest = false };

            var actionResult = layoutController.Add(1) as ViewResult;

            actionResult.Model.Should().BeOfType<AddPageModel>();
        }

        [Fact]
        public void LayoutController_AddGet_ShouldSetParentIdOfModelToIdInMethod()
        {
            var documentService = A.Fake<IDocumentService>();
            var layoutController = new LayoutController(documentService) { IsAjaxRequest = false };
            A.CallTo(() => documentService.GetDocument<Document>(1)).Returns(new TextPage { Id = 1 });

            var actionResult = layoutController.Add(1) as ViewResult;

            (actionResult.Model as AddPageModel).ParentId.Should().Be(1);
        }

        [Fact]
        public void LayoutController_AddPost_ShouldCallSaveDocument()
        {
            var documentService = A.Fake<IDocumentService>();
            var layoutController = new LayoutController(documentService) {IsAjaxRequest = false};

            var layout = new Layout();
            layoutController.Add(layout);

            A.CallTo(() => documentService.AddDocument(layout)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void LayoutController_AddPost_ShouldRedirectToView()
        {
            var documentService = A.Fake<IDocumentService>();
            var layoutController = new LayoutController(documentService) {IsAjaxRequest = false};

            var layout = new Layout {Id = 1};
            var result = layoutController.Add(layout) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void LayoutController_EditGet_ShouldReturnAViewResult()
        {
            var documentService = A.Fake<IDocumentService>();
            var layoutController = new LayoutController(documentService) {IsAjaxRequest = false};
            var layout = new Layout {Id = 1};

            var result = layoutController.Edit(1);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void LayoutController_EditGet_ShouldReturnLayoutAsViewModel()
        {
            var documentService = A.Fake<IDocumentService>();
            var layoutController = new LayoutController(documentService) {IsAjaxRequest = false};
            var layout = new Layout {Id = 1};
            A.CallTo(() => documentService.GetDocument<Layout>(1)).Returns(layout);

            var result = layoutController.Edit(1) as ViewResult;

            result.Model.Should().Be(layout);
        }

        [Fact]
        public void LayoutController_EditPost_ShouldCallSaveDocument()
        {
            var documentService = A.Fake<IDocumentService>();
            var layoutController = new LayoutController(documentService) {IsAjaxRequest = false};
            var layout = new Layout {Id = 1};

            layoutController.Edit(layout);

            A.CallTo(() => documentService.SaveDocument(layout)).MustHaveHappened();
        }

        [Fact]
        public void LayoutController_EditPost_ShouldRedirectToEdit()
        {
            var documentService = A.Fake<IDocumentService>();
            var layoutController = new LayoutController(documentService) {IsAjaxRequest = false};
            var layout = new Layout {Id = 1};

            var actionResult = layoutController.Edit(layout);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            (actionResult as RedirectToRouteResult).RouteValues["action"].Should().Be("Edit");
            (actionResult as RedirectToRouteResult).RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void LayoutController_Sort_ShouldCallGetDocumentsByParentId()
        {
            var documentService = A.Fake<IDocumentService>();
            var layoutController = new LayoutController(documentService) {IsAjaxRequest = false};

            layoutController.Sort(1);

            A.CallTo(() => documentService.GetAdminDocumentsByParentId<Layout>(1)).MustHaveHappened();
        }

        [Fact]
        public void LayoutController_Sort_ShouldUseTheResultOfDocumentsByParentIdsAsModel()
        {
            var documentService = A.Fake<IDocumentService>();
            var layoutController = new LayoutController(documentService) {IsAjaxRequest = false};
            var layouts = new List<Layout> {new Layout()};
            A.CallTo(() => documentService.GetAdminDocumentsByParentId<Layout>(1)).Returns(layouts);

            var viewResult = layoutController.Sort(1).As<ViewResult>();

            viewResult.Model.As<List<Layout>>().Should().BeEquivalentTo(layouts);
        }

        [Fact]
        public void LayoutController_SortAction_ShouldCallSortOrderOnTheDocumentServiceWithTheRelevantValues()
        {
            var documentService = A.Fake<IDocumentService>();
            var layoutController = new LayoutController(documentService) {IsAjaxRequest = false};
            layoutController.SortAction(1, 2);

            A.CallTo(() => documentService.SetOrder(1, 2)).MustHaveHappened();
        }

        [Fact]
        public void LayoutContoller_View_CallsGetDocumentWithId()
        {
            var documentService = A.Fake<IDocumentService>();
            var layoutController = new LayoutController(documentService) {IsAjaxRequest = false};

            layoutController.View(1);

            A.CallTo(() => documentService.GetDocument<Layout>(1)).MustHaveHappened();
        }

        [Fact]
        public void LayoutController_View_InvalidIdReturnsRedirectToIndex()
        {
            var documentService = A.Fake<IDocumentService>();
            A.CallTo(() => documentService.GetDocument<Layout>(1)).Returns(null);
            var layoutController = new LayoutController(documentService) {IsAjaxRequest = false};

            var actionResult = layoutController.View(1);

            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void LayoutController_Index_ReturnsViewResult()
        {
            var documentService = A.Fake<IDocumentService>();
            var layoutController = new LayoutController(documentService) { IsAjaxRequest = false };

            var actionResult = layoutController.Index();

            actionResult.Should().NotBeNull();
        }

        [Fact]
        public void LayoutController_SuggestDocumentUrl_ShouldCallGetDocumentUrl()
        {
            var documentService = A.Fake<IDocumentService>();
            var layoutController = new LayoutController(documentService) { IsAjaxRequest = false };

            layoutController.SuggestDocumentUrl(1, "test");

            A.CallTo(() => documentService.GetDocumentUrl("test", 1, false)).MustHaveHappened();
        }

        [Fact]
        public void LayoutController_SuggestDocumentUrl_ShouldReturnTheResultOfGetDocumentUrl()
        {
            var documentService = A.Fake<IDocumentService>();
            var layoutController = new LayoutController(documentService) { IsAjaxRequest = false };

            A.CallTo(() => documentService.GetDocumentUrl("test", 1, false)).Returns("test/result");
            var url = layoutController.SuggestDocumentUrl(1, "test");

            url.Should().BeEquivalentTo("test/result");
        }
    }
}