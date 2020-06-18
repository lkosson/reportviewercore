using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[CompilerGenerated]
	internal class RDLValidatingReaderStrings
	{
		[CompilerGenerated]
		public class Keys
		{
			private static ResourceManager resourceManager = new ResourceManager(typeof(RDLValidatingReaderStrings).FullName, typeof(RDLValidatingReaderStrings).Module.Assembly);

			private static CultureInfo _culture = null;

			public const string rdlValidationMissingChildElement = "rdlValidationMissingChildElement";

			public const string rdlValidationInvalidElement = "rdlValidationInvalidElement";

			public const string rdlValidationInvalidParent = "rdlValidationInvalidParent";

			public const string rdlValidationNoElementDecl = "rdlValidationNoElementDecl";

			public const string rdlValidationInvalidNamespaceElement = "rdlValidationInvalidNamespaceElement";

			public const string rdlValidationInvalidNamespaceAttribute = "rdlValidationInvalidNamespaceAttribute";

			public const string rdlValidationInvalidMicroVersionedElement = "rdlValidationInvalidMicroVersionedElement";

			public const string rdlValidationInvalidMicroVersionedAttribute = "rdlValidationInvalidMicroVersionedAttribute";

			public const string rdlValidationUnsupportedSchema = "rdlValidationUnsupportedSchema";

			public const string rdlValidationUndefinedSchemaNamespace = "rdlValidationUndefinedSchemaNamespace";

			public const string rdlValidationMultipleUndefinedSchemaNamespaces = "rdlValidationMultipleUndefinedSchemaNamespaces";

			public const string rdlValidationUnknownRequiredNamespaces = "rdlValidationUnknownRequiredNamespaces";

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

			public static string GetString(string key, object arg0, object arg1, object arg2, object arg3)
			{
				return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), arg0, arg1, arg2, arg3);
			}

			public static string GetString(string key, object arg0, object arg1, object arg2, object arg3, object arg4)
			{
				return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), arg0, arg1, arg2, arg3, arg4);
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

		protected RDLValidatingReaderStrings()
		{
		}

		public static string rdlValidationMissingChildElement(string parentType, string childType, string linenumber, string position)
		{
			return Keys.GetString("rdlValidationMissingChildElement", parentType, childType, linenumber, position);
		}

		public static string rdlValidationInvalidElement(string parentType, string childType, string linenumber, string position)
		{
			return Keys.GetString("rdlValidationInvalidElement", parentType, childType, linenumber, position);
		}

		public static string rdlValidationInvalidParent(string childType, string childNs, string parentType, string linenumber, string position)
		{
			return Keys.GetString("rdlValidationInvalidParent", childType, childNs, parentType, linenumber, position);
		}

		public static string rdlValidationNoElementDecl(string elementExpandedName, string element, string elementNs, string linenumber)
		{
			return Keys.GetString("rdlValidationNoElementDecl", elementExpandedName, element, elementNs, linenumber);
		}

		public static string rdlValidationInvalidNamespaceElement(string elementExpandedName, string nameSpace, string linenumber, string position)
		{
			return Keys.GetString("rdlValidationInvalidNamespaceElement", elementExpandedName, nameSpace, linenumber, position);
		}

		public static string rdlValidationInvalidNamespaceAttribute(string attributeExpandedName, string nameSpace, string linenumber, string position)
		{
			return Keys.GetString("rdlValidationInvalidNamespaceAttribute", attributeExpandedName, nameSpace, linenumber, position);
		}

		public static string rdlValidationInvalidMicroVersionedElement(string elementName, string parentName, string linenumber, string position)
		{
			return Keys.GetString("rdlValidationInvalidMicroVersionedElement", elementName, parentName, linenumber, position);
		}

		public static string rdlValidationInvalidMicroVersionedAttribute(string attributeName, string parentName, string linenumber, string position)
		{
			return Keys.GetString("rdlValidationInvalidMicroVersionedAttribute", attributeName, parentName, linenumber, position);
		}

		public static string rdlValidationUnsupportedSchema(string objectType, string objectName, string linenumber, string position)
		{
			return Keys.GetString("rdlValidationUnsupportedSchema", objectType, objectName, linenumber, position);
		}

		public static string rdlValidationUndefinedSchemaNamespace(string objectType, string objectName, string linenumber, string position)
		{
			return Keys.GetString("rdlValidationUndefinedSchemaNamespace", objectType, objectName, linenumber, position);
		}

		public static string rdlValidationMultipleUndefinedSchemaNamespaces(string objectType, string objectName, string linenumber, string position)
		{
			return Keys.GetString("rdlValidationMultipleUndefinedSchemaNamespaces", objectType, objectName, linenumber, position);
		}

		public static string rdlValidationUnknownRequiredNamespaces(string xmlns, string prefix, string sqlServerVersionName, string linenumber, string position)
		{
			return Keys.GetString("rdlValidationUnknownRequiredNamespaces", xmlns, prefix, sqlServerVersionName, linenumber, position);
		}
	}
}
