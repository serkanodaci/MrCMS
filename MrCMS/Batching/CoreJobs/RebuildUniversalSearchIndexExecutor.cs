﻿using System;
using System.Threading.Tasks;

namespace MrCMS.Batching.CoreJobs
{
    // TODO: universal search
    public class RebuildUniversalSearchIndexExecutor : BaseBatchJobExecutor<RebuildUniversalSearchIndex>
    {
        //private readonly IUniversalSearchIndexManager _universalSearchIndexManager;

        public RebuildUniversalSearchIndexExecutor(ISetBatchJobExecutionStatus setBatchJobJobExecutionStatus
            //, IUniversalSearchIndexManager universalSearchIndexManager
        )
            : base(setBatchJobJobExecutionStatus)
        {
            //_universalSearchIndexManager = universalSearchIndexManager;
        }

        protected override BatchJobExecutionResult OnExecute(RebuildUniversalSearchIndex batchJob)
        {
            try
            {
                //_universalSearchIndexManager.ReindexAll();
                return BatchJobExecutionResult.Success();
            }
            catch (Exception exception)
            {
                //CurrentRequestData.ErrorSignal.Raise(exception);
                return BatchJobExecutionResult.Failure(exception.Message);
            }
        }

        protected override Task<BatchJobExecutionResult> OnExecuteAsync(RebuildUniversalSearchIndex batchJob)
        {
            throw new NotImplementedException();
        }
    }
}