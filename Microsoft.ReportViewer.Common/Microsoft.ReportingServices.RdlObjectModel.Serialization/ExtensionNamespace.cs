namespace Microsoft.ReportingServices.RdlObjectModel.Serialization
{
	internal sealed class ExtensionNamespace
	{
		public string LocalName
		{
			get;
			private set;
		}

		public string Namespace
		{
			get;
			private set;
		}

		public bool MustUnderstand
		{
			get;
			private set;
		}

		public ExtensionNamespace(string localName, string xmlNamespace, bool mustUnderstand = false)
		{
			LocalName = localName;
			Namespace = xmlNamespace;
			MustUnderstand = mustUnderstand;
		}
	}
}
