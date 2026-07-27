using System;
using System.Collections.Generic;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	[Serializable]
	public class Hyperlink : ILink
	{
		public static Hyperlink NULL = new NullHyperlink();

		public Hyperlink()
		{
            Url = string.Empty;
            this.id = Project.NextUniqueID;
			Project.InvitationMapById.AddUnique(this);
		}

		/// <summary>
		/// Constructs a Hyperlink object from a &lt;link&gt; element
		/// </summary>
		public Hyperlink(IXmlElement element) : this()
		{
			setContents(element);
		}

		protected void setContents(IXmlElement element)
		{
			displayText = element.GetChild("description").GetChild("string").GetAttribute("value");
			urlExpression = new UrlExpression(element.GetChild("url"));
			openNewWindow = element.HasChild("new-window");
		}

		public int Id
		{
			get { return id; }
		}

		protected int id = 0;

		/// <summary>
		/// The text for the hyperlink as seen by the user
		/// </summary>
		public string DisplayText
		{
			get { return displayText; }
			set { displayText = value; }
		}

		private string displayText;

		/// <summary>
		/// The URL for the hyperlink as a string
		/// </summary>
		public string Url
		{
			get { return urlExpression.ToString(); }
			set { urlExpression = new UrlExpression(value); }
		}

		private UrlExpression urlExpression;

		/// <summary>
		/// Specifies whether the hyperlink opens a new browser window
		/// </summary>
		public bool OpenNewWindow
		{
			get { return openNewWindow; }
			set { openNewWindow = value; }
		}

		private bool openNewWindow;

		public string ToXml()
		{
			StringBuilder xmlString = new StringBuilder();

			xmlString.Append("<link>" + Environment.NewLine);

			if (openNewWindow)
			{
				xmlString.Append("<new-window/>" + Environment.NewLine);
			}

			xmlString.Append("<description>" + Environment.NewLine);
			xmlString.AppendFormat("<string value=\"" + displayText +"\"/>" + Environment.NewLine);
			xmlString.Append("</description>" + Environment.NewLine);
			xmlString.Append("<url>" + Environment.NewLine);
			xmlString.Append(urlExpression.ToXml());
			xmlString.Append("</url>" + Environment.NewLine);
			xmlString.Append("</link>" + Environment.NewLine);

			return xmlString.ToString();
		}

		[Serializable]
		private class NullHyperlink : Hyperlink
		{
			public NullHyperlink() : base()
			{
				Project.InvitationMapById.Remove(this.Id);
			}
		}
	}

}
