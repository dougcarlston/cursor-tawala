package com.tawala.util;

import java.util.Calendar;
import java.util.Date;

public class DateUtil {

	public static Date dateEarlierStartingAt12am(int daysEarlier) {
		Calendar calendar = Calendar.getInstance();
		calendar.add(Calendar.DAY_OF_YEAR, -daysEarlier);
		calendar.set(Calendar.HOUR_OF_DAY, 0);
		calendar.set(Calendar.MINUTE, 0);
		calendar.set(Calendar.SECOND, 0);
		calendar.set(Calendar.MILLISECOND, 0);
	
		Date startTime = calendar.getTime();
		return startTime;
	}

}
