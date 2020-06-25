using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal struct IntermediateFormatWriter
	{
		private int m_currentMemberIndex;

		private Declaration m_currentDeclaration;

		private Dictionary<ObjectType, Declaration> m_writtenDecls;

		private PersistenceBinaryWriter m_writer;

		private PersistenceHelper m_persistenceContext;

		private bool m_isSeekable;

		private int m_lastMemberInfoIndex;

		private MemberInfo m_currentMember;

		private readonly bool m_prohibitSerializableValues;

		private int m_compatVersion;

		private BinaryFormatter m_binaryFormatter;

		private bool UsesCompatVersion => m_compatVersion != 0;

		internal MemberInfo CurrentMember => m_currentMember;

		internal PersistenceHelper PersistenceHelper => m_persistenceContext;

		internal bool ProhibitSerializableValues => m_prohibitSerializableValues;

		internal IntermediateFormatWriter(Stream str, int compatVersion)
			: this(str, 0L, null, null, compatVersion, prohibitSerializableValues: false)
		{
		}

		internal IntermediateFormatWriter(Stream str, int compatVersion, bool prohibitSerializableValues)
			: this(str, 0L, null, null, compatVersion, prohibitSerializableValues)
		{
		}

		internal IntermediateFormatWriter(Stream str, PersistenceHelper persistenceContext, int compatVersion)
			: this(str, 0L, null, persistenceContext, compatVersion, prohibitSerializableValues: false)
		{
		}

		internal IntermediateFormatWriter(Stream str, List<Declaration> declarations, int compatVersion)
			: this(str, 0L, declarations, null, compatVersion, prohibitSerializableValues: false)
		{
		}

		internal IntermediateFormatWriter(Stream str, List<Declaration> declarations, int compatVersion, bool prohibitSerializableValues)
			: this(str, 0L, declarations, null, compatVersion, prohibitSerializableValues)
		{
		}

		internal IntermediateFormatWriter(Stream str, List<Declaration> declarations, PersistenceHelper persistenceContext, int compatVersion)
			: this(str, 0L, declarations, persistenceContext, compatVersion, prohibitSerializableValues: false)
		{
		}

		internal IntermediateFormatWriter(Stream str, List<Declaration> declarations, PersistenceHelper persistenceContext, int compatVersion, bool prohibitSerializableValues)
			: this(str, 0L, declarations, persistenceContext, compatVersion, prohibitSerializableValues)
		{
		}

		internal IntermediateFormatWriter(Stream str, long startOffset, List<Declaration> declarations, PersistenceHelper persistenceContext, int compatVersion, bool prohibitSerializableValues)
		{
			m_writer = new PersistenceBinaryWriter(str);
			m_writtenDecls = new Dictionary<ObjectType, Declaration>(EqualityComparers.ObjectTypeComparerInstance);
			m_currentDeclaration = null;
			m_currentMemberIndex = 0;
			m_lastMemberInfoIndex = 0;
			m_currentMember = null;
			m_persistenceContext = persistenceContext;
			m_isSeekable = false;
			m_binaryFormatter = null;
			m_compatVersion = compatVersion;
			m_prohibitSerializableValues = prohibitSerializableValues;
			if (startOffset == 0L)
			{
				Global.Tracer.Assert(!m_isSeekable, "(!m_isSeekable)");
				Write(IntermediateFormatVersion.Current);
			}
			m_isSeekable = (declarations != null);
			PersistenceFlags persistenceFlags = PersistenceFlags.None;
			if (m_isSeekable)
			{
				persistenceFlags = PersistenceFlags.Seekable;
			}
			if (UsesCompatVersion)
			{
				persistenceFlags |= PersistenceFlags.CompatVersioned;
			}
			if (startOffset == 0L)
			{
				m_writer.WriteEnum((int)persistenceFlags);
				if (UsesCompatVersion)
				{
					((BinaryWriter)m_writer).Write(m_compatVersion);
				}
				if (m_isSeekable)
				{
					WriteDeclarations(declarations);
				}
			}
			else if (m_isSeekable)
			{
				FilterAndStoreDeclarations(declarations);
			}
		}

		private void WriteDeclarations(List<Declaration> declarations)
		{
			m_writer.WriteListStart(ObjectType.Declaration, declarations.Count);
			for (int i = 0; i < declarations.Count; i++)
			{
				Declaration decl = declarations[i];
				WriteDeclaration(decl);
			}
		}

		private Declaration WriteDeclaration(Declaration decl)
		{
			decl = FilterAndStoreDeclaration(decl);
			m_writer.Write(decl);
			return decl;
		}

		private void FilterAndStoreDeclarations(List<Declaration> declarations)
		{
			for (int i = 0; i < declarations.Count; i++)
			{
				Declaration decl = declarations[i];
				FilterAndStoreDeclaration(decl);
			}
		}

		private Declaration FilterAndStoreDeclaration(Declaration decl)
		{
			decl = decl.CreateFilteredDeclarationForWriteVersion(m_compatVersion);
			m_writtenDecls.Add(decl.ObjectType, decl);
			return decl;
		}

		internal void RegisterDeclaration(Declaration declaration)
		{
			if (!m_writtenDecls.TryGetValue(declaration.ObjectType, out Declaration value))
			{
				value = WriteDeclaration(declaration);
			}
			m_currentDeclaration = value;
			m_lastMemberInfoIndex = m_currentDeclaration.MemberInfoList.Count - 1;
			m_currentMemberIndex = -1;
		}

		internal bool NextMember()
		{
			if (m_currentMemberIndex < m_lastMemberInfoIndex)
			{
				m_currentMemberIndex++;
				m_currentMember = m_currentDeclaration.MemberInfoList[m_currentMemberIndex];
				return true;
			}
			return false;
		}

		internal void Write(IPersistable persistableObj)
		{
			Write(persistableObj, verify: true);
		}

		private void Write(IPersistable persistableObj, bool verify)
		{
			if (persistableObj != null)
			{
				m_writer.Write(persistableObj.GetObjectType());
				persistableObj.Serialize(this);
			}
			else
			{
				m_writer.WriteNull();
			}
		}

		internal void WriteNameObjectCollection(INameObjectCollection collection)
		{
			if (collection == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, collection.Count);
			for (int i = 0; i < collection.Count; i++)
			{
				((BinaryWriter)m_writer).Write(collection.GetKey(i));
				Write(collection.GetValue(i));
			}
		}

		internal void WriteStringRIFObjectDictionary<TVal>(Dictionary<string, TVal> dictionary) where TVal : IPersistable
		{
			if (dictionary == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, dictionary.Count);
			foreach (KeyValuePair<string, TVal> item in dictionary)
			{
				((BinaryWriter)m_writer).Write(item.Key);
				Write(item.Value);
			}
		}

		internal void WriteStringListOfStringDictionary(Dictionary<string, List<string>> dictionary)
		{
			if (dictionary == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, dictionary.Count);
			foreach (KeyValuePair<string, List<string>> item in dictionary)
			{
				((BinaryWriter)m_writer).Write(item.Key);
				WriteListOfPrimitives(item.Value, verify: false);
			}
		}

		internal void WriteInt32RIFObjectDictionary<TVal>(Dictionary<int, TVal> dictionary) where TVal : IPersistable
		{
			if (dictionary == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, dictionary.Count);
			foreach (KeyValuePair<int, TVal> item in dictionary)
			{
				((BinaryWriter)m_writer).Write(item.Key);
				Write(item.Value);
			}
		}

		internal void WriteInt32RIFObjectDictionary<TVal>(IDictionary dictionary) where TVal : IPersistable
		{
			if (dictionary == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, dictionary.Count);
			foreach (DictionaryEntry item in dictionary)
			{
				((BinaryWriter)m_writer).Write((int)item.Key);
				Write((TVal)item.Value);
			}
		}

		internal void WriteStringRIFObjectHashtable(Hashtable hashtable)
		{
			if (hashtable == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, hashtable.Count);
			foreach (DictionaryEntry item in hashtable)
			{
				((BinaryWriter)m_writer).Write((string)item.Key);
				Write((IPersistable)item.Value);
			}
		}

		internal void WriteInt32PrimitiveListHashtable<T>(Hashtable hashtable)
		{
			if (hashtable == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, hashtable.Count);
			foreach (DictionaryEntry item in hashtable)
			{
				((BinaryWriter)m_writer).Write((int)item.Key);
				WriteListOfPrimitives((List<T>)item.Value, verify: false);
			}
		}

		internal void WriteStringObjectHashtable(IDictionary dictionary)
		{
			if (dictionary == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, dictionary.Count);
			foreach (DictionaryEntry item in dictionary)
			{
				((BinaryWriter)m_writer).Write((string)item.Key);
				Write(item.Value);
			}
		}

		internal void WriteStringRIFObjectHashtable(IDictionary dictionary)
		{
			if (dictionary == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, dictionary.Count);
			foreach (DictionaryEntry item in dictionary)
			{
				((BinaryWriter)m_writer).Write((string)item.Key);
				Write((IPersistable)item.Value);
			}
		}

		internal void WriteStringInt32Hashtable(IDictionary dictionary)
		{
			if (dictionary == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, dictionary.Count);
			foreach (DictionaryEntry item in dictionary)
			{
				((BinaryWriter)m_writer).Write((string)item.Key);
				((BinaryWriter)m_writer).Write((int)item.Value);
			}
		}

		internal void WriteStringStringHashtable(IDictionary dictionary)
		{
			if (dictionary == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, dictionary.Count);
			foreach (DictionaryEntry item in dictionary)
			{
				((BinaryWriter)m_writer).Write((string)item.Key);
				((BinaryWriter)m_writer).Write((string)item.Value);
			}
		}

		internal void WriteObjectHashtableHashtable(Hashtable hashtable)
		{
			if (hashtable == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, hashtable.Count);
			foreach (DictionaryEntry item in hashtable)
			{
				Write(item.Key);
				WriteVariantVariantHashtable((Hashtable)item.Value);
			}
		}

		internal void WriteNLevelVariantHashtable(Hashtable hashtable)
		{
			if (hashtable == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, hashtable.Count);
			foreach (DictionaryEntry item in hashtable)
			{
				Write(item.Key);
				Hashtable hashtable2 = item.Value as Hashtable;
				if (hashtable2 != null)
				{
					m_writer.Write(Token.Hashtable);
					WriteNLevelVariantHashtable(hashtable2);
				}
				else
				{
					m_writer.Write(Token.Object);
					Write(item.Value, verify: false, assertOnInvalidType: true);
				}
			}
		}

		internal void WriteRIFObjectStringHashtable(IDictionary hashtable)
		{
			if (hashtable == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, hashtable.Count);
			foreach (DictionaryEntry item in hashtable)
			{
				Write((IPersistable)item.Key);
				((BinaryWriter)m_writer).Write((string)item.Value);
			}
		}

		internal void WriteVariantVariantHashtable(Hashtable hashtable)
		{
			if (hashtable == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, hashtable.Count);
			foreach (DictionaryEntry item in hashtable)
			{
				Write(item.Key);
				Write(item.Value);
			}
		}

		internal void WriteVariantListVariantDictionary(Dictionary<List<object>, object> dictionary)
		{
			if (dictionary == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, dictionary.Count);
			foreach (KeyValuePair<List<object>, object> item in dictionary)
			{
				WriteListOfVariant(item.Key);
				Write(item.Value);
			}
		}

		internal void WriteStringVariantListDictionary(Dictionary<string, List<object>> dictionary)
		{
			if (dictionary == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, dictionary.Count);
			foreach (KeyValuePair<string, List<object>> item in dictionary)
			{
				Write(item.Key);
				WriteListOfVariant(item.Value);
			}
		}

		internal void WriteStringBoolArrayDictionary(Dictionary<string, bool[]> dictionary)
		{
			if (dictionary == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, dictionary.Count);
			foreach (KeyValuePair<string, bool[]> item in dictionary)
			{
				((BinaryWriter)m_writer).Write(item.Key);
				m_writer.Write(item.Value);
			}
		}

		internal void WriteInt32StringHashtable(Hashtable hashtable)
		{
			if (hashtable == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, hashtable.Count);
			foreach (DictionaryEntry item in hashtable)
			{
				((BinaryWriter)m_writer).Write((int)item.Key);
				((BinaryWriter)m_writer).Write((string)item.Value);
			}
		}

		internal void WriteByteVariantHashtable(IDictionary dictionary)
		{
			if (dictionary == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, dictionary.Count);
			foreach (DictionaryEntry item in dictionary)
			{
				((BinaryWriter)m_writer).Write((byte)item.Key);
				Write(item.Value);
			}
		}

		internal void WriteVariantRifObjectDictionary(IDictionary dictionary)
		{
			if (dictionary == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, dictionary.Count);
			foreach (DictionaryEntry item in dictionary)
			{
				Write(item.Key);
				Write((IPersistable)item.Value);
			}
		}

		internal void WriteVariantListOfRifObjectDictionary(IDictionary dictionary)
		{
			if (dictionary == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, dictionary.Count);
			foreach (DictionaryEntry item in dictionary)
			{
				Write(item.Key);
				Write((IPersistable)item.Value);
			}
		}

		internal void Int32SerializableDictionary(Dictionary<int, object> dictionary)
		{
			if (dictionary == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteDictionaryStart(m_currentMember.ObjectType, dictionary.Count);
			foreach (KeyValuePair<int, object> item in dictionary)
			{
				((BinaryWriter)m_writer).Write(item.Key);
				WriteSerializable(item.Value);
			}
		}

		internal void WriteListOfReferences(IList rifList)
		{
			if (rifList == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteListStart(m_currentMember.ObjectType, rifList.Count);
			for (int i = 0; i < rifList.Count; i++)
			{
				WriteReferenceInList((IReferenceable)rifList[i]);
			}
		}

		internal void WriteListOfGlobalReferences(IList rifList)
		{
			if (rifList == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteListStart(m_currentMember.ObjectType, rifList.Count);
			for (int i = 0; i < rifList.Count; i++)
			{
				WriteGlobalReferenceInList((IGloballyReferenceable)rifList[i]);
			}
		}

		internal void Write<T>(List<T> rifList) where T : IPersistable
		{
			WriteRIFList(rifList);
		}

		internal void WriteRIFList<T>(IList<T> rifList) where T : IPersistable
		{
			if (rifList == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteListStart(m_currentMember.ObjectType, rifList.Count);
			for (int i = 0; i < rifList.Count; i++)
			{
				Write(rifList[i]);
			}
		}

		internal void Write(ArrayList rifObjectList)
		{
			if (rifObjectList == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteListStart(m_currentMember.ObjectType, rifObjectList.Count);
			for (int i = 0; i < rifObjectList.Count; i++)
			{
				Write((IPersistable)rifObjectList[i]);
			}
		}

		internal void Write<T>(List<List<T>> rifObjectLists) where T : IPersistable
		{
			if (rifObjectLists == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteListStart(m_currentMember.ObjectType, rifObjectLists.Count);
			for (int i = 0; i < rifObjectLists.Count; i++)
			{
				Write(rifObjectLists[i]);
			}
		}

		internal void Write<T>(List<T[]> rifObjectArrays) where T : IPersistable
		{
			if (rifObjectArrays == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteListStart(m_currentMember.ObjectType, rifObjectArrays.Count);
			for (int i = 0; i < rifObjectArrays.Count; i++)
			{
				Write(rifObjectArrays[i]);
			}
		}

		internal void WriteListOfVariant(IList list)
		{
			if (list == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteListStart(ObjectType.VariantList, list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				Write(list[i], verify: false, assertOnInvalidType: true);
			}
		}

		internal void WriteArrayListOfPrimitives(ArrayList list)
		{
			if (list == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteListStart(ObjectType.PrimitiveList, list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				Write(list[i], verify: false, assertOnInvalidType: true);
			}
		}

		internal void WriteListOfPrimitives<T>(List<T> list)
		{
			WriteListOfPrimitives(list, verify: true);
		}

		private void WriteListOfPrimitives<T>(List<T> list, bool verify)
		{
			if (list == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteListStart(ObjectType.PrimitiveList, list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				Write(list[i], verify: false, assertOnInvalidType: true);
			}
		}

		internal void WriteArrayOfListsOfPrimitives<T>(List<T>[] arrayOfLists)
		{
			WriteArrayOfListsOfPrimitives(arrayOfLists, validate: true);
		}

		private void WriteArrayOfListsOfPrimitives<T>(List<T>[] arrayOfLists, bool validate)
		{
			if (arrayOfLists == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteArrayStart(ObjectType.PrimitiveArray, arrayOfLists.Length);
			for (int i = 0; i < arrayOfLists.Length; i++)
			{
				WriteListOfPrimitives(arrayOfLists[i]);
			}
		}

		internal void WriteListOfArrayOfListsOfPrimitives<T>(List<List<T>[]> outerList)
		{
			if (outerList == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteListStart(ObjectType.PrimitiveList, outerList.Count);
			for (int i = 0; i < outerList.Count; i++)
			{
				WriteArrayOfListsOfPrimitives(outerList[i], validate: false);
			}
		}

		internal void Write<T>(List<T>[] rifObjectListArray) where T : IPersistable
		{
			if (rifObjectListArray == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteArrayStart(ObjectType.RIFObjectArray, rifObjectListArray.Length);
			for (int i = 0; i < rifObjectListArray.Length; i++)
			{
				Write(rifObjectListArray[i]);
			}
		}

		internal void Write(string[] strings)
		{
			if (strings == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteArrayStart(ObjectType.PrimitiveArray, strings.Length);
			for (int i = 0; i < strings.Length; i++)
			{
				Write(strings[i]);
			}
		}

		internal void Write(object[] array)
		{
			if (array == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteArrayStart(ObjectType.PrimitiveArray, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				Write(array[i]);
			}
		}

		internal void WriteVariantOrPersistableArray(object[] array)
		{
			if (array == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteArrayStart(ObjectType.PrimitiveArray, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				WriteVariantOrPersistable(array[i]);
			}
		}

		internal void WriteSerializableArray(object[] array)
		{
			if (array == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteArrayStart(ObjectType.SerializableArray, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				WriteSerializable(array[i]);
			}
		}

		internal void Write(IPersistable[] array)
		{
			if (array == null)
			{
				m_writer.WriteNull();
				return;
			}
			m_writer.WriteArrayStart(ObjectType.RIFObjectArray, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				Write(array[i]);
			}
		}

		internal void Write(IPersistable[,][] array)
		{
			if (array == null)
			{
				m_writer.WriteNull();
				return;
			}
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			m_writer.Write2DArrayStart(ObjectType.Array2D, length, length2);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					Write(array[i, j]);
				}
			}
		}

		internal void Write(IPersistable[,] array)
		{
			if (array == null)
			{
				m_writer.WriteNull();
				return;
			}
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			m_writer.Write2DArrayStart(ObjectType.Array2D, length, length2);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					Write(array[i, j]);
				}
			}
		}

		internal void Write(float[] array)
		{
			m_writer.Write(array);
		}

		internal void Write(int[] array)
		{
			m_writer.Write(array);
		}

		internal void Write(long[] array)
		{
			m_writer.Write(array);
		}

		internal void Write(char[] array)
		{
			((BinaryWriter)m_writer).Write(array);
		}

		internal void Write(byte[] array)
		{
			((BinaryWriter)m_writer).Write(array);
		}

		internal void Write(bool[] array)
		{
			m_writer.Write(array);
		}

		internal void Write(double[] array)
		{
			m_writer.Write(array);
		}

		internal void Write(DateTime dateTime)
		{
			m_writer.Write(dateTime, m_currentMember.Token);
		}

		internal void Write(DateTimeOffset dateTimeOffset)
		{
			m_writer.Write(dateTimeOffset);
		}

		internal void Write(TimeSpan timeSpan)
		{
			m_writer.Write(timeSpan);
		}

		internal void Write(Guid guid)
		{
			m_writer.Write(guid);
		}

		internal void Write(string value)
		{
			((BinaryWriter)m_writer).Write(value);
		}

		internal void Write(bool value)
		{
			((BinaryWriter)m_writer).Write(value);
		}

		internal void Write(short value)
		{
			((BinaryWriter)m_writer).Write(value);
		}

		internal void Write(int value)
		{
			((BinaryWriter)m_writer).Write(value);
		}

		internal void Write(long value)
		{
			((BinaryWriter)m_writer).Write(value);
		}

		internal void Write(ushort value)
		{
			((BinaryWriter)m_writer).Write(value);
		}

		internal void Write(uint value)
		{
			((BinaryWriter)m_writer).Write(value);
		}

		internal void Write(ulong value)
		{
			((BinaryWriter)m_writer).Write(value);
		}

		internal void Write(char value)
		{
			((BinaryWriter)m_writer).Write(value);
		}

		internal void Write(byte value)
		{
			((BinaryWriter)m_writer).Write(value);
		}

		internal void Write(sbyte value)
		{
			((BinaryWriter)m_writer).Write(value);
		}

		internal void Write(float value)
		{
			((BinaryWriter)m_writer).Write(value);
		}

		internal void Write(double value)
		{
			((BinaryWriter)m_writer).Write(value);
		}

		internal void Write(decimal value)
		{
			((BinaryWriter)m_writer).Write(value);
		}

		internal void Write7BitEncodedInt(int value)
		{
			m_writer.WriteEnum(value);
		}

		internal void WriteEnum(int value)
		{
			m_writer.WriteEnum(value);
		}

		internal void WriteNull()
		{
			m_writer.WriteNull();
		}

		internal void Write(CultureInfo threadCulture)
		{
			if (threadCulture != null)
			{
				((BinaryWriter)m_writer).Write(threadCulture.LCID);
			}
			else
			{
				((BinaryWriter)m_writer).Write(-1);
			}
		}

		private void WriteReferenceInList(IReferenceable referenceableItem)
		{
			WriteReferenceID(referenceableItem?.ID ?? (-2));
		}

		internal void WriteReference(IReferenceable referenceableItem)
		{
			WriteReferenceID(referenceableItem?.ID ?? (-1));
		}

		internal void WriteReferenceID(int referenceID)
		{
			if (referenceID == -1)
			{
				WriteNull();
				return;
			}
			m_writer.Write(m_currentMember.ObjectType);
			((BinaryWriter)m_writer).Write(referenceID);
		}

		private void WriteGlobalReferenceInList(IGloballyReferenceable globalReference)
		{
			WriteGlobalReferenceID(globalReference?.GlobalID ?? (-2));
		}

		internal void WriteGlobalReference(IGloballyReferenceable globalReference)
		{
			WriteGlobalReferenceID(globalReference?.GlobalID ?? (-1));
		}

		internal void WriteGlobalReferenceID(int globalReferenceID)
		{
			if (globalReferenceID == -1)
			{
				WriteNull();
				return;
			}
			m_writer.Write(m_currentMember.ObjectType);
			((BinaryWriter)m_writer).Write(globalReferenceID);
		}

		internal void Write(ObjectType type)
		{
			m_writer.Write(type);
		}

		internal bool CanWrite(object obj)
		{
			if (obj == null)
			{
				return true;
			}
			switch (Type.GetTypeCode(obj.GetType()))
			{
			case TypeCode.Empty:
			case TypeCode.DBNull:
			case TypeCode.Boolean:
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
			case TypeCode.DateTime:
			case TypeCode.String:
				return true;
			default:
				if (obj is IPersistable || obj is DateTimeOffset || obj is TimeSpan || obj is Guid || obj is byte[])
				{
					return true;
				}
				return false;
			}
		}

		internal void WriteSerializable(object obj)
		{
			if (!TryWriteSerializable(obj))
			{
				m_writer.Write(Token.Null);
			}
		}

		internal bool TryWriteSerializable(object obj)
		{
			if (!TryWrite(obj))
			{
				long position = m_writer.BaseStream.Position;
				try
				{
					if (ProhibitSerializableValues || (!(obj is ISerializable) && (obj.GetType().Attributes & TypeAttributes.Serializable) == 0))
					{
						return false;
					}
					if (m_binaryFormatter == null)
					{
						m_binaryFormatter = new BinaryFormatter();
					}
					m_writer.Write(Token.Serializable);
					m_binaryFormatter.Serialize(m_writer.BaseStream, obj);
				}
				catch (RSException)
				{
					throw;
				}
				catch (Exception ex2)
				{
					m_writer.BaseStream.Position = position;
					Global.Tracer.Trace(TraceLevel.Warning, "Error occurred during serialization: " + ex2.Message);
					return false;
				}
			}
			return true;
		}

		internal void WriteVariantOrPersistable(object obj)
		{
			IPersistable persistable = obj as IPersistable;
			if (persistable != null)
			{
				m_writer.Write(Token.Object);
				Write(persistable, verify: false);
			}
			else
			{
				Write(obj);
			}
		}

		internal void Write(object obj)
		{
			Write(obj, verify: true, assertOnInvalidType: true);
		}

		internal bool TryWrite(object obj)
		{
			return Write(obj, verify: true, assertOnInvalidType: false);
		}

		private bool Write(object obj, bool verify, bool assertOnInvalidType)
		{
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Expected O, but got Unknown
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Expected O, but got Unknown
			if (obj == null || obj == DBNull.Value)
			{
				m_writer.Write(Token.Null);
			}
			else
			{
				switch (Type.GetTypeCode(obj.GetType()))
				{
				case TypeCode.Empty:
				case TypeCode.DBNull:
					m_writer.Write(Token.Null);
					break;
				case TypeCode.Boolean:
					m_writer.Write(Token.Boolean);
					((BinaryWriter)m_writer).Write((bool)obj);
					break;
				case TypeCode.Byte:
					m_writer.Write(Token.Byte);
					((BinaryWriter)m_writer).Write((byte)obj);
					break;
				case TypeCode.Char:
					m_writer.Write(Token.Char);
					((BinaryWriter)m_writer).Write((char)obj);
					break;
				case TypeCode.DateTime:
				{
					DateTime dateTime = (DateTime)obj;
					Token token = m_currentMember.Token;
					if (token == Token.Object || token == Token.Serializable)
					{
						token = ((dateTime.Kind == DateTimeKind.Unspecified) ? Token.DateTime : Token.DateTimeWithKind);
					}
					m_writer.Write(token);
					m_writer.Write(dateTime, token);
					break;
				}
				case TypeCode.Decimal:
					m_writer.Write(Token.Decimal);
					((BinaryWriter)m_writer).Write((decimal)obj);
					break;
				case TypeCode.Double:
					m_writer.Write(Token.Double);
					((BinaryWriter)m_writer).Write((double)obj);
					break;
				case TypeCode.Int16:
					m_writer.Write(Token.Int16);
					((BinaryWriter)m_writer).Write((short)obj);
					break;
				case TypeCode.Int32:
					m_writer.Write(Token.Int32);
					((BinaryWriter)m_writer).Write((int)obj);
					break;
				case TypeCode.Int64:
					m_writer.Write(Token.Int64);
					((BinaryWriter)m_writer).Write((long)obj);
					break;
				case TypeCode.SByte:
					m_writer.Write(Token.SByte);
					((BinaryWriter)m_writer).Write((sbyte)obj);
					break;
				case TypeCode.Single:
					m_writer.Write(Token.Single);
					((BinaryWriter)m_writer).Write((float)obj);
					break;
				case TypeCode.String:
					m_writer.Write(Token.String);
					m_writer.Write((string)obj, writeObjType: false);
					break;
				case TypeCode.UInt16:
					m_writer.Write(Token.UInt16);
					((BinaryWriter)m_writer).Write((ushort)obj);
					break;
				case TypeCode.UInt32:
					m_writer.Write(Token.UInt32);
					((BinaryWriter)m_writer).Write((uint)obj);
					break;
				case TypeCode.UInt64:
					m_writer.Write(Token.UInt64);
					((BinaryWriter)m_writer).Write((ulong)obj);
					break;
				default:
					if (obj is TimeSpan)
					{
						m_writer.Write(Token.TimeSpan);
						m_writer.Write((TimeSpan)obj);
						break;
					}
					if (obj is DateTimeOffset)
					{
						m_writer.Write(Token.DateTimeOffset);
						m_writer.Write((DateTimeOffset)obj);
						break;
					}
					if (obj is Guid)
					{
						m_writer.Write(Token.Guid);
						m_writer.Write((Guid)obj);
						break;
					}
					if (obj is Enum)
					{
						Global.Tracer.Assert(condition: false, "You must call WriteEnum for enums");
						break;
					}
					if (obj is byte[])
					{
						m_writer.Write(Token.ByteArray);
						((BinaryWriter)m_writer).Write((byte[])obj);
						break;
					}
					if (assertOnInvalidType)
					{
						Global.Tracer.Assert(condition: false, "Unsupported object type: " + obj.GetType());
					}
					return false;
				}
			}
			return true;
		}
	}
}
