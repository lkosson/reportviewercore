using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class OnDemandDocumentMapNode
	{
		private string m_label;

		private string m_id;

		private int m_level = -1;

		public string Label => m_label;

		public string Id => m_id;

		public int Level
		{
			get
			{
				return m_level;
			}
			internal set
			{
				m_level = value;
			}
		}

		internal OnDemandDocumentMapNode(string aLabel, string aId, int aLevel)
		{
			m_label = aLabel;
			m_id = aId;
			m_level = aLevel;
		}
	}
}
