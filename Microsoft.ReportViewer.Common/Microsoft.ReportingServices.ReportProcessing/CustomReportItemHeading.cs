using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItemHeading : TablixHeading, IRunningValueHolder
	{
		private bool m_static;

		private CustomReportItemHeadingList m_innerHeadings;

		private DataValueList m_customProperties;

		private int m_exprHostID = -1;

		private RunningValueInfoList m_runningValues;

		[NonSerialized]
		private DataGroupingExprHost m_exprHost;

		internal bool Static
		{
			get
			{
				return m_static;
			}
			set
			{
				m_static = value;
			}
		}

		internal CustomReportItemHeadingList InnerHeadings
		{
			get
			{
				return m_innerHeadings;
			}
			set
			{
				m_innerHeadings = value;
			}
		}

		internal DataValueList CustomProperties
		{
			get
			{
				return m_customProperties;
			}
			set
			{
				m_customProperties = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return m_exprHostID;
			}
			set
			{
				m_exprHostID = value;
			}
		}

		internal RunningValueInfoList RunningValues
		{
			get
			{
				return m_runningValues;
			}
			set
			{
				m_runningValues = value;
			}
		}

		internal DataGroupingExprHost ExprHost => m_exprHost;

		internal CustomReportItemHeading()
		{
		}

		internal CustomReportItemHeading(int id, CustomReportItem crItem)
			: base(id, crItem)
		{
			m_runningValues = new RunningValueInfoList();
		}

		RunningValueInfoList IRunningValueHolder.GetRunningValueList()
		{
			return m_runningValues;
		}

		void IRunningValueHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(m_runningValues != null);
			if (m_runningValues.Count == 0)
			{
				m_runningValues = null;
			}
		}

		internal bool Initialize(int level, CustomReportItemHeadingList peerHeadings, int headingIndex, DataCellsList dataRowCells, ref int currentIndex, ref int maxLevel, InitializationContext context)
		{
			m_level = level;
			if (level > maxLevel)
			{
				maxLevel = level;
			}
			context.ExprHostBuilder.DataGroupingStart(m_isColumn);
			if (m_static)
			{
				Global.Tracer.Assert(!m_subtotal);
				if (m_grouping != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidStaticDataGrouping, Severity.Error, context.ObjectType, context.ObjectName, "DataGrouping");
					m_grouping = null;
				}
				else
				{
					m_sorting = null;
					CommonInitialize(level, dataRowCells, ref currentIndex, ref maxLevel, context);
				}
			}
			else
			{
				if ((context.Location & LocationFlags.InDetail) != 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidDetailDataGrouping, Severity.Error, context.ObjectType, context.ObjectName, "DataGrouping");
					return false;
				}
				if (m_grouping != null && m_grouping.CustomProperties != null)
				{
					if (m_customProperties == null)
					{
						m_customProperties = new DataValueList(m_grouping.CustomProperties.Count);
					}
					m_customProperties.AddRange(m_grouping.CustomProperties);
					m_grouping.CustomProperties = null;
				}
				if (m_subtotal)
				{
					if (m_grouping != null)
					{
						context.AggregateRewriteScopes = new Hashtable();
						context.AggregateRewriteScopes.Add(m_grouping.Name, null);
					}
					Global.Tracer.Assert(peerHeadings[headingIndex] != null);
					int currentIndex2 = currentIndex;
					CustomReportItemHeading customReportItemHeading = HeadingClone(this, dataRowCells, ref currentIndex2, m_headingSpan, context);
					customReportItemHeading.m_innerHeadings = HeadingListClone(m_innerHeadings, dataRowCells, ref currentIndex2, m_headingSpan, context);
					Global.Tracer.Assert(currentIndex + m_headingSpan == currentIndex2);
					Global.Tracer.Assert(!customReportItemHeading.m_subtotal && m_subtotal);
					Global.Tracer.Assert(headingIndex < peerHeadings.Count);
					peerHeadings.Insert(headingIndex + 1, customReportItemHeading);
					context.AggregateRewriteScopes = null;
					context.AggregateRewriteMap = null;
				}
				if (m_grouping != null)
				{
					context.Location |= LocationFlags.InGrouping;
					context.RegisterGroupingScope(m_grouping.Name, m_grouping.SimpleGroupExpressions, m_grouping.Aggregates, m_grouping.PostSortAggregates, m_grouping.RecursiveAggregates, m_grouping);
					ObjectType objectType = context.ObjectType;
					string objectName = context.ObjectName;
					context.ObjectType = ObjectType.Grouping;
					context.ObjectName = m_grouping.Name;
					CommonInitialize(level, dataRowCells, ref currentIndex, ref maxLevel, context);
					context.ObjectType = objectType;
					context.ObjectName = objectName;
					context.UnRegisterGroupingScope(m_grouping.Name);
				}
				else
				{
					context.Location |= LocationFlags.InDetail;
					CommonInitialize(level, dataRowCells, ref currentIndex, ref maxLevel, context);
				}
			}
			m_exprHostID = context.ExprHostBuilder.DataGroupingEnd(m_isColumn);
			m_hasExprHost |= (m_exprHostID >= 0);
			return m_subtotal;
		}

		private void CommonInitialize(int level, DataCellsList dataRowCells, ref int currentIndex, ref int maxLevel, InitializationContext context)
		{
			Initialize(context);
			if (m_customProperties != null)
			{
				context.RegisterRunningValues(m_runningValues);
				m_customProperties.Initialize(null, isCustomProperty: true, context);
				context.UnRegisterRunningValues(m_runningValues);
			}
			if (m_innerHeadings != null)
			{
				Global.Tracer.Assert(context.AggregateEscalateScopes != null);
				if (m_grouping != null)
				{
					context.AggregateEscalateScopes.Add(m_grouping.Name);
				}
				m_headingSpan += m_innerHeadings.Initialize(level + 1, dataRowCells, ref currentIndex, ref maxLevel, context);
				if (m_grouping != null)
				{
					context.AggregateEscalateScopes.RemoveAt(context.AggregateEscalateScopes.Count - 1);
				}
			}
			else
			{
				currentIndex++;
			}
		}

		private static CustomReportItemHeading HeadingClone(CustomReportItemHeading heading, DataCellsList dataRowCells, ref int currentIndex, int headingSpan, InitializationContext context)
		{
			Global.Tracer.Assert(heading != null);
			CustomReportItemHeading customReportItemHeading = new CustomReportItemHeading(context.GenerateSubtotalID(), (CustomReportItem)heading.DataRegionDef);
			customReportItemHeading.m_isColumn = heading.m_isColumn;
			customReportItemHeading.m_level = heading.m_level;
			customReportItemHeading.m_static = true;
			customReportItemHeading.m_subtotal = false;
			customReportItemHeading.m_headingSpan = heading.m_headingSpan;
			if (heading.m_customProperties != null)
			{
				customReportItemHeading.m_customProperties = heading.m_customProperties.DeepClone(context);
			}
			if (heading.m_innerHeadings == null)
			{
				if (heading.m_isColumn)
				{
					int count = dataRowCells.Count;
					for (int i = 0; i < count; i++)
					{
						DataCellList dataCellList = dataRowCells[i];
						Global.Tracer.Assert(currentIndex + headingSpan <= dataCellList.Count);
						dataCellList.Insert(currentIndex + headingSpan, dataCellList[currentIndex].DeepClone(context));
					}
				}
				else
				{
					Global.Tracer.Assert(currentIndex + headingSpan <= dataRowCells.Count);
					DataCellList dataCellList2 = dataRowCells[currentIndex];
					int count2 = dataCellList2.Count;
					DataCellList dataCellList3 = new DataCellList(count2);
					dataRowCells.Insert(currentIndex + headingSpan, dataCellList3);
					for (int j = 0; j < count2; j++)
					{
						dataCellList3.Add(dataCellList2[j].DeepClone(context));
					}
				}
				currentIndex++;
			}
			return customReportItemHeading;
		}

		private static CustomReportItemHeadingList HeadingListClone(CustomReportItemHeadingList headings, DataCellsList dataRowCells, ref int currentIndex, int headingSpan, InitializationContext context)
		{
			if (headings == null)
			{
				return null;
			}
			int count = headings.Count;
			Global.Tracer.Assert(1 <= count);
			CustomReportItemHeadingList customReportItemHeadingList = new CustomReportItemHeadingList(count);
			for (int i = 0; i < count; i++)
			{
				CustomReportItemHeading customReportItemHeading = headings[i];
				if (customReportItemHeading.m_grouping != null)
				{
					context.AggregateRewriteScopes.Add(customReportItemHeading.m_grouping.Name, null);
				}
				CustomReportItemHeading customReportItemHeading2 = HeadingClone(customReportItemHeading, dataRowCells, ref currentIndex, headingSpan, context);
				if (customReportItemHeading.m_innerHeadings != null)
				{
					customReportItemHeading2.m_innerHeadings = HeadingListClone(customReportItemHeading.m_innerHeadings, dataRowCells, ref currentIndex, headingSpan, context);
				}
				if (customReportItemHeading.m_grouping != null)
				{
					context.AggregateRewriteScopes.Remove(customReportItemHeading.m_grouping.Name);
				}
				customReportItemHeadingList.Add(customReportItemHeading2);
			}
			return customReportItemHeadingList;
		}

		internal static bool ValidateProcessingRestrictions(CustomReportItemHeadingList headings, bool isColumn, bool hasStatic, InitializationContext context)
		{
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			string propertyName = isColumn ? "column" : "row";
			if (headings != null)
			{
				for (int i = 0; i < headings.Count; i++)
				{
					CustomReportItemHeading customReportItemHeading = headings[i];
					if (!customReportItemHeading.Static && customReportItemHeading.Grouping == null)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsInvalidGrouping, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
						flag = false;
					}
					if (customReportItemHeading.Subtotal)
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsCRISubtotalNotSupported, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
						flag = false;
					}
					if (customReportItemHeading.Static && hasStatic)
					{
						flag3 = true;
					}
					if (customReportItemHeading.Static && customReportItemHeading.InnerHeadings != null)
					{
						flag4 = true;
					}
					if (!customReportItemHeading.Static && headings.Count > 1)
					{
						flag2 = true;
					}
					if (flag && !flag2 && !flag3 && !flag4 && customReportItemHeading.InnerHeadings != null && !ValidateProcessingRestrictions(customReportItemHeading.InnerHeadings, isColumn, customReportItemHeading.Static, context))
					{
						flag = false;
					}
				}
			}
			if (flag3)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsCRIMultiStaticColumnsOrRows, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
				flag = false;
			}
			if (flag4)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsCRIStaticWithSubgroups, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
				flag = false;
			}
			if (flag2)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsCRIMultiNonStaticGroups, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
				flag = false;
			}
			return flag;
		}

		internal void CopySubHeadingAggregates()
		{
			if (m_innerHeadings != null)
			{
				int count = m_innerHeadings.Count;
				for (int i = 0; i < count; i++)
				{
					CustomReportItemHeading customReportItemHeading = m_innerHeadings[i];
					customReportItemHeading.CopySubHeadingAggregates();
					Tablix.CopyAggregates(customReportItemHeading.Aggregates, m_aggregates);
					Tablix.CopyAggregates(customReportItemHeading.PostSortAggregates, m_postSortAggregates);
					Tablix.CopyAggregates(customReportItemHeading.RecursiveAggregates, m_aggregates);
				}
			}
		}

		internal void TransferHeadingAggregates()
		{
			if (m_innerHeadings != null)
			{
				m_innerHeadings.TransferHeadingAggregates();
			}
			if (m_grouping != null)
			{
				for (int i = 0; i < m_aggregates.Count; i++)
				{
					m_grouping.Aggregates.Add(m_aggregates[i]);
				}
			}
			m_aggregates = null;
			if (m_grouping != null)
			{
				for (int j = 0; j < m_postSortAggregates.Count; j++)
				{
					m_grouping.PostSortAggregates.Add(m_postSortAggregates[j]);
				}
			}
			m_postSortAggregates = null;
			if (m_grouping != null)
			{
				for (int k = 0; k < m_recursiveAggregates.Count; k++)
				{
					m_grouping.RecursiveAggregates.Add(m_recursiveAggregates[k]);
				}
			}
			m_recursiveAggregates = null;
		}

		internal void SetExprHost(IList<DataGroupingExprHost> dataGroupingHosts, ObjectModelImpl reportObjectModel)
		{
			if (m_exprHostID >= 0)
			{
				Global.Tracer.Assert(dataGroupingHosts != null && dataGroupingHosts.Count > m_exprHostID && reportObjectModel != null);
				m_exprHost = dataGroupingHosts[m_exprHostID];
				m_exprHost.SetReportObjectModel(reportObjectModel);
				if (m_exprHost.GroupingHost != null)
				{
					Global.Tracer.Assert(m_grouping != null);
					m_grouping.SetExprHost(m_exprHost.GroupingHost, reportObjectModel);
				}
				if (m_exprHost.SortingHost != null)
				{
					Global.Tracer.Assert(m_sorting != null);
					m_sorting.SetExprHost(m_exprHost.SortingHost, reportObjectModel);
				}
				if (m_customProperties != null)
				{
					Global.Tracer.Assert(m_customProperties != null);
					m_customProperties.SetExprHost(m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Static, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.InnerHeadings, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.CustomReportItemHeadingList));
			memberInfoList.Add(new MemberInfo(MemberName.CustomProperties, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueList));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.RunningValues, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RunningValueInfoList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TablixHeading, memberInfoList);
		}
	}
}
