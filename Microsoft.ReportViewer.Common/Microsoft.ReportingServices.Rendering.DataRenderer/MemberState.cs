using System;
using System.Collections.Generic;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal class MemberState
{
	protected const string IDSeparator = "x";

	protected MemberState m_parent;

	protected List<MemberState> m_children;

	protected int m_activeChild;

	protected TablixMember m_tablixMember;

	protected ChartMember m_chartMember;

	protected MapMember m_mapMember;

	protected int m_id = -1;

	private Dictionary<string, int> m_memberIDMapper = new Dictionary<string, int>();

	private int m_nextID;

	private string m_activeDynamicMemberPath = string.Empty;

	private bool m_hasMember;

	protected bool m_hasRows = true;

	protected bool m_isDynamic;

	protected string m_memberID = string.Empty;

	protected string m_currentDynamicMemberPath = string.Empty;

	protected SubReport m_sr;

	protected MemberState ActiveChild => m_children[m_activeChild];

	internal SubReport SubReportMember
	{
		get
		{
			return m_sr;
		}
		set
		{
			m_sr = value;
		}
	}

	internal int NextID
	{
		get
		{
			return m_nextID;
		}
		set
		{
			m_nextID = value;
		}
	}

	internal bool HasRows => m_hasRows;

	internal virtual bool IsDynamic => m_isDynamic;

	internal bool HasMember => m_hasMember;

	internal string MemberID => m_memberID;

	internal string CurrentDynamicMemberPath
	{
		get
		{
			return m_currentDynamicMemberPath;
		}
		set
		{
			m_currentDynamicMemberPath = value;
		}
	}

	internal virtual string ActiveDynamicMemberPath => m_activeDynamicMemberPath;

	internal MemberState(DataRegionMember member, int id)
	{
		m_tablixMember = member as TablixMember;
		m_chartMember = member as ChartMember;
		m_mapMember = member as MapMember;
		if (member != null)
		{
			m_memberID = member.ID;
			m_activeDynamicMemberPath = AddMemberToDynamicPath(m_activeDynamicMemberPath, m_memberID);
			m_isDynamic = true;
			m_hasMember = true;
		}
		m_id = id;
	}

	internal static string AddMemberToDynamicPath(string path, string memberID)
	{
		if (!string.IsNullOrEmpty(memberID))
		{
			if (string.IsNullOrEmpty(path))
			{
				path += "x";
			}
			path += memberID;
			path += "x";
		}
		return path;
	}

	internal static int IndexOfMemberInDynamicPath(string path, string memberID)
	{
		return path.IndexOf("x" + memberID + "x", StringComparison.OrdinalIgnoreCase);
	}

	internal static bool RemoveMemberFromDynamicPath(ref string path, string memberID)
	{
		int num = IndexOfMemberInDynamicPath(path, memberID);
		if (num > -1)
		{
			path = path.Remove(num, memberID.Length + 1);
			return true;
		}
		return false;
	}

	internal void AddChild(MemberState MemberState)
	{
		if (m_children == null)
		{
			m_children = new List<MemberState>();
		}
		m_children.Add(MemberState);
		MemberState.m_parent = this;
		MemberState.CurrentDynamicMemberPath = CurrentDynamicMemberPath;
		MemberState.CurrentDynamicMemberPath = AddMemberToDynamicPath(MemberState.CurrentDynamicMemberPath, MemberID);
	}

	internal bool IsActiveChild()
	{
		if (m_parent != null && m_parent.m_children != null && m_parent.m_activeChild < m_parent.m_children.Count)
		{
			if (m_id == m_parent.ActiveChild.m_id)
			{
				return true;
			}
			if (!string.IsNullOrEmpty(ActiveDynamicMemberPath) && m_parent.ActiveChild.ActiveDynamicMemberPath.Contains(ActiveDynamicMemberPath))
			{
				return true;
			}
		}
		return false;
	}

	internal MemberState FindChild(int childID)
	{
		if (m_children != null)
		{
			foreach (MemberState child in m_children)
			{
				if (childID == child.m_id)
				{
					return child;
				}
			}
		}
		return null;
	}

	private bool ChangeMemberInstances(bool reset)
	{
		bool flag = false;
		if (m_tablixMember != null)
		{
			if (m_tablixMember.Instance is TablixDynamicMemberInstance tablixDynamicMemberInstance)
			{
				if (reset)
				{
					tablixDynamicMemberInstance.ResetContext();
				}
				flag = tablixDynamicMemberInstance.MoveNext();
				if (reset)
				{
					m_hasRows = flag;
				}
			}
		}
		else if (m_chartMember != null)
		{
			if (m_chartMember.Instance is ChartDynamicMemberInstance chartDynamicMemberInstance)
			{
				if (reset)
				{
					chartDynamicMemberInstance.ResetContext();
				}
				flag = chartDynamicMemberInstance.MoveNext();
				if (reset)
				{
					m_hasRows = flag;
				}
			}
		}
		else if (m_mapMember != null && m_mapMember.Instance is MapDynamicMemberInstance mapDynamicMemberInstance)
		{
			if (reset)
			{
				mapDynamicMemberInstance.ResetContext();
			}
			flag = mapDynamicMemberInstance.MoveNext();
			if (reset)
			{
				m_hasRows = flag;
			}
		}
		return flag;
	}

	private void TouchSubReportMember()
	{
		if (m_sr != null)
		{
			SubReportInstance subReportInstance = (SubReportInstance)m_sr.Instance;
			if (subReportInstance != null && subReportInstance.ProcessedWithError)
			{
				throw new ReportRenderingException(subReportInstance.ErrorMessage);
			}
		}
	}

	internal virtual bool AdvanceDynamicMembers()
	{
		bool flag = false;
		TouchSubReportMember();
		flag = AdvanceChildren();
		if (!flag)
		{
			flag = ChangeMemberInstances(reset: false);
			if (flag)
			{
				if (m_children != null)
				{
					foreach (MemberState child in m_children)
					{
						child.ResetDynamicMembers();
					}
				}
				m_activeChild = 0;
				if (m_children != null)
				{
					foreach (MemberState child2 in m_children)
					{
						if (child2.IsDynamic)
						{
							SetActiveDynamicMemberPath();
							return flag;
						}
						m_activeChild++;
					}
					return flag;
				}
			}
		}
		return flag;
	}

	protected bool AdvanceChildren()
	{
		bool flag = false;
		if (m_children != null)
		{
			int count = m_children.Count;
			if (m_activeChild < count)
			{
				RSTrace.RenderingTracer.Assert(m_activeChild < count, "Active Child index out of range in AdvanceChildren");
				string activeDynamicMemberPath = ActiveChild.ActiveDynamicMemberPath;
				flag = ActiveChild.AdvanceDynamicMembers();
				while (!flag && m_activeChild + 1 < count)
				{
					m_activeChild++;
					if (activeDynamicMemberPath != ActiveChild.ActiveDynamicMemberPath)
					{
						flag = ActiveChild.ResetDynamicMembers();
					}
				}
				if (m_activeChild < count && activeDynamicMemberPath != ActiveChild.ActiveDynamicMemberPath)
				{
					SetActiveDynamicMemberPath();
				}
			}
		}
		return flag;
	}

	internal bool ContainsDynamicMemberPath(string dynamicMemberPaths)
	{
		if (!string.IsNullOrEmpty(m_memberID) && !RemoveMemberFromDynamicPath(ref dynamicMemberPaths, m_memberID))
		{
			dynamicMemberPaths = dynamicMemberPaths.Replace("_0", "_1");
		}
		if (string.Compare("_1x", dynamicMemberPaths, StringComparison.Ordinal) == 0)
		{
			return true;
		}
		if (m_children != null && m_children.Count > 0)
		{
			foreach (MemberState child in m_children)
			{
				if (child.ContainsDynamicMemberPath(dynamicMemberPaths))
				{
					return true;
				}
			}
		}
		return false;
	}

	internal void SetActiveDynamicMemberPath()
	{
		if (m_children != null && m_children.Count > 0 && m_activeChild < m_children.Count)
		{
			m_activeDynamicMemberPath = string.Empty;
			m_activeDynamicMemberPath = AddMemberToDynamicPath(m_activeDynamicMemberPath, m_memberID);
			if (!string.IsNullOrEmpty(m_activeDynamicMemberPath) && !string.IsNullOrEmpty(ActiveChild.ActiveDynamicMemberPath))
			{
				m_activeDynamicMemberPath = m_activeDynamicMemberPath.Remove(m_activeDynamicMemberPath.Length - 1);
			}
			m_activeDynamicMemberPath += ActiveChild.ActiveDynamicMemberPath;
		}
	}

	internal virtual bool ResetAllMembers(bool atomHeaderInstanceWalk)
	{
		TouchSubReportMember();
		bool flag = false;
		bool flag2 = false;
		if (!atomHeaderInstanceWalk)
		{
			flag = ChangeMemberInstances(reset: true);
		}
		m_activeChild = 0;
		if (m_children != null && m_children.Count > 0)
		{
			foreach (MemberState child in m_children)
			{
				flag |= child.ResetAllMembers(atomHeaderInstanceWalk: false);
				bool isDynamic = child.IsDynamic;
				m_isDynamic |= isDynamic;
				if (isDynamic && !flag2)
				{
					flag2 = true;
					SetActiveDynamicMemberPath();
				}
				if (!flag2)
				{
					m_activeChild++;
				}
			}
			return flag;
		}
		return flag;
	}

	internal virtual bool ResetDynamicMembers()
	{
		TouchSubReportMember();
		bool flag = false;
		flag |= ChangeMemberInstances(reset: true);
		if (m_children != null && m_children.Count > 0)
		{
			m_activeChild = 0;
			bool flag2 = false;
			foreach (MemberState child in m_children)
			{
				if (child.IsDynamic)
				{
					flag2 = true;
					break;
				}
				m_activeChild++;
			}
			if (flag2)
			{
				flag |= ActiveChild.ResetDynamicMembers();
				SetActiveDynamicMemberPath();
			}
		}
		return flag;
	}

	internal bool GetDynamicItemID(string stringID, out int id)
	{
		return m_memberIDMapper.TryGetValue(stringID, out id);
	}

	internal void AddToMemoryMapper(string stringId, int id)
	{
		m_memberIDMapper.Add(stringId, id);
	}
}
