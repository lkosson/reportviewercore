using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.ReportProcessing.Persistence
{
	internal sealed class IntermediateFormatReader
	{
		internal sealed class State
		{
			private DeclarationList m_oldDeclarations;

			private IntList[][] m_oldIndexesToSkip;

			private bool[][] m_isInOldDeclaration;

			internal static readonly State Current = new State(DeclarationList.Current);

			internal DeclarationList OldDeclarations => m_oldDeclarations;

			internal IntList[][] OldIndexesToSkip => m_oldIndexesToSkip;

			internal bool[][] IsInOldDeclaration => m_isInOldDeclaration;

			internal State()
			{
				m_oldDeclarations = new DeclarationList();
				Initialize();
			}

			private State(DeclarationList declarations)
			{
				m_oldDeclarations = declarations;
				Initialize();
			}

			private void Initialize()
			{
				m_oldIndexesToSkip = new IntList[DeclarationList.Current.Count][];
				m_isInOldDeclaration = new bool[DeclarationList.Current.Count][];
			}
		}

		private sealed class Indexes
		{
			internal int CurrentIndex;
		}

		private sealed class ReportServerBinaryReader
		{
			private sealed class BinaryReaderWrapper : BinaryReader
			{
				internal BinaryReaderWrapper(Stream stream)
					: base(stream, Encoding.Unicode)
				{
				}

				internal new int Read7BitEncodedInt()
				{
					return base.Read7BitEncodedInt();
				}

				public override byte ReadByte()
				{
					return (byte)BaseStream.ReadByte();
				}
			}

			internal delegate void DeclarationCallback(ObjectType objectType, Declaration declaration);

			private BinaryReaderWrapper m_binaryReader;

			private DeclarationCallback m_declarationCallback;

			private Token m_token;

			private ObjectType m_objectType = ObjectTypeDefault;

			private int m_referenceValue = ReferenceDefault;

			private int m_enumValue = EnumDefault;

			private int m_arrayLength = ArrayLengthDefault;

			private Guid m_guidValue = GuidDefault;

			private string m_stringValue = StringDefault;

			private DateTime m_dateTimeValue = DateTimeDefault;

			private TimeSpan m_timeSpanValue = TimeSpanDefault;

			private char m_charValue = CharDefault;

			private bool m_booleanValue = BooleanDefault;

			private short m_int16Value = Int16Default;

			private int m_int32Value = Int32Default;

			private long m_int64Value = Int64Default;

			private ushort m_uint16Value = UInt16Default;

			private uint m_uint32Value = UInt32Default;

			private ulong m_uint64Value = UInt64Default;

			private byte m_byteValue = ByteDefault;

			private sbyte m_sbyteValue = SByteDefault;

			private float m_singleValue = SingleDefault;

			private double m_doubleValue = DoubleDefault;

			private decimal m_decimalValue = DecimalDefault;

			private Token m_arrayType = ArrayTypeDefault;

			private byte[] m_bytesValue = BytesDefault;

			private int[] m_int32sValue = Int32sDefault;

			private char[] m_charsValue = CharsDefault;

			private float[] m_floatsValue = FloatsDefault;

			private static readonly ObjectType ObjectTypeDefault = ObjectType.None;

			private static readonly int ReferenceDefault = 0;

			private static readonly int EnumDefault = 0;

			private static readonly int ArrayLengthDefault = 0;

			private static readonly Guid GuidDefault = Guid.Empty;

			private static readonly string StringDefault = null;

			private static readonly DateTime DateTimeDefault = new DateTime(0L);

			private static readonly TimeSpan TimeSpanDefault = new TimeSpan(0L);

			private static readonly char CharDefault = '\0';

			private static readonly bool BooleanDefault = false;

			private static readonly short Int16Default = 0;

			private static readonly int Int32Default = 0;

			private static readonly long Int64Default = 0L;

			private static readonly ushort UInt16Default = 0;

			private static readonly uint UInt32Default = 0u;

			private static readonly ulong UInt64Default = 0uL;

			private static readonly byte ByteDefault = 0;

			private static readonly sbyte SByteDefault = 0;

			private static readonly float SingleDefault = 0f;

			private static readonly double DoubleDefault = 0.0;

			private static readonly decimal DecimalDefault = default(decimal);

			private static readonly Token ArrayTypeDefault = Token.Byte;

			private static readonly byte[] BytesDefault = null;

			private static readonly int[] Int32sDefault = null;

			private static readonly char[] CharsDefault = null;

			private static readonly float[] FloatsDefault = null;

			internal Token Token => m_token;

			internal ObjectType ObjectType => m_objectType;

			internal Token ArrayType => m_arrayType;

			internal int ArrayLength => m_arrayLength;

			internal int ReferenceValue => m_referenceValue;

			internal string StringValue => m_stringValue;

			internal char CharValue => m_charValue;

			internal char[] CharsValue => m_charsValue;

			internal bool BooleanValue => m_booleanValue;

			internal short Int16Value => m_int16Value;

			internal int Int32Value => m_int32Value;

			internal int[] Int32sValue => m_int32sValue;

			internal long Int64Value => m_int64Value;

			internal ushort UInt16Value => m_uint16Value;

			internal uint UInt32Value => m_uint32Value;

			internal ulong UInt64Value => m_uint64Value;

			internal byte ByteValue => m_byteValue;

			internal byte[] BytesValue => m_bytesValue;

			internal sbyte SByteValue => m_sbyteValue;

			internal float SingleValue => m_singleValue;

			internal double DoubleValue => m_doubleValue;

			internal decimal DecimalValue => m_decimalValue;

			internal DateTime DateTimeValue => m_dateTimeValue;

			internal TimeSpan TimeSpanValue => m_timeSpanValue;

			internal Guid GuidValue => m_guidValue;

			internal DataFieldStatus DataFieldInfo => (DataFieldStatus)m_enumValue;

			internal ReportServerBinaryReader(Stream stream, DeclarationCallback declarationCallback)
			{
				Assert(declarationCallback != null);
				m_binaryReader = new BinaryReaderWrapper(stream);
				m_declarationCallback = declarationCallback;
			}

			internal bool Read()
			{
				bool flag;
				for (flag = Advance(); flag && Token.Declaration == m_token; flag = Advance())
				{
				}
				return flag;
			}

			internal bool ReadNoTypeReference()
			{
				bool flag;
				for (flag = ReadNoTypeReferenceAdvance(); flag && Token.Declaration == m_token; flag = ReadNoTypeReferenceAdvance())
				{
				}
				return flag;
			}

			internal void ReadDeclaration()
			{
				Assert(Advance());
				Assert(Token.Declaration == m_token);
			}

			private bool Advance()
			{
				try
				{
					m_objectType = ObjectTypeDefault;
					m_token = UnsafeReadToken();
					switch (m_token)
					{
					case Token.Object:
						m_objectType = UnsafeReadObjectType();
						break;
					case Token.Reference:
						m_objectType = UnsafeReadObjectType();
						m_referenceValue = m_binaryReader.ReadInt32();
						break;
					case Token.Enum:
						m_enumValue = m_binaryReader.Read7BitEncodedInt();
						break;
					case Token.TypedArray:
					{
						m_arrayType = ReadToken();
						int num2 = m_binaryReader.Read7BitEncodedInt();
						if (Token.Byte == m_arrayType)
						{
							m_bytesValue = m_binaryReader.ReadBytes(num2);
						}
						else if (Token.Int32 == m_arrayType)
						{
							m_int32sValue = new int[num2];
							for (int j = 0; j < num2; j++)
							{
								m_int32sValue[j] = m_binaryReader.ReadInt32();
							}
						}
						else if (Token.Single == m_arrayType)
						{
							m_floatsValue = new float[num2];
							for (int k = 0; k < num2; k++)
							{
								m_floatsValue[k] = m_binaryReader.ReadSingle();
							}
						}
						else
						{
							Assert(Token.Char == m_arrayType);
							m_charsValue = m_binaryReader.ReadChars(num2);
						}
						break;
					}
					case Token.Array:
						m_arrayLength = m_binaryReader.Read7BitEncodedInt();
						break;
					case Token.Declaration:
					{
						ObjectType objectType = ReadObjectType();
						ObjectType baseType = ReadObjectType();
						int num = m_binaryReader.Read7BitEncodedInt();
						MemberInfoList memberInfoList = new MemberInfoList(num);
						for (int i = 0; i < num; i++)
						{
							memberInfoList.Add(new MemberInfo(ReadMemberName(), ReadToken(), ReadObjectType()));
						}
						Declaration declaration = new Declaration(baseType, memberInfoList);
						m_declarationCallback(objectType, declaration);
						break;
					}
					case Token.Guid:
					{
						byte[] array = m_binaryReader.ReadBytes(16);
						Assert(array != null);
						Assert(16 == array.Length);
						m_guidValue = new Guid(array);
						break;
					}
					case Token.String:
						m_stringValue = m_binaryReader.ReadString();
						break;
					case Token.DateTime:
						m_dateTimeValue = new DateTime(m_binaryReader.ReadInt64());
						break;
					case Token.TimeSpan:
						m_timeSpanValue = new TimeSpan(m_binaryReader.ReadInt64());
						break;
					case Token.Char:
						m_charValue = m_binaryReader.ReadChar();
						break;
					case Token.Boolean:
						m_booleanValue = m_binaryReader.ReadBoolean();
						break;
					case Token.Int16:
						m_int16Value = m_binaryReader.ReadInt16();
						break;
					case Token.Int32:
						m_int32Value = m_binaryReader.ReadInt32();
						break;
					case Token.Int64:
						m_int64Value = m_binaryReader.ReadInt64();
						break;
					case Token.UInt16:
						m_uint16Value = m_binaryReader.ReadUInt16();
						break;
					case Token.UInt32:
						m_uint32Value = m_binaryReader.ReadUInt32();
						break;
					case Token.UInt64:
						m_uint64Value = m_binaryReader.ReadUInt64();
						break;
					case Token.Byte:
						m_byteValue = m_binaryReader.ReadByte();
						break;
					case Token.SByte:
						m_sbyteValue = m_binaryReader.ReadSByte();
						break;
					case Token.Single:
						m_singleValue = m_binaryReader.ReadSingle();
						break;
					case Token.Double:
						m_doubleValue = m_binaryReader.ReadDouble();
						break;
					case Token.Decimal:
						m_decimalValue = m_binaryReader.ReadDecimal();
						break;
					case Token.DataFieldInfo:
						m_enumValue = m_binaryReader.Read7BitEncodedInt();
						break;
					default:
						Assert(condition: false);
						return false;
					case Token.Null:
					case Token.EndObject:
						break;
					}
					return true;
				}
				catch (IOException)
				{
					return false;
				}
			}

			internal bool ReadNoTypeReferenceAdvance()
			{
				try
				{
					m_token = UnsafeReadToken();
					Assert(Token.Reference == m_token || m_token == Token.Null);
					if (Token.Reference == m_token)
					{
						m_referenceValue = m_binaryReader.ReadInt32();
					}
					return true;
				}
				catch (IOException)
				{
					return false;
				}
			}

			internal byte[] ReadBytes()
			{
				Assert(Read());
				if (m_token == Token.Null)
				{
					return null;
				}
				Assert(Token.TypedArray == m_token);
				Assert(Token.Byte == m_arrayType);
				return m_bytesValue;
			}

			internal int[] ReadInt32s()
			{
				Assert(Read());
				if (m_token == Token.Null)
				{
					return null;
				}
				Assert(Token.TypedArray == m_token);
				Assert(Token.Int32 == m_arrayType);
				return m_int32sValue;
			}

			internal float[] ReadFloatArray()
			{
				Assert(Read());
				if (m_token == Token.Null)
				{
					return null;
				}
				Assert(Token.TypedArray == m_token);
				Assert(Token.Single == m_arrayType);
				return m_floatsValue;
			}

			internal Guid ReadGuid()
			{
				Assert(Read());
				Assert(Token.Guid == m_token);
				return m_guidValue;
			}

			internal string ReadString()
			{
				Assert(Read());
				if (m_token == Token.Null)
				{
					return null;
				}
				Assert(Token.String == m_token);
				return m_stringValue;
			}

			internal int ReadInt32()
			{
				Assert(Read());
				Assert(Token.Int32 == m_token);
				return m_int32Value;
			}

			internal long ReadInt64()
			{
				Assert(Read());
				Assert(Token.Int64 == m_token);
				return m_int64Value;
			}

			internal double ReadDouble()
			{
				Assert(Read());
				Assert(Token.Double == m_token);
				return m_doubleValue;
			}

			internal bool ReadBoolean()
			{
				Assert(Read());
				Assert(Token.Boolean == m_token);
				return m_booleanValue;
			}

			internal DateTime ReadDateTime()
			{
				Assert(Read());
				Assert(Token.DateTime == m_token);
				return m_dateTimeValue;
			}

			internal int ReadEnum()
			{
				Assert(Read());
				Assert(Token.Enum == m_token);
				return m_enumValue;
			}

			internal ObjectType ReadObject()
			{
				Assert(Read());
				Assert(Token.Object == m_token);
				return m_objectType;
			}

			internal void ReadEndObject()
			{
				Assert(Read());
				Assert(Token.EndObject == m_token);
			}

			internal int ReadArray()
			{
				Assert(Read());
				Assert(Token.Array == m_token);
				return m_arrayLength;
			}

			private Token UnsafeReadToken()
			{
				return (Token)m_binaryReader.ReadByte();
			}

			private Token ReadToken()
			{
				return (Token)m_binaryReader.ReadByte();
			}

			private ObjectType UnsafeReadObjectType()
			{
				return (ObjectType)m_binaryReader.Read7BitEncodedInt();
			}

			private ObjectType ReadObjectType()
			{
				return (ObjectType)m_binaryReader.Read7BitEncodedInt();
			}

			private MemberName ReadMemberName()
			{
				return (MemberName)m_binaryReader.Read7BitEncodedInt();
			}
		}

		private ReportServerBinaryReader m_reader;

		private Hashtable m_definitionObjects;

		private Hashtable m_instanceObjects;

		private Hashtable m_parametersDef;

		private Hashtable m_parametersInfo;

		private Hashtable m_matrixHeadingInstanceObjects;

		private State m_state;

		private bool m_expectDeclarations;

		private Stack<GroupingList> m_groupingsWithHideDuplicatesStack;

		private IntermediateFormatVersion m_intermediateFormatVersion;

		private ArrayList m_textboxesWithUserSort;

		private int m_currentUniqueName = -1;

		internal IntermediateFormatVersion IntermediateFormatVersion => m_intermediateFormatVersion;

		internal Hashtable DefinitionObjects => m_definitionObjects;

		internal Hashtable InstanceObjects => m_instanceObjects;

		internal Hashtable MatrixHeadingInstanceObjects => m_matrixHeadingInstanceObjects;

		internal State ReaderState => m_state;

		internal IntermediateFormatReader(Stream stream)
		{
			Initialize(stream);
			m_definitionObjects = null;
			m_instanceObjects = null;
			m_matrixHeadingInstanceObjects = null;
			m_state = new State();
			m_expectDeclarations = true;
		}

		internal IntermediateFormatReader(Stream stream, Hashtable instanceObjects)
		{
			Initialize(stream);
			m_definitionObjects = null;
			m_instanceObjects = instanceObjects;
			m_matrixHeadingInstanceObjects = null;
			m_state = new State();
			m_expectDeclarations = true;
		}

		internal IntermediateFormatReader(Stream stream, Hashtable instanceObjects, Hashtable definitionObjects, IntermediateFormatVersion intermediateFormatVersion)
		{
			Initialize(stream);
			m_intermediateFormatVersion = intermediateFormatVersion;
			m_definitionObjects = definitionObjects;
			m_instanceObjects = instanceObjects;
			m_matrixHeadingInstanceObjects = null;
			m_state = new State();
			m_expectDeclarations = true;
		}

		internal IntermediateFormatReader(Stream stream, Hashtable instanceObjects, IntermediateFormatVersion intermediateFormatVersion)
		{
			Initialize(stream);
			m_definitionObjects = null;
			m_instanceObjects = instanceObjects;
			m_intermediateFormatVersion = intermediateFormatVersion;
			m_matrixHeadingInstanceObjects = null;
			m_state = new State();
			m_expectDeclarations = true;
		}

		internal IntermediateFormatReader(Stream stream, IntermediateFormatVersion intermediateFormatVersion)
		{
			Initialize(stream);
			m_definitionObjects = null;
			m_instanceObjects = null;
			m_intermediateFormatVersion = intermediateFormatVersion;
			m_matrixHeadingInstanceObjects = null;
			m_state = new State();
			m_expectDeclarations = true;
		}

		internal IntermediateFormatReader(Stream stream, State state, Hashtable definitionObjects, IntermediateFormatVersion intermediateFormatVersion)
		{
			Initialize(stream);
			m_definitionObjects = definitionObjects;
			m_instanceObjects = null;
			m_intermediateFormatVersion = intermediateFormatVersion;
			m_matrixHeadingInstanceObjects = null;
			if (state == null)
			{
				m_state = State.Current;
			}
			else
			{
				m_state = state;
			}
			m_expectDeclarations = false;
		}

		internal IntermediateFormatReader(Stream stream, State state, IntermediateFormatVersion intermediateFormatVersion)
		{
			Initialize(stream);
			m_definitionObjects = null;
			m_instanceObjects = null;
			m_intermediateFormatVersion = intermediateFormatVersion;
			m_matrixHeadingInstanceObjects = null;
			if (state == null)
			{
				m_state = State.Current;
			}
			else
			{
				m_state = state;
			}
			m_expectDeclarations = false;
		}

		private void Initialize(Stream stream)
		{
			Assert(stream != null);
			m_reader = new ReportServerBinaryReader(stream, DeclarationCallback);
			Assert(VersionStamp.Validate(m_reader.ReadBytes()));
		}

		internal IntermediateFormatVersion ReadIntermediateFormatVersion()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.IntermediateFormatVersion;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			IntermediateFormatVersion intermediateFormatVersion = new IntermediateFormatVersion();
			if (PreRead(objectType, indexes))
			{
				intermediateFormatVersion.Major = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				intermediateFormatVersion.Minor = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				intermediateFormatVersion.Build = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return intermediateFormatVersion;
		}

		internal Report ReadReport(ReportItem parent)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Report;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			if (m_groupingsWithHideDuplicatesStack == null)
			{
				m_groupingsWithHideDuplicatesStack = new Stack<GroupingList>();
			}
			m_groupingsWithHideDuplicatesStack.Push(new GroupingList());
			if (m_textboxesWithUserSort == null)
			{
				m_textboxesWithUserSort = new ArrayList();
			}
			m_textboxesWithUserSort.Add(new TextBoxList());
			if (PreRead(objectType, indexes))
			{
				m_intermediateFormatVersion = ReadIntermediateFormatVersion();
			}
			if (m_intermediateFormatVersion == null)
			{
				m_intermediateFormatVersion = new IntermediateFormatVersion(8, 0, 673);
			}
			Guid guid = Guid.Empty;
			if (PreRead(objectType, indexes))
			{
				guid = m_reader.ReadGuid();
			}
			if (guid == Guid.Empty)
			{
				guid = Guid.NewGuid();
			}
			Report report = new Report(parent, m_intermediateFormatVersion, guid);
			ReadReportItemBase(report);
			if (PreRead(objectType, indexes))
			{
				report.Author = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				report.AutoRefresh = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				report.EmbeddedImages = ReadEmbeddedImageHashtable();
			}
			if (PreRead(objectType, indexes))
			{
				report.PageHeader = ReadPageSection(isHeader: true, report);
			}
			if (PreRead(objectType, indexes))
			{
				report.PageFooter = ReadPageSection(isHeader: false, report);
			}
			if (PreRead(objectType, indexes))
			{
				report.ReportItems = ReadReportItemCollection(report);
			}
			if (PreRead(objectType, indexes))
			{
				report.DataSources = ReadDataSourceList();
			}
			if (PreRead(objectType, indexes))
			{
				report.PageHeight = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				report.PageHeightValue = m_reader.ReadDouble();
			}
			if (PreRead(objectType, indexes))
			{
				report.PageWidth = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				report.PageWidthValue = m_reader.ReadDouble();
			}
			if (PreRead(objectType, indexes))
			{
				report.LeftMargin = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				report.LeftMarginValue = m_reader.ReadDouble();
			}
			if (PreRead(objectType, indexes))
			{
				report.RightMargin = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				report.RightMarginValue = m_reader.ReadDouble();
			}
			if (PreRead(objectType, indexes))
			{
				report.TopMargin = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				report.TopMarginValue = m_reader.ReadDouble();
			}
			if (PreRead(objectType, indexes))
			{
				report.BottomMargin = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				report.BottomMarginValue = m_reader.ReadDouble();
			}
			if (PreRead(objectType, indexes))
			{
				report.Columns = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				report.ColumnSpacing = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				report.ColumnSpacingValue = m_reader.ReadDouble();
			}
			if (PreRead(objectType, indexes))
			{
				report.PageAggregates = ReadDataAggregateInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				report.CompiledCode = m_reader.ReadBytes();
			}
			if (PreRead(objectType, indexes))
			{
				report.MergeOnePass = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				report.PageMergeOnePass = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				report.SubReportMergeTransactions = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				report.NeedPostGroupProcessing = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				report.HasPostSortAggregates = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				report.HasReportItemReferences = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				report.ShowHideType = ReadShowHideTypes();
			}
			if (PreRead(objectType, indexes))
			{
				report.ImageStreamNames = ReadImageStreamNames();
			}
			if (PreRead(objectType, indexes))
			{
				report.LastID = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				report.BodyID = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				report.SubReports = ReadSubReportList();
			}
			if (PreRead(objectType, indexes))
			{
				report.HasImageStreams = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				report.HasLabels = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				report.HasBookmarks = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				report.ParametersNotUsedInQuery = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				report.Parameters = ReadParameterDefList();
			}
			if (PreRead(objectType, indexes))
			{
				report.OneDataSetName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				report.CodeModules = ReadStringList();
			}
			if (PreRead(objectType, indexes))
			{
				report.CodeClasses = ReadCodeClassList();
			}
			if (PreRead(objectType, indexes) && m_intermediateFormatVersion.IsRS2000_WithSpecialRecursiveAggregates)
			{
				report.HasSpecialRecursiveAggregates = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				report.Language = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				report.DataTransform = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				report.DataSchema = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				report.DataElementStyleAttribute = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				report.Code = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				report.HasUserSortFilter = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				report.CompiledCodeGeneratedWithRefusedPermissions = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				report.InteractiveHeight = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				report.InteractiveHeightValue = m_reader.ReadDouble();
			}
			if (PreRead(objectType, indexes))
			{
				report.InteractiveWidth = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				report.InteractiveWidthValue = m_reader.ReadDouble();
			}
			if (PreRead(objectType, indexes))
			{
				report.NonDetailSortFiltersInScope = ReadInScopeSortFilterTable();
			}
			if (PreRead(objectType, indexes))
			{
				report.DetailSortFiltersInScope = ReadInScopeSortFilterTable();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			ResolveReportItemReferenceForGroupings(m_groupingsWithHideDuplicatesStack.Pop());
			ResolveUserSortReferenceForTextBoxes();
			return report;
		}

		private void ResolveReportItemReferenceForGroupings(GroupingList groupingsWithHideDuplicates)
		{
			if (groupingsWithHideDuplicates == null)
			{
				return;
			}
			for (int i = 0; i < groupingsWithHideDuplicates.Count; i++)
			{
				Grouping grouping = groupingsWithHideDuplicates[i];
				IntList hideDuplicatesReportItemIDs = grouping.HideDuplicatesReportItemIDs;
				Global.Tracer.Assert(hideDuplicatesReportItemIDs != null, "(null != reportItemIDs)");
				for (int j = 0; j < hideDuplicatesReportItemIDs.Count; j++)
				{
					IDOwner definitionObject = GetDefinitionObject(hideDuplicatesReportItemIDs[j]);
					Assert(definitionObject is ReportItem);
					grouping.AddReportItemWithHideDuplicates((ReportItem)definitionObject);
				}
				grouping.HideDuplicatesReportItemIDs = null;
			}
		}

		private void ResolveUserSortReferenceForTextBoxes()
		{
			Global.Tracer.Assert(m_textboxesWithUserSort != null && 0 < m_textboxesWithUserSort.Count && m_textboxesWithUserSort[m_textboxesWithUserSort.Count - 1] != null);
			TextBoxList textBoxList = (TextBoxList)m_textboxesWithUserSort[m_textboxesWithUserSort.Count - 1];
			for (int i = 0; i < textBoxList.Count; i++)
			{
				ISortFilterScope sortFilterScope = null;
				TextBox textBox = textBoxList[i];
				if (-1 != textBox.UserSort.SortExpressionScopeID)
				{
					IDOwner definitionObject = GetDefinitionObject(textBox.UserSort.SortExpressionScopeID);
					sortFilterScope = (definitionObject as ISortFilterScope);
					if (sortFilterScope == null)
					{
						Assert(definitionObject is ReportHierarchyNode);
						sortFilterScope = ((ReportHierarchyNode)definitionObject).Grouping;
					}
					textBox.UserSort.SortExpressionScope = sortFilterScope;
					textBox.UserSort.SortExpressionScopeID = -1;
				}
				IntList groupInSortTargetIDs = textBox.UserSort.GroupInSortTargetIDs;
				if (groupInSortTargetIDs != null)
				{
					textBox.UserSort.GroupsInSortTarget = new GroupingList(groupInSortTargetIDs.Count);
					for (int j = 0; j < groupInSortTargetIDs.Count; j++)
					{
						IDOwner definitionObject2 = GetDefinitionObject(groupInSortTargetIDs[j]);
						Assert(definitionObject2 is ReportHierarchyNode);
						textBox.UserSort.GroupsInSortTarget.Add(((ReportHierarchyNode)definitionObject2).Grouping);
					}
					textBox.UserSort.GroupInSortTargetIDs = null;
				}
				if (-1 != textBox.UserSort.SortTargetID)
				{
					IDOwner definitionObject3 = GetDefinitionObject(textBox.UserSort.SortTargetID);
					sortFilterScope = (definitionObject3 as ISortFilterScope);
					if (sortFilterScope == null)
					{
						Assert(definitionObject3 is ReportHierarchyNode);
						sortFilterScope = ((ReportHierarchyNode)definitionObject3).Grouping;
					}
					textBox.UserSort.SortTarget = sortFilterScope;
					textBox.UserSort.SortTargetID = -1;
				}
			}
			m_textboxesWithUserSort.RemoveAt(m_textboxesWithUserSort.Count - 1);
		}

		internal ReportSnapshot ReadReportSnapshot()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReportSnapshot;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ReportSnapshot reportSnapshot = new ReportSnapshot();
			if (PreRead(objectType, indexes))
			{
				reportSnapshot.ExecutionTime = m_reader.ReadDateTime();
			}
			if (PreRead(objectType, indexes))
			{
				reportSnapshot.Report = ReadReport(null);
			}
			if (PreRead(objectType, indexes))
			{
				reportSnapshot.Parameters = ReadParameterInfoCollection();
			}
			if (PreRead(objectType, indexes))
			{
				reportSnapshot.ReportInstance = ReadReportInstance(reportSnapshot.Report);
			}
			if (PreRead(objectType, indexes))
			{
				reportSnapshot.HasDocumentMap = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				reportSnapshot.HasShowHide = m_reader.ReadBoolean();
			}
			if (m_intermediateFormatVersion.IsRS2005_WithSpecialChunkSplit)
			{
				if (PreRead(objectType, indexes))
				{
					reportSnapshot.HasBookmarks = m_reader.ReadBoolean();
				}
			}
			else
			{
				OffsetInfo offsetInfo = ReadOffsetInfo();
				if (offsetInfo != null)
				{
					reportSnapshot.DocumentMapOffset = offsetInfo;
					reportSnapshot.HasDocumentMap = true;
				}
				offsetInfo = ReadOffsetInfo();
				if (offsetInfo != null)
				{
					reportSnapshot.ShowHideSenderInfoOffset = offsetInfo;
					reportSnapshot.HasShowHide = true;
				}
				reportSnapshot.ShowHideReceiverInfoOffset = ReadOffsetInfo();
				reportSnapshot.QuickFindOffset = ReadOffsetInfo();
				indexes.CurrentIndex++;
				reportSnapshot.HasBookmarks = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				reportSnapshot.HasImageStreams = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				reportSnapshot.RequestUserName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				reportSnapshot.ReportServerUrl = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				reportSnapshot.ReportFolder = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				reportSnapshot.Language = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				reportSnapshot.Warnings = ReadProcessingMessageList();
			}
			if (PreRead(objectType, indexes))
			{
				reportSnapshot.PageSectionOffsets = ReadInt64List();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return reportSnapshot;
		}

		internal Report ReadReportFromSnapshot(out DateTime executionTime)
		{
			executionTime = DateTime.Now;
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReportSnapshot;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			Report result = null;
			if (PreRead(objectType, indexes))
			{
				executionTime = m_reader.ReadDateTime();
			}
			if (PreRead(objectType, indexes))
			{
				result = ReadReport(null);
			}
			return result;
		}

		internal ParameterInfoCollection ReadSnapshotParameters()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReportSnapshot;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ParameterInfoCollection result = null;
			if (PreRead(objectType, indexes))
			{
				m_reader.ReadDateTime();
			}
			if (PreRead(objectType, indexes))
			{
				ReadReport(null);
			}
			if (PreRead(objectType, indexes))
			{
				result = ReadParameterInfoCollection();
			}
			return result;
		}

		internal DocumentMapNode ReadDocumentMapNode()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.DocumentMapNode;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			DocumentMapNode documentMapNode = new DocumentMapNode();
			ReadInstanceInfoBase(documentMapNode);
			if (PreRead(objectType, indexes))
			{
				documentMapNode.Id = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				documentMapNode.Label = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				documentMapNode.Page = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				documentMapNode.Children = ReadDocumentMapNodes();
			}
			if (m_intermediateFormatVersion != null && !m_intermediateFormatVersion.IsRS2005_WithSpecialChunkSplit)
			{
				documentMapNode.Page = m_reader.ReadInt32();
				indexes.CurrentIndex++;
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return documentMapNode;
		}

		internal DocumentMapNodeInfo ReadDocumentMapNodeInfo()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.DocumentMapNode;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			DocumentMapNode documentMapNode = new DocumentMapNode();
			ReadInstanceInfoBase(documentMapNode);
			DocumentMapNodeInfo[] children = null;
			if (PreRead(objectType, indexes))
			{
				documentMapNode.Id = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				documentMapNode.Label = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				children = ReadDocumentMapNodesInfo();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return new DocumentMapNodeInfo(documentMapNode, children);
		}

		internal bool FindDocumentMapNodePage(string documentMapId, ref int page)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return false;
			}
			ObjectType objectType = ObjectType.DocumentMapNode;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			DocumentMapNode documentMapNode = new DocumentMapNode();
			ReadInstanceInfoBase(documentMapNode);
			if (PreRead(objectType, indexes))
			{
				documentMapNode.Id = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				documentMapNode.Page = m_reader.ReadInt32();
			}
			if (documentMapId.Equals(documentMapNode.Id, StringComparison.Ordinal))
			{
				page = documentMapNode.Page + 1;
				return true;
			}
			documentMapNode = null;
			bool flag = false;
			if (PreRead(objectType, indexes))
			{
				flag = FindDocumentMapNodesPage(documentMapId, ref page);
			}
			if (!flag)
			{
				PostRead(objectType, indexes);
				m_reader.ReadEndObject();
			}
			return flag;
		}

		internal TokensHashtable ReadTokensHashtable()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.TokensHashtable == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			TokensHashtable tokensHashtable = new TokensHashtable(num);
			for (int i = 0; i < num; i++)
			{
				int tokenID = m_reader.ReadInt32();
				object tokenValue = ReadVariant();
				tokensHashtable.Add(tokenID, tokenValue);
			}
			m_reader.ReadEndObject();
			return tokensHashtable;
		}

		internal BookmarksHashtable ReadBookmarksHashtable()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.BookmarksHashtable == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			BookmarksHashtable bookmarksHashtable = new BookmarksHashtable(num);
			for (int i = 0; i < num; i++)
			{
				string bookmark = m_reader.ReadString();
				BookmarkInformation bookmarkInfo = ReadBookmarkInformation();
				bookmarksHashtable.Add(bookmark, bookmarkInfo);
			}
			m_reader.ReadEndObject();
			return bookmarksHashtable;
		}

		internal BookmarkInformation FindBookmarkIdInfo(string bookmarkId)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.BookmarksHashtable == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			for (int i = 0; i < num; i++)
			{
				string value = m_reader.ReadString();
				BookmarkInformation result = ReadBookmarkInformation();
				if (bookmarkId.Equals(value, StringComparison.Ordinal))
				{
					return result;
				}
			}
			return null;
		}

		internal DrillthroughInformation FindDrillthroughIdInfo(string drillthroughId)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			bool flag = false;
			TokensHashtable dataSetTokenIDs = null;
			if (ObjectType.ReportDrillthroughInfo == m_reader.ObjectType)
			{
				flag = true;
				dataSetTokenIDs = ReadTokensHashtable();
				Assert(m_reader.Read());
				if (m_reader.Token == Token.Null)
				{
					return null;
				}
			}
			Assert(ObjectType.DrillthroughHashtable == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			for (int i = 0; i < num; i++)
			{
				string value = m_reader.ReadString();
				DrillthroughInformation drillthroughInformation = ReadDrillthroughInformation(flag);
				if (drillthroughId.Equals(value, StringComparison.Ordinal))
				{
					if (flag)
					{
						drillthroughInformation.ResolveDataSetTokenIDs(dataSetTokenIDs);
					}
					return drillthroughInformation;
				}
			}
			return null;
		}

		internal SenderInformationHashtable ReadSenderInformationHashtable()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.SenderInformationHashtable == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			SenderInformationHashtable senderInformationHashtable = new SenderInformationHashtable(num);
			for (int i = 0; i < num; i++)
			{
				int key = m_reader.ReadInt32();
				SenderInformation sender = ReadSenderInformation();
				senderInformationHashtable.Add(key, sender);
			}
			m_reader.ReadEndObject();
			return senderInformationHashtable;
		}

		internal ReceiverInformationHashtable ReadReceiverInformationHashtable()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ReceiverInformationHashtable == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ReceiverInformationHashtable receiverInformationHashtable = new ReceiverInformationHashtable(num);
			for (int i = 0; i < num; i++)
			{
				int key = m_reader.ReadInt32();
				ReceiverInformation receiver = ReadReceiverInformation();
				receiverInformationHashtable.Add(key, receiver);
			}
			m_reader.ReadEndObject();
			return receiverInformationHashtable;
		}

		internal QuickFindHashtable ReadQuickFindHashtable()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.QuickFindHashtable == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			QuickFindHashtable quickFindHashtable = new QuickFindHashtable(num);
			for (int i = 0; i < num; i++)
			{
				int key = m_reader.ReadInt32();
				ReportItemInstance val = ReadReportItemInstanceReference();
				quickFindHashtable.Add(key, val);
			}
			m_reader.ReadEndObject();
			return quickFindHashtable;
		}

		internal SortFilterEventInfoHashtable ReadSortFilterEventInfoHashtable()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.SortFilterEventInfoHashtable == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			SortFilterEventInfoHashtable sortFilterEventInfoHashtable = new SortFilterEventInfoHashtable(num);
			for (int i = 0; i < num; i++)
			{
				int key = m_reader.ReadInt32();
				SortFilterEventInfo val = ReadSortFilterEventInfo(getDefinition: true);
				sortFilterEventInfoHashtable.Add(key, val);
			}
			m_reader.ReadEndObject();
			return sortFilterEventInfoHashtable;
		}

		internal List<PageSectionInstance> ReadPageSections(int requestedPageNumber, int startPage, PageSection headerDef, PageSection footerDef)
		{
			Assert(startPage >= 0);
			List<PageSectionInstance> list = null;
			int num = (requestedPageNumber + 1) * 2;
			int num2 = 2;
			if (startPage == 0)
			{
				Assert(m_reader.Read());
				if (m_reader.Token == Token.Null)
				{
					return null;
				}
				Assert(Token.Object == m_reader.Token);
				Assert(ObjectType.PageSectionInstanceList == m_reader.ObjectType);
				num2 = m_reader.ReadArray();
				if (requestedPageNumber < 0)
				{
					num = num2;
				}
				Assert(num2 % 2 == 0);
			}
			list = new List<PageSectionInstance>((requestedPageNumber < 0) ? num2 : 2);
			for (int i = startPage * 2; i < num; i++)
			{
				PageSection pageSectionDef = (i % 2 == 0) ? headerDef : footerDef;
				PageSectionInstance item = ReadPageSectionInstance(pageSectionDef);
				if (requestedPageNumber < 0)
				{
					list.Add(item);
				}
				else if (requestedPageNumber == i >> 1)
				{
					list.Add(item);
				}
			}
			return list;
		}

		internal ActionInstance ReadActionInstance(Action action)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ActionInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ActionInstance actionInstance = new ActionInstance();
			if (PreRead(objectType, indexes))
			{
				ActionItemList actionItemList = null;
				if (action != null)
				{
					actionItemList = action.ActionItems;
				}
				actionInstance.ActionItemsValues = ReadActionItemInstanceList(actionItemList);
			}
			if (PreRead(objectType, indexes))
			{
				actionInstance.StyleAttributeValues = ReadVariants();
			}
			if (PreRead(objectType, indexes))
			{
				actionInstance.UniqueName = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return actionInstance;
		}

		private ActionItemInstanceList ReadActionItemInstanceList(ActionItemList actionItemList)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ActionItemInstanceList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ActionItemInstanceList actionItemInstanceList = new ActionItemInstanceList(num);
			ActionItem actionItem = null;
			for (int i = 0; i < num; i++)
			{
				actionItem = null;
				if (actionItemList != null)
				{
					actionItem = actionItemList[i];
				}
				actionItemInstanceList.Add(ReadActionItemInstance(actionItem));
			}
			m_reader.ReadEndObject();
			return actionItemInstanceList;
		}

		internal ActionItemInstance ReadActionItemInstance(ActionItem actionItemDef)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ActionItemInstance actionItemInstance = new ActionItemInstance();
			if (m_intermediateFormatVersion.IsRS2005_WithMultipleActions)
			{
				ObjectType objectType = ObjectType.ActionItemInstance;
				Indexes indexes = new Indexes();
				Assert(Token.Object == m_reader.Token);
				Assert(objectType == m_reader.ObjectType);
				if (PreRead(objectType, indexes))
				{
					actionItemInstance.HyperLinkURL = m_reader.ReadString();
				}
				if (PreRead(objectType, indexes))
				{
					actionItemInstance.BookmarkLink = m_reader.ReadString();
				}
				if (PreRead(objectType, indexes))
				{
					actionItemInstance.DrillthroughReportName = m_reader.ReadString();
				}
				if (PreRead(objectType, indexes))
				{
					actionItemInstance.DrillthroughParametersValues = ReadVariants(isMultiValue: true, readNextToken: true);
				}
				if (PreRead(objectType, indexes))
				{
					actionItemInstance.DrillthroughParametersOmits = ReadBoolList();
				}
				if (PreRead(objectType, indexes))
				{
					actionItemInstance.Label = m_reader.ReadString();
				}
				if (actionItemDef != null && m_intermediateFormatVersion.IsRS2005_WithSharedDrillthroughParams)
				{
					ParameterValueList drillthroughParameters = actionItemDef.DrillthroughParameters;
					if (drillthroughParameters != null && drillthroughParameters.Count > 0)
					{
						ExpressionInfo expressionInfo = null;
						DataSet dataSet = null;
						for (int i = 0; i < drillthroughParameters.Count; i++)
						{
							expressionInfo = drillthroughParameters[i].Value;
							if (expressionInfo != null && expressionInfo.Type == ExpressionInfo.Types.Token)
							{
								dataSet = (m_definitionObjects[expressionInfo.IntValue] as DataSet);
								if (dataSet != null && dataSet.Query != null)
								{
									actionItemInstance.DrillthroughParametersValues[i] = dataSet.Query.RewrittenCommandText;
								}
							}
						}
					}
				}
				PostRead(objectType, indexes);
			}
			else
			{
				Assert(Token.Object == m_reader.Token);
				Assert(ObjectType.ActionInstance == m_reader.ObjectType);
				actionItemInstance.HyperLinkURL = m_reader.ReadString();
				actionItemInstance.BookmarkLink = m_reader.ReadString();
				actionItemInstance.DrillthroughReportName = m_reader.ReadString();
				actionItemInstance.DrillthroughParametersValues = ReadVariants(isMultiValue: true, readNextToken: true);
				actionItemInstance.DrillthroughParametersOmits = ReadBoolList();
			}
			m_reader.ReadEndObject();
			return actionItemInstance;
		}

		internal ReportItemColInstanceInfo ReadReportItemColInstanceInfo()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReportItemColInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ReportItemColInstanceInfo reportItemColInstanceInfo = new ReportItemColInstanceInfo();
			ReadInstanceInfoBase(reportItemColInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				reportItemColInstanceInfo.ChildrenNonComputedUniqueNames = ReadNonComputedUniqueNamess();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return reportItemColInstanceInfo;
		}

		internal ListContentInstanceInfo ReadListContentInstanceInfo()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ListContentInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ListContentInstanceInfo listContentInstanceInfo = new ListContentInstanceInfo();
			ReadInstanceInfoBase(listContentInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				listContentInstanceInfo.StartHidden = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				listContentInstanceInfo.Label = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				listContentInstanceInfo.CustomPropertyInstances = ReadDataValueInstanceList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return listContentInstanceInfo;
		}

		internal MatrixHeadingInstanceInfo ReadMatrixHeadingInstanceInfoBase()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			if (ObjectType.MatrixHeadingInstanceInfo == m_reader.ObjectType)
			{
				MatrixHeadingInstanceInfo matrixHeadingInstanceInfo = new MatrixHeadingInstanceInfo();
				ReadMatrixHeadingInstanceInfo(matrixHeadingInstanceInfo);
				return matrixHeadingInstanceInfo;
			}
			Assert(ObjectType.MatrixSubtotalHeadingInstanceInfo == m_reader.ObjectType);
			MatrixSubtotalHeadingInstanceInfo matrixSubtotalHeadingInstanceInfo = new MatrixSubtotalHeadingInstanceInfo();
			ReadMatrixSubtotalHeadingInstanceInfo(matrixSubtotalHeadingInstanceInfo);
			return matrixSubtotalHeadingInstanceInfo;
		}

		internal void ReadMatrixHeadingInstanceInfo(MatrixHeadingInstanceInfo instanceInfo)
		{
			Assert(m_reader.Token != Token.Null);
			ObjectType objectType = ObjectType.MatrixHeadingInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ReadInstanceInfoBase(instanceInfo);
			if (PreRead(objectType, indexes))
			{
				instanceInfo.ContentUniqueNames = ReadNonComputedUniqueNames();
			}
			if (PreRead(objectType, indexes))
			{
				instanceInfo.StartHidden = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				instanceInfo.HeadingCellIndex = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				instanceInfo.HeadingSpan = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				instanceInfo.GroupExpressionValue = ReadVariant();
			}
			if (PreRead(objectType, indexes))
			{
				instanceInfo.Label = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				instanceInfo.CustomPropertyInstances = ReadDataValueInstanceList();
			}
			if (m_intermediateFormatVersion.IsRS2000_RTM_orNewer && m_intermediateFormatVersion.IsRS2005_IDW9_orOlder)
			{
				indexes.CurrentIndex++;
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
		}

		internal void ReadMatrixSubtotalHeadingInstanceInfo(MatrixSubtotalHeadingInstanceInfo instanceInfo)
		{
			Assert(m_reader.Token != Token.Null);
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.MatrixSubtotalHeadingInstanceInfo == m_reader.ObjectType);
			ReadInstanceInfoBase(instanceInfo);
			Assert(m_reader.Read());
			ReadMatrixHeadingInstanceInfo(instanceInfo);
			instanceInfo.StyleAttributeValues = ReadVariants();
			m_reader.ReadEndObject();
		}

		internal MatrixCellInstanceInfo ReadMatrixCellInstanceInfo()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.MatrixCellInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			MatrixCellInstanceInfo matrixCellInstanceInfo = new MatrixCellInstanceInfo();
			ReadInstanceInfoBase(matrixCellInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				matrixCellInstanceInfo.ContentUniqueNames = ReadNonComputedUniqueNames();
			}
			if (PreRead(objectType, indexes))
			{
				matrixCellInstanceInfo.RowIndex = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				matrixCellInstanceInfo.ColumnIndex = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return matrixCellInstanceInfo;
		}

		internal ChartHeadingInstanceInfo ReadChartHeadingInstanceInfo()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ChartHeadingInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ChartHeadingInstanceInfo chartHeadingInstanceInfo = new ChartHeadingInstanceInfo();
			ReadInstanceInfoBase(chartHeadingInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				chartHeadingInstanceInfo.HeadingLabel = ReadVariant();
			}
			if (PreRead(objectType, indexes))
			{
				chartHeadingInstanceInfo.HeadingCellIndex = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				chartHeadingInstanceInfo.HeadingSpan = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				chartHeadingInstanceInfo.GroupExpressionValue = ReadVariant();
			}
			if (PreRead(objectType, indexes))
			{
				chartHeadingInstanceInfo.StaticGroupingIndex = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				chartHeadingInstanceInfo.CustomPropertyInstances = ReadDataValueInstanceList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return chartHeadingInstanceInfo;
		}

		internal ChartDataPointInstanceInfo ReadChartDataPointInstanceInfo(ChartDataPointList chartDataPoints)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ChartDataPointInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ChartDataPointInstanceInfo chartDataPointInstanceInfo = new ChartDataPointInstanceInfo();
			ReadInstanceInfoBase(chartDataPointInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				chartDataPointInstanceInfo.DataPointIndex = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				chartDataPointInstanceInfo.DataValues = ReadVariants();
			}
			if (PreRead(objectType, indexes))
			{
				chartDataPointInstanceInfo.DataLabelValue = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				chartDataPointInstanceInfo.DataLabelStyleAttributeValues = ReadVariants();
			}
			if (PreRead(objectType, indexes))
			{
				if (m_intermediateFormatVersion.IsRS2005_WithMultipleActions)
				{
					ChartDataPoint chartDataPoint = chartDataPoints[chartDataPointInstanceInfo.DataPointIndex];
					chartDataPointInstanceInfo.Action = ReadActionInstance(chartDataPoint.Action);
				}
				else
				{
					ActionItemInstance actionItemInstance = ReadActionItemInstance(null);
					if (actionItemInstance != null)
					{
						chartDataPointInstanceInfo.Action = new ActionInstance(actionItemInstance);
					}
				}
			}
			if (PreRead(objectType, indexes))
			{
				chartDataPointInstanceInfo.StyleAttributeValues = ReadVariants();
			}
			if (PreRead(objectType, indexes))
			{
				chartDataPointInstanceInfo.MarkerStyleAttributeValues = ReadVariants();
			}
			if (PreRead(objectType, indexes))
			{
				chartDataPointInstanceInfo.CustomPropertyInstances = ReadDataValueInstanceList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return chartDataPointInstanceInfo;
		}

		internal TableGroupInstanceInfo ReadTableGroupInstanceInfo()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableGroupInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			TableGroupInstanceInfo tableGroupInstanceInfo = new TableGroupInstanceInfo();
			ReadInstanceInfoBase(tableGroupInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				tableGroupInstanceInfo.StartHidden = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				tableGroupInstanceInfo.Label = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				tableGroupInstanceInfo.CustomPropertyInstances = ReadDataValueInstanceList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return tableGroupInstanceInfo;
		}

		internal TableRowInstanceInfo ReadTableRowInstanceInfo()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableRowInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			TableRowInstanceInfo tableRowInstanceInfo = new TableRowInstanceInfo();
			ReadInstanceInfoBase(tableRowInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				tableRowInstanceInfo.StartHidden = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return tableRowInstanceInfo;
		}

		internal LineInstanceInfo ReadLineInstanceInfo(Line line)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.LineInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			LineInstanceInfo lineInstanceInfo = new LineInstanceInfo(line);
			ReadReportItemInstanceInfoBase(lineInstanceInfo);
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return lineInstanceInfo;
		}

		internal TextBoxInstanceInfo ReadTextBoxInstanceInfo(TextBox textBox)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TextBoxInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			TextBoxInstanceInfo textBoxInstanceInfo = new TextBoxInstanceInfo(textBox);
			ReadReportItemInstanceInfoBase(textBoxInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				textBoxInstanceInfo.FormattedValue = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				textBoxInstanceInfo.OriginalValue = ReadVariant();
			}
			bool flag = false;
			if (m_intermediateFormatVersion.IsRS2000_WithUnusedFieldsOptimization)
			{
				flag = true;
			}
			if ((!flag || textBox.HideDuplicates != null) && PreRead(objectType, indexes))
			{
				textBoxInstanceInfo.Duplicate = m_reader.ReadBoolean();
			}
			if ((!flag || textBox.Action != null) && PreRead(objectType, indexes))
			{
				if (m_intermediateFormatVersion.IsRS2005_WithMultipleActions)
				{
					textBoxInstanceInfo.Action = ReadActionInstance(textBox.Action);
				}
				else
				{
					ActionItemInstance actionItemInstance = ReadActionItemInstance(null);
					if (actionItemInstance != null)
					{
						textBoxInstanceInfo.Action = new ActionInstance(actionItemInstance);
					}
				}
			}
			if ((!flag || textBox.InitialToggleState != null) && PreRead(objectType, indexes))
			{
				textBoxInstanceInfo.InitialToggleState = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return textBoxInstanceInfo;
		}

		internal SimpleTextBoxInstanceInfo ReadSimpleTextBoxInstanceInfo(TextBox textBox)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.SimpleTextBoxInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			SimpleTextBoxInstanceInfo simpleTextBoxInstanceInfo = new SimpleTextBoxInstanceInfo(textBox);
			ReadInstanceInfoBase(simpleTextBoxInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				simpleTextBoxInstanceInfo.FormattedValue = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				simpleTextBoxInstanceInfo.OriginalValue = ReadVariant();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return simpleTextBoxInstanceInfo;
		}

		internal RectangleInstanceInfo ReadRectangleInstanceInfo(Rectangle rectangle)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.RectangleInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			RectangleInstanceInfo rectangleInstanceInfo = new RectangleInstanceInfo(rectangle);
			ReadReportItemInstanceInfoBase(rectangleInstanceInfo);
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return rectangleInstanceInfo;
		}

		internal CheckBoxInstanceInfo ReadCheckBoxInstanceInfo(CheckBox checkBox)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.CheckBoxInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			CheckBoxInstanceInfo checkBoxInstanceInfo = new CheckBoxInstanceInfo(checkBox);
			ReadReportItemInstanceInfoBase(checkBoxInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				checkBoxInstanceInfo.Value = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				checkBoxInstanceInfo.Duplicate = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return checkBoxInstanceInfo;
		}

		internal ImageInstanceInfo ReadImageInstanceInfo(Image image)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ImageInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ImageInstanceInfo imageInstanceInfo = new ImageInstanceInfo(image);
			ReadReportItemInstanceInfoBase(imageInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				imageInstanceInfo.ImageValue = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				if (m_intermediateFormatVersion.IsRS2005_WithMultipleActions)
				{
					imageInstanceInfo.Action = ReadActionInstance(image.Action);
				}
				else
				{
					ActionItemInstance actionItemInstance = ReadActionItemInstance(null);
					if (actionItemInstance != null)
					{
						imageInstanceInfo.Action = new ActionInstance(actionItemInstance);
					}
				}
			}
			if (PreRead(objectType, indexes))
			{
				imageInstanceInfo.BrokenImage = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				imageInstanceInfo.ImageMapAreas = ReadImageMapAreaInstanceList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return imageInstanceInfo;
		}

		internal SubReportInstanceInfo ReadSubReportInstanceInfo(SubReport subReport)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.SubReportInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			SubReportInstanceInfo subReportInstanceInfo = new SubReportInstanceInfo(subReport);
			ReadReportItemInstanceInfoBase(subReportInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				subReportInstanceInfo.NoRows = m_reader.ReadString();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return subReportInstanceInfo;
		}

		internal ActiveXControlInstanceInfo ReadActiveXControlInstanceInfo(ActiveXControl activeXControl)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ActiveXControlInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ActiveXControlInstanceInfo activeXControlInstanceInfo = new ActiveXControlInstanceInfo(activeXControl);
			ReadReportItemInstanceInfoBase(activeXControlInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				activeXControlInstanceInfo.ParameterValues = ReadVariants();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return activeXControlInstanceInfo;
		}

		internal ListInstanceInfo ReadListInstanceInfo(List list)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ListInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ListInstanceInfo listInstanceInfo = new ListInstanceInfo(list);
			ReadReportItemInstanceInfoBase(listInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				listInstanceInfo.NoRows = m_reader.ReadString();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return listInstanceInfo;
		}

		internal MatrixInstanceInfo ReadMatrixInstanceInfo(Matrix matrix)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.MatrixInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			MatrixInstanceInfo matrixInstanceInfo = new MatrixInstanceInfo(matrix);
			ReadReportItemInstanceInfoBase(matrixInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				matrixInstanceInfo.CornerNonComputedNames = ReadNonComputedUniqueNames();
			}
			if (PreRead(objectType, indexes))
			{
				matrixInstanceInfo.NoRows = m_reader.ReadString();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return matrixInstanceInfo;
		}

		internal TableInstanceInfo ReadTableInstanceInfo(Table table)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			TableInstanceInfo tableInstanceInfo = new TableInstanceInfo(table);
			ReadReportItemInstanceInfoBase(tableInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				tableInstanceInfo.ColumnInstances = ReadTableColumnInstances();
			}
			if (PreRead(objectType, indexes))
			{
				tableInstanceInfo.NoRows = m_reader.ReadString();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return tableInstanceInfo;
		}

		internal OWCChartInstanceInfo ReadOWCChartInstanceInfo(OWCChart chart)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.OWCChartInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			OWCChartInstanceInfo oWCChartInstanceInfo = new OWCChartInstanceInfo(chart);
			ReadReportItemInstanceInfoBase(oWCChartInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				oWCChartInstanceInfo.ChartData = ReadVariantLists(convertDBNull: false);
			}
			if (PreRead(objectType, indexes))
			{
				oWCChartInstanceInfo.NoRows = m_reader.ReadString();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return oWCChartInstanceInfo;
		}

		internal ChartInstanceInfo ReadChartInstanceInfo(Chart chart)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ChartInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ChartInstanceInfo chartInstanceInfo = new ChartInstanceInfo(chart);
			ReadReportItemInstanceInfoBase(chartInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				chartInstanceInfo.CategoryAxis = ReadAxisInstance();
			}
			if (PreRead(objectType, indexes))
			{
				chartInstanceInfo.ValueAxis = ReadAxisInstance();
			}
			if (PreRead(objectType, indexes))
			{
				chartInstanceInfo.Title = ReadChartTitleInstance();
			}
			if (PreRead(objectType, indexes))
			{
				chartInstanceInfo.PlotAreaStyleAttributeValues = ReadVariants();
			}
			if (PreRead(objectType, indexes))
			{
				chartInstanceInfo.LegendStyleAttributeValues = ReadVariants();
			}
			if (PreRead(objectType, indexes))
			{
				chartInstanceInfo.CultureName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				chartInstanceInfo.NoRows = m_reader.ReadString();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return chartInstanceInfo;
		}

		internal CustomReportItemInstanceInfo ReadCustomReportItemInstanceInfo(CustomReportItem cri)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.CustomReportItemInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			CustomReportItemInstanceInfo customReportItemInstanceInfo = new CustomReportItemInstanceInfo(cri);
			ReadReportItemInstanceInfoBase(customReportItemInstanceInfo);
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return customReportItemInstanceInfo;
		}

		internal PageSectionInstanceInfo ReadPageSectionInstanceInfo(PageSection pageSectionDef)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.PageSectionInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			PageSectionInstanceInfo pageSectionInstanceInfo = new PageSectionInstanceInfo(pageSectionDef);
			ReadReportItemInstanceInfoBase(pageSectionInstanceInfo);
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return pageSectionInstanceInfo;
		}

		internal ReportInstanceInfo ReadReportInstanceInfo(Report report)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReportInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ReportInstanceInfo reportInstanceInfo = new ReportInstanceInfo(report);
			ReadReportItemInstanceInfoBase(reportInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				reportInstanceInfo.Parameters = ReadParameterInfoCollection();
			}
			if (PreRead(objectType, indexes))
			{
				reportInstanceInfo.ReportName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				reportInstanceInfo.NoRows = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				reportInstanceInfo.BodyUniqueName = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return reportInstanceInfo;
		}

		internal RecordSetInfo ReadRecordSetInfo()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.RecordSetInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			RecordSetInfo recordSetInfo = new RecordSetInfo();
			if (PreRead(objectType, indexes))
			{
				recordSetInfo.ReaderExtensionsSupported = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				recordSetInfo.FieldPropertyNames = ReadRecordSetPropertyNamesList();
			}
			if (PreRead(objectType, indexes))
			{
				recordSetInfo.CompareOptions = ReadCompareOptions();
				recordSetInfo.ValidCompareOptions = true;
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return recordSetInfo;
		}

		private CompareOptions ReadCompareOptions()
		{
			return (CompareOptions)m_reader.ReadEnum();
		}

		private RecordSetPropertyNamesList ReadRecordSetPropertyNamesList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.RecordSetPropertyNamesList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			RecordSetPropertyNamesList recordSetPropertyNamesList = new RecordSetPropertyNamesList(num);
			for (int i = 0; i < num; i++)
			{
				recordSetPropertyNamesList.Add(ReadRecordSetPropertyNames());
			}
			m_reader.ReadEndObject();
			return recordSetPropertyNamesList;
		}

		internal RecordSetPropertyNames ReadRecordSetPropertyNames()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.RecordSetPropertyNames;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			RecordSetPropertyNames recordSetPropertyNames = new RecordSetPropertyNames();
			if (PreRead(objectType, indexes))
			{
				recordSetPropertyNames.PropertyNames = ReadStringList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return recordSetPropertyNames;
		}

		internal RecordRow ReadRecordRow()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.RecordRow;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			RecordRow recordRow = new RecordRow();
			if (PreRead(objectType, indexes))
			{
				recordRow.RecordFields = ReadRecordFields();
			}
			if (PreRead(objectType, indexes))
			{
				recordRow.IsAggregateRow = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				recordRow.AggregationFieldCount = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return recordRow;
		}

		private static void Assert(bool condition)
		{
			if (!condition)
			{
				Global.Tracer.Assert(condition: false);
				throw new ReportProcessingException(ErrorCode.rsUnexpectedError);
			}
		}

		private void RegisterParameterDef(ParameterDef paramDef)
		{
			Assert(paramDef != null);
			if (m_parametersDef == null)
			{
				m_parametersDef = new Hashtable();
			}
			else
			{
				Assert(!m_parametersDef.ContainsKey(paramDef.Name));
			}
			m_parametersDef.Add(paramDef.Name, paramDef);
		}

		private ParameterDef GetParameterDefObject(string name)
		{
			Assert(m_parametersDef != null);
			ParameterDef parameterDef = (ParameterDef)m_parametersDef[name];
			Assert(parameterDef != null);
			return parameterDef;
		}

		private void RegisterParameterInfo(ParameterInfo paramInfo)
		{
			Assert(paramInfo != null);
			if (m_parametersInfo == null)
			{
				m_parametersInfo = new Hashtable();
			}
			else
			{
				Assert(!m_parametersInfo.ContainsKey(paramInfo.Name));
			}
			m_parametersInfo.Add(paramInfo.Name, paramInfo);
		}

		private ParameterInfo GetParameterInfoObject(string name)
		{
			Assert(m_parametersInfo != null);
			ParameterInfo parameterInfo = (ParameterInfo)m_parametersInfo[name];
			Assert(parameterInfo != null);
			return parameterInfo;
		}

		private void RegisterDefinitionObject(IDOwner idOwner)
		{
			Assert(idOwner != null);
			if (m_definitionObjects == null)
			{
				m_definitionObjects = new Hashtable();
			}
			else
			{
				Assert(!m_definitionObjects.ContainsKey(idOwner.ID));
			}
			m_definitionObjects.Add(idOwner.ID, idOwner);
		}

		private IDOwner GetDefinitionObject(int id)
		{
			Assert(m_definitionObjects != null);
			IDOwner iDOwner = (IDOwner)m_definitionObjects[id];
			Assert(iDOwner != null);
			return iDOwner;
		}

		private void RegisterInstanceObject(ReportItemInstance reportItemInstance)
		{
			Assert(reportItemInstance != null);
			if (m_instanceObjects == null)
			{
				m_instanceObjects = new Hashtable();
			}
			else
			{
				Assert(!m_instanceObjects.ContainsKey(reportItemInstance.UniqueName));
			}
			m_instanceObjects.Add(reportItemInstance.UniqueName, reportItemInstance);
		}

		private ReportItemInstance GetInstanceObject(int uniqueName)
		{
			Assert(m_instanceObjects != null);
			ReportItemInstance reportItemInstance = (ReportItemInstance)m_instanceObjects[uniqueName];
			Assert(reportItemInstance != null);
			return reportItemInstance;
		}

		private void RegisterMatrixHeadingInstanceObject(MatrixHeadingInstance matrixHeadingInstance)
		{
			Assert(matrixHeadingInstance != null);
			if (m_matrixHeadingInstanceObjects == null)
			{
				m_matrixHeadingInstanceObjects = new Hashtable();
			}
			else
			{
				Assert(!m_matrixHeadingInstanceObjects.ContainsKey(matrixHeadingInstance.UniqueName));
			}
			m_matrixHeadingInstanceObjects.Add(matrixHeadingInstance.UniqueName, matrixHeadingInstance);
		}

		private MatrixHeadingInstance GetMatrixHeadingInstanceObject(int uniqueName)
		{
			Assert(m_matrixHeadingInstanceObjects != null);
			MatrixHeadingInstance matrixHeadingInstance = (MatrixHeadingInstance)m_matrixHeadingInstanceObjects[uniqueName];
			Assert(matrixHeadingInstance != null);
			return matrixHeadingInstance;
		}

		private void DeclarationCallback(ObjectType objectType, Declaration declaration)
		{
			Assert(m_expectDeclarations);
			Assert(declaration != null);
			Assert(declaration.Members != null);
			bool flag = false;
			if (m_intermediateFormatVersion != null && !m_intermediateFormatVersion.IsRS2005_WithTableOptimizations && ObjectType.TableGroupInstance == objectType)
			{
				flag = true;
			}
			Hashtable hashtable = new Hashtable();
			for (int i = 0; i < declaration.Members.Count; i++)
			{
				MemberInfo memberInfo = declaration.Members[i];
				if (flag && memberInfo.MemberName == MemberName.DetailRowInstances)
				{
					memberInfo.MemberName = MemberName.TableDetailInstances;
				}
				hashtable[memberInfo.MemberName] = i;
			}
			Declaration declaration2 = DeclarationList.Current[objectType];
			Assert(declaration2 != null);
			Assert(declaration2.Members != null);
			int lastIndex = -1;
			bool[] array = new bool[declaration2.Members.Count];
			IntList[] array2 = new IntList[declaration2.Members.Count + 1];
			for (int j = 0; j < declaration2.Members.Count; j++)
			{
				if (!hashtable.ContainsKey(declaration2.Members[j].MemberName))
				{
					continue;
				}
				int num = (int)hashtable[declaration2.Members[j].MemberName];
				bool flag2 = MemberInfo.Equals(declaration2.Members[j], declaration.Members[num]);
				if (!flag2)
				{
					if (declaration2.Members[j].ObjectType == ObjectType.ExpressionInfo)
					{
						flag2 = (declaration.Members[num].ObjectType == ObjectType.None && (declaration.Members[num].Token == Token.String || declaration.Members[num].Token == Token.Boolean || declaration.Members[num].Token == Token.Int32));
					}
					if (!flag2 && objectType == ObjectType.Axis && declaration2.Members[j].ObjectType == ObjectType.ExpressionInfo)
					{
						flag2 = (declaration.Members[num].ObjectType == ObjectType.Variant);
					}
				}
				if (flag2)
				{
					array[j] = true;
					array2[j] = CreateOldIndexesToSkip(num, lastIndex);
					lastIndex = num;
				}
			}
			array2[declaration2.Members.Count] = CreateOldIndexesToSkip(declaration.Members.Count, lastIndex);
			Assert(m_state.OldDeclarations[objectType] == null);
			Assert(m_state.IsInOldDeclaration[(int)objectType] == null);
			Assert(m_state.OldIndexesToSkip[(int)objectType] == null);
			m_state.OldDeclarations[objectType] = declaration;
			m_state.IsInOldDeclaration[(int)objectType] = array;
			m_state.OldIndexesToSkip[(int)objectType] = array2;
		}

		private IntList CreateOldIndexesToSkip(int index, int lastIndex)
		{
			Assert(index > lastIndex);
			IntList intList = null;
			if (index - lastIndex > 1)
			{
				intList = new IntList();
				for (int i = lastIndex + 1; i < index; i++)
				{
					intList.Add(i);
				}
			}
			return intList;
		}

		private bool PreRead(ObjectType objectType, Indexes indexes)
		{
			PostRead(objectType, indexes);
			bool result = IsInOldDeclaration(objectType, indexes);
			indexes.CurrentIndex++;
			return result;
		}

		private void PostRead(ObjectType objectType, Indexes indexes)
		{
			while (!m_state.OldDeclarations.ContainsKey(objectType))
			{
				m_reader.ReadDeclaration();
			}
			Skip(objectType, indexes);
		}

		private void Skip(ObjectType objectType, Indexes indexes)
		{
			IntList[] array = m_state.OldIndexesToSkip[(int)objectType];
			if (array == null || array.Length <= indexes.CurrentIndex)
			{
				return;
			}
			IntList intList = array[indexes.CurrentIndex];
			if (intList != null)
			{
				for (int i = 0; i < intList.Count; i++)
				{
					ReadRemovedItemType(m_state.OldDeclarations[objectType].Members[intList[i]].Token, m_state.OldDeclarations[objectType].Members[intList[i]].ObjectType);
				}
			}
		}

		private void ReadRemovedItemType(Token token, ObjectType objectType)
		{
			switch (token)
			{
			case Token.Object:
				switch (objectType)
				{
				case ObjectType.DataAggregateInfoList:
					ReadDataAggregateInfoList();
					return;
				case ObjectType.TableGroup:
					Global.Tracer.Assert(!m_intermediateFormatVersion.IsRS2005_WithTableDetailFix);
					return;
				}
				break;
			case Token.String:
				m_reader.ReadString();
				return;
			case Token.Int32:
				m_reader.ReadInt32();
				return;
			case Token.Reference:
				switch (objectType)
				{
				case ObjectType.ReportItem:
					ReadReportItemReference(getDefinition: false);
					return;
				case ObjectType.List:
				case ObjectType.MatrixHeading:
				case ObjectType.TableGroup:
				case ObjectType.TableRow:
				case ObjectType.ReportItemCollection:
				case ObjectType.TableDetail:
					ReadRemovedReference();
					return;
				case ObjectType.ChartHeading:
					if (m_intermediateFormatVersion.IsRS2000_RTM_orOlder)
					{
						ReadRemovedReference();
					}
					return;
				}
				break;
			}
			Assert(condition: false);
		}

		private void ReadRemovedReference()
		{
			Assert(m_reader.Read());
		}

		private bool IsInOldDeclaration(ObjectType objectType, Indexes indexes)
		{
			bool[] array = m_state.IsInOldDeclaration[(int)objectType];
			if (array != null)
			{
				return array[indexes.CurrentIndex];
			}
			return true;
		}

		private ValidValueList ReadValidValueList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ValidValueList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ValidValueList validValueList = new ValidValueList(num);
			for (int i = 0; i < num; i++)
			{
				validValueList.Add(ReadValidValue());
			}
			m_reader.ReadEndObject();
			return validValueList;
		}

		private ParameterDefList ReadParameterDefList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ParameterDefList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ParameterDefList parameterDefList = new ParameterDefList(num);
			m_parametersDef = null;
			for (int i = 0; i < num; i++)
			{
				parameterDefList.Add(ReadParameterDef());
			}
			m_reader.ReadEndObject();
			return parameterDefList;
		}

		private ParameterDefList ReadParameterDefRefList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ParameterDefList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ParameterDefList parameterDefList = new ParameterDefList(num);
			for (int i = 0; i < num; i++)
			{
				parameterDefList.Add(GetParameterDefObject(m_reader.ReadString()));
			}
			m_reader.ReadEndObject();
			return parameterDefList;
		}

		private ParameterInfoCollection ReadParameterInfoCollection()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ParameterInfoCollection == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection(num);
			m_parametersInfo = null;
			for (int i = 0; i < num; i++)
			{
				parameterInfoCollection.Add(ReadParameterInfo());
			}
			m_reader.ReadEndObject();
			return parameterInfoCollection;
		}

		private ParameterInfoCollection ReadParameterInfoRefCollection()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ParameterInfoCollection == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection(num);
			for (int i = 0; i < num; i++)
			{
				parameterInfoCollection.Add(GetParameterInfoObject(m_reader.ReadString()));
			}
			m_reader.ReadEndObject();
			return parameterInfoCollection;
		}

		private FilterList ReadFilterList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.FilterList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			FilterList filterList = new FilterList(num);
			for (int i = 0; i < num; i++)
			{
				filterList.Add(ReadFilter());
			}
			m_reader.ReadEndObject();
			return filterList;
		}

		private DataSourceList ReadDataSourceList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.DataSourceList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			DataSourceList dataSourceList = new DataSourceList(num);
			for (int i = 0; i < num; i++)
			{
				dataSourceList.Add(ReadDataSource());
			}
			m_reader.ReadEndObject();
			return dataSourceList;
		}

		private DataAggregateInfoList ReadDataAggregateInfoList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.DataAggregateInfoList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			DataAggregateInfoList dataAggregateInfoList = new DataAggregateInfoList(num);
			for (int i = 0; i < num; i++)
			{
				dataAggregateInfoList.Add(ReadDataAggregateInfo());
			}
			m_reader.ReadEndObject();
			return dataAggregateInfoList;
		}

		private ReportItemList ReadReportItemList(ReportItem parent)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ReportItemList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ReportItemList reportItemList = new ReportItemList(num);
			for (int i = 0; i < num; i++)
			{
				reportItemList.Add(ReadReportItem(parent));
			}
			m_reader.ReadEndObject();
			return reportItemList;
		}

		private ReportItemIndexerList ReadReportItemIndexerList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ReportItemIndexerList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ReportItemIndexerList reportItemIndexerList = new ReportItemIndexerList(num);
			for (int i = 0; i < num; i++)
			{
				reportItemIndexerList.Add(ReadReportItemIndexer());
			}
			m_reader.ReadEndObject();
			return reportItemIndexerList;
		}

		private RunningValueInfoList ReadRunningValueInfoList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.RunningValueInfoList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			RunningValueInfoList runningValueInfoList = new RunningValueInfoList(num);
			for (int i = 0; i < num; i++)
			{
				runningValueInfoList.Add(ReadRunningValueInfo());
			}
			m_reader.ReadEndObject();
			return runningValueInfoList;
		}

		private StyleAttributeHashtable ReadStyleAttributeHashtable()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.StyleAttributeHashtable == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			StyleAttributeHashtable styleAttributeHashtable = new StyleAttributeHashtable(num);
			for (int i = 0; i < num; i++)
			{
				string key = m_reader.ReadString();
				AttributeInfo value = ReadAttributeInfo();
				styleAttributeHashtable.Add(key, value);
			}
			m_reader.ReadEndObject();
			return styleAttributeHashtable;
		}

		private ImageInfo ReadImageInfo()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ImageInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ImageInfo imageInfo = new ImageInfo();
			if (PreRead(objectType, indexes))
			{
				imageInfo.StreamName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				imageInfo.MimeType = m_reader.ReadString();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return imageInfo;
		}

		private DrillthroughParameters ReadDrillthroughParameters()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.DrillthroughParameters == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			DrillthroughParameters drillthroughParameters = new DrillthroughParameters(num);
			for (int i = 0; i < num; i++)
			{
				string key = m_reader.ReadString();
				object value = ReadMultiValue();
				drillthroughParameters.Add(key, value);
			}
			m_reader.ReadEndObject();
			return drillthroughParameters;
		}

		private ImageStreamNames ReadImageStreamNames()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ImageStreamNames == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ImageStreamNames imageStreamNames = new ImageStreamNames(num);
			for (int i = 0; i < num; i++)
			{
				string key = m_reader.ReadString();
				if (m_intermediateFormatVersion.IsRS2000_WithImageInfo)
				{
					ImageInfo value = ReadImageInfo();
					imageStreamNames.Add(key, value);
				}
				else
				{
					string streamName = m_reader.ReadString();
					imageStreamNames.Add(key, new ImageInfo(streamName, null));
				}
			}
			m_reader.ReadEndObject();
			return imageStreamNames;
		}

		private EmbeddedImageHashtable ReadEmbeddedImageHashtable()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.EmbeddedImageHashtable == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			EmbeddedImageHashtable embeddedImageHashtable = new EmbeddedImageHashtable(num);
			for (int i = 0; i < num; i++)
			{
				string key = m_reader.ReadString();
				if (m_intermediateFormatVersion.IsRS2000_WithImageInfo)
				{
					ImageInfo value = ReadImageInfo();
					embeddedImageHashtable.Add(key, value);
				}
				else
				{
					string streamName = m_reader.ReadString();
					embeddedImageHashtable.Add(key, new ImageInfo(streamName, null));
				}
			}
			m_reader.ReadEndObject();
			return embeddedImageHashtable;
		}

		private ExpressionInfoList ReadExpressionInfoList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ExpressionInfoList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ExpressionInfoList expressionInfoList = new ExpressionInfoList(num);
			for (int i = 0; i < num; i++)
			{
				expressionInfoList.Add(ReadExpressionInfo());
			}
			m_reader.ReadEndObject();
			return expressionInfoList;
		}

		private DataSetList ReadDataSetList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.DataSetList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			DataSetList dataSetList = new DataSetList(num);
			for (int i = 0; i < num; i++)
			{
				dataSetList.Add(ReadDataSet());
			}
			m_reader.ReadEndObject();
			return dataSetList;
		}

		private ExpressionInfo[] ReadExpressionInfos()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Array == m_reader.Token);
			int arrayLength = m_reader.ArrayLength;
			ExpressionInfo[] array = new ExpressionInfo[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				array[i] = ReadExpressionInfo();
			}
			return array;
		}

		private StringList ReadStringList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.StringList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			StringList stringList = new StringList(num);
			for (int i = 0; i < num; i++)
			{
				stringList.Add(m_reader.ReadString());
			}
			m_reader.ReadEndObject();
			return stringList;
		}

		private DataFieldList ReadDataFieldList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.DataFieldList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			DataFieldList dataFieldList = new DataFieldList(num);
			for (int i = 0; i < num; i++)
			{
				dataFieldList.Add(ReadDataField());
			}
			m_reader.ReadEndObject();
			return dataFieldList;
		}

		private DataRegionList ReadDataRegionList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.DataRegionList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			DataRegionList dataRegionList = new DataRegionList(num);
			for (int i = 0; i < num; i++)
			{
				dataRegionList.Add(ReadDataRegionReference());
			}
			m_reader.ReadEndObject();
			return dataRegionList;
		}

		private ParameterValueList ReadParameterValueList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ParameterValueList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ParameterValueList parameterValueList = new ParameterValueList(num);
			for (int i = 0; i < num; i++)
			{
				parameterValueList.Add(ReadParameterValue());
			}
			m_reader.ReadEndObject();
			return parameterValueList;
		}

		private CodeClassList ReadCodeClassList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.CodeClassList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			CodeClassList codeClassList = new CodeClassList(num);
			for (int i = 0; i < num; i++)
			{
				codeClassList.Add(ReadCodeClass());
			}
			m_reader.ReadEndObject();
			return codeClassList;
		}

		private IntList ReadIntList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.IntList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			IntList intList = new IntList(num);
			for (int i = 0; i < num; i++)
			{
				intList.Add(m_reader.ReadInt32());
			}
			m_reader.ReadEndObject();
			return intList;
		}

		private Int64List ReadInt64List()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.Int64List == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			Int64List int64List = new Int64List(num);
			for (int i = 0; i < num; i++)
			{
				int64List.Add(m_reader.ReadInt64());
			}
			m_reader.ReadEndObject();
			return int64List;
		}

		private BoolList ReadBoolList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.BoolList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			BoolList boolList = new BoolList(num);
			for (int i = 0; i < num; i++)
			{
				boolList.Add(m_reader.ReadBoolean());
			}
			m_reader.ReadEndObject();
			return boolList;
		}

		private MatrixRowList ReadMatrixRowList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.MatrixRowList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			MatrixRowList matrixRowList = new MatrixRowList(num);
			for (int i = 0; i < num; i++)
			{
				matrixRowList.Add(ReadMatrixRow());
			}
			m_reader.ReadEndObject();
			return matrixRowList;
		}

		private MatrixColumnList ReadMatrixColumnList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.MatrixColumnList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			MatrixColumnList matrixColumnList = new MatrixColumnList(num);
			for (int i = 0; i < num; i++)
			{
				matrixColumnList.Add(ReadMatrixColumn());
			}
			m_reader.ReadEndObject();
			return matrixColumnList;
		}

		private TableColumnList ReadTableColumnList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.TableColumnList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			TableColumnList tableColumnList = new TableColumnList(num);
			for (int i = 0; i < num; i++)
			{
				tableColumnList.Add(ReadTableColumn());
			}
			m_reader.ReadEndObject();
			return tableColumnList;
		}

		private TableRowList ReadTableRowList(ReportItem parent)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.TableRowList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			TableRowList tableRowList = new TableRowList(num);
			for (int i = 0; i < num; i++)
			{
				tableRowList.Add(ReadTableRow(parent));
			}
			m_reader.ReadEndObject();
			return tableRowList;
		}

		private ChartColumnList ReadChartColumnList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ChartColumnList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ChartColumnList chartColumnList = new ChartColumnList(num);
			for (int i = 0; i < num; i++)
			{
				chartColumnList.Add(ReadChartColumn());
			}
			m_reader.ReadEndObject();
			return chartColumnList;
		}

		private SubReportList ReadSubReportList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.SubReportList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			SubReportList subReportList = new SubReportList(num);
			for (int i = 0; i < num; i++)
			{
				subReportList.Add(ReadSubReportReference());
			}
			m_reader.ReadEndObject();
			return subReportList;
		}

		private NonComputedUniqueNames[] ReadNonComputedUniqueNamess()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Array == m_reader.Token);
			int arrayLength = m_reader.ArrayLength;
			NonComputedUniqueNames[] array = new NonComputedUniqueNames[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				array[i] = ReadNonComputedUniqueNames();
			}
			return array;
		}

		private ReportItemInstanceList ReadReportItemInstanceList(ReportItemCollection reportItemsDef)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ReportItemInstanceList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ReportItemInstanceList reportItemInstanceList = new ReportItemInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				reportItemInstanceList.Add(ReadReportItemInstance(reportItemsDef.ComputedReportItems[i]));
			}
			m_reader.ReadEndObject();
			return reportItemInstanceList;
		}

		private RenderingPagesRangesList ReadRenderingPagesRangesList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.RenderingPagesRangesList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			RenderingPagesRangesList renderingPagesRangesList = new RenderingPagesRangesList(num);
			for (int i = 0; i < num; i++)
			{
				renderingPagesRangesList.Add(ReadRenderingPagesRanges());
			}
			m_reader.ReadEndObject();
			return renderingPagesRangesList;
		}

		private ListContentInstanceList ReadListContentInstanceList(List listDef)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ListContentInstanceList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ListContentInstanceList listContentInstanceList = new ListContentInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				listContentInstanceList.Add(ReadListContentInstance(listDef));
			}
			m_reader.ReadEndObject();
			return listContentInstanceList;
		}

		private MatrixHeadingInstanceList ReadMatrixHeadingInstanceList(MatrixHeading headingDef)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.MatrixHeadingInstanceList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			MatrixHeadingInstanceList matrixHeadingInstanceList = new MatrixHeadingInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				matrixHeadingInstanceList.Add(ReadMatrixHeadingInstance(headingDef, i, num));
			}
			m_reader.ReadEndObject();
			return matrixHeadingInstanceList;
		}

		private MatrixCellInstancesList ReadMatrixCellInstancesList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.MatrixCellInstancesList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			MatrixCellInstancesList matrixCellInstancesList = new MatrixCellInstancesList(num);
			for (int i = 0; i < num; i++)
			{
				matrixCellInstancesList.Add(ReadMatrixCellInstanceList());
			}
			m_reader.ReadEndObject();
			return matrixCellInstancesList;
		}

		private MatrixCellInstanceList ReadMatrixCellInstanceList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.MatrixCellInstanceList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			MatrixCellInstanceList matrixCellInstanceList = new MatrixCellInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				matrixCellInstanceList.Add(ReadMatrixCellInstanceBase());
			}
			m_reader.ReadEndObject();
			return matrixCellInstanceList;
		}

		private MultiChartInstanceList ReadMultiChartInstanceList(Chart chartDef)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.MultiChartInstanceList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			MultiChartInstanceList multiChartInstanceList = new MultiChartInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				multiChartInstanceList.Add(ReadMultiChartInstance(chartDef));
			}
			m_reader.ReadEndObject();
			return multiChartInstanceList;
		}

		private ChartHeadingInstanceList ReadChartHeadingInstanceList(ChartHeading headingDef)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ChartHeadingInstanceList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ChartHeadingInstanceList chartHeadingInstanceList = new ChartHeadingInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				chartHeadingInstanceList.Add(ReadChartHeadingInstance(headingDef));
			}
			m_reader.ReadEndObject();
			return chartHeadingInstanceList;
		}

		private ChartDataPointInstancesList ReadChartDataPointInstancesList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ChartDataPointInstancesList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ChartDataPointInstancesList chartDataPointInstancesList = new ChartDataPointInstancesList(num);
			for (int i = 0; i < num; i++)
			{
				chartDataPointInstancesList.Add(ReadChartDataPointInstanceList());
			}
			m_reader.ReadEndObject();
			return chartDataPointInstancesList;
		}

		private ChartDataPointInstanceList ReadChartDataPointInstanceList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ChartDataPointInstanceList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ChartDataPointInstanceList chartDataPointInstanceList = new ChartDataPointInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				chartDataPointInstanceList.Add(ReadChartDataPointInstance());
			}
			m_reader.ReadEndObject();
			return chartDataPointInstanceList;
		}

		private TableRowInstance[] ReadTableRowInstances(TableRowList rowDefs, int rowStartUniqueName)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Array == m_reader.Token);
			int arrayLength = m_reader.ArrayLength;
			TableRowInstance[] array = new TableRowInstance[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				array[i] = ReadTableRowInstance(rowDefs, i, rowStartUniqueName);
				if (-1 == rowStartUniqueName)
				{
					continue;
				}
				rowStartUniqueName++;
				Global.Tracer.Assert(rowDefs != null, "(null != rowDefs)");
				if (rowDefs[i] != null)
				{
					ReportItemCollection reportItems = rowDefs[i].ReportItems;
					if (reportItems != null && reportItems.NonComputedReportItems != null)
					{
						rowStartUniqueName += reportItems.NonComputedReportItems.Count;
					}
				}
			}
			return array;
		}

		private TableDetailInstanceList ReadTableDetailInstanceList(TableDetail detailDef)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.TableDetailInstanceList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			TableDetailInstanceList tableDetailInstanceList = new TableDetailInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				tableDetailInstanceList.Add(ReadTableDetailInstance(detailDef));
			}
			m_reader.ReadEndObject();
			return tableDetailInstanceList;
		}

		private TableGroupInstanceList ReadTableGroupInstanceList(TableGroup groupDef)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.TableGroupInstanceList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			TableGroupInstanceList tableGroupInstanceList = new TableGroupInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				tableGroupInstanceList.Add(ReadTableGroupInstance(groupDef));
			}
			m_reader.ReadEndObject();
			return tableGroupInstanceList;
		}

		private TableColumnInstance[] ReadTableColumnInstances()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Array == m_reader.Token);
			int arrayLength = m_reader.ArrayLength;
			TableColumnInstance[] array = new TableColumnInstance[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				array[i] = ReadTableColumnInstance();
			}
			return array;
		}

		private CustomReportItemHeadingList ReadCustomReportItemHeadingList(ReportItem parent)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.CustomReportItemHeadingList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			CustomReportItemHeadingList customReportItemHeadingList = new CustomReportItemHeadingList(num);
			for (int i = 0; i < num; i++)
			{
				customReportItemHeadingList.Add(ReadCustomReportItemHeading(parent));
			}
			m_reader.ReadEndObject();
			return customReportItemHeadingList;
		}

		private CustomReportItemHeadingInstanceList ReadCustomReportItemHeadingInstanceList(CustomReportItemHeadingList headingDefinitions)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.CustomReportItemHeadingInstanceList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			CustomReportItemHeadingInstanceList customReportItemHeadingInstanceList = new CustomReportItemHeadingInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				customReportItemHeadingInstanceList.Add(ReadCustomReportItemHeadingInstance(headingDefinitions, i, num));
			}
			m_reader.ReadEndObject();
			return customReportItemHeadingInstanceList;
		}

		private CustomReportItemCellInstancesList ReadCustomReportItemCellInstancesList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.CustomReportItemCellInstancesList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			CustomReportItemCellInstancesList customReportItemCellInstancesList = new CustomReportItemCellInstancesList(num);
			for (int i = 0; i < num; i++)
			{
				customReportItemCellInstancesList.Add(ReadCustomReportItemCellInstanceList());
			}
			m_reader.ReadEndObject();
			return customReportItemCellInstancesList;
		}

		private CustomReportItemCellInstanceList ReadCustomReportItemCellInstanceList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.CustomReportItemCellInstanceList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			CustomReportItemCellInstanceList customReportItemCellInstanceList = new CustomReportItemCellInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				customReportItemCellInstanceList.Add(ReadCustomReportItemCellInstance());
			}
			m_reader.ReadEndObject();
			return customReportItemCellInstanceList;
		}

		private DocumentMapNode[] ReadDocumentMapNodes()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Array == m_reader.Token);
			int arrayLength = m_reader.ArrayLength;
			DocumentMapNode[] array = new DocumentMapNode[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				array[i] = ReadDocumentMapNode();
			}
			return array;
		}

		private DocumentMapNodeInfo[] ReadDocumentMapNodesInfo()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Array == m_reader.Token);
			int arrayLength = m_reader.ArrayLength;
			DocumentMapNodeInfo[] array = new DocumentMapNodeInfo[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				array[i] = ReadDocumentMapNodeInfo();
			}
			return array;
		}

		private bool FindDocumentMapNodesPage(string documentMapId, ref int page)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return false;
			}
			Assert(Token.Array == m_reader.Token);
			int arrayLength = m_reader.ArrayLength;
			for (int i = 0; i < arrayLength; i++)
			{
				if (FindDocumentMapNodePage(documentMapId, ref page))
				{
					return true;
				}
			}
			return false;
		}

		private object[] ReadVariants()
		{
			return ReadVariants(isMultiValue: false, readNextToken: true);
		}

		private object[] ReadVariants(bool isMultiValue, bool readNextToken)
		{
			if (readNextToken)
			{
				Assert(m_reader.Read());
			}
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Array == m_reader.Token);
			int arrayLength = m_reader.ArrayLength;
			object[] array = new object[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				if (isMultiValue)
				{
					ReadMultiValue(array, i);
				}
				else
				{
					array[i] = ReadVariant();
				}
			}
			return array;
		}

		private void ReadMultiValue(object[] parentArray, int index)
		{
			Assert(m_reader.Read());
			if (m_reader.Token != 0)
			{
				if (Token.Array != m_reader.Token)
				{
					Assert(parentArray != null);
					parentArray[index] = ReadVariant(readNextToken: false);
				}
				else
				{
					Assert(parentArray != null);
					parentArray[index] = ReadVariants(isMultiValue: false, readNextToken: false);
				}
			}
		}

		private object ReadMultiValue()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			if (Token.Array != m_reader.Token)
			{
				return ReadVariant(readNextToken: false);
			}
			return ReadVariants(isMultiValue: false, readNextToken: false);
		}

		private string[] ReadStrings()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Array == m_reader.Token);
			int arrayLength = m_reader.ArrayLength;
			string[] array = new string[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				array[i] = m_reader.ReadString();
			}
			return array;
		}

		private VariantList ReadVariantList(bool convertDBNull)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.VariantList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			VariantList variantList = new VariantList(num);
			for (int i = 0; i < num; i++)
			{
				variantList.Add(ReadVariant(readNextToken: true, convertDBNull));
			}
			m_reader.ReadEndObject();
			return variantList;
		}

		private VariantList[] ReadVariantLists(bool convertDBNull)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Array == m_reader.Token);
			int arrayLength = m_reader.ArrayLength;
			VariantList[] array = new VariantList[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				array[i] = ReadVariantList(convertDBNull);
			}
			return array;
		}

		private ProcessingMessageList ReadProcessingMessageList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ProcessingMessageList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ProcessingMessageList processingMessageList = new ProcessingMessageList(num);
			for (int i = 0; i < num; i++)
			{
				processingMessageList.Add(ReadProcessingMessage());
			}
			m_reader.ReadEndObject();
			return processingMessageList;
		}

		private DataCellsList ReadDataCellsList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.DataCellsList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			DataCellsList dataCellsList = new DataCellsList(num);
			for (int i = 0; i < num; i++)
			{
				dataCellsList.Add(ReadDataCellList());
			}
			m_reader.ReadEndObject();
			return dataCellsList;
		}

		private DataCellList ReadDataCellList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.DataCellList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			DataCellList dataCellList = new DataCellList(num);
			for (int i = 0; i < num; i++)
			{
				dataCellList.Add(ReadDataValueCRIList());
			}
			m_reader.ReadEndObject();
			return dataCellList;
		}

		private DataValueCRIList ReadDataValueCRIList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = m_reader.ObjectType;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.DataValueCRIList == objectType);
			DataValueCRIList dataValueCRIList = new DataValueCRIList();
			ReadDataValueList(dataValueCRIList);
			if (PreRead(objectType, indexes))
			{
				dataValueCRIList.RDLRowIndex = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				dataValueCRIList.RDLColumnIndex = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return dataValueCRIList;
		}

		private DataValueList ReadDataValueList()
		{
			DataValueList values = new DataValueList();
			return ReadDataValueList(values);
		}

		private DataValueList ReadDataValueList(DataValueList values)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.DataValueList == m_reader.ObjectType);
			int num2 = values.Capacity = m_reader.ReadArray();
			for (int i = 0; i < num2; i++)
			{
				values.Add(ReadDataValue());
			}
			m_reader.ReadEndObject();
			return values;
		}

		private DataValueInstanceList ReadDataValueInstanceList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.DataValueInstanceList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			DataValueInstanceList dataValueInstanceList = new DataValueInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				dataValueInstanceList.Add(ReadDataValueInstance());
			}
			m_reader.ReadEndObject();
			return dataValueInstanceList;
		}

		private ImageMapAreaInstanceList ReadImageMapAreaInstanceList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ImageMapAreaInstanceList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ImageMapAreaInstanceList imageMapAreaInstanceList = new ImageMapAreaInstanceList(num);
			for (int i = 0; i < num; i++)
			{
				imageMapAreaInstanceList.Add(ReadImageMapAreaInstance());
			}
			m_reader.ReadEndObject();
			return imageMapAreaInstanceList;
		}

		private void ReadIDOwnerBase(IDOwner idOwner)
		{
			Assert(idOwner != null);
			ObjectType objectType = ObjectType.IDOwner;
			Indexes indexes = new Indexes();
			if (PreRead(objectType, indexes))
			{
				idOwner.ID = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
		}

		private ReportItem ReadReportItem(ReportItem parent)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			if (ObjectType.Line == m_reader.ObjectType)
			{
				return ReadLineInternals(parent);
			}
			if (ObjectType.Rectangle == m_reader.ObjectType)
			{
				return ReadRectangleInternals(parent);
			}
			if (ObjectType.Image == m_reader.ObjectType)
			{
				return ReadImageInternals(parent);
			}
			if (ObjectType.CheckBox == m_reader.ObjectType)
			{
				return ReadCheckBoxInternals(parent);
			}
			if (ObjectType.TextBox == m_reader.ObjectType)
			{
				return ReadTextBoxInternals(parent);
			}
			if (ObjectType.SubReport == m_reader.ObjectType)
			{
				return ReadSubReportInternals(parent);
			}
			if (ObjectType.ActiveXControl == m_reader.ObjectType)
			{
				return ReadActiveXControlInternals(parent);
			}
			return ReadDataRegionInternals(parent);
		}

		private void ReadReportItemBase(ReportItem reportItem)
		{
			Assert(reportItem != null);
			ObjectType objectType = ObjectType.ReportItem;
			Indexes indexes = new Indexes();
			ReadIDOwnerBase(reportItem);
			RegisterDefinitionObject(reportItem);
			if (PreRead(objectType, indexes))
			{
				reportItem.Name = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.StyleClass = ReadStyle();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.Top = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.TopValue = m_reader.ReadDouble();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.Left = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.LeftValue = m_reader.ReadDouble();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.Height = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.HeightValue = m_reader.ReadDouble();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.Width = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.WidthValue = m_reader.ReadDouble();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.ZIndex = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.Visibility = ReadVisibility();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.ToolTip = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.Label = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.Bookmark = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.Custom = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.RepeatedSibling = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.IsFullSize = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.ExprHostID = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.DataElementName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.DataElementOutput = ReadDataElementOutputType(reportItem.Visibility);
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.DistanceFromReportTop = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.DistanceBeforeTop = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.SiblingAboveMe = ReadIntList();
			}
			if (PreRead(objectType, indexes))
			{
				reportItem.CustomProperties = ReadDataValueList();
			}
			PostRead(objectType, indexes);
		}

		private ReportItem ReadReportItemReference(bool getDefinition)
		{
			if (m_intermediateFormatVersion.IsRS2000_WithOtherPageChunkSplit)
			{
				Assert(m_reader.ReadNoTypeReference());
			}
			else
			{
				Assert(m_reader.Read());
			}
			if (m_reader.Token == Token.Null || !getDefinition)
			{
				return null;
			}
			Assert(Token.Reference == m_reader.Token);
			IDOwner definitionObject = GetDefinitionObject(m_reader.ReferenceValue);
			Assert(definitionObject is ReportItem);
			return (ReportItem)definitionObject;
		}

		private SubReport ReadSubReportReference()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Reference == m_reader.Token);
			Assert(ObjectType.SubReport == m_reader.ObjectType);
			IDOwner definitionObject = GetDefinitionObject(m_reader.ReferenceValue);
			Assert(definitionObject is SubReport);
			return (SubReport)definitionObject;
		}

		private PageSection ReadPageSection(bool isHeader, ReportItem parent)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.PageSection;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			PageSection pageSection = new PageSection(isHeader, parent);
			ReadReportItemBase(pageSection);
			if (PreRead(objectType, indexes))
			{
				pageSection.PrintOnFirstPage = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				pageSection.PrintOnLastPage = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				pageSection.ReportItems = ReadReportItemCollection(pageSection);
			}
			if (PreRead(objectType, indexes))
			{
				pageSection.PostProcessEvaluate = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return pageSection;
		}

		private ReportItemCollection ReadReportItemCollection(ReportItem parent)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReportItemCollection;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ReportItemCollection reportItemCollection = new ReportItemCollection();
			ReadIDOwnerBase(reportItemCollection);
			RegisterDefinitionObject(reportItemCollection);
			if (PreRead(objectType, indexes))
			{
				reportItemCollection.NonComputedReportItems = ReadReportItemList(parent);
			}
			if (PreRead(objectType, indexes))
			{
				reportItemCollection.ComputedReportItems = ReadReportItemList(parent);
			}
			if (PreRead(objectType, indexes))
			{
				reportItemCollection.SortedReportItems = ReadReportItemIndexerList();
			}
			if (PreRead(objectType, indexes))
			{
				reportItemCollection.RunningValues = ReadRunningValueInfoList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return reportItemCollection;
		}

		private Report.ShowHideTypes ReadShowHideTypes()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(Report.ShowHideTypes), num));
			return (Report.ShowHideTypes)num;
		}

		private DataElementOutputTypes ReadDataElementOutputType(Visibility visibility)
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(DataElementOutputTypes), num));
			DataElementOutputTypes dataElementOutputTypes = (DataElementOutputTypes)num;
			if (dataElementOutputTypes == DataElementOutputTypes.Output && (m_intermediateFormatVersion == null || !m_intermediateFormatVersion.IsRS2005_WithXmlDataElementOutputChange) && visibility != null && visibility.Hidden != null && ExpressionInfo.Types.Constant == visibility.Hidden.Type && visibility.Hidden.BoolValue && visibility.Toggle == null)
			{
				dataElementOutputTypes = DataElementOutputTypes.NoOutput;
			}
			return dataElementOutputTypes;
		}

		private Style ReadStyle()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Style;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			Style style = new Style(ConstructionPhase.Deserializing);
			if (PreRead(objectType, indexes))
			{
				style.StyleAttributes = ReadStyleAttributeHashtable();
			}
			if (PreRead(objectType, indexes))
			{
				style.ExpressionList = ReadExpressionInfoList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return style;
		}

		private Visibility ReadVisibility()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Visibility;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			Visibility visibility = new Visibility();
			if (PreRead(objectType, indexes))
			{
				visibility.Hidden = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				visibility.Toggle = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				visibility.RecursiveReceiver = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return visibility;
		}

		private Filter ReadFilter()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Filter;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			Filter filter = new Filter();
			if (PreRead(objectType, indexes))
			{
				filter.Expression = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				filter.Operator = ReadOperators();
			}
			if (PreRead(objectType, indexes))
			{
				filter.Values = ReadExpressionInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				filter.ExprHostID = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return filter;
		}

		private Filter.Operators ReadOperators()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(Filter.Operators), num));
			return (Filter.Operators)num;
		}

		private DataSource ReadDataSource()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.DataSource;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			DataSource dataSource = new DataSource();
			if (PreRead(objectType, indexes))
			{
				dataSource.Name = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				dataSource.Transaction = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				dataSource.Type = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				dataSource.ConnectStringExpression = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				dataSource.IntegratedSecurity = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				dataSource.Prompt = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				dataSource.DataSourceReference = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				dataSource.DataSets = ReadDataSetList();
			}
			if (PreRead(objectType, indexes))
			{
				dataSource.ID = m_reader.ReadGuid();
			}
			if (PreRead(objectType, indexes))
			{
				dataSource.ExprHostID = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				dataSource.SharedDataSourceReferencePath = m_reader.ReadString();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return dataSource;
		}

		private DataAggregateInfo ReadDataAggregateInfo()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			if (ObjectType.RunningValueInfo == m_reader.ObjectType)
			{
				return ReadRunningValueInfoInternals();
			}
			Assert(ObjectType.DataAggregateInfo == m_reader.ObjectType);
			DataAggregateInfo dataAggregateInfo = new DataAggregateInfo();
			ReadDataAggregateInfoBase(dataAggregateInfo);
			m_reader.ReadEndObject();
			return dataAggregateInfo;
		}

		private void ReadDataAggregateInfoBase(DataAggregateInfo aggregate)
		{
			Assert(aggregate != null);
			ObjectType objectType = ObjectType.DataAggregateInfo;
			Indexes indexes = new Indexes();
			if (PreRead(objectType, indexes))
			{
				aggregate.Name = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				aggregate.AggregateType = ReadAggregateTypes();
			}
			if (PreRead(objectType, indexes))
			{
				aggregate.Expressions = ReadExpressionInfos();
			}
			if (PreRead(objectType, indexes))
			{
				aggregate.DuplicateNames = ReadStringList();
			}
			PostRead(objectType, indexes);
		}

		private ExpressionInfo ReadExpressionInfo()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ExpressionInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token || Token.String == m_reader.Token || Token.Boolean == m_reader.Token || Token.Int32 == m_reader.Token);
			Assert(objectType == m_reader.ObjectType || m_reader.ObjectType == ObjectType.None);
			ExpressionInfo expressionInfo = new ExpressionInfo();
			if (m_reader.ObjectType == ObjectType.None)
			{
				expressionInfo.Type = ExpressionInfo.Types.Constant;
				switch (m_reader.Token)
				{
				case Token.String:
					expressionInfo.Value = m_reader.StringValue;
					break;
				case Token.Boolean:
					expressionInfo.BoolValue = m_reader.BooleanValue;
					break;
				case Token.Int32:
					expressionInfo.IntValue = m_reader.Int32Value;
					break;
				default:
					Assert(condition: false);
					break;
				}
			}
			else
			{
				if (PreRead(objectType, indexes))
				{
					expressionInfo.Type = ReadTypes();
				}
				if (PreRead(objectType, indexes))
				{
					expressionInfo.Value = m_reader.ReadString();
				}
				if (PreRead(objectType, indexes))
				{
					expressionInfo.BoolValue = m_reader.ReadBoolean();
				}
				if (PreRead(objectType, indexes))
				{
					expressionInfo.IntValue = m_reader.ReadInt32();
				}
				if (PreRead(objectType, indexes))
				{
					expressionInfo.ExprHostID = m_reader.ReadInt32();
				}
				if (PreRead(objectType, indexes))
				{
					expressionInfo.OriginalText = m_reader.ReadString();
				}
				PostRead(objectType, indexes);
				m_reader.ReadEndObject();
			}
			return expressionInfo;
		}

		private DataAggregateInfo.AggregateTypes ReadAggregateTypes()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(DataAggregateInfo.AggregateTypes), num));
			return (DataAggregateInfo.AggregateTypes)num;
		}

		private ExpressionInfo.Types ReadTypes()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(ExpressionInfo.Types), num));
			return (ExpressionInfo.Types)num;
		}

		private ReportItemIndexer ReadReportItemIndexer()
		{
			ObjectType objectType = ObjectType.ReportItemIndexer;
			Indexes indexes = new Indexes();
			Assert(objectType == m_reader.ReadObject());
			ReportItemIndexer result = default(ReportItemIndexer);
			if (PreRead(objectType, indexes))
			{
				result.IsComputed = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				result.Index = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return result;
		}

		private RenderingPagesRanges ReadRenderingPagesRanges()
		{
			ObjectType objectType = ObjectType.RenderingPagesRanges;
			Indexes indexes = new Indexes();
			Assert(objectType == m_reader.ReadObject());
			RenderingPagesRanges result = default(RenderingPagesRanges);
			if (PreRead(objectType, indexes))
			{
				result.StartPage = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				result.EndPage = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return result;
		}

		private RunningValueInfo ReadRunningValueInfo()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			return ReadRunningValueInfoInternals();
		}

		private RunningValueInfo ReadRunningValueInfoInternals()
		{
			ObjectType objectType = ObjectType.RunningValueInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			RunningValueInfo runningValueInfo = new RunningValueInfo();
			ReadDataAggregateInfoBase(runningValueInfo);
			if (PreRead(objectType, indexes))
			{
				runningValueInfo.Scope = m_reader.ReadString();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return runningValueInfo;
		}

		private AttributeInfo ReadAttributeInfo()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.AttributeInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			AttributeInfo attributeInfo = new AttributeInfo();
			if (PreRead(objectType, indexes))
			{
				attributeInfo.IsExpression = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				attributeInfo.Value = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				attributeInfo.BoolValue = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				attributeInfo.IntValue = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return attributeInfo;
		}

		private DataSet ReadDataSet()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.DataSet;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			DataSet dataSet = new DataSet();
			if (m_intermediateFormatVersion.Is_WithUserSort)
			{
				ReadIDOwnerBase(dataSet);
				RegisterDefinitionObject(dataSet);
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.Name = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.Fields = ReadDataFieldList();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.Query = ReadReportQuery();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.CaseSensitivity = ReadSensitivity();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.Collation = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.AccentSensitivity = ReadSensitivity();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.KanatypeSensitivity = ReadSensitivity();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.WidthSensitivity = ReadSensitivity();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.DataRegions = ReadDataRegionList();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.Aggregates = ReadDataAggregateInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.Filters = ReadFilterList();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.RecordSetSize = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.UsedOnlyInParameters = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.NonCalculatedFieldCount = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.ExprHostID = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.PostSortAggregates = ReadDataAggregateInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.LCID = (uint)m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.HasDetailUserSortFilter = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.UserSortExpressions = ReadExpressionInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.DynamicFieldReferences = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				dataSet.InterpretSubtotalsAsDetails = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return dataSet;
		}

		private ReportQuery ReadReportQuery()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReportQuery;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ReportQuery reportQuery = new ReportQuery();
			if (PreRead(objectType, indexes))
			{
				reportQuery.CommandType = ReadCommandType();
			}
			if (PreRead(objectType, indexes))
			{
				reportQuery.CommandText = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				reportQuery.Parameters = ReadParameterValueList();
			}
			if (PreRead(objectType, indexes))
			{
				reportQuery.TimeOut = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				reportQuery.CommandTextValue = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				reportQuery.RewrittenCommandText = m_reader.ReadString();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return reportQuery;
		}

		private DataSet.Sensitivity ReadSensitivity()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(DataSet.Sensitivity), num));
			return (DataSet.Sensitivity)num;
		}

		private CommandType ReadCommandType()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(CommandType), num));
			return (CommandType)num;
		}

		private Field ReadDataField()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Field;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			Field field = new Field();
			if (PreRead(objectType, indexes))
			{
				field.Name = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				field.DataField = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				field.Value = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				field.ExprHostID = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				field.DynamicPropertyReferences = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				field.ReferencedProperties = ReadFieldPropertyHashtable();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return field;
		}

		internal FieldPropertyHashtable ReadFieldPropertyHashtable()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.FieldPropertyHashtable == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			FieldPropertyHashtable fieldPropertyHashtable = new FieldPropertyHashtable(num);
			for (int i = 0; i < num; i++)
			{
				string key = m_reader.ReadString();
				fieldPropertyHashtable.Add(key);
			}
			m_reader.ReadEndObject();
			return fieldPropertyHashtable;
		}

		private ParameterValue ReadParameterValue()
		{
			ObjectType objectType = ObjectType.ParameterValue;
			Indexes indexes = new Indexes();
			Assert(objectType == m_reader.ReadObject());
			ParameterValue parameterValue = new ParameterValue();
			if (PreRead(objectType, indexes))
			{
				parameterValue.Name = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				parameterValue.Value = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				parameterValue.ExprHostID = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				parameterValue.Omit = ReadExpressionInfo();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return parameterValue;
		}

		private CodeClass ReadCodeClass()
		{
			ObjectType objectType = ObjectType.CodeClass;
			Indexes indexes = new Indexes();
			Assert(objectType == m_reader.ReadObject());
			CodeClass result = default(CodeClass);
			if (PreRead(objectType, indexes))
			{
				result.ClassName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				result.InstanceName = m_reader.ReadString();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return result;
		}

		private Action ReadAction()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Action;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			Action action = new Action();
			if (PreRead(objectType, indexes))
			{
				action.ActionItems = ReadActionItemList();
			}
			if (PreRead(objectType, indexes))
			{
				action.StyleClass = ReadStyle();
			}
			if (PreRead(objectType, indexes))
			{
				action.ComputedActionItemsCount = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return action;
		}

		private ActionItemList ReadActionItemList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ActionItemList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ActionItemList actionItemList = new ActionItemList(num);
			for (int i = 0; i < num; i++)
			{
				actionItemList.Add(ReadActionItem());
			}
			m_reader.ReadEndObject();
			return actionItemList;
		}

		private ActionItem ReadActionItem()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ActionItem actionItem = new ActionItem();
			if (m_intermediateFormatVersion.IsRS2005_WithMultipleActions)
			{
				ObjectType objectType = ObjectType.ActionItem;
				Indexes indexes = new Indexes();
				Assert(Token.Object == m_reader.Token);
				Assert(objectType == m_reader.ObjectType);
				if (PreRead(objectType, indexes))
				{
					actionItem.HyperLinkURL = ReadExpressionInfo();
				}
				if (PreRead(objectType, indexes))
				{
					actionItem.DrillthroughReportName = ReadExpressionInfo();
				}
				if (PreRead(objectType, indexes))
				{
					actionItem.DrillthroughParameters = ReadParameterValueList();
				}
				if (PreRead(objectType, indexes))
				{
					actionItem.DrillthroughBookmarkLink = ReadExpressionInfo();
				}
				if (PreRead(objectType, indexes))
				{
					actionItem.BookmarkLink = ReadExpressionInfo();
				}
				if (PreRead(objectType, indexes))
				{
					actionItem.Label = ReadExpressionInfo();
				}
				if (PreRead(objectType, indexes))
				{
					actionItem.ExprHostID = m_reader.ReadInt32();
				}
				if (PreRead(objectType, indexes))
				{
					actionItem.ComputedIndex = m_reader.ReadInt32();
				}
				PostRead(objectType, indexes);
			}
			else
			{
				Assert(Token.Object == m_reader.Token);
				Assert(ObjectType.Action == m_reader.ObjectType);
				actionItem.ComputedIndex = 0;
				actionItem.HyperLinkURL = ReadExpressionInfo();
				actionItem.DrillthroughReportName = ReadExpressionInfo();
				actionItem.DrillthroughParameters = ReadParameterValueList();
				actionItem.DrillthroughBookmarkLink = ReadExpressionInfo();
				actionItem.BookmarkLink = ReadExpressionInfo();
			}
			m_reader.ReadEndObject();
			return actionItem;
		}

		private Line ReadLineInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.Line;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			Line line = new Line(parent);
			ReadReportItemBase(line);
			if (PreRead(objectType, indexes))
			{
				line.LineSlant = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return line;
		}

		private Rectangle ReadRectangleInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.Rectangle;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			Rectangle rectangle = new Rectangle(parent);
			ReadReportItemBase(rectangle);
			if (PreRead(objectType, indexes))
			{
				rectangle.ReportItems = ReadReportItemCollection(rectangle);
			}
			if (PreRead(objectType, indexes))
			{
				rectangle.PageBreakAtEnd = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				rectangle.PageBreakAtStart = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				rectangle.LinkToChild = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return rectangle;
		}

		private Image ReadImageInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.Image;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			Image image = new Image(parent);
			ReadReportItemBase(image);
			if (PreRead(objectType, indexes))
			{
				if (m_intermediateFormatVersion.IsRS2005_WithMultipleActions)
				{
					image.Action = ReadAction();
				}
				else
				{
					ActionItem actionItem = ReadActionItem();
					if (actionItem != null)
					{
						image.Action = new Action(actionItem, computed: true);
					}
				}
			}
			if (PreRead(objectType, indexes))
			{
				image.Source = ReadSourceType();
			}
			if (PreRead(objectType, indexes))
			{
				image.Value = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				image.MIMEType = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				image.Sizing = ReadSizings();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return image;
		}

		private ImageMapAreaInstance ReadImageMapAreaInstance()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ImageMapAreaInstance imageMapAreaInstance = new ImageMapAreaInstance();
			ObjectType objectType = ObjectType.ImageMapAreaInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			if (PreRead(objectType, indexes))
			{
				imageMapAreaInstance.ID = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				imageMapAreaInstance.Shape = ReadImageMapAreaShape();
			}
			if (PreRead(objectType, indexes))
			{
				imageMapAreaInstance.Coordinates = m_reader.ReadFloatArray();
			}
			if (PreRead(objectType, indexes))
			{
				imageMapAreaInstance.Action = ReadAction();
			}
			if (PreRead(objectType, indexes))
			{
				imageMapAreaInstance.ActionInstance = ReadActionInstance(null);
			}
			if (PreRead(objectType, indexes))
			{
				imageMapAreaInstance.UniqueName = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return imageMapAreaInstance;
		}

		private Image.SourceType ReadSourceType()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(Image.SourceType), num));
			return (Image.SourceType)num;
		}

		private Image.Sizings ReadSizings()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(Image.Sizings), num));
			return (Image.Sizings)num;
		}

		private ImageMapArea.ImageMapAreaShape ReadImageMapAreaShape()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(ImageMapArea.ImageMapAreaShape), num));
			return (ImageMapArea.ImageMapAreaShape)num;
		}

		private CheckBox ReadCheckBoxInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.CheckBox;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			CheckBox checkBox = new CheckBox(parent);
			ReadReportItemBase(checkBox);
			if (PreRead(objectType, indexes))
			{
				checkBox.Value = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				checkBox.HideDuplicates = m_reader.ReadString();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return checkBox;
		}

		private TextBox ReadTextBoxInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.TextBox;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			TextBox textBox = new TextBox(parent);
			ReadReportItemBase(textBox);
			if (PreRead(objectType, indexes))
			{
				textBox.Value = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				textBox.CanGrow = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				textBox.CanShrink = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				textBox.HideDuplicates = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				if (m_intermediateFormatVersion.IsRS2005_WithMultipleActions)
				{
					textBox.Action = ReadAction();
				}
				else
				{
					ActionItem actionItem = ReadActionItem();
					if (actionItem != null)
					{
						textBox.Action = new Action(actionItem, computed: true);
					}
				}
			}
			if (PreRead(objectType, indexes))
			{
				textBox.IsToggle = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				textBox.InitialToggleState = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				textBox.ValueType = ReadTypeCode();
			}
			if (PreRead(objectType, indexes))
			{
				textBox.Formula = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				textBox.ValueReferenced = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				textBox.RecursiveSender = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				textBox.DataElementStyleAttribute = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				textBox.ContainingScopes = ReadGroupingReferenceList();
			}
			if (PreRead(objectType, indexes))
			{
				textBox.UserSort = ReadEndUserSort(textBox);
			}
			if (PreRead(objectType, indexes))
			{
				textBox.IsMatrixCellScope = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				textBox.IsSubReportTopLevelScope = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return textBox;
		}

		private EndUserSort ReadEndUserSort(TextBox eventSource)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.EndUserSort;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			EndUserSort endUserSort = new EndUserSort();
			if (PreRead(objectType, indexes))
			{
				endUserSort.DataSetID = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				endUserSort.SortExpressionScopeID = ReadIDOwnerID(ObjectType.ISortFilterScope);
			}
			if (PreRead(objectType, indexes))
			{
				endUserSort.GroupInSortTargetIDs = ReadGroupingIDList();
			}
			if (PreRead(objectType, indexes))
			{
				endUserSort.SortTargetID = ReadIDOwnerID(ObjectType.ISortFilterScope);
			}
			if (PreRead(objectType, indexes))
			{
				endUserSort.SortExpressionIndex = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				endUserSort.DetailScopeSubReports = ReadSubReportList();
			}
			if (-1 != endUserSort.SortExpressionScopeID || endUserSort.GroupInSortTargetIDs != null || -1 != endUserSort.SortTargetID)
			{
				Global.Tracer.Assert(m_textboxesWithUserSort != null && 0 < m_textboxesWithUserSort.Count);
				TextBoxList textBoxList = (TextBoxList)m_textboxesWithUserSort[m_textboxesWithUserSort.Count - 1];
				Global.Tracer.Assert(textBoxList != null, "(null != textboxesWithUserSort)");
				textBoxList.Add(eventSource);
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return endUserSort;
		}

		private IntList ReadGroupingIDList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.GroupingList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			IntList intList = new IntList(num);
			for (int i = 0; i < num; i++)
			{
				intList.Add(ReadIDOwnerID(ObjectType.Grouping));
			}
			m_reader.ReadEndObject();
			return intList;
		}

		private GroupingList ReadGroupingReferenceList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.GroupingList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			GroupingList groupingList = new GroupingList(num);
			for (int i = 0; i < num; i++)
			{
				groupingList.Add(ReadGroupingReference());
			}
			m_reader.ReadEndObject();
			return groupingList;
		}

		private Grouping ReadGroupingReference()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Reference == m_reader.Token);
			Assert(ObjectType.Grouping == m_reader.ObjectType);
			IDOwner definitionObject = GetDefinitionObject(m_reader.ReferenceValue);
			Assert(definitionObject is ReportHierarchyNode);
			return ((ReportHierarchyNode)definitionObject).Grouping;
		}

		private TypeCode ReadTypeCode()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(TypeCode), num));
			return (TypeCode)num;
		}

		private SubReport ReadSubReportInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.SubReport;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			SubReport subReport = new SubReport(parent);
			ReadReportItemBase(subReport);
			if (PreRead(objectType, indexes))
			{
				subReport.ReportPath = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				subReport.Parameters = ReadParameterValueList();
			}
			if (PreRead(objectType, indexes))
			{
				subReport.NoRows = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				subReport.MergeTransactions = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				subReport.ContainingScopes = ReadGroupingReferenceList();
			}
			if (PreRead(objectType, indexes))
			{
				subReport.IsMatrixCellScope = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				subReport.DataSetUniqueNameMap = ReadScopeLookupTable();
			}
			if (PreRead(objectType, indexes))
			{
				subReport.RetrievalStatus = ReadStatus();
			}
			if (PreRead(objectType, indexes))
			{
				subReport.ReportName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				subReport.Description = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				subReport.Report = ReadReport(subReport);
			}
			if (PreRead(objectType, indexes))
			{
				subReport.StringUri = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				subReport.ParametersFromCatalog = ReadParameterInfoCollection();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return subReport;
		}

		private ScopeLookupTable ReadScopeLookupTable()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ScopeLookupTable;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ScopeLookupTable scopeLookupTable = new ScopeLookupTable();
			if (PreRead(objectType, indexes))
			{
				scopeLookupTable.LookupTable = ReadScopeTableValues();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return scopeLookupTable;
		}

		private object ReadScopeTableValues()
		{
			Assert(m_reader.Read());
			if (Token.Int32 == m_reader.Token)
			{
				return m_reader.Int32Value;
			}
			Assert(Token.Array == m_reader.Token);
			int arrayLength = m_reader.ArrayLength;
			Hashtable hashtable = new Hashtable(arrayLength);
			for (int i = 0; i < arrayLength; i++)
			{
				object key = ReadVariant(readNextToken: true, convertDBNull: true);
				object value = ReadScopeTableValues();
				hashtable.Add(key, value);
			}
			return hashtable;
		}

		private SubReport.Status ReadStatus()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(SubReport.Status), num));
			return (SubReport.Status)num;
		}

		private ActiveXControl ReadActiveXControlInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.ActiveXControl;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ActiveXControl activeXControl = new ActiveXControl(parent);
			ReadReportItemBase(activeXControl);
			if (PreRead(objectType, indexes))
			{
				activeXControl.ClassID = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				activeXControl.CodeBase = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				activeXControl.Parameters = ReadParameterValueList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return activeXControl;
		}

		private ParameterBase ReadParameterBase(ParameterBase parameter)
		{
			Assert(parameter != null);
			ObjectType objectType = ObjectType.ParameterBase;
			Indexes indexes = new Indexes();
			if (PreRead(objectType, indexes))
			{
				parameter.Name = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				parameter.DataType = ReadDataType();
			}
			if (PreRead(objectType, indexes))
			{
				parameter.Nullable = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				parameter.Prompt = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				parameter.UsedInQuery = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				parameter.AllowBlank = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				parameter.MultiValue = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				parameter.DefaultValues = ReadVariants();
			}
			if (PreRead(objectType, indexes))
			{
				parameter.PromptUser = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			return parameter;
		}

		private ParameterDef ReadParameterDef()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ParameterDef;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ParameterDef parameterDef = new ParameterDef();
			ReadParameterBase(parameterDef);
			RegisterParameterDef(parameterDef);
			if (PreRead(objectType, indexes))
			{
				parameterDef.ValidValuesDataSource = ReadParameterDataSource();
			}
			if (PreRead(objectType, indexes))
			{
				parameterDef.ValidValuesValueExpressions = ReadExpressionInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				parameterDef.ValidValuesLabelExpressions = ReadExpressionInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				parameterDef.DefaultDataSource = ReadParameterDataSource();
			}
			if (PreRead(objectType, indexes))
			{
				parameterDef.DefaultExpressions = ReadExpressionInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				parameterDef.DependencyList = ReadParameterDefRefList();
			}
			if (PreRead(objectType, indexes))
			{
				parameterDef.ExprHostID = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return parameterDef;
		}

		private ParameterDataSource ReadParameterDataSource()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ParameterDataSource;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ParameterDataSource parameterDataSource = new ParameterDataSource();
			if (PreRead(objectType, indexes))
			{
				parameterDataSource.DataSourceIndex = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				parameterDataSource.DataSetIndex = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				parameterDataSource.ValueFieldIndex = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				parameterDataSource.LabelFieldIndex = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return parameterDataSource;
		}

		private ValidValue ReadValidValue()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ValidValue;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ValidValue validValue = new ValidValue();
			if (PreRead(objectType, indexes))
			{
				validValue.Label = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				validValue.Value = ReadVariant();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return validValue;
		}

		private DataRegion ReadDataRegionInternals(ReportItem parent)
		{
			Assert(Token.Object == m_reader.Token);
			if (ObjectType.List == m_reader.ObjectType)
			{
				return ReadListInternals(parent);
			}
			if (ObjectType.Matrix == m_reader.ObjectType)
			{
				return ReadMatrixInternals(parent);
			}
			if (ObjectType.Table == m_reader.ObjectType)
			{
				return ReadTableInternals(parent);
			}
			if (ObjectType.Chart == m_reader.ObjectType)
			{
				return ReadChartInternals(parent);
			}
			if (ObjectType.CustomReportItem == m_reader.ObjectType)
			{
				return ReadCustomReportItemInternals(parent);
			}
			Assert(ObjectType.OWCChart == m_reader.ObjectType);
			return ReadOWCChartInternals(parent);
		}

		private void ReadDataRegionBase(DataRegion dataRegion)
		{
			Assert(dataRegion != null);
			ObjectType objectType = ObjectType.DataRegion;
			Indexes indexes = new Indexes();
			ReadReportItemBase(dataRegion);
			if (PreRead(objectType, indexes))
			{
				dataRegion.DataSetName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				dataRegion.NoRows = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				dataRegion.PageBreakAtEnd = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				dataRegion.PageBreakAtStart = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				dataRegion.KeepTogether = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				dataRegion.RepeatSiblings = ReadIntList();
			}
			if (PreRead(objectType, indexes))
			{
				dataRegion.Filters = ReadFilterList();
			}
			if (PreRead(objectType, indexes))
			{
				dataRegion.Aggregates = ReadDataAggregateInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				dataRegion.PostSortAggregates = ReadDataAggregateInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				dataRegion.UserSortExpressions = ReadExpressionInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				dataRegion.DetailSortFiltersInScope = ReadInScopeSortFilterTable();
			}
			PostRead(objectType, indexes);
		}

		private DataRegion ReadDataRegionReference()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Reference == m_reader.Token);
			Assert(ObjectType.List == m_reader.ObjectType || ObjectType.Table == m_reader.ObjectType || ObjectType.Matrix == m_reader.ObjectType || ObjectType.Chart == m_reader.ObjectType || ObjectType.CustomReportItem == m_reader.ObjectType || ObjectType.OWCChart == m_reader.ObjectType);
			IDOwner definitionObject = GetDefinitionObject(m_reader.ReferenceValue);
			Assert(definitionObject is DataRegion);
			return (DataRegion)definitionObject;
		}

		private ReportHierarchyNode ReadReportHierarchyNode(ReportItem parent)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			if (ObjectType.TableGroup == m_reader.ObjectType)
			{
				return ReadTableGroupInternals(parent);
			}
			if (ObjectType.MatrixHeading == m_reader.ObjectType)
			{
				return ReadMatrixHeadingInternals(parent);
			}
			if (ObjectType.MultiChart == m_reader.ObjectType)
			{
				return ReadMultiChartInternals(parent);
			}
			if (ObjectType.ChartHeading == m_reader.ObjectType)
			{
				return ReadChartHeadingInternals(parent);
			}
			if (ObjectType.CustomReportItemHeading == m_reader.ObjectType)
			{
				return ReadCustomReportItemHeadingInternals(parent);
			}
			Assert(ObjectType.ReportHierarchyNode == m_reader.ObjectType);
			ReportHierarchyNode reportHierarchyNode = new ReportHierarchyNode();
			ReadReportHierarchyNodeBase(reportHierarchyNode, parent);
			m_reader.ReadEndObject();
			return reportHierarchyNode;
		}

		private void ReadReportHierarchyNodeBase(ReportHierarchyNode node, ReportItem parent)
		{
			Assert(node != null);
			ReadIDOwnerBase(node);
			RegisterDefinitionObject(node);
			ObjectType objectType = ObjectType.ReportHierarchyNode;
			Indexes indexes = new Indexes();
			if (PreRead(objectType, indexes))
			{
				node.Grouping = ReadGrouping();
			}
			if (PreRead(objectType, indexes))
			{
				node.Sorting = ReadSorting();
			}
			if (PreRead(objectType, indexes))
			{
				node.InnerHierarchy = ReadReportHierarchyNode(parent);
			}
			if (PreRead(objectType, indexes))
			{
				node.DataRegionDef = ReadDataRegionReference();
			}
			PostRead(objectType, indexes);
		}

		private Grouping ReadGrouping()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Grouping;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			Grouping grouping = new Grouping(ConstructionPhase.Deserializing);
			if (PreRead(objectType, indexes))
			{
				grouping.Name = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.GroupExpressions = ReadExpressionInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.GroupLabel = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.SortDirections = ReadBoolList();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.PageBreakAtEnd = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.PageBreakAtStart = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.Custom = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.Aggregates = ReadDataAggregateInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.GroupAndSort = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.Filters = ReadFilterList();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.HideDuplicatesReportItemIDs = ReadReportItemIDList();
				if (grouping.HideDuplicatesReportItemIDs != null)
				{
					m_groupingsWithHideDuplicatesStack.Peek().Add(grouping);
				}
			}
			if (PreRead(objectType, indexes))
			{
				grouping.Parent = ReadExpressionInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.RecursiveAggregates = ReadDataAggregateInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.PostSortAggregates = ReadDataAggregateInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.DataElementName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.DataCollectionName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.DataElementOutput = ReadDataElementOutputType(null);
			}
			if (PreRead(objectType, indexes))
			{
				grouping.CustomProperties = ReadDataValueList();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.SaveGroupExprValues = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.UserSortExpressions = ReadExpressionInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.NonDetailSortFiltersInScope = ReadInScopeSortFilterTable();
			}
			if (PreRead(objectType, indexes))
			{
				grouping.DetailSortFiltersInScope = ReadInScopeSortFilterTable();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return grouping;
		}

		private InScopeSortFilterHashtable ReadInScopeSortFilterTable()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.InScopeSortFilterHashtable == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			InScopeSortFilterHashtable inScopeSortFilterHashtable = new InScopeSortFilterHashtable(num);
			for (int i = 0; i < num; i++)
			{
				int num2 = m_reader.ReadInt32();
				IntList value = ReadIntList();
				inScopeSortFilterHashtable.Add(num2, value);
			}
			m_reader.ReadEndObject();
			return inScopeSortFilterHashtable;
		}

		private IntList ReadReportItemIDList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ReportItemList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			IntList intList = new IntList(num);
			for (int i = 0; i < num; i++)
			{
				intList.Add(ReadIDOwnerID(ObjectType.TextBox));
			}
			m_reader.ReadEndObject();
			return intList;
		}

		private int ReadIDOwnerID(ObjectType objectType)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return -1;
			}
			Assert(Token.Reference == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			return m_reader.ReferenceValue;
		}

		private Sorting ReadSorting()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Sorting;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			Sorting sorting = new Sorting(ConstructionPhase.Deserializing);
			if (PreRead(objectType, indexes))
			{
				sorting.SortExpressions = ReadExpressionInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				sorting.SortDirections = ReadBoolList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return sorting;
		}

		private TableGroup ReadTableGroup(ReportItem parent)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			return ReadTableGroupInternals(parent);
		}

		private TableGroup ReadTableGroupInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.TableGroup;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			TableGroup tableGroup = new TableGroup();
			ReadReportHierarchyNodeBase(tableGroup, parent);
			if (PreRead(objectType, indexes))
			{
				tableGroup.HeaderRows = ReadTableRowList(parent);
			}
			if (PreRead(objectType, indexes))
			{
				tableGroup.HeaderRepeatOnNewPage = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				tableGroup.FooterRows = ReadTableRowList(parent);
			}
			if (PreRead(objectType, indexes))
			{
				tableGroup.FooterRepeatOnNewPage = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				tableGroup.Visibility = ReadVisibility();
			}
			if (PreRead(objectType, indexes))
			{
				tableGroup.PropagatedPageBreakAtStart = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				tableGroup.PropagatedPageBreakAtEnd = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				tableGroup.RunningValues = ReadRunningValueInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				tableGroup.HasExprHost = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return tableGroup;
		}

		private TableGroup ReadTableGroupReference()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Reference == m_reader.Token);
			Assert(ObjectType.TableGroup == m_reader.ObjectType);
			IDOwner definitionObject = GetDefinitionObject(m_reader.ReferenceValue);
			Assert(definitionObject is TableGroup);
			return (TableGroup)definitionObject;
		}

		private TableDetail ReadTableDetail(ReportItem parent)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			return ReadTableDetailInternals(parent);
		}

		private TableDetail ReadTableDetailInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.TableDetail;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			TableDetail tableDetail = new TableDetail();
			ReadIDOwnerBase(tableDetail);
			RegisterDefinitionObject(tableDetail);
			if (PreRead(objectType, indexes))
			{
				tableDetail.DetailRows = ReadTableRowList(parent);
			}
			if (PreRead(objectType, indexes))
			{
				tableDetail.Sorting = ReadSorting();
			}
			if (PreRead(objectType, indexes))
			{
				tableDetail.Visibility = ReadVisibility();
			}
			if (PreRead(objectType, indexes))
			{
				tableDetail.RunningValues = ReadRunningValueInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				tableDetail.HasExprHost = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				tableDetail.SimpleDetailRows = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return tableDetail;
		}

		private void ReadPivotHeadingBase(PivotHeading pivotHeading, ReportItem parent)
		{
			Assert(pivotHeading != null);
			ObjectType objectType = ObjectType.PivotHeading;
			Indexes indexes = new Indexes();
			ReadReportHierarchyNodeBase(pivotHeading, parent);
			if (PreRead(objectType, indexes))
			{
				pivotHeading.Visibility = ReadVisibility();
			}
			if (PreRead(objectType, indexes))
			{
				pivotHeading.Subtotal = ReadSubtotal(parent);
			}
			if (PreRead(objectType, indexes))
			{
				pivotHeading.Level = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				pivotHeading.IsColumn = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				pivotHeading.HasExprHost = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				pivotHeading.SubtotalSpan = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				pivotHeading.IDs = ReadIntList();
			}
			PostRead(objectType, indexes);
		}

		private MatrixHeading ReadMatrixHeading(ReportItem parent)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			return ReadMatrixHeadingInternals(parent);
		}

		private MatrixHeading ReadMatrixHeadingInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.MatrixHeading;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			MatrixHeading matrixHeading = new MatrixHeading();
			ReadPivotHeadingBase(matrixHeading, parent);
			if (PreRead(objectType, indexes))
			{
				matrixHeading.Size = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				matrixHeading.SizeValue = m_reader.ReadDouble();
			}
			if (PreRead(objectType, indexes))
			{
				matrixHeading.ReportItems = ReadReportItemCollection(parent);
			}
			if (PreRead(objectType, indexes))
			{
				matrixHeading.OwcGroupExpression = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return matrixHeading;
		}

		private MatrixHeading ReadMatrixHeadingReference()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Reference == m_reader.Token);
			Assert(ObjectType.MatrixHeading == m_reader.ObjectType);
			IDOwner definitionObject = GetDefinitionObject(m_reader.ReferenceValue);
			Assert(definitionObject is MatrixHeading);
			return (MatrixHeading)definitionObject;
		}

		private void ReadTablixHeadingBase(TablixHeading tablixHeading, ReportItem parent)
		{
			Assert(tablixHeading != null);
			ObjectType objectType = ObjectType.TablixHeading;
			Indexes indexes = new Indexes();
			ReadReportHierarchyNodeBase(tablixHeading, null);
			if (PreRead(objectType, indexes))
			{
				tablixHeading.Subtotal = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				tablixHeading.IsColumn = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				tablixHeading.Level = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				tablixHeading.HasExprHost = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				tablixHeading.HeadingSpan = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
		}

		private CustomReportItemHeading ReadCustomReportItemHeading(ReportItem parent)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			return ReadCustomReportItemHeadingInternals(parent);
		}

		private CustomReportItemHeading ReadCustomReportItemHeadingInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.CustomReportItemHeading;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			CustomReportItemHeading customReportItemHeading = new CustomReportItemHeading();
			ReadTablixHeadingBase(customReportItemHeading, parent);
			if (PreRead(objectType, indexes))
			{
				customReportItemHeading.Static = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				customReportItemHeading.InnerHeadings = ReadCustomReportItemHeadingList(parent);
			}
			if (PreRead(objectType, indexes))
			{
				customReportItemHeading.CustomProperties = ReadDataValueList();
			}
			if (PreRead(objectType, indexes))
			{
				customReportItemHeading.ExprHostID = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				customReportItemHeading.RunningValues = ReadRunningValueInfoList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return customReportItemHeading;
		}

		private CustomReportItemHeading ReadCustomReportItemHeadingReference()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Reference == m_reader.Token);
			Assert(ObjectType.CustomReportItemHeading == m_reader.ObjectType);
			IDOwner definitionObject = GetDefinitionObject(m_reader.ReferenceValue);
			Assert(definitionObject is CustomReportItemHeading);
			return (CustomReportItemHeading)definitionObject;
		}

		private TableRow ReadTableRow(ReportItem parent)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableRow;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			TableRow tableRow = new TableRow();
			ReadIDOwnerBase(tableRow);
			RegisterDefinitionObject(tableRow);
			if (PreRead(objectType, indexes))
			{
				tableRow.ReportItems = ReadReportItemCollection(parent);
			}
			if (PreRead(objectType, indexes))
			{
				tableRow.IDs = ReadIntList();
			}
			if (PreRead(objectType, indexes))
			{
				tableRow.ColSpans = ReadIntList();
			}
			if (PreRead(objectType, indexes))
			{
				tableRow.Height = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				tableRow.HeightValue = m_reader.ReadDouble();
			}
			if (PreRead(objectType, indexes))
			{
				tableRow.Visibility = ReadVisibility();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return tableRow;
		}

		private Subtotal ReadSubtotal(ReportItem parent)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Subtotal;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			Subtotal subtotal = new Subtotal();
			ReadIDOwnerBase(subtotal);
			if (PreRead(objectType, indexes))
			{
				subtotal.AutoDerived = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				subtotal.ReportItems = ReadReportItemCollection(parent);
			}
			if (PreRead(objectType, indexes))
			{
				subtotal.StyleClass = ReadStyle();
			}
			if (PreRead(objectType, indexes))
			{
				subtotal.Position = ReadPositionType();
			}
			if (PreRead(objectType, indexes))
			{
				subtotal.DataElementName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				subtotal.DataElementOutput = ReadDataElementOutputType(null);
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return subtotal;
		}

		private Subtotal.PositionType ReadPositionType()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(Subtotal.PositionType), num));
			return (Subtotal.PositionType)num;
		}

		private List ReadListInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.List;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			List list = new List(parent);
			ReadDataRegionBase(list);
			if (PreRead(objectType, indexes))
			{
				list.HierarchyDef = ReadReportHierarchyNode(list);
			}
			if (PreRead(objectType, indexes))
			{
				list.ReportItems = ReadReportItemCollection(list);
			}
			if (PreRead(objectType, indexes))
			{
				list.FillPage = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				list.DataInstanceName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				list.DataInstanceElementOutput = ReadDataElementOutputType(null);
			}
			if (PreRead(objectType, indexes))
			{
				list.IsListMostInner = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return list;
		}

		private void ReadPivotBase(Pivot pivot)
		{
			Assert(pivot != null);
			ObjectType objectType = ObjectType.Pivot;
			Indexes indexes = new Indexes();
			ReadDataRegionBase(pivot);
			if (PreRead(objectType, indexes))
			{
				pivot.ColumnCount = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				pivot.RowCount = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				pivot.CellAggregates = ReadDataAggregateInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				pivot.ProcessingInnerGrouping = ReadProcessingInnerGrouping();
			}
			if (PreRead(objectType, indexes))
			{
				pivot.RunningValues = ReadRunningValueInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				pivot.CellPostSortAggregates = ReadDataAggregateInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				pivot.CellDataElementOutput = ReadDataElementOutputType(null);
			}
			PostRead(objectType, indexes);
		}

		private void ReadTablixBase(Tablix tablix)
		{
			Assert(tablix != null);
			ObjectType objectType = ObjectType.Tablix;
			Indexes indexes = new Indexes();
			ReadDataRegionBase(tablix);
			if (PreRead(objectType, indexes))
			{
				tablix.ColumnCount = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				tablix.RowCount = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				tablix.CellAggregates = ReadDataAggregateInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				tablix.ProcessingInnerGrouping = ReadProcessingInnerGrouping();
			}
			if (PreRead(objectType, indexes))
			{
				tablix.RunningValues = ReadRunningValueInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				tablix.CellPostSortAggregates = ReadDataAggregateInfoList();
			}
			PostRead(objectType, indexes);
		}

		private CustomReportItem ReadCustomReportItemInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.CustomReportItem;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			CustomReportItem customReportItem = new CustomReportItem(parent);
			ReadTablixBase(customReportItem);
			if (PreRead(objectType, indexes))
			{
				customReportItem.Type = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				customReportItem.AltReportItem = ReadReportItemCollection(parent);
			}
			if (PreRead(objectType, indexes))
			{
				customReportItem.Columns = ReadCustomReportItemHeadingList(customReportItem);
			}
			if (PreRead(objectType, indexes))
			{
				customReportItem.Rows = ReadCustomReportItemHeadingList(customReportItem);
			}
			if (PreRead(objectType, indexes))
			{
				customReportItem.DataRowCells = ReadDataCellsList();
			}
			if (PreRead(objectType, indexes))
			{
				customReportItem.CellRunningValues = ReadRunningValueInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				customReportItem.CellExprHostIDs = ReadIntList();
			}
			if (PreRead(objectType, indexes))
			{
				customReportItem.RenderReportItem = ReadReportItemCollection(parent);
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return customReportItem;
		}

		private ChartHeading ReadChartHeading(ReportItem parent)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			return ReadChartHeadingInternals(parent);
		}

		private ChartHeading ReadChartHeadingInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.ChartHeading;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ChartHeading chartHeading = new ChartHeading();
			ReadPivotHeadingBase(chartHeading, parent);
			if (PreRead(objectType, indexes))
			{
				chartHeading.Labels = ReadExpressionInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				chartHeading.RunningValues = ReadRunningValueInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				chartHeading.ChartGroupExpression = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				chartHeading.PlotTypesLine = ReadBoolList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return chartHeading;
		}

		private ChartHeading ReadChartHeadingReference()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Reference == m_reader.Token);
			Assert(ObjectType.ChartHeading == m_reader.ObjectType);
			IDOwner definitionObject = GetDefinitionObject(m_reader.ReferenceValue);
			Assert(definitionObject is ChartHeading);
			return (ChartHeading)definitionObject;
		}

		private ChartDataPointList ReadChartDataPointList()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.ChartDataPointList == m_reader.ObjectType);
			int num = m_reader.ReadArray();
			ChartDataPointList chartDataPointList = new ChartDataPointList(num);
			for (int i = 0; i < num; i++)
			{
				chartDataPointList.Add(ReadChartDataPoint());
			}
			m_reader.ReadEndObject();
			return chartDataPointList;
		}

		private ChartDataPoint ReadChartDataPoint()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ChartDataPoint;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ChartDataPoint chartDataPoint = new ChartDataPoint();
			if (PreRead(objectType, indexes))
			{
				chartDataPoint.DataValues = ReadExpressionInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				chartDataPoint.DataLabel = ReadChartDataLabel();
			}
			if (PreRead(objectType, indexes))
			{
				if (m_intermediateFormatVersion.IsRS2005_WithMultipleActions)
				{
					chartDataPoint.Action = ReadAction();
				}
				else
				{
					ActionItem actionItem = ReadActionItem();
					if (actionItem != null)
					{
						chartDataPoint.Action = new Action(actionItem, computed: true);
					}
				}
			}
			if (PreRead(objectType, indexes))
			{
				chartDataPoint.StyleClass = ReadStyle();
			}
			if (PreRead(objectType, indexes))
			{
				chartDataPoint.MarkerType = ReadMarkerType();
			}
			if (PreRead(objectType, indexes))
			{
				chartDataPoint.MarkerSize = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				chartDataPoint.MarkerStyleClass = ReadStyle();
			}
			if (PreRead(objectType, indexes))
			{
				chartDataPoint.DataElementName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				chartDataPoint.DataElementOutput = ReadDataElementOutputType(null);
			}
			if (PreRead(objectType, indexes))
			{
				chartDataPoint.ExprHostID = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				chartDataPoint.CustomProperties = ReadDataValueList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return chartDataPoint;
		}

		private ChartDataLabel ReadChartDataLabel()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ChartDataLabel;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ChartDataLabel chartDataLabel = new ChartDataLabel();
			if (PreRead(objectType, indexes))
			{
				chartDataLabel.Visible = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				chartDataLabel.Value = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				chartDataLabel.StyleClass = ReadStyle();
			}
			if (PreRead(objectType, indexes))
			{
				chartDataLabel.Position = ReadDataLabelPosition();
			}
			if (PreRead(objectType, indexes))
			{
				chartDataLabel.Rotation = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return chartDataLabel;
		}

		private MultiChart ReadMultiChart(ReportItem parent)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			return ReadMultiChartInternals(parent);
		}

		private MultiChart ReadMultiChartInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.MultiChart;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			MultiChart multiChart = new MultiChart();
			ReadReportHierarchyNodeBase(multiChart, parent);
			if (PreRead(objectType, indexes))
			{
				multiChart.Layout = ReadMultiChartLayout();
			}
			if (PreRead(objectType, indexes))
			{
				multiChart.MaxCount = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				multiChart.SyncScale = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return multiChart;
		}

		private Axis ReadAxis()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Axis;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			Axis axis = new Axis();
			if (PreRead(objectType, indexes))
			{
				axis.Visible = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				axis.StyleClass = ReadStyle();
			}
			if (PreRead(objectType, indexes))
			{
				axis.Title = ReadChartTitle();
			}
			if (PreRead(objectType, indexes))
			{
				axis.Margin = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				axis.MajorTickMarks = ReadTickMark();
			}
			if (PreRead(objectType, indexes))
			{
				axis.MinorTickMarks = ReadTickMark();
			}
			if (PreRead(objectType, indexes))
			{
				axis.MajorGridLines = ReadGridLines();
			}
			if (PreRead(objectType, indexes))
			{
				axis.MinorGridLines = ReadGridLines();
			}
			if (PreRead(objectType, indexes))
			{
				axis.MajorInterval = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				axis.MinorInterval = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				axis.Reverse = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				axis.CrossAt = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				axis.AutoCrossAt = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				axis.Interlaced = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				axis.Scalar = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				axis.Min = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				axis.Max = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				axis.AutoScaleMin = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				axis.AutoScaleMax = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				axis.LogScale = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				axis.CustomProperties = ReadDataValueList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return axis;
		}

		private ChartTitle ReadChartTitle()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ChartTitle;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ChartTitle chartTitle = new ChartTitle();
			if (PreRead(objectType, indexes))
			{
				chartTitle.Caption = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				chartTitle.StyleClass = ReadStyle();
			}
			if (PreRead(objectType, indexes))
			{
				chartTitle.Position = ReadChartTitlePosition();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return chartTitle;
		}

		private Legend ReadLegend()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.Legend;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			Legend legend = new Legend();
			if (PreRead(objectType, indexes))
			{
				legend.Visible = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				legend.StyleClass = ReadStyle();
			}
			if (PreRead(objectType, indexes))
			{
				legend.Position = ReadLegendPosition();
			}
			if (PreRead(objectType, indexes))
			{
				legend.Layout = ReadLegendLayout();
			}
			if (PreRead(objectType, indexes))
			{
				legend.InsidePlotArea = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return legend;
		}

		private GridLines ReadGridLines()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.GridLines;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			GridLines gridLines = new GridLines();
			if (PreRead(objectType, indexes))
			{
				gridLines.ShowGridLines = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				gridLines.StyleClass = ReadStyle();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return gridLines;
		}

		private ThreeDProperties ReadThreeDProperties()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ThreeDProperties;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ThreeDProperties threeDProperties = new ThreeDProperties();
			if (PreRead(objectType, indexes))
			{
				threeDProperties.Enabled = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				threeDProperties.PerspectiveProjectionMode = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				threeDProperties.Rotation = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				threeDProperties.Inclination = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				threeDProperties.Perspective = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				threeDProperties.HeightRatio = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				threeDProperties.DepthRatio = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				threeDProperties.Shading = ReadShading();
			}
			if (PreRead(objectType, indexes))
			{
				threeDProperties.GapDepth = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				threeDProperties.WallThickness = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				threeDProperties.DrawingStyleCube = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				threeDProperties.Clustered = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return threeDProperties;
		}

		private PlotArea ReadPlotArea()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.PlotArea;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			PlotArea plotArea = new PlotArea();
			if (PreRead(objectType, indexes))
			{
				plotArea.Origin = ReadPlotAreaOrigin();
			}
			if (PreRead(objectType, indexes))
			{
				plotArea.StyleClass = ReadStyle();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return plotArea;
		}

		private Chart ReadChartInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.Chart;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			Chart chart = new Chart(parent);
			ReadPivotBase(chart);
			if (PreRead(objectType, indexes))
			{
				chart.Columns = ReadChartHeading(chart);
			}
			if (PreRead(objectType, indexes))
			{
				chart.Rows = ReadChartHeading(chart);
			}
			if (PreRead(objectType, indexes))
			{
				chart.ChartDataPoints = ReadChartDataPointList();
			}
			if (PreRead(objectType, indexes))
			{
				chart.CellRunningValues = ReadRunningValueInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				chart.MultiChart = ReadMultiChart(chart);
			}
			if (PreRead(objectType, indexes))
			{
				chart.Legend = ReadLegend();
			}
			if (PreRead(objectType, indexes))
			{
				chart.CategoryAxis = ReadAxis();
			}
			if (PreRead(objectType, indexes))
			{
				chart.ValueAxis = ReadAxis();
			}
			if (PreRead(objectType, indexes))
			{
				chart.StaticColumns = ReadChartHeadingReference();
			}
			if (PreRead(objectType, indexes))
			{
				chart.StaticRows = ReadChartHeadingReference();
			}
			if (PreRead(objectType, indexes))
			{
				chart.Type = ReadChartType();
			}
			if (PreRead(objectType, indexes))
			{
				chart.SubType = ReadChartSubType();
			}
			if (PreRead(objectType, indexes))
			{
				chart.Palette = ReadChartPalette();
			}
			if (PreRead(objectType, indexes))
			{
				chart.Title = ReadChartTitle();
			}
			if (PreRead(objectType, indexes))
			{
				chart.PointWidth = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				chart.ThreeDProperties = ReadThreeDProperties();
			}
			if (PreRead(objectType, indexes))
			{
				chart.PlotArea = ReadPlotArea();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return chart;
		}

		private ChartDataLabel.Positions ReadDataLabelPosition()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(ChartDataLabel.Positions), num));
			return (ChartDataLabel.Positions)num;
		}

		private ChartDataPoint.MarkerTypes ReadMarkerType()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(ChartDataPoint.MarkerTypes), num));
			return (ChartDataPoint.MarkerTypes)num;
		}

		private MultiChart.Layouts ReadMultiChartLayout()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(MultiChart.Layouts), num));
			return (MultiChart.Layouts)num;
		}

		private Axis.TickMarks ReadTickMark()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(Axis.TickMarks), num));
			return (Axis.TickMarks)num;
		}

		private ThreeDProperties.ShadingTypes ReadShading()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(ThreeDProperties.ShadingTypes), num));
			return (ThreeDProperties.ShadingTypes)num;
		}

		private PlotArea.Origins ReadPlotAreaOrigin()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(PlotArea.Origins), num));
			return (PlotArea.Origins)num;
		}

		private Legend.LegendLayout ReadLegendLayout()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(Legend.LegendLayout), num));
			return (Legend.LegendLayout)num;
		}

		private Legend.Positions ReadLegendPosition()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(Legend.Positions), num));
			return (Legend.Positions)num;
		}

		private ChartTitle.Positions ReadChartTitlePosition()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(ChartTitle.Positions), num));
			return (ChartTitle.Positions)num;
		}

		private Chart.ChartTypes ReadChartType()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(Chart.ChartTypes), num));
			return (Chart.ChartTypes)num;
		}

		private Chart.ChartSubTypes ReadChartSubType()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(Chart.ChartSubTypes), num));
			return (Chart.ChartSubTypes)num;
		}

		private Chart.ChartPalette ReadChartPalette()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(Chart.ChartPalette), num));
			return (Chart.ChartPalette)num;
		}

		private Matrix ReadMatrixInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.Matrix;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			Matrix matrix = new Matrix(parent);
			ReadPivotBase(matrix);
			if (PreRead(objectType, indexes))
			{
				matrix.CornerReportItems = ReadReportItemCollection(matrix);
			}
			if (PreRead(objectType, indexes))
			{
				matrix.Columns = ReadMatrixHeading(matrix);
			}
			if (PreRead(objectType, indexes))
			{
				matrix.Rows = ReadMatrixHeading(matrix);
			}
			if (PreRead(objectType, indexes))
			{
				matrix.CellReportItems = ReadReportItemCollection(matrix);
			}
			if (PreRead(objectType, indexes))
			{
				matrix.CellIDs = ReadIntList();
			}
			if (PreRead(objectType, indexes))
			{
				matrix.PropagatedPageBreakAtStart = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				matrix.PropagatedPageBreakAtEnd = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				matrix.InnerRowLevelWithPageBreak = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				matrix.MatrixRows = ReadMatrixRowList();
			}
			if (PreRead(objectType, indexes))
			{
				matrix.MatrixColumns = ReadMatrixColumnList();
			}
			if (PreRead(objectType, indexes))
			{
				matrix.GroupsBeforeRowHeaders = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				matrix.LayoutDirection = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				matrix.StaticColumns = ReadMatrixHeadingReference();
			}
			if (PreRead(objectType, indexes))
			{
				matrix.StaticRows = ReadMatrixHeadingReference();
			}
			if (PreRead(objectType, indexes))
			{
				matrix.UseOWC = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				matrix.OwcCellNames = ReadStringList();
			}
			if (PreRead(objectType, indexes))
			{
				matrix.CellDataElementName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				matrix.ColumnGroupingFixedHeader = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				matrix.RowGroupingFixedHeader = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return matrix;
		}

		private Pivot.ProcessingInnerGroupings ReadProcessingInnerGrouping()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(Pivot.ProcessingInnerGroupings), num));
			return (Pivot.ProcessingInnerGroupings)num;
		}

		private MatrixRow ReadMatrixRow()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.MatrixRow;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			MatrixRow matrixRow = new MatrixRow();
			if (PreRead(objectType, indexes))
			{
				matrixRow.Height = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				matrixRow.HeightValue = m_reader.ReadDouble();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return matrixRow;
		}

		private MatrixColumn ReadMatrixColumn()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.MatrixColumn;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			MatrixColumn matrixColumn = new MatrixColumn();
			if (PreRead(objectType, indexes))
			{
				matrixColumn.Width = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				matrixColumn.WidthValue = m_reader.ReadDouble();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return matrixColumn;
		}

		private Table ReadTableInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.Table;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			Table table = new Table(parent);
			ReadDataRegionBase(table);
			if (PreRead(objectType, indexes))
			{
				table.TableColumns = ReadTableColumnList();
			}
			if (PreRead(objectType, indexes))
			{
				table.HeaderRows = ReadTableRowList(table);
			}
			if (PreRead(objectType, indexes))
			{
				table.HeaderRepeatOnNewPage = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				table.TableGroups = ReadTableGroup(table);
			}
			if (PreRead(objectType, indexes))
			{
				table.TableDetail = ReadTableDetail(table);
			}
			if (PreRead(objectType, indexes) && m_intermediateFormatVersion.IsRS2005_WithTableDetailFix)
			{
				table.DetailGroup = ReadTableGroupReference();
			}
			if (PreRead(objectType, indexes))
			{
				table.FooterRows = ReadTableRowList(table);
			}
			if (PreRead(objectType, indexes))
			{
				table.FooterRepeatOnNewPage = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				table.PropagatedPageBreakAtStart = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				table.GroupBreakAtStart = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				table.PropagatedPageBreakAtEnd = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				table.GroupBreakAtEnd = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				table.FillPage = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				table.UseOWC = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				table.OWCNonSharedStyles = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				table.RunningValues = ReadRunningValueInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				table.DetailDataElementName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				table.DetailDataCollectionName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				table.DetailDataElementOutput = ReadDataElementOutputType(null);
			}
			if (PreRead(objectType, indexes))
			{
				table.FixedHeader = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return table;
		}

		private TableColumn ReadTableColumn()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableColumn;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			TableColumn tableColumn = new TableColumn();
			if (PreRead(objectType, indexes))
			{
				tableColumn.Width = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				tableColumn.WidthValue = m_reader.ReadDouble();
			}
			if (PreRead(objectType, indexes))
			{
				tableColumn.Visibility = ReadVisibility();
			}
			if (PreRead(objectType, indexes))
			{
				tableColumn.FixedHeader = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return tableColumn;
		}

		private OWCChart ReadOWCChartInternals(ReportItem parent)
		{
			ObjectType objectType = ObjectType.OWCChart;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			OWCChart oWCChart = new OWCChart(parent);
			ReadDataRegionBase(oWCChart);
			if (PreRead(objectType, indexes))
			{
				oWCChart.ChartData = ReadChartColumnList();
			}
			if (PreRead(objectType, indexes))
			{
				oWCChart.ChartDefinition = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				oWCChart.DetailRunningValues = ReadRunningValueInfoList();
			}
			if (PreRead(objectType, indexes))
			{
				oWCChart.RunningValues = ReadRunningValueInfoList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return oWCChart;
		}

		private ChartColumn ReadChartColumn()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ChartColumn;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ChartColumn chartColumn = new ChartColumn();
			if (PreRead(objectType, indexes))
			{
				chartColumn.Name = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				chartColumn.Value = ReadExpressionInfo();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return chartColumn;
		}

		private DataValue ReadDataValue()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.DataValue;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			DataValue dataValue = new DataValue();
			if (PreRead(objectType, indexes))
			{
				dataValue.Name = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				dataValue.Value = ReadExpressionInfo();
			}
			if (PreRead(objectType, indexes))
			{
				dataValue.ExprHostID = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return dataValue;
		}

		private ParameterInfo ReadParameterInfo()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ParameterInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ParameterInfo parameterInfo = new ParameterInfo();
			ReadParameterBase(parameterInfo);
			RegisterParameterInfo(parameterInfo);
			if (PreRead(objectType, indexes))
			{
				parameterInfo.IsUserSupplied = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				parameterInfo.Values = ReadVariants();
			}
			if (PreRead(objectType, indexes))
			{
				parameterInfo.DynamicValidValues = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				parameterInfo.DynamicDefaultValue = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				parameterInfo.DependencyList = ReadParameterInfoRefCollection();
			}
			if (PreRead(objectType, indexes))
			{
				parameterInfo.ValidValues = ReadValidValueList();
			}
			if (PreRead(objectType, indexes))
			{
				parameterInfo.Labels = ReadStrings();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return parameterInfo;
		}

		private ProcessingMessage ReadProcessingMessage()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ProcessingMessage;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ProcessingMessage processingMessage = new ProcessingMessage();
			if (PreRead(objectType, indexes))
			{
				processingMessage.Code = ReadProcessingErrorCode();
			}
			if (PreRead(objectType, indexes))
			{
				processingMessage.Severity = ReadProcessingErrorSeverity();
			}
			if (PreRead(objectType, indexes))
			{
				processingMessage.ObjectType = ReadProcessingErrorObjectType();
			}
			if (PreRead(objectType, indexes))
			{
				processingMessage.ObjectName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				processingMessage.PropertyName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				processingMessage.Message = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				processingMessage.ProcessingMessages = ReadProcessingMessageList();
			}
			if (PreRead(objectType, indexes))
			{
				processingMessage.CommonCode = ReadCommonErrorCode();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return processingMessage;
		}

		private DataValueInstance ReadDataValueInstance()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.DataValueInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			DataValueInstance dataValueInstance = new DataValueInstance();
			if (PreRead(objectType, indexes))
			{
				dataValueInstance.Name = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				dataValueInstance.Value = ReadVariant();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return dataValueInstance;
		}

		private ProcessingErrorCode ReadProcessingErrorCode()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(ProcessingErrorCode), num));
			return (ProcessingErrorCode)num;
		}

		private ErrorCode ReadCommonErrorCode()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(ErrorCode), num));
			return (ErrorCode)num;
		}

		private Severity ReadProcessingErrorSeverity()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(Severity), num));
			return (Severity)num;
		}

		private Microsoft.ReportingServices.ReportProcessing.ObjectType ReadProcessingErrorObjectType()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(Microsoft.ReportingServices.ReportProcessing.ObjectType), num));
			return (Microsoft.ReportingServices.ReportProcessing.ObjectType)num;
		}

		private DataType ReadDataType()
		{
			int num = m_reader.ReadEnum();
			Assert(Enum.IsDefined(typeof(DataType), num));
			return (DataType)num;
		}

		private BookmarkInformation ReadBookmarkInformation()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.BookmarkInformation;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			BookmarkInformation bookmarkInformation = new BookmarkInformation();
			if (PreRead(objectType, indexes))
			{
				bookmarkInformation.Id = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				bookmarkInformation.Page = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return bookmarkInformation;
		}

		private DrillthroughInformation ReadDrillthroughInformation(bool hasTokensIDs)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.DrillthroughInformation;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			DrillthroughInformation drillthroughInformation = new DrillthroughInformation();
			if (PreRead(objectType, indexes))
			{
				drillthroughInformation.ReportName = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				drillthroughInformation.ReportParameters = ReadDrillthroughParameters();
			}
			if (hasTokensIDs && PreRead(objectType, indexes))
			{
				drillthroughInformation.DataSetTokenIDs = ReadIntList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return drillthroughInformation;
		}

		private SenderInformation ReadSenderInformation()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.SenderInformation;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			SenderInformation senderInformation = new SenderInformation();
			if (PreRead(objectType, indexes))
			{
				senderInformation.StartHidden = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				senderInformation.ReceiverUniqueNames = ReadIntList();
			}
			if (PreRead(objectType, indexes))
			{
				senderInformation.ContainerUniqueNames = m_reader.ReadInt32s();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return senderInformation;
		}

		private ReceiverInformation ReadReceiverInformation()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReceiverInformation;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ReceiverInformation receiverInformation = new ReceiverInformation();
			if (PreRead(objectType, indexes))
			{
				receiverInformation.StartHidden = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				receiverInformation.SenderUniqueName = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return receiverInformation;
		}

		private SortFilterEventInfo ReadSortFilterEventInfo(bool getDefinition)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.SortFilterEventInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			SortFilterEventInfo sortFilterEventInfo = new SortFilterEventInfo();
			if (PreRead(objectType, indexes))
			{
				sortFilterEventInfo.EventSource = (TextBox)ReadReportItemReference(getDefinition);
			}
			if (PreRead(objectType, indexes))
			{
				sortFilterEventInfo.EventSourceScopeInfo = ReadVariantLists(convertDBNull: true);
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return sortFilterEventInfo;
		}

		private void ReadInfoBaseBase(InfoBase infoBase)
		{
			Assert(infoBase != null);
			ObjectType objectType = ObjectType.InfoBase;
			Indexes indexes = new Indexes();
			PostRead(objectType, indexes);
		}

		private OffsetInfo ReadSimpleOffsetInfo()
		{
			return new OffsetInfo
			{
				Offset = m_reader.ReadInt64()
			};
		}

		private OffsetInfo ReadOffsetInfo()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.OffsetInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			OffsetInfo offsetInfo = new OffsetInfo();
			ReadInfoBaseBase(offsetInfo);
			if (PreRead(objectType, indexes))
			{
				offsetInfo.Offset = m_reader.ReadInt64();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return offsetInfo;
		}

		private void ReadInstanceInfoBase(InstanceInfo instanceInfo)
		{
			Assert(instanceInfo != null);
			ObjectType objectType = ObjectType.InstanceInfo;
			Indexes indexes = new Indexes();
			ReadInfoBaseBase(instanceInfo);
			PostRead(objectType, indexes);
		}

		private void ReadReportItemInstanceInfoBase(ReportItemInstanceInfo instanceInfo)
		{
			Assert(instanceInfo != null);
			ObjectType objectType = ObjectType.ReportItemInstanceInfo;
			Indexes indexes = new Indexes();
			ReadInstanceInfoBase(instanceInfo);
			bool flag = false;
			ReportItem reportItemDef = instanceInfo.ReportItemDef;
			if (m_intermediateFormatVersion.IsRS2000_WithUnusedFieldsOptimization)
			{
				flag = true;
			}
			if ((!flag || (reportItemDef.StyleClass != null && reportItemDef.StyleClass.ExpressionList != null)) && PreRead(objectType, indexes))
			{
				instanceInfo.StyleAttributeValues = ReadVariants();
			}
			if ((!flag || reportItemDef.Visibility != null) && PreRead(objectType, indexes))
			{
				instanceInfo.StartHidden = m_reader.ReadBoolean();
			}
			if ((!flag || reportItemDef.Label != null) && PreRead(objectType, indexes))
			{
				instanceInfo.Label = m_reader.ReadString();
			}
			if ((!flag || reportItemDef.Bookmark != null) && PreRead(objectType, indexes))
			{
				instanceInfo.Bookmark = m_reader.ReadString();
			}
			if ((!flag || reportItemDef.ToolTip != null) && PreRead(objectType, indexes))
			{
				instanceInfo.ToolTip = m_reader.ReadString();
			}
			if ((!flag || reportItemDef.CustomProperties != null) && PreRead(objectType, indexes))
			{
				instanceInfo.CustomPropertyInstances = ReadDataValueInstanceList();
			}
			PostRead(objectType, indexes);
		}

		private NonComputedUniqueNames ReadNonComputedUniqueNames()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.NonComputedUniqueNames;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			NonComputedUniqueNames nonComputedUniqueNames = new NonComputedUniqueNames();
			if (PreRead(objectType, indexes))
			{
				nonComputedUniqueNames.UniqueName = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				nonComputedUniqueNames.ChildrenUniqueNames = ReadNonComputedUniqueNamess();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return nonComputedUniqueNames;
		}

		private void ReadInstanceInfoOwnerBase(InstanceInfoOwner owner)
		{
			Assert(owner != null);
			ObjectType objectType = ObjectType.InstanceInfoOwner;
			Indexes indexes = new Indexes();
			if (PreRead(objectType, indexes))
			{
				if (m_intermediateFormatVersion.IsRS2000_WithOtherPageChunkSplit)
				{
					owner.OffsetInfo = ReadSimpleOffsetInfo();
				}
				else
				{
					owner.OffsetInfo = ReadOffsetInfo();
				}
			}
			PostRead(objectType, indexes);
		}

		private ReportItemInstance ReadReportItemInstance(ReportItem reportItemDef)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			if (ObjectType.LineInstance == m_reader.ObjectType)
			{
				return ReadLineInstanceInternals(reportItemDef);
			}
			if (ObjectType.RectangleInstance == m_reader.ObjectType)
			{
				return ReadRectangleInstanceInternals(reportItemDef);
			}
			if (ObjectType.ImageInstance == m_reader.ObjectType)
			{
				return ReadImageInstanceInternals(reportItemDef);
			}
			if (ObjectType.CheckBoxInstance == m_reader.ObjectType)
			{
				return ReadCheckBoxInstanceInternals(reportItemDef);
			}
			if (ObjectType.TextBoxInstance == m_reader.ObjectType)
			{
				return ReadTextBoxInstanceInternals(reportItemDef);
			}
			if (ObjectType.SubReportInstance == m_reader.ObjectType)
			{
				return ReadSubReportInstanceInternals(reportItemDef);
			}
			if (ObjectType.ActiveXControlInstance == m_reader.ObjectType)
			{
				return ReadActiveXControlInstanceInternals(reportItemDef);
			}
			if (ObjectType.ListInstance == m_reader.ObjectType)
			{
				return ReadListInstanceInternals(reportItemDef);
			}
			if (ObjectType.MatrixInstance == m_reader.ObjectType)
			{
				return ReadMatrixInstanceInternals(reportItemDef);
			}
			if (ObjectType.TableInstance == m_reader.ObjectType)
			{
				return ReadTableInstanceInternals(reportItemDef);
			}
			if (ObjectType.ChartInstance == m_reader.ObjectType)
			{
				return ReadChartInstanceInternals(reportItemDef);
			}
			if (ObjectType.CustomReportItemInstance == m_reader.ObjectType)
			{
				Assert(reportItemDef is CustomReportItem);
				return ReadCustomReportItemInstanceInternals(reportItemDef as CustomReportItem);
			}
			Assert(ObjectType.OWCChartInstance == m_reader.ObjectType);
			return ReadOWCChartInstanceInternals(reportItemDef);
		}

		private void ReadReportItemInstanceBase(ReportItemInstance reportItemInstance, ReportItem reportItemDef)
		{
			Global.Tracer.Assert(reportItemDef != null, "(null != reportItemDef)");
			ReadReportItemInstanceBase(reportItemInstance, ref reportItemDef);
		}

		private void ReadReportItemInstanceBase(ReportItemInstance reportItemInstance, ref ReportItem reportItemDef)
		{
			Assert(reportItemInstance != null);
			ObjectType objectType = ObjectType.ReportItemInstance;
			Indexes indexes = new Indexes();
			ReadInstanceInfoOwnerBase(reportItemInstance);
			if (PreRead(objectType, indexes))
			{
				if (-1 == m_currentUniqueName)
				{
					reportItemInstance.UniqueName = m_reader.ReadInt32();
				}
				else
				{
					Global.Tracer.Assert(m_intermediateFormatVersion.IsRS2005_WithTableOptimizations);
					reportItemInstance.UniqueName = m_currentUniqueName++;
				}
			}
			if (reportItemDef == null)
			{
				reportItemDef = ReadReportItemReference(getDefinition: true);
				indexes.CurrentIndex++;
			}
			Global.Tracer.Assert(reportItemDef != null, "(null != reportItemDef)");
			reportItemInstance.ReportItemDef = reportItemDef;
			PostRead(objectType, indexes);
		}

		private ReportItemInstance ReadReportItemInstanceReference()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Reference == m_reader.Token);
			Assert(ObjectType.OWCChartInstance == m_reader.ObjectType || ObjectType.ChartInstance == m_reader.ObjectType);
			ReportItemInstance instanceObject = GetInstanceObject(m_reader.ReferenceValue);
			Assert(instanceObject is OWCChartInstance || instanceObject is ChartInstance);
			return instanceObject;
		}

		private ReportInstance ReadReportInstance(Report reportDef)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReportInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ReportInstance reportInstance = new ReportInstance();
			ReadReportItemInstanceBase(reportInstance, reportDef);
			if (PreRead(objectType, indexes))
			{
				reportInstance.ReportItemColInstance = ReadReportItemColInstance(reportDef.ReportItems);
			}
			if (PreRead(objectType, indexes))
			{
				reportInstance.Language = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				reportInstance.NumberOfPages = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return reportInstance;
		}

		private ReportItemColInstance ReadReportItemColInstance(ReportItemCollection reportItemsDef)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ReportItemColInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ReportItemColInstance reportItemColInstance = new ReportItemColInstance();
			ReadInstanceInfoOwnerBase(reportItemColInstance);
			if (PreRead(objectType, indexes))
			{
				reportItemColInstance.ReportItemInstances = ReadReportItemInstanceList(reportItemsDef);
			}
			reportItemColInstance.ReportItemColDef = reportItemsDef;
			if (PreRead(objectType, indexes))
			{
				reportItemColInstance.ChildrenStartAndEndPages = ReadRenderingPagesRangesList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return reportItemColInstance;
		}

		private LineInstance ReadLineInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.LineInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			LineInstance lineInstance = new LineInstance();
			ReadReportItemInstanceBase(lineInstance, ref reportItemDef);
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return lineInstance;
		}

		private void UpdateUniqueNameForAction(Action actionDef)
		{
			if (-1 != m_currentUniqueName && actionDef != null)
			{
				Global.Tracer.Assert(m_intermediateFormatVersion.IsRS2005_WithTableOptimizations);
				if ((actionDef.StyleClass != null && actionDef.StyleClass.ExpressionList != null && 0 < actionDef.StyleClass.ExpressionList.Count) || actionDef.ComputedActionItemsCount > 0)
				{
					m_currentUniqueName++;
				}
			}
		}

		private TextBoxInstance ReadTextBoxInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.TextBoxInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			TextBoxInstance textBoxInstance = new TextBoxInstance();
			ReadReportItemInstanceBase(textBoxInstance, ref reportItemDef);
			UpdateUniqueNameForAction(((TextBox)reportItemDef).Action);
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return textBoxInstance;
		}

		private RectangleInstance ReadRectangleInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.RectangleInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			RectangleInstance rectangleInstance = new RectangleInstance();
			ReadReportItemInstanceBase(rectangleInstance, ref reportItemDef);
			if (PreRead(objectType, indexes))
			{
				rectangleInstance.ReportItemColInstance = ReadReportItemColInstance(((Rectangle)reportItemDef).ReportItems);
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return rectangleInstance;
		}

		private CheckBoxInstance ReadCheckBoxInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.CheckBoxInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			CheckBoxInstance checkBoxInstance = new CheckBoxInstance();
			ReadReportItemInstanceBase(checkBoxInstance, ref reportItemDef);
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return checkBoxInstance;
		}

		private ImageInstance ReadImageInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.ImageInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ImageInstance imageInstance = new ImageInstance();
			ReadReportItemInstanceBase(imageInstance, ref reportItemDef);
			UpdateUniqueNameForAction(((Image)reportItemDef).Action);
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return imageInstance;
		}

		private SubReportInstance ReadSubReportInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.SubReportInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			SubReportInstance subReportInstance = new SubReportInstance();
			ReadReportItemInstanceBase(subReportInstance, ref reportItemDef);
			if (PreRead(objectType, indexes))
			{
				subReportInstance.ReportInstance = ReadReportInstance(((SubReport)reportItemDef).Report);
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return subReportInstance;
		}

		private ActiveXControlInstance ReadActiveXControlInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.ActiveXControlInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ActiveXControlInstance activeXControlInstance = new ActiveXControlInstance();
			ReadReportItemInstanceBase(activeXControlInstance, ref reportItemDef);
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return activeXControlInstance;
		}

		private ListInstance ReadListInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.ListInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ListInstance listInstance = new ListInstance();
			ReadReportItemInstanceBase(listInstance, ref reportItemDef);
			if (PreRead(objectType, indexes))
			{
				listInstance.ListContents = ReadListContentInstanceList((List)reportItemDef);
			}
			if (PreRead(objectType, indexes))
			{
				listInstance.ChildrenStartAndEndPages = ReadRenderingPagesRangesList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return listInstance;
		}

		private ListContentInstance ReadListContentInstance(List listDef)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ListContentInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ListContentInstance listContentInstance = new ListContentInstance();
			ReadInstanceInfoOwnerBase(listContentInstance);
			if (PreRead(objectType, indexes))
			{
				listContentInstance.UniqueName = m_reader.ReadInt32();
			}
			listContentInstance.ListDef = listDef;
			if (PreRead(objectType, indexes))
			{
				listContentInstance.ReportItemColInstance = ReadReportItemColInstance(listDef.ReportItems);
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return listContentInstance;
		}

		private MatrixInstance ReadMatrixInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.MatrixInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			MatrixInstance matrixInstance = new MatrixInstance();
			Matrix matrix = (Matrix)reportItemDef;
			ReadReportItemInstanceBase(matrixInstance, ref reportItemDef);
			if (PreRead(objectType, indexes))
			{
				matrixInstance.CornerContent = ReadReportItemInstance(matrix.CornerReportItem);
			}
			if (PreRead(objectType, indexes))
			{
				matrixInstance.ColumnInstances = ReadMatrixHeadingInstanceList(matrix.Columns);
			}
			if (PreRead(objectType, indexes))
			{
				matrixInstance.RowInstances = ReadMatrixHeadingInstanceList(matrix.Rows);
			}
			if (PreRead(objectType, indexes))
			{
				matrixInstance.Cells = ReadMatrixCellInstancesList();
			}
			if (PreRead(objectType, indexes))
			{
				matrixInstance.InstanceCountOfInnerRowWithPageBreak = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				matrixInstance.ChildrenStartAndEndPages = ReadRenderingPagesRangesList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return matrixInstance;
		}

		private MatrixHeadingInstance ReadMatrixHeadingInstance(MatrixHeading headingDef, int index, int totalCount)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.MatrixHeadingInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			MatrixHeadingInstance matrixHeadingInstance = new MatrixHeadingInstance();
			bool flag = false;
			if (headingDef.Grouping != null && headingDef.Subtotal != null && ((index == 0 && Subtotal.PositionType.Before == headingDef.Subtotal.Position) || (totalCount - 1 == index && headingDef.Subtotal.Position == Subtotal.PositionType.After)))
			{
				flag = true;
			}
			ReadInstanceInfoOwnerBase(matrixHeadingInstance);
			if (PreRead(objectType, indexes))
			{
				matrixHeadingInstance.UniqueName = m_reader.ReadInt32();
			}
			matrixHeadingInstance.MatrixHeadingDef = headingDef;
			if (PreRead(objectType, indexes))
			{
				ReportItem reportItem = null;
				if (headingDef.Grouping != null)
				{
					reportItem = ((!flag) ? headingDef.ReportItem : headingDef.Subtotal.ReportItem);
				}
				else
				{
					Global.Tracer.Assert(headingDef.ReportItems != null, "(null != headingDef.ReportItems)");
					reportItem = headingDef.ReportItems[index];
				}
				matrixHeadingInstance.Content = ReadReportItemInstance(reportItem);
			}
			if (PreRead(objectType, indexes))
			{
				MatrixHeading subHeading = headingDef.SubHeading;
				if (flag)
				{
					while (subHeading != null && subHeading.Grouping != null)
					{
						subHeading = subHeading.SubHeading;
					}
				}
				matrixHeadingInstance.SubHeadingInstances = ReadMatrixHeadingInstanceList(subHeading);
			}
			if (PreRead(objectType, indexes))
			{
				matrixHeadingInstance.IsSubtotal = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				matrixHeadingInstance.ChildrenStartAndEndPages = ReadRenderingPagesRangesList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			if (matrixHeadingInstance.IsSubtotal && matrixHeadingInstance.MatrixHeadingDef.Subtotal.StyleClass != null)
			{
				RegisterMatrixHeadingInstanceObject(matrixHeadingInstance);
			}
			return matrixHeadingInstance;
		}

		internal MatrixCellInstance ReadMatrixCellInstanceBase()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Object == m_reader.Token);
			if (ObjectType.MatrixCellInstance == m_reader.ObjectType)
			{
				MatrixCellInstance matrixCellInstance = new MatrixCellInstance();
				ReadMatrixCellInstance(matrixCellInstance);
				return matrixCellInstance;
			}
			Assert(ObjectType.MatrixSubtotalCellInstance == m_reader.ObjectType);
			MatrixSubtotalCellInstance matrixSubtotalCellInstance = new MatrixSubtotalCellInstance();
			ReadMatrixSubtotalCellInstance(matrixSubtotalCellInstance);
			return matrixSubtotalCellInstance;
		}

		private void ReadMatrixCellInstance(MatrixCellInstance instance)
		{
			ObjectType objectType = ObjectType.MatrixCellInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ReadInstanceInfoOwnerBase(instance);
			ReportItem reportItemDef = null;
			if (PreRead(objectType, indexes))
			{
				reportItemDef = ReadReportItemReference(getDefinition: true);
			}
			if (PreRead(objectType, indexes))
			{
				instance.Content = ReadReportItemInstance(reportItemDef);
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
		}

		internal void ReadMatrixSubtotalCellInstance(MatrixSubtotalCellInstance instance)
		{
			Assert(Token.Object == m_reader.Token);
			Assert(ObjectType.MatrixSubtotalCellInstance == m_reader.ObjectType);
			ReadInstanceInfoOwnerBase(instance);
			Assert(m_reader.Read());
			ReadMatrixCellInstance(instance);
			int uniqueName = m_reader.ReadInt32();
			instance.SubtotalHeadingInstance = GetMatrixHeadingInstanceObject(uniqueName);
			m_reader.ReadEndObject();
		}

		private MultiChartInstance ReadMultiChartInstance(Chart chartDef)
		{
			Assert(m_reader.Read());
			ObjectType objectType = ObjectType.MultiChartInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			MultiChartInstance multiChartInstance = new MultiChartInstance();
			if (PreRead(objectType, indexes))
			{
				multiChartInstance.ColumnInstances = ReadChartHeadingInstanceList(chartDef.Columns);
			}
			if (PreRead(objectType, indexes))
			{
				multiChartInstance.RowInstances = ReadChartHeadingInstanceList(chartDef.Rows);
			}
			if (PreRead(objectType, indexes))
			{
				multiChartInstance.DataPoints = ReadChartDataPointInstancesList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return multiChartInstance;
		}

		private ChartInstance ReadChartInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.ChartInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ChartInstance chartInstance = new ChartInstance();
			ReadReportItemInstanceBase(chartInstance, ref reportItemDef);
			if (PreRead(objectType, indexes))
			{
				chartInstance.MultiCharts = ReadMultiChartInstanceList((Chart)reportItemDef);
			}
			RegisterInstanceObject(chartInstance);
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return chartInstance;
		}

		private ChartHeadingInstance ReadChartHeadingInstance(ChartHeading headingDef)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ChartHeadingInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ChartHeadingInstance chartHeadingInstance = new ChartHeadingInstance();
			ReadInstanceInfoOwnerBase(chartHeadingInstance);
			if (PreRead(objectType, indexes))
			{
				chartHeadingInstance.UniqueName = m_reader.ReadInt32();
			}
			chartHeadingInstance.ChartHeadingDef = headingDef;
			if (PreRead(objectType, indexes))
			{
				chartHeadingInstance.SubHeadingInstances = ReadChartHeadingInstanceList(headingDef.SubHeading);
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return chartHeadingInstance;
		}

		private ChartDataPointInstance ReadChartDataPointInstance()
		{
			Assert(m_reader.Read());
			ObjectType objectType = ObjectType.ChartDataPointInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ChartDataPointInstance chartDataPointInstance = new ChartDataPointInstance();
			ReadInstanceInfoOwnerBase(chartDataPointInstance);
			if (PreRead(objectType, indexes))
			{
				chartDataPointInstance.UniqueName = m_reader.ReadInt32();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return chartDataPointInstance;
		}

		private AxisInstance ReadAxisInstance()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.AxisInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			AxisInstance axisInstance = new AxisInstance();
			if (PreRead(objectType, indexes))
			{
				axisInstance.UniqueName = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				axisInstance.Title = ReadChartTitleInstance();
			}
			if (PreRead(objectType, indexes))
			{
				axisInstance.StyleAttributeValues = ReadVariants();
			}
			if (PreRead(objectType, indexes))
			{
				axisInstance.MajorGridLinesStyleAttributeValues = ReadVariants();
			}
			if (PreRead(objectType, indexes))
			{
				axisInstance.MinorGridLinesStyleAttributeValues = ReadVariants();
			}
			if (PreRead(objectType, indexes))
			{
				axisInstance.MinValue = ReadVariant();
			}
			if (PreRead(objectType, indexes))
			{
				axisInstance.MaxValue = ReadVariant();
			}
			if (PreRead(objectType, indexes))
			{
				axisInstance.CrossAtValue = ReadVariant();
			}
			if (PreRead(objectType, indexes))
			{
				axisInstance.MajorIntervalValue = ReadVariant();
			}
			if (PreRead(objectType, indexes))
			{
				axisInstance.MinorIntervalValue = ReadVariant();
			}
			if (PreRead(objectType, indexes))
			{
				axisInstance.CustomPropertyInstances = ReadDataValueInstanceList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return axisInstance;
		}

		private ChartTitleInstance ReadChartTitleInstance()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.ChartTitleInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			ChartTitleInstance chartTitleInstance = new ChartTitleInstance();
			if (PreRead(objectType, indexes))
			{
				chartTitleInstance.UniqueName = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				chartTitleInstance.Caption = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				chartTitleInstance.StyleAttributeValues = ReadVariants();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return chartTitleInstance;
		}

		private TableInstance ReadTableInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.TableInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			TableInstance tableInstance = new TableInstance();
			Table table = (Table)reportItemDef;
			ReadReportItemInstanceBase(tableInstance, ref reportItemDef);
			if (PreRead(objectType, indexes) && (!m_intermediateFormatVersion.IsRS2005_WithTableOptimizations || table.HeaderRows != null))
			{
				tableInstance.HeaderRowInstances = ReadTableRowInstances(table.HeaderRows, -1);
			}
			if (PreRead(objectType, indexes) && (!m_intermediateFormatVersion.IsRS2005_WithTableOptimizations || table.TableGroups != null))
			{
				tableInstance.TableGroupInstances = ReadTableGroupInstanceList(table.TableGroups);
			}
			tableInstance.TableDetailInstances = ReadTableDetailInstances(table, table.TableGroups, objectType, indexes);
			if (PreRead(objectType, indexes) && (!m_intermediateFormatVersion.IsRS2005_WithTableOptimizations || table.FooterRows != null))
			{
				tableInstance.FooterRowInstances = ReadTableRowInstances(table.FooterRows, -1);
			}
			if (PreRead(objectType, indexes))
			{
				tableInstance.ChildrenStartAndEndPages = ReadRenderingPagesRangesList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return tableInstance;
		}

		private TableDetailInstanceList ReadTableDetailInstances(Table tableDef, TableGroup tableGroup, ObjectType objectType, Indexes indexes)
		{
			TableDetailInstanceList result = null;
			bool flag = false;
			if (PreRead(objectType, indexes) && tableGroup == null && tableDef.TableDetail != null && tableDef.TableDetail.SimpleDetailRows)
			{
				Global.Tracer.Assert(m_intermediateFormatVersion.IsRS2005_WithTableOptimizations);
				flag = true;
				m_currentUniqueName = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes) && (!m_intermediateFormatVersion.IsRS2005_WithTableOptimizations || (tableGroup == null && tableDef.TableDetail != null)))
			{
				result = ReadTableDetailInstanceList(tableDef.TableDetail);
			}
			if (flag)
			{
				m_currentUniqueName = -1;
			}
			return result;
		}

		private TableGroupInstance ReadTableGroupInstance(TableGroup groupDef)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableGroupInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			TableGroupInstance tableGroupInstance = new TableGroupInstance();
			ReadInstanceInfoOwnerBase(tableGroupInstance);
			if (PreRead(objectType, indexes))
			{
				tableGroupInstance.UniqueName = m_reader.ReadInt32();
			}
			tableGroupInstance.TableGroupDef = groupDef;
			if (PreRead(objectType, indexes) && (!m_intermediateFormatVersion.IsRS2005_WithTableOptimizations || groupDef.HeaderRows != null))
			{
				tableGroupInstance.HeaderRowInstances = ReadTableRowInstances(groupDef.HeaderRows, -1);
			}
			if (PreRead(objectType, indexes) && (!m_intermediateFormatVersion.IsRS2005_WithTableOptimizations || groupDef.FooterRows != null))
			{
				tableGroupInstance.FooterRowInstances = ReadTableRowInstances(groupDef.FooterRows, -1);
			}
			if (PreRead(objectType, indexes) && (!m_intermediateFormatVersion.IsRS2005_WithTableOptimizations || groupDef.SubGroup != null))
			{
				tableGroupInstance.SubGroupInstances = ReadTableGroupInstanceList(groupDef.SubGroup);
			}
			tableGroupInstance.TableDetailInstances = ReadTableDetailInstances((Table)groupDef.DataRegionDef, groupDef.SubGroup, objectType, indexes);
			if (PreRead(objectType, indexes))
			{
				tableGroupInstance.ChildrenStartAndEndPages = ReadRenderingPagesRangesList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return tableGroupInstance;
		}

		private TableDetailInstance ReadTableDetailInstance(TableDetail detailDef)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableDetailInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			TableDetailInstance tableDetailInstance = new TableDetailInstance();
			ReadInstanceInfoOwnerBase(tableDetailInstance);
			if (PreRead(objectType, indexes))
			{
				if (-1 == m_currentUniqueName)
				{
					tableDetailInstance.UniqueName = m_reader.ReadInt32();
				}
				else
				{
					tableDetailInstance.UniqueName = m_currentUniqueName++;
				}
			}
			tableDetailInstance.TableDetailDef = detailDef;
			int currentUniqueName = m_currentUniqueName;
			if (-1 != m_currentUniqueName && detailDef.DetailRows != null)
			{
				for (int i = 0; i < detailDef.DetailRows.Count; i++)
				{
					m_currentUniqueName++;
					if (detailDef.DetailRows[i] != null)
					{
						ReportItemCollection reportItems = detailDef.DetailRows[i].ReportItems;
						if (reportItems.NonComputedReportItems != null)
						{
							m_currentUniqueName += reportItems.NonComputedReportItems.Count;
						}
					}
				}
			}
			if (PreRead(objectType, indexes))
			{
				tableDetailInstance.DetailRowInstances = ReadTableRowInstances(detailDef.DetailRows, currentUniqueName);
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return tableDetailInstance;
		}

		internal TableDetailInstanceInfo ReadTableDetailInstanceInfo()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableDetailInstanceInfo;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			TableDetailInstanceInfo tableDetailInstanceInfo = new TableDetailInstanceInfo();
			ReadInstanceInfoBase(tableDetailInstanceInfo);
			if (PreRead(objectType, indexes))
			{
				tableDetailInstanceInfo.StartHidden = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return tableDetailInstanceInfo;
		}

		private TableRowInstance ReadTableRowInstance(TableRowList rowDefs, int index, int rowUniqueName)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableRowInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			TableRowInstance tableRowInstance = new TableRowInstance();
			ReadInstanceInfoOwnerBase(tableRowInstance);
			if (PreRead(objectType, indexes))
			{
				if (-1 == rowUniqueName)
				{
					tableRowInstance.UniqueName = m_reader.ReadInt32();
				}
				else
				{
					tableRowInstance.UniqueName = rowUniqueName;
				}
			}
			tableRowInstance.TableRowDef = rowDefs[index];
			if (PreRead(objectType, indexes))
			{
				tableRowInstance.TableRowReportItemColInstance = ReadReportItemColInstance(tableRowInstance.TableRowDef.ReportItems);
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return tableRowInstance;
		}

		private TableColumnInstance ReadTableColumnInstance()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.TableColumnInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			TableColumnInstance tableColumnInstance = new TableColumnInstance();
			if (PreRead(objectType, indexes))
			{
				tableColumnInstance.UniqueName = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				tableColumnInstance.StartHidden = m_reader.ReadBoolean();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return tableColumnInstance;
		}

		private OWCChartInstance ReadOWCChartInstanceInternals(ReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.OWCChartInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			OWCChartInstance oWCChartInstance = new OWCChartInstance();
			ReadReportItemInstanceBase(oWCChartInstance, ref reportItemDef);
			RegisterInstanceObject(oWCChartInstance);
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return oWCChartInstance;
		}

		private CustomReportItemInstance ReadCustomReportItemInstanceInternals(CustomReportItem reportItemDef)
		{
			ObjectType objectType = ObjectType.CustomReportItemInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			CustomReportItemInstance customReportItemInstance = new CustomReportItemInstance();
			ReadReportItemInstanceBase(customReportItemInstance, reportItemDef);
			if (PreRead(objectType, indexes))
			{
				if (reportItemDef.RenderReportItem != null && 1 == reportItemDef.RenderReportItem.Count)
				{
					customReportItemInstance.AltReportItemColInstance = ReadReportItemColInstance(reportItemDef.RenderReportItem);
				}
				else
				{
					customReportItemInstance.AltReportItemColInstance = ReadReportItemColInstance(reportItemDef.AltReportItem);
				}
			}
			if (PreRead(objectType, indexes))
			{
				customReportItemInstance.ColumnInstances = ReadCustomReportItemHeadingInstanceList(reportItemDef.Columns);
			}
			if (PreRead(objectType, indexes))
			{
				customReportItemInstance.RowInstances = ReadCustomReportItemHeadingInstanceList(reportItemDef.Rows);
			}
			if (PreRead(objectType, indexes))
			{
				customReportItemInstance.Cells = ReadCustomReportItemCellInstancesList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return customReportItemInstance;
		}

		private CustomReportItemHeadingInstance ReadCustomReportItemHeadingInstance(CustomReportItemHeadingList headingDef, int index, int totalCount)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.CustomReportItemHeadingInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			CustomReportItemHeadingInstance customReportItemHeadingInstance = new CustomReportItemHeadingInstance();
			if (PreRead(objectType, indexes))
			{
				customReportItemHeadingInstance.SubHeadingInstances = ReadCustomReportItemHeadingInstanceList(headingDef);
			}
			if (PreRead(objectType, indexes))
			{
				customReportItemHeadingInstance.HeadingDefinition = ReadCustomReportItemHeadingReference();
			}
			if (PreRead(objectType, indexes))
			{
				customReportItemHeadingInstance.HeadingCellIndex = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				customReportItemHeadingInstance.HeadingSpan = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				customReportItemHeadingInstance.CustomPropertyInstances = ReadDataValueInstanceList();
			}
			if (PreRead(objectType, indexes))
			{
				customReportItemHeadingInstance.Label = m_reader.ReadString();
			}
			if (PreRead(objectType, indexes))
			{
				customReportItemHeadingInstance.GroupExpressionValues = ReadVariantList(convertDBNull: false);
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return customReportItemHeadingInstance;
		}

		internal CustomReportItemCellInstance ReadCustomReportItemCellInstance()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.CustomReportItemCellInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			CustomReportItemCellInstance customReportItemCellInstance = new CustomReportItemCellInstance();
			if (PreRead(objectType, indexes))
			{
				customReportItemCellInstance.RowIndex = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				customReportItemCellInstance.ColumnIndex = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				customReportItemCellInstance.DataValueInstances = ReadDataValueInstanceList();
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return customReportItemCellInstance;
		}

		private PageSectionInstance ReadPageSectionInstance(PageSection pageSectionDef)
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.PageSectionInstance;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			PageSectionInstance pageSectionInstance = new PageSectionInstance();
			ReadReportItemInstanceBase(pageSectionInstance, pageSectionDef);
			if (PreRead(objectType, indexes))
			{
				pageSectionInstance.PageNumber = m_reader.ReadInt32();
			}
			if (PreRead(objectType, indexes))
			{
				pageSectionInstance.ReportItemColInstance = ReadReportItemColInstance(pageSectionDef.ReportItems);
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return pageSectionInstance;
		}

		private object ReadVariant()
		{
			return ReadVariant(readNextToken: true);
		}

		private object ReadVariant(bool readNextToken)
		{
			return ReadVariant(readNextToken, convertDBNull: false);
		}

		private object ReadVariant(bool readNextToken, bool convertDBNull)
		{
			if (readNextToken)
			{
				Assert(m_reader.Read());
			}
			switch (m_reader.Token)
			{
			case Token.Null:
				if (convertDBNull)
				{
					return DBNull.Value;
				}
				return null;
			case Token.String:
				return m_reader.StringValue;
			case Token.Char:
				return m_reader.CharValue;
			case Token.Boolean:
				return m_reader.BooleanValue;
			case Token.Int16:
				return m_reader.Int16Value;
			case Token.Int32:
				return m_reader.Int32Value;
			case Token.Int64:
				return m_reader.Int64Value;
			case Token.UInt16:
				return m_reader.UInt16Value;
			case Token.UInt32:
				return m_reader.UInt32Value;
			case Token.UInt64:
				return m_reader.UInt64Value;
			case Token.Byte:
				return m_reader.ByteValue;
			case Token.SByte:
				return m_reader.SByteValue;
			case Token.Single:
				return m_reader.SingleValue;
			case Token.Double:
				return m_reader.DoubleValue;
			case Token.Decimal:
				return m_reader.DecimalValue;
			case Token.DateTime:
				return m_reader.DateTimeValue;
			default:
				Assert(Token.TimeSpan == m_reader.Token);
				return m_reader.TimeSpanValue;
			}
		}

		private RecordField[] ReadRecordFields()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			Assert(Token.Array == m_reader.Token);
			int arrayLength = m_reader.ArrayLength;
			RecordField[] array = new RecordField[arrayLength];
			for (int i = 0; i < arrayLength; i++)
			{
				array[i] = ReadRecordField();
			}
			return array;
		}

		private RecordField ReadRecordField()
		{
			Assert(m_reader.Read());
			if (m_reader.Token == Token.Null)
			{
				return null;
			}
			ObjectType objectType = ObjectType.RecordField;
			Indexes indexes = new Indexes();
			Assert(Token.Object == m_reader.Token);
			Assert(objectType == m_reader.ObjectType);
			RecordField recordField = new RecordField();
			if (PreRead(objectType, indexes))
			{
				recordField.FieldValue = ReadFieldValue(out DataFieldStatus fieldStatus);
				recordField.FieldStatus = fieldStatus;
			}
			if (PreRead(objectType, indexes))
			{
				recordField.IsAggregationField = m_reader.ReadBoolean();
			}
			if (PreRead(objectType, indexes))
			{
				recordField.FieldPropertyValues = ReadVariantList(convertDBNull: false);
			}
			PostRead(objectType, indexes);
			m_reader.ReadEndObject();
			return recordField;
		}

		private object ReadFieldValue(out DataFieldStatus fieldStatus)
		{
			Assert(m_reader.Read());
			fieldStatus = DataFieldStatus.None;
			switch (m_reader.Token)
			{
			case Token.Null:
				return DBNull.Value;
			case Token.String:
				return m_reader.StringValue;
			case Token.Char:
				return m_reader.CharValue;
			case Token.Boolean:
				return m_reader.BooleanValue;
			case Token.Int16:
				return m_reader.Int16Value;
			case Token.Int32:
				return m_reader.Int32Value;
			case Token.Int64:
				return m_reader.Int64Value;
			case Token.UInt16:
				return m_reader.UInt16Value;
			case Token.UInt32:
				return m_reader.UInt32Value;
			case Token.UInt64:
				return m_reader.UInt64Value;
			case Token.Byte:
				return m_reader.ByteValue;
			case Token.SByte:
				return m_reader.SByteValue;
			case Token.Single:
				return m_reader.SingleValue;
			case Token.Double:
				return m_reader.DoubleValue;
			case Token.Decimal:
				return m_reader.DecimalValue;
			case Token.DateTime:
				return m_reader.DateTimeValue;
			case Token.TimeSpan:
				return m_reader.TimeSpanValue;
			case Token.Guid:
				return m_reader.GuidValue;
			case Token.DataFieldInfo:
				fieldStatus = m_reader.DataFieldInfo;
				return null;
			default:
				Assert(Token.TypedArray == m_reader.Token);
				if (Token.Byte == m_reader.ArrayType)
				{
					return m_reader.BytesValue;
				}
				if (Token.Int32 == m_reader.ArrayType)
				{
					return m_reader.Int32sValue;
				}
				Assert(Token.Char == m_reader.ArrayType);
				return m_reader.CharsValue;
			}
		}
	}
}
