using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Management;

namespace MrCMS.Indexing.Definitions
{
    public class PublishedOnFieldDefinition : StringFieldDefinition<AdminWebpageIndexDefinition, Webpage>
    {
        public PublishedOnFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "publishon")
        {
        }

        protected override async IAsyncEnumerable<string> GetValues(Webpage obj)
        {
            yield return DateTools.DateToString(obj.PublishOn.GetValueOrDefault(DateTime.MaxValue), DateTools.Resolution.SECOND);
        }
    }
}