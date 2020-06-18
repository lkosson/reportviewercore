using Microsoft.ReportingServices.Diagnostics;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.Reporting
{
	[Serializable]
	internal class PreviewItemContext : CatalogItemContextBase<string>
	{
		private sealed class ControlRequestParameters : RSRequestParameters
		{
			public ControlRequestParameters()
			{
				SetCatalogParameters(null);
				base.PaginationModeValue = "Estimate";
			}

			protected override void ApplyDefaultRenderingParameters()
			{
			}
		}

		private DefinitionSource m_definitionSource;

		private string m_previewStorePath;

		private Assembly m_embeddedResourceAssembly;

		public override string ItemPathAsString => base.ItemPath;

		public DefinitionSource DefinitionSource => m_definitionSource;

		public string PreviewStorePath => m_previewStorePath;

		protected virtual string RdlExtension => ".rdlc";

		public override string HostRootUri => null;

		public Assembly EmbeddedResourceAssembly => m_embeddedResourceAssembly;

		public PreviewItemContext()
		{
			m_primaryContext = this;
		}

		public void SetPath(string pathForFileDefinitionSource, string fullyQualifiedPath, DefinitionSource definitionSource, Assembly embeddedResourceAssembly)
		{
			m_definitionSource = definitionSource;
			if (m_definitionSource == DefinitionSource.EmbeddedResource)
			{
				m_embeddedResourceAssembly = embeddedResourceAssembly;
			}
			if (definitionSource == DefinitionSource.File)
			{
				SetPath(pathForFileDefinitionSource, ItemPathOptions.None);
			}
			else
			{
				SetPath(string.Empty, ItemPathOptions.None);
			}
			m_previewStorePath = fullyQualifiedPath;
			m_ItemName = CalculateDisplayName(fullyQualifiedPath, definitionSource);
		}

		public override bool SetPath(string path, ItemPathOptions pathOptions)
		{
			m_ItemPath = path;
			m_OriginalItemPath = path;
			return true;
		}

		protected override CatalogItemContextBase<string> CreateContext(IPathTranslator pathTranslator)
		{
			return new PreviewItemContext();
		}

		public override ICatalogItemContext GetSubreportContext(string subreportPath)
		{
			PreviewItemContext obj = (PreviewItemContext)base.GetSubreportContext(subreportPath);
			string text = MapUserProvidedPath(subreportPath);
			obj.SetPath(text, text, DefinitionSource, m_embeddedResourceAssembly);
			obj.OriginalItemPath = subreportPath;
			return obj;
		}

		public virtual ICatalogItemContext GetDataSetContext(string dataSetPath)
		{
			PreviewItemContext previewItemContext = new PreviewItemContext();
			previewItemContext.SetPath(dataSetPath, ItemPathOptions.None);
			return previewItemContext;
		}

		public override string MapUserProvidedPath(string path)
		{
			switch (m_definitionSource)
			{
			case DefinitionSource.File:
				if (Path.IsPathRooted(path))
				{
					return path;
				}
				return Path.Combine(Path.GetDirectoryName(PreviewStorePath), path) + RdlExtension;
			case DefinitionSource.EmbeddedResource:
				return FindEmbeddedResource(path);
			default:
				return path;
			}
		}

		public override bool IsReportServerPathOrUrl(string pathOrUrl, bool checkProtocol, out bool isRelative)
		{
			isRelative = false;
			return false;
		}

		public override bool IsSupportedProtocol(string path, bool protocolRestriction)
		{
			bool isRelative;
			return IsSupportedProtocol(path, protocolRestriction, out isRelative);
		}

		public override bool IsSupportedProtocol(string path, bool protocolRestriction, out bool isRelative)
		{
			return ((IPathManager)RSPathUtil.Instance).IsSupportedUrl(path, protocolRestriction, out isRelative);
		}

		private string FindEmbeddedResource(string path)
		{
			if (m_embeddedResourceAssembly == null)
			{
				return path;
			}
			string[] manifestResourceNames = m_embeddedResourceAssembly.GetManifestResourceNames();
			StringDictionary stringDictionary = new StringDictionary();
			string[] array = manifestResourceNames;
			foreach (string key in array)
			{
				stringDictionary.Add(key, null);
			}
			if (stringDictionary.ContainsKey(path))
			{
				return path;
			}
			if (PreviewStorePath != null)
			{
				string text = PreviewStorePath;
				while (text.Length > 0)
				{
					int num = text.LastIndexOf('.');
					if (num == -1)
					{
						num = 0;
					}
					text = text.Substring(0, num);
					string text2 = text;
					if (text2.Length > 0)
					{
						text2 += ".";
					}
					string text3 = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", text2, path, RdlExtension);
					if (stringDictionary.ContainsKey(text3))
					{
						return text3;
					}
				}
			}
			return path;
		}

		private static string CalculateDisplayName(string fullyQualifiedPath, DefinitionSource definitionSource)
		{
			switch (definitionSource)
			{
			case DefinitionSource.File:
				return Path.GetFileNameWithoutExtension(fullyQualifiedPath);
			case DefinitionSource.EmbeddedResource:
			{
				string[] array = fullyQualifiedPath.Split('.');
				if (array.Length >= 2)
				{
					return array[array.Length - 2];
				}
				return array[0];
			}
			case DefinitionSource.Direct:
				return fullyQualifiedPath;
			default:
				return string.Empty;
			}
		}

		protected override RSRequestParameters CreateRequestParametersInstance()
		{
			return new ControlRequestParameters();
		}
	}
}
