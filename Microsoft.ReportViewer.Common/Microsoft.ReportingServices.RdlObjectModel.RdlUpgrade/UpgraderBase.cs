using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.IO;
using System.Xml;

namespace Microsoft.ReportingServices.RdlObjectModel.RdlUpgrade
{
	internal abstract class UpgraderBase
	{
		protected RDLUpgradeResult m_upgradeResults;

		internal RDLUpgradeResult UpgradeResults => m_upgradeResults;

		internal UpgraderBase()
		{
		}

		internal abstract Type GetReportType();

		public void Upgrade(XmlReader xmlReader, Stream outStream)
		{
			InitUpgrade();
			RdlSerializerSettings settings = CreateReaderSettings();
			SetupReaderSettings(settings);
			Report report = (Report)new RdlSerializer(settings).Deserialize(xmlReader, GetReportType());
			Upgrade(report);
			new RdlSerializer(CreateWriterSettings()).Serialize(outStream, report);
		}

		protected virtual void InitUpgrade()
		{
			m_upgradeResults = new RDLUpgradeResult();
		}

		protected virtual void Upgrade(Report report)
		{
		}

		protected abstract RdlSerializerSettings CreateReaderSettings();

		protected virtual void SetupReaderSettings(RdlSerializerSettings settings)
		{
		}

		protected abstract RdlSerializerSettings CreateWriterSettings();
	}
}
