using System;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	[StrongNameIdentityPermission(SecurityAction.InheritanceDemand, PublicKey = "0024000004800000940000000602000000240000525341310004000001000100272736ad6e5f9586bac2d531eabc3acc666c2f8ec879fa94f8f7b0327d2ff2ed523448f83c3d5c5dd2dfc7bc99c5286b2c125117bf5cbe242b9d41750732b2bdffe649c6efb8e5526d526fdd130095ecdb7bf210809c6cdad8824faa9ac0310ac3cba2aa0523567b2dfa7fe250b30facbd62d4ec99b94ac47c7d3b28f1f6e4c8")]
	public abstract class Field : MarshalByRefObject
	{
		public abstract object Value
		{
			get;
		}

		public abstract bool IsMissing
		{
			get;
		}

		public abstract string UniqueName
		{
			get;
		}

		public abstract string BackgroundColor
		{
			get;
		}

		public abstract string Color
		{
			get;
		}

		public abstract string FontFamily
		{
			get;
		}

		public abstract string FontSize
		{
			get;
		}

		public abstract string FontWeight
		{
			get;
		}

		public abstract string FontStyle
		{
			get;
		}

		public abstract string TextDecoration
		{
			get;
		}

		public abstract string FormattedValue
		{
			get;
		}

		public abstract object Key
		{
			get;
		}

		public abstract int LevelNumber
		{
			get;
		}

		public abstract string ParentUniqueName
		{
			get;
		}

		[IndexerName("Properties")]
		public abstract object this[string key]
		{
			get;
		}
	}
}
