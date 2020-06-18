using System.IO;
using System.Xml;

namespace Microsoft.ReportingServices.RdlObjectModel2005.Upgrade
{
	internal class Upgrader
	{
		public static void Upgrade(string inputFile, string outputFile, bool throwUpgradeException)
		{
			using (FileStream inStream = File.OpenRead(inputFile))
			{
				using (FileStream outStream = File.Open(outputFile, FileMode.Create, FileAccess.ReadWrite))
				{
					Upgrade(inStream, outStream, throwUpgradeException);
				}
			}
		}

		public static void Upgrade(Stream inStream, Stream outStream, bool throwUpgradeException)
		{
			Upgrade(new XmlTextReader(inStream)
			{
				ProhibitDtd = true
			}, outStream, throwUpgradeException);
		}

		public static void Upgrade(XmlReader xmlReader, Stream outStream, bool throwUpgradeException)
		{
			new UpgradeImpl2005(throwUpgradeException).Upgrade(xmlReader, outStream);
		}
	}
}
