// $Workfile: RtfPictureData.cs $
// $Revision: 5 $	$Date: 10/29/06 11:33a $
// Copyright © 2005 - 2006 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.RtfSupport
{
	[Serializable]
	public class RtfPictureData
	{
		public RtfPictureData()
		{
		}

		public RtfPictureData(IXmlElement element)
		{
			w = Convert.ToInt32(element.GetAttribute("w"));
			h = Convert.ToInt32(element.GetAttribute("h"));
			wGoal = Convert.ToInt32(element.GetAttribute("wgoal"));
			hGoal = Convert.ToInt32(element.GetAttribute("hgoal"));
			scaleX = Convert.ToInt32(element.GetAttribute("scalex"));
			scaleY = Convert.ToInt32(element.GetAttribute("scaley"));
			upi = Convert.ToInt32(element.GetAttribute("upi"));
		}

		private string metafileHexString = "";

		public string MetafileHexString
		{
			get { return metafileHexString; }
			set { metafileHexString = value; }
		}

        private int h = 0;

        public int H
        {
            get { return h; }
            set { h = value; }
        }

        private int w = 0;

        public int W
        {
            get { return w; }
            set { w = value; }
        }

        private int hGoal = 0;

        public int HGoal
        {
            get { return hGoal; }
            set { hGoal = value; }
        }

        private int wGoal = 0;

        public int WGoal
        {
            get { return wGoal; }
            set { wGoal = value; }
        }

        private int scaleX = 0;

        public int ScaleX
        {
            get { return scaleX; }
            set { scaleX = value; }
        }

        private int scaleY = 0;

        public int ScaleY
        {
            get { return scaleY; }
            set { scaleY = value; }
        }

		private int upi = 0;

		public int Upi
		{
			get { return upi; }
			set { upi = value; }
		}

		public string ToXml()
		{
			string xmlString = "w=\"{0}\" h=\"{1}\" wgoal=\"{2}\" hgoal=\"{3}\" scalex=\"{4}\" scaley=\"{5}\" upi=\"{6}\"";
			return string.Format(xmlString, w, h, wGoal, hGoal, scaleX, scaleY, upi);
		}

		public string ToRtf()
		{
			StringBuilder rtfString = new StringBuilder();

			rtfString.AppendFormat(@"\picw{0}", w);
			rtfString.AppendFormat(@"\pich{0}", h);
			rtfString.AppendFormat(@"\picwgoal{0}", wGoal);
			rtfString.AppendFormat(@"\pichgoal{0}", hGoal);
			rtfString.AppendFormat(@"\picscalex{0}", scaleX);
			rtfString.AppendFormat(@"\picscaley{0}", scaleY);
			rtfString.AppendFormat(@"\blipupi{0} ", upi);

			return rtfString.ToString();
		}
	}
}
