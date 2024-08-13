using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

[CompilerGenerated]
internal class StringResources
{
	[CompilerGenerated]
	public class Keys
	{
		public const string LocalizedXmlRendererName = "LocalizedXmlRendererName";

		public const string LocalizedCsvRendererName = "LocalizedCsvRendererName";

		public const string LocalizedAtomDataRendererName = "LocalizedAtomDataRendererName";

		public const string rrAttrNameCollision = "rrAttrNameCollision";

		public const string rrElementNameCollision = "rrElementNameCollision";

		public const string rrCanNotLoadXSLT = "rrCanNotLoadXSLT";

		public const string rrBadXsltPath = "rrBadXsltPath";

		public const string rrUnknownCardinality = "rrUnknownCardinality";

		public const string rrBadXsltTransformation = "rrBadXsltTransformation";

		public const string rrSameDelimiterError = "rrSameDelimiterError";

		public const string rrSameQualiferDelimiterError = "rrSameQualiferDelimiterError";

		public const string rrInvalidQualifier = "rrInvalidQualifier";

		public const string rrExpectedTopLevelElement = "rrExpectedTopLevelElement";

		public const string rrParameterExpected = "rrParameterExpected";

		public const string rrExpectedEndElement = "rrExpectedEndElement";

		public const string rrInvalidParamValue = "rrInvalidParamValue";

		public const string rrRenderSubreportError = "rrRenderSubreportError";

		public const string rrDataFeedNotFound = "rrDataFeedNotFound";

		public const string rrDuplicatedBatchItem = "rrDuplicatedBatchItem";

		private static ResourceManager resourceManager = new ResourceManager(typeof(StringResources).FullName, typeof(StringResources).Module.Assembly);

		private static CultureInfo _culture = null;

		public static CultureInfo Culture
		{
			get
			{
				return _culture;
			}
			set
			{
				_culture = value;
			}
		}

		private Keys()
		{
		}

		public static string GetString(string key)
		{
			return resourceManager.GetString(key, _culture);
		}

		public static string GetString(string key, object arg0)
		{
			return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), arg0);
		}
	}

	public static CultureInfo Culture
	{
		get
		{
			return Keys.Culture;
		}
		set
		{
			Keys.Culture = value;
		}
	}

	public static string LocalizedXmlRendererName => Keys.GetString("LocalizedXmlRendererName");

	public static string LocalizedCsvRendererName => Keys.GetString("LocalizedCsvRendererName");

	public static string LocalizedAtomDataRendererName => Keys.GetString("LocalizedAtomDataRendererName");

	public static string rrBadXsltPath => Keys.GetString("rrBadXsltPath");

	public static string rrUnknownCardinality => Keys.GetString("rrUnknownCardinality");

	public static string rrBadXsltTransformation => Keys.GetString("rrBadXsltTransformation");

	public static string rrSameDelimiterError => Keys.GetString("rrSameDelimiterError");

	public static string rrSameQualiferDelimiterError => Keys.GetString("rrSameQualiferDelimiterError");

	public static string rrInvalidQualifier => Keys.GetString("rrInvalidQualifier");

	public static string rrParameterExpected => Keys.GetString("rrParameterExpected");

	public static string rrRenderSubreportError => Keys.GetString("rrRenderSubreportError");

	public static string rrDataFeedNotFound => Keys.GetString("rrDataFeedNotFound");

	public static string rrDuplicatedBatchItem => Keys.GetString("rrDuplicatedBatchItem");

	protected StringResources()
	{
	}

	public static string rrAttrNameCollision(string name)
	{
		return Keys.GetString("rrAttrNameCollision", name);
	}

	public static string rrElementNameCollision(string name)
	{
		return Keys.GetString("rrElementNameCollision", name);
	}

	public static string rrCanNotLoadXSLT(string path)
	{
		return Keys.GetString("rrCanNotLoadXSLT", path);
	}

	public static string rrExpectedTopLevelElement(string elementName)
	{
		return Keys.GetString("rrExpectedTopLevelElement", elementName);
	}

	public static string rrExpectedEndElement(string elementName)
	{
		return Keys.GetString("rrExpectedEndElement", elementName);
	}

	public static string rrInvalidParamValue(string paramName)
	{
		return Keys.GetString("rrInvalidParamValue", paramName);
	}
}
