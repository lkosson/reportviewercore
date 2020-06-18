using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class CustomReportItem : DataRegion, ICreateSubtotals, IPersistable
	{
		private DataMemberList m_dataColumnMembers;

		private DataMemberList m_dataRowMembers;

		private CustomDataRowList m_dataRows;

		private bool m_isDataRegion;

		private string m_type;

		private ReportItem m_altReportItem;

		private int m_altReportItemIndexInParentCollectionDef = -1;

		private ReportItemCollection m_renderReportItem;

		private bool m_explicitAltReportItemDefined;

		[NonSerialized]
		private bool m_createdSubtotals;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private CustomReportItemExprHost m_criExprHost;

		internal override bool IsDataRegion => m_isDataRegion;

		internal override Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.CustomReportItem;

		internal override HierarchyNodeList ColumnMembers => m_dataColumnMembers;

		internal override HierarchyNodeList RowMembers => m_dataRowMembers;

		internal override RowList Rows => m_dataRows;

		internal CustomReportItemExprHost CustomReportItemExprHost => m_criExprHost;

		protected override IndexedExprHost UserSortExpressionsHost
		{
			get
			{
				if (m_criExprHost == null)
				{
					return null;
				}
				return m_criExprHost.UserSortExpressionsHost;
			}
		}

		internal DataMemberList DataColumnMembers
		{
			get
			{
				return m_dataColumnMembers;
			}
			set
			{
				m_dataColumnMembers = value;
			}
		}

		internal DataMemberList DataRowMembers
		{
			get
			{
				return m_dataRowMembers;
			}
			set
			{
				m_dataRowMembers = value;
			}
		}

		internal CustomDataRowList DataRows
		{
			get
			{
				return m_dataRows;
			}
			set
			{
				m_dataRows = value;
			}
		}

		internal string Type
		{
			get
			{
				return m_type;
			}
			set
			{
				m_type = value;
			}
		}

		internal ReportItem AltReportItem
		{
			get
			{
				return m_altReportItem;
			}
			set
			{
				m_altReportItem = value;
			}
		}

		internal int AltReportItemIndexInParentCollectionDef
		{
			get
			{
				return m_altReportItemIndexInParentCollectionDef;
			}
			set
			{
				m_altReportItemIndexInParentCollectionDef = value;
			}
		}

		internal ReportItemCollection RenderReportItem
		{
			get
			{
				return m_renderReportItem;
			}
			set
			{
				m_renderReportItem = value;
			}
		}

		internal bool ExplicitlyDefinedAltReportItem
		{
			get
			{
				return m_explicitAltReportItemDefined;
			}
			set
			{
				m_explicitAltReportItemDefined = value;
			}
		}

		internal CustomReportItem(ReportItem parent)
			: base(parent)
		{
		}

		internal CustomReportItem(int id, ReportItem parent)
			: base(id, parent)
		{
		}

		internal void SetAsDataRegion()
		{
			m_isDataRegion = true;
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			if (IsDataRegion)
			{
				if (!context.RegisterDataRegion(this))
				{
					return false;
				}
				context.Location |= (Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataSet | Microsoft.ReportingServices.ReportPublishing.LocationFlags.InDataRegion);
			}
			context.ExprHostBuilder.DataRegionStart(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.CustomReportItem, m_name);
			base.Initialize(context);
			base.ExprHostID = context.ExprHostBuilder.DataRegionEnd(Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.CustomReportItem);
			if (IsDataRegion)
			{
				context.UnRegisterDataRegion(this);
			}
			return false;
		}

		protected override bool ValidateInnerStructure(InitializationContext context)
		{
			if (!IsDataRegion)
			{
				return false;
			}
			if (m_dataRows == null || m_dataRows.Count == 0)
			{
				if (m_rowCount != 0 || m_columnCount != 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfDataRows, Severity.Error, context.ObjectType, context.ObjectName, m_rowCount.ToString(CultureInfo.InvariantCulture.NumberFormat));
					return false;
				}
				return false;
			}
			return true;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			CustomReportItem customReportItem = (CustomReportItem)(context.CurrentDataRegionClone = (CustomReportItem)base.PublishClone(context));
			if (m_dataColumnMembers != null)
			{
				customReportItem.m_dataColumnMembers = new DataMemberList(m_dataColumnMembers.Count);
				foreach (DataMember dataColumnMember in m_dataColumnMembers)
				{
					customReportItem.m_dataColumnMembers.Add(dataColumnMember.PublishClone(context, customReportItem));
				}
			}
			if (m_dataRowMembers != null)
			{
				customReportItem.m_dataRowMembers = new DataMemberList(m_dataRowMembers.Count);
				foreach (DataMember dataRowMember in m_dataRowMembers)
				{
					customReportItem.m_dataRowMembers.Add(dataRowMember.PublishClone(context, customReportItem));
				}
			}
			if (m_dataRows != null)
			{
				customReportItem.m_dataRows = new CustomDataRowList(m_dataRows.Count);
				foreach (CustomDataRow dataRow in m_dataRows)
				{
					customReportItem.m_dataRows.Add((CustomDataRow)dataRow.PublishClone(context));
				}
			}
			context.CreateSubtotalsDefinitions.Add(customReportItem);
			return customReportItem;
		}

		public void CreateAutomaticSubtotals(AutomaticSubtotalContext context)
		{
			if (m_createdSubtotals || !IsDataRegion || m_dataRows == null || m_rowCount != m_dataRows.Count)
			{
				return;
			}
			for (int i = 0; i < m_dataRows.Count; i++)
			{
				if (m_dataRows[i].Cells == null || m_dataRows[i].Cells.Count != m_columnCount)
				{
					return;
				}
			}
			context.Location = Microsoft.ReportingServices.ReportPublishing.LocationFlags.None;
			context.ObjectType = ObjectType;
			context.ObjectName = "CustomReportItem";
			context.CurrentDataRegion = this;
			context.CurrentScope = base.DataSetName;
			context.CurrentDataScope = this;
			context.CellLists = new List<CellList>(m_dataRows.Count);
			for (int j = 0; j < m_dataRows.Count; j++)
			{
				context.CellLists.Add(new CellList());
			}
			context.Rows = new RowList(m_dataRows.Count);
			context.StartIndex = 0;
			CreateAutomaticSubtotals(context, m_dataColumnMembers, isColumn: true);
			context.StartIndex = 0;
			CreateAutomaticSubtotals(context, m_dataRowMembers, isColumn: false);
			context.CurrentScope = null;
			context.CurrentDataScope = null;
			m_createdSubtotals = true;
		}

		private int CreateAutomaticSubtotals(AutomaticSubtotalContext context, DataMemberList members, bool isColumn)
		{
			int num = 0;
			for (int i = 0; i < members.Count; i++)
			{
				DataMember dataMember = members[i];
				if (dataMember.Subtotal)
				{
					context.CurrentIndex = context.StartIndex;
					if (isColumn)
					{
						foreach (CellList cellList in context.CellLists)
						{
							cellList.Clear();
						}
					}
					else
					{
						context.Rows.Clear();
					}
					BuildAndSetupAxisScopeTreeForAutoSubtotals(ref context, dataMember);
					DataMember dataMember2 = (DataMember)dataMember.PublishClone(context, null, isSubtotal: true);
					context.AdjustReferences();
					dataMember2.IsAutoSubtotal = true;
					dataMember2.Subtotal = false;
					members.Insert(i + 1, dataMember2);
					num = context.CurrentIndex - context.StartIndex;
					if (isColumn)
					{
						int num2 = 0;
						while (i < m_dataRows.Count)
						{
							m_dataRows[num2].Cells.InsertRange(context.CurrentIndex, context.CellLists[num2]);
							num2++;
						}
						m_columnCount += num;
					}
					else
					{
						m_dataRows.InsertRange(context.CurrentIndex, context.Rows);
						m_rowCount += num;
					}
					if (dataMember.SubMembers != null)
					{
						context.CurrentScope = dataMember.Grouping.Name;
						context.CurrentDataScope = dataMember;
						int num3 = CreateAutomaticSubtotals(context, dataMember.SubMembers, isColumn);
						if (isColumn)
						{
							dataMember.ColSpan += num3;
						}
						else
						{
							dataMember.RowSpan += num3;
						}
						num += num3;
					}
					else
					{
						context.StartIndex++;
					}
				}
				else if (dataMember.SubMembers != null)
				{
					if (dataMember.Grouping != null)
					{
						context.CurrentScope = dataMember.Grouping.Name;
						context.CurrentDataScope = dataMember;
					}
					int num4 = CreateAutomaticSubtotals(context, dataMember.SubMembers, isColumn);
					if (isColumn)
					{
						dataMember.ColSpan += num4;
					}
					else
					{
						dataMember.RowSpan += num4;
					}
					num += num4;
				}
				else
				{
					context.StartIndex++;
				}
			}
			return num;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataColumnMembers, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataMember));
			list.Add(new MemberInfo(MemberName.DataRowMembers, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataMember));
			list.Add(new MemberInfo(MemberName.DataRows, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CustomDataRow));
			list.Add(new MemberInfo(MemberName.Type, Token.String));
			list.Add(new MemberInfo(MemberName.AltReportItem, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, Token.Reference));
			list.Add(new MemberInfo(MemberName.RenderReportItemColDef, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemCollection));
			list.Add(new MemberInfo(MemberName.AltReportItemIndexInParentCollectionDef, Token.Int32));
			list.Add(new MemberInfo(MemberName.ExplicitAltReportItem, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IsDataRegion, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CustomReportItem, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataColumnMembers:
					writer.Write(m_dataColumnMembers);
					break;
				case MemberName.DataRowMembers:
					writer.Write(m_dataRowMembers);
					break;
				case MemberName.DataRows:
					writer.Write(m_dataRows);
					break;
				case MemberName.Type:
					writer.Write(m_type);
					break;
				case MemberName.AltReportItem:
					writer.WriteReference(m_altReportItem);
					break;
				case MemberName.AltReportItemIndexInParentCollectionDef:
					writer.Write(m_altReportItemIndexInParentCollectionDef);
					break;
				case MemberName.RenderReportItemColDef:
					writer.Write(m_renderReportItem);
					break;
				case MemberName.ExplicitAltReportItem:
					writer.Write(m_explicitAltReportItemDefined);
					break;
				case MemberName.IsDataRegion:
					writer.Write(m_isDataRegion);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			m_isDataRegion = (m_dataSetName != null);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DataColumnMembers:
					m_dataColumnMembers = reader.ReadListOfRIFObjects<DataMemberList>();
					break;
				case MemberName.DataRowMembers:
					m_dataRowMembers = reader.ReadListOfRIFObjects<DataMemberList>();
					break;
				case MemberName.DataRows:
					m_dataRows = reader.ReadListOfRIFObjects<CustomDataRowList>();
					break;
				case MemberName.Type:
					m_type = reader.ReadString();
					break;
				case MemberName.AltReportItem:
					m_altReportItem = reader.ReadReference<ReportItem>(this);
					break;
				case MemberName.AltReportItemIndexInParentCollectionDef:
					m_altReportItemIndexInParentCollectionDef = reader.ReadInt32();
					break;
				case MemberName.RenderReportItemColDef:
					m_renderReportItem = (ReportItemCollection)reader.ReadRIFObject();
					break;
				case MemberName.ExplicitAltReportItem:
					m_explicitAltReportItemDefined = reader.ReadBoolean();
					break;
				case MemberName.IsDataRegion:
					m_isDataRegion = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.AltReportItem)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_altReportItem = (ReportItem)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CustomReportItem;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				m_criExprHost = reportExprHost.CustomReportItemHostsRemotable[base.ExprHostID];
				DataRegionSetExprHost(m_criExprHost, m_criExprHost.SortHost, m_criExprHost.FilterHostsRemotable, m_criExprHost.UserSortExpressionsHost, m_criExprHost.PageBreakExprHost, m_criExprHost.JoinConditionExprHostsRemotable, reportObjectModel);
			}
		}

		internal override void DataRegionContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
			if (m_dataRows != null && m_dataRows.Count > 0)
			{
				IList<DataCellExprHost> list = (m_criExprHost != null) ? m_criExprHost.CellHostsRemotable : null;
				if (list == null)
				{
					return;
				}
				for (int i = 0; i < m_dataRows.Count; i++)
				{
					CustomDataRow customDataRow = m_dataRows[i];
					Global.Tracer.Assert(customDataRow != null && customDataRow.Cells != null, "(null != row && null != row.Cells)");
					for (int j = 0; j < customDataRow.DataCells.Count; j++)
					{
						DataCell dataCell = customDataRow.DataCells[j];
						Global.Tracer.Assert(dataCell != null && dataCell.DataValues != null, "(null != cell && null != cell.DataValues)");
						if (dataCell.ExpressionHostID >= 0)
						{
							dataCell.DataValues.SetExprHost(list[dataCell.ExpressionHostID].DataValueHostsRemotable, reportObjectModel);
						}
					}
				}
			}
			else
			{
				Global.Tracer.Assert(m_criExprHost == null || m_criExprHost.CellHostsRemotable == null || m_criExprHost.CellHostsRemotable.Count == 0);
			}
		}

		internal override object EvaluateNoRowsMessageExpression()
		{
			return m_criExprHost.NoRowsExpr;
		}

		protected override ReportHierarchyNode CreateHierarchyNode(int id)
		{
			return new DataMember(id, this);
		}

		protected override Row CreateRow(int id, int columnCount)
		{
			return new CustomDataRow(id)
			{
				DataCells = new DataCellList(columnCount)
			};
		}

		protected override Cell CreateCell(int id, int rowIndex, int colIndex)
		{
			return new DataCell(id, this);
		}
	}
}
