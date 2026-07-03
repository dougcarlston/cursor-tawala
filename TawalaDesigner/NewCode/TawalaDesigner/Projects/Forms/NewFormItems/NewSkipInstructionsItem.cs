using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;
using Tawala.Projects.Forms.FormItemContents;

namespace Tawala.Projects.Forms.NewFormItems
{
	[Serializable]
	public class NewSkipInstructionsItem : FormItem, ISkipInstructionsItem
	{
		public NewSkipInstructionsItem()
		{
			contents = new NewSkipInstructions();
		}

		public NewSkipInstructionsItem(IXmlElement element)
		{
			contents = new NewSkipInstructions(element);
			contents.ResolveFieldReferences();
		}

		public override string ToXml()
		{
			return contents.ToXml();
		}

		#region ISkipInstructionsItem Members

		public string GetSummary()
		{
			if (Instructions.Lines.Count == 0)
			{
				return Properties.Resources.SkipSummaryEmpty;
			}
			else
			{
				StringCollection destinationNames = new StringCollection();

				bool unconditionalSkip = true;

				foreach (ProcessLine line in Instructions.Lines)
				{
					SkipToStatement skipToStatement = line.Statement as SkipToStatement;

					if (skipToStatement != null)
					{
						if (isContainedInIfStatement(line))
						{
							unconditionalSkip = false;
						}

						string destinationName = skipToStatement.Destination.ToString();

						if (!destinationNames.Contains(destinationName))
						{
							destinationNames.Add(destinationName);
						}
					}
				}

				if (destinationNames.Count > 1)
				{
					unconditionalSkip = false;
				}

				if (destinationNames.Count == 0)
				{
					return Properties.Resources.SkipSummaryNoSkips;
				}
				
				if (unconditionalSkip)
				{
					return string.Format(Properties.Resources.SkipSummaryAlwaysSkip, destinationNames[0]);
				}
				else
				{
					StringBuilder summaryString = new StringBuilder();

					foreach (string destination in destinationNames)
					{
						if (summaryString.Length != 0)
						{
							summaryString.Append(", ");
						}

						summaryString.Append(destination);
					}

					return string.Format(Properties.Resources.SkipSummaryMaySkip, summaryString.ToString());
				}
			}
		}

		private static bool isContainedInIfStatement(ProcessLine line)
		{
			return line.IndentLevel > 0;
		}

		public Process Instructions
		{
			get { return contents as Process; }
			set { contents = value as IFormItemContents; }
		}

		#endregion

		#region IEnumerable Interface

		public override IEnumerator GetEnumerator()
		{
			yield break;
		}

		#endregion

		#region IRecursiveEnumerable Interface

		public override IEnumerable RecursiveEnumerator
		{
			get
			{
				yield break;
			}
		}

		#endregion
	}
}
