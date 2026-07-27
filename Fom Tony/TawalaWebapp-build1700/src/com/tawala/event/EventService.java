package com.tawala.event;

import com.scissor.Log;
import com.tawala.hibernate.TawalaSessionFactory;

public class EventService {
	public static void createEvent(Event event) {
		try {
			TawalaSessionFactory.MAIN.getHibernateTemplate().save(event);
		} catch (Exception e) {
			Log.error(EventService.class, "Failed to create event:", e);
		}
	}
}
