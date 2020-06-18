using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal abstract class IDOwner : IInstancePath, IPersistable, IReferenceable, IGlobalIDOwner
	{
		protected int m_ID;

		[NonSerialized]
		protected bool m_isClone;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		protected string m_cachedDefinitionPath;

		[NonSerialized]
		private InstancePathItem m_instancePathItem;

		[NonSerialized]
		protected IDOwner m_parentIDOwner;

		[NonSerialized]
		protected List<InstancePathItem> m_cachedInstancePath;

		[NonSerialized]
		protected int m_globalID;

		[NonSerialized]
		protected string m_renderingModelID;

		public int ID
		{
			get
			{
				return m_ID;
			}
			set
			{
				m_ID = value;
			}
		}

		public int GlobalID
		{
			get
			{
				return m_globalID;
			}
			set
			{
				m_globalID = value;
			}
		}

		internal string RenderingModelID
		{
			get
			{
				if (m_renderingModelID == null)
				{
					m_renderingModelID = m_globalID.ToString(CultureInfo.InvariantCulture);
				}
				return m_renderingModelID;
			}
		}

		public InstancePathItem InstancePathItem
		{
			get
			{
				if (m_instancePathItem == null)
				{
					m_instancePathItem = CreateInstancePathItem();
				}
				return m_instancePathItem;
			}
		}

		internal string SubReportDefinitionPath
		{
			get
			{
				if (m_cachedDefinitionPath == null)
				{
					if (m_parentIDOwner != null)
					{
						m_cachedDefinitionPath = m_parentIDOwner.SubReportDefinitionPath;
					}
					else
					{
						m_cachedDefinitionPath = "";
					}
					if (InstancePathItem.Type == InstancePathItemType.SubReport)
					{
						m_cachedDefinitionPath = m_cachedDefinitionPath + "x" + m_ID.ToString(CultureInfo.InvariantCulture);
					}
				}
				return m_cachedDefinitionPath;
			}
		}

		public virtual List<InstancePathItem> InstancePath
		{
			get
			{
				if (m_cachedInstancePath == null)
				{
					m_cachedInstancePath = new List<InstancePathItem>();
					if (ParentInstancePath != null)
					{
						List<InstancePathItem> instancePath = ParentInstancePath.InstancePath;
						m_cachedInstancePath.AddRange(instancePath);
					}
					if (!InstancePathItem.IsEmpty)
					{
						m_cachedInstancePath.Add(InstancePathItem);
					}
				}
				return m_cachedInstancePath;
			}
		}

		public IInstancePath ParentInstancePath
		{
			get
			{
				return m_parentIDOwner;
			}
			set
			{
				Global.Tracer.Assert(value == null || value is IDOwner, "((value != null) ? (value is IDOwner) : true)");
				m_parentIDOwner = (IDOwner)value;
			}
		}

		public virtual string UniqueName => InstancePathItem.GenerateUniqueNameString(ID, InstancePath);

		internal bool IsClone => m_isClone;

		protected IDOwner()
		{
		}

		protected IDOwner(int id)
		{
			m_ID = id;
		}

		protected virtual InstancePathItem CreateInstancePathItem()
		{
			return new InstancePathItem();
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context)
		{
			IDOwner obj = (IDOwner)MemberwiseClone();
			obj.m_ID = context.GenerateID();
			obj.m_isClone = true;
			return obj;
		}

		internal virtual void SetupCriRenderItemDef(ReportItem reportItem)
		{
			reportItem.m_parentIDOwner = m_parentIDOwner;
		}

		protected static IRIFReportDataScope FindReportDataScope(IInstancePath candidate)
		{
			IRIFReportDataScope iRIFReportDataScope = null;
			while (candidate != null && iRIFReportDataScope == null)
			{
				InstancePathItemType type = candidate.InstancePathItem.Type;
				if (type == InstancePathItemType.DataRegion || (uint)(type - 3) <= 3u)
				{
					iRIFReportDataScope = (IRIFReportDataScope)candidate;
				}
				candidate = candidate.ParentInstancePath;
			}
			return iRIFReportDataScope;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				if (writer.CurrentMember.MemberName == MemberName.ID)
				{
					writer.Write(m_ID);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				if (reader.CurrentMember.MemberName == MemberName.ID)
				{
					m_ID = reader.ReadInt32();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public virtual Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner;
		}
	}
}
