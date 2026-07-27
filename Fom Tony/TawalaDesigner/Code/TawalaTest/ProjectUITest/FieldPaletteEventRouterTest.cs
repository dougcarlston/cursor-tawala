using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using Tawala.ProjectUI;
using Tawala.Documents;
using NUnit.Framework;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectUITest
{
	[TestFixture]
	public class FieldPaletteEventRouterTest
	{
		private static FieldPaletteEventRouter router = null;
		private Listener1 listener1 = null;
		private Listener2 listener2 = null;
		private static MethodInfo routerInvokeDoubleClick = null;
		private static MethodInfo routerSetDoubleClick = null;
		private static MethodInfo routerRemoveDoubleClick = null;

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
			router = Activator.CreateInstance(typeof(FieldPaletteEventRouter), true) as FieldPaletteEventRouter;
			listener1 = new Listener1();
			listener2 = new Listener2();
			Assert.IsNotNull(router);

			routerInvokeDoubleClick = router.GetType().GetMethod("InvokeDoubleClick", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			Assert.IsNotNull(routerInvokeDoubleClick);

			routerSetDoubleClick = router.GetType().GetMethod("SetDoubleClick", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			Assert.IsNotNull(routerSetDoubleClick);

			routerRemoveDoubleClick = router.GetType().GetMethod("RemoveDoubleClick", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			Assert.IsNotNull(routerRemoveDoubleClick);
		}

		[Test]
		public void SingleListenerHandlesOnlyNotifiesOneListener()
		{
			Assert.AreEqual(0, listener1.Count);
			Assert.AreEqual(0, listener2.Count);

			listener1.Attach(router);
			invokeDoubleClick();

			Assert.AreEqual(1, listener1.Count);
			Assert.AreEqual(0, listener2.Count);

			listener2.Attach(router);
			invokeDoubleClick();
			invokeDoubleClick();

			Assert.AreEqual(1, listener1.Count);
			Assert.AreEqual(2, listener2.Count);
		}

		[Test]
		public void RemoveOfNonOwningListenerHasNoEffect()
		{
			Assert.AreEqual(0, listener1.Count);
			Assert.AreEqual(0, listener2.Count);

			listener1.Attach(router);
			invokeDoubleClick();

			Assert.AreEqual(1, listener1.Count);
			Assert.AreEqual(0, listener2.Count);

			listener2.Detach(router);

			invokeDoubleClick();

			Assert.AreEqual(2, listener1.Count);
			Assert.AreEqual(0, listener2.Count);
		}

		[Test]
		public void EventNotFiredAfterRemovingOwningListener()
		{
			Assert.AreEqual(0, listener1.Count);
			Assert.AreEqual(0, listener2.Count);

			listener2.Attach(router);
			invokeDoubleClick();

			Assert.AreEqual(0, listener1.Count);
			Assert.AreEqual(1, listener2.Count);

			listener2.Detach(router);

			invokeDoubleClick();

			Assert.AreEqual(0, listener1.Count);
			Assert.AreEqual(1, listener2.Count);
		}

		[Test]
		public void ListeningControlDetachedWhenDestroyed()
		{
			System.Windows.Forms.Form form = new Form();
			form.Size = new Size(100, 100);
			PaletteAwareTextBox textBox = new PaletteAwareTextBox();
			form.Controls.Add(textBox);
			form.Show();

			textBox.Attach(router);

			Assert.AreEqual(0, textBox.Count);
			invokeDoubleClick();
			Assert.AreEqual(1, textBox.Count);

			form.Close();
			form = null;

			invokeDoubleClick();
			Assert.AreEqual(1, textBox.Count);
		}

		private static void invokeDoubleClick()
		{
			TreeNodeMouseClickEventArgs args = new TreeNodeMouseClickEventArgs(null, MouseButtons.None, 1, 0, 0);
			routerInvokeDoubleClick.Invoke(router, new object[] { new object(), args });
		}

		private static void setDoubleClick(TreeNodeMouseClickEventHandler handler)
		{
			routerSetDoubleClick.Invoke(router, new object[] { handler });
		}

		private static void removeDoubleClick(TreeNodeMouseClickEventHandler handler)
		{
			routerRemoveDoubleClick.Invoke(router, new object[] { handler });
		}

		public class Listener1
		{
			private int count = 0;

			public void Attach(FieldPaletteEventRouter router)
			{
				FieldPaletteEventRouterTest.setDoubleClick(new TreeNodeMouseClickEventHandler(listener1));
			}

			public void Detach(FieldPaletteEventRouter router)
			{
				FieldPaletteEventRouterTest.removeDoubleClick(new TreeNodeMouseClickEventHandler(listener1));
			}


			public int Count { get { return count; } }

			private void listener1(object sender, TreeNodeMouseClickEventArgs e)
			{
				++count;
			}
		}

		public class Listener2
		{
			private int count = 0;

			public void Attach(FieldPaletteEventRouter router)
			{
				FieldPaletteEventRouterTest.setDoubleClick(new TreeNodeMouseClickEventHandler(listener2));
			}

			public void Detach(FieldPaletteEventRouter router)
			{
				FieldPaletteEventRouterTest.removeDoubleClick(new TreeNodeMouseClickEventHandler(listener2));
			}

			public int Count { get { return count; } }

			private void listener2(object sender, TreeNodeMouseClickEventArgs e)
			{
				++count;
			}
		}

		public class PaletteAwareTextBox : TextBox
		{
			private int count = 0;

			public void Attach(FieldPaletteEventRouter router)
			{
				FieldPaletteEventRouterTest.setDoubleClick(new TreeNodeMouseClickEventHandler(nodeDoubleClicked));
			}

			public int Count { get { return count; } }

			private void nodeDoubleClicked(object sender, TreeNodeMouseClickEventArgs e)
			{
				++count;
			}
		}
	}

}
