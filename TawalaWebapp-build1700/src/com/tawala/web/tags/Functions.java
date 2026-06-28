package com.tawala.web.tags;

import java.util.Set;

public class Functions {
	public static boolean contains(Set<Object> set, Object object) {
		return set == null ? false : set.contains(object);
	}
}
