using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class FormatDigitReplacement
	{
		private const int DbnumHundred = 11;

		private const int DbnumThousand = 12;

		private const int DbnumTenThousand = 13;

		private const int DbnumHundredMillion = 14;

		private const int DbnumTrillion = 15;

		private const int NUM_ASCII = 0;

		private const int NUM_ARABIC_INDIC = 1;

		private const int NUM_EXTENDED_ARABIC_INDIC = 2;

		private const int NUM_DEVANAGARI = 3;

		private const int NUM_BENGALI = 4;

		private const int NUM_GURMUKHI = 5;

		private const int NUM_GUJARATI = 6;

		private const int NUM_ORIYA = 7;

		private const int NUM_TAMIL = 8;

		private const int NUM_TELUGU = 9;

		private const int NUM_KANNADA = 10;

		private const int NUM_MALAYALAM = 11;

		private const int NUM_THAI = 12;

		private const int NUM_LAO = 13;

		private const int NUM_TIBETAN = 14;

		private const int NUM_JAPANESE1 = 15;

		private const int NUM_JAPANESE2 = 16;

		private const int NUM_JAPANESE3 = 17;

		private const int NUM_CHINESE_SIMP1 = 18;

		private const int NUM_CHINESE_SIMP2 = 19;

		private const int NUM_CHINESE_SIMP3 = 20;

		private const int NUM_CHINESE_TRAD1 = 21;

		private const int NUM_CHINESE_TRAD2 = 22;

		private const int NUM_CHINESE_TRAD3 = 23;

		private const int NUM_KOREAN1 = 24;

		private const int NUM_KOREAN2 = 25;

		private const int NUM_KOREAN3 = 26;

		private const int NUM_KOREAN4 = 27;

		internal static uint[][] DBNum_Japanese = new uint[16][]
		{
			new uint[4]
			{
				12295u,
				12295u,
				65296u,
				0u
			},
			new uint[4]
			{
				19968u,
				22769u,
				65297u,
				0u
			},
			new uint[4]
			{
				20108u,
				24336u,
				65298u,
				0u
			},
			new uint[4]
			{
				19977u,
				21442u,
				65299u,
				0u
			},
			new uint[4]
			{
				22235u,
				22235u,
				65300u,
				0u
			},
			new uint[4]
			{
				20116u,
				20237u,
				65301u,
				0u
			},
			new uint[4]
			{
				20845u,
				20845u,
				65302u,
				0u
			},
			new uint[4]
			{
				19971u,
				19971u,
				65303u,
				0u
			},
			new uint[4]
			{
				20843u,
				20843u,
				65304u,
				0u
			},
			new uint[4]
			{
				20061u,
				20061u,
				65305u,
				0u
			},
			new uint[4]
			{
				21313u,
				25342u,
				21313u,
				0u
			},
			new uint[4]
			{
				30334u,
				30334u,
				30334u,
				0u
			},
			new uint[4]
			{
				21315u,
				38433u,
				21315u,
				0u
			},
			new uint[4]
			{
				19975u,
				33836u,
				19975u,
				0u
			},
			new uint[4]
			{
				20740u,
				20740u,
				20740u,
				0u
			},
			new uint[4]
			{
				20806u,
				20806u,
				20806u,
				0u
			}
		};

		internal static uint[][] DBNum_Korean = new uint[16][]
		{
			new uint[4]
			{
				65296u,
				63922u,
				65296u,
				50689u
			},
			new uint[4]
			{
				19968u,
				22777u,
				65297u,
				51068u
			},
			new uint[4]
			{
				20108u,
				36019u,
				65298u,
				51060u
			},
			new uint[4]
			{
				19977u,
				63851u,
				65299u,
				49340u
			},
			new uint[4]
			{
				22235u,
				22235u,
				65300u,
				49324u
			},
			new uint[4]
			{
				20116u,
				20237u,
				65301u,
				50724u
			},
			new uint[4]
			{
				63953u,
				63953u,
				65302u,
				50977u
			},
			new uint[4]
			{
				19971u,
				19971u,
				65303u,
				52832u
			},
			new uint[4]
			{
				20843u,
				20843u,
				65304u,
				54036u
			},
			new uint[4]
			{
				20061u,
				20061u,
				65305u,
				44396u
			},
			new uint[4]
			{
				21313u,
				63859u,
				21313u,
				49901u
			},
			new uint[4]
			{
				30334u,
				30334u,
				30334u,
				48177u
			},
			new uint[4]
			{
				21315u,
				38433u,
				21315u,
				52380u
			},
			new uint[4]
			{
				19975u,
				33836u,
				19975u,
				47564u
			},
			new uint[4]
			{
				20740u,
				20740u,
				20740u,
				50613u
			},
			new uint[4]
			{
				20806u,
				20806u,
				20806u,
				51312u
			}
		};

		internal static uint[][] DBNum_SimplChinese = new uint[16][]
		{
			new uint[4]
			{
				9675u,
				38646u,
				65296u,
				0u
			},
			new uint[4]
			{
				19968u,
				22777u,
				65297u,
				0u
			},
			new uint[4]
			{
				20108u,
				36144u,
				65298u,
				0u
			},
			new uint[4]
			{
				19977u,
				21441u,
				65299u,
				0u
			},
			new uint[4]
			{
				22235u,
				32902u,
				65300u,
				0u
			},
			new uint[4]
			{
				20116u,
				20237u,
				65301u,
				0u
			},
			new uint[4]
			{
				20845u,
				38470u,
				65302u,
				0u
			},
			new uint[4]
			{
				19971u,
				26578u,
				65303u,
				0u
			},
			new uint[4]
			{
				20843u,
				25420u,
				65304u,
				0u
			},
			new uint[4]
			{
				20061u,
				29590u,
				65305u,
				0u
			},
			new uint[4]
			{
				21313u,
				25342u,
				21313u,
				0u
			},
			new uint[4]
			{
				30334u,
				20336u,
				30334u,
				0u
			},
			new uint[4]
			{
				21315u,
				20191u,
				21315u,
				0u
			},
			new uint[4]
			{
				19975u,
				19975u,
				19975u,
				0u
			},
			new uint[4]
			{
				20159u,
				20159u,
				20159u,
				0u
			},
			new uint[4]
			{
				20806u,
				20806u,
				20806u,
				0u
			}
		};

		internal static uint[][] DBNum_TradChinese = new uint[16][]
		{
			new uint[4]
			{
				9675u,
				38646u,
				65296u,
				0u
			},
			new uint[4]
			{
				19968u,
				22777u,
				65297u,
				0u
			},
			new uint[4]
			{
				20108u,
				36019u,
				65298u,
				0u
			},
			new uint[4]
			{
				19977u,
				21443u,
				65299u,
				0u
			},
			new uint[4]
			{
				22235u,
				32902u,
				65300u,
				0u
			},
			new uint[4]
			{
				20116u,
				20237u,
				65301u,
				0u
			},
			new uint[4]
			{
				20845u,
				38520u,
				65302u,
				0u
			},
			new uint[4]
			{
				19971u,
				26578u,
				65303u,
				0u
			},
			new uint[4]
			{
				20843u,
				25420u,
				65304u,
				0u
			},
			new uint[4]
			{
				20061u,
				29590u,
				65305u,
				0u
			},
			new uint[4]
			{
				21313u,
				25342u,
				21313u,
				0u
			},
			new uint[4]
			{
				30334u,
				20336u,
				30334u,
				0u
			},
			new uint[4]
			{
				21315u,
				20191u,
				21315u,
				0u
			},
			new uint[4]
			{
				33836u,
				33836u,
				33836u,
				0u
			},
			new uint[4]
			{
				20740u,
				20740u,
				20740u,
				0u
			},
			new uint[4]
			{
				20806u,
				20806u,
				20806u,
				0u
			}
		};

		internal static char[][] SimpleDigitMapping = new char[15][]
		{
			new char[2]
			{
				'0',
				'1'
			},
			new char[2]
			{
				'٠',
				'١'
			},
			new char[2]
			{
				'۰',
				'۱'
			},
			new char[2]
			{
				'०',
				'१'
			},
			new char[2]
			{
				'০',
				'১'
			},
			new char[2]
			{
				'੦',
				'੧'
			},
			new char[2]
			{
				'૦',
				'૧'
			},
			new char[2]
			{
				'୦',
				'୧'
			},
			new char[2]
			{
				'0',
				'௧'
			},
			new char[2]
			{
				'౦',
				'౧'
			},
			new char[2]
			{
				'೦',
				'೧'
			},
			new char[2]
			{
				'൦',
				'൧'
			},
			new char[2]
			{
				'๐',
				'๑'
			},
			new char[2]
			{
				'໐',
				'໑'
			},
			new char[2]
			{
				'༠',
				'༡'
			}
		};

		private FormatDigitReplacement()
		{
		}

		internal static char SimpleDigitFromNumeralShape(char asciiDigit, int numeralShape)
		{
			switch (asciiDigit)
			{
			default:
				return asciiDigit;
			case '0':
				return SimpleDigitMapping[numeralShape][0];
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				return (char)(SimpleDigitMapping[numeralShape][1] + asciiDigit - 49);
			}
		}

		private static string SimpleTranslateNumber(string numberValue, int numeralShape, char numberDecimalSeparator)
		{
			if (numeralShape < 0 || numeralShape > 14)
			{
				return numberValue;
			}
			char[] array = new char[numberValue.Length];
			for (int i = 0; i < numberValue.Length; i++)
			{
				char c = numberValue[i];
				if (c != numberDecimalSeparator)
				{
					array[i] = SimpleDigitFromNumeralShape(c, numeralShape);
				}
				else
				{
					array[i] = c;
				}
			}
			return new string(array);
		}

		private static void SkipNonDigits(string number, ref int index)
		{
			while (index < number.Length && (number[index] < '0' || number[index] > '9'))
			{
				index++;
			}
		}

		private static string ComplexTranslateNumber(string number, int numeralShape, char numberDecimalSeparator, int numVariant)
		{
			if (numeralShape < 15 || numeralShape > 27)
			{
				return number;
			}
			int index = 0;
			int num = 0;
			int num2 = 0;
			char[] array = new char[2 * number.Length];
			int num3 = 0;
			int num4 = 0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			uint[][] array2 = null;
			if (numeralShape <= 17)
			{
				array2 = DBNum_Japanese;
			}
			else if (numeralShape <= 20)
			{
				flag2 = true;
				array2 = DBNum_SimplChinese;
			}
			else if (numeralShape <= 23)
			{
				flag2 = true;
				array2 = DBNum_TradChinese;
			}
			else
			{
				array2 = DBNum_Korean;
				if (numVariant == 0 || numVariant == 3)
				{
					flag4 = true;
				}
			}
			if (numVariant == 1)
			{
				flag4 = true;
			}
			while (index < number.Length && (number[index] < '0' || number[index] > '9'))
			{
				array[num] = number[index];
				index++;
				num++;
			}
			for (int i = index; i < number.Length && number[i] != numberDecimalSeparator; i++)
			{
				if (number[i] >= '0' && number[i] <= '9')
				{
					num2++;
				}
			}
			if (num2 > 12)
			{
				if (num2 > 16)
				{
					while (12 < num2)
					{
						SkipNonDigits(number, ref index);
						array[num] = (char)array2[number[index] - 48][numVariant];
						index++;
						num++;
						num2--;
					}
				}
				else
				{
					num4 = 16;
					num3 = 12;
					do
					{
						if (num4 > num2)
						{
							num4--;
							num3--;
							continue;
						}
						SkipNonDigits(number, ref index);
						if (number[index] != '0')
						{
							if (flag2 || flag4 || number[index] > '1' || num4 % 4 == 1)
							{
								if (flag2 && flag)
								{
									array[num] = (char)array2[0][numVariant];
									num++;
									flag = false;
								}
								array[num] = (char)array2[number[index] - 48][numVariant];
								num++;
							}
							if (num3 >= 10)
							{
								array[num] = (char)array2[num3][numVariant];
								num++;
							}
						}
						else
						{
							flag = true;
						}
						num4--;
						num3--;
						num2--;
						index++;
					}
					while (num4 > 12);
				}
				array[num] = (char)array2[15][numVariant];
				num++;
			}
			num4 = 12;
			do
			{
				num3 = 12;
				flag3 = false;
				flag = false;
				do
				{
					if (num4 > num2)
					{
						num4--;
						num3--;
						continue;
					}
					SkipNonDigits(number, ref index);
					if (number[index] != '0' || num == 0)
					{
						if (flag2 || flag4 || number[index] > '1' || num4 % 4 == 1)
						{
							if (flag2 && flag)
							{
								array[num] = (char)array2[0][numVariant];
								num++;
								flag = false;
							}
							array[num] = (char)array2[number[index] - 48][numVariant];
							num++;
						}
						if (num3 >= 10)
						{
							array[num] = (char)array2[num3][numVariant];
							num++;
						}
						flag3 = true;
					}
					else
					{
						flag = true;
					}
					num4--;
					num3--;
					num2--;
					index++;
				}
				while (num4 % 4 > 0);
				if (flag3 && num2 / 4 > 0)
				{
					switch (num2)
					{
					case 8:
						array[num] = (char)array2[14][numVariant];
						num++;
						break;
					case 4:
						array[num] = (char)array2[13][numVariant];
						num++;
						break;
					}
				}
			}
			while (num2 > 0);
			if (index < number.Length && number[index] == numberDecimalSeparator)
			{
				array[num] = number[index];
				index++;
				num++;
				while (index < number.Length)
				{
					if (number[index] < '0' || number[index] > '9')
					{
						array[num] = number[index];
					}
					else
					{
						array[num] = (char)array2[number[index] - 48][numVariant];
					}
					index++;
					num++;
				}
			}
			return new string(array).TrimEnd(default(char));
		}

		private static int GetNumeralShape(int numeralVariant, CultureInfo numeralLanguage)
		{
			if (numeralLanguage == null)
			{
				return 0;
			}
			if (numeralVariant < 3)
			{
				return 0;
			}
			int lCID = numeralLanguage.LCID;
			if (numeralVariant == 7 && (lCID & 0xFF) == 18)
			{
				return 27;
			}
			switch (numeralVariant)
			{
			case 4:
				if ((lCID & 0xFF) == 18)
				{
					return 24;
				}
				if ((lCID & 0xFF) == 17)
				{
					return 15;
				}
				if ((lCID & 0xFF) == 4)
				{
					if (lCID == 31748)
					{
						return 21;
					}
					return 18;
				}
				break;
			case 5:
				if ((lCID & 0xFF) == 18)
				{
					return 25;
				}
				if ((lCID & 0xFF) == 17)
				{
					return 16;
				}
				if ((lCID & 0xFF) == 4)
				{
					if (lCID == 31748)
					{
						return 22;
					}
					return 19;
				}
				break;
			case 6:
				if ((lCID & 0xFF) == 18)
				{
					return 26;
				}
				if ((lCID & 0xFF) == 17)
				{
					return 17;
				}
				if ((lCID & 0xFF) == 4)
				{
					if (lCID == 31748)
					{
						return 23;
					}
					return 20;
				}
				break;
			case 3:
				switch (lCID)
				{
				case 1108:
					return 13;
				case 1105:
					return 14;
				case 1096:
					return 7;
				case 1093:
					return 4;
				}
				if ((lCID & 0xFF) == 1)
				{
					return 1;
				}
				if ((lCID & 0xFF) == 32 || (lCID & 0xFF) == 41)
				{
					return 2;
				}
				if ((lCID & 0xFF) == 57 || (lCID & 0xFF) == 87 || (lCID & 0xFF) == 78 || (lCID & 0xFF) == 79)
				{
					return 3;
				}
				if ((lCID & 0xFF) == 70)
				{
					return 5;
				}
				if ((lCID & 0xFF) == 71)
				{
					return 6;
				}
				if ((lCID & 0xFF) == 73)
				{
					return 8;
				}
				if ((lCID & 0xFF) == 74)
				{
					return 9;
				}
				if ((lCID & 0xFF) == 75)
				{
					return 10;
				}
				if ((lCID & 0xFF) == 62)
				{
					return 11;
				}
				if ((lCID & 0xFF) == 30)
				{
					return 12;
				}
				break;
			}
			return 0;
		}

		internal static string FormatNumeralVariant(string number, int numeralVariant, CultureInfo numeralLanguage, string numberDecimalSeparator, out bool numberTranslated)
		{
			numberTranslated = true;
			if (number == null || number == string.Empty)
			{
				return number;
			}
			int numeralShape = GetNumeralShape(numeralVariant, numeralLanguage);
			if (numeralShape == 0)
			{
				numberTranslated = false;
				return number;
			}
			char numberDecimalSeparator2 = '.';
			if (numberDecimalSeparator != null && numberDecimalSeparator != string.Empty)
			{
				numberDecimalSeparator2 = numberDecimalSeparator[0];
			}
			string text = number;
			if (numeralVariant >= 4)
			{
				return ComplexTranslateNumber(number, numeralShape, numberDecimalSeparator2, numeralVariant - 4);
			}
			return SimpleTranslateNumber(number, numeralShape, numberDecimalSeparator2);
		}
	}
}
