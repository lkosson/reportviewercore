using Microsoft.ReportingServices.RdlObjectModel;
using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class ThreeDProperties2005 : ChartThreeDProperties
	{
		internal new class Definition : DefinitionStore<ThreeDProperties2005, Definition.Properties>
		{
			public enum Properties
			{
				DrawingStyle = 10,
				ProjectionMode,
				Rotation,
				Inclination,
				WallThickness
			}

			private Definition()
			{
			}
		}

		[DefaultValue(DrawingStyleTypes2005.Cube)]
		public DrawingStyleTypes2005 DrawingStyle
		{
			get
			{
				return (DrawingStyleTypes2005)base.PropertyStore.GetInteger(10);
			}
			set
			{
				base.PropertyStore.SetInteger(10, (int)value);
			}
		}

		[DefaultValue(ProjectionModes2005.Perspective)]
		public new ProjectionModes2005 ProjectionMode
		{
			get
			{
				return (ProjectionModes2005)base.PropertyStore.GetInteger(11);
			}
			set
			{
				base.PropertyStore.SetInteger(11, (int)value);
			}
		}

		[DefaultValue(0)]
		public new int Rotation
		{
			get
			{
				return base.PropertyStore.GetInteger(12);
			}
			set
			{
				base.PropertyStore.SetInteger(12, value);
			}
		}

		[DefaultValue(0)]
		public new int Inclination
		{
			get
			{
				return base.PropertyStore.GetInteger(13);
			}
			set
			{
				base.PropertyStore.SetInteger(13, value);
			}
		}

		[DefaultValue(0)]
		public new int WallThickness
		{
			get
			{
				return base.PropertyStore.GetInteger(14);
			}
			set
			{
				base.PropertyStore.SetInteger(14, value);
			}
		}

		public ThreeDProperties2005()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ProjectionMode = ProjectionModes2005.Perspective;
			base.Shading = ChartShadings.None;
		}

		public ThreeDProperties2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
