using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using Microsoft.ReportingServices.RdlObjectModel2010.Upgrade;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlObjectModel2008.Upgrade
{
	internal class UpgradeImpl2008 : UpgradeImpl2010
	{
		private List<IUpgradeable2008> m_upgradeable;

		internal UpgradeImpl2008()
		{
		}

		internal override Type GetReportType()
		{
			return typeof(Report2008);
		}

		protected override void InitUpgrade()
		{
			m_upgradeable = new List<IUpgradeable2008>();
			base.InitUpgrade();
		}

		protected override void Upgrade(Report report)
		{
			foreach (IUpgradeable2008 item in m_upgradeable)
			{
				item.Upgrade(this);
			}
			base.Upgrade(report);
		}

		protected override RdlSerializerSettings CreateReaderSettings()
		{
			return UpgradeSerializerSettings2008.CreateReaderSettings();
		}

		protected override RdlSerializerSettings CreateWriterSettings()
		{
			return UpgradeSerializerSettings2008.CreateWriterSettings();
		}

		protected override void SetupReaderSettings(RdlSerializerSettings settings)
		{
			((SerializerHost2008)settings.Host).Upgradeable2008 = m_upgradeable;
			base.SetupReaderSettings(settings);
		}

		internal void UpgradeReport(Report2008 report)
		{
			ReportSection reportSection = new ReportSection();
			reportSection.Body = report.Body;
			reportSection.Page = report.Page;
			reportSection.Width = report.Width;
			report.ReportSections = new List<ReportSection>(1);
			report.ReportSections.Add(reportSection);
			report.Body = null;
			report.Page = null;
			report.Width = ReportSize.Empty;
		}
	}
}
