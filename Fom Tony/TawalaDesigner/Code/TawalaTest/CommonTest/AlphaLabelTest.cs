using System;
using NUnit.Framework;
using Tawala.Common;

namespace TawalaTest.CommonTest
{
	[TestFixture]
	public class ItemLabelTest 
	{ 
		[Test]
		public void IndexToLabel() 
		{
			const int arraysize = 18;

			int[] indexes = new int[arraysize]		{ 0,  1,  2, 23, 24, 25,
													 26, 27, 28, 49, 50, 51,
													 52, 53, 54, 75, 76, 77 };

			string[] expLabels = new string[arraysize]	{  "a",   "b",   "c",   "x",   "y",   "z",
														  "aa",  "bb",  "cc",  "xx",  "yy",  "zz",
														 "aaa", "bbb", "ccc", "xxx", "yyy", "zzz" };

			AlphaLabel[] labels = new AlphaLabel[arraysize];

			// create labels
			for (int i = 0; i < arraysize; i++)
			{
				labels[i] = new AlphaLabel(indexes[i]);
			}

			// verify matches between base 10 and base 26 values
			for (int i = 0; i < arraysize; i++)
			{
				Assert.AreEqual (expLabels[i], labels[i].ToString());
			}

		}
	}
}
