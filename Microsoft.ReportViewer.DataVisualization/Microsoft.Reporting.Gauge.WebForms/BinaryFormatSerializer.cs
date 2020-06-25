using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class BinaryFormatSerializer : SerializerBase
	{
		private CaseInsensitiveHashCodeProvider hashCodeProvider = new CaseInsensitiveHashCodeProvider(CultureInfo.InvariantCulture);

		public override void Serialize(object objectToSerialize, object destination)
		{
			if (objectToSerialize == null)
			{
				throw new ArgumentNullException("objectToSerialize");
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (destination is string)
			{
				Serialize(objectToSerialize, (string)destination);
				return;
			}
			if (destination is Stream)
			{
				Serialize(objectToSerialize, (Stream)destination);
				return;
			}
			if (destination is BinaryWriter)
			{
				Serialize(objectToSerialize, (BinaryWriter)destination);
				return;
			}
			throw new ArgumentException(Utils.SRGetStr("ExceptionSerializerInvalidWriter"), "destination");
		}

		public void Serialize(object objectToSerialize, string fileName)
		{
			FileStream fileStream = new FileStream(fileName, FileMode.Create);
			Serialize(objectToSerialize, new BinaryWriter(fileStream));
			fileStream.Close();
		}

		public void Serialize(object objectToSerialize, Stream stream)
		{
			Serialize(objectToSerialize, new BinaryWriter(stream));
		}

		public void Serialize(object objectToSerialize, BinaryWriter writer)
		{
			if (objectToSerialize == null)
			{
				throw new ArgumentNullException("objectToSerialize");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			char[] chars = new char[15]
			{
				'D',
				'C',
				'B',
				'F',
				'2',
				'0',
				'0',
				'\0',
				'\0',
				'\0',
				'\0',
				'\0',
				'\0',
				'\0',
				'\0'
			};
			writer.Write(chars);
			SerializeObject(objectToSerialize, null, GetObjectName(objectToSerialize), writer);
			writer.Flush();
			writer.Seek(0, SeekOrigin.Begin);
		}

		private void SerializeObject(object objectToSerialize, object parent, string elementName, BinaryWriter writer)
		{
			if (objectToSerialize == null)
			{
				return;
			}
			if (parent != null)
			{
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(parent)[elementName];
				if (propertyDescriptor != null)
				{
					SerializationVisibilityAttribute serializationVisibilityAttribute = (SerializationVisibilityAttribute)propertyDescriptor.Attributes[typeof(SerializationVisibilityAttribute)];
					if (serializationVisibilityAttribute != null && serializationVisibilityAttribute.Visibility == SerializationVisibility.Hidden)
					{
						return;
					}
				}
			}
			if (objectToSerialize is ICollection)
			{
				SerializeCollection(objectToSerialize, elementName, writer);
				return;
			}
			writer.Write((short)hashCodeProvider.GetHashCode(elementName));
			long num = writer.Seek(0, SeekOrigin.Current);
			ArrayList arrayList = new ArrayList();
			PropertyInfo[] properties = objectToSerialize.GetType().GetProperties();
			if (properties != null)
			{
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (IsGaugeBaseProperty(objectToSerialize, parent, propertyInfo))
					{
						continue;
					}
					if (propertyInfo.CanRead && propertyInfo.PropertyType.GetInterface("ICollection", ignoreCase: true) != null)
					{
						bool flag = true;
						if (objectToSerialize != null)
						{
							PropertyDescriptor propertyDescriptor2 = TypeDescriptor.GetProperties(objectToSerialize)[propertyInfo.Name];
							if (propertyDescriptor2 != null)
							{
								SerializationVisibilityAttribute serializationVisibilityAttribute2 = (SerializationVisibilityAttribute)propertyDescriptor2.Attributes[typeof(SerializationVisibilityAttribute)];
								if (serializationVisibilityAttribute2 != null && serializationVisibilityAttribute2.Visibility == SerializationVisibility.Hidden)
								{
									flag = false;
								}
							}
						}
						if (flag)
						{
							arrayList.Add(propertyInfo.Name);
							SerializeCollection(propertyInfo.GetValue(objectToSerialize, null), propertyInfo.Name, writer);
						}
					}
					else
					{
						if (!propertyInfo.CanRead || !propertyInfo.CanWrite || propertyInfo.Name == "Item")
						{
							continue;
						}
						if (ShouldSerializeAsAttribute(propertyInfo, objectToSerialize))
						{
							if (IsSerializableContent(propertyInfo.Name, objectToSerialize))
							{
								SerializeProperty(propertyInfo.GetValue(objectToSerialize, null), objectToSerialize, propertyInfo.Name, writer);
							}
						}
						else
						{
							SerializeObject(propertyInfo.GetValue(objectToSerialize, null), objectToSerialize, propertyInfo.Name, writer);
						}
						arrayList.Add(propertyInfo.Name);
					}
				}
				CheckPropertiesID(arrayList);
			}
			if (writer.Seek(0, SeekOrigin.Current) == num)
			{
				writer.Seek(-2, SeekOrigin.Current);
				writer.Write((short)0);
				writer.Seek(-2, SeekOrigin.Current);
			}
			else
			{
				writer.Write((short)0);
			}
		}

		private void SerializeCollection(object objectToSerialize, string elementName, BinaryWriter writer)
		{
			if (!(objectToSerialize is ICollection))
			{
				return;
			}
			writer.Write((short)hashCodeProvider.GetHashCode(elementName));
			long num = writer.Seek(0, SeekOrigin.Current);
			foreach (object item in (ICollection)objectToSerialize)
			{
				SerializeObject(item, objectToSerialize, GetObjectName(item), writer);
			}
			if (writer.Seek(0, SeekOrigin.Current) == num)
			{
				writer.Seek(-2, SeekOrigin.Current);
				writer.Write((short)0);
				writer.Seek(-2, SeekOrigin.Current);
			}
			else
			{
				writer.Write((short)0);
			}
		}

		private void SerializeProperty(object objectToSerialize, object parent, string elementName, BinaryWriter writer)
		{
			if (objectToSerialize == null || parent == null)
			{
				return;
			}
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(parent)[elementName];
			if (propertyDescriptor != null)
			{
				DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)propertyDescriptor.Attributes[typeof(DefaultValueAttribute)];
				if (defaultValueAttribute != null)
				{
					if (objectToSerialize.Equals(defaultValueAttribute.Value))
					{
						return;
					}
				}
				else
				{
					MethodInfo method = parent.GetType().GetMethod("ShouldSerialize" + elementName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (method != null)
					{
						object obj = method.Invoke(parent, null);
						if (obj is bool && !(bool)obj)
						{
							return;
						}
					}
				}
				SerializationVisibilityAttribute serializationVisibilityAttribute = (SerializationVisibilityAttribute)propertyDescriptor.Attributes[typeof(SerializationVisibilityAttribute)];
				if (serializationVisibilityAttribute != null && serializationVisibilityAttribute.Visibility == SerializationVisibility.Hidden)
				{
					return;
				}
			}
			WritePropertyValue(objectToSerialize, parent, elementName, writer);
		}

		private void WritePropertyValue(object obj, object parent, string elementName, BinaryWriter writer)
		{
			writer.Write((short)hashCodeProvider.GetHashCode(elementName));
			if (obj is bool)
			{
				writer.Write((bool)obj);
				return;
			}
			if (obj is double)
			{
				writer.Write((double)obj);
				return;
			}
			if (obj is string)
			{
				writer.Write((string)obj);
				return;
			}
			if (obj is int)
			{
				writer.Write((int)obj);
				return;
			}
			if (obj is long)
			{
				writer.Write((long)obj);
				return;
			}
			if (obj is float)
			{
				writer.Write((float)obj);
				return;
			}
			if (obj.GetType().IsEnum)
			{
				writer.Write(Enum.GetName(obj.GetType(), obj));
				return;
			}
			if (obj is Font)
			{
				writer.Write(SerializerBase.fontConverter.ConvertToString(null, CultureInfo.InvariantCulture, obj));
				return;
			}
			if (obj is Color)
			{
				writer.Write(((Color)obj).ToArgb());
				return;
			}
			if (obj is DateTime)
			{
				writer.Write(((DateTime)obj).Ticks);
				return;
			}
			if (obj is Size)
			{
				writer.Write(((Size)obj).Width);
				writer.Write(((Size)obj).Height);
				return;
			}
			if (obj is Point)
			{
				writer.Write(((Point)obj).X);
				writer.Write(((Point)obj).Y);
				return;
			}
			if (obj is double[])
			{
				double[] array = (double[])obj;
				writer.Write(array.Length);
				double[] array2 = array;
				foreach (double value in array2)
				{
					writer.Write(value);
				}
				return;
			}
			if (obj is Image)
			{
				MemoryStream memoryStream = new MemoryStream();
				ImageFormat rawFormat = ((Image)obj).RawFormat;
				rawFormat = ((rawFormat == ImageFormat.MemoryBmp || ImageFormat.MemoryBmp.Guid.Equals(rawFormat.Guid)) ? ImageFormat.Png : rawFormat);
				((Image)obj).Save(memoryStream, rawFormat);
				int value2 = (int)memoryStream.Seek(0L, SeekOrigin.End);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				writer.Write(value2);
				writer.Write(memoryStream.ToArray());
				memoryStream.Close();
				return;
			}
			throw new InvalidOperationException(Utils.SRGetStr("ExceptionSerializerInvalidObjectType"));
		}

		public void CheckPropertiesID(ArrayList propNames)
		{
		}

		public override void Deserialize(object objectToDeserialize, object source)
		{
			if (objectToDeserialize == null)
			{
				throw new ArgumentNullException("objectToDeserialize");
			}
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (source is string)
			{
				Deserialize(objectToDeserialize, (string)source);
				return;
			}
			if (source is Stream)
			{
				Deserialize(objectToDeserialize, (Stream)source);
				return;
			}
			if (source is BinaryWriter)
			{
				Deserialize(objectToDeserialize, (BinaryWriter)source);
				return;
			}
			throw new ArgumentException(Utils.SRGetStr("ExceptionSerializerInvalidReader"), "source");
		}

		public void Deserialize(object objectToDeserialize, string fileName)
		{
			FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			Deserialize(objectToDeserialize, new BinaryReader(fileStream));
			fileStream.Close();
		}

		public void Deserialize(object objectToDeserialize, Stream stream)
		{
			Deserialize(objectToDeserialize, new BinaryReader(stream));
		}

		public void Deserialize(object objectToDeserialize, BinaryReader reader)
		{
			if (objectToDeserialize == null)
			{
				throw new ArgumentNullException("objectToDeserialize");
			}
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (base.IgnoreUnknownAttributes)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionSerializerUnsupportedAttribute"));
			}
			char[] array = reader.ReadChars(15);
			if (array[0] != 'D' || array[1] != 'C' || array[2] != 'B' || array[3] != 'F')
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionSerializerInvalidBinary"));
			}
			reader.ReadInt16();
			if (base.ResetWhenLoading)
			{
				ResetObjectProperties(objectToDeserialize);
			}
			DeserializeObject(objectToDeserialize, null, GetObjectName(objectToDeserialize), reader);
		}

		protected virtual int DeserializeObject(object objectToDeserialize, object parent, string elementName, BinaryReader reader)
		{
			int num = 0;
			if (objectToDeserialize == null)
			{
				return num;
			}
			Type[] array = null;
			int num2 = 0;
			if (objectToDeserialize is IList)
			{
				short num3 = 0;
				while ((num3 = reader.ReadInt16()) != 0)
				{
					string itemTypeName = string.Empty;
					PropertyInfo property = objectToDeserialize.GetType().GetProperty("Item");
					if (property != null)
					{
						Assembly assembly = property.PropertyType.Assembly;
						if (assembly != null)
						{
							if (array == null)
							{
								array = assembly.GetExportedTypes();
							}
							Type[] array2 = array;
							foreach (Type type in array2)
							{
								if (type.IsSubclassOf(property.PropertyType) && (short)hashCodeProvider.GetHashCode(type.Name) == num3)
								{
									itemTypeName = type.Name;
								}
							}
						}
					}
					string itemName = null;
					bool reusedObject = false;
					object listNewItem = GetListNewItem((IList)objectToDeserialize, itemTypeName, ref itemName, ref reusedObject);
					long offset = reader.BaseStream.Seek(0L, SeekOrigin.Current);
					int num4 = DeserializeObject(listNewItem, objectToDeserialize, "", reader);
					if (num4 > 0 || reusedObject)
					{
						bool flag = true;
						PropertyInfo property2 = listNewItem.GetType().GetProperty("Name");
						if (property2 != null)
						{
							object obj = null;
							try
							{
								itemName = (string)property2.GetValue(listNewItem, null);
								if (itemName != null && itemName.Length > 0)
								{
									bool reusedObject2 = false;
									obj = GetListNewItem((IList)objectToDeserialize, itemTypeName, ref itemName, ref reusedObject2);
									if (itemName == null)
									{
										obj = null;
									}
								}
							}
							catch (Exception)
							{
							}
							if (obj != null)
							{
								flag = false;
								reader.BaseStream.Seek(offset, SeekOrigin.Begin);
								num4 = DeserializeObject(obj, objectToDeserialize, "", reader);
								((IList)objectToDeserialize).Remove(obj);
								((IList)objectToDeserialize).Insert(num2++, obj);
							}
						}
						if (flag)
						{
							((IList)objectToDeserialize).Insert(num2++, listNewItem);
						}
					}
					num += num4;
				}
				return num;
			}
			PropertyInfo[] properties = objectToDeserialize.GetType().GetProperties();
			if (properties == null)
			{
				return num;
			}
			PropertyInfo propertyInfo = null;
			while ((propertyInfo = ReadPropertyInfo(objectToDeserialize, parent, properties, reader)) != null)
			{
				if (ShouldSerializeAsAttribute(propertyInfo, objectToDeserialize))
				{
					if (SetPropertyValue(objectToDeserialize, propertyInfo, reader))
					{
						num++;
					}
					continue;
				}
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(objectToDeserialize)[propertyInfo.Name];
				if (propertyDescriptor != null)
				{
					object value = propertyDescriptor.GetValue(objectToDeserialize);
					num += DeserializeObject(value, objectToDeserialize, propertyInfo.Name, reader);
				}
				else if (!base.IgnoreUnknownAttributes)
				{
					throw new InvalidOperationException(Utils.SRGetStr("ExceptionSerializerUnknownProperty", propertyInfo.Name, objectToDeserialize.GetType().ToString()));
				}
			}
			return num;
		}

		private bool SetPropertyValue(object obj, PropertyInfo pi, BinaryReader reader)
		{
			if (pi != null)
			{
				object obj2 = null;
				if (pi.PropertyType == typeof(bool))
				{
					obj2 = reader.ReadBoolean();
				}
				else if (pi.PropertyType == typeof(double))
				{
					obj2 = reader.ReadDouble();
				}
				else if (pi.PropertyType == typeof(string))
				{
					obj2 = reader.ReadString();
				}
				else if (pi.PropertyType == typeof(int))
				{
					obj2 = reader.ReadInt32();
				}
				else if (pi.PropertyType == typeof(long))
				{
					obj2 = reader.ReadInt64();
				}
				else if (pi.PropertyType == typeof(float))
				{
					obj2 = reader.ReadSingle();
				}
				else if (pi.PropertyType.IsEnum)
				{
					obj2 = Enum.Parse(pi.PropertyType, reader.ReadString());
				}
				else if (pi.PropertyType == typeof(Font))
				{
					obj2 = SerializerBase.fontConverter.ConvertFromString(null, CultureInfo.InvariantCulture, reader.ReadString());
				}
				else if (pi.PropertyType == typeof(Color))
				{
					obj2 = Color.FromArgb(reader.ReadInt32());
				}
				else if (pi.PropertyType == typeof(DateTime))
				{
					obj2 = new DateTime(reader.ReadInt64());
				}
				else if (pi.PropertyType == typeof(Size))
				{
					obj2 = new Size(reader.ReadInt32(), reader.ReadInt32());
				}
				else if (pi.PropertyType == typeof(Point))
				{
					obj2 = new Point(reader.ReadInt32(), reader.ReadInt32());
				}
				else if (pi.PropertyType == typeof(double[]))
				{
					double[] array = new double[reader.ReadInt32()];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = reader.ReadDouble();
					}
					obj2 = array;
				}
				else
				{
					if (!(pi.PropertyType == typeof(Image)))
					{
						throw new InvalidOperationException(Utils.SRGetStr("ExceptionSerializerInvalidObjectType", obj.GetType().ToString()));
					}
					int num = reader.ReadInt32();
					MemoryStream memoryStream = new MemoryStream(num + 10);
					memoryStream.Write(reader.ReadBytes(num), 0, num);
					obj2 = new Bitmap(Image.FromStream(memoryStream));
					memoryStream.Close();
				}
				if (IsSerializableContent(pi.Name, obj))
				{
					pi.SetValue(obj, obj2, null);
					return true;
				}
			}
			return false;
		}

		private PropertyInfo ReadPropertyInfo(object objectToDeserialize, object parent, PropertyInfo[] properties, BinaryReader reader)
		{
			short num = reader.ReadInt16();
			if (num == 0)
			{
				return null;
			}
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (IsGaugeBaseProperty(objectToDeserialize, parent, propertyInfo))
				{
					continue;
				}
				if (propertyInfo.CanRead && propertyInfo.PropertyType.GetInterface("ICollection", ignoreCase: true) != null)
				{
					if ((short)hashCodeProvider.GetHashCode(propertyInfo.Name) == num)
					{
						return propertyInfo;
					}
				}
				else if (propertyInfo.CanRead && propertyInfo.CanWrite && !(propertyInfo.Name == "Item") && (short)hashCodeProvider.GetHashCode(propertyInfo.Name) == num)
				{
					return propertyInfo;
				}
			}
			throw new InvalidOperationException(Utils.SRGetStr("ExceptionSerializerIdLoading"));
		}
	}
}
