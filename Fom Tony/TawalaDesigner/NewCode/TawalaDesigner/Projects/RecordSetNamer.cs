// $Workfile: RecordSetNamer.cs $
// $Revision: 1 $	$Date: 7/31/06 5:47p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace Tawala.Projects
{
	public static class RecordSetNamer
	{
		public static string GetNextName()
		{
			return string.Format("Record List {0}", nextRecordListNumber());
		}

		private static int nextRecordListNumber()
		{
			int maxListNumber = 0;

			foreach (Process process in Project.Current.ProcessList)
			{
				foreach (ProcessLine line in process.Lines)
				{
					if (line is GetLine)
					{
						GetStatement statement = ((GetLine)line).Statement as GetStatement;

						string listNumberPattern = @"Record List (\d+)$";
						Match match = Regex.Match(statement.Records.FieldName, listNumberPattern);

						if (match.Success)
						{
							int listNumber = Convert.ToInt32(match.Groups[1].Value);

							if (listNumber > maxListNumber)
							{
								maxListNumber = listNumber;
							}
						}
					}
				}
			}

			return maxListNumber + 1;
		}
	}
}
