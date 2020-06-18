using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal static class GroupTreeTracer
	{
		private static List<string> m_strings = new List<string>();

		internal static void TraceReportInstance(OnDemandMetadata odpMetadata, ReportInstance reportInstance, Report reportDefinition, int level)
		{
			if (!Global.Tracer.TraceVerbose || reportInstance == null)
			{
				return;
			}
			if (odpMetadata != null)
			{
				TraceMetadata(odpMetadata, level);
			}
			Global.Tracer.Trace("{0}Report: NoRows={1}, Language={2}, Variables={3}", GetEmptyString(level), reportInstance.NoRows, reportInstance.Language, FlattenObjectArray(reportInstance.VariableValues));
			if (odpMetadata == null && reportDefinition != null && reportDefinition.MappingDataSetIndexToDataSet.Count > 0)
			{
				Global.Tracer.Trace("{0}{1} DataSetInstances: ", GetEmptyString(level), reportDefinition.MappingDataSetIndexToDataSet.Count);
				IEnumerator cachedDataSetInstances = reportInstance.GetCachedDataSetInstances();
				while (cachedDataSetInstances.MoveNext())
				{
					DataSetInstance dataSetInstance = (DataSetInstance)cachedDataSetInstances.Current;
					if (dataSetInstance != null)
					{
						TraceDataSetInstance(dataSetInstance, level + 1);
					}
				}
			}
			TraceScopeInstance(reportInstance, level);
		}

		private static string GetEmptyString(int level)
		{
			int count = m_strings.Count;
			if (level >= count)
			{
				for (int i = count; i <= level; i++)
				{
					m_strings.Add(new string(' ', level * 3));
				}
			}
			return m_strings[level];
		}

		private static void TraceMetadata(OnDemandMetadata odpMetadata, int level)
		{
			if (odpMetadata == null)
			{
				return;
			}
			if (odpMetadata.ReportSnapshot != null)
			{
				Global.Tracer.Trace("{0}ReportSnapshot: Time={1}, Language={2}, User={3}, Path={4}", GetEmptyString(level), odpMetadata.ReportSnapshot.ExecutionTime, odpMetadata.ReportSnapshot.Language, odpMetadata.ReportSnapshot.RequestUserName, odpMetadata.ReportSnapshot.ReportServerUrl + odpMetadata.ReportSnapshot.ReportFolder);
				Global.Tracer.Trace("{0}Snapshot flags: Bookmarks={1}, DocumentMap={2}, ShowHide={3}, UserSort={4}", GetEmptyString(level), odpMetadata.ReportSnapshot.HasBookmarks, odpMetadata.ReportSnapshot.HasDocumentMap, odpMetadata.ReportSnapshot.HasShowHide, odpMetadata.ReportSnapshot.HasUserSortFilter);
			}
			if (odpMetadata.DataChunkMap == null)
			{
				return;
			}
			Global.Tracer.Trace("{0}Data chunk map: ", GetEmptyString(level));
			lock (odpMetadata.DataChunkMap)
			{
				IDictionaryEnumerator dictionaryEnumerator = odpMetadata.DataChunkMap.GetEnumerator();
				while (dictionaryEnumerator.MoveNext())
				{
					Global.Tracer.Trace("{0}Data chunk={1}", GetEmptyString(level + 1), (string)dictionaryEnumerator.Key);
					TraceDataSetInstance(dictionaryEnumerator.Value as DataSetInstance, level + 2);
				}
			}
		}

		private static void TraceScopeInstance(ScopeInstance scopeInstance, int level)
		{
			Global.Tracer.Assert(scopeInstance != null, "(null != scopeInstance)");
			if (0 < scopeInstance.FirstRowOffset)
			{
				Global.Tracer.Trace("{0}FirstRowOffset={1}", GetEmptyString(level), scopeInstance.FirstRowOffset);
			}
			if (scopeInstance.AggregateValues != null && scopeInstance.AggregateValues.Count != 0)
			{
				StringBuilder stringBuilder = new StringBuilder(GetEmptyString(level));
				stringBuilder.Append("Aggregates=");
				foreach (DataAggregateObjResult aggregateValue in scopeInstance.AggregateValues)
				{
					if (aggregateValue == null)
					{
						stringBuilder.Append("(NULL)");
					}
					else
					{
						stringBuilder.Append(aggregateValue.Value);
					}
					stringBuilder.Append("#");
				}
				Global.Tracer.Trace(stringBuilder.ToString());
			}
			if (scopeInstance.SubreportInstances != null && scopeInstance.SubreportInstances.Count != 0)
			{
				int count = scopeInstance.SubreportInstances.Count;
				Global.Tracer.Trace("{0}{1} SubReportInstances:", GetEmptyString(level), count);
				for (int i = 0; i < count; i++)
				{
					if (scopeInstance.SubreportInstances[i] != null)
					{
						TraceSubReportInstance(scopeInstance.SubreportInstances[i].Value(), level + 1);
					}
				}
			}
			if (scopeInstance.DataRegionInstances == null || scopeInstance.DataRegionInstances.Count == 0)
			{
				return;
			}
			int count2 = scopeInstance.DataRegionInstances.Count;
			Global.Tracer.Trace("{0}{1} DataRegionInstances:", GetEmptyString(level), count2);
			for (int j = 0; j < count2; j++)
			{
				if (scopeInstance.DataRegionInstances[j] != null)
				{
					TraceDataRegionInstance(scopeInstance.DataRegionInstances[j].Value(), level + 1);
				}
			}
		}

		private static void TraceDataSetInstance(DataSetInstance instance, int level)
		{
			Global.Tracer.Assert(instance != null, "(null != instance)");
			Global.Tracer.Trace("{0}DataSet={1}, NoRows={2}, RowCount={3}, Lcid={4}, CS={5}, AS={6}, KS={7}, WS={8}, CommandText={9}", GetEmptyString(level), (instance.DataSetDef != null) ? instance.DataSetDef.Name : null, instance.NoRows, instance.RecordSetSize, instance.LCID, instance.CaseSensitivity.ToString(), instance.AccentSensitivity.ToString(), instance.KanatypeSensitivity.ToString(), instance.WidthSensitivity.ToString(), instance.RewrittenCommandText);
			TraceScopeInstance(instance, level);
		}

		private static void TraceDataRegionInstance(DataRegionInstance instance, int level)
		{
			if (instance != null)
			{
				Global.Tracer.Assert(instance != null, "(null != instance)");
				Global.Tracer.Trace("{0}DataRegion={1}, [id={2}], DataSetIndex={3}", GetEmptyString(level), instance.DataRegionDef.Name, instance.DataRegionDef.ID, instance.DataSetIndexInCollection);
				TraceScopeInstance(instance, level);
				if (instance.TopLevelColumnMembers != null)
				{
					Global.Tracer.Trace("{0}{1} Top Level COLUMN Members:", GetEmptyString(level), instance.TopLevelColumnMembers.Count);
					TraceDataRegionMemberInstances(instance.TopLevelColumnMembers, level);
				}
				if (instance.TopLevelRowMembers != null)
				{
					Global.Tracer.Trace("{0}{1} Top Level ROW Members:", GetEmptyString(level), instance.TopLevelRowMembers.Count);
					TraceDataRegionMemberInstances(instance.TopLevelRowMembers, level);
				}
				if (instance.Cells != null && instance.Cells.Count > 0)
				{
					Global.Tracer.Trace("{0} DataRegion Cells:", GetEmptyString(level));
					TraceCellInstances(instance.Cells, level + 1);
				}
			}
		}

		private static void TraceDataRegionMemberInstances(List<ScalableList<DataRegionMemberInstance>> members, int level)
		{
			if (members == null || members.Count == 0)
			{
				return;
			}
			foreach (ScalableList<DataRegionMemberInstance> member in members)
			{
				foreach (DataRegionMemberInstance item in member)
				{
					TraceDataRegionMemberInstance(item, level + 1);
				}
			}
		}

		private static void TraceDataRegionMemberInstance(DataRegionMemberInstance instance, int level)
		{
			Global.Tracer.Assert(instance != null, "(null != instance)");
			string emptyString = GetEmptyString(level);
			Global.Tracer.Trace("{0}DataRegionMemberInstance={1}, [id={2}], Index={3}, RecursiveLevel={4}", emptyString, instance.MemberDef.Grouping.Name, instance.MemberDef.ID, instance.MemberInstanceIndexWithinScopeLevel, instance.RecursiveLevel);
			if (instance.GroupVariables != null)
			{
				Global.Tracer.Trace("{0}Group Variables={1}", emptyString, FlattenObjectArray(instance.GroupVariables));
			}
			if (instance.GroupExprValues != null)
			{
				Global.Tracer.Trace("{0}Group Expr.Vals={1}", emptyString, FlattenObjectArray(instance.GroupExprValues));
			}
			TraceScopeInstance(instance, level + 1);
			TraceCellInstances(instance.Cells, level + 1);
			if (instance.Children != null && instance.Children.Count != 0)
			{
				TraceDataRegionMemberInstances(instance.Children, level + 1);
			}
		}

		private static void TraceCellInstances(ScalableList<DataCellInstanceList> cells, int level)
		{
			if (cells == null || cells.Count == 0)
			{
				return;
			}
			int num = 0;
			foreach (DataCellInstanceList cell in cells)
			{
				Global.Tracer.Trace("{0}CellInstances for index={1}:", GetEmptyString(level), num++);
				if (cell == null)
				{
					continue;
				}
				foreach (DataCellInstance item in cell)
				{
					TraceCellInstance(item, level + 1);
				}
			}
		}

		private static void TraceCellInstance(DataCellInstance instance, int level)
		{
			if (instance == null)
			{
				return;
			}
			string format = "{0}DataCellInstance [id={1}]";
			string text = null;
			string text2 = null;
			if (instance.CellDef is TablixCell && ((TablixCell)instance.CellDef).CellContents != null)
			{
				format = "{0}DataCellInstance [id={1}, name={2}]";
				text = ((TablixCell)instance.CellDef).CellContents.Name;
				if (((TablixCell)instance.CellDef).AltCellContents != null)
				{
					format = "{0}DataCellInstance [id={1}, name={2}, altname={3}]";
					text2 = ((TablixCell)instance.CellDef).AltCellContents.Name;
				}
			}
			Global.Tracer.Trace(format, GetEmptyString(level), instance.CellDef.ID, text, text2);
			TraceScopeInstance(instance, level);
		}

		private static void TraceSubReportInstance(SubReportInstance instance, int level)
		{
			if (instance != null)
			{
				Global.Tracer.Trace("{0}SubReport, Status={1}, Error={2}, NoRows={3}, InstanceUniqueName={4}, Culture={5}", GetEmptyString(level), instance.RetrievalStatus, instance.ProcessedWithError, instance.ProcessedWithError || instance.NoRows, instance.InstanceUniqueName, instance.ThreadCulture);
				TraceParameters(instance.Parameters, level);
				if (instance.SubReportDef.Report != null)
				{
					_ = instance.SubReportDef.Report.MappingDataSetIndexToDataSet.Count;
				}
				TraceReportInstance(null, instance.ReportInstance.Value(), instance.SubReportDef.Report, level);
			}
		}

		private static void TraceParameters(ParametersImpl parameters, int level)
		{
			if (parameters != null)
			{
				for (int i = 0; i < parameters.Count; i++)
				{
					Global.Tracer.Trace("{0}Parameter{1}: MV={2}, {3} values: [{4}] labels: [{5}]", GetEmptyString(level), i, parameters.Collection[i].IsMultiValue, parameters.Collection[i].Count, FlattenParameterArray(parameters.Collection[i].Value), FlattenParameterArray(parameters.Collection[i].Label));
				}
			}
		}

		private static string FlattenParameterArray(object parameterValueOrLabel)
		{
			if (parameterValueOrLabel == null)
			{
				return null;
			}
			ICollection collection = parameterValueOrLabel as ICollection;
			if (collection != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (object item in collection)
				{
					stringBuilder.Append(GetObjectValue(item));
					stringBuilder.Append("#");
				}
				return stringBuilder.ToString();
			}
			return GetObjectValue(parameterValueOrLabel);
		}

		private static string FlattenObjectArray(object[] objects)
		{
			if (objects == null || objects.Length == 0)
			{
				return null;
			}
			if (objects.Length == 1)
			{
				return GetObjectValue(objects[0]);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GetObjectValue(objects[0]));
			for (int i = 1; i < objects.Length; i++)
			{
				stringBuilder.Append("#");
				stringBuilder.Append(GetObjectValue(objects[i]));
			}
			return stringBuilder.ToString();
		}

		private static string GetObjectValue(object obj)
		{
			if (obj == null)
			{
				return "";
			}
			return obj.ToString();
		}
	}
}
