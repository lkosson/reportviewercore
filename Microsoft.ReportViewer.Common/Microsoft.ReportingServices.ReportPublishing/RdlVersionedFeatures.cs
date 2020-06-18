using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class RdlVersionedFeatures
	{
		private sealed class FeatureDescriptor
		{
			private readonly int m_addedInCompatVersion;

			private readonly RenderMode m_allowedRenderModes;

			public int AddedInCompatVersion => m_addedInCompatVersion;

			public RenderMode AllowedRenderModes => m_allowedRenderModes;

			public FeatureDescriptor(int addedInCompatVersion, RenderMode allowedRenderModes)
			{
				m_addedInCompatVersion = addedInCompatVersion;
				m_allowedRenderModes = allowedRenderModes;
			}
		}

		private readonly FeatureDescriptor[] m_rdlFeatureVersioningStructure;

		public RdlVersionedFeatures()
		{
			int length = Enum.GetValues(typeof(RdlFeatures)).Length;
			m_rdlFeatureVersioningStructure = new FeatureDescriptor[length];
		}

		internal void Add(RdlFeatures featureType, int addedInCompatVersion, RenderMode allowedRenderModes)
		{
			m_rdlFeatureVersioningStructure[(int)featureType] = new FeatureDescriptor(addedInCompatVersion, allowedRenderModes);
		}

		internal bool IsRdlFeatureAllowed(RdlFeatures feature, int configVersion, RenderMode renderMode)
		{
			FeatureDescriptor featureDescriptor = m_rdlFeatureVersioningStructure[(int)feature];
			bool num = configVersion == 0 || featureDescriptor.AddedInCompatVersion <= configVersion;
			bool flag = (featureDescriptor.AllowedRenderModes & renderMode) == renderMode;
			return num && flag;
		}

		internal void VerifyAllFeaturesAreAdded()
		{
		}

		[Conditional("DEBUG")]
		private void VerifyAllFeaturesAreAdded(FeatureDescriptor[] rdlFeatureVersioningStructure)
		{
			for (int i = 0; i < rdlFeatureVersioningStructure.Length; i++)
			{
				Global.Tracer.Assert(rdlFeatureVersioningStructure[i] != null, "Missing RDL feature for: {0}", (RdlFeatures)i);
			}
		}
	}
}
