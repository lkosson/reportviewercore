using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ImageMapAreaInstance : IPersistable
	{
		private ImageMapArea.ImageMapAreaShape m_shape;

		private float[] m_coordinates;

		private string m_toolTip;

		private static readonly Declaration m_Declaration = GetDeclaration();

		public ImageMapArea.ImageMapAreaShape Shape => m_shape;

		public float[] Coordinates => m_coordinates;

		public string ToolTip => m_toolTip;

		internal ImageMapAreaInstance(ImageMapArea.ImageMapAreaShape shape, float[] coordinates)
			: this(shape, coordinates, null)
		{
		}

		internal ImageMapAreaInstance(ImageMapArea.ImageMapAreaShape shape, float[] coordinates, string toolTip)
		{
			m_shape = shape;
			m_coordinates = coordinates;
			m_toolTip = toolTip;
		}

		internal ImageMapAreaInstance()
		{
		}

		internal ImageMapAreaInstance(Microsoft.ReportingServices.ReportRendering.ImageMapArea renderImageMapArea)
		{
			m_shape = (ImageMapArea.ImageMapAreaShape)renderImageMapArea.Shape;
			m_coordinates = renderImageMapArea.Coordinates;
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Shape:
					writer.WriteEnum((int)m_shape);
					break;
				case MemberName.Coordinates:
					writer.Write(m_coordinates);
					break;
				case MemberName.ToolTip:
					writer.Write(m_toolTip);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Shape:
					m_shape = (ImageMapArea.ImageMapAreaShape)reader.ReadEnum();
					break;
				case MemberName.Coordinates:
					m_coordinates = reader.ReadSingleArray();
					break;
				case MemberName.ToolTip:
					m_toolTip = reader.ReadString();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageMapAreaInstance;
		}

		private static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Shape, Token.Enum));
			list.Add(new MemberInfo(MemberName.Coordinates, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Single));
			list.Add(new MemberInfo(MemberName.ToolTip, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageMapAreaInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}
	}
}
