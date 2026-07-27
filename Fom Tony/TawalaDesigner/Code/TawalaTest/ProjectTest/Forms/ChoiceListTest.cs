using System;
using System.Collections.Specialized;
using NUnit.Framework;
using Tawala.Common;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the ChoiceList class
	/// </summary>
	[TestFixture]
	public class ChoiceListTest
	{
		[Test]
		public void AddChoice() 
		{ 
			Choice choice = new Choice();

			ChoiceList list = new ChoiceList();
			list.Add(choice);

			//Assertions 
			Assert.AreEqual(1, list.Count);
		} 

		[Test]
		public void AddTwoChoicesRetrieveOne() 
		{ 
			Choice choice1 = new Choice();
			choice1.Text = "Choice 1";
			Choice choice2 = new Choice();
			choice2.Text = "Choice 2";

			ChoiceList list = new ChoiceList();
			list.Add(choice1);
			list.Add(choice2);

			IChoice choice2ByIndex = list[1];

			//Assertions 
			Assert.AreEqual("Choice 2", choice2ByIndex.Text);
		} 

		[Test]
		public void Contains() 
		{ 
			Choice choice1 = new Choice("Choice 1");
			Choice choice2 = new Choice("Choice 2");

			ChoiceList list = new ChoiceList();
			list.Add(choice1);
			list.Add(choice2);

			//Assertions 
			Assert.AreEqual(true, list.Contains(choice1));
			Assert.AreEqual(true, list.Contains(choice2));
		}

		[Test]
		public void GetLabelsCollection()
		{
			Choice choice1 = new Choice("Choice 1");
			Choice choice2 = new Choice("Choice 2");
			Choice choice3 = new Choice("Choice 3");

			ChoiceList list = new ChoiceList();
			list.Add(choice1);
			list.Add(choice2);
			list.Add(choice3);

			StringCollection labels = list.GetLabelsCollection();

			Assert.AreEqual(3, list.Count);

			Assert.AreEqual("a", labels[0]);
			Assert.AreEqual("b", labels[1]);
			Assert.AreEqual("c", labels[2]);
		}

		[Test]
		public void IndexOf() 
		{ 
			Choice choice1 = new Choice("Choice 1");
			Choice choice2 = new Choice("Choice 2");

			ChoiceList list = new ChoiceList();
			list.Add(choice1);
			list.Add(choice2);

			//Assertions 
			Assert.AreEqual(0, list.IndexOf(choice1));
			Assert.AreEqual(1, list.IndexOf(choice2));
		}

		[Test]
		public void IndexOfLabel()
		{
			Choice choice1 = new Choice("Choice 1");
			Choice choice2 = new Choice("Choice 2");

			ChoiceList list = new ChoiceList();
			list.Add(choice1);
			list.Add(choice2);

			//Assertions 
			Assert.AreEqual(0, ChoiceList.IndexOfLabel("a"));
			Assert.AreEqual(1, ChoiceList.IndexOfLabel("b"));
			Assert.AreEqual(25, ChoiceList.IndexOfLabel("z"));
			Assert.AreEqual(77, ChoiceList.IndexOfLabel("zzz"));
		}

		[Test]
		public void Insert() 
		{ 
			Choice choice1 = new Choice("Choice 1");
			Choice choice2 = new Choice("Choice 2");
			Choice choice3 = new Choice("Choice 3");

			ChoiceList list = new ChoiceList();
			list.Add(choice1);
			list.Add(choice3);

			list.Insert(1, choice2);

			//Assertions 
			Assert.AreEqual(choice1, list[0]);
			Assert.AreEqual(choice2, list[1]);
			Assert.AreEqual(choice3, list[2]);
		}

		[Test]
		public void Remove() 
		{ 
			Choice choice1 = new Choice("Choice 1");
			Choice choice2 = new Choice("Choice 2");
			Choice choice3 = new Choice("Choice 3");

			ChoiceList list = new ChoiceList();
			list.Add(choice1);
			list.Add(choice2);
			list.Add(choice3);

			//Assertions 
			Assert.AreEqual(choice1, list[0]);
			Assert.AreEqual(choice2, list[1]);
			Assert.AreEqual(choice3, list[2]);

			list.Remove(choice2);

			//Assertions 
			Assert.AreEqual(choice1, list[0]);
			Assert.AreEqual(choice3, list[1]);
		}

		[Test]
		public void GetXml() 
		{ 
			const int arraysize = 78;

			string[] expStrings = new string[arraysize];
			string[] labelStrings = new string[arraysize]	{"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
															 "aa", "bb", "cc", "dd", "ee", "ff", "gg", "hh", "ii", "jj", "kk", "ll", "mm", "nn", "oo", "pp", "qq", "rr", "ss", "tt", "uu", "vv", "ww", "xx", "yy", "zz",
															 "aaa", "bbb", "ccc", "ddd", "eee", "fff", "ggg", "hhh", "iii", "jjj", "kkk", "lll", "mmm", "nnn", "ooo", "ppp", "qqq", "rrr", "sss", "ttt", "uuu", "vvv", "www", "xxx", "yyy", "zzz"};

			ChoiceList list = new ChoiceList();

			for (int i = 1; i <= arraysize; i++)
			{
				// generate choice and add to list
				Choice choice = new Choice();
				choice.Text = "Choice " + i;
				list.Add(choice);

				expStrings[i - 1] = 
					"<choice label=\"" + labelStrings[i - 1] + "\">" +
					"<paragraph indent=\"0\" align=\"left\">" +
					XmlConstants.FullBeginFont +
					"Choice " + i + 
					XmlConstants.EndFont +
					"</paragraph>" +
					"</choice>";
			}

			//Assertions
			for (int i = 0; i < arraysize; i++)
			{
				// generate item label from index
				AlphaLabel itemLabel = new AlphaLabel(i);

				Assert.AreEqual(expStrings[i], list[i].ToXml(itemLabel.ToString()));
			}
		} 
	}
}
