namespace Microsoft.ReportingServices.RdlObjectModel.Serialization
{
	internal class NameKey
	{
		internal string ns;

		internal string name;

		public string Name => name;

		public string Namespace => ns;

		internal NameKey(string name, string ns)
		{
			this.name = name;
			this.ns = ns;
		}

		public override bool Equals(object other)
		{
			if (!(other is NameKey))
			{
				return false;
			}
			NameKey nameKey = (NameKey)other;
			if (name == nameKey.name)
			{
				return ns == nameKey.ns;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((ns != null) ? ns.GetHashCode() : 0) ^ ((name != null) ? name.GetHashCode() : 0);
		}

		public static bool operator ==(NameKey a, NameKey b)
		{
			if (a.name == b.name)
			{
				return a.ns == b.ns;
			}
			return false;
		}

		public static bool operator !=(NameKey a, NameKey b)
		{
			return !(a == b);
		}
	}
}
