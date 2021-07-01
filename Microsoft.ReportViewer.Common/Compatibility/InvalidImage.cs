using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices
{
	class InvalidImage
	{
		public static Bitmap Image { get; }
		public static byte[] ImageData { get; }

		static InvalidImage()
		{
			ImageData = Convert.FromBase64String("Qk3+AAAAAAAAAHYAAAAoAAAADwAAABEAAAABAAQAAAAAAAAAAADEDgAAxA4AABAAAAAQAAAAAAAA/wAAgP8AgAD/AICA/4AAAP+AAID/gIAA/8DAwP+AgID/AAD//wD/AP8A/////wAA//8A/////wD//////3AAAAAAAAAAeHd3d3d3dwB4///////3AHj///////cAeP//////9wB4///////3AHj/+Z/5n/cAeP//mZn/9wB4///5n//3AHj//5mZ//cAeP/5n/mf9wB4///////3AHj///////cAeP//////9wB4///////3AHiIiIiIiIiAd3d3d3d3d3A=");
			Image = new Bitmap(new MemoryStream(ImageData));
		}
	}
}
