using System;

namespace Microsoft.Cloud.Platform.Utils
{
	internal static class PrivateInformation
	{
		private static readonly Type c_iContainsPrivateInformationType = typeof(IContainsPrivateInformation);

		public static bool ContainsPrivateInformation(Type type)
		{
			return c_iContainsPrivateInformationType.IsAssignableFrom(type);
		}

		public static string MarkAsUserContent(this string userContent)
		{
			return userContent;
		}

		public static string MarkAsModelInfo(this string modelInfo)
		{
			return modelInfo;
		}

		public static string MarkAsUtterance(this string utterance)
		{
			return utterance;
		}

		public static string MarkAsPrivate(this string plainString)
		{
			return plainString;
		}

		public static string MarkAsPrivate(this IContainsPrivateInformation pi)
		{
			return pi?.ToOriginalString();
		}

		public static string MarkIfPrivate(this object o)
		{
			if (o == null)
			{
				return null;
			}
			IContainsPrivateInformation containsPrivateInformation = o as IContainsPrivateInformation;
			if (containsPrivateInformation == null)
			{
				return o.ToString();
			}
			return containsPrivateInformation.ToOriginalString();
		}

		public static string MarkAsTenantId(this string tenantId)
		{
			return tenantId;
		}

		public static string MarkAsOrgId(this string orgId)
		{
			return orgId;
		}

		public static string MarkAsInternal(this string plainString)
		{
			return plainString;
		}

		public static string MarkAsInternal(this IContainsPrivateInformation pi)
		{
			return pi?.ToOriginalString();
		}

		public static string MarkIfInternal(this object o)
		{
			if (o == null)
			{
				return null;
			}
			IContainsPrivateInformation containsPrivateInformation = o as IContainsPrivateInformation;
			if (containsPrivateInformation == null)
			{
				return o.ToString();
			}
			return containsPrivateInformation.ToOriginalString();
		}

		public static string ScrubPII(this string plainString)
		{
			return plainString;
		}

		public static string ScrubPII(this IContainsPrivateInformation pi)
		{
			if (pi == null)
			{
				return string.Empty;
			}
			return pi.ToPrivateString();
		}

		public static string ScrubIfPII(this object o)
		{
			if (o == null)
			{
				return null;
			}
			IContainsPrivateInformation containsPrivateInformation = o as IContainsPrivateInformation;
			if (containsPrivateInformation == null)
			{
				return o.ToString();
			}
			return containsPrivateInformation.ToOriginalString();
		}

		public static string TagAsPII(this string plainString)
		{
			return plainString;
		}

		public static string TagAsPII(this IContainsPrivateInformation pi)
		{
			return pi?.ToOriginalString();
		}

		public static string TagIfPII(this object o)
		{
			if (o == null)
			{
				return null;
			}
			IContainsPrivateInformation containsPrivateInformation = o as IContainsPrivateInformation;
			if (containsPrivateInformation == null)
			{
				return o.ToString();
			}
			return containsPrivateInformation.ToOriginalString();
		}

		public static string UntagPII(this string taggedString)
		{
			return taggedString;
		}

		public static string RemovePrivateAndInternalMarkup(this string markedupString)
		{
			return markedupString;
		}
	}
}
