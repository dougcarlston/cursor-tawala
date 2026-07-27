using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Tawala.RtfSupport
{
	public class RtfImage
	{
		private string wmfFileString;
		private uint width;
		private uint height;
		private string base64PngString;

		public RtfImage(string wmfFileString)
		{
			this.wmfFileString = wmfFileString;
			width = getImageWidth(wmfFileString);
			height = getImageHeight(wmfFileString);

			constructFromBmpData(wmfFileString);
//			constructFromWmfData(wmfFileString);
		}

		private void constructFromBmpData(string wmfFileString)
		{
			base64PngString = getBase64PngStringFromBmp(wmfFileString);
		}

		private void constructFromWmfData(string wmfFileString)
		{
			base64PngString = getBase64PngStringFromWmf(wmfFileString);
		}

		/// <summary>
		/// Image width in pixels.
		/// </summary>
		public uint Width
		{
			get { return width; }
		}

		/// <summary>
		/// Image height in pixels.
		/// </summary>
		public uint Height
		{
			get { return height; }
		}

		/// <summary>
		/// Image in PNG format, encoded as a base 64 string.
		/// </summary>
		public string Base64PngString
		{
			get { return base64PngString; }
		}

		/// <summary>
		/// WMF file string encoded as Binary Hexadecimal.
		/// </summary>
		public string WmfFileString
		{
			get { return wmfFileString; }
		}

		/// <summary>
		/// Returns a string representing a PNG image, encoded as a base 64 string.
		/// </summary>
		private string getBase64PngStringFromBmp(string wmfFileString)
		{
			Bitmap bmp = getBitmap(GetBmpFileString(wmfFileString));

			MemoryStream pngStream = new MemoryStream();
			bmp.Save(pngStream, ImageFormat.Png);

			return Convert.ToBase64String(pngStream.ToArray(), Base64FormattingOptions.InsertLineBreaks);
		}

		private Bitmap getBitmap(string bmpFileString)
		{
			byte[] bmpBytes = RtfUtility.HexStringToBinary(bmpFileString);
			MemoryStream bmpStream = new MemoryStream(bmpBytes);
			return new Bitmap(bmpStream);
		}

		public BitmapFileHeader GetBitmapFileHeader(string bmpFileString)
		{
			byte[] bmpBytes = RtfUtility.HexStringToBinary(bmpFileString);
			MemoryStream byteStream = new MemoryStream(bmpBytes);
			BinaryReader reader = new BinaryReader(byteStream);

			BitmapFileHeader header;

			header.type = reader.ReadUInt16();
			header.fileSize = reader.ReadUInt32();
			header.reserved1 = reader.ReadUInt16();
			header.reserved2 = reader.ReadUInt16();
			header.dataOffset = reader.ReadUInt32();

			return header;
		}

		public static BitmapInfoHeader GetBitmapInfoHeader(string bmpFileString)
		{
			byte[] bmpBytes = RtfUtility.HexStringToBinary(bmpFileString);
			MemoryStream byteStream = new MemoryStream(bmpBytes);
			BinaryReader reader = new BinaryReader(byteStream);

			BitmapInfoHeader header;

			header.headerSize = reader.ReadUInt32();
			header.imageWidth = reader.ReadUInt32();
			header.imageHeight = reader.ReadUInt32();
			header.numPlanes = reader.ReadUInt16();
			header.bitsPerPixel = reader.ReadUInt16();
			header.compression = reader.ReadUInt32();
			header.imageSize = reader.ReadUInt32();
			header.xPelsPerMeter = reader.ReadUInt32();
			header.yPelsPerMeter = reader.ReadUInt32();
			header.numColorMapEntries = reader.ReadUInt32();
			header.numImportantColorMapEntries = reader.ReadUInt32();
			
			return header;
		}


		/// <summary>
		/// Returns a string representing a PNG image, encoded as a base 64 string.
		/// </summary>
		private string getBase64PngStringFromWmf(string wmfFileString)
		{
			Metafile wmf = getMetafile(wmfFileString);
			
			MemoryStream pngStream = new MemoryStream();
			wmf.Save(pngStream, ImageFormat.Png);

			return Convert.ToBase64String(pngStream.ToArray(), Base64FormattingOptions.InsertLineBreaks);
		}

		private Metafile getMetafile(string wmfFileString)
		{
			byte[] wmfBytes = RtfUtility.HexStringToBinary(wmfFileString);
			MemoryStream wmfStream = new MemoryStream(wmfBytes);
			return new Metafile(wmfStream);
		}

		private const uint BITMAPFILEHEADER_SIZE = 14;
		private const uint BITMAPHEADER_SIZE = 40;

		/// <summary>
		/// Extracts a string representing a BMP file from a string representing a WMF file.
		/// </summary>
		public string GetBmpFileString(string wmfFileString)
		{
			uint dataSize = getDataSize(wmfFileString);
			uint colorTableSize = getColorTableSize(wmfFileString);

			uint bmpFileSize = BITMAPFILEHEADER_SIZE + BITMAPHEADER_SIZE + colorTableSize + dataSize;

			string bmpDataPattern = @"^.+(28000000.{72}.{" + dataSize * 2 + "})";
			string bmpDataString = Regex.Match(wmfFileString, bmpDataPattern).Groups[1].Value;

			string bmpFileString =
				"424D" +
				RtfUtility.UIntToHexString(bmpFileSize, 8) +
				"00000000" +
				RtfUtility.UIntToHexString(BITMAPFILEHEADER_SIZE + BITMAPHEADER_SIZE, 8) +
				bmpDataString;

			return bmpFileString;
		}

		/// <summary>
		/// Returns the size, in bytes, of the BMP pixel data in the specified file string
		/// </summary>
		private uint getDataSize(string fileString)
		{
			string dataSizeString = Regex.Match(fileString, @"^.+28000000.{32}(.{8}).+$").Groups[1].Value;
			uint dataSize = RtfUtility.HexStringToUInt(dataSizeString);
			return dataSize;
		}

		/// <summary>
		/// Returns the size, in bytes, of the BMP color table in the specified file string
		/// </summary>
		private uint getColorTableSize(string fileString)
		{
			string numColorsString = Regex.Match(fileString, @"^.+28000000.{56}(.{8}).+$").Groups[1].Value;
			uint numColors = RtfUtility.HexStringToUInt(numColorsString);
			return numColors * 4;
		}

		/// <summary>
		/// Returns the height, in pixels, of the BMP image in the specified file string
		/// </summary>
		private uint getImageHeight(string fileString)
		{
			string heightString = Regex.Match(fileString, @"^.+28000000.{8}(.{8}).+$").Groups[1].Value;
			uint height = RtfUtility.HexStringToUInt(heightString);
			return height;
		}

		/// <summary>
		/// Returns the width, in pixels, of the BMP image in the specified file string
		/// </summary>
		private uint getImageWidth(string fileString)
		{
			string widthString = Regex.Match(fileString, @"^.+28000000(.{8}).+$").Groups[1].Value;
			uint width = RtfUtility.HexStringToUInt(widthString);
			return width;
		}

	}

}
