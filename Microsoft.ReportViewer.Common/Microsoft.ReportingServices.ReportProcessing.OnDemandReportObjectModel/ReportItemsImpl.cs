using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections;
using System.Threading;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class ReportItemsImpl : ReportItems
	{
		private bool m_lockAdd;

		private Hashtable m_collection;

		private bool m_specialMode;

		private string m_specialModeIndex;

		public override Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.ReportItem this[string key]
		{
			get
			{
				if (key == null || m_collection == null)
				{
					throw new ReportProcessingException_NonExistingReportItemReference(key);
				}
				if (m_specialMode)
				{
					m_specialModeIndex = key;
				}
				Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.ReportItem reportItem = m_collection[key] as Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.ReportItem;
				if (reportItem == null)
				{
					throw new ReportProcessingException_NonExistingReportItemReference(key);
				}
				return reportItem;
			}
		}

		internal bool SpecialMode
		{
			set
			{
				m_specialMode = value;
			}
		}

		internal ReportItemsImpl(bool lockAdd)
		{
			m_lockAdd = lockAdd;
			m_collection = new Hashtable();
			m_specialMode = false;
			m_specialModeIndex = null;
		}

		internal void ResetAll()
		{
			foreach (ReportItemImpl value in m_collection.Values)
			{
				value.Reset();
			}
		}

		internal void ResetAll(Microsoft.ReportingServices.RdlExpressions.VariantResult aResult)
		{
			foreach (ReportItemImpl value in m_collection.Values)
			{
				value.Reset(aResult);
			}
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

		internal void AddAll(ReportItemsImpl reportItems)
		{
			foreach (ReportItemImpl value in reportItems.m_collection.Values)
			{
				Add(value);
			}
		}

		internal Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.ReportItem GetReportItem(string aName)
		{
			return m_collection[aName] as Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.ReportItem;
		}

		internal string GetSpecialModeIndex()
		{
			string specialModeIndex = m_specialModeIndex;
			m_specialModeIndex = null;
			return specialModeIndex;
		}
	}
}
