using System;
using System.Collections;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class BuiltInFonts
	{
		private static Ffn Times_New_Roman;

		private static Ffn Symbol;

		private static Ffn Arial;

		private static Ffn Agency_FB;

		private static Ffn Algerian;

		private static Ffn Arial_Black;

		private static Ffn Arial_Narrow;

		private static Ffn Arial_Rounded_MT_Bold;

		private static Ffn Arial_Unicode_MS;

		private static Ffn Baskerville_Old_Face;

		private static Ffn Batang;

		private static Ffn Bauhaus_93;

		private static Ffn Bell_MT;

		private static Ffn Berlin_Sans_FB;

		private static Ffn Berlin_Sans_FB_Demi;

		private static Ffn Bernard_MT_Condensed;

		private static Ffn Bitstream_Vera_Sans;

		private static Ffn Bitstream_Vera_Sans_Mono;

		private static Ffn Bitstream_Vera_Serif;

		private static Ffn Blackadder_ITC;

		private static Ffn Bodoni_MT;

		private static Ffn Bodoni_MT_Black;

		private static Ffn Bodoni_MT_Condensed;

		private static Ffn Bodoni_MT_Poster_Compressed;

		private static Ffn Book_Antiqua;

		private static Ffn Bookman_Old_Style;

		private static Ffn Bradley_Hand_ITC;

		private static Ffn Britannic_Bold;

		private static Ffn Broadway;

		private static Ffn Brush_Script_MT;

		private static Ffn Californian_FB;

		private static Ffn Calisto_MT;

		private static Ffn Castellar;

		private static Ffn Centaur;

		private static Ffn Century;

		private static Ffn Century_Gothic;

		private static Ffn Century_Schoolbook;

		private static Ffn Chiller;

		private static Ffn Colonna_MT;

		private static Ffn Comic_Sans_MS;

		private static Ffn Cooper_Black;

		private static Ffn Copperplate_Gothic_Bold;

		private static Ffn Copperplate_Gothic_Light;

		private static Ffn Courier_New;

		private static Ffn Curlz_MT;

		private static Ffn Edwardian_Script_ITC;

		private static Ffn Elephant;

		private static Ffn Engravers_MT;

		private static Ffn Eras_Bold_ITC;

		private static Ffn Eras_Demi_ITC;

		private static Ffn Eras_Light_ITC;

		private static Ffn Eras_Medium_ITC;

		private static Ffn Estrangelo_Edessa;

		private static Ffn Felix_Titling;

		private static Ffn Footlight_MT_Light;

		private static Ffn Forte;

		private static Ffn Franklin_Gothic_Book;

		private static Ffn Franklin_Gothic_Demi;

		private static Ffn Franklin_Gothic_Demi_Cond;

		private static Ffn Franklin_Gothic_Heavy;

		private static Ffn Franklin_Gothic_Medium;

		private static Ffn Franklin_Gothic_Medium_Cond;

		private static Ffn Freestyle_Script;

		private static Ffn French_Script_MT;

		private static Ffn Garamond;

		private static Ffn Gautami;

		private static Ffn Georgia;

		private static Ffn Gigi;

		private static Ffn Gill_Sans_MT;

		private static Ffn Gill_Sans_MT_Condensed;

		private static Ffn Gill_Sans_MT_Ext_Condensed_Bold;

		private static Ffn Gill_Sans_Ultra_Bold;

		private static Ffn Gill_Sans_Ultra_Bold_Condensed;

		private static Ffn Gloucester_MT_Extra_Condensed;

		private static Ffn Goudy_Stout;

		private static Ffn Haettenschweiler;

		private static Ffn Harlow_Solid_Italic;

		private static Ffn Harrington;

		private static Ffn High_Tower_Text;

		private static Ffn Impact;

		private static Ffn Imprint_MT_Shadow;

		private static Ffn Informal_Roman;

		private static Ffn Jokerman;

		private static Ffn Juice_ITC;

		private static Ffn Kristen_ITC;

		private static Ffn Kunstler_Script;

		private static Ffn Latha;

		private static Ffn Lucida_Bright;

		private static Ffn Lucida_Calligraphy;

		private static Ffn Lucida_Console;

		private static Ffn Lucida_Fax;

		private static Ffn Onyx;

		private static Ffn Lucida_Handwriting;

		private static Ffn Lucida_Sans;

		private static Ffn Lucida_Sans_Typewriter;

		private static Ffn Lucida_Sans_Unicode;

		private static Ffn Magneto;

		private static Ffn Maiandra_GD;

		private static Ffn Mangal;

		private static Ffn Matura_MT_Script_Capitals;

		private static Ffn Microsoft_Sans_Serif;

		private static Ffn Mistral;

		private static Ffn Modern_No__20;

		private static Ffn Monotype_Corsiva;

		private static Ffn MS_Mincho;

		private static Ffn MS_Reference_Sans_Serif;

		private static Ffn MV_Boli;

		private static Ffn Niagara_Engraved;

		private static Ffn Niagara_Solid;

		private static Ffn OCR_A_Extended;

		private static Ffn Old_English_Text_MT;

		private static Ffn Palace_Script_MT;

		private static Ffn Palatino_Linotype;

		private static Ffn Papyrus;

		private static Ffn Parchment;

		private static Ffn Perpetua;

		private static Ffn Perpetua_Titling_MT;

		private static Ffn Playbill;

		private static Ffn Poor_Richard;

		private static Ffn Pristina;

		private static Ffn Raavi;

		private static Ffn Rage_Italic;

		private static Ffn Ravie;

		private static Ffn Rockwell;

		private static Ffn Rockwell_Condensed;

		private static Ffn Rockwell_Extra_Bold;

		private static Ffn Script_MT_Bold;

		private static Ffn Showcard_Gothic;

		private static Ffn Shruti;

		private static Ffn SimSun;

		private static Ffn Snap_ITC;

		private static Ffn Stencil;

		private static Ffn Sylfaen;

		private static Ffn Tahoma;

		private static Ffn Tempus_Sans_ITC;

		private static Ffn Trebuchet_MS;

		private static Ffn Tunga;

		private static Ffn Tw_Cen_MT;

		private static Ffn Tw_Cen_MT_Condensed;

		private static Ffn Tw_Cen_MT_Condensed_Extra_Bold;

		private static Ffn Verdana;

		private static Ffn Viner_Hand_ITC;

		private static Ffn Vivaldi;

		private static Ffn Vladimir_Script;

		private static Ffn Wide_Latin;

		private static Hashtable m_fontMap;

		private static readonly int BaseFontSize;

		internal static Ffn GetFont(string name)
		{
			Ffn ffn = (Ffn)m_fontMap[name];
			if (ffn == null)
			{
				char[] array = new char[name.Length + 1];
				Array.Copy(name.ToCharArray(), array, name.Length);
				ffn = new Ffn(BaseFontSize + array.Length * 2, 38, 400, 0, 0, new byte[10]
				{
					2,
					11,
					6,
					4,
					2,
					2,
					2,
					2,
					2,
					4
				}, new byte[24]
				{
					135,
					122,
					0,
					32,
					0,
					0,
					0,
					128,
					8,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					255,
					1,
					0,
					0,
					0,
					0,
					0,
					0
				}, array);
			}
			return ffn;
		}

		static BuiltInFonts()
		{
			Times_New_Roman = new Ffn(71, 22, 400, 0, 0, new byte[10]
			{
				2,
				2,
				6,
				3,
				5,
				4,
				5,
				2,
				3,
				4
			}, new byte[24]
			{
				135,
				122,
				0,
				32,
				0,
				0,
				0,
				128,
				8,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				255,
				1,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[16]
			{
				'T',
				'i',
				'm',
				'e',
				's',
				' ',
				'N',
				'e',
				'w',
				' ',
				'R',
				'o',
				'm',
				'a',
				'n',
				'\0'
			});
			byte[] panose = new byte[10]
			{
				5,
				5,
				1,
				2,
				1,
				7,
				6,
				2,
				5,
				7
			};
			byte[] array = new byte[24];
			array[7] = 16;
			array[19] = 128;
			Symbol = new Ffn(53, 22, 400, 2, 0, panose, array, new char[7]
			{
				'S',
				'y',
				'm',
				'b',
				'o',
				'l',
				'\0'
			});
			Arial = new Ffn(51, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				4,
				2,
				2,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				122,
				0,
				32,
				0,
				0,
				0,
				128,
				8,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				255,
				1,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[6]
			{
				'A',
				'r',
				'i',
				'a',
				'l',
				'\0'
			});
			byte[] panose2 = new byte[10]
			{
				2,
				11,
				5,
				3,
				2,
				2,
				2,
				2,
				2,
				4
			};
			byte[] array2 = new byte[24];
			array2[0] = 3;
			array2[16] = 1;
			Agency_FB = new Ffn(59, 38, 400, 0, 0, panose2, array2, new char[10]
			{
				'A',
				'g',
				'e',
				'n',
				'c',
				'y',
				' ',
				'F',
				'B',
				'\0'
			});
			byte[] panose3 = new byte[10]
			{
				4,
				2,
				7,
				5,
				4,
				10,
				2,
				6,
				7,
				2
			};
			byte[] array3 = new byte[24];
			array3[0] = 3;
			array3[16] = 1;
			Algerian = new Ffn(57, 86, 400, 0, 0, panose3, array3, new char[9]
			{
				'A',
				'l',
				'g',
				'e',
				'r',
				'i',
				'a',
				'n',
				'\0'
			});
			Arial_Black = new Ffn(63, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				10,
				4,
				2,
				1,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[12]
			{
				'A',
				'r',
				'i',
				'a',
				'l',
				' ',
				'B',
				'l',
				'a',
				'c',
				'k',
				'\0'
			});
			Arial_Narrow = new Ffn(65, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				5,
				6,
				2,
				2,
				2,
				3,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[13]
			{
				'A',
				'r',
				'i',
				'a',
				'l',
				' ',
				'N',
				'a',
				'r',
				'r',
				'o',
				'w',
				'\0'
			});
			byte[] panose4 = new byte[10]
			{
				2,
				15,
				7,
				4,
				3,
				5,
				4,
				3,
				2,
				4
			};
			byte[] array4 = new byte[24];
			array4[0] = 3;
			array4[16] = 1;
			Arial_Rounded_MT_Bold = new Ffn(83, 38, 400, 0, 0, panose4, array4, new char[22]
			{
				'A',
				'r',
				'i',
				'a',
				'l',
				' ',
				'R',
				'o',
				'u',
				'n',
				'd',
				'e',
				'd',
				' ',
				'M',
				'T',
				' ',
				'B',
				'o',
				'l',
				'd',
				'\0'
			});
			Arial_Unicode_MS = new Ffn(73, 38, 400, 128, 0, new byte[10]
			{
				2,
				11,
				6,
				4,
				2,
				2,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				255,
				255,
				255,
				255,
				255,
				255,
				255,
				233,
				63,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				255,
				1,
				63,
				0,
				0,
				0,
				0,
				0
			}, new char[17]
			{
				'A',
				'r',
				'i',
				'a',
				'l',
				' ',
				'U',
				'n',
				'i',
				'c',
				'o',
				'd',
				'e',
				' ',
				'M',
				'S',
				'\0'
			});
			byte[] panose5 = new byte[10]
			{
				2,
				2,
				6,
				2,
				8,
				5,
				5,
				2,
				3,
				3
			};
			byte[] array5 = new byte[24];
			array5[0] = 3;
			array5[16] = 1;
			Baskerville_Old_Face = new Ffn(81, 22, 400, 0, 0, panose5, array5, new char[21]
			{
				'B',
				'a',
				's',
				'k',
				'e',
				'r',
				'v',
				'i',
				'l',
				'l',
				'e',
				' ',
				'O',
				'l',
				'd',
				' ',
				'F',
				'a',
				'c',
				'e',
				'\0'
			});
			Batang = new Ffn(59, 22, 400, 129, 7, new byte[10]
			{
				2,
				3,
				6,
				0,
				0,
				1,
				1,
				1,
				1,
				1
			}, new byte[24]
			{
				175,
				2,
				0,
				176,
				251,
				124,
				215,
				105,
				48,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				8,
				0,
				0,
				0,
				0,
				0
			}, new char[10]
			{
				'B',
				'a',
				't',
				'a',
				'n',
				'g',
				'\0',
				'?',
				'?',
				'\0'
			});
			byte[] panose6 = new byte[10]
			{
				4,
				3,
				9,
				5,
				2,
				11,
				2,
				2,
				12,
				2
			};
			byte[] array6 = new byte[24];
			array6[0] = 3;
			array6[16] = 1;
			Bauhaus_93 = new Ffn(61, 86, 400, 0, 0, panose6, array6, new char[11]
			{
				'B',
				'a',
				'u',
				'h',
				'a',
				'u',
				's',
				' ',
				'9',
				'3',
				'\0'
			});
			byte[] panose7 = new byte[10]
			{
				2,
				2,
				5,
				3,
				6,
				3,
				5,
				2,
				3,
				3
			};
			byte[] array7 = new byte[24];
			array7[0] = 3;
			array7[16] = 1;
			Bell_MT = new Ffn(55, 22, 400, 0, 0, panose7, array7, new char[8]
			{
				'B',
				'e',
				'l',
				'l',
				' ',
				'M',
				'T',
				'\0'
			});
			byte[] panose8 = new byte[10]
			{
				2,
				14,
				6,
				2,
				2,
				5,
				2,
				2,
				3,
				6
			};
			byte[] array8 = new byte[24];
			array8[0] = 3;
			array8[16] = 1;
			Berlin_Sans_FB = new Ffn(69, 38, 400, 0, 0, panose8, array8, new char[15]
			{
				'B',
				'e',
				'r',
				'l',
				'i',
				'n',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'F',
				'B',
				'\0'
			});
			byte[] panose9 = new byte[10]
			{
				2,
				14,
				8,
				2,
				2,
				5,
				2,
				2,
				3,
				6
			};
			byte[] array9 = new byte[24];
			array9[0] = 3;
			array9[16] = 1;
			Berlin_Sans_FB_Demi = new Ffn(79, 38, 700, 0, 0, panose9, array9, new char[20]
			{
				'B',
				'e',
				'r',
				'l',
				'i',
				'n',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'F',
				'B',
				' ',
				'D',
				'e',
				'm',
				'i',
				'\0'
			});
			byte[] panose10 = new byte[10]
			{
				2,
				5,
				8,
				6,
				6,
				9,
				5,
				2,
				4,
				4
			};
			byte[] array10 = new byte[24];
			array10[0] = 3;
			array10[16] = 1;
			Bernard_MT_Condensed = new Ffn(81, 22, 400, 0, 0, panose10, array10, new char[21]
			{
				'B',
				'e',
				'r',
				'n',
				'a',
				'r',
				'd',
				' ',
				'M',
				'T',
				' ',
				'C',
				'o',
				'n',
				'd',
				'e',
				'n',
				's',
				'e',
				'd',
				'\0'
			});
			Bitstream_Vera_Sans = new Ffn(79, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				3,
				3,
				8,
				4,
				2,
				2,
				4
			}, new byte[24]
			{
				175,
				0,
				0,
				128,
				74,
				32,
				0,
				16,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[20]
			{
				'B',
				'i',
				't',
				's',
				't',
				'r',
				'e',
				'a',
				'm',
				' ',
				'V',
				'e',
				'r',
				'a',
				' ',
				'S',
				'a',
				'n',
				's',
				'\0'
			});
			Bitstream_Vera_Sans_Mono = new Ffn(89, 53, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				9,
				3,
				8,
				4,
				2,
				2,
				4
			}, new byte[24]
			{
				175,
				0,
				0,
				128,
				74,
				32,
				0,
				16,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[25]
			{
				'B',
				'i',
				't',
				's',
				't',
				'r',
				'e',
				'a',
				'm',
				' ',
				'V',
				'e',
				'r',
				'a',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'M',
				'o',
				'n',
				'o',
				'\0'
			});
			Bitstream_Vera_Serif = new Ffn(81, 22, 400, 0, 0, new byte[10]
			{
				2,
				6,
				6,
				3,
				5,
				6,
				5,
				2,
				2,
				4
			}, new byte[24]
			{
				175,
				0,
				0,
				128,
				74,
				32,
				0,
				16,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[21]
			{
				'B',
				'i',
				't',
				's',
				't',
				'r',
				'e',
				'a',
				'm',
				' ',
				'V',
				'e',
				'r',
				'a',
				' ',
				'S',
				'e',
				'r',
				'i',
				'f',
				'\0'
			});
			byte[] panose11 = new byte[10]
			{
				4,
				2,
				5,
				5,
				5,
				16,
				7,
				2,
				13,
				2
			};
			byte[] array11 = new byte[24];
			array11[0] = 3;
			array11[16] = 1;
			Blackadder_ITC = new Ffn(69, 86, 400, 0, 0, panose11, array11, new char[15]
			{
				'B',
				'l',
				'a',
				'c',
				'k',
				'a',
				'd',
				'd',
				'e',
				'r',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			byte[] panose12 = new byte[10]
			{
				2,
				7,
				6,
				3,
				8,
				6,
				6,
				2,
				2,
				3
			};
			byte[] array12 = new byte[24];
			array12[0] = 3;
			array12[16] = 1;
			Bodoni_MT = new Ffn(59, 22, 400, 0, 0, panose12, array12, new char[10]
			{
				'B',
				'o',
				'd',
				'o',
				'n',
				'i',
				' ',
				'M',
				'T',
				'\0'
			});
			byte[] panose13 = new byte[10]
			{
				2,
				7,
				10,
				3,
				8,
				6,
				6,
				2,
				2,
				3
			};
			byte[] array13 = new byte[24];
			array13[0] = 3;
			array13[16] = 1;
			Bodoni_MT_Black = new Ffn(71, 22, 900, 0, 0, panose13, array13, new char[16]
			{
				'B',
				'o',
				'd',
				'o',
				'n',
				'i',
				' ',
				'M',
				'T',
				' ',
				'B',
				'l',
				'a',
				'c',
				'k',
				'\0'
			});
			byte[] panose14 = new byte[10]
			{
				2,
				7,
				6,
				6,
				8,
				6,
				6,
				2,
				2,
				3
			};
			byte[] array14 = new byte[24];
			array14[0] = 3;
			array14[16] = 1;
			Bodoni_MT_Condensed = new Ffn(79, 22, 400, 0, 0, panose14, array14, new char[20]
			{
				'B',
				'o',
				'd',
				'o',
				'n',
				'i',
				' ',
				'M',
				'T',
				' ',
				'C',
				'o',
				'n',
				'd',
				'e',
				'n',
				's',
				'e',
				'd',
				'\0'
			});
			byte[] panose15 = new byte[10]
			{
				2,
				7,
				7,
				6,
				8,
				6,
				1,
				5,
				2,
				4
			};
			byte[] array15 = new byte[24];
			array15[0] = 7;
			array15[16] = 17;
			Bodoni_MT_Poster_Compressed = new Ffn(95, 22, 300, 0, 0, panose15, array15, new char[28]
			{
				'B',
				'o',
				'd',
				'o',
				'n',
				'i',
				' ',
				'M',
				'T',
				' ',
				'P',
				'o',
				's',
				't',
				'e',
				'r',
				' ',
				'C',
				'o',
				'm',
				'p',
				'r',
				'e',
				's',
				's',
				'e',
				'd',
				'\0'
			});
			Book_Antiqua = new Ffn(65, 22, 400, 0, 0, new byte[10]
			{
				2,
				4,
				6,
				2,
				5,
				3,
				5,
				3,
				3,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[13]
			{
				'B',
				'o',
				'o',
				'k',
				' ',
				'A',
				'n',
				't',
				'i',
				'q',
				'u',
				'a',
				'\0'
			});
			Bookman_Old_Style = new Ffn(75, 22, 300, 0, 0, new byte[10]
			{
				2,
				5,
				6,
				4,
				5,
				5,
				5,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[18]
			{
				'B',
				'o',
				'o',
				'k',
				'm',
				'a',
				'n',
				' ',
				'O',
				'l',
				'd',
				' ',
				'S',
				't',
				'y',
				'l',
				'e',
				'\0'
			});
			byte[] panose16 = new byte[10]
			{
				3,
				7,
				4,
				2,
				5,
				3,
				2,
				3,
				2,
				3
			};
			byte[] array16 = new byte[24];
			array16[0] = 3;
			array16[16] = 1;
			Bradley_Hand_ITC = new Ffn(73, 70, 400, 0, 0, panose16, array16, new char[17]
			{
				'B',
				'r',
				'a',
				'd',
				'l',
				'e',
				'y',
				' ',
				'H',
				'a',
				'n',
				'd',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			byte[] panose17 = new byte[10]
			{
				2,
				11,
				9,
				3,
				6,
				7,
				3,
				2,
				2,
				4
			};
			byte[] array17 = new byte[24];
			array17[0] = 3;
			array17[16] = 1;
			Britannic_Bold = new Ffn(69, 38, 400, 0, 0, panose17, array17, new char[15]
			{
				'B',
				'r',
				'i',
				't',
				'a',
				'n',
				'n',
				'i',
				'c',
				' ',
				'B',
				'o',
				'l',
				'd',
				'\0'
			});
			byte[] panose18 = new byte[10]
			{
				4,
				4,
				9,
				5,
				8,
				11,
				2,
				2,
				5,
				2
			};
			byte[] array18 = new byte[24];
			array18[0] = 3;
			array18[16] = 1;
			Broadway = new Ffn(57, 86, 400, 0, 0, panose18, array18, new char[9]
			{
				'B',
				'r',
				'o',
				'a',
				'd',
				'w',
				'a',
				'y',
				'\0'
			});
			byte[] panose19 = new byte[10]
			{
				3,
				6,
				8,
				2,
				4,
				4,
				6,
				7,
				3,
				4
			};
			byte[] array19 = new byte[24];
			array19[0] = 3;
			array19[16] = 1;
			Brush_Script_MT = new Ffn(71, 70, 400, 0, 0, panose19, array19, new char[16]
			{
				'B',
				'r',
				'u',
				's',
				'h',
				' ',
				'S',
				'c',
				'r',
				'i',
				'p',
				't',
				' ',
				'M',
				'T',
				'\0'
			});
			byte[] panose20 = new byte[10]
			{
				2,
				7,
				4,
				3,
				6,
				8,
				11,
				3,
				2,
				4
			};
			byte[] array20 = new byte[24];
			array20[0] = 3;
			array20[16] = 1;
			Californian_FB = new Ffn(69, 22, 400, 0, 0, panose20, array20, new char[15]
			{
				'C',
				'a',
				'l',
				'i',
				'f',
				'o',
				'r',
				'n',
				'i',
				'a',
				'n',
				' ',
				'F',
				'B',
				'\0'
			});
			byte[] panose21 = new byte[10]
			{
				2,
				4,
				6,
				3,
				5,
				5,
				5,
				3,
				3,
				4
			};
			byte[] array21 = new byte[24];
			array21[0] = 3;
			array21[16] = 1;
			Calisto_MT = new Ffn(61, 22, 400, 0, 0, panose21, array21, new char[11]
			{
				'C',
				'a',
				'l',
				'i',
				's',
				't',
				'o',
				' ',
				'M',
				'T',
				'\0'
			});
			byte[] panose22 = new byte[10]
			{
				2,
				10,
				4,
				2,
				6,
				4,
				6,
				1,
				3,
				1
			};
			byte[] array22 = new byte[24];
			array22[0] = 3;
			array22[16] = 1;
			Castellar = new Ffn(59, 22, 400, 0, 0, panose22, array22, new char[10]
			{
				'C',
				'a',
				's',
				't',
				'e',
				'l',
				'l',
				'a',
				'r',
				'\0'
			});
			byte[] panose23 = new byte[10]
			{
				2,
				3,
				5,
				4,
				5,
				2,
				5,
				2,
				3,
				4
			};
			byte[] array23 = new byte[24];
			array23[0] = 3;
			array23[16] = 1;
			Centaur = new Ffn(55, 22, 400, 0, 0, panose23, array23, new char[8]
			{
				'C',
				'e',
				'n',
				't',
				'a',
				'u',
				'r',
				'\0'
			});
			Century = new Ffn(55, 22, 400, 0, 0, new byte[10]
			{
				2,
				4,
				6,
				4,
				5,
				5,
				5,
				2,
				3,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'C',
				'e',
				'n',
				't',
				'u',
				'r',
				'y',
				'\0'
			});
			Century_Gothic = new Ffn(69, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				5,
				2,
				2,
				2,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[15]
			{
				'C',
				'e',
				'n',
				't',
				'u',
				'r',
				'y',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				'\0'
			});
			Century_Schoolbook = new Ffn(77, 22, 400, 0, 0, new byte[10]
			{
				2,
				4,
				6,
				4,
				5,
				5,
				5,
				2,
				3,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[19]
			{
				'C',
				'e',
				'n',
				't',
				'u',
				'r',
				'y',
				' ',
				'S',
				'c',
				'h',
				'o',
				'o',
				'l',
				'b',
				'o',
				'o',
				'k',
				'\0'
			});
			byte[] panose24 = new byte[10]
			{
				4,
				2,
				4,
				4,
				3,
				16,
				7,
				2,
				6,
				2
			};
			byte[] array24 = new byte[24];
			array24[0] = 3;
			array24[16] = 1;
			Chiller = new Ffn(55, 86, 400, 0, 0, panose24, array24, new char[8]
			{
				'C',
				'h',
				'i',
				'l',
				'l',
				'e',
				'r',
				'\0'
			});
			byte[] panose25 = new byte[10]
			{
				4,
				2,
				8,
				5,
				6,
				2,
				2,
				3,
				2,
				3
			};
			byte[] array25 = new byte[24];
			array25[0] = 3;
			array25[16] = 1;
			Colonna_MT = new Ffn(61, 86, 400, 0, 0, panose25, array25, new char[11]
			{
				'C',
				'o',
				'l',
				'o',
				'n',
				'n',
				'a',
				' ',
				'M',
				'T',
				'\0'
			});
			Comic_Sans_MS = new Ffn(67, 70, 400, 0, 0, new byte[10]
			{
				3,
				15,
				7,
				2,
				3,
				3,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[14]
			{
				'C',
				'o',
				'm',
				'i',
				'c',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'M',
				'S',
				'\0'
			});
			byte[] panose26 = new byte[10]
			{
				2,
				8,
				9,
				4,
				4,
				3,
				11,
				2,
				4,
				4
			};
			byte[] array26 = new byte[24];
			array26[0] = 3;
			array26[16] = 1;
			Cooper_Black = new Ffn(65, 22, 400, 0, 0, panose26, array26, new char[13]
			{
				'C',
				'o',
				'o',
				'p',
				'e',
				'r',
				' ',
				'B',
				'l',
				'a',
				'c',
				'k',
				'\0'
			});
			byte[] panose27 = new byte[10]
			{
				2,
				14,
				7,
				5,
				2,
				2,
				6,
				2,
				4,
				4
			};
			byte[] array27 = new byte[24];
			array27[0] = 3;
			array27[16] = 1;
			Copperplate_Gothic_Bold = new Ffn(87, 38, 400, 0, 0, panose27, array27, new char[24]
			{
				'C',
				'o',
				'p',
				'p',
				'e',
				'r',
				'p',
				'l',
				'a',
				't',
				'e',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				' ',
				'B',
				'o',
				'l',
				'd',
				'\0'
			});
			byte[] panose28 = new byte[10]
			{
				2,
				14,
				5,
				7,
				2,
				2,
				6,
				2,
				4,
				4
			};
			byte[] array28 = new byte[24];
			array28[0] = 3;
			array28[16] = 1;
			Copperplate_Gothic_Light = new Ffn(89, 38, 400, 0, 0, panose28, array28, new char[25]
			{
				'C',
				'o',
				'p',
				'p',
				'e',
				'r',
				'p',
				'l',
				'a',
				't',
				'e',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				' ',
				'L',
				'i',
				'g',
				'h',
				't',
				'\0'
			});
			Courier_New = new Ffn(63, 53, 400, 0, 0, new byte[10]
			{
				2,
				7,
				3,
				9,
				2,
				2,
				5,
				2,
				4,
				4
			}, new byte[24]
			{
				135,
				122,
				0,
				32,
				0,
				0,
				0,
				128,
				8,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				255,
				1,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[12]
			{
				'C',
				'o',
				'u',
				'r',
				'i',
				'e',
				'r',
				' ',
				'N',
				'e',
				'w',
				'\0'
			});
			byte[] panose29 = new byte[10]
			{
				4,
				4,
				4,
				4,
				5,
				7,
				2,
				2,
				2,
				2
			};
			byte[] array29 = new byte[24];
			array29[0] = 3;
			array29[16] = 1;
			Curlz_MT = new Ffn(57, 86, 400, 0, 0, panose29, array29, new char[9]
			{
				'C',
				'u',
				'r',
				'l',
				'z',
				' ',
				'M',
				'T',
				'\0'
			});
			byte[] panose30 = new byte[10]
			{
				3,
				3,
				3,
				2,
				4,
				7,
				7,
				13,
				8,
				4
			};
			byte[] array30 = new byte[24];
			array30[0] = 3;
			array30[16] = 1;
			Edwardian_Script_ITC = new Ffn(81, 70, 400, 0, 0, panose30, array30, new char[21]
			{
				'E',
				'd',
				'w',
				'a',
				'r',
				'd',
				'i',
				'a',
				'n',
				' ',
				'S',
				'c',
				'r',
				'i',
				'p',
				't',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			byte[] panose31 = new byte[10]
			{
				2,
				2,
				9,
				4,
				9,
				5,
				5,
				2,
				3,
				3
			};
			byte[] array31 = new byte[24];
			array31[0] = 3;
			array31[16] = 1;
			Elephant = new Ffn(57, 22, 400, 0, 0, panose31, array31, new char[9]
			{
				'E',
				'l',
				'e',
				'p',
				'h',
				'a',
				'n',
				't',
				'\0'
			});
			byte[] panose32 = new byte[10]
			{
				2,
				9,
				7,
				7,
				8,
				5,
				5,
				2,
				3,
				4
			};
			byte[] array32 = new byte[24];
			array32[0] = 3;
			array32[16] = 1;
			Engravers_MT = new Ffn(65, 22, 500, 0, 0, panose32, array32, new char[13]
			{
				'E',
				'n',
				'g',
				'r',
				'a',
				'v',
				'e',
				'r',
				's',
				' ',
				'M',
				'T',
				'\0'
			});
			byte[] panose33 = new byte[10]
			{
				2,
				11,
				9,
				7,
				3,
				5,
				4,
				2,
				2,
				4
			};
			byte[] array33 = new byte[24];
			array33[0] = 3;
			array33[16] = 1;
			Eras_Bold_ITC = new Ffn(67, 38, 400, 0, 0, panose33, array33, new char[14]
			{
				'E',
				'r',
				'a',
				's',
				' ',
				'B',
				'o',
				'l',
				'd',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			byte[] panose34 = new byte[10]
			{
				2,
				11,
				8,
				5,
				3,
				5,
				4,
				2,
				8,
				4
			};
			byte[] array34 = new byte[24];
			array34[0] = 3;
			array34[16] = 1;
			Eras_Demi_ITC = new Ffn(67, 38, 400, 0, 0, panose34, array34, new char[14]
			{
				'E',
				'r',
				'a',
				's',
				' ',
				'D',
				'e',
				'm',
				'i',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			byte[] panose35 = new byte[10]
			{
				2,
				11,
				4,
				2,
				3,
				5,
				4,
				2,
				8,
				4
			};
			byte[] array35 = new byte[24];
			array35[0] = 3;
			array35[16] = 1;
			Eras_Light_ITC = new Ffn(69, 38, 400, 0, 0, panose35, array35, new char[15]
			{
				'E',
				'r',
				'a',
				's',
				' ',
				'L',
				'i',
				'g',
				'h',
				't',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			byte[] panose36 = new byte[10]
			{
				2,
				11,
				6,
				2,
				3,
				5,
				4,
				2,
				8,
				4
			};
			byte[] array36 = new byte[24];
			array36[0] = 3;
			array36[16] = 1;
			Eras_Medium_ITC = new Ffn(71, 38, 400, 0, 0, panose36, array36, new char[16]
			{
				'E',
				'r',
				'a',
				's',
				' ',
				'M',
				'e',
				'd',
				'i',
				'u',
				'm',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			Estrangelo_Edessa = new Ffn(75, 70, 400, 0, 0, new byte[10], new byte[24]
			{
				67,
				96,
				0,
				128,
				0,
				0,
				0,
				0,
				128,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[18]
			{
				'E',
				's',
				't',
				'r',
				'a',
				'n',
				'g',
				'e',
				'l',
				'o',
				' ',
				'E',
				'd',
				'e',
				's',
				's',
				'a',
				'\0'
			});
			byte[] panose37 = new byte[10]
			{
				4,
				6,
				5,
				5,
				6,
				2,
				2,
				2,
				10,
				4
			};
			byte[] array37 = new byte[24];
			array37[0] = 3;
			array37[16] = 1;
			Felix_Titling = new Ffn(67, 86, 400, 0, 0, panose37, array37, new char[14]
			{
				'F',
				'e',
				'l',
				'i',
				'x',
				' ',
				'T',
				'i',
				't',
				'l',
				'i',
				'n',
				'g',
				'\0'
			});
			byte[] panose38 = new byte[10]
			{
				2,
				4,
				6,
				2,
				6,
				3,
				10,
				2,
				3,
				4
			};
			byte[] array38 = new byte[24];
			array38[0] = 3;
			array38[16] = 1;
			Footlight_MT_Light = new Ffn(77, 22, 300, 0, 0, panose38, array38, new char[19]
			{
				'F',
				'o',
				'o',
				't',
				'l',
				'i',
				'g',
				'h',
				't',
				' ',
				'M',
				'T',
				' ',
				'L',
				'i',
				'g',
				'h',
				't',
				'\0'
			});
			byte[] panose39 = new byte[10]
			{
				3,
				6,
				9,
				2,
				4,
				5,
				2,
				7,
				2,
				3
			};
			byte[] array39 = new byte[24];
			array39[0] = 3;
			array39[16] = 1;
			Forte = new Ffn(51, 70, 400, 0, 0, panose39, array39, new char[6]
			{
				'F',
				'o',
				'r',
				't',
				'e',
				'\0'
			});
			Franklin_Gothic_Book = new Ffn(81, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				5,
				3,
				2,
				1,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[21]
			{
				'F',
				'r',
				'a',
				'n',
				'k',
				'l',
				'i',
				'n',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				' ',
				'B',
				'o',
				'o',
				'k',
				'\0'
			});
			Franklin_Gothic_Demi = new Ffn(81, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				7,
				3,
				2,
				1,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[21]
			{
				'F',
				'r',
				'a',
				'n',
				'k',
				'l',
				'i',
				'n',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				' ',
				'D',
				'e',
				'm',
				'i',
				'\0'
			});
			Franklin_Gothic_Demi_Cond = new Ffn(91, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				7,
				6,
				3,
				4,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[26]
			{
				'F',
				'r',
				'a',
				'n',
				'k',
				'l',
				'i',
				'n',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				' ',
				'D',
				'e',
				'm',
				'i',
				' ',
				'C',
				'o',
				'n',
				'd',
				'\0'
			});
			Franklin_Gothic_Heavy = new Ffn(83, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				9,
				3,
				2,
				1,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[22]
			{
				'F',
				'r',
				'a',
				'n',
				'k',
				'l',
				'i',
				'n',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				' ',
				'H',
				'e',
				'a',
				'v',
				'y',
				'\0'
			});
			Franklin_Gothic_Medium = new Ffn(85, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				3,
				2,
				1,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[23]
			{
				'F',
				'r',
				'a',
				'n',
				'k',
				'l',
				'i',
				'n',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				' ',
				'M',
				'e',
				'd',
				'i',
				'u',
				'm',
				'\0'
			});
			Franklin_Gothic_Medium_Cond = new Ffn(95, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				6,
				3,
				4,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[28]
			{
				'F',
				'r',
				'a',
				'n',
				'k',
				'l',
				'i',
				'n',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				' ',
				'M',
				'e',
				'd',
				'i',
				'u',
				'm',
				' ',
				'C',
				'o',
				'n',
				'd',
				'\0'
			});
			byte[] panose40 = new byte[10]
			{
				3,
				8,
				4,
				2,
				3,
				2,
				5,
				11,
				4,
				4
			};
			byte[] array40 = new byte[24];
			array40[0] = 3;
			array40[16] = 1;
			Freestyle_Script = new Ffn(73, 70, 400, 0, 0, panose40, array40, new char[17]
			{
				'F',
				'r',
				'e',
				'e',
				's',
				't',
				'y',
				'l',
				'e',
				' ',
				'S',
				'c',
				'r',
				'i',
				'p',
				't',
				'\0'
			});
			byte[] panose41 = new byte[10]
			{
				3,
				2,
				4,
				2,
				4,
				6,
				7,
				4,
				6,
				5
			};
			byte[] array41 = new byte[24];
			array41[0] = 3;
			array41[16] = 1;
			French_Script_MT = new Ffn(73, 70, 400, 0, 0, panose41, array41, new char[17]
			{
				'F',
				'r',
				'e',
				'n',
				'c',
				'h',
				' ',
				'S',
				'c',
				'r',
				'i',
				'p',
				't',
				' ',
				'M',
				'T',
				'\0'
			});
			Garamond = new Ffn(57, 22, 400, 0, 0, new byte[10]
			{
				2,
				2,
				4,
				4,
				3,
				3,
				1,
				1,
				8,
				3
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[9]
			{
				'G',
				'a',
				'r',
				'a',
				'm',
				'o',
				'n',
				'd',
				'\0'
			});
			Gautami = new Ffn(55, 6, 400, 0, 0, new byte[10]
			{
				2,
				0,
				5,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new byte[24]
			{
				3,
				0,
				32,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'G',
				'a',
				'u',
				't',
				'a',
				'm',
				'i',
				'\0'
			});
			Georgia = new Ffn(55, 22, 400, 0, 0, new byte[10]
			{
				2,
				4,
				5,
				2,
				5,
				4,
				5,
				2,
				3,
				3
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'G',
				'e',
				'o',
				'r',
				'g',
				'i',
				'a',
				'\0'
			});
			byte[] panose42 = new byte[10]
			{
				4,
				4,
				5,
				4,
				6,
				16,
				7,
				2,
				13,
				2
			};
			byte[] array42 = new byte[24];
			array42[0] = 3;
			array42[16] = 1;
			Gigi = new Ffn(49, 86, 400, 0, 0, panose42, array42, new char[5]
			{
				'G',
				'i',
				'g',
				'i',
				'\0'
			});
			byte[] panose43 = new byte[10]
			{
				2,
				11,
				5,
				2,
				2,
				1,
				4,
				2,
				2,
				3
			};
			byte[] array43 = new byte[24];
			array43[0] = 7;
			array43[16] = 3;
			Gill_Sans_MT = new Ffn(65, 38, 400, 0, 0, panose43, array43, new char[13]
			{
				'G',
				'i',
				'l',
				'l',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'M',
				'T',
				'\0'
			});
			byte[] panose44 = new byte[10]
			{
				2,
				11,
				5,
				6,
				2,
				1,
				4,
				2,
				2,
				3
			};
			byte[] array44 = new byte[24];
			array44[0] = 7;
			array44[16] = 3;
			Gill_Sans_MT_Condensed = new Ffn(85, 38, 400, 0, 0, panose44, array44, new char[23]
			{
				'G',
				'i',
				'l',
				'l',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'M',
				'T',
				' ',
				'C',
				'o',
				'n',
				'd',
				'e',
				'n',
				's',
				'e',
				'd',
				'\0'
			});
			byte[] panose45 = new byte[10]
			{
				2,
				11,
				9,
				2,
				2,
				1,
				4,
				2,
				2,
				3
			};
			byte[] array45 = new byte[24];
			array45[0] = 7;
			array45[16] = 3;
			Gill_Sans_MT_Ext_Condensed_Bold = new Ffn(103, 38, 400, 0, 0, panose45, array45, new char[32]
			{
				'G',
				'i',
				'l',
				'l',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'M',
				'T',
				' ',
				'E',
				'x',
				't',
				' ',
				'C',
				'o',
				'n',
				'd',
				'e',
				'n',
				's',
				'e',
				'd',
				' ',
				'B',
				'o',
				'l',
				'd',
				'\0'
			});
			byte[] panose46 = new byte[10]
			{
				2,
				11,
				10,
				2,
				2,
				1,
				4,
				2,
				2,
				3
			};
			byte[] array46 = new byte[24];
			array46[0] = 7;
			array46[16] = 3;
			Gill_Sans_Ultra_Bold = new Ffn(81, 38, 400, 0, 0, panose46, array46, new char[21]
			{
				'G',
				'i',
				'l',
				'l',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'U',
				'l',
				't',
				'r',
				'a',
				' ',
				'B',
				'o',
				'l',
				'd',
				'\0'
			});
			byte[] panose47 = new byte[10]
			{
				2,
				11,
				10,
				6,
				2,
				1,
				4,
				2,
				2,
				3
			};
			byte[] array47 = new byte[24];
			array47[0] = 7;
			array47[16] = 3;
			Gill_Sans_Ultra_Bold_Condensed = new Ffn(101, 38, 400, 0, 0, panose47, array47, new char[31]
			{
				'G',
				'i',
				'l',
				'l',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'U',
				'l',
				't',
				'r',
				'a',
				' ',
				'B',
				'o',
				'l',
				'd',
				' ',
				'C',
				'o',
				'n',
				'd',
				'e',
				'n',
				's',
				'e',
				'd',
				'\0'
			});
			byte[] panose48 = new byte[10]
			{
				2,
				3,
				8,
				8,
				2,
				6,
				1,
				1,
				1,
				1
			};
			byte[] array48 = new byte[24];
			array48[0] = 3;
			array48[16] = 1;
			Gloucester_MT_Extra_Condensed = new Ffn(99, 22, 400, 0, 0, panose48, array48, new char[30]
			{
				'G',
				'l',
				'o',
				'u',
				'c',
				'e',
				's',
				't',
				'e',
				'r',
				' ',
				'M',
				'T',
				' ',
				'E',
				'x',
				't',
				'r',
				'a',
				' ',
				'C',
				'o',
				'n',
				'd',
				'e',
				'n',
				's',
				'e',
				'd',
				'\0'
			});
			byte[] panose49 = new byte[10]
			{
				2,
				2,
				9,
				4,
				7,
				3,
				11,
				2,
				4,
				1
			};
			byte[] array49 = new byte[24];
			array49[0] = 3;
			array49[16] = 1;
			Goudy_Stout = new Ffn(63, 22, 400, 0, 0, panose49, array49, new char[12]
			{
				'G',
				'o',
				'u',
				'd',
				'y',
				' ',
				'S',
				't',
				'o',
				'u',
				't',
				'\0'
			});
			Haettenschweiler = new Ffn(73, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				7,
				6,
				4,
				9,
				2,
				6,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[17]
			{
				'H',
				'a',
				'e',
				't',
				't',
				'e',
				'n',
				's',
				'c',
				'h',
				'w',
				'e',
				'i',
				'l',
				'e',
				'r',
				'\0'
			});
			byte[] panose50 = new byte[10]
			{
				4,
				3,
				6,
				4,
				2,
				15,
				2,
				2,
				13,
				2
			};
			byte[] array50 = new byte[24];
			array50[0] = 3;
			array50[16] = 1;
			Harlow_Solid_Italic = new Ffn(79, 86, 400, 0, 0, panose50, array50, new char[20]
			{
				'H',
				'a',
				'r',
				'l',
				'o',
				'w',
				' ',
				'S',
				'o',
				'l',
				'i',
				'd',
				' ',
				'I',
				't',
				'a',
				'l',
				'i',
				'c',
				'\0'
			});
			byte[] panose51 = new byte[10]
			{
				4,
				4,
				5,
				5,
				5,
				10,
				2,
				2,
				7,
				2
			};
			byte[] array51 = new byte[24];
			array51[0] = 3;
			array51[16] = 1;
			Harrington = new Ffn(61, 86, 400, 0, 0, panose51, array51, new char[11]
			{
				'H',
				'a',
				'r',
				'r',
				'i',
				'n',
				'g',
				't',
				'o',
				'n',
				'\0'
			});
			byte[] panose52 = new byte[10]
			{
				2,
				4,
				5,
				2,
				5,
				5,
				6,
				3,
				3,
				3
			};
			byte[] array52 = new byte[24];
			array52[0] = 3;
			array52[16] = 1;
			High_Tower_Text = new Ffn(71, 22, 400, 0, 0, panose52, array52, new char[16]
			{
				'H',
				'i',
				'g',
				'h',
				' ',
				'T',
				'o',
				'w',
				'e',
				'r',
				' ',
				'T',
				'e',
				'x',
				't',
				'\0'
			});
			Impact = new Ffn(53, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				8,
				6,
				3,
				9,
				2,
				5,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[7]
			{
				'I',
				'm',
				'p',
				'a',
				'c',
				't',
				'\0'
			});
			byte[] panose53 = new byte[10]
			{
				4,
				2,
				6,
				5,
				6,
				3,
				3,
				3,
				2,
				2
			};
			byte[] array53 = new byte[24];
			array53[0] = 3;
			array53[16] = 1;
			Imprint_MT_Shadow = new Ffn(75, 86, 400, 0, 0, panose53, array53, new char[18]
			{
				'I',
				'm',
				'p',
				'r',
				'i',
				'n',
				't',
				' ',
				'M',
				'T',
				' ',
				'S',
				'h',
				'a',
				'd',
				'o',
				'w',
				'\0'
			});
			byte[] panose54 = new byte[10]
			{
				3,
				6,
				4,
				2,
				3,
				4,
				6,
				11,
				2,
				4
			};
			byte[] array54 = new byte[24];
			array54[0] = 3;
			array54[16] = 1;
			Informal_Roman = new Ffn(69, 70, 400, 0, 0, panose54, array54, new char[15]
			{
				'I',
				'n',
				'f',
				'o',
				'r',
				'm',
				'a',
				'l',
				' ',
				'R',
				'o',
				'm',
				'a',
				'n',
				'\0'
			});
			byte[] panose55 = new byte[10]
			{
				4,
				9,
				6,
				5,
				6,
				13,
				6,
				2,
				7,
				2
			};
			byte[] array55 = new byte[24];
			array55[0] = 3;
			array55[16] = 1;
			Jokerman = new Ffn(57, 86, 400, 0, 0, panose55, array55, new char[9]
			{
				'J',
				'o',
				'k',
				'e',
				'r',
				'm',
				'a',
				'n',
				'\0'
			});
			byte[] panose56 = new byte[10]
			{
				4,
				4,
				4,
				3,
				4,
				10,
				2,
				2,
				2,
				2
			};
			byte[] array56 = new byte[24];
			array56[0] = 3;
			array56[16] = 1;
			Juice_ITC = new Ffn(59, 86, 400, 0, 0, panose56, array56, new char[10]
			{
				'J',
				'u',
				'i',
				'c',
				'e',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			byte[] panose57 = new byte[10]
			{
				3,
				5,
				5,
				2,
				4,
				2,
				2,
				3,
				2,
				2
			};
			byte[] array57 = new byte[24];
			array57[0] = 3;
			array57[16] = 1;
			Kristen_ITC = new Ffn(63, 70, 400, 0, 0, panose57, array57, new char[12]
			{
				'K',
				'r',
				'i',
				's',
				't',
				'e',
				'n',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			byte[] panose58 = new byte[10]
			{
				3,
				3,
				4,
				2,
				2,
				6,
				7,
				13,
				13,
				6
			};
			byte[] array58 = new byte[24];
			array58[0] = 3;
			array58[16] = 1;
			Kunstler_Script = new Ffn(71, 70, 400, 0, 0, panose58, array58, new char[16]
			{
				'K',
				'u',
				'n',
				's',
				't',
				'l',
				'e',
				'r',
				' ',
				'S',
				'c',
				'r',
				'i',
				'p',
				't',
				'\0'
			});
			Latha = new Ffn(51, 6, 400, 0, 0, new byte[10]
			{
				2,
				0,
				4,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new byte[24]
			{
				3,
				0,
				16,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[6]
			{
				'L',
				'a',
				't',
				'h',
				'a',
				'\0'
			});
			byte[] panose59 = new byte[10]
			{
				2,
				4,
				6,
				2,
				5,
				5,
				5,
				2,
				3,
				4
			};
			byte[] array59 = new byte[24];
			array59[0] = 3;
			array59[16] = 1;
			Lucida_Bright = new Ffn(67, 22, 400, 0, 0, panose59, array59, new char[14]
			{
				'L',
				'u',
				'c',
				'i',
				'd',
				'a',
				' ',
				'B',
				'r',
				'i',
				'g',
				'h',
				't',
				'\0'
			});
			byte[] panose60 = new byte[10]
			{
				3,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				1
			};
			byte[] array60 = new byte[24];
			array60[0] = 3;
			array60[16] = 1;
			Lucida_Calligraphy = new Ffn(77, 70, 400, 0, 0, panose60, array60, new char[19]
			{
				'L',
				'u',
				'c',
				'i',
				'd',
				'a',
				' ',
				'C',
				'a',
				'l',
				'l',
				'i',
				'g',
				'r',
				'a',
				'p',
				'h',
				'y',
				'\0'
			});
			Lucida_Console = new Ffn(69, 53, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				9,
				4,
				5,
				4,
				2,
				2,
				4
			}, new byte[24]
			{
				143,
				2,
				0,
				128,
				0,
				24,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				31,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[15]
			{
				'L',
				'u',
				'c',
				'i',
				'd',
				'a',
				' ',
				'C',
				'o',
				'n',
				's',
				'o',
				'l',
				'e',
				'\0'
			});
			byte[] panose61 = new byte[10]
			{
				2,
				6,
				6,
				2,
				5,
				5,
				5,
				2,
				2,
				4
			};
			byte[] array61 = new byte[24];
			array61[0] = 3;
			array61[16] = 1;
			Lucida_Fax = new Ffn(61, 22, 400, 0, 0, panose61, array61, new char[11]
			{
				'L',
				'u',
				'c',
				'i',
				'd',
				'a',
				' ',
				'F',
				'a',
				'x',
				'\0'
			});
			byte[] panose62 = new byte[10]
			{
				4,
				5,
				6,
				2,
				8,
				7,
				2,
				2,
				2,
				3
			};
			byte[] array62 = new byte[24];
			array62[0] = 3;
			array62[16] = 1;
			Onyx = new Ffn(49, 86, 400, 0, 0, panose62, array62, new char[5]
			{
				'O',
				'n',
				'y',
				'x',
				'\0'
			});
			byte[] panose63 = new byte[10]
			{
				3,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				1
			};
			byte[] array63 = new byte[24];
			array63[0] = 3;
			array63[16] = 1;
			Lucida_Handwriting = new Ffn(77, 70, 400, 0, 0, panose63, array63, new char[19]
			{
				'L',
				'u',
				'c',
				'i',
				'd',
				'a',
				' ',
				'H',
				'a',
				'n',
				'd',
				'w',
				'r',
				'i',
				't',
				'i',
				'n',
				'g',
				'\0'
			});
			byte[] panose64 = new byte[10]
			{
				2,
				11,
				6,
				2,
				3,
				5,
				4,
				2,
				2,
				4
			};
			byte[] array64 = new byte[24];
			array64[0] = 3;
			array64[16] = 1;
			Lucida_Sans = new Ffn(63, 38, 400, 0, 0, panose64, array64, new char[12]
			{
				'L',
				'u',
				'c',
				'i',
				'd',
				'a',
				' ',
				'S',
				'a',
				'n',
				's',
				'\0'
			});
			byte[] panose65 = new byte[10]
			{
				2,
				11,
				5,
				9,
				3,
				5,
				4,
				3,
				2,
				4
			};
			byte[] array65 = new byte[24];
			array65[0] = 3;
			array65[16] = 1;
			Lucida_Sans_Typewriter = new Ffn(85, 53, 400, 0, 0, panose65, array65, new char[23]
			{
				'L',
				'u',
				'c',
				'i',
				'd',
				'a',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'T',
				'y',
				'p',
				'e',
				'w',
				'r',
				'i',
				't',
				'e',
				'r',
				'\0'
			});
			Lucida_Sans_Unicode = new Ffn(79, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				2,
				3,
				5,
				4,
				2,
				2,
				4
			}, new byte[24]
			{
				255,
				26,
				0,
				128,
				107,
				57,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				63,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[20]
			{
				'L',
				'u',
				'c',
				'i',
				'd',
				'a',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'U',
				'n',
				'i',
				'c',
				'o',
				'd',
				'e',
				'\0'
			});
			byte[] panose66 = new byte[10]
			{
				4,
				3,
				8,
				5,
				5,
				8,
				2,
				2,
				13,
				2
			};
			byte[] array66 = new byte[24];
			array66[0] = 3;
			array66[16] = 1;
			Magneto = new Ffn(55, 86, 700, 0, 0, panose66, array66, new char[8]
			{
				'M',
				'a',
				'g',
				'n',
				'e',
				't',
				'o',
				'\0'
			});
			byte[] panose67 = new byte[10]
			{
				2,
				14,
				5,
				2,
				3,
				3,
				8,
				2,
				2,
				4
			};
			byte[] array67 = new byte[24];
			array67[0] = 3;
			array67[16] = 1;
			Maiandra_GD = new Ffn(63, 38, 400, 0, 0, panose67, array67, new char[12]
			{
				'M',
				'a',
				'i',
				'a',
				'n',
				'd',
				'r',
				'a',
				' ',
				'G',
				'D',
				'\0'
			});
			Mangal = new Ffn(53, 6, 400, 0, 0, new byte[10]
			{
				0,
				0,
				4,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new byte[24]
			{
				3,
				128,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[7]
			{
				'M',
				'a',
				'n',
				'g',
				'a',
				'l',
				'\0'
			});
			byte[] panose68 = new byte[10]
			{
				3,
				2,
				8,
				2,
				6,
				6,
				2,
				7,
				2,
				2
			};
			byte[] array68 = new byte[24];
			array68[0] = 3;
			array68[16] = 1;
			Matura_MT_Script_Capitals = new Ffn(91, 70, 400, 0, 0, panose68, array68, new char[26]
			{
				'M',
				'a',
				't',
				'u',
				'r',
				'a',
				' ',
				'M',
				'T',
				' ',
				'S',
				'c',
				'r',
				'i',
				'p',
				't',
				' ',
				'C',
				'a',
				'p',
				'i',
				't',
				'a',
				'l',
				's',
				'\0'
			});
			Microsoft_Sans_Serif = new Ffn(81, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				4,
				2,
				2,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				122,
				0,
				33,
				0,
				0,
				0,
				128,
				8,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				255,
				1,
				1,
				0,
				0,
				0,
				0,
				0
			}, new char[21]
			{
				'M',
				'i',
				'c',
				'r',
				'o',
				's',
				'o',
				'f',
				't',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'S',
				'e',
				'r',
				'i',
				'f',
				'\0'
			});
			Mistral = new Ffn(55, 70, 400, 0, 0, new byte[10]
			{
				3,
				9,
				7,
				2,
				3,
				4,
				7,
				2,
				4,
				3
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'M',
				'i',
				's',
				't',
				'r',
				'a',
				'l',
				'\0'
			});
			byte[] panose69 = new byte[10]
			{
				2,
				7,
				7,
				4,
				7,
				5,
				5,
				2,
				3,
				3
			};
			byte[] array69 = new byte[24];
			array69[0] = 3;
			array69[16] = 1;
			Modern_No__20 = new Ffn(67, 22, 400, 0, 0, panose69, array69, new char[14]
			{
				'M',
				'o',
				'd',
				'e',
				'r',
				'n',
				' ',
				'N',
				'o',
				'.',
				' ',
				'2',
				'0',
				'\0'
			});
			Monotype_Corsiva = new Ffn(73, 70, 400, 0, 0, new byte[10]
			{
				3,
				1,
				1,
				1,
				1,
				2,
				1,
				1,
				1,
				1
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[17]
			{
				'M',
				'o',
				'n',
				'o',
				't',
				'y',
				'p',
				'e',
				' ',
				'C',
				'o',
				'r',
				's',
				'i',
				'v',
				'a',
				'\0'
			});
			MS_Mincho = new Ffn(71, 53, 400, 128, 10, new byte[10]
			{
				2,
				2,
				6,
				9,
				4,
				2,
				5,
				8,
				3,
				4
			}, new byte[24]
			{
				191,
				2,
				0,
				160,
				251,
				252,
				199,
				104,
				16,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				2,
				0,
				0,
				0,
				0,
				0
			}, new char[16]
			{
				'M',
				'S',
				' ',
				'M',
				'i',
				'n',
				'c',
				'h',
				'o',
				'\0',
				'?',
				'?',
				' ',
				'?',
				'?',
				'\0'
			});
			MS_Reference_Sans_Serif = new Ffn(87, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				4,
				3,
				5,
				4,
				4,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				32,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				1,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[24]
			{
				'M',
				'S',
				' ',
				'R',
				'e',
				'f',
				'e',
				'r',
				'e',
				'n',
				'c',
				'e',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'S',
				'e',
				'r',
				'i',
				'f',
				'\0'
			});
			MV_Boli = new Ffn(55, 6, 400, 0, 0, new byte[10], new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'M',
				'V',
				' ',
				'B',
				'o',
				'l',
				'i',
				'\0'
			});
			byte[] panose70 = new byte[10]
			{
				4,
				2,
				5,
				2,
				7,
				7,
				3,
				3,
				2,
				2
			};
			byte[] array70 = new byte[24];
			array70[0] = 3;
			array70[16] = 1;
			Niagara_Engraved = new Ffn(73, 86, 400, 0, 0, panose70, array70, new char[17]
			{
				'N',
				'i',
				'a',
				'g',
				'a',
				'r',
				'a',
				' ',
				'E',
				'n',
				'g',
				'r',
				'a',
				'v',
				'e',
				'd',
				'\0'
			});
			byte[] panose71 = new byte[10]
			{
				4,
				2,
				5,
				2,
				7,
				7,
				2,
				2,
				2,
				2
			};
			byte[] array71 = new byte[24];
			array71[0] = 3;
			array71[16] = 1;
			Niagara_Solid = new Ffn(67, 86, 400, 0, 0, panose71, array71, new char[14]
			{
				'N',
				'i',
				'a',
				'g',
				'a',
				'r',
				'a',
				' ',
				'S',
				'o',
				'l',
				'i',
				'd',
				'\0'
			});
			byte[] panose72 = new byte[10]
			{
				2,
				1,
				5,
				9,
				2,
				1,
				2,
				1,
				3,
				3
			};
			byte[] array72 = new byte[24];
			array72[0] = 3;
			array72[16] = 1;
			OCR_A_Extended = new Ffn(69, 54, 400, 0, 0, panose72, array72, new char[15]
			{
				'O',
				'C',
				'R',
				' ',
				'A',
				' ',
				'E',
				'x',
				't',
				'e',
				'n',
				'd',
				'e',
				'd',
				'\0'
			});
			byte[] panose73 = new byte[10]
			{
				3,
				4,
				9,
				2,
				4,
				5,
				8,
				3,
				8,
				6
			};
			byte[] array73 = new byte[24];
			array73[0] = 3;
			array73[16] = 1;
			Old_English_Text_MT = new Ffn(79, 70, 400, 0, 0, panose73, array73, new char[20]
			{
				'O',
				'l',
				'd',
				' ',
				'E',
				'n',
				'g',
				'l',
				'i',
				's',
				'h',
				' ',
				'T',
				'e',
				'x',
				't',
				' ',
				'M',
				'T',
				'\0'
			});
			byte[] panose74 = new byte[10]
			{
				3,
				3,
				3,
				2,
				2,
				6,
				7,
				12,
				11,
				5
			};
			byte[] array74 = new byte[24];
			array74[0] = 3;
			array74[16] = 1;
			Palace_Script_MT = new Ffn(73, 70, 400, 0, 0, panose74, array74, new char[17]
			{
				'P',
				'a',
				'l',
				'a',
				'c',
				'e',
				' ',
				'S',
				'c',
				'r',
				'i',
				'p',
				't',
				' ',
				'M',
				'T',
				'\0'
			});
			Palatino_Linotype = new Ffn(75, 22, 400, 0, 0, new byte[10]
			{
				2,
				4,
				5,
				2,
				5,
				5,
				5,
				3,
				3,
				4
			}, new byte[24]
			{
				135,
				3,
				0,
				224,
				19,
				0,
				0,
				64,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				1,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[18]
			{
				'P',
				'a',
				'l',
				'a',
				't',
				'i',
				'n',
				'o',
				' ',
				'L',
				'i',
				'n',
				'o',
				't',
				'y',
				'p',
				'e',
				'\0'
			});
			byte[] panose75 = new byte[10]
			{
				3,
				7,
				5,
				2,
				6,
				5,
				2,
				3,
				2,
				5
			};
			byte[] array75 = new byte[24];
			array75[0] = 3;
			array75[16] = 1;
			Papyrus = new Ffn(55, 70, 400, 0, 0, panose75, array75, new char[8]
			{
				'P',
				'a',
				'p',
				'y',
				'r',
				'u',
				's',
				'\0'
			});
			byte[] panose76 = new byte[10]
			{
				3,
				4,
				6,
				2,
				4,
				7,
				8,
				4,
				8,
				4
			};
			byte[] array76 = new byte[24];
			array76[0] = 3;
			array76[16] = 1;
			Parchment = new Ffn(59, 70, 400, 0, 0, panose76, array76, new char[10]
			{
				'P',
				'a',
				'r',
				'c',
				'h',
				'm',
				'e',
				'n',
				't',
				'\0'
			});
			byte[] panose77 = new byte[10]
			{
				2,
				2,
				5,
				2,
				6,
				4,
				1,
				2,
				3,
				3
			};
			byte[] array77 = new byte[24];
			array77[0] = 3;
			array77[16] = 1;
			Perpetua = new Ffn(57, 22, 400, 0, 0, panose77, array77, new char[9]
			{
				'P',
				'e',
				'r',
				'p',
				'e',
				't',
				'u',
				'a',
				'\0'
			});
			byte[] panose78 = new byte[10]
			{
				2,
				2,
				5,
				2,
				6,
				5,
				5,
				2,
				8,
				4
			};
			byte[] array78 = new byte[24];
			array78[0] = 3;
			array78[16] = 1;
			Perpetua_Titling_MT = new Ffn(79, 22, 300, 0, 0, panose78, array78, new char[20]
			{
				'P',
				'e',
				'r',
				'p',
				'e',
				't',
				'u',
				'a',
				' ',
				'T',
				'i',
				't',
				'l',
				'i',
				'n',
				'g',
				' ',
				'M',
				'T',
				'\0'
			});
			byte[] panose79 = new byte[10]
			{
				4,
				5,
				6,
				3,
				10,
				6,
				2,
				2,
				2,
				2
			};
			byte[] array79 = new byte[24];
			array79[0] = 3;
			array79[16] = 1;
			Playbill = new Ffn(57, 86, 400, 0, 0, panose79, array79, new char[9]
			{
				'P',
				'l',
				'a',
				'y',
				'b',
				'i',
				'l',
				'l',
				'\0'
			});
			byte[] panose80 = new byte[10]
			{
				2,
				8,
				5,
				2,
				5,
				5,
				5,
				2,
				7,
				2
			};
			byte[] array80 = new byte[24];
			array80[0] = 3;
			array80[16] = 1;
			Poor_Richard = new Ffn(65, 22, 400, 0, 0, panose80, array80, new char[13]
			{
				'P',
				'o',
				'o',
				'r',
				' ',
				'R',
				'i',
				'c',
				'h',
				'a',
				'r',
				'd',
				'\0'
			});
			byte[] panose81 = new byte[10]
			{
				3,
				6,
				4,
				2,
				4,
				4,
				6,
				8,
				2,
				4
			};
			byte[] array81 = new byte[24];
			array81[0] = 3;
			array81[16] = 1;
			Pristina = new Ffn(57, 70, 400, 0, 0, panose81, array81, new char[9]
			{
				'P',
				'r',
				'i',
				's',
				't',
				'i',
				'n',
				'a',
				'\0'
			});
			Raavi = new Ffn(51, 6, 400, 0, 0, new byte[10]
			{
				2,
				0,
				5,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new byte[24]
			{
				3,
				0,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[6]
			{
				'R',
				'a',
				'a',
				'v',
				'i',
				'\0'
			});
			byte[] panose82 = new byte[10]
			{
				3,
				7,
				5,
				2,
				4,
				5,
				7,
				7,
				3,
				4
			};
			byte[] array82 = new byte[24];
			array82[0] = 3;
			array82[16] = 1;
			Rage_Italic = new Ffn(63, 70, 400, 0, 0, panose82, array82, new char[12]
			{
				'R',
				'a',
				'g',
				'e',
				' ',
				'I',
				't',
				'a',
				'l',
				'i',
				'c',
				'\0'
			});
			byte[] panose83 = new byte[10]
			{
				4,
				4,
				8,
				5,
				5,
				8,
				9,
				2,
				6,
				2
			};
			byte[] array83 = new byte[24];
			array83[0] = 3;
			array83[16] = 1;
			Ravie = new Ffn(51, 86, 400, 0, 0, panose83, array83, new char[6]
			{
				'R',
				'a',
				'v',
				'i',
				'e',
				'\0'
			});
			byte[] panose84 = new byte[10]
			{
				2,
				6,
				6,
				3,
				2,
				2,
				5,
				2,
				4,
				3
			};
			byte[] array84 = new byte[24];
			array84[0] = 3;
			array84[16] = 1;
			Rockwell = new Ffn(57, 22, 400, 0, 0, panose84, array84, new char[9]
			{
				'R',
				'o',
				'c',
				'k',
				'w',
				'e',
				'l',
				'l',
				'\0'
			});
			byte[] panose85 = new byte[10]
			{
				2,
				6,
				6,
				3,
				5,
				4,
				5,
				2,
				1,
				4
			};
			byte[] array85 = new byte[24];
			array85[0] = 3;
			array85[16] = 1;
			Rockwell_Condensed = new Ffn(77, 22, 400, 0, 0, panose85, array85, new char[19]
			{
				'R',
				'o',
				'c',
				'k',
				'w',
				'e',
				'l',
				'l',
				' ',
				'C',
				'o',
				'n',
				'd',
				'e',
				'n',
				's',
				'e',
				'd',
				'\0'
			});
			byte[] panose86 = new byte[10]
			{
				2,
				6,
				9,
				3,
				4,
				5,
				5,
				2,
				4,
				3
			};
			byte[] array86 = new byte[24];
			array86[0] = 3;
			array86[16] = 1;
			Rockwell_Extra_Bold = new Ffn(79, 22, 800, 0, 0, panose86, array86, new char[20]
			{
				'R',
				'o',
				'c',
				'k',
				'w',
				'e',
				'l',
				'l',
				' ',
				'E',
				'x',
				't',
				'r',
				'a',
				' ',
				'B',
				'o',
				'l',
				'd',
				'\0'
			});
			byte[] panose87 = new byte[10]
			{
				3,
				4,
				6,
				2,
				4,
				6,
				7,
				8,
				9,
				4
			};
			byte[] array87 = new byte[24];
			array87[0] = 3;
			array87[16] = 1;
			Script_MT_Bold = new Ffn(69, 70, 700, 0, 0, panose87, array87, new char[15]
			{
				'S',
				'c',
				'r',
				'i',
				'p',
				't',
				' ',
				'M',
				'T',
				' ',
				'B',
				'o',
				'l',
				'd',
				'\0'
			});
			byte[] panose88 = new byte[10]
			{
				4,
				2,
				9,
				4,
				2,
				1,
				2,
				2,
				6,
				4
			};
			byte[] array88 = new byte[24];
			array88[0] = 3;
			array88[16] = 1;
			Showcard_Gothic = new Ffn(71, 86, 400, 0, 0, panose88, array88, new char[16]
			{
				'S',
				'h',
				'o',
				'w',
				'c',
				'a',
				'r',
				'd',
				' ',
				'G',
				'o',
				't',
				'h',
				'i',
				'c',
				'\0'
			});
			Shruti = new Ffn(53, 6, 400, 0, 0, new byte[10]
			{
				2,
				0,
				5,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new byte[24]
			{
				3,
				0,
				4,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[7]
			{
				'S',
				'h',
				'r',
				'u',
				't',
				'i',
				'\0'
			});
			SimSun = new Ffn(59, 6, 400, 134, 7, new byte[10]
			{
				2,
				1,
				6,
				0,
				3,
				1,
				1,
				1,
				1,
				1
			}, new byte[24]
			{
				3,
				0,
				0,
				0,
				0,
				0,
				14,
				8,
				16,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				4,
				0,
				0,
				0,
				0,
				0
			}, new char[10]
			{
				'S',
				'i',
				'm',
				'S',
				'u',
				'n',
				'\0',
				'?',
				'?',
				'\0'
			});
			byte[] panose89 = new byte[10]
			{
				4,
				4,
				10,
				7,
				6,
				10,
				2,
				2,
				2,
				2
			};
			byte[] array89 = new byte[24];
			array89[0] = 3;
			array89[16] = 1;
			Snap_ITC = new Ffn(57, 86, 400, 0, 0, panose89, array89, new char[9]
			{
				'S',
				'n',
				'a',
				'p',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			byte[] panose90 = new byte[10]
			{
				4,
				4,
				9,
				5,
				13,
				8,
				2,
				2,
				4,
				4
			};
			byte[] array90 = new byte[24];
			array90[0] = 3;
			array90[16] = 1;
			Stencil = new Ffn(55, 86, 400, 0, 0, panose90, array90, new char[8]
			{
				'S',
				't',
				'e',
				'n',
				'c',
				'i',
				'l',
				'\0'
			});
			Sylfaen = new Ffn(55, 22, 400, 0, 0, new byte[10]
			{
				1,
				10,
				5,
				2,
				5,
				3,
				6,
				3,
				3,
				3
			}, new byte[24]
			{
				135,
				6,
				0,
				4,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'S',
				'y',
				'l',
				'f',
				'a',
				'e',
				'n',
				'\0'
			});
			Tahoma = new Ffn(53, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				4,
				3,
				5,
				4,
				4,
				2,
				4
			}, new byte[24]
			{
				135,
				122,
				0,
				97,
				0,
				0,
				0,
				128,
				8,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				255,
				1,
				1,
				0,
				0,
				0,
				0,
				0
			}, new char[7]
			{
				'T',
				'a',
				'h',
				'o',
				'm',
				'a',
				'\0'
			});
			byte[] panose91 = new byte[10]
			{
				4,
				2,
				4,
				4,
				3,
				13,
				7,
				2,
				2,
				2
			};
			byte[] array91 = new byte[24];
			array91[0] = 3;
			array91[16] = 1;
			Tempus_Sans_ITC = new Ffn(71, 86, 400, 0, 0, panose91, array91, new char[16]
			{
				'T',
				'e',
				'm',
				'p',
				'u',
				's',
				' ',
				'S',
				'a',
				'n',
				's',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			Trebuchet_MS = new Ffn(65, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				3,
				2,
				2,
				2,
				2,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[13]
			{
				'T',
				'r',
				'e',
				'b',
				'u',
				'c',
				'h',
				'e',
				't',
				' ',
				'M',
				'S',
				'\0'
			});
			Tunga = new Ffn(51, 6, 400, 0, 0, new byte[10]
			{
				0,
				0,
				4,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new byte[24]
			{
				3,
				0,
				64,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[6]
			{
				'T',
				'u',
				'n',
				'g',
				'a',
				'\0'
			});
			byte[] panose92 = new byte[10]
			{
				2,
				11,
				6,
				2,
				2,
				1,
				4,
				2,
				6,
				3
			};
			byte[] array92 = new byte[24];
			array92[0] = 7;
			array92[16] = 3;
			Tw_Cen_MT = new Ffn(59, 38, 400, 0, 0, panose92, array92, new char[10]
			{
				'T',
				'w',
				' ',
				'C',
				'e',
				'n',
				' ',
				'M',
				'T',
				'\0'
			});
			byte[] panose93 = new byte[10]
			{
				2,
				11,
				6,
				6,
				2,
				1,
				4,
				2,
				2,
				3
			};
			byte[] array93 = new byte[24];
			array93[0] = 7;
			array93[16] = 3;
			Tw_Cen_MT_Condensed = new Ffn(79, 38, 400, 0, 0, panose93, array93, new char[20]
			{
				'T',
				'w',
				' ',
				'C',
				'e',
				'n',
				' ',
				'M',
				'T',
				' ',
				'C',
				'o',
				'n',
				'd',
				'e',
				'n',
				's',
				'e',
				'd',
				'\0'
			});
			byte[] panose94 = new byte[10]
			{
				2,
				11,
				8,
				3,
				2,
				2,
				2,
				2,
				2,
				4
			};
			byte[] array94 = new byte[24];
			array94[0] = 7;
			array94[16] = 3;
			Tw_Cen_MT_Condensed_Extra_Bold = new Ffn(101, 38, 400, 0, 0, panose94, array94, new char[31]
			{
				'T',
				'w',
				' ',
				'C',
				'e',
				'n',
				' ',
				'M',
				'T',
				' ',
				'C',
				'o',
				'n',
				'd',
				'e',
				'n',
				's',
				'e',
				'd',
				' ',
				'E',
				'x',
				't',
				'r',
				'a',
				' ',
				'B',
				'o',
				'l',
				'd',
				'\0'
			});
			Verdana = new Ffn(55, 38, 400, 0, 0, new byte[10]
			{
				2,
				11,
				6,
				4,
				3,
				5,
				4,
				4,
				2,
				4
			}, new byte[24]
			{
				135,
				2,
				0,
				32,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				159,
				1,
				0,
				0,
				0,
				0,
				0,
				0
			}, new char[8]
			{
				'V',
				'e',
				'r',
				'd',
				'a',
				'n',
				'a',
				'\0'
			});
			byte[] panose95 = new byte[10]
			{
				3,
				7,
				5,
				2,
				3,
				5,
				2,
				2,
				2,
				3
			};
			byte[] array95 = new byte[24];
			array95[0] = 3;
			array95[16] = 1;
			Viner_Hand_ITC = new Ffn(69, 70, 400, 0, 0, panose95, array95, new char[15]
			{
				'V',
				'i',
				'n',
				'e',
				'r',
				' ',
				'H',
				'a',
				'n',
				'd',
				' ',
				'I',
				'T',
				'C',
				'\0'
			});
			byte[] panose96 = new byte[10]
			{
				3,
				2,
				6,
				2,
				5,
				5,
				6,
				9,
				8,
				4
			};
			byte[] array96 = new byte[24];
			array96[0] = 3;
			array96[16] = 1;
			Vivaldi = new Ffn(55, 70, 400, 0, 0, panose96, array96, new char[8]
			{
				'V',
				'i',
				'v',
				'a',
				'l',
				'd',
				'i',
				'\0'
			});
			byte[] panose97 = new byte[10]
			{
				3,
				5,
				4,
				2,
				4,
				4,
				7,
				7,
				3,
				5
			};
			byte[] array97 = new byte[24];
			array97[0] = 3;
			array97[16] = 1;
			Vladimir_Script = new Ffn(71, 70, 400, 0, 0, panose97, array97, new char[16]
			{
				'V',
				'l',
				'a',
				'd',
				'i',
				'm',
				'i',
				'r',
				' ',
				'S',
				'c',
				'r',
				'i',
				'p',
				't',
				'\0'
			});
			byte[] panose98 = new byte[10]
			{
				2,
				10,
				10,
				7,
				5,
				5,
				5,
				2,
				4,
				4
			};
			byte[] array98 = new byte[24];
			array98[0] = 3;
			array98[16] = 1;
			Wide_Latin = new Ffn(61, 22, 400, 0, 0, panose98, array98, new char[11]
			{
				'W',
				'i',
				'd',
				'e',
				' ',
				'L',
				'a',
				't',
				'i',
				'n',
				'\0'
			});
			m_fontMap = new Hashtable();
			BaseFontSize = 39;
			m_fontMap["Times New Roman"] = Times_New_Roman;
			m_fontMap["Symbol"] = Symbol;
			m_fontMap["Arial"] = Arial;
			m_fontMap["Agency FB"] = Agency_FB;
			m_fontMap["Algerian"] = Algerian;
			m_fontMap["Arial Black"] = Arial_Black;
			m_fontMap["Arial Narrow"] = Arial_Narrow;
			m_fontMap["Arial Rounded MT Bold"] = Arial_Rounded_MT_Bold;
			m_fontMap["Arial Unicode MS"] = Arial_Unicode_MS;
			m_fontMap["Baskerville Old Face"] = Baskerville_Old_Face;
			m_fontMap["Batang"] = Batang;
			m_fontMap["Bauhaus 93"] = Bauhaus_93;
			m_fontMap["Bell MT"] = Bell_MT;
			m_fontMap["Berlin Sans FB"] = Berlin_Sans_FB;
			m_fontMap["Berlin Sans FB Demi"] = Berlin_Sans_FB_Demi;
			m_fontMap["Bernard MT Condensed"] = Bernard_MT_Condensed;
			m_fontMap["Bitstream Vera Sans"] = Bitstream_Vera_Sans;
			m_fontMap["Bitstream Vera Sans Mono"] = Bitstream_Vera_Sans_Mono;
			m_fontMap["Bitstream Vera Serif"] = Bitstream_Vera_Serif;
			m_fontMap["Blackadder ITC"] = Blackadder_ITC;
			m_fontMap["Bodoni MT"] = Bodoni_MT;
			m_fontMap["Bodoni MT Black"] = Bodoni_MT_Black;
			m_fontMap["Bodoni MT Condensed"] = Bodoni_MT_Condensed;
			m_fontMap["Bodoni MT Poster Compressed"] = Bodoni_MT_Poster_Compressed;
			m_fontMap["Book Antiqua"] = Book_Antiqua;
			m_fontMap["Bookman Old Style"] = Bookman_Old_Style;
			m_fontMap["Bradley Hand ITC"] = Bradley_Hand_ITC;
			m_fontMap["Britannic Bold"] = Britannic_Bold;
			m_fontMap["Broadway"] = Broadway;
			m_fontMap["Brush Script MT"] = Brush_Script_MT;
			m_fontMap["Californian FB"] = Californian_FB;
			m_fontMap["Calisto MT"] = Calisto_MT;
			m_fontMap["Castellar"] = Castellar;
			m_fontMap["Centaur"] = Centaur;
			m_fontMap["Century"] = Century;
			m_fontMap["Century Gothic"] = Century_Gothic;
			m_fontMap["Century Schoolbook"] = Century_Schoolbook;
			m_fontMap["Chiller"] = Chiller;
			m_fontMap["Colonna MT"] = Colonna_MT;
			m_fontMap["Comic Sans MS"] = Comic_Sans_MS;
			m_fontMap["Cooper Black"] = Cooper_Black;
			m_fontMap["Copperplate Gothic Bold"] = Copperplate_Gothic_Bold;
			m_fontMap["Copperplate Gothic Light"] = Copperplate_Gothic_Light;
			m_fontMap["Courier New"] = Courier_New;
			m_fontMap["Curlz MT"] = Curlz_MT;
			m_fontMap["Edwardian Script ITC"] = Edwardian_Script_ITC;
			m_fontMap["Elephant"] = Elephant;
			m_fontMap["Engravers MT"] = Engravers_MT;
			m_fontMap["Eras Bold ITC"] = Eras_Bold_ITC;
			m_fontMap["Eras Demi ITC"] = Eras_Demi_ITC;
			m_fontMap["Eras Light ITC"] = Eras_Light_ITC;
			m_fontMap["Eras Medium ITC"] = Eras_Medium_ITC;
			m_fontMap["Estrangelo Edessa"] = Estrangelo_Edessa;
			m_fontMap["Felix Titling"] = Felix_Titling;
			m_fontMap["Footlight MT Light"] = Footlight_MT_Light;
			m_fontMap["Forte"] = Forte;
			m_fontMap["Franklin Gothic Book"] = Franklin_Gothic_Book;
			m_fontMap["Franklin Gothic Demi"] = Franklin_Gothic_Demi;
			m_fontMap["Franklin Gothic Demi Cond"] = Franklin_Gothic_Demi_Cond;
			m_fontMap["Franklin Gothic Heavy"] = Franklin_Gothic_Heavy;
			m_fontMap["Franklin Gothic Medium"] = Franklin_Gothic_Medium;
			m_fontMap["Franklin Gothic Medium Cond"] = Franklin_Gothic_Medium_Cond;
			m_fontMap["Freestyle Script"] = Freestyle_Script;
			m_fontMap["French Script MT"] = French_Script_MT;
			m_fontMap["Garamond"] = Garamond;
			m_fontMap["Gautami"] = Gautami;
			m_fontMap["Georgia"] = Georgia;
			m_fontMap["Gigi"] = Gigi;
			m_fontMap["Gill Sans MT"] = Gill_Sans_MT;
			m_fontMap["Gill Sans MT Condensed"] = Gill_Sans_MT_Condensed;
			m_fontMap["Gill Sans MT Ext Condensed Bold"] = Gill_Sans_MT_Ext_Condensed_Bold;
			m_fontMap["Gill Sans Ultra Bold"] = Gill_Sans_Ultra_Bold;
			m_fontMap["Gill Sans Ultra Bold Condensed"] = Gill_Sans_Ultra_Bold_Condensed;
			m_fontMap["Gloucester MT Extra Condensed"] = Gloucester_MT_Extra_Condensed;
			m_fontMap["Goudy Stout"] = Goudy_Stout;
			m_fontMap["Haettenschweiler"] = Haettenschweiler;
			m_fontMap["Harlow Solid Italic"] = Harlow_Solid_Italic;
			m_fontMap["Harrington"] = Harrington;
			m_fontMap["High Tower Text"] = High_Tower_Text;
			m_fontMap["Impact"] = Impact;
			m_fontMap["Imprint MT Shadow"] = Imprint_MT_Shadow;
			m_fontMap["Informal Roman"] = Informal_Roman;
			m_fontMap["Jokerman"] = Jokerman;
			m_fontMap["Juice ITC"] = Juice_ITC;
			m_fontMap["Kristen ITC"] = Kristen_ITC;
			m_fontMap["Kunstler Script"] = Kunstler_Script;
			m_fontMap["Latha"] = Latha;
			m_fontMap["Lucida Bright"] = Lucida_Bright;
			m_fontMap["Lucida Calligraphy"] = Lucida_Calligraphy;
			m_fontMap["Lucida Console"] = Lucida_Console;
			m_fontMap["Lucida Fax"] = Lucida_Fax;
			m_fontMap["Onyx"] = Onyx;
			m_fontMap["Lucida Handwriting"] = Lucida_Handwriting;
			m_fontMap["Lucida Sans"] = Lucida_Sans;
			m_fontMap["Lucida Sans Typewriter"] = Lucida_Sans_Typewriter;
			m_fontMap["Lucida Sans Unicode"] = Lucida_Sans_Unicode;
			m_fontMap["Magneto"] = Magneto;
			m_fontMap["Maiandra GD"] = Maiandra_GD;
			m_fontMap["Mangal"] = Mangal;
			m_fontMap["Matura MT Script Capitals"] = Matura_MT_Script_Capitals;
			m_fontMap["Microsoft Sans Serif"] = Microsoft_Sans_Serif;
			m_fontMap["Mistral"] = Mistral;
			m_fontMap["Modern No. 20"] = Modern_No__20;
			m_fontMap["Monotype Corsiva"] = Monotype_Corsiva;
			m_fontMap["MS Mincho"] = MS_Mincho;
			m_fontMap["MS Reference Sans Serif"] = MS_Reference_Sans_Serif;
			m_fontMap["MV Boli"] = MV_Boli;
			m_fontMap["Niagara Engraved"] = Niagara_Engraved;
			m_fontMap["Niagara Solid"] = Niagara_Solid;
			m_fontMap["OCR A Extended"] = OCR_A_Extended;
			m_fontMap["Old English Text MT"] = Old_English_Text_MT;
			m_fontMap["Palace Script MT"] = Palace_Script_MT;
			m_fontMap["Palatino Linotype"] = Palatino_Linotype;
			m_fontMap["Papyrus"] = Papyrus;
			m_fontMap["Parchment"] = Parchment;
			m_fontMap["Perpetua"] = Perpetua;
			m_fontMap["Perpetua Titling MT"] = Perpetua_Titling_MT;
			m_fontMap["Playbill"] = Playbill;
			m_fontMap["Poor Richard"] = Poor_Richard;
			m_fontMap["Pristina"] = Pristina;
			m_fontMap["Raavi"] = Raavi;
			m_fontMap["Rage Italic"] = Rage_Italic;
			m_fontMap["Ravie"] = Ravie;
			m_fontMap["Rockwell"] = Rockwell;
			m_fontMap["Rockwell Condensed"] = Rockwell_Condensed;
			m_fontMap["Rockwell Extra Bold"] = Rockwell_Extra_Bold;
			m_fontMap["Script MT Bold"] = Script_MT_Bold;
			m_fontMap["Showcard Gothic"] = Showcard_Gothic;
			m_fontMap["Shruti"] = Shruti;
			m_fontMap["SimSun"] = SimSun;
			m_fontMap["Snap ITC"] = Snap_ITC;
			m_fontMap["Stencil"] = Stencil;
			m_fontMap["Sylfaen"] = Sylfaen;
			m_fontMap["Tahoma"] = Tahoma;
			m_fontMap["Tempus Sans ITC"] = Tempus_Sans_ITC;
			m_fontMap["Trebuchet MS"] = Trebuchet_MS;
			m_fontMap["Tunga"] = Tunga;
			m_fontMap["Tw Cen MT"] = Tw_Cen_MT;
			m_fontMap["Tw Cen MT Condensed"] = Tw_Cen_MT_Condensed;
			m_fontMap["Tw Cen MT Condensed Extra Bold"] = Tw_Cen_MT_Condensed_Extra_Bold;
			m_fontMap["Verdana"] = Verdana;
			m_fontMap["Viner Hand ITC"] = Viner_Hand_ITC;
			m_fontMap["Vivaldi"] = Vivaldi;
			m_fontMap["Vladimir Script"] = Vladimir_Script;
			m_fontMap["Wide Latin"] = Wide_Latin;
		}
	}
}
