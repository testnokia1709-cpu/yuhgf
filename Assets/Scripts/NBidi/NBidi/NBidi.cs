using System;
using System.Collections;
using System.Text;

namespace NBidi
{
	public static class NBidi
	{
		private struct CharData
		{
			public char _char;

			public byte _el;

			public BidiCharacterType _ct;

			public int _idx;
		}

		private class Paragraph
		{
			private string _original_text;

			private string _text;

			private string _bidi_text;

			private char _paragraph_separator = '\uffff';

			private byte embedding_level;

			private CharData[] _text_data;

			private int[] _char_lengths;

			private int[] _bidi_indexes;

			private bool _hasArabic;

			private bool _hasNSMs;

			public string Text
			{
				get
				{
					return _original_text;
				}
				set
				{
					_original_text = value;
					_text = value;
					NormalizeText();
					RecalculateParagraphEmbeddingLevel();
					RecalculateCharactersEmbeddingLevels();
					RemoveBidiMarkers();
				}
			}

			public char ParagraphSeparator
			{
				get
				{
					return _paragraph_separator;
				}
				internal set
				{
					_paragraph_separator = value;
				}
			}

			public string BidiText
			{
				get
				{
					string text = _bidi_text;
					if (_paragraph_separator != '\uffff')
					{
						text += _paragraph_separator;
					}
					return text;
				}
			}

			public int[] BidiIndexes
			{
				get
				{
					return _bidi_indexes;
				}
			}

			public int[] BidiIndexLengths
			{
				get
				{
					return _char_lengths;
				}
			}

			public byte EmbeddingLevel
			{
				get
				{
					return embedding_level;
				}
				set
				{
					embedding_level = value;
				}
			}

			public Paragraph(string text)
			{
				Text = text;
			}

			private void RemoveBidiMarkers()
			{
				string text = "\u200f\u202b\u202e\u200e\u202a\u202d\u202c";
				StringBuilder stringBuilder = new StringBuilder(_bidi_text);
				ArrayList arrayList = new ArrayList(_bidi_indexes);
				ArrayList arrayList2 = new ArrayList(_char_lengths);
				int num = 0;
				while (num < stringBuilder.Length)
				{
					if (text.Contains(stringBuilder[num].ToString()))
					{
						stringBuilder.Remove(num, 1);
						arrayList.RemoveAt(num);
						arrayList2.RemoveAt(num);
					}
					else
					{
						num++;
					}
				}
				_bidi_text = stringBuilder.ToString();
				_bidi_indexes = (int[])arrayList.ToArray(typeof(int));
				_char_lengths = (int[])arrayList2.ToArray(typeof(int));
			}

			public void RecalculateParagraphEmbeddingLevel()
			{
				string text = _text;
				foreach (char c in text)
				{
					BidiCharacterType bidiCharacterType = UnicodeCharacterDataResolver.GetBidiCharacterType(c);
					if (bidiCharacterType == BidiCharacterType.R || bidiCharacterType == BidiCharacterType.AL)
					{
						embedding_level = 1;
						break;
					}
					if (bidiCharacterType == BidiCharacterType.L)
					{
						break;
					}
				}
			}

			public void NormalizeText()
			{
				ArrayList arrayList = new ArrayList();
				StringBuilder stringBuilder = InternalDecompose(arrayList);
				InternalCompose(stringBuilder, arrayList);
				_char_lengths = (int[])arrayList.ToArray(typeof(int));
				_text = stringBuilder.ToString();
			}

			public void RecalculateCharactersEmbeddingLevels()
			{
				if (_hasArabic)
				{
					_text = PerformArabicShaping(_text);
				}
				_text_data = new CharData[_text.Length];
				byte b = EmbeddingLevel;
				DirectionalOverrideStatus directionalOverrideStatus = DirectionalOverrideStatus.Neutral;
				Stack stack = new Stack();
				Stack stack2 = new Stack();
				int num = 0;
				for (int i = 0; i < _text.Length; i++)
				{
					bool flag = false;
					char c = _text[i];
					_text_data[i]._ct = UnicodeCharacterDataResolver.GetBidiCharacterType(c);
					_text_data[i]._char = c;
					_text_data[i]._idx = num;
					num += _char_lengths[i];
					if (c == '\u202b' || c == '\u202e')
					{
						flag = true;
						if (b < 60)
						{
							stack2.Push(b);
							stack.Push(directionalOverrideStatus);
							b++;
							b |= 1;
							directionalOverrideStatus = ((c != '\u202b') ? DirectionalOverrideStatus.RTL : DirectionalOverrideStatus.Neutral);
						}
					}
					else if (c == '\u202a' || c == '\u202d')
					{
						flag = true;
						if (b < 59)
						{
							stack2.Push(b);
							stack.Push(directionalOverrideStatus);
							b |= 1;
							b++;
							directionalOverrideStatus = ((c != '\u202a') ? DirectionalOverrideStatus.LTR : DirectionalOverrideStatus.Neutral);
						}
					}
					else if (c != '\u202c')
					{
						_text_data[i]._el = b;
						switch (directionalOverrideStatus)
						{
						case DirectionalOverrideStatus.LTR:
							_text_data[i]._ct = BidiCharacterType.L;
							break;
						case DirectionalOverrideStatus.RTL:
							_text_data[i]._ct = BidiCharacterType.R;
							break;
						}
					}
					else if (c == '\u202c')
					{
						flag = true;
						if (stack2.Count > 0)
						{
							b = (byte)stack2.Pop();
							directionalOverrideStatus = (DirectionalOverrideStatus)stack.Pop();
						}
					}
					if (flag || _text_data[i]._ct == BidiCharacterType.BN)
					{
						_text_data[i]._el = b;
					}
				}
				int val = EmbeddingLevel;
				int num2 = 0;
				while (num2 < _text.Length)
				{
					byte el = _text_data[num2]._el;
					BidiCharacterType sor = TypeForLevel(Math.Max(val, el));
					int j;
					for (j = num2 + 1; j < _text.Length && _text_data[j]._el == el; j++)
					{
					}
					byte val2 = ((j < _text.Length) ? _text_data[j]._el : EmbeddingLevel);
					BidiCharacterType eor = TypeForLevel(Math.Max(val2, el));
					ResolveWeakTypes(num2, j, sor, eor);
					ResolveNeutralTypes(num2, j, sor, eor, el);
					ResolveImplicitTypes(num2, j, el);
					val = el;
					num2 = j;
				}
				ReorderString();
				FixMirroredCharacters();
				ArrayList arrayList = new ArrayList();
				ArrayList arrayList2 = new ArrayList();
				StringBuilder stringBuilder = new StringBuilder();
				CharData[] text_data = _text_data;
				for (int k = 0; k < text_data.Length; k++)
				{
					CharData charData = text_data[k];
					stringBuilder.Append(charData._char);
					arrayList.Add(charData._idx);
					arrayList2.Add(1);
				}
				_bidi_text = stringBuilder.ToString();
				_bidi_indexes = (int[])arrayList.ToArray(typeof(int));
			}

			private void ResolveWeakTypes(int start, int limit, BidiCharacterType sor, BidiCharacterType eor)
			{
				if (_hasNSMs)
				{
					BidiCharacterType ct = sor;
					for (int i = start; i < limit; i++)
					{
						BidiCharacterType ct2 = _text_data[i]._ct;
						if (ct2 == BidiCharacterType.NSM)
						{
							_text_data[i]._ct = ct;
						}
						else
						{
							ct = ct2;
						}
					}
				}
				BidiCharacterType ct3 = BidiCharacterType.EN;
				for (int j = start; j < limit; j++)
				{
					if (_text_data[j]._ct == BidiCharacterType.L || _text_data[j]._ct == BidiCharacterType.R)
					{
						ct3 = BidiCharacterType.EN;
					}
					else if (_text_data[j]._ct == BidiCharacterType.AL)
					{
						ct3 = BidiCharacterType.AN;
					}
					else if (_text_data[j]._ct == BidiCharacterType.EN)
					{
						_text_data[j]._ct = ct3;
					}
				}
				if (_hasArabic)
				{
					for (int k = start; k < limit; k++)
					{
						if (_text_data[k]._ct == BidiCharacterType.AL)
						{
							_text_data[k]._ct = BidiCharacterType.R;
						}
					}
				}
				for (int l = start + 1; l < limit - 1; l++)
				{
					if (_text_data[l]._ct == BidiCharacterType.ES || _text_data[l]._ct == BidiCharacterType.CS)
					{
						BidiCharacterType ct4 = _text_data[l - 1]._ct;
						BidiCharacterType ct5 = _text_data[l + 1]._ct;
						if (ct4 == BidiCharacterType.EN && ct5 == BidiCharacterType.EN)
						{
							_text_data[l]._ct = BidiCharacterType.EN;
						}
						else if (_text_data[l]._ct == BidiCharacterType.CS && ct4 == BidiCharacterType.AN && ct5 == BidiCharacterType.AN)
						{
							_text_data[l]._ct = BidiCharacterType.AN;
						}
					}
				}
				for (int m = start; m < limit; m++)
				{
					if (_text_data[m]._ct == BidiCharacterType.ET)
					{
						int num = m;
						int num2 = FindRunLimit(num, limit, new BidiCharacterType[1] { BidiCharacterType.ET });
						BidiCharacterType bidiCharacterType = ((num == start) ? sor : _text_data[num - 1]._ct);
						if (bidiCharacterType != BidiCharacterType.EN)
						{
							bidiCharacterType = ((num2 == limit) ? eor : _text_data[num2]._ct);
						}
						if (bidiCharacterType == BidiCharacterType.EN)
						{
							SetTypes(num, num2, BidiCharacterType.EN);
						}
						m = num2;
					}
				}
				for (int n = start; n < limit; n++)
				{
					BidiCharacterType ct6 = _text_data[n]._ct;
					if (ct6 == BidiCharacterType.ES || ct6 == BidiCharacterType.ET || ct6 == BidiCharacterType.CS)
					{
						_text_data[n]._ct = BidiCharacterType.ON;
					}
				}
				BidiCharacterType ct7 = ((sor != BidiCharacterType.L) ? BidiCharacterType.EN : BidiCharacterType.L);
				for (int num3 = start; num3 < limit; num3++)
				{
					if (_text_data[num3]._ct == BidiCharacterType.R)
					{
						ct7 = BidiCharacterType.EN;
					}
					else if (_text_data[num3]._ct == BidiCharacterType.L)
					{
						ct7 = BidiCharacterType.L;
					}
					else if (_text_data[num3]._ct == BidiCharacterType.EN)
					{
						_text_data[num3]._ct = ct7;
					}
				}
			}

			private void ResolveNeutralTypes(int start, int limit, BidiCharacterType sor, BidiCharacterType eor, int level)
			{
				for (int i = start; i < limit; i++)
				{
					BidiCharacterType ct = _text_data[i]._ct;
					if (ct != BidiCharacterType.WS && ct != BidiCharacterType.ON && ct != BidiCharacterType.B && ct != BidiCharacterType.S)
					{
						continue;
					}
					int num = i;
					int num2 = FindRunLimit(num, limit, new BidiCharacterType[4]
					{
						BidiCharacterType.B,
						BidiCharacterType.S,
						BidiCharacterType.WS,
						BidiCharacterType.ON
					});
					BidiCharacterType bidiCharacterType;
					if (num == start)
					{
						bidiCharacterType = sor;
					}
					else
					{
						bidiCharacterType = _text_data[num - 1]._ct;
						if (bidiCharacterType == BidiCharacterType.AN || bidiCharacterType == BidiCharacterType.EN)
						{
							bidiCharacterType = BidiCharacterType.R;
						}
					}
					BidiCharacterType bidiCharacterType2;
					if (num2 == limit)
					{
						bidiCharacterType2 = eor;
					}
					else
					{
						bidiCharacterType2 = _text_data[num2]._ct;
						if (bidiCharacterType2 == BidiCharacterType.AN || bidiCharacterType2 == BidiCharacterType.EN)
						{
							bidiCharacterType2 = BidiCharacterType.R;
						}
					}
					BidiCharacterType newType = ((bidiCharacterType != bidiCharacterType2) ? TypeForLevel(level) : bidiCharacterType);
					SetTypes(num, num2, newType);
					i = num2;
				}
			}

			private void ResolveImplicitTypes(int start, int limit, int level)
			{
				if ((level & 1) == 0)
				{
					for (int i = start; i < limit; i++)
					{
						BidiCharacterType ct = _text_data[i]._ct;
						switch (ct)
						{
						case BidiCharacterType.R:
							_text_data[i]._el++;
							continue;
						default:
							if (ct != BidiCharacterType.EN)
							{
								continue;
							}
							break;
						case BidiCharacterType.AN:
							break;
						}
						_text_data[i]._el += 2;
					}
					return;
				}
				for (int j = start; j < limit; j++)
				{
					BidiCharacterType ct2 = _text_data[j]._ct;
					if (ct2 == BidiCharacterType.L || ct2 == BidiCharacterType.AN || ct2 == BidiCharacterType.EN)
					{
						_text_data[j]._el++;
					}
				}
			}

			private void ReorderString()
			{
				int num = 0;
				for (int i = 0; i < _text_data.Length; i++)
				{
					if (_text_data[i]._ct == BidiCharacterType.S || _text_data[i]._ct == BidiCharacterType.B)
					{
						for (int j = num; j <= i; j++)
						{
							_text_data[j]._el = EmbeddingLevel;
						}
					}
					if (_text_data[i]._ct != BidiCharacterType.WS)
					{
						num = i + 1;
					}
				}
				for (int k = num; k < _text_data.Length; k++)
				{
					_text_data[k]._el = EmbeddingLevel;
				}
				byte b = 0;
				byte b2 = 63;
				CharData[] text_data = _text_data;
				for (int l = 0; l < text_data.Length; l++)
				{
					CharData charData = text_data[l];
					if (charData._el > b)
					{
						b = charData._el;
					}
					if ((charData._el & 1) == 1 && charData._el < b2)
					{
						b2 = charData._el;
					}
				}
				for (byte b3 = b; b3 >= b2; b3--)
				{
					for (int m = 0; m < _text_data.Length; m++)
					{
						if (_text_data[m]._el >= b3)
						{
							int num2 = m;
							int n;
							for (n = m + 1; n < _text_data.Length && _text_data[n]._el >= b3; n++)
							{
							}
							int num3 = num2;
							int num4 = n - 1;
							while (num3 < num4)
							{
								CharData charData2 = _text_data[num3];
								_text_data[num3] = _text_data[num4];
								_text_data[num4] = charData2;
								num3++;
								num4--;
							}
							m = n;
						}
					}
				}
			}

			private void FixMirroredCharacters()
			{
				for (int i = 0; i < _text_data.Length; i++)
				{
					if ((_text_data[i]._el & 1) == 1)
					{
						_text_data[i]._char = BidiCharacterMirrorResolver.GetBidiCharacterMirror(_text_data[i]._char);
					}
				}
			}

			private string PerformArabicShaping(string text)
			{
				ArabicShapeJoiningType arabicShapeJoiningType = ArabicShapeJoiningType.U;
				LetterForm letterForm = LetterForm.Isolated;
				int num = 0;
				char c = '\uffff';
				LetterForm[] array = new LetterForm[text.Length];
				for (int i = 0; i < text.Length; i++)
				{
					char c2 = text[i];
					ArabicShapeJoiningType arabicShapeJoiningType2 = UnicodeArabicShapingResolver.GetArabicShapeJoiningType(c2);
					if ((arabicShapeJoiningType2 == ArabicShapeJoiningType.R || arabicShapeJoiningType2 == ArabicShapeJoiningType.D || arabicShapeJoiningType2 == ArabicShapeJoiningType.C) && (arabicShapeJoiningType == ArabicShapeJoiningType.L || arabicShapeJoiningType == ArabicShapeJoiningType.D || arabicShapeJoiningType == ArabicShapeJoiningType.C))
					{
						if (letterForm == LetterForm.Isolated && (arabicShapeJoiningType == ArabicShapeJoiningType.D || arabicShapeJoiningType == ArabicShapeJoiningType.L))
						{
							array[num] = LetterForm.Initial;
						}
						else if (letterForm == LetterForm.Final && arabicShapeJoiningType == ArabicShapeJoiningType.D)
						{
							array[num] = LetterForm.Medial;
						}
						array[i] = LetterForm.Final;
						letterForm = LetterForm.Final;
						arabicShapeJoiningType = arabicShapeJoiningType2;
						num = i;
						c = c2;
					}
					else if (arabicShapeJoiningType2 != ArabicShapeJoiningType.T)
					{
						array[i] = LetterForm.Isolated;
						letterForm = LetterForm.Isolated;
						arabicShapeJoiningType = arabicShapeJoiningType2;
						num = i;
						c = c2;
					}
					else
					{
						array[i] = LetterForm.Isolated;
					}
				}
				c = '\uffff';
				num = 0;
				int index = 0;
				StringBuilder stringBuilder = new StringBuilder();
				ArrayList arrayList = new ArrayList(_char_lengths);
				for (int j = 0; j < text.Length; j++)
				{
					char c3 = text[j];
					ArabicShapeJoiningType arabicShapeJoiningType3 = UnicodeArabicShapingResolver.GetArabicShapeJoiningType(c3);
					if (c == 'ل' && c3 != 'ا' && c3 != 'آ' && c3 != 'أ' && c3 != 'إ' && arabicShapeJoiningType3 != ArabicShapeJoiningType.T)
					{
						c = '\uffff';
					}
					else if (c3 == 'ل')
					{
						c = c3;
						num = j;
						index = stringBuilder.Length;
					}
					if (c == 'ل')
					{
						if (array[num] == LetterForm.Medial)
						{
							switch (c3)
							{
							case 'ا':
								stringBuilder[index] = 'ﻼ';
								arrayList.RemoveAt(index);
								continue;
							case 'آ':
								stringBuilder[index] = 'ﻶ';
								arrayList.RemoveAt(index);
								arrayList[index] = (int)arrayList[index] + 1;
								continue;
							case 'أ':
								stringBuilder[index] = 'ﻸ';
								arrayList.RemoveAt(index);
								continue;
							case 'إ':
								stringBuilder[index] = 'ﻺ';
								arrayList.RemoveAt(index);
								continue;
							}
						}
						else if (array[num] == LetterForm.Initial)
						{
							switch (c3)
							{
							case 'ا':
								stringBuilder[index] = 'ﻻ';
								arrayList.RemoveAt(index);
								continue;
							case 'آ':
								stringBuilder[index] = 'ﻵ';
								arrayList.RemoveAt(index);
								arrayList[index] = (int)arrayList[index] + 1;
								continue;
							case 'أ':
								stringBuilder[index] = 'ﻷ';
								arrayList.RemoveAt(index);
								continue;
							case 'إ':
								stringBuilder[index] = 'ﻹ';
								arrayList.RemoveAt(index);
								continue;
							}
						}
					}
					stringBuilder.Append(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(c3, array[j]));
				}
				_char_lengths = (int[])arrayList.ToArray(typeof(int));
				return stringBuilder.ToString();
			}

			private char GetPairwiseComposition(char first, char second)
			{
				if (first < '\0' || first > '\uffff' || second < '\0' || second > '\uffff')
				{
					return '\uffff';
				}
				return UnicodeCharacterDataResolver.Compose(first.ToString() + second);
			}

			private void InternalCompose(StringBuilder target, ArrayList char_lengths)
			{
				if (target.Length == 0)
				{
					return;
				}
				int index = 0;
				int num = 1;
				int num2 = 0;
				char c = target[0];
				char_lengths[index] = (int)char_lengths[index] + 1;
				UnicodeCanonicalClass unicodeCanonicalClass = UnicodeCharacterDataResolver.GetUnicodeCanonicalClass(c);
				if (unicodeCanonicalClass != UnicodeCanonicalClass.NR)
				{
					unicodeCanonicalClass = (UnicodeCanonicalClass)256;
				}
				int length = target.Length;
				for (int i = num; i < target.Length; i++)
				{
					char c2 = target[i];
					UnicodeCanonicalClass unicodeCanonicalClass2 = UnicodeCharacterDataResolver.GetUnicodeCanonicalClass(c2);
					char pairwiseComposition = GetPairwiseComposition(c, c2);
					if (UnicodeCharacterDataResolver.GetUnicodeDecompositionType(pairwiseComposition) == UnicodeDecompositionType.None && pairwiseComposition != '\uffff' && (unicodeCanonicalClass < unicodeCanonicalClass2 || unicodeCanonicalClass == UnicodeCanonicalClass.NR))
					{
						target[index] = pairwiseComposition;
						char_lengths[index] = (int)char_lengths[index] + 1;
						c = pairwiseComposition;
						continue;
					}
					if (unicodeCanonicalClass2 == UnicodeCanonicalClass.NR)
					{
						index = num;
						c = c2;
						num2++;
					}
					unicodeCanonicalClass = unicodeCanonicalClass2;
					target[num] = c2;
					int j = num;
					if ((int)char_lengths[j] < 0)
					{
						for (; (int)char_lengths[j] < 0; j++)
						{
							char_lengths[j] = (int)char_lengths[j] + 1;
							char_lengths.Insert(num, 0);
						}
					}
					else
					{
						char_lengths[j] = (int)char_lengths[j] + 1;
					}
					if (target.Length != length)
					{
						i += target.Length - length;
						length = target.Length;
					}
					num++;
				}
				target.Length = num;
				char_lengths.RemoveRange(num, char_lengths.Count - num);
			}

			private void GetRecursiveDecomposition(bool canonical, char ch, StringBuilder builder)
			{
				string unicodeDecompositionMapping = UnicodeCharacterDataResolver.GetUnicodeDecompositionMapping(ch);
				if (unicodeDecompositionMapping != null && (!canonical || UnicodeCharacterDataResolver.GetUnicodeDecompositionType(ch) == UnicodeDecompositionType.None))
				{
					for (int i = 0; i < unicodeDecompositionMapping.Length; i++)
					{
						GetRecursiveDecomposition(canonical, unicodeDecompositionMapping[i], builder);
					}
				}
				else
				{
					builder.Append(ch);
				}
			}

			private StringBuilder InternalDecompose(ArrayList char_lengths)
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				_hasArabic = false;
				_hasNSMs = false;
				for (int i = 0; i < _text.Length; i++)
				{
					BidiCharacterType bidiCharacterType = UnicodeCharacterDataResolver.GetBidiCharacterType(_text[i]);
					_hasArabic |= bidiCharacterType == BidiCharacterType.AL || bidiCharacterType == BidiCharacterType.AN;
					_hasNSMs |= bidiCharacterType == BidiCharacterType.NSM;
					stringBuilder2.Length = 0;
					GetRecursiveDecomposition(false, _text[i], stringBuilder2);
					char_lengths.Add(1 - stringBuilder2.Length);
					for (int j = 0; j < stringBuilder2.Length; j++)
					{
						char c = stringBuilder2[j];
						UnicodeCanonicalClass unicodeCanonicalClass = UnicodeCharacterDataResolver.GetUnicodeCanonicalClass(c);
						int num = stringBuilder.Length;
						if (unicodeCanonicalClass != UnicodeCanonicalClass.NR)
						{
							while (num > 0)
							{
								char c2 = stringBuilder[num - 1];
								if (UnicodeCharacterDataResolver.GetUnicodeCanonicalClass(c2) <= unicodeCanonicalClass)
								{
									break;
								}
								num--;
							}
						}
						stringBuilder.Insert(num, c);
					}
				}
				return stringBuilder;
			}

			private static BidiCharacterType TypeForLevel(int level)
			{
				return ((level & 1) != 0) ? BidiCharacterType.R : BidiCharacterType.L;
			}

			private int FindRunLimit(int index, int limit, BidiCharacterType[] validSet)
			{
				index--;
				bool flag = false;
				while (++index < limit)
				{
					BidiCharacterType ct = _text_data[index]._ct;
					flag = false;
					for (int i = 0; i < validSet.Length; i++)
					{
						if (flag)
						{
							break;
						}
						if (ct == validSet[i])
						{
							flag = true;
						}
					}
					if (!flag)
					{
						return index;
					}
				}
				return limit;
			}

			private void SetTypes(int start, int limit, BidiCharacterType newType)
			{
				for (int i = start; i < limit; i++)
				{
					_text_data[i]._ct = newType;
				}
			}
		}

		public static string LogicalToVisual(string logicalString)
		{
			Paragraph[] array = SplitStringToParagraphs(logicalString);
			StringBuilder stringBuilder = new StringBuilder();
			Paragraph[] array2 = array;
			foreach (Paragraph paragraph in array2)
			{
				stringBuilder.Append(paragraph.BidiText);
			}
			return stringBuilder.ToString();
		}

		public static string LogicalToVisual(string logicalString, out int[] indexes, out int[] lengths)
		{
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			Paragraph[] array = SplitStringToParagraphs(logicalString);
			StringBuilder stringBuilder = new StringBuilder();
			Paragraph[] array2 = array;
			foreach (Paragraph paragraph in array2)
			{
				stringBuilder.Append(paragraph.BidiText);
				arrayList.AddRange(paragraph.BidiIndexes);
				arrayList2.AddRange(paragraph.BidiIndexLengths);
			}
			indexes = (int[])arrayList.ToArray(typeof(int));
			lengths = (int[])arrayList2.ToArray(typeof(int));
			return stringBuilder.ToString();
		}

		private static Paragraph[] SplitStringToParagraphs(string logicalString)
		{
			ArrayList arrayList = new ArrayList();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in logicalString)
			{
				BidiCharacterType bidiCharacterType = UnicodeCharacterDataResolver.GetBidiCharacterType(c);
				if (bidiCharacterType == BidiCharacterType.B)
				{
					Paragraph paragraph = new Paragraph(stringBuilder.ToString());
					paragraph.ParagraphSeparator = c;
					arrayList.Add(paragraph);
					stringBuilder.Length = 0;
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			if (stringBuilder.Length > 0)
			{
				arrayList.Add(new Paragraph(stringBuilder.ToString()));
			}
			return (Paragraph[])arrayList.ToArray(typeof(Paragraph));
		}
	}
}
