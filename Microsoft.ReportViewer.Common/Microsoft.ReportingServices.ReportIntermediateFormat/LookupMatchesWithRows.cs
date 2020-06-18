using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class LookupMatchesWithRows : LookupMatches
	{
		private int m_firstRow = -1;

		private ScalableList<int> m_rows;

		[NonSerialized]
		private bool m_hasBeenTransferred;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		public override int Size => base.Size + 4 + ItemSizes.SizeOf(m_rows);

		internal LookupMatchesWithRows()
		{
		}

		internal override void AddRow(long rowOffset, int rowIndex, IScalabilityCache scaleCache)
		{
			if (base.HasRow)
			{
				if (m_rows == null)
				{
					m_rows = new ScalableList<int>(0, scaleCache, 500, 10);
				}
				m_rows.Add(rowIndex);
			}
			else
			{
				m_firstRow = rowIndex;
			}
			base.AddRow(rowOffset, rowIndex, scaleCache);
		}

		internal override void SetupRow(int matchIndex, OnDemandProcessingContext odpContext)
		{
			if (m_hasBeenTransferred)
			{
				base.SetupRow(matchIndex, odpContext);
				return;
			}
			CommonRowCache tablixProcessingLookupRowCache = odpContext.TablixProcessingLookupRowCache;
			int num = -1;
			num = ((matchIndex != 0) ? m_rows[matchIndex - 1] : m_firstRow);
			tablixProcessingLookupRowCache.SetupRow(num, odpContext);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			if (m_hasBeenTransferred)
			{
				return;
			}
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.FirstRow:
					writer.Write(m_firstRow);
					break;
				case MemberName.Rows:
					writer.Write(m_rows);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.FirstRow:
					m_firstRow = reader.ReadInt32();
					break;
				case MemberName.Rows:
					m_rows = reader.ReadRIFObject<ScalableList<int>>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			if (m_hasBeenTransferred)
			{
				return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupMatches;
			}
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupMatchesWithRows;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_Declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.FirstRow, Token.Int32));
				list.Add(new MemberInfo(MemberName.Rows, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, Token.Int32));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupMatchesWithRows, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupMatches, list);
			}
			return m_Declaration;
		}

		public override void TransferTo(IScalabilityCache scaleCache)
		{
			base.TransferTo(scaleCache);
			if (m_rows != null)
			{
				m_rows.Dispose();
				m_rows = null;
			}
			m_hasBeenTransferred = true;
		}
	}
}
