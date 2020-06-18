using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Coverflow : Navigation
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private NavigationItem m_navigationItem;

		private Slider m_slider;

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

		internal NavigationItem NavigationItem
		{
			get
			{
				return m_navigationItem;
			}
			set
			{
				m_navigationItem = value;
			}
		}

		internal override void Initialize(Tablix tablix, InitializationContext context)
		{
			if (m_slider != null)
			{
				m_slider.Initialize(tablix, context);
			}
			if (m_navigationItem != null)
			{
				m_navigationItem.Initialize(tablix, context, "Coverflow");
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.NavigationItem, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.NavigationItem));
			list.Add(new MemberInfo(MemberName.Slider, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Slider));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Coverflow, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Navigation, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.NavigationItem:
					writer.Write(m_navigationItem);
					break;
				case MemberName.Slider:
					writer.Write(m_slider);
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
				case MemberName.NavigationItem:
					m_navigationItem = reader.ReadRIFObject<NavigationItem>();
					break;
				case MemberName.Slider:
					m_slider = reader.ReadRIFObject<Slider>();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Coverflow;
		}
	}
}
