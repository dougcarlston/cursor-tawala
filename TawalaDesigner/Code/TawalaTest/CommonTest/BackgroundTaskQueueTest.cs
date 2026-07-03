using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Specialized;
using NUnit.Framework;
using Tawala.Common;

namespace TawalaTest.CommonTest
{
//	[Ignore]
	[TestFixture]
	public class BackgroundTaskQueueTest : System.Windows.Forms.IWin32Window
	{
		private static BackgroundTaskQueue queue;
		public static StringCollection AllTasksRun = new StringCollection();
		public static StringCollection AllTasksOK = new StringCollection();
		public static StringCollection AllTasksError = new StringCollection();
		private const double msTimeout = 10000.0;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			Assert.IsNull(queue);
			queue = new BackgroundTaskQueue(this);
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			Assert.IsNotNull(queue);
			queue = null;
		}

		[SetUp]
		public void Setup()
		{
			AllTasksError.Clear();
			AllTasksOK.Clear();
			AllTasksRun.Clear();
		}

		[Test]
		public void ArrayOfTasks()
		{
			TaskOK1 taskOK1 = new TaskOK1();
			TaskOK2 taskOK2 = new TaskOK2();
			TaskError3 taskError3 = new TaskError3();
			TaskOK4 taskOK4 = new TaskOK4();

			TaskBase[] taskArray = new TaskBase[4] { taskOK1, taskOK2, taskError3, taskOK4 };
			queue.Add(taskArray);

			double msExecTime = waitUntilTasksComplete();

			Assert.IsTrue(msExecTime < msTimeout);

			Assert.IsFalse(queue.IsBusy);

			Assert.AreEqual(taskArray.Length, AllTasksRun.Count);
			Assert.AreEqual(3, AllTasksOK.Count);
			Assert.AreEqual(1, AllTasksError.Count);

			Assert.AreEqual(taskArray[0].GetType().Name, AllTasksRun[0]);
			Assert.AreEqual(taskArray[1].GetType().Name, AllTasksRun[1]);
			Assert.AreEqual(taskArray[2].GetType().Name, AllTasksRun[2]);
			Assert.AreEqual(taskArray[3].GetType().Name, AllTasksRun[3]);

			Assert.AreEqual(taskArray[0].GetType().Name, AllTasksOK[0]);
			Assert.AreEqual(taskArray[1].GetType().Name, AllTasksOK[1]);
			Assert.AreEqual(taskArray[3].GetType().Name, AllTasksOK[2]);

			Assert.AreEqual(taskArray[2].GetType().Name, AllTasksError[0]);
		}

		[Test]
		public void SingleTaskWithoutError()
		{
			TaskOKSingle taskOK = new TaskOKSingle();
			queue.Add(taskOK);

			double msExecTime = waitUntilTasksComplete();

			Assert.IsTrue(msExecTime < msTimeout);

			Assert.IsFalse(queue.IsBusy);

			Assert.AreEqual(1, AllTasksRun.Count);
			Assert.AreEqual(1, AllTasksOK.Count);
			Assert.AreEqual(0, AllTasksError.Count);

			Assert.AreEqual(taskOK.GetType().Name, AllTasksRun[0]);
			Assert.AreEqual(taskOK.GetType().Name, AllTasksOK[0]);

		}

		[Test]
		public void SingleTaskWithError()
		{
			TaskError3 taskError = new TaskError3();
			queue.Add(taskError);

			double msExecTime = waitUntilTasksComplete();

			Assert.IsTrue(msExecTime < msTimeout);

			Assert.IsFalse(queue.IsBusy);

			Assert.AreEqual(1, AllTasksRun.Count);
			Assert.AreEqual(0, AllTasksOK.Count);
			Assert.AreEqual(1, AllTasksError.Count);

			Assert.AreEqual(taskError.GetType().Name, AllTasksRun[0]);
			Assert.AreEqual(taskError.GetType().Name, AllTasksError[0]);
		}

		// returns time waited in milliseconds, should be less than 10000

		private double waitUntilTasksComplete()
		{
			DateTime begin = DateTime.Now;
			double totalMilliseconds = 0.0;

			while (queue.IsBusy)
			{
				Thread.Sleep(100);
				TimeSpan ts = DateTime.Now - begin;
				totalMilliseconds = ts.TotalMilliseconds;

				if (totalMilliseconds >= msTimeout)
				{
					break;
				}
			}

			return totalMilliseconds;
		}

		// Classes used by test

		internal abstract class TaskBase : BackgroundTaskQueue.Task
		{
			public override void RunWorkerCompletedError(BackgroundTaskQueue btq, Exception e)
			{
				lock (AllTasksRun.SyncRoot)
				{
					AllTasksRun.Add(GetType().Name);
					AllTasksError.Add(GetType().Name);
				}
			}
		}

		internal class TaskOK1 : TaskBase
		{
			public override void DoWorkOnBackgroundThread(BackgroundTaskQueue btq, System.ComponentModel.DoWorkEventArgs e)
			{
				// normally we would lock shared data
				e.Result = new object();
			}

			public override void RunWorkerCompletedOk(BackgroundTaskQueue btq, object result)
			{
				if (result != null)
				{
					lock (AllTasksRun.SyncRoot)
					{
						AllTasksRun.Add(GetType().Name);
						AllTasksOK.Add(GetType().Name);
					}
				}
			}
		}

		internal class TaskOK2 : TaskBase
		{
			public override void DoWorkOnBackgroundThread(BackgroundTaskQueue btq, System.ComponentModel.DoWorkEventArgs e)
			{
				// normally we would lock shared data
				e.Result = null;
			}

			public override void RunWorkerCompletedOk(BackgroundTaskQueue btq, object result)
			{
				if (result == null)
				{
					lock (AllTasksRun.SyncRoot)
					{
						AllTasksRun.Add(GetType().Name);
						AllTasksOK.Add(GetType().Name);
					}
				}
			}
		}

		internal class TaskError3 : TaskBase
		{
			public override void DoWorkOnBackgroundThread(BackgroundTaskQueue btq, System.ComponentModel.DoWorkEventArgs e)
			{
				e.Result = null;
				throw new Exception();
			}

			// this shouldn't execute
			public override void RunWorkerCompletedOk(BackgroundTaskQueue btq, object result)
			{
				lock (AllTasksRun.SyncRoot)
				{
					AllTasksRun.Add(GetType().Name);
					AllTasksOK.Add(GetType().Name);
				}
			}
		}

		internal class TaskOK4 : TaskBase
		{
			public override void DoWorkOnBackgroundThread(BackgroundTaskQueue btq, System.ComponentModel.DoWorkEventArgs e)
			{
				// normally we would lock shared data
				e.Result = "foo";
			}

			public override void RunWorkerCompletedOk(BackgroundTaskQueue btq, object result)
			{
				if (result.ToString().CompareTo("foo") == 0)
				{
					lock (AllTasksRun.SyncRoot)
					{
						AllTasksRun.Add(GetType().Name);
						AllTasksOK.Add(GetType().Name);
					}
				}
			}
		}

		// since type names are used to verify what was run...

		internal class TaskOKSingle : TaskOK4
		{
		}

		internal class TaskErrorSingle : TaskError3
		{
		}

		#region IWin32Window Members

		public IntPtr Handle
		{
			get { return IntPtr.Zero; }
		}

		#endregion
	}

}
