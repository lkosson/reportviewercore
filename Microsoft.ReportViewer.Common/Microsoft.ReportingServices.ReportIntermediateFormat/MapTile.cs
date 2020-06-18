using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapTile : IPersistable
	{
		private int m_exprHostID = -1;

		[NonSerialized]
		private MapTileExprHost m_exprHost;

		[Reference]
		private Map m_map;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private string m_name;

		private string m_tileData;

		private string m_mIMEType;

		internal string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		internal string TileData
		{
			get
			{
				return m_tileData;
			}
			set
			{
				m_tileData = value;
			}
		}

		internal string MIMEType
		{
			get
			{
				return m_mIMEType;
			}
			set
			{
				m_mIMEType = value;
			}
		}

		internal string OwnerName => m_map.Name;

		internal MapTileExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal MapTile()
		{
		}

		internal MapTile(Map map)
		{
			m_map = map;
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.MapTileStart(index.ToString(CultureInfo.InvariantCulture.NumberFormat));
			m_exprHostID = context.ExprHostBuilder.MapTileEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			MapTile obj = (MapTile)MemberwiseClone();
			obj.m_map = context.CurrentMapClone;
			return obj;
		}

		internal void SetExprHost(MapTileExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.TileData, Token.String));
			list.Add(new MemberInfo(MemberName.MIMEType, Token.String));
			list.Add(new MemberInfo(MemberName.Map, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapTile, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(m_map);
					break;
				case MemberName.Name:
					writer.Write(m_name);
					break;
				case MemberName.TileData:
					writer.Write(m_tileData);
					break;
				case MemberName.MIMEType:
					writer.Write(m_mIMEType);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
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
				case MemberName.Map:
					m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.TileData:
					m_tileData = reader.ReadString();
					break;
				case MemberName.MIMEType:
					m_mIMEType = reader.ReadString();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.Map)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_map = (Map)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapTile;
		}
	}
}
