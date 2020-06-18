using Microsoft.ReportingServices.ReportProcessing;
using System.IO;
using System.Xml;

namespace Microsoft.ReportingServices.RdlObjectModel2008.Upgrade
{
	internal class Upgrader
	{
		internal static void Upgrade(string inputFile, string outputFile, bool throwUpgradeException)
		{
			using (FileStream inStream = File.OpenRead(inputFile))
			{
				using (FileStream outStream = File.Open(outputFile, FileMode.Create, FileAccess.ReadWrite))
				{
					Upgrade(inStream, outStream, throwUpgradeException);
				}
			}
		}

		internal static void Upgrade(Stream inStream, Stream outStream, bool throwUpgradeException)
		{
			Upgrade(new XmlTextReader(inStream)
			{
				ProhibitDtd = true
			}, outStream, throwUpgradeException);
		}

		internal static void Upgrade(XmlReader xmlReader, Stream outStream, bool throwUpgradeException)
		{
			new UpgradeImpl2008().Upgrade(xmlReader, outStream);
		}

		internal static Stream UpgradeToCurrent(Stream inStream, bool throwUpgradeException)
		{
			return RDLUpgrader.UpgradeToCurrent(inStream, throwUpgradeException, renameInvalidDataSources: true);
		}
	}
}
