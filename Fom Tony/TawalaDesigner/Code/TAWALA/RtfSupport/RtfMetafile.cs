using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Tawala.XmlSupport;

namespace Tawala.RtfSupport
{
	/// <summary>
	/// Represents a standard Windows Metafile (WMF).
	/// </summary>
    [Serializable]
    public class RtfMetafile
	{
		public RtfMetafile(RtfPictureData pictureData)
		{
			this.pictureData = pictureData;
			this.header = getHeaderFromWmfHexString(pictureData.MetafileHexString);
			this.records = getRecordsFromWmfHexString(pictureData.MetafileHexString);

			this.bitmap = getBitmap();
		}


		public RtfMetafile(IXmlElement element)
		{
			this.pictureData = new RtfPictureData(element);

			this.header = new MetafileHeader(element.GetChild("metafileHeader"));

			foreach (IXmlElement childElement in element.GetChildren("metafileRecord"))
			{
				StandardMetaRecord stdRecord = new StandardMetaRecord();
				stdRecord.Size = Convert.ToUInt32(childElement.GetAttribute("size"));
				stdRecord.Function = Convert.ToUInt16(childElement.GetAttribute("function"));

				if (childElement.Text != null)
				{
					stdRecord.Parameters = RtfUtility.HexStringToBinary(childElement.Text);
				}

				MetafileRecord record = createMetafileRecord(stdRecord);
				this.records.Add(record);
			}

			this.bitmap = getBitmap();
		}

		//public RtfMetafile(string metafileHexString)
		//{
		//    this.header = getHeaderFromWmfHexString(metafileHexString);
		//    this.records = getRecordsFromWmfHexString(metafileHexString);

		//    this.bitmap = getBitmap();
		//}

		public RtfMetafile(string metafileHexString)
		{
			pictureData.MetafileHexString = metafileHexString;
			pictureData.Upi = 96;

			this.header = getHeaderFromWmfHexString(metafileHexString);
			this.records = getRecordsFromWmfHexString(metafileHexString);

			this.bitmap = getBitmap();
		}

		private RtfPictureData pictureData = new RtfPictureData();

		private Bitmap bitmap;

		public Bitmap Bitmap
		{
			get { return bitmap; }
		}

		private MetafileHeader header;

		public MetafileHeader Header
		{
			get { return header; }
		}

		private Collection<MetafileRecord> records = new Collection<MetafileRecord>();

		public Collection<MetafileRecord> Records
		{
			get { return records; }
		}

		/// <summary>
		/// Image width in pixels.
		/// </summary>
		public int Width
		{
			get { return bitmap.Width; }
		}

		/// <summary>
		/// Image height in pixels.
		/// </summary>
		public int Height
		{
			get { return bitmap.Height; }
		}

		/// <summary>
		/// Image in PNG format, encoded as a base 64 string.
		/// </summary>
		public string Base64PngString
		{
			get
			{
				MemoryStream pngStream = new MemoryStream();
				bitmap.Save(pngStream, ImageFormat.Png);

				return Convert.ToBase64String(pngStream.ToArray(), Base64FormattingOptions.InsertLineBreaks);
			}
		}

		private MetafileHeader getHeaderFromWmfHexString(string metafileHexString)
		{
			BinaryReader reader = getHexStringMetafileReader(metafileHexString);

			return readHeader(reader);
		}

		private Collection<MetafileRecord> getRecordsFromWmfHexString(string metafileHexString)
		{
			Collection<MetafileRecord> records = new Collection<MetafileRecord>();

			BinaryReader reader = getHexStringMetafileReader(metafileHexString);

			readHeader(reader);

			StandardMetaRecord record;

			Boolean readDone = false;

			do
			{
				try
				{
					record = readRecord(reader);
					records.Add(createMetafileRecord(record));
				}
				catch
				{
					readDone = true;
				}

			} while (!readDone);

			return records;
		}

		private Bitmap getBitmap()
		{
			foreach (MetafileRecord record in records)
			{
				if (record is MetafileImageRecord)
				{
					return ((MetafileImageRecord)record).Bitmap;
				}
			}

			return null;
		}

		private BinaryReader getHexStringMetafileReader(string metafileHexString)
		{
			byte[] wmfBytes = RtfUtility.HexStringToBinary(metafileHexString);
			MemoryStream byteStream = new MemoryStream(wmfBytes);
			BinaryReader reader = new BinaryReader(byteStream);
			return reader;
		}

		private static MetafileHeader readHeader(BinaryReader reader)
		{
			WindowsMetaHeader headerStruct;

			headerStruct.FileType = reader.ReadUInt16();
			headerStruct.HeaderSize = reader.ReadUInt16();
			headerStruct.Version = reader.ReadUInt16();
			headerStruct.FileSize = reader.ReadUInt32();
			headerStruct.NumOfObjects = reader.ReadUInt16();
			headerStruct.MaxRecordSize = reader.ReadUInt32();
			headerStruct.NumOfParams = reader.ReadUInt16();

			return new MetafileHeader(headerStruct);
		}

		/// <summary>
		/// Reads a StandardMetaRecord from the current position of the specified reader.
		/// </summary>
		private static StandardMetaRecord readRecord(BinaryReader reader)
		{
			StandardMetaRecord record;

			record.Size = reader.ReadUInt32();
			record.Function = reader.ReadUInt16();
			record.Parameters = reader.ReadBytes(((int)record.Size - 3) * 2);

			return record;
		}

		/// <summary>
		/// Factory method that returns a particular subtype of MetafileRecord based on information found in
		/// the specified standard metafile record.
		/// </summary>
		private MetafileRecord createMetafileRecord(StandardMetaRecord record)
		{
			if (record.Function == 0xf43)
			{
				return new StretchDiBitsRecord(record);
			}

			if (record.Function == 0x0940)
			{
				return new DibBitBltRecord(record);
			}

			return new MetafileRecord(record);
		}

		public string ToXml()
		{
			StringBuilder xmlStringBuilder = new StringBuilder();

			xmlStringBuilder.AppendFormat("<image {0}>", pictureData.ToXml());

			xmlStringBuilder.Append(header.ToXml());

			foreach (MetafileRecord record in records)
			{
			    xmlStringBuilder.Append(record.ToXml());
			}

			xmlStringBuilder.Append("</image>");

			return xmlStringBuilder.ToString();
		}

		public string ToRtf()
		{
			StringBuilder rtfString = new StringBuilder(@"{\pict\wmetafile8");

			rtfString.Append(pictureData.ToRtf());

			rtfString.Append(ToHexString());

			rtfString.Append("");
			rtfString.Append(@"}");

			return rtfString.ToString();
		}

		private const string endOfMetafileRecordString = "030000000000";

		public string ToHexString()
		{
			StringBuilder hexStringBuilder = new StringBuilder();

			hexStringBuilder.Append(header.ToHexString());

			foreach (MetafileRecord record in records)
			{
				hexStringBuilder.Append(record.ToHexString());
			}

			if (records.Count > 0)
			{
				// without this "end of metafile record," images will not appear
				// in TX control after Windows Security Update for Windows XP (KB938829)
				//											jdf - 9/10/07
				if (!lastRecordIsEndOfMetfileRecord())
				{
					hexStringBuilder.Append(endOfMetafileRecordString);
				}
			}

			return hexStringBuilder.ToString();
		}

		private bool lastRecordIsEndOfMetfileRecord()
		{
			int lastRecordIndex = records.Count - 1;

			return (records[lastRecordIndex].Function == 0 &&
					records[lastRecordIndex].Size == 3 &&
					records[lastRecordIndex].Parameters == null);
		}
	}

    [Serializable]
    public class MetafileHeader
	{
		private WindowsMetaHeader headerStruct;

		public MetafileHeader(WindowsMetaHeader headerStruct)
		{
			this.headerStruct = headerStruct;
		}

		public MetafileHeader(IXmlElement element)
		{
			headerStruct.FileType = Convert.ToUInt16(element.GetAttribute("fileType"));
			headerStruct.HeaderSize = Convert.ToUInt16(element.GetAttribute("headerSize"));
			headerStruct.Version = Convert.ToUInt16(element.GetAttribute("version"));
			headerStruct.FileSize = Convert.ToUInt32(element.GetAttribute("fileSize"));
			headerStruct.NumOfObjects = Convert.ToUInt16(element.GetAttribute("numOfObjects"));
			headerStruct.MaxRecordSize = Convert.ToUInt32(element.GetAttribute("maxRecordSize"));
			headerStruct.NumOfParams = Convert.ToUInt16(element.GetAttribute("numOfParams"));
		}

		public ushort FileType
		{
			get {return headerStruct.FileType;}
		}

		public ushort HeaderSize
		{
			get {return headerStruct.HeaderSize;}
		}

		public ushort Version
		{
			get {return headerStruct.Version;}
		}

		public uint FileSize
		{
			get {return headerStruct.FileSize;}
		}

		public ushort NumOfObjects
		{
			get {return headerStruct.NumOfObjects;}
		}

		public uint MaxRecordSize
		{
			get {return headerStruct.MaxRecordSize;}
		}

		public ushort NumOfParams
		{
			get {return headerStruct.NumOfParams;}
		}

		public string ToXml()
		{
			return string.Format("<metafileHeader fileType=\"{0}\" headerSize=\"{1}\" version=\"{2}\" fileSize=\"{3}\" numOfObjects=\"{4}\" maxRecordSize=\"{5}\" numOfParams=\"{6}\" />\r\n",
				headerStruct.FileType, headerStruct.HeaderSize, headerStruct.Version, headerStruct.FileSize, headerStruct.NumOfObjects, headerStruct.MaxRecordSize, headerStruct.NumOfParams);
		}

		public string ToHexString()
		{
			StringBuilder hexStringBuilder = new StringBuilder();

			hexStringBuilder.Append(RtfUtility.UIntToLowerCaseHexString(headerStruct.FileType, 4));
			hexStringBuilder.Append(RtfUtility.UIntToLowerCaseHexString(headerStruct.HeaderSize, 4));
			hexStringBuilder.Append(RtfUtility.UIntToLowerCaseHexString(headerStruct.Version, 4));
			hexStringBuilder.Append(RtfUtility.UIntToLowerCaseHexString(headerStruct.FileSize, 8));
			hexStringBuilder.Append(RtfUtility.UIntToLowerCaseHexString(headerStruct.NumOfObjects, 4));
			hexStringBuilder.Append(RtfUtility.UIntToLowerCaseHexString(headerStruct.MaxRecordSize, 8));
			hexStringBuilder.Append(RtfUtility.UIntToLowerCaseHexString(headerStruct.NumOfParams, 4));

			return hexStringBuilder.ToString();
		}
	}

	/// <summary>
	/// Base class for any metafile record.
	/// </summary>
    [Serializable]
    public class MetafileRecord
	{
		protected StandardMetaRecord record;

		public MetafileRecord()
		{
		}

		public MetafileRecord(StandardMetaRecord record)
		{
			this.record = record;
		}

		public uint Size
		{
			get
			{
				return record.Size;
			}
		}

		public ushort Function
		{
			get
			{
				return record.Function;
			}
		}

		public int ParametersLength
		{
			get
			{
				return record.Parameters.Length / 2;
			}
		}

		public byte[] Parameters
		{
			get
			{
				return record.Parameters;
			}
		}

		public string ToXml()
		{
			StringBuilder xmlStringBuilder = new StringBuilder();

			xmlStringBuilder.AppendFormat("<metafileRecord size=\"{0}\" function=\"{1}\">", record.Size, record.Function);

			if (record.Parameters != null)
			{
				foreach (byte b in record.Parameters)
				{
//					xmlStringBuilder.Append(RtfUtility.UIntToLowerCaseHexString(b, 2));
					xmlStringBuilder.Append(RtfUtility.HexByteStrings[b]);
				}
			}

			xmlStringBuilder.Append("</metafileRecord>\r\n");

			return xmlStringBuilder.ToString();
		}

		public string ToHexString()
		{
			StringBuilder hexStringBuilder = new StringBuilder();

			hexStringBuilder.Append(RtfUtility.UIntToLowerCaseHexString(record.Size, 8));
			hexStringBuilder.Append(RtfUtility.UIntToLowerCaseHexString(record.Function, 4));

			if (record.Parameters != null)
			{
				foreach (byte b in record.Parameters)
				{
//					hexStringBuilder.Append(RtfUtility.UIntToLowerCaseHexString(b, 2));
					hexStringBuilder.Append(RtfUtility.HexByteStrings[b]);
				}
			}

			return hexStringBuilder.ToString();
		}
	}

	/// <summary>
	/// Base class for any metafile record containing image data.
	/// </summary>
    [Serializable]
    public class MetafileImageRecord : MetafileRecord
	{
		private const uint BITMAPFILEHEADER_SIZE = 14;
		private const uint BITMAPHEADER_SIZE = 40;
		private int bitmapOffset;

		public MetafileImageRecord(StandardMetaRecord record, int bitmapOffset) : base(record)
		{
			this.bitmapOffset = bitmapOffset;
		}

		/// <summary>
		/// Returns the bitmap that is imbedded in the metafile record.
		/// </summary>
		public Bitmap Bitmap
		{
			get
			{
				BitmapInfoHeader bmpInfoheader = getBitmapInfoHeader(record.Parameters, bitmapOffset);

				int colorMapSize = (bmpInfoheader.bitsPerPixel == 8 ? 256 * 4 : 0);

				BitmapFileHeader bmpFileHeader = new BitmapFileHeader();
				bmpFileHeader.type = 0x4d42;
				bmpFileHeader.fileSize = BITMAPFILEHEADER_SIZE + ((uint)record.Parameters.Length - (uint)bitmapOffset);
				bmpFileHeader.reserved1 = 0;
				bmpFileHeader.reserved2 = 0;
				bmpFileHeader.dataOffset = BITMAPFILEHEADER_SIZE + BITMAPHEADER_SIZE + (uint)colorMapSize;

				MemoryStream bmpStream = new MemoryStream();
				BinaryWriter writer = new BinaryWriter(bmpStream);
				writer.Write(bmpFileHeader.type);
				writer.Write(bmpFileHeader.fileSize);
				writer.Write(bmpFileHeader.reserved1);
				writer.Write(bmpFileHeader.reserved2);
				writer.Write(bmpFileHeader.dataOffset);

				bmpStream.Write(record.Parameters, bitmapOffset, record.Parameters.Length - bitmapOffset);
				return new Bitmap(bmpStream);
			}
		}

		private BitmapInfoHeader getBitmapInfoHeader(byte[] buffer, int startIndex)
		{
			MemoryStream byteStream = new MemoryStream(buffer);
			BinaryReader reader = new BinaryReader(byteStream);

			reader.Read(buffer, 0, startIndex);

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
	}

	/// <summary>
	/// Metafile record representing the StretchDiBits function (0xf43)
	/// </summary>
    [Serializable]
    public class StretchDiBitsRecord : MetafileImageRecord
	{
		public StretchDiBitsRecord(StandardMetaRecord record) : base(record, 22)
		{
		}
	}

	/// <summary>
	/// Metafile record representing the DibBitBlt function (0x0940)
	/// </summary>
    [Serializable]
    public class DibBitBltRecord : MetafileImageRecord
	{
		public DibBitBltRecord(StandardMetaRecord record) : base(record, 16)
		{
		}
	}

}
