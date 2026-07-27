using System.Collections;

namespace NBidi
{
	public abstract class UnicodeArabicShapingResolver
	{
		private static Hashtable charForms;

		public static ArabicShapeJoiningType GetArabicShapeJoiningType(char c)
		{
			if (c >= '\u0600' && c <= '\u0603')
			{
				return ArabicShapeJoiningType.U;
			}
			if (c == '؈')
			{
				return ArabicShapeJoiningType.U;
			}
			if (c == '؋')
			{
				return ArabicShapeJoiningType.U;
			}
			if (c == 'ء')
			{
				return ArabicShapeJoiningType.U;
			}
			if (c >= 'آ' && c <= 'إ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c == 'ئ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c == 'ا')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c == 'ب')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c == 'ة')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c >= 'ت' && c <= 'خ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c >= 'د' && c <= 'ز')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c >= 'س' && c <= 'ؿ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c == 'ـ')
			{
				return ArabicShapeJoiningType.C;
			}
			if (c >= 'ف' && c <= 'ه')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c == 'و')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c >= 'ى' && c <= 'ي')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c >= 'ٮ' && c <= 'ٯ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c >= 'ٱ' && c <= 'ٳ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c == 'ٴ')
			{
				return ArabicShapeJoiningType.U;
			}
			if (c >= 'ٵ' && c <= 'ٷ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c >= 'ٸ' && c <= 'ڇ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c >= 'ڈ' && c <= 'ڙ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c >= 'ښ' && c <= 'ڿ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c == 'ۀ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c >= 'ہ' && c <= 'ۂ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c >= 'ۃ' && c <= 'ۋ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c == 'ی')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c == 'ۍ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c == 'ێ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c == 'ۏ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c >= 'ې' && c <= 'ۑ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c >= 'ے' && c <= 'ۓ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c == 'ە')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c == '\u06dd')
			{
				return ArabicShapeJoiningType.U;
			}
			if (c >= 'ۮ' && c <= 'ۯ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c >= 'ۺ' && c <= 'ۼ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c == 'ۿ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c == 'ܐ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c >= 'ܒ' && c <= 'ܔ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c >= 'ܕ' && c <= 'ܙ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c >= 'ܚ' && c <= 'ܝ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c == 'ܞ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c >= 'ܟ' && c <= 'ܧ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c == 'ܨ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c == 'ܩ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c == 'ܪ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c == 'ܫ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c == 'ܬ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c >= 'ܭ' && c <= 'ܮ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c == 'ܯ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c == 'ݍ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c >= 'ݎ' && c <= 'ݘ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c >= 'ݙ' && c <= 'ݛ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c >= 'ݜ' && c <= 'ݪ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c >= 'ݫ' && c <= 'ݬ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c >= 'ݭ' && c <= 'ݰ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c == 'ݱ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c == 'ݲ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c >= 'ݳ' && c <= 'ݴ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c >= 'ݵ' && c <= 'ݷ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c >= 'ݸ' && c <= 'ݹ')
			{
				return ArabicShapeJoiningType.R;
			}
			if (c >= 'ݺ' && c <= 'ݿ')
			{
				return ArabicShapeJoiningType.D;
			}
			if (c >= 'ߊ' && c <= 'ߪ')
			{
				return ArabicShapeJoiningType.D;
			}
			switch (c)
			{
			case 'ߺ':
				return ArabicShapeJoiningType.C;
			case '\u200d':
				return ArabicShapeJoiningType.C;
			default:
			{
				UnicodeGeneralCategory unicodeGeneralCategory = UnicodeCharacterDataResolver.GetUnicodeGeneralCategory(c);
				if (unicodeGeneralCategory == UnicodeGeneralCategory.Mn || unicodeGeneralCategory == UnicodeGeneralCategory.Me || unicodeGeneralCategory == UnicodeGeneralCategory.Cf)
				{
					return ArabicShapeJoiningType.T;
				}
				return ArabicShapeJoiningType.U;
			}
			}
		}

		public static char GetArabicCharacterByLetterForm(char ch, LetterForm form)
		{
			int num = ch | ((int)form << 16);
			if (charForms.ContainsKey(num))
			{
				return (char)charForms[num];
			}
			return ch;
		}

		static UnicodeArabicShapingResolver()
		{
			charForms = new Hashtable();
			charForms[198257] = 'ﭐ';
			charForms[132721] = 'ﭑ';
			charForms[198267] = 'ﭒ';
			charForms[132731] = 'ﭓ';
			charForms[1659] = 'ﭔ';
			charForms[67195] = 'ﭕ';
			charForms[198270] = 'ﭖ';
			charForms[132734] = 'ﭗ';
			charForms[1662] = 'ﭘ';
			charForms[67198] = 'ﭙ';
			charForms[198272] = 'ﭚ';
			charForms[132736] = 'ﭛ';
			charForms[1664] = 'ﭜ';
			charForms[67200] = 'ﭝ';
			charForms[198266] = 'ﭞ';
			charForms[132730] = 'ﭟ';
			charForms[1658] = 'ﭠ';
			charForms[67194] = 'ﭡ';
			charForms[198271] = 'ﭢ';
			charForms[132735] = 'ﭣ';
			charForms[1663] = 'ﭤ';
			charForms[67199] = 'ﭥ';
			charForms[198265] = 'ﭦ';
			charForms[132729] = 'ﭧ';
			charForms[1657] = 'ﭨ';
			charForms[67193] = 'ﭩ';
			charForms[198308] = 'ﭪ';
			charForms[132772] = 'ﭫ';
			charForms[1700] = 'ﭬ';
			charForms[67236] = 'ﭭ';
			charForms[198310] = 'ﭮ';
			charForms[132774] = 'ﭯ';
			charForms[1702] = 'ﭰ';
			charForms[67238] = 'ﭱ';
			charForms[198276] = 'ﭲ';
			charForms[132740] = 'ﭳ';
			charForms[1668] = 'ﭴ';
			charForms[67204] = 'ﭵ';
			charForms[198275] = 'ﭶ';
			charForms[132739] = 'ﭷ';
			charForms[1667] = 'ﭸ';
			charForms[67203] = 'ﭹ';
			charForms[198278] = 'ﭺ';
			charForms[132742] = 'ﭻ';
			charForms[1670] = 'ﭼ';
			charForms[67206] = 'ﭽ';
			charForms[198279] = 'ﭾ';
			charForms[132743] = 'ﭿ';
			charForms[1671] = 'ﮀ';
			charForms[67207] = 'ﮁ';
			charForms[198285] = 'ﮂ';
			charForms[132749] = 'ﮃ';
			charForms[198284] = 'ﮄ';
			charForms[132748] = 'ﮅ';
			charForms[198286] = 'ﮆ';
			charForms[132750] = 'ﮇ';
			charForms[198280] = 'ﮈ';
			charForms[132744] = 'ﮉ';
			charForms[198296] = 'ﮊ';
			charForms[132760] = 'ﮋ';
			charForms[198289] = 'ﮌ';
			charForms[132753] = 'ﮍ';
			charForms[198313] = 'ﮎ';
			charForms[132777] = 'ﮏ';
			charForms[1705] = 'ﮐ';
			charForms[67241] = 'ﮑ';
			charForms[198319] = 'ﮒ';
			charForms[132783] = 'ﮓ';
			charForms[1711] = 'ﮔ';
			charForms[67247] = 'ﮕ';
			charForms[198323] = 'ﮖ';
			charForms[132787] = 'ﮗ';
			charForms[1715] = 'ﮘ';
			charForms[67251] = 'ﮙ';
			charForms[198321] = 'ﮚ';
			charForms[132785] = 'ﮛ';
			charForms[1713] = 'ﮜ';
			charForms[67249] = 'ﮝ';
			charForms[198330] = 'ﮞ';
			charForms[132794] = 'ﮟ';
			charForms[198331] = 'ﮠ';
			charForms[132795] = 'ﮡ';
			charForms[1723] = 'ﮢ';
			charForms[67259] = 'ﮣ';
			charForms[198336] = 'ﮤ';
			charForms[132800] = 'ﮥ';
			charForms[198337] = 'ﮦ';
			charForms[132801] = 'ﮧ';
			charForms[1729] = 'ﮨ';
			charForms[67265] = 'ﮩ';
			charForms[198334] = 'ﮪ';
			charForms[132798] = 'ﮫ';
			charForms[1726] = 'ﮬ';
			charForms[67262] = 'ﮭ';
			charForms[198354] = 'ﮮ';
			charForms[132818] = 'ﮯ';
			charForms[198355] = 'ﮰ';
			charForms[132819] = 'ﮱ';
			charForms[198317] = 'ﯓ';
			charForms[132781] = 'ﯔ';
			charForms[1709] = 'ﯕ';
			charForms[67245] = 'ﯖ';
			charForms[198343] = 'ﯗ';
			charForms[132807] = 'ﯘ';
			charForms[198342] = 'ﯙ';
			charForms[132806] = 'ﯚ';
			charForms[198344] = 'ﯛ';
			charForms[132808] = 'ﯜ';
			charForms[198263] = 'ﯝ';
			charForms[198347] = 'ﯞ';
			charForms[132811] = 'ﯟ';
			charForms[198341] = 'ﯠ';
			charForms[132805] = 'ﯡ';
			charForms[198345] = 'ﯢ';
			charForms[132809] = 'ﯣ';
			charForms[198352] = 'ﯤ';
			charForms[132816] = 'ﯥ';
			charForms[1744] = 'ﯦ';
			charForms[67280] = 'ﯧ';
			charForms[1609] = 'ﯨ';
			charForms[67145] = 'ﯩ';
			charForms[198348] = 'ﯼ';
			charForms[132812] = 'ﯽ';
			charForms[1740] = 'ﯾ';
			charForms[67276] = 'ﯿ';
			charForms[198177] = 'ﺀ';
			charForms[198178] = 'ﺁ';
			charForms[132642] = 'ﺂ';
			charForms[198179] = 'ﺃ';
			charForms[132643] = 'ﺄ';
			charForms[198180] = 'ﺅ';
			charForms[132644] = 'ﺆ';
			charForms[198181] = 'ﺇ';
			charForms[132645] = 'ﺈ';
			charForms[198182] = 'ﺉ';
			charForms[132646] = 'ﺊ';
			charForms[1574] = 'ﺋ';
			charForms[67110] = 'ﺌ';
			charForms[198183] = 'ﺍ';
			charForms[132647] = 'ﺎ';
			charForms[198184] = 'ﺏ';
			charForms[132648] = 'ﺐ';
			charForms[1576] = 'ﺑ';
			charForms[67112] = 'ﺒ';
			charForms[198185] = 'ﺓ';
			charForms[132649] = 'ﺔ';
			charForms[198186] = 'ﺕ';
			charForms[132650] = 'ﺖ';
			charForms[1578] = 'ﺗ';
			charForms[67114] = 'ﺘ';
			charForms[198187] = 'ﺙ';
			charForms[132651] = 'ﺚ';
			charForms[1579] = 'ﺛ';
			charForms[67115] = 'ﺜ';
			charForms[198188] = 'ﺝ';
			charForms[132652] = 'ﺞ';
			charForms[1580] = 'ﺟ';
			charForms[67116] = 'ﺠ';
			charForms[198189] = 'ﺡ';
			charForms[132653] = 'ﺢ';
			charForms[1581] = 'ﺣ';
			charForms[67117] = 'ﺤ';
			charForms[198190] = 'ﺥ';
			charForms[132654] = 'ﺦ';
			charForms[1582] = 'ﺧ';
			charForms[67118] = 'ﺨ';
			charForms[198191] = 'ﺩ';
			charForms[132655] = 'ﺪ';
			charForms[198192] = 'ﺫ';
			charForms[132656] = 'ﺬ';
			charForms[198193] = 'ﺭ';
			charForms[132657] = 'ﺮ';
			charForms[198194] = 'ﺯ';
			charForms[132658] = 'ﺰ';
			charForms[198195] = 'ﺱ';
			charForms[132659] = 'ﺲ';
			charForms[1587] = 'ﺳ';
			charForms[67123] = 'ﺴ';
			charForms[198196] = 'ﺵ';
			charForms[132660] = 'ﺶ';
			charForms[1588] = 'ﺷ';
			charForms[67124] = 'ﺸ';
			charForms[198197] = 'ﺹ';
			charForms[132661] = 'ﺺ';
			charForms[1589] = 'ﺻ';
			charForms[67125] = 'ﺼ';
			charForms[198198] = 'ﺽ';
			charForms[132662] = 'ﺾ';
			charForms[1590] = 'ﺿ';
			charForms[67126] = 'ﻀ';
			charForms[198199] = 'ﻁ';
			charForms[132663] = 'ﻂ';
			charForms[1591] = 'ﻃ';
			charForms[67127] = 'ﻄ';
			charForms[198200] = 'ﻅ';
			charForms[132664] = 'ﻆ';
			charForms[1592] = 'ﻇ';
			charForms[67128] = 'ﻈ';
			charForms[198201] = 'ﻉ';
			charForms[132665] = 'ﻊ';
			charForms[1593] = 'ﻋ';
			charForms[67129] = 'ﻌ';
			charForms[198202] = 'ﻍ';
			charForms[132666] = 'ﻎ';
			charForms[1594] = 'ﻏ';
			charForms[67130] = 'ﻐ';
			charForms[198209] = 'ﻑ';
			charForms[132673] = 'ﻒ';
			charForms[1601] = 'ﻓ';
			charForms[67137] = 'ﻔ';
			charForms[198210] = 'ﻕ';
			charForms[132674] = 'ﻖ';
			charForms[1602] = 'ﻗ';
			charForms[67138] = 'ﻘ';
			charForms[198211] = 'ﻙ';
			charForms[132675] = 'ﻚ';
			charForms[1603] = 'ﻛ';
			charForms[67139] = 'ﻜ';
			charForms[198212] = 'ﻝ';
			charForms[132676] = 'ﻞ';
			charForms[1604] = 'ﻟ';
			charForms[67140] = 'ﻠ';
			charForms[198213] = 'ﻡ';
			charForms[132677] = 'ﻢ';
			charForms[1605] = 'ﻣ';
			charForms[67141] = 'ﻤ';
			charForms[198214] = 'ﻥ';
			charForms[132678] = 'ﻦ';
			charForms[1606] = 'ﻧ';
			charForms[67142] = 'ﻨ';
			charForms[198215] = 'ﻩ';
			charForms[132679] = 'ﻪ';
			charForms[1607] = 'ﻫ';
			charForms[67143] = 'ﻬ';
			charForms[198216] = 'ﻭ';
			charForms[132680] = 'ﻮ';
			charForms[198217] = 'ﻯ';
			charForms[132681] = 'ﻰ';
			charForms[198218] = 'ﻱ';
			charForms[132682] = 'ﻲ';
			charForms[1610] = 'ﻳ';
			charForms[67146] = 'ﻴ';
		}
	}
}
