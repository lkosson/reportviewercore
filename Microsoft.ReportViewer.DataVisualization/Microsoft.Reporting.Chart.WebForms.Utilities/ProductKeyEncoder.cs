using System.IO;
using System.Security.Cryptography;

namespace Microsoft.Reporting.Chart.WebForms.Utilities
{
	internal class ProductKeyEncoder
	{
		private byte[] rijnKey = new byte[32]
		{
			56,
			98,
			2,
			45,
			98,
			27,
			93,
			225,
			157,
			61,
			123,
			89,
			221,
			187,
			6,
			83,
			35,
			87,
			12,
			193,
			41,
			32,
			193,
			34,
			19,
			4,
			201,
			44,
			77,
			39,
			1,
			134
		};

		private byte[] rijnIV = new byte[16]
		{
			46,
			35,
			95,
			143,
			9,
			222,
			73,
			126,
			184,
			19,
			73,
			55,
			168,
			214,
			250,
			3
		};

		private string BytesToString(byte[] bytes)
		{
			string text = "";
			foreach (byte b in bytes)
			{
				string str = text;
				char c = (char)b;
				text = str + c;
			}
			return text;
		}

		private byte[] StringToBytes(string str)
		{
			byte[] array = new byte[str.Length];
			int num = 0;
			foreach (char c in str)
			{
				array[num++] = (byte)c;
			}
			return array;
		}

		public string DecryptLicenseKey(string key)
		{
			MemoryStream memoryStream = new MemoryStream(1024);
			MemoryStream memoryStream2 = new MemoryStream(1024);
			byte[] array = StringToBytes(key);
			SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateDecryptor(rijnKey, rijnIV), CryptoStreamMode.Write);
			CryptoStream cryptoStream2 = new CryptoStream(memoryStream2, new FromBase64Transform(), CryptoStreamMode.Write);
			cryptoStream2.Write(array, 0, array.Length);
			cryptoStream2.Close();
			cryptoStream.Write(memoryStream2.ToArray(), 0, memoryStream2.ToArray().Length);
			cryptoStream.Close();
			return BytesToString(memoryStream.ToArray());
		}
	}
}
