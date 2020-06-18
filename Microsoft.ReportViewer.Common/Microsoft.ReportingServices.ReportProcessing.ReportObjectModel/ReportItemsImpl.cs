using System.Collections;
using System.Threading;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	internal sealed class ReportItemsImpl : ReportItems
	{
		private bool m_lockAdd;

		private Hashtable m_collection;

		private bool m_specialMode;

		private string m_specialModeIndex;

		internal const string Name = "ReportItems";

		internal const string FullName = "Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.ReportItems";

		public override ReportItem this[string key]
		{
			get
			{
				if (key == null || m_collection == null)
				{
					throw new ReportProcessingException_NonExistingReportItemReference(key);
				}
				try
				{
					if (m_specialMode)
					{
						m_specialModeIndex = key;
					}
					ReportItem reportItem = m_collection[key] as ReportItem;
					if (reportItem == null)
					{
						throw new ReportProcessingException_NonExistingReportItemReference(key);
					}
					return reportItem;
				}
				catch
				{
					throw new ReportProcessingException_NonExistingReportItemReference(key);
				}
			}
		}

		internal bool SpecialMode
		{
			set
			{
				m_specialMode = value;
			}
		}

		internal ReportItemsImpl()
			: this(lockAdd: false)
		{
		}

		internal ReportItemsImpl(bool lockAdd)
		{
			m_lockAdd = lockAdd;
			m_collection = new Hashtable();
			m_specialMode = false;
			m_specialModeIndex = null;
		}

		internal void Add(ReportItemImpl reportItem)
		{
			try
			{
				if (m_lockAdd)
				{
					Monitor.Enter(m_collection);
				}
				m_collection.Add(reportItem.Name, reportItem);
			}
			finally
			{
				if (m_lockAdd)
				{
					Monitor.Exit(m_collection);
				}
			}
		}

		internal string GetSpecialModeIndex()
		{
			string specialModeIndex = m_specialModeIndex;
			m_specialModeIndex = null;
			return specialModeIndex;
		}
	}
}
