package com.tawala.web.library;

import javax.servlet.http.HttpSession;

import com.scissor.Log;
import com.tawala.World;
import com.tawala.project.Project;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.project.ProjectTestDriveController;

public class TestDriveSupport {

	/** After the /t/ tab hides, discard the World unless a newer /t/ request arrives. */
	public static final long LEAVE_GRACE_MS = 20 * 1000L;
	/** No /t/ or testdrive touch for this long → discard (tab left open but idle). */
	public static final long IDLE_MS = 30 * 60 * 1000L;

	public static final String ATTR_EPOCH = "testdriveEpoch";
	public static final String ATTR_LEAVE_AT = "testdriveLeaveAt";
	public static final String ATTR_LAST_ACTIVE = "testdriveLastActive";

	/**
	 * Intentionally empty (owner Sep 14, 2026 — Library B1).
	 *
	 * The session World already has the project definition ({@code UserProject}
	 * put into the World). Copying {@code FormSubmission} rows from
	 * {@code WorldInitializer.getDefaultWorld()} cloned live {@code /p/} data
	 * (questions, sign-ups, exam answers) into every new {@code /t/} Test Drive.
	 * A fresh drive must start empty so two visitors never inherit the public
	 * pile or each other.
	 *
	 * Content products that need the author’s questions in the sandbox are a
	 * later Publish rule — not this snapshot.
	 */
	public static void copyExistingSubmissionsToTestWorld(Project project,
			World testDriveWorld) {
	}

	public static void discardIfDue(HttpSession session) {
		if (session == null) {
			return;
		}
		long now = System.currentTimeMillis();
		Long leaveAt = (Long) session.getAttribute(ATTR_LEAVE_AT);
		if (leaveAt != null && now >= leaveAt.longValue()) {
			clearWorld(session);
			return;
		}
		Long last = (Long) session.getAttribute(ATTR_LAST_ACTIVE);
		if (last != null && now - last.longValue() >= IDLE_MS) {
			clearWorld(session);
		}
	}

	public static void onWorldCreated(HttpSession session) {
		if (session == null) {
			return;
		}
		session.setAttribute(ATTR_EPOCH, Integer.valueOf(1));
		session.setAttribute(ATTR_LAST_ACTIVE, Long.valueOf(System
				.currentTimeMillis()));
		session.removeAttribute(ATTR_LEAVE_AT);
	}

	public static void onTestDrivePage(HttpSession session) {
		if (session == null) {
			return;
		}
		discardIfDue(session);
		if (session.getAttribute(ProjectTestDriveController.TESTWORLD_ATTRIBUTE_NAME) == null) {
			return;
		}
		Integer ep = (Integer) session.getAttribute(ATTR_EPOCH);
		int next = ep == null ? 1 : ep.intValue() + 1;
		session.setAttribute(ATTR_EPOCH, Integer.valueOf(next));
		session.setAttribute(ATTR_LAST_ACTIVE, Long.valueOf(System
				.currentTimeMillis()));
		session.removeAttribute(ATTR_LEAVE_AT);
	}

	public static int currentEpoch(HttpSession session) {
		if (session == null) {
			return 0;
		}
		Integer ep = (Integer) session.getAttribute(ATTR_EPOCH);
		return ep == null ? 0 : ep.intValue();
	}

	public static void scheduleLeave(HttpSession session, String epochParam) {
		if (session == null) {
			return;
		}
		if (session.getAttribute(ProjectTestDriveController.TESTWORLD_ATTRIBUTE_NAME) == null) {
			return;
		}
		int client = parseEpoch(epochParam);
		int current = currentEpoch(session);
		if (client != current) {
			Log.warn(TestDriveSupport.class, "testdrive leave ignored epoch client="
					+ client + " session=" + current);
			return;
		}
		session.setAttribute(ATTR_LEAVE_AT, Long.valueOf(System
				.currentTimeMillis() + LEAVE_GRACE_MS));
		Log.info(TestDriveSupport.class, "testdrive leave scheduled epoch=" + current);
	}

	public static void keepAlive(HttpSession session, String epochParam) {
		if (session == null) {
			return;
		}
		if (session.getAttribute(ProjectTestDriveController.TESTWORLD_ATTRIBUTE_NAME) == null) {
			return;
		}
		int client = parseEpoch(epochParam);
		if (client != 0 && client != currentEpoch(session)) {
			return;
		}
		session.setAttribute(ATTR_LAST_ACTIVE, Long.valueOf(System
				.currentTimeMillis()));
		session.removeAttribute(ATTR_LEAVE_AT);
	}

	private static int parseEpoch(String raw) {
		if (raw == null || raw.length() == 0) {
			return 0;
		}
		try {
			return Integer.parseInt(raw.trim());
		} catch (NumberFormatException e) {
			return 0;
		}
	}

	private static void clearWorld(HttpSession session) {
		session.removeAttribute(ProjectTestDriveController.TESTWORLD_ATTRIBUTE_NAME);
		session.removeAttribute(ATTR_EPOCH);
		session.removeAttribute(ATTR_LEAVE_AT);
		session.removeAttribute(ATTR_LAST_ACTIVE);
		ExecutionContext.purgeTestDriveSessionData(session);
	}

	/** Drop the current Test Drive World (switching to another project, or force reinit). */
	public static void discardNow(HttpSession session) {
		if (session != null) {
			clearWorld(session);
		}
	}
}
