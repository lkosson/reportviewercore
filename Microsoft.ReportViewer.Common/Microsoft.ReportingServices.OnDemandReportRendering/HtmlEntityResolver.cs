using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class HtmlEntityResolver
	{
		private static Dictionary<string, char> m_entityLookupTable;

		static HtmlEntityResolver()
		{
			m_entityLookupTable = new Dictionary<string, char>(StringEqualityComparer.Instance);
			m_entityLookupTable.Add("&quot;", '"');
			m_entityLookupTable.Add("&QUOT;", '"');
			m_entityLookupTable.Add("&#34;", '"');
			m_entityLookupTable.Add("&apos;", '\'');
			m_entityLookupTable.Add("&APOS;", '\'');
			m_entityLookupTable.Add("&#39;", '\'');
			m_entityLookupTable.Add("&amp;", '&');
			m_entityLookupTable.Add("&AMP;", '&');
			m_entityLookupTable.Add("&#38;", '&');
			m_entityLookupTable.Add("&lt;", '<');
			m_entityLookupTable.Add("&LT;", '<');
			m_entityLookupTable.Add("&#60;", '<');
			m_entityLookupTable.Add("&gt;", '>');
			m_entityLookupTable.Add("&GT;", '>');
			m_entityLookupTable.Add("&#62;", '>');
			m_entityLookupTable.Add("&nbsp;", '\u00a0');
			m_entityLookupTable.Add("&#160;", '\u00a0');
			m_entityLookupTable.Add("&iexcl;", '¡');
			m_entityLookupTable.Add("&#161;", '¡');
			m_entityLookupTable.Add("&cent;", '¢');
			m_entityLookupTable.Add("&#162;", '¢');
			m_entityLookupTable.Add("&pound;", '£');
			m_entityLookupTable.Add("&#163;", '£');
			m_entityLookupTable.Add("&curren;", '¤');
			m_entityLookupTable.Add("&#164;", '¤');
			m_entityLookupTable.Add("&yen;", '¥');
			m_entityLookupTable.Add("&#165;", '¥');
			m_entityLookupTable.Add("&brvbar;", '¦');
			m_entityLookupTable.Add("&#166;", '¦');
			m_entityLookupTable.Add("&sect;", '§');
			m_entityLookupTable.Add("&#167;", '§');
			m_entityLookupTable.Add("&uml;", '\u00a8');
			m_entityLookupTable.Add("&#168;", '\u00a8');
			m_entityLookupTable.Add("&copy;", '©');
			m_entityLookupTable.Add("&COPY;", '©');
			m_entityLookupTable.Add("&#169;", '©');
			m_entityLookupTable.Add("&ordf;", 'ª');
			m_entityLookupTable.Add("&#170;", 'ª');
			m_entityLookupTable.Add("&laquo;", '«');
			m_entityLookupTable.Add("&#171;", '«');
			m_entityLookupTable.Add("&not;", '¬');
			m_entityLookupTable.Add("&#172;", '¬');
			m_entityLookupTable.Add("&shy;", '­');
			m_entityLookupTable.Add("&#173;", '­');
			m_entityLookupTable.Add("&reg;", '®');
			m_entityLookupTable.Add("&REG;", '®');
			m_entityLookupTable.Add("&#174;", '®');
			m_entityLookupTable.Add("&macr;", '\u00af');
			m_entityLookupTable.Add("&#175;", '\u00af');
			m_entityLookupTable.Add("&deg;", '°');
			m_entityLookupTable.Add("&#176;", '°');
			m_entityLookupTable.Add("&plusmn;", '±');
			m_entityLookupTable.Add("&#177;", '±');
			m_entityLookupTable.Add("&sup2;", '²');
			m_entityLookupTable.Add("&#178;", '²');
			m_entityLookupTable.Add("&sup3;", '³');
			m_entityLookupTable.Add("&#179;", '³');
			m_entityLookupTable.Add("&acute;", '\u00b4');
			m_entityLookupTable.Add("&#180;", '\u00b4');
			m_entityLookupTable.Add("&micro;", 'µ');
			m_entityLookupTable.Add("&#181;", 'µ');
			m_entityLookupTable.Add("&para;", '¶');
			m_entityLookupTable.Add("&#182;", '¶');
			m_entityLookupTable.Add("&middot;", '·');
			m_entityLookupTable.Add("&#183;", '·');
			m_entityLookupTable.Add("&cedil;", '\u00b8');
			m_entityLookupTable.Add("&#184;", '\u00b8');
			m_entityLookupTable.Add("&sup1;", '¹');
			m_entityLookupTable.Add("&#185;", '¹');
			m_entityLookupTable.Add("&ordm;", 'º');
			m_entityLookupTable.Add("&#186;", 'º');
			m_entityLookupTable.Add("&raquo;", '»');
			m_entityLookupTable.Add("&#187;", '»');
			m_entityLookupTable.Add("&frac14;", '¼');
			m_entityLookupTable.Add("&#188;", '¼');
			m_entityLookupTable.Add("&frac12;", '½');
			m_entityLookupTable.Add("&#189;", '½');
			m_entityLookupTable.Add("&frac34;", '¾');
			m_entityLookupTable.Add("&#190;", '¾');
			m_entityLookupTable.Add("&iquest;", '¿');
			m_entityLookupTable.Add("&#191;", '¿');
			m_entityLookupTable.Add("&Agrave;", 'À');
			m_entityLookupTable.Add("&#192;", 'À');
			m_entityLookupTable.Add("&Aacute;", 'Á');
			m_entityLookupTable.Add("&#193;", 'Á');
			m_entityLookupTable.Add("&Acirc;", 'Â');
			m_entityLookupTable.Add("&#194;", 'Â');
			m_entityLookupTable.Add("&Atilde;", 'Ã');
			m_entityLookupTable.Add("&#195;", 'Ã');
			m_entityLookupTable.Add("&Auml;", 'Ä');
			m_entityLookupTable.Add("&#196;", 'Ä');
			m_entityLookupTable.Add("&Aring;", 'Å');
			m_entityLookupTable.Add("&#197;", 'Å');
			m_entityLookupTable.Add("&AElig;", 'Æ');
			m_entityLookupTable.Add("&#198;", 'Æ');
			m_entityLookupTable.Add("&Ccedil;", 'Ç');
			m_entityLookupTable.Add("&#199;", 'Ç');
			m_entityLookupTable.Add("&Egrave;", 'È');
			m_entityLookupTable.Add("&#200;", 'È');
			m_entityLookupTable.Add("&Eacute;", 'É');
			m_entityLookupTable.Add("&#201;", 'É');
			m_entityLookupTable.Add("&Ecirc;", 'Ê');
			m_entityLookupTable.Add("&#202;", 'Ê');
			m_entityLookupTable.Add("&Euml;", 'Ë');
			m_entityLookupTable.Add("&#203;", 'Ë');
			m_entityLookupTable.Add("&Igrave;", 'Ì');
			m_entityLookupTable.Add("&#204;", 'Ì');
			m_entityLookupTable.Add("&Iacute;", 'Í');
			m_entityLookupTable.Add("&#205;", 'Í');
			m_entityLookupTable.Add("&Icirc;", 'Î');
			m_entityLookupTable.Add("&#206;", 'Î');
			m_entityLookupTable.Add("&Iuml;", 'Ï');
			m_entityLookupTable.Add("&#207;", 'Ï');
			m_entityLookupTable.Add("&ETH;", 'Ð');
			m_entityLookupTable.Add("&#208;", 'Ð');
			m_entityLookupTable.Add("&Ntilde;", 'Ñ');
			m_entityLookupTable.Add("&#209;", 'Ñ');
			m_entityLookupTable.Add("&Ograve;", 'Ò');
			m_entityLookupTable.Add("&#210;", 'Ò');
			m_entityLookupTable.Add("&Oacute;", 'Ó');
			m_entityLookupTable.Add("&#211;", 'Ó');
			m_entityLookupTable.Add("&Ocirc;", 'Ô');
			m_entityLookupTable.Add("&#212;", 'Ô');
			m_entityLookupTable.Add("&Otilde;", 'Õ');
			m_entityLookupTable.Add("&#213;", 'Õ');
			m_entityLookupTable.Add("&Ouml;", 'Ö');
			m_entityLookupTable.Add("&#214;", 'Ö');
			m_entityLookupTable.Add("&times;", '×');
			m_entityLookupTable.Add("&#215;", '×');
			m_entityLookupTable.Add("&Oslash;", 'Ø');
			m_entityLookupTable.Add("&#216;", 'Ø');
			m_entityLookupTable.Add("&Ugrave;", 'Ù');
			m_entityLookupTable.Add("&#217;", 'Ù');
			m_entityLookupTable.Add("&Uacute;", 'Ú');
			m_entityLookupTable.Add("&#218;", 'Ú');
			m_entityLookupTable.Add("&Ucirc;", 'Û');
			m_entityLookupTable.Add("&#219;", 'Û');
			m_entityLookupTable.Add("&Uuml;", 'Ü');
			m_entityLookupTable.Add("&#220;", 'Ü');
			m_entityLookupTable.Add("&Yacute;", 'Ý');
			m_entityLookupTable.Add("&#221;", 'Ý');
			m_entityLookupTable.Add("&THORN;", 'Þ');
			m_entityLookupTable.Add("&#222;", 'Þ');
			m_entityLookupTable.Add("&szlig;", 'ß');
			m_entityLookupTable.Add("&#223;", 'ß');
			m_entityLookupTable.Add("&agrave;", 'à');
			m_entityLookupTable.Add("&#224;", 'à');
			m_entityLookupTable.Add("&aacute;", 'á');
			m_entityLookupTable.Add("&#225;", 'á');
			m_entityLookupTable.Add("&acirc;", 'â');
			m_entityLookupTable.Add("&#226;", 'â');
			m_entityLookupTable.Add("&atilde;", 'ã');
			m_entityLookupTable.Add("&#227;", 'ã');
			m_entityLookupTable.Add("&auml;", 'ä');
			m_entityLookupTable.Add("&#228;", 'ä');
			m_entityLookupTable.Add("&aring;", 'å');
			m_entityLookupTable.Add("&#229;", 'å');
			m_entityLookupTable.Add("&aelig;", 'æ');
			m_entityLookupTable.Add("&#230;", 'æ');
			m_entityLookupTable.Add("&ccedil;", 'ç');
			m_entityLookupTable.Add("&#231;", 'ç');
			m_entityLookupTable.Add("&egrave;", 'è');
			m_entityLookupTable.Add("&#232;", 'è');
			m_entityLookupTable.Add("&eacute;", 'é');
			m_entityLookupTable.Add("&#233;", 'é');
			m_entityLookupTable.Add("&ecirc;", 'ê');
			m_entityLookupTable.Add("&#234;", 'ê');
			m_entityLookupTable.Add("&euml;", 'ë');
			m_entityLookupTable.Add("&#235;", 'ë');
			m_entityLookupTable.Add("&igrave;", 'ì');
			m_entityLookupTable.Add("&#236;", 'ì');
			m_entityLookupTable.Add("&iacute;", 'í');
			m_entityLookupTable.Add("&#237;", 'í');
			m_entityLookupTable.Add("&icirc;", 'î');
			m_entityLookupTable.Add("&#238;", 'î');
			m_entityLookupTable.Add("&iuml;", 'ï');
			m_entityLookupTable.Add("&#239;", 'ï');
			m_entityLookupTable.Add("&eth;", 'ð');
			m_entityLookupTable.Add("&#240;", 'ð');
			m_entityLookupTable.Add("&ntilde;", 'ñ');
			m_entityLookupTable.Add("&#241;", 'ñ');
			m_entityLookupTable.Add("&ograve;", 'ò');
			m_entityLookupTable.Add("&#242;", 'ò');
			m_entityLookupTable.Add("&oacute;", 'ó');
			m_entityLookupTable.Add("&#243;", 'ó');
			m_entityLookupTable.Add("&ocirc;", 'ô');
			m_entityLookupTable.Add("&#244;", 'ô');
			m_entityLookupTable.Add("&otilde;", 'õ');
			m_entityLookupTable.Add("&#245;", 'õ');
			m_entityLookupTable.Add("&ouml;", 'ö');
			m_entityLookupTable.Add("&#246;", 'ö');
			m_entityLookupTable.Add("&divide;", '÷');
			m_entityLookupTable.Add("&#247;", '÷');
			m_entityLookupTable.Add("&oslash;", 'ø');
			m_entityLookupTable.Add("&#248;", 'ø');
			m_entityLookupTable.Add("&ugrave;", 'ù');
			m_entityLookupTable.Add("&#249;", 'ù');
			m_entityLookupTable.Add("&uacute;", 'ú');
			m_entityLookupTable.Add("&#250;", 'ú');
			m_entityLookupTable.Add("&ucirc;", 'û');
			m_entityLookupTable.Add("&#251;", 'û');
			m_entityLookupTable.Add("&uuml;", 'ü');
			m_entityLookupTable.Add("&#252;", 'ü');
			m_entityLookupTable.Add("&yacute;", 'ý');
			m_entityLookupTable.Add("&#253;", 'ý');
			m_entityLookupTable.Add("&thorn;", 'þ');
			m_entityLookupTable.Add("&#254;", 'þ');
			m_entityLookupTable.Add("&yuml;", 'ÿ');
			m_entityLookupTable.Add("&#255;", 'ÿ');
			m_entityLookupTable.Add("&OElig;", 'Œ');
			m_entityLookupTable.Add("&#338;", 'Œ');
			m_entityLookupTable.Add("&oelig;", 'œ');
			m_entityLookupTable.Add("&#339;", 'œ');
			m_entityLookupTable.Add("&Scaron;", 'Š');
			m_entityLookupTable.Add("&#352;", 'Š');
			m_entityLookupTable.Add("&scaron;", 'š');
			m_entityLookupTable.Add("&#353;", 'š');
			m_entityLookupTable.Add("&hellip;", '…');
			m_entityLookupTable.Add("&#8230;", '…');
			m_entityLookupTable.Add("&permil;", '‰');
			m_entityLookupTable.Add("&#8240;", '‰');
			m_entityLookupTable.Add("&lsaquo;", '‹');
			m_entityLookupTable.Add("&#8249;", '‹');
			m_entityLookupTable.Add("&rsaquo;", '›');
			m_entityLookupTable.Add("&#8250;", '›');
			m_entityLookupTable.Add("&euro;", '€');
			m_entityLookupTable.Add("&#8364;", '€');
			m_entityLookupTable.Add("&Yuml;", 'Ÿ');
			m_entityLookupTable.Add("&#376;", 'Ÿ');
			m_entityLookupTable.Add("&circ;", 'ˆ');
			m_entityLookupTable.Add("&#710;", 'ˆ');
			m_entityLookupTable.Add("&tilde;", '\u02dc');
			m_entityLookupTable.Add("&#732;", '\u02dc');
			m_entityLookupTable.Add("&ensp;", '\u2002');
			m_entityLookupTable.Add("&#8194;", '\u2002');
			m_entityLookupTable.Add("&emsp;", '\u2003');
			m_entityLookupTable.Add("&#8195;", '\u2003');
			m_entityLookupTable.Add("&thinsp;", '\u2009');
			m_entityLookupTable.Add("&#8201;", '\u2009');
			m_entityLookupTable.Add("&zwnj;", '\u200c');
			m_entityLookupTable.Add("&#8204;", '\u200c');
			m_entityLookupTable.Add("&zwj;", '\u200d');
			m_entityLookupTable.Add("&#8205;", '\u200d');
			m_entityLookupTable.Add("&lrm;", '\u200e');
			m_entityLookupTable.Add("&#8206;", '\u200e');
			m_entityLookupTable.Add("&rlm;", '\u200f');
			m_entityLookupTable.Add("&#8207;", '\u200f');
			m_entityLookupTable.Add("&ndash;", '–');
			m_entityLookupTable.Add("&#8211;", '–');
			m_entityLookupTable.Add("&mdash;", '—');
			m_entityLookupTable.Add("&#8212;", '—');
			m_entityLookupTable.Add("&lsquo;", '‘');
			m_entityLookupTable.Add("&#8216;", '‘');
			m_entityLookupTable.Add("&rsquo;", '’');
			m_entityLookupTable.Add("&#8217;", '’');
			m_entityLookupTable.Add("&sbquo;", '‚');
			m_entityLookupTable.Add("&#8218;", '‚');
			m_entityLookupTable.Add("&ldquo;", '“');
			m_entityLookupTable.Add("&#8220;", '“');
			m_entityLookupTable.Add("&rdquo;", '”');
			m_entityLookupTable.Add("&#8221;", '”');
			m_entityLookupTable.Add("&bdquo;", '„');
			m_entityLookupTable.Add("&#8222;", '„');
			m_entityLookupTable.Add("&dagger;", '†');
			m_entityLookupTable.Add("&#8224;", '†');
			m_entityLookupTable.Add("&Dagger;", '‡');
			m_entityLookupTable.Add("&#8225;", '‡');
			m_entityLookupTable.Add("&trade;", '™');
			m_entityLookupTable.Add("&TRADE;", '™');
			m_entityLookupTable.Add("&#8482;", '™');
		}

		internal static string ResolveEntities(string html)
		{
			StringBuilder stringBuilder = new StringBuilder(html);
			ResolveEntities(stringBuilder);
			return stringBuilder.ToString();
		}

		internal static void ResolveEntities(StringBuilder sb)
		{
			for (int i = 0; i < sb.Length; i++)
			{
				if (sb[i] == '&' && GetEntity(sb, i, out string entity, out string entityName) && m_entityLookupTable.TryGetValue(entityName, out char value))
				{
					sb.Replace(entity, value.ToString(CultureInfo.InvariantCulture), i, entity.Length);
				}
			}
		}

		private static bool GetEntity(StringBuilder sb, int index, out string entity, out string entityName)
		{
			entity = null;
			entityName = null;
			int num = index + 1;
			while (num < sb.Length)
			{
				char c = sb[num];
				if (c != ' ' && c != '&')
				{
					if (c != ';')
					{
						num++;
						continue;
					}
				}
				else
				{
					num--;
				}
				int num2 = num - index + 1;
				if (num2 >= 2)
				{
					entity = sb.ToString(index, num2);
					if (c != ';')
					{
						entityName = entityName + entity + ";";
					}
					else
					{
						entityName = entity;
					}
					return true;
				}
				return false;
			}
			return false;
		}

		internal static string ResolveEntity(string entity)
		{
			if (m_entityLookupTable.TryGetValue(entity, out char value))
			{
				return new string(value, 1);
			}
			if (entity.Length > 3 && entity[1] == '#' && int.TryParse(entity.Substring(2, entity.Length - 3), out int result))
			{
				return new string((char)result, 1);
			}
			return entity;
		}
	}
}
