using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class MapImageCollection : NamedCollection
	{
		private MapImage this[int index]
		{
			get
			{
				return (MapImage)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private MapImage this[string name]
		{
			get
			{
				return (MapImage)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public MapImage this[object obj]
		{
			get
			{
				if (obj is string)
				{
					return this[(string)obj];
				}
				if (obj is int)
				{
					return this[(int)obj];
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgument);
			}
			set
			{
				if (obj is string)
				{
					this[(string)obj] = value;
					return;
				}
				if (obj is int)
				{
					this[(int)obj] = value;
					return;
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgument);
			}
		}

		internal MapImageCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(MapImage);
		}

		public MapImage Add(string name)
		{
			MapImage mapImage = new MapImage();
			mapImage.Name = name;
			Add(mapImage);
			return mapImage;
		}

		public int Add(MapImage value)
		{
			return base.List.Add(value);
		}

		public void Remove(MapImage value)
		{
			base.List.Remove(value);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "Image1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Image{0}";
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			MapImage mapImage = (MapImage)value;
			if (mapImage.Position.DefaultValues && index != 0)
			{
				MapImage mapImage2 = this[index - 1];
				mapImage.Location.X = mapImage2.Location.X + 3f;
				mapImage.Location.Y = mapImage2.Location.Y + 3f;
			}
		}
	}
}
