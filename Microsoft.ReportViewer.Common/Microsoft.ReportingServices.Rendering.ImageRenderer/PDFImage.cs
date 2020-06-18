using Microsoft.ReportingServices.Rendering.RPLProcessing;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class PDFImage
	{
		internal int ImageId = -1;

		internal byte[] ImageData;

		internal GDIImageProps GdiProperties;

		private bool m_isMonochromeJpeg;

		internal bool IsMonochromeJpeg
		{
			get
			{
				if (ImageData == null)
				{
					return m_isMonochromeJpeg;
				}
				m_isMonochromeJpeg = false;
				int num = 0;
				int num2 = 2;
				if (ImageData[0] != byte.MaxValue || ImageData[1] != 216)
				{
					return m_isMonochromeJpeg;
				}
				do
				{
					byte b = ImageData[num2 + 1];
					num2 += 2;
					switch (b)
					{
					default:
						switch (b)
						{
						default:
							if ((b >= 197 && b <= 203) || (b >= 205 && b <= 207))
							{
								break;
							}
							num = ImageData[num2] << 8;
							num += ImageData[num2 + 1];
							num2 += num;
							continue;
						case 192:
						case 193:
						case 194:
						case 195:
						case 222:
							break;
						}
						num2 += 7;
						if (ImageData[num2] == 1)
						{
							m_isMonochromeJpeg = true;
						}
						break;
					case 1:
					case 208:
					case 209:
					case 210:
					case 211:
					case 212:
					case 213:
					case 214:
					case 215:
					case 216:
					case 217:
						continue;
					}
					break;
				}
				while (num2 < ImageData.Length);
				return m_isMonochromeJpeg;
			}
		}
	}
}
