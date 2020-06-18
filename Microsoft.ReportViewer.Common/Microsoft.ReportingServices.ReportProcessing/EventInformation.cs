using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class EventInformation
	{
		[Serializable]
		internal class SortEventInfo
		{
			[Serializable]
			private struct SortInfoStruct
			{
				internal int ReportItemUniqueName;

				internal bool SortDirection;

				internal Hashtable PeerSorts;

				internal SortInfoStruct(int uniqueName, bool direction, Hashtable peerSorts)
				{
					ReportItemUniqueName = uniqueName;
					SortDirection = direction;
					PeerSorts = peerSorts;
				}
			}

			private ArrayList m_collection;

			private Hashtable m_nameMap;

			internal int Count => m_collection.Count;

			internal SortEventInfo()
			{
				m_collection = new ArrayList();
				m_nameMap = new Hashtable();
			}

			private SortEventInfo(SortEventInfo copy)
			{
				m_collection = (ArrayList)copy.m_collection.Clone();
				m_nameMap = (Hashtable)copy.m_nameMap.Clone();
			}

			internal void Add(int uniqueName, bool direction, Hashtable peerSorts)
			{
				Remove(uniqueName);
				m_nameMap.Add(uniqueName, m_collection.Count);
				m_collection.Add(new SortInfoStruct(uniqueName, direction, peerSorts));
			}

			internal bool Remove(int uniqueName)
			{
				object obj = m_nameMap[uniqueName];
				if (obj != null)
				{
					m_nameMap.Remove(uniqueName);
					m_collection.RemoveAt((int)obj);
					for (int i = (int)obj; i < m_collection.Count; i++)
					{
						SortInfoStruct sortInfoStruct = (SortInfoStruct)m_collection[i];
						m_nameMap[sortInfoStruct.ReportItemUniqueName] = i;
					}
					return true;
				}
				return false;
			}

			internal bool ClearPeerSorts(int uniqueName)
			{
				bool result = false;
				IntList intList = null;
				for (int i = 0; i < m_collection.Count; i++)
				{
					SortInfoStruct sortInfoStruct = (SortInfoStruct)m_collection[i];
					Hashtable peerSorts = sortInfoStruct.PeerSorts;
					if (peerSorts != null)
					{
						if (intList == null)
						{
							intList = new IntList();
						}
						if (peerSorts.Contains(uniqueName))
						{
							intList.Add(sortInfoStruct.ReportItemUniqueName);
						}
					}
				}
				if (intList != null)
				{
					if (0 < intList.Count)
					{
						for (int j = 0; j < intList.Count; j++)
						{
							Remove(intList[j]);
						}
						result = true;
					}
				}
				else if (m_collection.Count > 0)
				{
					m_nameMap.Clear();
					m_collection.Clear();
					result = true;
				}
				return result;
			}

			internal int GetUniqueNameAt(int index)
			{
				Global.Tracer.Assert(0 <= index && index < m_collection.Count, "(0 <= index && index < m_collection.Count)");
				return ((SortInfoStruct)m_collection[index]).ReportItemUniqueName;
			}

			internal bool GetSortDirectionAt(int index)
			{
				Global.Tracer.Assert(0 <= index && index < m_collection.Count, "(0 <= index && index < m_collection.Count)");
				return ((SortInfoStruct)m_collection[index]).SortDirection;
			}

			internal SortOptions GetSortState(int uniqueName)
			{
				if (m_nameMap != null)
				{
					Global.Tracer.Assert(m_collection != null, "(null != m_collection)");
					object obj = m_nameMap[uniqueName];
					if (obj != null)
					{
						if (!((SortInfoStruct)m_collection[(int)obj]).SortDirection)
						{
							return SortOptions.Descending;
						}
						return SortOptions.Ascending;
					}
				}
				return SortOptions.None;
			}

			internal SortEventInfo Clone()
			{
				return new SortEventInfo(this);
			}
		}

		[Serializable]
		internal class OdpSortEventInfo
		{
			[Serializable]
			private struct SortInfoStruct
			{
				internal string EventSourceUniqueName;

				internal bool SortDirection;

				internal Hashtable PeerSorts;

				internal SortInfoStruct(string uniqueName, bool direction, Hashtable peerSorts)
				{
					EventSourceUniqueName = uniqueName;
					SortDirection = direction;
					PeerSorts = peerSorts;
				}
			}

			private ArrayList m_collection;

			private Dictionary<string, int> m_uniqueNameMap;

			internal int Count => m_collection.Count;

			internal OdpSortEventInfo()
			{
				m_collection = new ArrayList();
				m_uniqueNameMap = new Dictionary<string, int>();
			}

			private OdpSortEventInfo(OdpSortEventInfo copy)
			{
				m_collection = (ArrayList)copy.m_collection.Clone();
				if (copy.m_uniqueNameMap == null)
				{
					return;
				}
				m_uniqueNameMap = new Dictionary<string, int>(copy.m_uniqueNameMap.Count);
				IDictionaryEnumerator dictionaryEnumerator = copy.m_uniqueNameMap.GetEnumerator();
				while (dictionaryEnumerator.MoveNext())
				{
					if (dictionaryEnumerator.Key != null)
					{
						m_uniqueNameMap.Add(dictionaryEnumerator.Key as string, (int)dictionaryEnumerator.Value);
					}
				}
			}

			internal void Add(string uniqueNameString, bool direction, Hashtable peerSorts)
			{
				Remove(uniqueNameString);
				m_uniqueNameMap.Add(uniqueNameString, m_collection.Count);
				m_collection.Add(new SortInfoStruct(uniqueNameString, direction, peerSorts));
			}

			internal bool Remove(int id, List<InstancePathItem> instancePath)
			{
				return Remove(InstancePathItem.GenerateUniqueNameString(id, instancePath));
			}

			internal bool Remove(string uniqueNameString)
			{
				if (!m_uniqueNameMap.TryGetValue(uniqueNameString, out int value))
				{
					return false;
				}
				m_uniqueNameMap.Remove(uniqueNameString);
				m_collection.RemoveAt(value);
				for (int i = value; i < m_collection.Count; i++)
				{
					SortInfoStruct sortInfoStruct = (SortInfoStruct)m_collection[i];
					m_uniqueNameMap[sortInfoStruct.EventSourceUniqueName] = i;
				}
				return true;
			}

			internal bool ClearPeerSorts(string uniqueNameString)
			{
				bool result = false;
				List<string> list = null;
				int num = 0;
				for (int i = 0; i < m_collection.Count; i++)
				{
					SortInfoStruct sortInfoStruct = (SortInfoStruct)m_collection[i];
					Hashtable peerSorts = sortInfoStruct.PeerSorts;
					if (peerSorts != null && peerSorts.Count != 0)
					{
						if (list == null)
						{
							list = new List<string>();
						}
						if (peerSorts.Contains(uniqueNameString))
						{
							list.Add(sortInfoStruct.EventSourceUniqueName);
							num++;
						}
					}
				}
				if (num != 0)
				{
					for (int j = 0; j < num; j++)
					{
						Remove(list[j]);
					}
					result = true;
				}
				return result;
			}

			internal string GetUniqueNameAt(int index)
			{
				Global.Tracer.Assert(0 <= index && index < m_collection.Count, "(0 <= index && index < m_collection.Count)");
				return ((SortInfoStruct)m_collection[index]).EventSourceUniqueName;
			}

			internal bool GetSortDirectionAt(int index)
			{
				Global.Tracer.Assert(0 <= index && index < m_collection.Count, "(0 <= index && index < m_collection.Count)");
				return ((SortInfoStruct)m_collection[index]).SortDirection;
			}

			internal SortOptions GetSortState(string eventSourceUniqueName)
			{
				if (m_uniqueNameMap != null)
				{
					Global.Tracer.Assert(m_collection != null, "(null != m_collection)");
					if (m_uniqueNameMap.TryGetValue(eventSourceUniqueName, out int value))
					{
						if (!((SortInfoStruct)m_collection[value]).SortDirection)
						{
							return SortOptions.Descending;
						}
						return SortOptions.Ascending;
					}
				}
				return SortOptions.None;
			}

			internal OdpSortEventInfo Clone()
			{
				return new OdpSortEventInfo(this);
			}
		}

		[Serializable]
		internal class RendererEventInformation
		{
			private Hashtable m_validToggleSenders;

			private Hashtable m_drillthroughInfo;

			internal Hashtable ValidToggleSenders
			{
				get
				{
					return m_validToggleSenders;
				}
				set
				{
					m_validToggleSenders = value;
				}
			}

			internal Hashtable DrillthroughInfo
			{
				get
				{
					return m_drillthroughInfo;
				}
				set
				{
					m_drillthroughInfo = value;
				}
			}

			internal RendererEventInformation()
			{
			}

			internal RendererEventInformation(RendererEventInformation copy)
			{
				Global.Tracer.Assert(copy != null, "(null != copy)");
				if (copy.m_validToggleSenders != null)
				{
					m_validToggleSenders = (Hashtable)copy.m_validToggleSenders.Clone();
				}
				if (copy.m_drillthroughInfo != null)
				{
					m_drillthroughInfo = (Hashtable)copy.m_drillthroughInfo.Clone();
				}
			}

			internal void Reset()
			{
				m_validToggleSenders = null;
				m_drillthroughInfo = null;
			}

			internal bool ValidToggleSender(string senderId)
			{
				if (m_validToggleSenders != null)
				{
					return m_validToggleSenders.ContainsKey(senderId);
				}
				return false;
			}

			internal DrillthroughInfo GetDrillthroughInfo(string drillthroughId)
			{
				if (m_drillthroughInfo != null)
				{
					return (DrillthroughInfo)m_drillthroughInfo[drillthroughId];
				}
				return null;
			}
		}

		private bool m_hasShowHideInfo;

		private Hashtable m_toggleStateInfo;

		private Hashtable m_hiddenInfo;

		private bool m_hasSortInfo;

		private SortEventInfo m_sortInfo;

		private OdpSortEventInfo m_odpSortInfo;

		private Dictionary<string, RendererEventInformation> m_rendererEventInformation;

		[NonSerialized]
		private bool m_changed;

		internal Hashtable ToggleStateInfo
		{
			get
			{
				return m_toggleStateInfo;
			}
			set
			{
				m_hasShowHideInfo = (value != null);
				m_toggleStateInfo = value;
			}
		}

		internal Hashtable HiddenInfo
		{
			get
			{
				return m_hiddenInfo;
			}
			set
			{
				m_hiddenInfo = value;
			}
		}

		internal SortEventInfo SortInfo
		{
			get
			{
				return m_sortInfo;
			}
			set
			{
				m_hasSortInfo = (value != null);
				m_sortInfo = value;
			}
		}

		internal OdpSortEventInfo OdpSortInfo
		{
			get
			{
				return m_odpSortInfo;
			}
			set
			{
				m_hasSortInfo = (value != null);
				m_odpSortInfo = value;
			}
		}

		internal bool Changed
		{
			get
			{
				return m_changed;
			}
			set
			{
				m_changed = value;
			}
		}

		internal EventInformation()
		{
		}

		public EventInformation(EventInformation copy)
		{
			Global.Tracer.Assert(copy != null, "(null != copy)");
			m_hasShowHideInfo = copy.m_hasShowHideInfo;
			if (copy.m_toggleStateInfo != null)
			{
				m_toggleStateInfo = (Hashtable)copy.m_toggleStateInfo.Clone();
			}
			if (copy.m_hiddenInfo != null)
			{
				m_hiddenInfo = (Hashtable)copy.m_hiddenInfo.Clone();
			}
			m_hasSortInfo = copy.m_hasSortInfo;
			if (copy.m_sortInfo != null)
			{
				m_sortInfo = copy.m_sortInfo.Clone();
			}
			else if (copy.m_odpSortInfo != null)
			{
				m_odpSortInfo = copy.m_odpSortInfo.Clone();
			}
			if (copy.m_rendererEventInformation == null)
			{
				return;
			}
			m_rendererEventInformation = new Dictionary<string, RendererEventInformation>(copy.m_rendererEventInformation.Count);
			foreach (string key in copy.m_rendererEventInformation.Keys)
			{
				RendererEventInformation value = new RendererEventInformation(copy.m_rendererEventInformation[key]);
				m_rendererEventInformation[key] = value;
			}
		}

		public byte[] Serialize()
		{
			Global.Tracer.Assert(m_hasShowHideInfo || m_hasSortInfo || m_rendererEventInformation != null, "(m_hasShowHideInfo || m_hasSortInfo || m_rendererEventInformation != null)");
			MemoryStream memoryStream = null;
			try
			{
				if (m_hasShowHideInfo)
				{
					Global.Tracer.Assert(m_toggleStateInfo != null, "(null != m_toggleStateInfo)");
				}
				if (m_hasSortInfo)
				{
					Global.Tracer.Assert(m_sortInfo != null || m_odpSortInfo != null, "(null != m_sortInfo || null != m_odpSortInfo)");
				}
				memoryStream = new MemoryStream();
				new BinaryFormatter().Serialize(memoryStream, this);
				return memoryStream.ToArray();
			}
			finally
			{
				memoryStream?.Close();
			}
		}

		public static EventInformation Deserialize(byte[] data)
		{
			if (data == null)
			{
				return null;
			}
			MemoryStream stream = null;
			EventInformation result = null;
			try
			{
				stream = new MemoryStream(data, writable: false);
				BinaryFormatter bFormatter = new BinaryFormatter();
				RevertImpersonationContext.Run(delegate
				{
					EventInformation eventInformation = (EventInformation)bFormatter.Deserialize(stream);
					eventInformation.m_changed = false;
					result = eventInformation;
				});
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
			return result;
		}

		internal RendererEventInformation GetRendererEventInformation(string aRenderFormat)
		{
			if (m_rendererEventInformation == null)
			{
				m_rendererEventInformation = new Dictionary<string, RendererEventInformation>();
			}
			RendererEventInformation value = null;
			if (!m_rendererEventInformation.TryGetValue(aRenderFormat, out value))
			{
				value = new RendererEventInformation();
				m_rendererEventInformation[aRenderFormat] = value;
			}
			return value;
		}

		internal bool ValidToggleSender(string senderId)
		{
			if (m_rendererEventInformation == null)
			{
				return false;
			}
			foreach (string key in m_rendererEventInformation.Keys)
			{
				if (m_rendererEventInformation[key].ValidToggleSender(senderId))
				{
					return true;
				}
			}
			return false;
		}

		internal DrillthroughInfo GetDrillthroughInfo(string drillthroughId)
		{
			if (m_rendererEventInformation != null)
			{
				foreach (string key in m_rendererEventInformation.Keys)
				{
					DrillthroughInfo drillthroughInfo = m_rendererEventInformation[key].GetDrillthroughInfo(drillthroughId);
					if (drillthroughInfo != null)
					{
						return drillthroughInfo;
					}
				}
			}
			return null;
		}
	}
}
