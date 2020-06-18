using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class ComponentLibraryUpgrader
	{
		private class ReportPartsUpgrader
		{
			private bool m_needsUpgrade;

			private string m_oldNamespace = string.Empty;

			private string m_oldNsPrefix = string.Empty;

			private XmlDocument m_document;

			private string[] m_knowNamespaces = new string[2]
			{
				"http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition",
				"http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition"
			};

			private const string NamespaceURI201001 = "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition";

			private const string NamespaceURI201601 = "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition";

			private bool NeedsUpgrade => m_needsUpgrade;

			private string OldRdlNamespace => m_oldNamespace;

			private string OldRdlNSPrefix => m_oldNsPrefix;

			internal Stream UpgradeToCurrent(Stream stream, ref Stream outStream)
			{
				XmlElement documentElement = LoadDefinition(stream).DocumentElement;
				ScanRdlNamespace(documentElement);
				if (NeedsUpgrade)
				{
					string oldRdlNamespace = OldRdlNamespace;
					if (!(oldRdlNamespace == "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition"))
					{
						if (!(oldRdlNamespace == "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition"))
						{
							throw new RDLUpgradeException(RDLUpgradeStrings.rdlInvalidTargetNamespace(OldRdlNamespace));
						}
					}
					else
					{
						UpdateNamespaceURI("http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition", "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition");
					}
				}
				return SaveDefinition(ref outStream);
			}

			private void ScanRdlNamespace(XmlElement root)
			{
				string empty = string.Empty;
				string[] knowNamespaces = m_knowNamespaces;
				int num = 0;
				string text;
				while (true)
				{
					if (num < knowNamespaces.Length)
					{
						text = knowNamespaces[num];
						empty = root.GetPrefixOfNamespace(text);
						if (!string.IsNullOrEmpty(empty))
						{
							break;
						}
						num++;
						continue;
					}
					return;
				}
				m_oldNamespace = text;
				m_needsUpgrade = true;
				m_oldNsPrefix = empty;
			}

			private XmlDocument LoadDefinition(Stream stream)
			{
				m_document = new XmlDocument();
				m_document.PreserveWhitespace = true;
				m_document.Load(stream);
				return m_document;
			}

			private Stream SaveDefinition(ref Stream outStream)
			{
				m_document.Save(outStream);
				m_document = null;
				outStream.Seek(0L, SeekOrigin.Begin);
				return outStream;
			}

			private void UpdateNamespaceURI(string oldNamespaceURI, string newNamespaceURI)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(m_document.OuterXml);
				stringBuilder.Replace(oldNamespaceURI, newNamespaceURI);
				m_document.LoadXml(stringBuilder.ToString());
			}
		}

		internal static Stream UpgradeToCurrent(Stream stream, ref Stream outStream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			return new ReportPartsUpgrader().UpgradeToCurrent(stream, ref outStream);
		}

		internal static byte[] UpgradeToCurrent(byte[] definition)
		{
			byte[] array = null;
			MemoryStream stream = new MemoryStream(definition);
			Stream outStream = new MemoryStream();
			UpgradeToCurrent(stream, ref outStream);
			array = new byte[outStream.Length];
			outStream.Position = 0L;
			outStream.Read(array, 0, (int)outStream.Length);
			outStream.Close();
			return array;
		}
	}
}
