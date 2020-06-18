using System;
using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal class ScriptProperties
	{
		private long m_value;

		private static ScriptProperties[] ScriptsProps;

		internal bool IsComplex => ((m_value >> 17) & 1) > 0;

		internal byte CharSet => (byte)((m_value >> 20) & 0xFF);

		internal bool IsAmbiguousCharSet => ((m_value >> 34) & 1) > 0;

		internal static int Length => ScriptsProps.Length;

		internal ScriptProperties(long value)
		{
			m_value = value;
		}

		static ScriptProperties()
		{
			IntPtr ppScriptProperties;
			int pNumScripts;
			int num = Win32.ScriptGetProperties(out ppScriptProperties, out pNumScripts);
			if (Win32.Failed(num))
			{
				Marshal.ThrowExceptionForHR(num);
			}
			ScriptsProps = new ScriptProperties[pNumScripts];
			for (int i = 0; i < pNumScripts; i++)
			{
				long value = Marshal.ReadInt64(Marshal.ReadIntPtr(ppScriptProperties, i * IntPtr.Size));
				ScriptsProps[i] = new ScriptProperties(value);
			}
		}

		internal static ScriptProperties GetProperties(int script)
		{
			return ScriptsProps[script];
		}
	}
}
