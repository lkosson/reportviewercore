using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal struct IntermediateFormatReader
	{
		private int m_currentMemberIndex;

		private Declaration m_currentPersistedDeclaration;

		private Dictionary<ObjectType, Declaration> m_readDecls;

		private PersistenceBinaryReader m_reader;

		private Dictionary<IPersistable, Dictionary<ObjectType, List<MemberReference>>> m_memberReferencesCollection;

		private Dictionary<int, IReferenceable> m_referenceableItems;

		private GlobalIDOwnerCollection m_globalIDOwners;

		private IRIFObjectCreator m_rifObjectCreator;

		private PersistenceHelper m_persistenceHelper;

		private IntermediateFormatVersion m_version;

		private long m_objectStartPosition;

		private PersistenceFlags m_persistenceFlags;

		private int m_currentMemberInfoCount;

		private MemberInfo m_currentMember;

		private int m_compatVersion;

		private BinaryFormatter m_binaryFormatter;

		internal bool CanSeek => HasPersistenceFlag(m_persistenceFlags, PersistenceFlags.Seekable);

		internal bool EOS => m_reader.EOS;

		internal IntermediateFormatVersion IntermediateFormatVersion => m_version;

		internal MemberInfo CurrentMember => m_currentMember;

		internal PersistenceHelper PersistenceHelper => m_persistenceHelper;

		internal long ObjectStartPosition => m_objectStartPosition;

		internal bool HasReferences
		{
			get
			{
				if (m_memberReferencesCollection != null)
				{
					return m_memberReferencesCollection.Count > 0;
				}
				return false;
			}
		}

		internal GlobalIDOwnerCollection GlobalIDOwners => m_globalIDOwners;

		internal IntermediateFormatReader(Stream str, IRIFObjectCreator rifObjectCreator)
			: this(str, rifObjectCreator, null, null)
		{
		}

		internal IntermediateFormatReader(Stream str, IRIFObjectCreator rifObjectCreator, PersistenceHelper persistenceHelper)
			: this(str, rifObjectCreator, null, persistenceHelper)
		{
		}

		internal IntermediateFormatReader(Stream str, IRIFObjectCreator rifObjectCreator, GlobalIDOwnerCollection globalIDOwnersFromOtherStream)
			: this(str, rifObjectCreator, globalIDOwnersFromOtherStream, null)
		{
		}

		internal IntermediateFormatReader(Stream str, IRIFObjectCreator rifObjectCreator, GlobalIDOwnerCollection globalIDOwnersFromOtherStream, PersistenceHelper persistenceHelper)
			: this(str, rifObjectCreator, globalIDOwnersFromOtherStream, persistenceHelper, null, null, PersistenceFlags.None, initFromStream: true, PersistenceConstants.CurrentCompatVersion)
		{
		}

		internal IntermediateFormatReader(Stream str, IRIFObjectCreator rifObjectCreator, GlobalIDOwnerCollection globalIDOwnersFromOtherStream, PersistenceHelper persistenceHelper, int compatVersion)
			: this(str, rifObjectCreator, globalIDOwnersFromOtherStream, persistenceHelper, null, null, PersistenceFlags.None, initFromStream: true, compatVersion)
		{
		}

		internal IntermediateFormatReader(Stream str, IRIFObjectCreator rifObjectCreator, GlobalIDOwnerCollection globalIDOwnersFromOtherStream, PersistenceHelper persistenceHelper, List<Declaration> declarations, IntermediateFormatVersion version, PersistenceFlags flags)
			: this(str, rifObjectCreator, globalIDOwnersFromOtherStream, persistenceHelper, declarations, version, flags, initFromStream: false, PersistenceConstants.CurrentCompatVersion)
		{
		}

		private IntermediateFormatReader(Stream str, IRIFObjectCreator rifObjectCreator, GlobalIDOwnerCollection globalIDOwnersFromOtherStream, PersistenceHelper persistenceHelper, List<Declaration> declarations, IntermediateFormatVersion version, PersistenceFlags flags, bool initFromStream, int compatVersion)
		{
			m_currentMemberIndex = -1;
			m_readDecls = new Dictionary<ObjectType, Declaration>(EqualityComparers.ObjectTypeComparerInstance);
			m_currentPersistedDeclaration = null;
			m_reader = new PersistenceBinaryReader(str);
			m_referenceableItems = new Dictionary<int, IReferenceable>(EqualityComparers.Int32ComparerInstance);
			m_memberReferencesCollection = new Dictionary<IPersistable, Dictionary<ObjectType, List<MemberReference>>>();
			m_rifObjectCreator = rifObjectCreator;
			m_persistenceHelper = persistenceHelper;
			m_version = null;
			m_persistenceFlags = PersistenceFlags.None;
			m_objectStartPosition = 0L;
			m_globalIDOwners = null;
			m_currentMemberInfoCount = 0;
			m_currentMember = null;
			m_binaryFormatter = null;
			m_compatVersion = compatVersion;
			if (globalIDOwnersFromOtherStream == null)
			{
				m_globalIDOwners = new GlobalIDOwnerCollection();
			}
			else
			{
				m_globalIDOwners = globalIDOwnersFromOtherStream;
			}
			if (initFromStream)
			{
				m_version = ReadIntermediateFormatVersion();
				m_persistenceFlags = (PersistenceFlags)m_reader.ReadEnum();
				if (HasPersistenceFlag(m_persistenceFlags, PersistenceFlags.CompatVersioned))
				{
					IncompatibleRIFVersionException.ThrowIfIncompatible(m_reader.ReadInt32(), m_compatVersion);
				}
				if (HasPersistenceFlag(m_persistenceFlags, PersistenceFlags.Seekable))
				{
					ReadDeclarations();
				}
			}
			else
			{
				m_version = version;
				m_persistenceFlags = flags;
				PrepareDeclarationsFromInitialization(declarations);
			}
			m_objectStartPosition = m_reader.StreamPosition;
		}

		private static bool HasPersistenceFlag(PersistenceFlags flags, PersistenceFlags flagToTest)
		{
			return (flags & flagToTest) != 0;
		}

		private void PrepareDeclarationsFromInitialization(List<Declaration> declarations)
		{
			for (int i = 0; i < declarations.Count; i++)
			{
				Declaration declaration = declarations[i].CreateFilteredDeclarationForWriteVersion(m_compatVersion);
				m_readDecls.Add(declaration.ObjectType, declaration);
			}
		}

		private void ReadDeclarations()
		{
			if (m_reader.ReadListStart(ObjectType.Declaration, out int listSize))
			{
				for (int i = 0; i < listSize; i++)
				{
					Declaration declaration = m_reader.ReadDeclaration();
					m_readDecls.Add(declaration.ObjectType, declaration);
				}
			}
		}

		internal void RegisterDeclaration(Declaration declaration)
		{
			m_currentMemberIndex = -1;
			if (!m_readDecls.TryGetValue(declaration.ObjectType, out m_currentPersistedDeclaration))
			{
				m_currentPersistedDeclaration = m_reader.ReadDeclaration();
				m_currentPersistedDeclaration.RegisterCurrentDeclaration(declaration);
				m_readDecls.Add(declaration.ObjectType, m_currentPersistedDeclaration);
			}
			else if (!m_currentPersistedDeclaration.RegisteredCurrentDeclaration)
			{
				m_currentPersistedDeclaration.RegisterCurrentDeclaration(declaration);
			}
			m_currentMemberInfoCount = m_currentPersistedDeclaration.MemberInfoList.Count;
		}

		internal bool NextMember()
		{
			m_currentMemberIndex++;
			if (m_currentMemberIndex < m_currentMemberInfoCount)
			{
				if (m_currentPersistedDeclaration.HasSkippedMembers && m_currentPersistedDeclaration.IsMemberSkipped(m_currentMemberIndex))
				{
					SkipMembers(m_currentPersistedDeclaration.MembersToSkip(m_currentMemberIndex));
					return NextMember();
				}
				m_currentMember = m_currentPersistedDeclaration.MemberInfoList[m_currentMemberIndex];
				return true;
			}
			return false;
		}

		internal void ResolveReferences()
		{
			foreach (KeyValuePair<IPersistable, Dictionary<ObjectType, List<MemberReference>>> item in m_memberReferencesCollection)
			{
				item.Key.ResolveReferences(item.Value, m_referenceableItems);
				item.Value.Clear();
			}
		}

		internal void ClearReferences()
		{
			m_referenceableItems = new Dictionary<int, IReferenceable>(EqualityComparers.Int32ComparerInstance);
			m_memberReferencesCollection = new Dictionary<IPersistable, Dictionary<ObjectType, List<MemberReference>>>();
		}

		private void SkipMembers(int toSkip)
		{
			for (int i = 0; i < toSkip; i++)
			{
				m_currentMember = m_currentPersistedDeclaration.MemberInfoList[m_currentMemberIndex];
				switch (m_currentMember.ObjectType)
				{
				case ObjectType.PrimitiveTypedArray:
					switch (m_currentMember.Token)
					{
					case Token.Byte:
					case Token.SByte:
						m_reader.SkipTypedArray(1);
						break;
					case Token.Char:
					case Token.Int16:
					case Token.UInt16:
						m_reader.SkipTypedArray(2);
						break;
					case Token.Int32:
					case Token.UInt32:
					case Token.Single:
						m_reader.SkipTypedArray(4);
						break;
					case Token.DateTime:
					case Token.TimeSpan:
					case Token.Int64:
					case Token.UInt64:
					case Token.Double:
						m_reader.SkipTypedArray(8);
						break;
					case Token.DateTimeOffset:
					case Token.Guid:
						m_reader.SkipBytes(16);
						break;
					case Token.Decimal:
						m_reader.ReadDecimal();
						break;
					default:
						Global.Tracer.Assert(condition: false);
						break;
					}
					break;
				case ObjectType.RIFObjectArray:
					if (m_currentMember.Token == Token.Reference)
					{
						SkipListOrArrayOfReferences();
					}
					else
					{
						SkipListOrArrayOfRIFObjects();
					}
					break;
				case ObjectType.RIFObjectList:
					if (m_currentMember.Token == Token.Reference)
					{
						SkipListOrArrayOfReferences();
					}
					else
					{
						SkipListOrArrayOfRIFObjects();
					}
					break;
				case ObjectType.PrimitiveArray:
					SkipArrayOfPrimitives();
					break;
				case ObjectType.PrimitiveList:
					SkipListOfPrimitives();
					break;
				case ObjectType.StringRIFObjectDictionary:
					SkipStringRIFObjectDictionary();
					break;
				case ObjectType.Int32PrimitiveListHashtable:
					SkipInt32PrimitiveListHashtable();
					break;
				case ObjectType.ObjectHashtableHashtable:
					SkipObjectHashtableHashtable();
					break;
				case ObjectType.StringObjectHashtable:
					SkipStringObjectHashtable();
					break;
				case ObjectType.Int32RIFObjectDictionary:
					SkipInt32RIFObjectDictionary();
					break;
				case ObjectType.None:
					SkipPrimitive(m_currentMember.Token);
					break;
				default:
					if (m_currentMember.Token == Token.Reference)
					{
						m_reader.SkipMultiByteInt();
					}
					else
					{
						SkipRIFObject();
					}
					break;
				case ObjectType.Null:
					break;
				}
				m_currentMemberIndex++;
			}
			m_currentMemberIndex--;
		}

		private void SkipPrimitive(Token token)
		{
			switch (token)
			{
			case Token.Null:
				break;
			case Token.String:
				m_reader.SkipString();
				break;
			case Token.Boolean:
			case Token.Byte:
			case Token.SByte:
				m_reader.SkipBytes(1);
				break;
			case Token.Char:
			case Token.Int16:
			case Token.UInt16:
				m_reader.SkipBytes(2);
				break;
			case Token.Int32:
			case Token.UInt32:
			case Token.Single:
				m_reader.SkipBytes(4);
				break;
			case Token.Reference:
			case Token.Enum:
				m_reader.SkipMultiByteInt();
				break;
			case Token.DateTime:
			case Token.TimeSpan:
			case Token.Int64:
			case Token.UInt64:
			case Token.Double:
				m_reader.SkipBytes(8);
				break;
			case Token.DateTimeOffset:
			case Token.Guid:
			case Token.Decimal:
				m_reader.SkipBytes(16);
				break;
			case Token.Object:
				SkipPrimitive(m_reader.ReadToken());
				break;
			default:
				Global.Tracer.Assert(condition: false);
				break;
			}
		}

		private void SkipArrayOfPrimitives()
		{
			if (m_reader.ReadArrayStart(m_currentMember.ObjectType, out int arraySize))
			{
				for (int i = 0; i < arraySize; i++)
				{
					SkipPrimitive(m_reader.ReadToken());
				}
			}
		}

		private void SkipListOfPrimitives()
		{
			if (m_reader.ReadListStart(m_currentMember.ObjectType, out int listSize))
			{
				for (int i = 0; i < listSize; i++)
				{
					SkipPrimitive(m_reader.ReadToken());
				}
			}
		}

		private void SkipListOrArrayOfReferences()
		{
			if (!m_reader.ReadListStart(m_currentMember.ObjectType, out int listSize))
			{
				return;
			}
			for (int i = 0; i < listSize; i++)
			{
				if (m_reader.ReadObjectType() != 0)
				{
					m_reader.SkipMultiByteInt();
				}
			}
		}

		private void SkipListOrArrayOfRIFObjects()
		{
			if (m_reader.ReadListStart(m_currentMember.ObjectType, out int listSize))
			{
				for (int i = 0; i < listSize; i++)
				{
					SkipRIFObject();
				}
			}
		}

		private void SkipRIFObject()
		{
			ObjectType objectType = m_reader.ReadObjectType();
			if (objectType != 0)
			{
				((IntermediateFormatReader)MemberwiseClone()).__SkipRIFObjectPrivate(objectType);
			}
		}

		private void __SkipRIFObjectPrivate(ObjectType objectType)
		{
			m_currentMemberIndex = 0;
			if (!m_readDecls.TryGetValue(objectType, out m_currentPersistedDeclaration))
			{
				m_currentPersistedDeclaration = m_reader.ReadDeclaration();
				m_readDecls.Add(objectType, m_currentPersistedDeclaration);
			}
			m_currentMemberInfoCount = m_currentPersistedDeclaration.MemberInfoList.Count;
			SkipMembers(m_currentMemberInfoCount);
		}

		private void SkipStringRIFObjectDictionary()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				for (int i = 0; i < dictionarySize; i++)
				{
					m_reader.SkipString();
					SkipRIFObject();
				}
			}
		}

		private void SkipInt32RIFObjectDictionary()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				for (int i = 0; i < dictionarySize; i++)
				{
					m_reader.SkipBytes(4);
					SkipRIFObject();
				}
			}
		}

		internal void SkipInt32PrimitiveListHashtable()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				for (int i = 0; i < dictionarySize; i++)
				{
					m_reader.SkipBytes(4);
					SkipListOfPrimitives();
				}
			}
		}

		internal void SkipStringObjectHashtable()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				for (int i = 0; i < dictionarySize; i++)
				{
					m_reader.SkipString();
					SkipPrimitive(m_reader.ReadToken());
				}
			}
		}

		internal void SkipObjectHashtableHashtable()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				for (int i = 0; i < dictionarySize; i++)
				{
					SkipPrimitive(m_reader.ReadToken());
					SkipObjectHashtableHashtable();
				}
			}
		}

		internal void Seek(long newPosition)
		{
			Seek(newPosition, SeekOrigin.Begin);
		}

		internal void Seek(long newPosition, SeekOrigin seekOrigin)
		{
			m_reader.Seek(newPosition, seekOrigin);
		}

		internal IPersistable ReadRIFObject()
		{
			return ReadRIFObject(verify: true);
		}

		internal IPersistable ReadRIFObject(IPersistable persitObj)
		{
			ObjectType objectType = ReadRIFObjectStart();
			if (objectType != 0)
			{
				persitObj.Deserialize(this);
				ReadRIFObjectFinish(objectType, persitObj, verify: false);
			}
			else
			{
				persitObj = null;
			}
			return persitObj;
		}

		internal T ReadRIFObject<T>() where T : IPersistable, new()
		{
			ObjectType objectType = ReadRIFObjectStart();
			T val = default(T);
			if (objectType != 0)
			{
				val = new T();
				val.Deserialize(this);
				ReadRIFObjectFinish(objectType, val, verify: false);
			}
			return val;
		}

		private IPersistable ReadRIFObject(bool verify)
		{
			ObjectType objectType = ReadRIFObjectStart();
			IPersistable persistable = null;
			if (objectType != 0)
			{
				persistable = m_rifObjectCreator.CreateRIFObject(objectType, ref this);
				AddReferenceableItem(persistable);
			}
			return persistable;
		}

		private void AddReferenceableItem(IPersistable persistObj)
		{
			IReferenceable referenceable = persistObj as IReferenceable;
			if (referenceable != null && referenceable.ID != -2)
			{
				m_referenceableItems.Add(referenceable.ID, referenceable);
			}
			IGlobalIDOwner globalIDOwner = persistObj as IGlobalIDOwner;
			if (globalIDOwner != null)
			{
				int num = globalIDOwner.GlobalID = m_globalIDOwners.GetGlobalID();
				IGloballyReferenceable globallyReferenceable = persistObj as IGloballyReferenceable;
				if (globallyReferenceable != null)
				{
					m_globalIDOwners.Add(globallyReferenceable);
				}
			}
		}

		private ObjectType ReadRIFObjectStart()
		{
			m_objectStartPosition = m_reader.StreamPosition;
			return m_reader.ReadObjectType();
		}

		private void ReadRIFObjectFinish(ObjectType persistedType, IPersistable persitObj, bool verify)
		{
			_ = (m_currentPersistedDeclaration != null && verify);
			if (persitObj is IReferenceable)
			{
				IReferenceable referenceable = (IReferenceable)persitObj;
				m_referenceableItems.Add(referenceable.ID, referenceable);
			}
		}

		internal Dictionary<string, TValue> ReadStringRIFObjectDictionary<TValue>() where TValue : IPersistable
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				Dictionary<string, TValue> dictionary = new Dictionary<string, TValue>(dictionarySize, EqualityComparers.StringComparerInstance);
				for (int i = 0; i < dictionarySize; i++)
				{
					dictionary.Add(ReadString(verify: false), (TValue)ReadRIFObject());
				}
				return dictionary;
			}
			return null;
		}

		internal Dictionary<int, TValue> ReadInt32RIFObjectDictionary<TValue>() where TValue : IPersistable
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				Dictionary<int, TValue> dictionary = new Dictionary<int, TValue>(dictionarySize, EqualityComparers.Int32ComparerInstance);
				for (int i = 0; i < dictionarySize; i++)
				{
					dictionary.Add(ReadInt32(verify: false), (TValue)ReadRIFObject());
				}
				return dictionary;
			}
			return null;
		}

		internal IDictionary ReadInt32RIFObjectDictionary<T>(CreateDictionary<T> dictionaryCreator) where T : IDictionary
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				IDictionary dictionary = dictionaryCreator(dictionarySize);
				for (int i = 0; i < dictionarySize; i++)
				{
					dictionary.Add(ReadInt32(verify: false), ReadRIFObject());
				}
				return dictionary;
			}
			return null;
		}

		internal T ReadInt32PrimitiveListHashtable<T, U>() where T : Hashtable, new()where U : struct
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				T val = new T();
				for (int i = 0; i < dictionarySize; i++)
				{
					val.Add(ReadInt32(verify: false), ReadListOfPrimitives<U>());
				}
				return val;
			}
			return null;
		}

		internal T ReadStringInt32Hashtable<T>() where T : IDictionary, new()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				T result = new T();
				for (int i = 0; i < dictionarySize; i++)
				{
					result.Add(ReadString(verify: false), ReadInt32());
				}
				return result;
			}
			return default(T);
		}

		internal T ReadByteVariantHashtable<T>() where T : IDictionary, new()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				T result = new T();
				for (int i = 0; i < dictionarySize; i++)
				{
					result.Add(ReadByte(verify: false), ReadVariant());
				}
				return result;
			}
			return default(T);
		}

		internal T ReadStringStringHashtable<T>() where T : IDictionary, new()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				T result = new T();
				for (int i = 0; i < dictionarySize; i++)
				{
					result.Add(ReadString(verify: false), ReadString(verify: false));
				}
				return result;
			}
			return default(T);
		}

		internal T ReadStringObjectHashtable<T>() where T : IDictionary, new()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				T result = new T();
				for (int i = 0; i < dictionarySize; i++)
				{
					result.Add(ReadString(verify: false), ReadVariant());
				}
				return result;
			}
			return default(T);
		}

		internal T ReadStringRIFObjectHashtable<T>() where T : IDictionary, new()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				T result = new T();
				for (int i = 0; i < dictionarySize; i++)
				{
					result.Add(ReadString(verify: false), ReadRIFObject());
				}
				return result;
			}
			return default(T);
		}

		internal Dictionary<string, List<string>> ReadStringListOfStringDictionary()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
				for (int i = 0; i < dictionarySize; i++)
				{
					dictionary.Add(ReadString(verify: false), ReadListOfPrimitives<string>());
				}
				return dictionary;
			}
			return null;
		}

		internal T ReadStringObjectHashtable<T>(CreateDictionary<T> createDictionary, Predicate<string> allowKey, Converter<string, string> processName, Converter<object, object> processValue) where T : IDictionary
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				T result = createDictionary(dictionarySize);
				for (int i = 0; i < dictionarySize; i++)
				{
					string text = ReadString(verify: false);
					object input = ReadVariant();
					if (allowKey(text))
					{
						result.Add(processName(text), processValue(input));
					}
				}
				return result;
			}
			return default(T);
		}

		internal Hashtable ReadObjectHashtableHashtable()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				Hashtable hashtable = new Hashtable();
				for (int i = 0; i < dictionarySize; i++)
				{
					hashtable.Add(ReadVariant(), ReadVariantVariantHashtable());
				}
				return hashtable;
			}
			return null;
		}

		internal Hashtable ReadNLevelVariantHashtable()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				Hashtable hashtable = new Hashtable();
				for (int i = 0; i < dictionarySize; i++)
				{
					object key = ReadVariant();
					Token token = m_reader.ReadToken();
					object value;
					switch (token)
					{
					case Token.Hashtable:
						value = ReadNLevelVariantHashtable();
						break;
					case Token.Object:
						value = ReadVariant();
						break;
					default:
						Global.Tracer.Assert(false, "Invalid token: {0} while reading NLevelVariantHashtable", token);
						value = null;
						break;
					}
					hashtable.Add(key, value);
				}
				return hashtable;
			}
			return null;
		}

		internal T ReadNameObjectCollection<T>() where T : class, INameObjectCollection, new()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				T val = new T();
				for (int i = 0; i < dictionarySize; i++)
				{
					val.Add(ReadString(verify: false), ReadVariant());
				}
				return val;
			}
			return null;
		}

		internal T? ReadNullable<T>() where T : struct
		{
			return (T?)ReadVariant();
		}

		internal Dictionary<T, string> ReadRIFObjectStringHashtable<T>() where T : IPersistable
		{
			return ReadRIFObjectStringHashtable<T>(null);
		}

		internal Dictionary<T, string> ReadRIFObjectStringHashtable<T>(Dictionary<T, string> dictionary) where T : IPersistable
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				if (dictionary == null)
				{
					dictionary = new Dictionary<T, string>(dictionarySize);
				}
				for (int i = 0; i < dictionarySize; i++)
				{
					dictionary.Add((T)ReadRIFObject(), ReadString(verify: false));
				}
			}
			return dictionary;
		}

		internal Hashtable ReadVariantVariantHashtable()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				Hashtable hashtable = new Hashtable();
				for (int i = 0; i < dictionarySize; i++)
				{
					hashtable.Add(ReadVariant(), ReadVariant());
				}
				return hashtable;
			}
			return null;
		}

		internal Dictionary<List<object>, object> ReadVariantListVariantDictionary()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				Dictionary<List<object>, object> dictionary = new Dictionary<List<object>, object>();
				for (int i = 0; i < dictionarySize; i++)
				{
					dictionary.Add(ReadListOfVariant<List<object>>(), ReadVariant());
				}
				return dictionary;
			}
			return null;
		}

		internal Dictionary<string, List<object>> ReadStringVariantListDictionary()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				Dictionary<string, List<object>> dictionary = new Dictionary<string, List<object>>();
				for (int i = 0; i < dictionarySize; i++)
				{
					dictionary.Add(ReadString(), ReadListOfVariant<List<object>>());
				}
				return dictionary;
			}
			return null;
		}

		internal Dictionary<string, bool[]> ReadStringBoolArrayDictionary()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				Dictionary<string, bool[]> dictionary = new Dictionary<string, bool[]>();
				for (int i = 0; i < dictionarySize; i++)
				{
					dictionary.Add(ReadString(verify: false), m_reader.ReadBooleanArray());
				}
				return dictionary;
			}
			return null;
		}

		internal Hashtable ReadInt32StringHashtable()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				Hashtable hashtable = new Hashtable();
				for (int i = 0; i < dictionarySize; i++)
				{
					hashtable.Add(ReadInt32(verify: false), ReadString(verify: false));
				}
				return hashtable;
			}
			return null;
		}

		internal T ReadVariantRIFObjectDictionary<T>(CreateDictionary<T> creator) where T : IDictionary, new()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				T result = creator(dictionarySize);
				for (int i = 0; i < dictionarySize; i++)
				{
					result.Add(ReadVariant(), ReadRIFObject());
				}
				return result;
			}
			return default(T);
		}

		internal T ReadVariantListOfRIFObjectDictionary<T, V>(CreateDictionary<T> creator) where T : IDictionary, new()where V : class, IList, new()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				T result = creator(dictionarySize);
				for (int i = 0; i < dictionarySize; i++)
				{
					result.Add(ReadVariant(), ReadListOfRIFObjects<V>());
				}
				return result;
			}
			return default(T);
		}

		internal Dictionary<int, object> Int32SerializableDictionary()
		{
			if (m_reader.ReadDictionaryStart(m_currentMember.ObjectType, out int dictionarySize))
			{
				Dictionary<int, object> dictionary = new Dictionary<int, object>();
				for (int i = 0; i < dictionarySize; i++)
				{
					dictionary.Add(ReadInt32(verify: false), ReadSerializable());
				}
				return dictionary;
			}
			return null;
		}

		internal T ReadListOfRIFObjects<T>() where T : class, IList, new()
		{
			if (m_reader.ReadListStart(m_currentMember.ObjectType, out int listSize))
			{
				T val = new T();
				for (int i = 0; i < listSize; i++)
				{
					val.Add(ReadRIFObject());
				}
				return val;
			}
			return null;
		}

		internal void ReadListOfRIFObjects(IList list)
		{
			if (m_reader.ReadListStart(m_currentMember.ObjectType, out int listSize))
			{
				for (int i = 0; i < listSize; i++)
				{
					list.Add(ReadRIFObject());
				}
			}
		}

		internal void ReadListOfRIFObjects<T>(Action<T> addRIFObject) where T : IPersistable
		{
			if (m_reader.ReadListStart(m_currentMember.ObjectType, out int listSize))
			{
				for (int i = 0; i < listSize; i++)
				{
					addRIFObject((T)ReadRIFObject());
				}
			}
		}

		internal List<T> ReadGenericListOfRIFObjects<T>() where T : IPersistable
		{
			if (m_reader.ReadListStart(m_currentMember.ObjectType, out int listSize))
			{
				List<T> list = new List<T>(listSize);
				for (int i = 0; i < listSize; i++)
				{
					list.Add((T)ReadRIFObject());
				}
				return list;
			}
			return null;
		}

		internal List<T> ReadGenericListOfRIFObjectsUsingNew<T>() where T : IPersistable, new()
		{
			if (m_reader.ReadListStart(m_currentMember.ObjectType, out int listSize))
			{
				List<T> list = new List<T>(listSize);
				for (int i = 0; i < listSize; i++)
				{
					list.Add(ReadRIFObject<T>());
				}
				return list;
			}
			return null;
		}

		internal List<T> ReadGenericListOfRIFObjects<T>(Action<T> action) where T : IPersistable
		{
			if (m_reader.ReadListStart(m_currentMember.ObjectType, out int listSize))
			{
				List<T> list = new List<T>(listSize);
				for (int i = 0; i < listSize; i++)
				{
					T val = (T)ReadRIFObject();
					action(val);
					list.Add(val);
				}
				return list;
			}
			return null;
		}

		internal List<List<T>> ReadListOfListsOfRIFObjects<T>() where T : IPersistable
		{
			if (m_reader.ReadListStart(m_currentMember.ObjectType, out int listSize))
			{
				List<List<T>> list = new List<List<T>>(listSize);
				for (int i = 0; i < listSize; i++)
				{
					List<T> item = ReadGenericListOfRIFObjects<T>();
					list.Add(item);
				}
				return list;
			}
			return null;
		}

		internal List<T[]> ReadListOfRIFObjectArrays<T>() where T : IPersistable
		{
			if (m_reader.ReadListStart(m_currentMember.ObjectType, out int listSize))
			{
				List<T[]> list = new List<T[]>(listSize);
				for (int i = 0; i < listSize; i++)
				{
					T[] item = ReadArrayOfRIFObjects<T>();
					list.Add(item);
				}
				return list;
			}
			return null;
		}

		internal List<T> ReadListOfPrimitives<T>()
		{
			if (m_reader.ReadListStart(ObjectType.PrimitiveList, out int listSize))
			{
				List<T> list = new List<T>(listSize);
				for (int i = 0; i < listSize; i++)
				{
					list.Add((T)ReadVariant());
				}
				return list;
			}
			return null;
		}

		internal List<List<T>[]> ReadListOfArrayOfListsOfPrimitives<T>()
		{
			if (m_reader.ReadListStart(ObjectType.PrimitiveList, out int listSize))
			{
				List<List<T>[]> list = new List<List<T>[]>(listSize);
				for (int i = 0; i < listSize; i++)
				{
					list.Add(ReadArrayOfListsOfPrimitives<T>());
				}
				return list;
			}
			return null;
		}

		internal T ReadListOfVariant<T>() where T : class, IList, new()
		{
			if (m_reader.ReadListStart(m_currentMember.ObjectType, out int listSize))
			{
				T val = new T();
				for (int i = 0; i < listSize; i++)
				{
					val.Add(ReadVariant());
				}
				return val;
			}
			return null;
		}

		internal List<T>[] ReadArrayOfListsOfPrimitives<T>()
		{
			if (m_reader.ReadArrayStart(ObjectType.PrimitiveArray, out int arraySize))
			{
				List<T>[] array = new List<T>[arraySize];
				for (int i = 0; i < arraySize; i++)
				{
					array[i] = ReadListOfPrimitives<T>();
				}
				return array;
			}
			return null;
		}

		internal List<T>[] ReadArrayOfRIFObjectLists<T>() where T : IPersistable
		{
			if (m_reader.ReadArrayStart(ObjectType.RIFObjectArray, out int arraySize))
			{
				List<T>[] array = new List<T>[arraySize];
				for (int i = 0; i < arraySize; i++)
				{
					array[i] = ReadGenericListOfRIFObjects<T>();
				}
				return array;
			}
			return null;
		}

		internal T[] ReadArrayOfRIFObjects<T>() where T : IPersistable
		{
			return ReadArrayOfRIFObjects<T>(verify: true);
		}

		private T[] ReadArrayOfRIFObjects<T>(bool verify) where T : IPersistable
		{
			if (m_reader.ReadArrayStart(ObjectType.RIFObjectArray, out int arraySize))
			{
				T[] array = new T[arraySize];
				for (int i = 0; i < arraySize; i++)
				{
					array[i] = (T)ReadRIFObject(verify);
				}
				return array;
			}
			return null;
		}

		internal T[,][] Read2DArrayOfArrayOfRIFObjects<T>() where T : IPersistable
		{
			int arrayXLength = -1;
			int arrayYLength = -1;
			if (m_reader.Read2DArrayStart(ObjectType.Array2D, out arrayXLength, out arrayYLength))
			{
				T[,][] array = new T[arrayXLength, arrayYLength][];
				for (int i = 0; i < arrayXLength; i++)
				{
					for (int j = 0; j < arrayYLength; j++)
					{
						array[i, j] = ReadArrayOfRIFObjects<T>(verify: false);
					}
				}
				return array;
			}
			return null;
		}

		internal T[,] Read2DArrayOfRIFObjects<T>() where T : IPersistable
		{
			int arrayXLength = -1;
			int arrayYLength = -1;
			if (m_reader.Read2DArrayStart(ObjectType.Array2D, out arrayXLength, out arrayYLength))
			{
				T[,] array = new T[arrayXLength, arrayYLength];
				for (int i = 0; i < arrayXLength; i++)
				{
					for (int j = 0; j < arrayYLength; j++)
					{
						array[i, j] = (T)ReadRIFObject();
					}
				}
				return array;
			}
			return null;
		}

		internal T[] ReadArrayOfRIFObjects<RIFT, T>(Converter<RIFT, T> convertRIFObject) where RIFT : IPersistable
		{
			if (m_reader.ReadArrayStart(ObjectType.RIFObjectArray, out int arraySize))
			{
				T[] array = new T[arraySize];
				for (int i = 0; i < arraySize; i++)
				{
					array[i] = convertRIFObject((RIFT)ReadRIFObject());
				}
				return array;
			}
			return null;
		}

		internal string[] ReadStringArray()
		{
			if (m_reader.ReadArrayStart(m_currentMember.ObjectType, out int arraySize))
			{
				string[] array = new string[arraySize];
				for (int i = 0; i < arraySize; i++)
				{
					array[i] = ReadString();
				}
				return array;
			}
			return null;
		}

		internal object[] ReadVariantArray()
		{
			if (m_reader.ReadArrayStart(m_currentMember.ObjectType, out int arraySize))
			{
				object[] array = new object[arraySize];
				for (int i = 0; i < arraySize; i++)
				{
					array[i] = ReadVariant();
				}
				return array;
			}
			return null;
		}

		internal object[] ReadSerializableArray()
		{
			if (m_reader.ReadArrayStart(m_currentMember.ObjectType, out int arraySize))
			{
				object[] array = new object[arraySize];
				for (int i = 0; i < arraySize; i++)
				{
					array[i] = ReadSerializable();
				}
				return array;
			}
			return null;
		}

		internal object ReadSerializable()
		{
			Token token = m_reader.ReadToken();
			if (token == Token.Serializable)
			{
				return ReadISerializable();
			}
			return ReadVariant(token);
		}

		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.SerializationFormatter)]
		private object ReadISerializable()
		{
			try
			{
				if (m_binaryFormatter == null)
				{
					m_binaryFormatter = new BinaryFormatter();
				}
				return m_binaryFormatter.Deserialize(m_reader.BaseStream);
			}
			catch (Exception innerException)
			{
				throw new RSException(ErrorCode.rsProcessingError, ErrorStrings.Keys.GetString(ErrorCode.rsProcessingError.ToString()), innerException, Global.Tracer, null);
			}
		}

		internal object ReadVariant()
		{
			Token token = m_reader.ReadToken();
			return ReadVariant(token);
		}

		private object ReadVariant(Token token)
		{
			switch (token)
			{
			case Token.Null:
				return null;
			case Token.String:
				return m_reader.ReadString(checkforNull: false);
			case Token.Char:
				return m_reader.ReadChar();
			case Token.Boolean:
				return m_reader.ReadBoolean();
			case Token.Int16:
				return m_reader.ReadInt16();
			case Token.Int32:
				return m_reader.ReadInt32();
			case Token.Int64:
				return m_reader.ReadInt64();
			case Token.UInt16:
				return m_reader.ReadUInt16();
			case Token.UInt32:
				return m_reader.ReadUInt32();
			case Token.UInt64:
				return m_reader.ReadUInt64();
			case Token.Byte:
				return m_reader.ReadByte();
			case Token.SByte:
				return m_reader.ReadSByte();
			case Token.Single:
				return m_reader.ReadSingle();
			case Token.Double:
				return m_reader.ReadDouble();
			case Token.Decimal:
				return m_reader.ReadDecimal();
			case Token.DateTime:
				return m_reader.ReadDateTime();
			case Token.DateTimeWithKind:
				return m_reader.ReadDateTimeWithKind();
			case Token.DateTimeOffset:
				return m_reader.ReadDateTimeOffset();
			case Token.TimeSpan:
				return m_reader.ReadTimeSpan();
			case Token.Guid:
				return m_reader.ReadGuid();
			case Token.ByteArray:
				return m_reader.ReadByteArray();
			case Token.Object:
				return ReadRIFObject(verify: false);
			default:
				Global.Tracer.Assert(condition: false);
				return null;
			}
		}

		internal int[] ReadInt32Array()
		{
			return m_reader.ReadInt32Array();
		}

		internal long[] ReadInt64Array()
		{
			return m_reader.ReadInt64Array();
		}

		internal float[] ReadSingleArray()
		{
			return m_reader.ReadFloatArray();
		}

		internal char[] ReadCharArray()
		{
			return m_reader.ReadCharArray();
		}

		internal byte[] ReadByteArray()
		{
			return m_reader.ReadByteArray();
		}

		internal bool[] ReadBooleanArray()
		{
			return m_reader.ReadBooleanArray();
		}

		internal double[] ReadDoubleArray()
		{
			return m_reader.ReadDoubleArray();
		}

		internal byte ReadByte()
		{
			return ReadByte(verify: true);
		}

		internal byte ReadByte(bool verify)
		{
			return m_reader.ReadByte();
		}

		internal sbyte ReadSByte()
		{
			return m_reader.ReadSByte();
		}

		internal char ReadChar()
		{
			return m_reader.ReadChar();
		}

		internal short ReadInt16()
		{
			return m_reader.ReadInt16();
		}

		internal ushort ReadUInt16()
		{
			return m_reader.ReadUInt16();
		}

		internal int ReadInt32()
		{
			return ReadInt32(verify: true);
		}

		private int ReadInt32(bool verify)
		{
			return m_reader.ReadInt32();
		}

		internal uint ReadUInt32()
		{
			return m_reader.ReadUInt32();
		}

		internal long ReadInt64()
		{
			return m_reader.ReadInt64();
		}

		internal ulong ReadUInt64()
		{
			return m_reader.ReadUInt64();
		}

		internal float ReadSingle()
		{
			return m_reader.ReadSingle();
		}

		internal double ReadDouble()
		{
			return m_reader.ReadDouble();
		}

		internal decimal ReadDecimal()
		{
			return m_reader.ReadDecimal();
		}

		internal string ReadString()
		{
			return ReadString(verify: true);
		}

		private string ReadString(bool verify)
		{
			return m_reader.ReadString();
		}

		internal bool ReadBoolean()
		{
			return m_reader.ReadBoolean();
		}

		internal DateTime ReadDateTime()
		{
			return m_reader.ReadDateTime();
		}

		internal DateTime ReadDateTimeWithKind()
		{
			return m_reader.ReadDateTimeWithKind();
		}

		internal DateTimeOffset ReadDateTimeOffset()
		{
			return m_reader.ReadDateTimeOffset();
		}

		internal TimeSpan ReadTimeSpan()
		{
			return m_reader.ReadTimeSpan();
		}

		internal int Read7BitEncodedInt()
		{
			return m_reader.ReadEnum();
		}

		internal int ReadEnum()
		{
			return m_reader.ReadEnum();
		}

		internal Guid ReadGuid()
		{
			return m_reader.ReadGuid();
		}

		internal CultureInfo ReadCultureInfo()
		{
			int num = m_reader.ReadInt32();
			if (num == -1)
			{
				return null;
			}
			return new CultureInfo(num, useUserOverride: false);
		}

		internal List<T> ReadGenericListOfReferences<T>(IPersistable obj) where T : IReferenceable
		{
			if (m_reader.ReadListStart(m_currentMember.ObjectType, out int listSize))
			{
				List<T> list = new List<T>();
				for (int i = 0; i < listSize; i++)
				{
					T val = ReadReference<T>(obj, delayReferenceResolution: false);
					if (val != null)
					{
						list.Add(val);
					}
				}
				return list;
			}
			return null;
		}

		internal int ReadListOfReferencesNoResolution(IPersistable obj)
		{
			if (m_reader.ReadListStart(m_currentMember.ObjectType, out int listSize))
			{
				for (int i = 0; i < listSize; i++)
				{
					ReadReference<IReferenceable>(obj, delayReferenceResolution: true);
				}
			}
			return listSize;
		}

		internal T ReadListOfReferences<T, U>(IPersistable obj) where T : class, IList, new()where U : IReferenceable
		{
			if (m_reader.ReadListStart(m_currentMember.ObjectType, out int listSize))
			{
				T val = new T();
				for (int i = 0; i < listSize; i++)
				{
					U val2 = ReadReference<U>(obj, delayReferenceResolution: false);
					if (val2 != null)
					{
						val.Add(val2);
					}
				}
				return val;
			}
			return null;
		}

		internal T ReadReference<T>(IPersistable obj) where T : IReferenceable
		{
			return ReadReference<T>(obj, delayReferenceResolution: false);
		}

		private T ReadReference<T>(IPersistable obj, bool delayReferenceResolution) where T : IReferenceable
		{
			if (m_reader.ReadReference(out int refID, out ObjectType _))
			{
				IReferenceable value = null;
				if (delayReferenceResolution || !m_referenceableItems.TryGetValue(refID, out value))
				{
					if (!m_memberReferencesCollection.TryGetValue(obj, out Dictionary<ObjectType, List<MemberReference>> value2))
					{
						value2 = new Dictionary<ObjectType, List<MemberReference>>(EqualityComparers.ObjectTypeComparerInstance);
						m_memberReferencesCollection.Add(obj, value2);
					}
					if (!value2.TryGetValue(m_currentPersistedDeclaration.ObjectType, out List<MemberReference> value3))
					{
						value3 = new List<MemberReference>();
						value2.Add(m_currentPersistedDeclaration.ObjectType, value3);
					}
					value3.Add(new MemberReference(m_currentMember.MemberName, refID));
				}
				return (T)value;
			}
			return default(T);
		}

		internal List<T> ReadGenericListOfGloablReferences<T>() where T : IGloballyReferenceable
		{
			if (m_reader.ReadListStart(m_currentMember.ObjectType, out int listSize))
			{
				List<T> list = new List<T>();
				for (int i = 0; i < listSize; i++)
				{
					T val = ReadGlobalReference<T>();
					if (val != null)
					{
						list.Add(val);
					}
				}
				return list;
			}
			return null;
		}

		internal T ReadListOfGloablReferences<T, U>() where T : class, IList, new()where U : IGloballyReferenceable
		{
			if (m_reader.ReadListStart(m_currentMember.ObjectType, out int listSize))
			{
				T val = new T();
				for (int i = 0; i < listSize; i++)
				{
					U val2 = ReadGlobalReference<U>();
					if (val2 != null)
					{
						val.Add(val2);
					}
				}
				return val;
			}
			return null;
		}

		internal T ReadGlobalReference<T>() where T : IGloballyReferenceable
		{
			IGloballyReferenceable referenceableItem = null;
			if (m_reader.ReadReference(out int refID, out ObjectType _))
			{
				m_globalIDOwners.TryGetValue(refID, out referenceableItem);
			}
			return (T)referenceableItem;
		}

		internal IntermediateFormatVersion ReadIntermediateFormatVersion()
		{
			long streamPosition = m_reader.StreamPosition;
			ObjectType objectType = m_reader.ReadObjectType();
			if (objectType != ObjectType.IntermediateFormatVersion)
			{
				throw new IncompatibleFormatVersionException(objectType, streamPosition);
			}
			IntermediateFormatVersion intermediateFormatVersion = new IntermediateFormatVersion();
			intermediateFormatVersion.Deserialize(this);
			return intermediateFormatVersion;
		}
	}
}
