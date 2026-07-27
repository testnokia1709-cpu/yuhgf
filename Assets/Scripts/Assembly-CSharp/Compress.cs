using System;
using System.Text;
using Lzf;

public class Compress
{
	private static int bufferSize = 100000;

	public static byte[] CompressText(string text)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(text);
		byte[] array = new byte[bufferSize];
		LZF lZF = new LZF();
		int num = lZF.Compress(bytes, bytes.Length, array, array.Length);
		byte[] array2 = new byte[num];
		Array.Copy(array, array2, num);
		return array2;
	}

	public static string DecompressText(byte[] byteData)
	{
		byte[] array = new byte[bufferSize];
		LZF lZF = new LZF();
		int num = lZF.Decompress(byteData, byteData.Length, array, array.Length);
		byte[] array2 = new byte[num];
		Array.Copy(array, array2, num);
		return Encoding.ASCII.GetString(array2);
	}
}
