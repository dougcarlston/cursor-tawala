// Copyright © 2005 - 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Tawala.Common
{
	public class BackgroundTaskQueue
	{
		// base class for a task
		public abstract class Task
		{
			// 1) Do not manipulate Windows Forms controls created on the (main) UI thread.
			// 2) Exceptions are automatically trapped
			public abstract void DoWorkOnBackgroundThread(BackgroundTaskQueue btq, DoWorkEventArgs e);

			// Called if task completed without an exception or cancel
			// Runs on UI thread
			public abstract void RunWorkerCompletedOk(BackgroundTaskQueue btq, Object result);

			// Called if task generates an exception
			// Runs on UI thread
			public virtual void RunWorkerCompletedError(BackgroundTaskQueue btq, Exception e)
			{
			}

			// Called if task is cancelled (we don't use but case must be handled)
			// Runs on UI thread
			public void RunWorkerCompletedCancelled(BackgroundTaskQueue btq)
			{
			}
		}

		private ManualResetEvent taskCompletedEvent;
		private ManualResetEvent backgroundMonitorStartedEvent;

		private void backgroundMonitor_DoWork(object sender, DoWorkEventArgs e)
		{
			taskCompletedEvent.WaitOne();
			taskCompletedEvent.Reset();

			tasks.Dequeue();

			backgroundMonitorStartedEvent.Set();

			tasks.WaitQueued();
		}

		void backgroundMonitor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			backgroundWorker.RunWorkerAsync();
			backgroundMonitor.RunWorkerAsync();
		}

		public BackgroundTaskQueue(IWin32Window o)
		{
			owner = o;
			taskCompletedEvent = new ManualResetEvent(true);
			backgroundMonitorStartedEvent = new ManualResetEvent(false);

			backgroundWorker = new WorkerThread(this);

			backgroundMonitor = new MonitorThread(this);
			backgroundMonitor.RunWorkerAsync();

			backgroundMonitorStartedEvent.WaitOne();
		}

		public IWin32Window OwnerForm
		{
			get
			{
				return owner;
			}
		}


		public void Add(Task task)
		{
			Add(new Task[1] { task });
		}

		public void Add(Task[] aTasks)
		{
			tasks.Enqueue(aTasks);
		}

		public bool IsEmpty(int msWait)
		{
			return tasks.IsEmpty(msWait);
		}

		private class TaskQueue
		{
			private Queue<Task> queue = new Queue<Task>();
			private Object queueLock = new Object();
			private ManualResetEvent emptyEvent = new ManualResetEvent(true);
			private ManualResetEvent queuedEvent = new ManualResetEvent(false);

			public void Enqueue(Task[] aTasks)
			{
				lock (queueLock)
				{
					for (int i = 0; i < aTasks.Length; ++i)
					{
						queue.Enqueue(aTasks[i]);
					}

					emptyEvent.Reset();
					queuedEvent.Set();
				}
			}

			public Task Current()
			{
				lock (queueLock)
				{
					return queue.Peek();
				}
			}

			public void Dequeue()
			{
				lock (queueLock)
				{
					if (queue.Count != 0)
					{
						queue.Dequeue();

						if (queue.Count == 0)
						{
							emptyEvent.Set();
							queuedEvent.Reset();
						}
					}
				}
			}

			public void WaitQueued()
			{
				queuedEvent.WaitOne();
			}

			public bool IsEmpty(int msWait)
			{
				return emptyEvent.WaitOne(msWait, false);
			}
		}

		public bool IsBusy  // more like "HasWork"
		{
			get
			{
				return !IsEmpty(0);
			}
		}

		private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			tasks.Current().DoWorkOnBackgroundThread(this, e);
		}

		private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			// Per MS docs must check e.Cancelled before e.Error and both before e.Result.
			// Accessing e.Result if Cancelled is true or Error isn't null causes an exception (according to doc)
			try
			{
//				This should hold true, but not in tests.
//				Debug.Assert(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA);

				if (e.Cancelled)
				{
					tasks.Current().RunWorkerCompletedCancelled(this);
					Debug.WriteLine(tasks.Current().GetType().Name + ": Cancelled (IMPOSSIBLE! Functionality not used)");
				}
				else if (e.Error != null)
				{
					tasks.Current().RunWorkerCompletedError(this, e.Error);
					Debug.WriteLine(tasks.Current().GetType().Name + ": Completed with error - " + e.Error.Message);
				}
				else
				{
					tasks.Current().RunWorkerCompletedOk(this, e.Result);
				}
			}
			catch (Exception ex)
			{
				tasks.Current().RunWorkerCompletedError(this, ex);
				Debug.WriteLine(tasks.Current().GetType().Name + ": Exception in RunWorkCompletedXXX method - " + ex.Message);
			}
		}

		private MonitorThread backgroundMonitor;
		private WorkerThread backgroundWorker;
		private TaskQueue tasks = new TaskQueue();
		private IWin32Window owner = null;

		private class WorkerThread : BackgroundWorker
		{
			private BackgroundTaskQueue taskQueue;

			public WorkerThread(BackgroundTaskQueue taskQueue)
			{
				this.taskQueue = taskQueue;
				DoWork += taskQueue.backgroundWorker_DoWork;
				RunWorkerCompleted += taskQueue.backgroundWorker_RunWorkerCompleted;
			}

			protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
			{
				try
				{
					base.OnRunWorkerCompleted(e);
				}
				finally
				{
					taskQueue.taskCompletedEvent.Set();
				}
			}
		}

		private class MonitorThread : BackgroundWorker
		{
			private BackgroundTaskQueue taskQueue;

			public MonitorThread(BackgroundTaskQueue taskQueue)
			{
				this.taskQueue = taskQueue;
				DoWork += taskQueue.backgroundMonitor_DoWork;
				RunWorkerCompleted += taskQueue.backgroundMonitor_RunWorkerCompleted;
			}
		}
	}
}
