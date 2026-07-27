// $Workfile: RtfUtility.cs $
// $Revision: 11 $	$Date: 2/15/07 2:16p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

namespace Tawala.RtfSupport
{
	public class RtfUtility
	{
		public static string[] HexByteStrings = new string[256];

		static RtfUtility()
		{
			for (int i = 0; i < HexByteStrings.Length; i++)
			{
				HexByteStrings[i] = String.Format("{0:x2}", i);
			}
		}

		public static string DecodeHexString(string hexString)
		{
			MatchCollection matches = Regex.Matches(hexString, @"[0-9A-Fa-f]{2}00");

			byte[] hexBytes = new byte[matches.Count-1];

			for (int i = 0; i < matches.Count-1; i++)
			{
				hexBytes[i] = Convert.ToByte(Convert.ToInt32(matches[i].Value, 16) / 256);
			}

			Encoding encoding = Encoding.UTF8;

			return encoding.GetString(hexBytes);
		}

		public static string EncodeHexString(string textString)
		{
			return EncodeHexString(textString, true);
		}

		public static string EncodeHexString(string textString, bool nullTerminate)
		{
			StringBuilder hexString = new StringBuilder(encodeHexString(textString));

			if (nullTerminate)
			{
				hexString.Append("0000");
			}

			return hexString.ToString().ToLower();
		}

		private static string encodeHexString(string textString)
		{
			StringBuilder hexString = new StringBuilder();

			for (int i = 0; i < textString.Length; i++)
			{
				hexString.AppendFormat("{0:X4}", (int)textString[i] * 256);
			}

			return hexString.ToString().ToLower();
		}

		public static byte[] HexStringToBinary(string hexString)
		{
			byte[] binary = new byte[hexString.Length/2];

			for (int i = 0, j = 0; i < hexString.Length; i+=2, j++)
			{
				string twoCharString = hexString.Substring(i, 2);
				binary[j] = Convert.ToByte(twoCharString, 16);
			}

			return binary;
		}

		/// <summary>
		/// Converts a hex string with the LSB first to an unsigned integer.
		/// </summary>
		public static uint HexStringToUInt(string hexString)
		{
			uint value = 0;

			byte[] binary = HexStringToBinary(hexString);

			for (int i = binary.Length-1; i >= 0; i--)
			{
				value <<= 8;
				value += binary[i];
			}

			return value;
		}

		private static StringBuilder hexString = new StringBuilder();

		/// <summary>
		/// Converts an unsigned integer to a hex string with the LSB first.
		/// </summary>
		private static string UIntToHexString(uint value, int width, bool lowerCase)
		{
			hexString.Length = 0;

			uint currentValue = value;

			do
			{
				hexString.Append(String.Format(lowerCase ? "{0:x2}" : "{0:X2}", currentValue & 0xff));
				currentValue >>= 8;

			} while (currentValue > 0);

			while (hexString.Length < width)
			{
				hexString.Append("0");
			}

			return hexString.ToString();
		}

		public static string UIntToLowerCaseHexString(uint value, int width)
		{
			return UIntToHexString(value, width, true);
		}

		public static string UIntToUpperCaseHexString(uint value, int width)
		{
			return UIntToHexString(value, width, false);
		}

		public static string EscapeSpecialCharacters(string rawTextString)
		{
			StringBuilder fixUpString = new StringBuilder(rawTextString);

			fixUpString.Replace(@"\", @"\\");
			fixUpString.Replace(@"{", @"\{");
			fixUpString.Replace(@"}", @"\}");

			return fixUpString.ToString();
		}

		public static string NonEditableFieldFlags
		{
			get
			{
				return "219";
			}
		}
	}

	[Serializable]
	public struct BitmapFileHeader
	{
		public ushort type;
		public uint fileSize;
		public ushort reserved1;
		public ushort reserved2;
		public uint dataOffset;
	}

	[Serializable]
	public struct BitmapInfoHeader
	{
		public uint headerSize;
		public uint imageWidth;
		public uint imageHeight;
		public ushort numPlanes;
		public ushort bitsPerPixel;
		public uint compression;
		public uint imageSize;
		public uint xPelsPerMeter;
		public uint yPelsPerMeter;
		public uint numColorMapEntries;
		public uint numImportantColorMapEntries;
	}

	[Serializable]
	public struct WindowsMetaHeader
	{
	  public ushort FileType;		/* Type of metafile (0=memory, 1=disk) */
	  public ushort HeaderSize;		/* Size of header in words (always 9) */
	  public ushort Version;		/* Version of Microsoft Windows used */
	  public uint FileSize;			/* Total size of the metafile in public ushorts */
	  public ushort NumOfObjects;	/* Number of objects in the file */
	  public uint MaxRecordSize;	/* The size of largest record in public ushorts */
	  public ushort NumOfParams;	/* Not Used (always 0) */
	}

	[Serializable]
	public struct StandardMetaRecord
	{
		public uint Size;				/* Total size of the record in WORDs */
		public ushort  Function;		/* Function number (defined in WINDOWS.H) */
		public byte[]  Parameters;	/* Parameter values passed to function */
	}



}
