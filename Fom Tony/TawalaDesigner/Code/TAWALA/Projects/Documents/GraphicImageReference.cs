// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using Tawala.RtfSupport;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public class GraphicImageReference : ParagraphComponent
    {
        private int height;
        private string id;
        private RtfMetafile metafile;
        private int width;

        public GraphicImageReference()
        {
        }

        public GraphicImageReference(IXmlElement element)
        {
            id = element.GetAttribute("id");
            width = Convert.ToInt32(element.GetAttribute("width"));
            height = Convert.ToInt32(element.GetAttribute("height"));

            if (element.HasChild("wmfFileString"))
            {
                // old XML format (pre-Build 90)
                metafile = new RtfMetafile(element.GetChild("wmfFileString").Text);
            }
            else
            {
                var xmlStringBuilder = new StringBuilder("<image>");

                xmlStringBuilder.Append(element.GetChild("metafileHeader").OuterXml);

                var image = Project.Images.GetImageById(id);
                xmlStringBuilder.Append(image.BitmapAsMetafileRecord.ToXml());

                xmlStringBuilder.Append("</image>");

                metafile = new RtfMetafile(new XmlElement(xmlStringBuilder.ToString()));
            }
        }

        public string Id
        {
            get { return id; }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public string MetafileHexString
        {
            get { return metafile.ToHexString(); }
        }

        public override string ToXml()
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat("<image id=\"{0}\" width=\"{1}\" height=\"{2}\">", Id, width, height);

            xmlString.Append(metafile.Header.ToXml());

            xmlString.Append("</image>");

            return xmlString.ToString();
        }

        public override string ToHtml()
        {
            return "";
        }

        public override string ToRtf()
        {
            var rtfString = new StringBuilder(@"{\pict\wmetafile8");

            rtfString.AppendFormat(@"\picw{0}", (int)(width*26.5));
            rtfString.AppendFormat(@"\pich{0}", (int)(height*26.5));
            rtfString.AppendFormat(@"\picwgoal{0}", (int)(width*1440/96));
            rtfString.AppendFormat(@"\pichgoal{0}", (int)(height*1440/96));
            rtfString.Append(@"\picscalex100");
            rtfString.Append(@"\picscaley100");
            rtfString.Append(@"\blipupi96 ");
            rtfString.Append(MetafileHexString);
            rtfString.Append(@"}");

            return rtfString.ToString();
        }

        public override string ToRtf(RtfDocument document)
        {
            return ToRtf();
        }
    }
}