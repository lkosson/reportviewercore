using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class PlayAxis : Navigation
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private Slider m_slider;

		private DockingOption m_dockingOption;

		internal Slider Slider
		{
			get
			{
				return m_slider;
			}
			set
			{
				m_slider = value;
			}
		}

		internal DockingOption DockingOption
		{
			get
			{
				return m_dockingOption;
			}
			set
			{
				m_dockingOption = value;
			}
		}

		internal override void Initialize(Tablix tablix, InitializationContext context)
		{
			if (m_slider != null)
			{
				m_slider.Initialize(tablix, context);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Slider, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Slider));
			list.Add(new MemberInfo(MemberName.DockingOption, Token.Enum));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PlayAxis, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Navigation, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Slider:
					writer.Write(m_slider);
					break;
				case MemberName.DockingOption:
					writer.WriteEnum((int)m_dockingOption);
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
				case MemberName.Slider:
					m_slider = reader.ReadRIFObject<Slider>();
					break;
				case MemberName.DockingOption:
					m_dockingOption = (DockingOption)reader.ReadEnum();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PlayAxis;
		}
	}
}
