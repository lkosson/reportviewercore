namespace Microsoft.Cloud.Platform.Utils
{
	internal interface IContainsPrivateInformation
	{
		string ToPrivateString();

		string ToInternalString();

		string ToOriginalString();
	}
}
