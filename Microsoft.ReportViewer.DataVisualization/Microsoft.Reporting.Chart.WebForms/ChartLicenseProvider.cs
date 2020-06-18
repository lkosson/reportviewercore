using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class ChartLicenseProvider : LicenseProvider
	{
		protected virtual bool IsKeyValid(string key, Type type)
		{
			if (key != null)
			{
				string text = "";
				try
				{
					text = new ProductKeyEncoder().DecryptLicenseKey(key);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionLicenceProductKeyInvalid);
				}
				string[] array = text.Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = array[i].Trim();
				}
				if (array.Length >= 2 && array[0] == Chart.productID && array[1] == GetNic())
				{
					return true;
				}
				return false;
			}
			return false;
		}

		private string GetNic()
		{
			string text = Guid.NewGuid().ToString();
			return text.Substring(text.Length - 12, 12);
		}

		public bool CheckKey(string key, Type type)
		{
			return IsKeyValid(key, type);
		}

		public override License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions)
		{
			ChartLicense chartLicense = null;
			if (context != null)
			{
				if (context.UsageMode == LicenseUsageMode.Designtime)
				{
					chartLicense = new ChartLicense(this, "Dundas Chart. Design-Time License");
				}
				if (chartLicense == null)
				{
					string savedLicenseKey = context.GetSavedLicenseKey(type, null);
					if (savedLicenseKey != null && IsKeyValid(savedLicenseKey, type))
					{
						chartLicense = new ChartLicense(this, savedLicenseKey);
					}
				}
				if (chartLicense == null)
				{
					string text = null;
					if (context != null)
					{
						ITypeResolutionService typeResolutionService = (ITypeResolutionService)context.GetService(typeof(ITypeResolutionService));
						if (typeResolutionService != null)
						{
							text = typeResolutionService.GetPathOfAssembly(type.Assembly.GetName());
						}
					}
					if (text == null)
					{
						text = type.Module.FullyQualifiedName;
					}
					string directoryName = Path.GetDirectoryName(text);
					string path = directoryName + "\\" + type.FullName + ".lic";
					bool flag = File.Exists(path);
					if (!flag)
					{
						string path2 = directoryName + "\\__AssemblyInfo__.ini";
						if (File.Exists(path2))
						{
							string text2 = null;
							StreamReader streamReader = new StreamReader(new FileStream(path2, FileMode.Open, FileAccess.Read, FileShare.Read));
							do
							{
								text2 = streamReader.ReadLine();
								text2 = text2.ToUpper(CultureInfo.InvariantCulture);
								if (text2 != null && text2.StartsWith("URL=", StringComparison.Ordinal))
								{
									directoryName = text2.Substring(4);
									if (directoryName.StartsWith("FILE:///", StringComparison.Ordinal))
									{
										directoryName = directoryName.Substring(8);
									}
									directoryName = directoryName.Replace('/', '\\');
									int num = directoryName.LastIndexOf('\\');
									if (num >= 0)
									{
										directoryName = directoryName.Substring(0, num);
									}
									path = directoryName + "\\" + type.FullName + ".lic";
									flag = File.Exists(path);
									text2 = null;
								}
							}
							while (text2 != null);
							streamReader.Close();
						}
					}
					if (flag)
					{
						StreamReader streamReader2 = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
						string key = streamReader2.ReadLine();
						streamReader2.Close();
						if (IsKeyValid(key, type))
						{
							chartLicense = new ChartLicense(this, key);
						}
					}
					if (chartLicense != null)
					{
						context.SetSavedLicenseKey(type, chartLicense.LicenseKey);
					}
				}
			}
			return chartLicense;
		}
	}
}
