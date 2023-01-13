using System;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
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
