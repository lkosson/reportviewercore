using Microsoft.ReportingServices.OnDemandReportRendering;
using System.Collections;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.Utilities
{
	internal sealed class DiagnosticsUtilities
	{
		public const string MSG_PAGE_CREATED = "PR-DIAG [Page {0}] Page created by {1} page break";

		public const string APPEND_MSG_PAGE_NUMBER_RESET = ". Page number reset";

		public const string MSG_PAGE_NAME_CHANGED = "PR-DIAG [Page {0}] Page name changed";

		public const string MSG_PAGE_BREAK_IGNORED = "PR-DIAG [Page {0}] Page break on '{1}' ignored";

		public const string APPEND_MSG_INSIDE_TABLIX_CELL = " - inside TablixCell";

		public const string APPEND_MSG_PART_OF_TOGGLEABLE_REGION = " - part of toggleable region";

		public const string APPEND_MSG_INSIDE_HEADER_FOOTER = " - inside header or footer";

		public const string MSG_PAGE_BREAK_IGNORED_BY_PEER_ITEM = "PR-DIAG [Page {0}] Page break on '{1}' ignored – peer item precedence";

		public const string MSG_PAGE_BREAK_IGNORED_DISABLED_TRUE = "PR-DIAG [Page {0}] Page break on '{1}' ignored – Disable is True";

		public const string MSG_PAGE_BREAK_IGNORED_AT_PAGE_TOP = "PR-DIAG [Page {0}] Page break on '{1}' ignored – at top of page";

		public const string MSG_PAGE_BREAK_IGNORED_AT_PAGE_BOTTOM = "PR-DIAG [Page {0}] Page break on '{1}' ignored – bottom of page";

		public const string MSG_KEEP_TOGETHER_MEMBER_ROWCONTEXT_PAGE_GROWN = "PR-DIAG [Page {0}] Member '{1}' kept together - RowContext - Page grown";

		public const string MSG_KEEP_TOGETHER_MEMBER_IMPLICIT_PAGE_GROWN = "PR-DIAG [Page {0}] Member '{1}' kept together - Implicit - Page grown";

		public const string MSG_KEEP_TOGETHER_ITEM_EXPLICIT_PAGE_GROWN = "PR-DIAG [Page {0}] Item '{1}' kept together - Explicit - Page grown";

		public const string MSG_KEEP_TOGETHER_MEMBER_KEEP_WITH_PAGE_GROWN = "PR-DIAG [Page {0}] Member '{1}' kept together - KeepWith - Page grown";

		public const string MSG_KEEP_TOGETHER_MEMBER = "PR-DIAG [Page {0}] Member '{1}' kept together - Explicit - Pushed to next page";

		public const string MSG_KEEP_TOGETHER_ROW = "PR-DIAG [Page {0}] Item(s) '{1}' kept together - RowContext - Pushed to next page";

		public const string MSG_KEEP_TOGETHER_COLUMN = "PR-DIAG [Page {0}] Item kept together - ColumnContext - Pushed to next page";

		public const string MSG_INVALIDATE_KEEP_TOGETHER_MEMBER = "PR-DIAG [Page {0}] KeepTogether on Member '{1}' not honored - larger than page";

		public const string MSG_KEEP_TOGETHER_REPORT_ITEM_HORIZONTAL = "PR-DIAG [Page {0}] Item '{1}' kept together horizontally - Explicit - Pushed to next page";

		public const string MSG_KEEP_TOGETHER_REPORT_ITEM_VERTICAL = "PR-DIAG [Page {0}] Item '{1}' kept together vertically - Explicit - Pushed to next page";

		public const string MSG_INVALIDATE_KEEP_TOGETHER_REPORT_ITEM_HORIZONTAL = "PR-DIAG [Page {0}] Horizontal KeepTogether on Item '{1}' not honored - larger than page";

		public const string MSG_INVALIDATE_KEEP_TOGETHER_REPORT_ITEM_VERTICAL = "PR-DIAG [Page {0}] Vertical KeepTogether on Item '{1}' not honored - larger than page";

		public const string MSG_REPEAT_ON_NEW_PAGE = "PR-DIAG [Page {0}] '{1}' appears on page due to RepeatOnNewPage";

		public const string MSG_INVALIDATE_REPEAT_ON_NEW_PAGE_SIZE = "PR-DIAG [Page {0}] '{1}' not repeated due to page size contraints";

		public const string MSG_INVALIDATE_REPEAT_ON_NEW_PAGE_SPLIT_MEMBER = "PR-DIAG [Page {0}] '{1}' not repeated because child member spans pages";

		public static string BuildTablixMemberPath(string tablixName, TablixMember tablixMember, Hashtable memberAtLevelIndexes)
		{
			bool isColumn = tablixMember.IsColumn;
			StringBuilder stringBuilder = new StringBuilder();
			do
			{
				int value = -1;
				object obj = memberAtLevelIndexes[tablixMember.DefinitionPath];
				if (obj != null)
				{
					value = (int)obj;
				}
				stringBuilder.Insert(0, ")");
				if (tablixMember.Group != null)
				{
					stringBuilder.Insert(0, tablixMember.Group.Name);
				}
				else
				{
					stringBuilder.Insert(0, "Static");
				}
				stringBuilder.Insert(0, "(");
				stringBuilder.Insert(0, value);
				stringBuilder.Insert(0, ".");
				tablixMember = tablixMember.Parent;
			}
			while (tablixMember != null);
			if (isColumn)
			{
				stringBuilder.Insert(0, ".CH");
			}
			else
			{
				stringBuilder.Insert(0, ".RH");
			}
			stringBuilder.Insert(0, tablixName);
			return stringBuilder.ToString();
		}
	}
}
