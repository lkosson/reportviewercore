using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Sorting : IPersistable
	{
		private List<ExpressionInfo> m_sortExpressions;

		private List<bool> m_sortDirections;

		private List<bool> m_naturalSortFlags;

		private bool m_naturalSort;

		private List<bool> m_deferredSortFlags;

		private bool m_deferredSort;

		[NonSerialized]
		private SortExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal List<ExpressionInfo> SortExpressions
		{
			get
			{
				return m_sortExpressions;
			}
			set
			{
				m_sortExpressions = value;
			}
		}

		internal List<bool> SortDirections
		{
			get
			{
				return m_sortDirections;
			}
			set
			{
				m_sortDirections = value;
			}
		}

		internal List<bool> NaturalSortFlags
		{
			get
			{
				return m_naturalSortFlags;
			}
			set
			{
				m_naturalSortFlags = value;
			}
		}

		internal List<bool> DeferredSortFlags
		{
			get
			{
				return m_deferredSortFlags;
			}
			set
			{
				m_deferredSortFlags = value;
			}
		}

		internal SortExprHost ExprHost => m_exprHost;

		internal bool NaturalSort
		{
			get
			{
				return m_naturalSort;
			}
			set
			{
				m_naturalSort = value;
			}
		}

		internal bool DeferredSort => m_deferredSort;

		internal bool ShouldApplySorting
		{
			get
			{
				if (!m_naturalSort && !m_deferredSort && m_sortDirections != null)
				{
					return m_sortDirections.Count > 0;
				}
				return false;
			}
		}

		internal Sorting(ConstructionPhase phase)
		{
			if (phase == ConstructionPhase.Publishing)
			{
				m_sortExpressions = new List<ExpressionInfo>();
				m_sortDirections = new List<bool>();
				m_naturalSortFlags = new List<bool>();
				m_deferredSortFlags = new List<bool>();
			}
		}

		internal void ValidateNaturalSortFlags(PublishingContextStruct context)
		{
			m_naturalSort = ValidateExclusiveSortFlag(context, m_naturalSortFlags, "NaturalSort");
		}

		internal void ValidateDeferredSortFlags(PublishingContextStruct context)
		{
			m_deferredSort = ValidateExclusiveSortFlag(context, m_deferredSortFlags, "DeferredSort");
		}

		private static bool ValidateExclusiveSortFlag(PublishingContextStruct context, List<bool> flags, string propertyName)
		{
			if (flags == null || flags.Count == 0)
			{
				return false;
			}
			int count = flags.Count;
			bool flag = flags[0];
			for (int i = 1; i < count; i++)
			{
				if (flag != flags[i])
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidSortFlagCombination, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
					return false;
				}
			}
			return flag;
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.SortStart();
			if (m_sortExpressions != null)
			{
				for (int i = 0; i < m_sortExpressions.Count; i++)
				{
					ExpressionInfo expressionInfo = m_sortExpressions[i];
					expressionInfo.Initialize("SortExpression", context);
					context.ExprHostBuilder.SortExpression(expressionInfo);
				}
			}
			context.ExprHostBuilder.SortEnd();
		}

		internal void SetExprHost(SortExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			Sorting sorting = (Sorting)MemberwiseClone();
			if (m_sortExpressions != null)
			{
				sorting.m_sortExpressions = new List<ExpressionInfo>(m_sortExpressions.Count);
				foreach (ExpressionInfo sortExpression in m_sortExpressions)
				{
					sorting.m_sortExpressions.Add((ExpressionInfo)sortExpression.PublishClone(context));
				}
			}
			if (m_sortDirections != null)
			{
				sorting.m_sortDirections = new List<bool>(m_sortDirections.Count);
				{
					foreach (bool sortDirection in m_sortDirections)
					{
						sorting.m_sortDirections.Add(sortDirection);
					}
					return sorting;
				}
			}
			return sorting;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.SortExpressions, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SortDirections, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Boolean));
			list.Add(new MemberInfo(MemberName.NaturalSortFlags, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Boolean));
			list.Add(new MemberInfo(MemberName.NaturalSort, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DeferredSortFlags, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Boolean, Lifetime.AddedIn(100)));
			list.Add(new MemberInfo(MemberName.DeferredSort, Token.Boolean, Lifetime.AddedIn(100)));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Sorting, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.SortExpressions:
					writer.Write(m_sortExpressions);
					break;
				case MemberName.SortDirections:
					writer.WriteListOfPrimitives(m_sortDirections);
					break;
				case MemberName.NaturalSortFlags:
					writer.WriteListOfPrimitives(m_naturalSortFlags);
					break;
				case MemberName.NaturalSort:
					writer.Write(m_naturalSort);
					break;
				case MemberName.DeferredSortFlags:
					writer.WriteListOfPrimitives(m_deferredSortFlags);
					break;
				case MemberName.DeferredSort:
					writer.Write(m_deferredSort);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.SortExpressions:
					m_sortExpressions = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.SortDirections:
					m_sortDirections = reader.ReadListOfPrimitives<bool>();
					break;
				case MemberName.NaturalSortFlags:
					m_naturalSortFlags = reader.ReadListOfPrimitives<bool>();
					break;
				case MemberName.NaturalSort:
					m_naturalSort = reader.ReadBoolean();
					break;
				case MemberName.DeferredSortFlags:
					m_deferredSortFlags = reader.ReadListOfPrimitives<bool>();
					break;
				case MemberName.DeferredSort:
					m_deferredSort = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Sorting;
		}
	}
}
