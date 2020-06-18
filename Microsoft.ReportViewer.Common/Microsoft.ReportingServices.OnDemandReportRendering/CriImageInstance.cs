using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CriImageInstance : ImageInstance
	{
		private byte[] m_imageData;

		private string m_mimeType;

		private ActionInfoWithDynamicImageMapCollection m_actionInfoImageMapAreas;

		[NonSerialized]
		private string m_streamName;

		[NonSerialized]
		private bool m_mimeTypeEvaluated;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		public override byte[] ImageData
		{
			get
			{
				return m_imageData;
			}
			set
			{
				if (m_reportElementDef.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance)
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMDefinitionWriteback);
				}
				m_imageData = value;
			}
		}

		public override string StreamName
		{
			get
			{
				return m_streamName;
			}
			internal set
			{
				m_streamName = value;
			}
		}

		public override string MIMEType
		{
			get
			{
				if (!m_mimeTypeEvaluated)
				{
					m_mimeTypeEvaluated = true;
					if (base.ImageDef.ImageDef.MIMEType != null && !base.ImageDef.ImageDef.MIMEType.IsExpression)
					{
						m_mimeType = base.ImageDef.MIMEType.Value;
					}
				}
				return m_mimeType;
			}
			set
			{
				if (m_reportElementDef.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_reportElementDef.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !base.ImageDef.MIMEType.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				m_mimeTypeEvaluated = true;
				m_mimeType = value;
			}
		}

		public override TypeCode TagDataType => TypeCode.Empty;

		public override object Tag => null;

		internal override string ImageDataId => StreamName;

		public override ActionInfoWithDynamicImageMapCollection ActionInfoWithDynamicImageMapAreas
		{
			get
			{
				if (m_actionInfoImageMapAreas == null)
				{
					m_actionInfoImageMapAreas = new ActionInfoWithDynamicImageMapCollection();
				}
				return m_actionInfoImageMapAreas;
			}
		}

		internal override bool IsNullImage => false;

		internal CriImageInstance(Image reportItemDef)
			: base(reportItemDef)
		{
			Global.Tracer.Assert(m_reportElementDef.CriOwner != null, "Expected CRI Owner");
		}

		internal override List<string> GetFieldsUsedInValueExpression()
		{
			return null;
		}

		public override ActionInfoWithDynamicImageMap CreateActionInfoWithDynamicImageMap()
		{
			if (base.ReportElementDef.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance)
			{
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMDefinitionWriteback);
			}
			if (m_actionInfoImageMapAreas == null)
			{
				m_actionInfoImageMapAreas = new ActionInfoWithDynamicImageMapCollection();
			}
			return m_actionInfoImageMapAreas.Add(base.RenderingContext, base.ImageDef, base.ImageDef);
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_streamName = null;
			m_imageData = null;
			m_mimeTypeEvaluated = false;
			m_mimeType = null;
			m_actionInfoImageMapAreas = null;
		}

		internal override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ImageData:
					writer.Write(m_imageData);
					break;
				case MemberName.MIMEType:
				{
					string value = null;
					if (base.ImageDef.MIMEType != null && base.ImageDef.MIMEType.IsExpression)
					{
						value = m_mimeType;
					}
					writer.Write(value);
					break;
				}
				case MemberName.Actions:
				{
					ActionInstance[] array = null;
					if (base.ImageDef.ActionInfo != null)
					{
						array = new ActionInstance[base.ImageDef.ActionInfo.Actions.Count];
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = base.ImageDef.ActionInfo.Actions[i].Instance;
						}
					}
					writer.Write(array);
					break;
				}
				case MemberName.ImageMapAreas:
					writer.WriteRIFList(ActionInfoWithDynamicImageMapAreas.InternalList);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		internal override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ImageData:
					m_imageData = reader.ReadByteArray();
					break;
				case MemberName.MIMEType:
				{
					string text = reader.ReadString();
					if (base.ImageDef.MIMEType != null && base.ImageDef.MIMEType.IsExpression)
					{
						m_mimeTypeEvaluated = true;
						m_mimeType = text;
					}
					else
					{
						Global.Tracer.Assert(text == null, "(mimeType == null)");
					}
					break;
				}
				case MemberName.Actions:
					((ROMInstanceObjectCreator)reader.PersistenceHelper).StartActionInfoInstancesDeserialization(base.ImageDef.ActionInfo);
					reader.ReadArrayOfRIFObjects<ActionInstance>();
					((ROMInstanceObjectCreator)reader.PersistenceHelper).CompleteActionInfoInstancesDeserialization();
					break;
				case MemberName.ImageMapAreas:
					m_actionInfoImageMapAreas = new ActionInfoWithDynamicImageMapCollection();
					reader.ReadListOfRIFObjects(m_actionInfoImageMapAreas.InternalList);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageInstance;
		}

		[SkipMemberStaticValidation(MemberName.Actions)]
		private static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ImageData, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.MIMEType, Token.String));
			list.Add(new MemberInfo(MemberName.Actions, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionInstance));
			list.Add(new MemberInfo(MemberName.ImageMapAreas, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionInfoWithDynamicImageMap));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemInstance, list);
		}
	}
}
