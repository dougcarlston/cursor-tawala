package com.tawala.jbpm;

import org.jbpm.job.executor.JobExecutor;

import com.scissor.Log;

public class JobExecutorLauncher {

	public void start() {
		JobExecutor jobExecutor = JbpmService.getJobExecutor();
		if (jobExecutor != null) {
			Log.info(this, "Starting JBPM Job Executor");
			jobExecutor.start();
			Log.info(this, "JBPM Job Executor started");
		} else {
			throw new IllegalStateException("Job Executor is not found");
		}
	}

	public void stop() {
		JobExecutor jobExecutor = JbpmService.getJobExecutor();
		if (jobExecutor != null) {
			Log.info(this, "Attempting to stop JBPM Job Executor");
			jobExecutor.stop();
			Log.info(this, "Stopped JBPM Job Executor");
		} else {
			throw new IllegalStateException("Job Executor is not found");
		}
	}

}
