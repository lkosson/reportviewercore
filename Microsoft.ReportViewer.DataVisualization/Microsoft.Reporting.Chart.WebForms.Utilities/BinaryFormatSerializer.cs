using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.Reporting.Chart.WebForms.Utilities
{
	internal class BinaryFormatSerializer : SerializerBase
	{
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
			throw new ArgumentException(SR.ExceptionChartSerializerDestinationObjectInvalid, "destination");
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
			binaryFormatVersion = 300;
			char[] chars = new char[15]
			{
				'D',
				'C',
				'B',
				'F',
				'3',
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

		protected virtual void SerializeObject(object objectToSerialize, object parent, string elementName, BinaryWriter writer)
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
			writer.Write(SerializerBase.GetStringHashCode(elementName));
			long num = writer.Seek(0, SeekOrigin.Current);
			ArrayList arrayList = new ArrayList();
			PropertyInfo[] properties = objectToSerialize.GetType().GetProperties();
			if (properties != null)
			{
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (IsChartBaseProperty(objectToSerialize, parent, propertyInfo))
					{
						continue;
					}
					if (propertyInfo.CanRead && propertyInfo.PropertyType.GetInterface("ICollection", ignoreCase: true) != null && !SerializeICollAsAtribute(propertyInfo, objectToSerialize))
					{
						bool flag = IsSerializableContent(propertyInfo.Name, objectToSerialize);
						if (flag && objectToSerialize != null)
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
						MethodInfo method = objectToSerialize.GetType().GetMethod("ShouldSerialize" + propertyInfo.Name);
						if (flag && method != null)
						{
							object obj = method.Invoke(objectToSerialize, null);
							if (obj is bool && !(bool)obj)
							{
								flag = false;
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
						bool flag2 = true;
						MethodInfo method2 = objectToSerialize.GetType().GetMethod("ShouldSerialize" + propertyInfo.Name);
						if (method2 != null)
						{
							object obj2 = method2.Invoke(objectToSerialize, null);
							if (obj2 is bool && !(bool)obj2)
							{
								flag2 = false;
							}
						}
						if (flag2)
						{
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

		protected virtual void SerializeCollection(object objectToSerialize, string elementName, BinaryWriter writer)
		{
			if (!(objectToSerialize is ICollection))
			{
				return;
			}
			writer.Write(SerializerBase.GetStringHashCode(elementName));
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

		protected virtual void SerializeProperty(object objectToSerialize, object parent, string elementName, BinaryWriter writer)
		{
			if (objectToSerialize == null || parent == null)
			{
				return;
			}
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(parent)[elementName];
			if (propertyDescriptor != null)
			{
				DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)propertyDescriptor.Attributes[typeof(DefaultValueAttribute)];
				if (defaultValueAttribute != null && objectToSerialize.Equals(defaultValueAttribute.Value))
				{
					return;
				}
				SerializationVisibilityAttribute serializationVisibilityAttribute = (SerializationVisibilityAttribute)propertyDescriptor.Attributes[typeof(SerializationVisibilityAttribute)];
				if (serializationVisibilityAttribute != null && serializationVisibilityAttribute.Visibility == SerializationVisibility.Hidden)
				{
					return;
				}
			}
			WritePropertyValue(objectToSerialize, parent, elementName, writer);
		}

		protected void WritePropertyValue(object obj, object parent, string elementName, BinaryWriter writer)
		{
			writer.Write(SerializerBase.GetStringHashCode(elementName));
			if (obj is bool)
			{
				writer.Write((bool)obj);
			}
			else if (obj is double)
			{
				writer.Write((double)obj);
			}
			else if (obj is string)
			{
				writer.Write((string)obj);
			}
			else if (obj is int)
			{
				writer.Write((int)obj);
			}
			else if (obj is long)
			{
				writer.Write((long)obj);
			}
			else if (obj is float)
			{
				writer.Write((float)obj);
			}
			else if (obj.GetType().IsEnum)
			{
				string value = ((Enum)obj).ToString();
				writer.Write(value);
			}
			else if (obj is byte)
			{
				writer.Write((byte)obj);
			}
			else if (obj is Font)
			{
				writer.Write(SerializerBase.FontToString((Font)obj));
			}
			else if (obj is Color)
			{
				writer.Write(((Color)obj).ToArgb());
			}
			else if (obj is DateTime)
			{
				writer.Write(((DateTime)obj).Ticks);
			}
			else if (obj is Size)
			{
				writer.Write(((Size)obj).Width);
				writer.Write(((Size)obj).Height);
			}
			else if (obj is double[])
			{
				double[] array = (double[])obj;
				writer.Write(array.Length);
				double[] array2 = array;
				foreach (double value2 in array2)
				{
					writer.Write(value2);
				}
			}
			else if (obj is Color[])
			{
				Color[] array3 = (Color[])obj;
				writer.Write(array3.Length);
				Color[] array4 = array3;
				foreach (Color color in array4)
				{
					writer.Write(color.ToArgb());
				}
			}
			else if (obj is Image)
			{
				MemoryStream memoryStream = new MemoryStream();
				((Image)obj).Save(memoryStream, ((Image)obj).RawFormat);
				int value3 = (int)memoryStream.Seek(0L, SeekOrigin.End);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				writer.Write(value3);
				writer.Write(memoryStream.ToArray());
				memoryStream.Close();
			}
			else
			{
				if (!(obj is Margins))
				{
					throw new InvalidOperationException(SR.ExceptionChartSerializerBinaryTypeUnsupported(obj.GetType().ToString()));
				}
				writer.Write(((Margins)obj).Top);
				writer.Write(((Margins)obj).Bottom);
				writer.Write(((Margins)obj).Left);
				writer.Write(((Margins)obj).Right);
			}
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
			throw new ArgumentException(SR.ExceptionChartSerializerSourceObjectInvalid, "source");
		}

		public void Deserialize(object objectToDeserialize, string fileName)
		{
			FileStream fileStream = new FileStream(fileName, FileMode.Open);
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
				throw new InvalidOperationException(SR.ExceptionChartSerializerBinaryIgnoreUnknownAttributesUnsupported);
			}
			char[] array = reader.ReadChars(15);
			if (array[0] != 'D' || array[1] != 'C' || array[2] != 'B' || array[3] != 'F')
			{
				throw new InvalidOperationException(SR.ExceptionChartSerializerBinaryFromatInvalid);
			}
			binaryFormatVersion = int.Parse(new string(array, 4, 3), CultureInfo.InvariantCulture);
			ReadHashID(reader, isCollectionMember: false);
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
				while ((num3 = ReadHashID(reader, isCollectionMember: true)) != 0)
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
								if (type.IsSubclassOf(property.PropertyType) && SerializerBase.GetStringHashCode(type.Name) == num3)
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
					throw new InvalidOperationException(SR.ExceptionChartSerializerPropertyNameUnknown(propertyInfo.Name, objectToDeserialize.GetType().ToString()));
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
				else if (pi.PropertyType == typeof(byte))
				{
					obj2 = reader.ReadByte();
				}
				else if (pi.PropertyType == typeof(Font))
				{
					obj2 = SerializerBase.FontFromString(reader.ReadString());
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
				else if (pi.PropertyType == typeof(Margins))
				{
					obj2 = new Margins(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
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
				else if (pi.PropertyType == typeof(Color[]))
				{
					Color[] array2 = new Color[reader.ReadInt32()];
					for (int j = 0; j < array2.Length; j++)
					{
						array2[j] = Color.FromArgb(reader.ReadInt32());
					}
					obj2 = array2;
				}
				else
				{
					if (!(pi.PropertyType == typeof(Image)))
					{
						throw new InvalidOperationException(SR.ExceptionChartSerializerBinaryTypeUnsupported(obj.GetType().ToString()));
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
			short num = ReadHashID(reader, isCollectionMember: false);
			if (num == 0)
			{
				return null;
			}
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (IsChartBaseProperty(objectToDeserialize, parent, propertyInfo))
				{
					continue;
				}
				if (propertyInfo.CanRead && propertyInfo.PropertyType.GetInterface("ICollection", ignoreCase: true) != null)
				{
					if (SerializerBase.GetStringHashCode(propertyInfo.Name) == num)
					{
						return propertyInfo;
					}
				}
				else if (propertyInfo.CanRead && propertyInfo.CanWrite && !(propertyInfo.Name == "Item") && SerializerBase.GetStringHashCode(propertyInfo.Name) == num)
				{
					return propertyInfo;
				}
			}
			throw new InvalidOperationException(SR.ExceptionChartSerializerPropertyNotFound);
		}
	}
}
