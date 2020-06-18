using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class OnDemandStateManagerDefinitionOnly : OnDemandStateManager
	{
		internal override IReportScopeInstance LastROMInstance
		{
			get
			{
				FireAssert("LastROMInstance");
				return null;
			}
		}

		internal override IRIFReportScope LastTablixProcessingReportScope
		{
			get
			{
				FireAssert("LastTablixProcessingReportScope");
				return null;
			}
			set
			{
				FireAssert("LastTablixProcessingReportScope");
			}
		}

		internal override IInstancePath LastRIFObject
		{
			get
			{
				FireAssert("LastRIFObject");
				return null;
			}
			set
			{
				FireAssert("LastRIFObject");
			}
		}

		internal override QueryRestartInfo QueryRestartInfo => null;

		internal override ExecutedQueryCache ExecutedQueryCache => null;

		public OnDemandStateManagerDefinitionOnly(OnDemandProcessingContext odpContext)
			: base(odpContext)
		{
		}

		internal override ExecutedQueryCache SetupExecutedQueryCache()
		{
			return ExecutedQueryCache;
		}

		internal override void ResetOnDemandState()
		{
		}

		internal override int RecursiveLevel(string scopeName)
		{
			FireAssert("RecursiveLevel");
			return -1;
		}

		internal override bool InScope(string scopeName)
		{
			FireAssert("InScope");
			return false;
		}

		internal override Dictionary<string, object> GetCurrentSpecialGroupingValues()
		{
			FireAssert("GetCurrentSpecialGroupingValues");
			return null;
		}

		internal override void RestoreContext(IInstancePath originalObject)
		{
			FireAssert("RestoreContext");
		}

		internal override void SetupContext(IInstancePath rifObject, IReportScopeInstance romInstance)
		{
			FireAssert("SetupContext");
		}

		internal override void SetupContext(IInstancePath rifObject, IReportScopeInstance romInstance, int moveNextInstanceIndex)
		{
			FireAssert("SetupContext");
		}

		internal override bool CalculateAggregate(string aggregateName)
		{
			FireAssert("CalculateAggregate");
			return false;
		}

		internal override bool CalculateLookup(LookupInfo lookup)
		{
			FireAssert("CalculateLookup");
			return false;
		}

		internal override bool PrepareFieldsCollectionForDirectFields()
		{
			FireAssert("PrepareFieldsCollectionForDirectFields");
			return false;
		}

		internal override void EvaluateScopedFieldReference(string scopeName, int fieldIndex, ref Microsoft.ReportingServices.RdlExpressions.VariantResult result)
		{
			FireAssert("EvaluateScopedFieldReference");
		}

		private void FireAssert(string methodOrPropertyName)
		{
			Global.Tracer.Assert(condition: false, methodOrPropertyName + " should not be called in Definition-only mode.");
		}

		internal override IRecordRowReader CreateSequentialDataReader(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, out Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance)
		{
			FireAssert("CreateSequentialDataReader");
			throw new InvalidOperationException("This method is not valid for this StateManager type.");
		}

		internal override void BindNextMemberInstance(IInstancePath rifObject, IReportScopeInstance romInstance, int moveNextInstanceIndex)
		{
			FireAssert("BindNextMemberInstance");
			throw new InvalidOperationException("This method is not valid for this StateManager type.");
		}

		internal override bool ShouldStopPipelineAdvance(bool rowAccepted)
		{
			FireAssert("ShouldStopPipelineAdvance");
			throw new InvalidOperationException("This method is not valid for this StateManager type.");
		}

		internal override void CreatedScopeInstance(IRIFReportDataScope scope)
		{
			FireAssert("CreateScopeInstance");
			throw new InvalidOperationException("This method is not valid for this StateManager type.");
		}

		internal override bool ProcessOneRow(IRIFReportDataScope scope)
		{
			FireAssert("ProcessOneRow");
			throw new InvalidOperationException("This method is not valid for this StateManager type.");
		}

		internal override bool CheckForPrematureServerAggregate(string aggregateName)
		{
			FireAssert("CheckForPrematureServerAggregate");
			throw new InvalidOperationException("This method is not valid for this StateManager type.");
		}
	}
}
