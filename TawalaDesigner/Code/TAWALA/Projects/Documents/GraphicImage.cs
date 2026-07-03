// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Tawala.RtfSupport;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public class GraphicImage : ParagraphComponent
    {
        private const uint bitmapFileHeaderSize = 14;
        private const uint bitmapHeaderSize = 40;
        private const uint bitmapRecordHeaderSize = 16;
        public new static GraphicImage NULL = new GraphicImage();
        private readonly GraphicImageDefinition imageDefinition;

        private readonly RtfMetafile metafile;
        protected Bitmap bitmap;
        protected string format;

        public GraphicImage()
        {
        }

        public GraphicImage(IXmlElement element)
        {
            format = "PNG";

            metafile = new RtfMetafile(element);
            bitmap = metafile.Bitmap;

            if (Project.Current != null)
            {
                string imageDefString = "<imagedef>" + "<imagedata imageFormat=\"PNG\">" + "{0}" + "</imagedata>" + "</imagedef>";

                IXmlElement definitionElement = new XmlElement(String.Format(imageDefString, Base64PngString));
                imageDefinition = new GraphicImageDefinition(definitionElement);
                Project.Images.AddUnique(imageDefinition);
            }
        }

        public int Width { get { return bitmap.Width; } }

        public int Height { get { return bitmap.Height; } }

        public string Format { get { return format; } }

        public Bitmap Bitmap { get { return bitmap; } }

        public virtual string Id { get { return Project.Images[imageDefinition].Id; } set { } }

        public string Base64PngString { get { return (bitmap != null ? getBase64PngStringFromBmp() : ""); } }

        public MetafileRecord BitmapAsMetafileRecord
        {
            get
            {
                uint rowSize = getRowSize(bitmap.Width);
                uint imageSize = rowSize*(uint)bitmap.Height;

                uint parametersLength = bitmapRecordHeaderSize + bitmapHeaderSize + imageSize;

                var recordStruct = new StandardMetaRecord();
                recordStruct.Function = 0x0940;
                recordStruct.Size = (parametersLength/2) + 3;
                recordStruct.Parameters = new byte[parametersLength];

                var stream = new MemoryStream(recordStruct.Parameters);
                var writer = new BinaryWriter(stream);

                // 16 byte header - exact definition unknown, but the only variable seem to be the bitmap size
                byte[] array = {0x20, 0x00, 0xcc, 0x00, 0x00, 0x00, 0x00, 0x00};
                writer.Write(array);
                writer.Write((UInt16)bitmap.Height);
                writer.Write((UInt16)bitmap.Width);
                writer.Write((UInt32)0);

                // BITMAPHEADER
                writer.Write(bitmapHeaderSize); // headerSize
                writer.Write((UInt32)bitmap.Width); // imageWidth
                writer.Write((UInt32)bitmap.Height); // imageHeight
                writer.Write((UInt16)1); // numPlanes
                writer.Write((UInt16)24); // bitsPerPixel
                writer.Write((UInt32)0); // compression
                writer.Write(imageSize); // imageSize
                writer.Write((UInt32)0); // xPelsPerMeter
                writer.Write((UInt32)0); // yPelsPerMeter
                writer.Write((UInt32)0); // numColorMapEntries
                writer.Write((UInt32)0); // numImportantColorMapEntries

                // bitmap data
                var bmpStream = new MemoryStream();
                bitmap.Save(bmpStream, ImageFormat.Bmp);
                var reader = new BinaryReader(bmpStream);

                bmpStream.Position = bitmapHeaderSize + bitmapFileHeaderSize;

                for (int y = 0; y < bitmap.Height; y++)
                {
                    var buffer = new byte[rowSize];
                    reader.Read(buffer, 0, (int)rowSize);
                    writer.Write(buffer);
                }

                return new MetafileRecord(recordStruct);
            }
        }

        public override string ToXml()
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat("<image id=\"{0}\" width=\"{1}\" height=\"{2}\">", Id, Width, Height);

            xmlString.Append(metafile.Header.ToXml());

            xmlString.Append("</image>");

            return xmlString.ToString();
        }

        public override string ToRtf()
        {
            return metafile.ToRtf();
        }

        public override string ToRtf(RtfDocument document)
        {
            return ToRtf();
        }

        /// <summary>
        /// Converts a base-64 representation of a PNG image to a bitmap object.
        /// </summary>
        protected Bitmap pngStringToBitmap(string pngString)
        {
            byte[] pngData = Convert.FromBase64String(pngString);
            var pngStream = new MemoryStream(pngData);
            return new Bitmap(pngStream);
        }

        /// <summary>
        /// Returns a string representing a PNG image, encoded as a base 64 string.
        /// </summary>
        private string getBase64PngStringFromBmp()
        {
            var pngStream = new MemoryStream();
            bitmap.Save(pngStream, ImageFormat.Png);

            return Convert.ToBase64String(pngStream.ToArray(), Base64FormattingOptions.InsertLineBreaks);
        }

        /// <summary>
        /// Returns the size of a row, in bytes, based on the specified width.
        /// </summary>
        private uint getRowSize(int widthInPixels)
        {
            return (uint)(widthInPixels + 1)*3/4*4;
        }
    }

    [Serializable]
    public class GraphicImageDefinition : GraphicImage
    {
        private string id = "Unidentified Image";

        public GraphicImageDefinition()
        {
        }

        public GraphicImageDefinition(string fileName)
        {
            format = "PNG";
            bitmap = new Bitmap(fileName);
        }

        public GraphicImageDefinition(IXmlElement element)
        {
            format = element.GetChild("imagedata").GetAttribute("imageFormat");
            bitmap = pngStringToBitmap(element.GetChild("imagedata").Text);

            //REVISIT: Is it acceptable to force the resolution like this? - SB 10/30/2006
            bitmap.SetResolution(96.0f, 96.0f);

            id = element.GetAttribute("id", "Unidentified Image");
        }

        public override string Id { get { return id; } set { id = value; } }

        public override string ToXml()
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat("<imagedef id=\"{0}\">", id);
            xmlString.AppendFormat("<imagedata imageFormat=\"{0}\">", Format);

            xmlString.Append(Base64PngString);

            xmlString.Append("</imagedata>");
            xmlString.Append("</imagedef>");

            return xmlString.ToString();
        }
    }
}