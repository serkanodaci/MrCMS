using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Application.Pages;
using MrCMS.Web.Areas.Admin.Controllers;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class SearchControllerTests
    {
        private static IDocumentService documentService;
        private static INavigationService navigationService;
        private static ISiteService _siteService;

        [Fact]
        public void SearchController_GetSearchResults_NullStringShouldReturnEmptyObject()
        {
            var searchController = GetSearchController();

            var result = searchController.GetSearchResults(null, null);

            result.Data.Should().BeOfType<object>();
        }

        private static SearchController GetSearchController()
        {
            documentService = A.Fake<IDocumentService>();
            navigationService = A.Fake<INavigationService>();
            _siteService = A.Fake<ISiteService>();
            var searchController = new SearchController(documentService, navigationService, _siteService) { IsAjaxRequest = false };
            return searchController;
        }

        [Fact]
        public void SearchController_GetSearchResults_EmptyStringShouldReturnEmptyObject()
        {
            var searchController = GetSearchController();

            var result = searchController.GetSearchResults("", null);

            result.Data.Should().BeOfType<object>();
        }
        [Fact]
        public void SearchController_GetSearchResults_WhiteSpaceStringgShouldReturnEmptyObject()
        {
            var searchController = GetSearchController();

            var result = searchController.GetSearchResults("  ", null);

            result.Data.Should().BeOfType<object>();
        }

        [Fact]
        public void SearchController_GetSearchResults_CallsDocumentServiceSearchDocuments()
        {
            var searchController = GetSearchController();

            searchController.GetSearchResults("test", null);

            A.CallTo(() => documentService.SearchDocuments<Document>("test")).MustHaveHappened();
        }

        [Fact]
        public void SearchController_GetSearchResults_ReturnsIEnumerableSearchResultModels()
        {
            var searchController = GetSearchController();

            IEnumerable<SearchResultModel> searchResultModels = A.CollectionOfFake<SearchResultModel>(1);
            A.CallTo(() => documentService.SearchDocuments<Document>("test")).Returns(
                searchResultModels);

            var searchResults = searchController.GetSearchResults("test", null);

            searchResults.Data.As<IEnumerable<SearchResultModel>>().Should().BeEquivalentTo(searchResultModels);
        }

        [Fact]
        public void SearchController_GetSearchResultsDetailed_TypeSetCallsSearchDocumentsWithCorrectGenericType()
        {
            var searchController = GetSearchController();

            searchController.GetSearchResults("test", "TextPage");

            A.CallTo(() => documentService.SearchDocuments<TextPage>("test")).MustHaveHappened();
        }

        [Fact]
        public void SearchController_Index_ReturnsViewResult()
        {
            var searchController = GetSearchController();

            searchController.Index("searchterm", "TextPage", 1, 1).Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void SearchController_Index_SetsViewData()
        {
            var searchController = GetSearchController();
            var site = new Site();
            A.CallTo(() => _siteService.GetCurrentSite()).Returns(site);

            var selectListItems = new List<SelectListItem>();
            var documentTypes = new List<SelectListItem>();

            A.CallTo(() => navigationService.GetParentsList(site)).Returns(selectListItems);
            A.CallTo(() => navigationService.GetDocumentTypes("TextPage")).Returns(documentTypes);

            var result = searchController.Index("searchterm", "TextPage", 1, 1).As<ViewResult>();

            result.ViewData["term"].Should().Be("searchterm");
            result.ViewData["type"].Should().Be("TextPage");
            result.ViewData["parent-val"].Should().Be(1);
            result.ViewData["parents"].Should().Be(selectListItems);
            result.ViewData["doc-types"].Should().Be(documentTypes);
        }

        [Fact]
        public void SearchController_Index_IfTypeIsSetUsesTypePassedAsGenericArgument()
        {
            var searchController = GetSearchController();

            searchController.Index("searchterm", "TextPage", 1, 1).As<ViewResult>();

            A.CallTo(() => documentService.SearchDocumentsDetailed<TextPage>("searchterm", 1, 1)).MustHaveHappened();
        }

        [Fact]
        public void SearchController_Index_IfTypeIsNotSetUsesDocument()
        {
            var searchController = GetSearchController();

            searchController.Index("searchterm", "", 1, 1).As<ViewResult>();

            A.CallTo(() => documentService.SearchDocumentsDetailed<Document>("searchterm", 1, 1)).MustHaveHappened();
        }

        [Fact]
        public void SearchController_IndexPost_ReturnsRedirectToRoute()
        {
            var searchController = GetSearchController();

            searchController.IndexPost("test", "TextPage", 1, 1).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void SearchController_IndexPost_PassesArgumentsAsRouteValues()
        {
            var searchController = GetSearchController();

            var result = searchController.IndexPost("test", "TextPage", 1, 1).As<RedirectToRouteResult>();

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["term"].Should().Be("test");
            result.RouteValues["type"].Should().Be("TextPage");
            result.RouteValues["parent"].Should().Be(1);
        }
    }
}