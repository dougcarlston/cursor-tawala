package com.tawala.project.caching;

import net.sf.ehcache.Cache;
import net.sf.ehcache.CacheException;
import net.sf.ehcache.CacheManager;
import net.sf.ehcache.Element;

import com.scissor.Log;
import com.tawala.project.Project;

public class ProjectRuntimeCache {
	private static final String CACHE_REGION_NAME = "project.runtime";

	public static ProjectRuntimeCache instance = new ProjectRuntimeCache();

	private CacheManager manager;
	private Cache cache;

	private void buildCache(String name)
			throws CacheException {
		cache = manager.getCache(name);

		if (cache == null) {
			Log.warn(this, "Could not find configuration [" + name
					+ "]; using defaults.");

			manager.addCache(name);
			cache = manager.getCache(name);
			Log.debug(this, "started EHCache region: " + name);
		}
	}

	public void start() throws CacheException {
		if (manager != null) {
			Log
					.warn(
							this,
							"Attempt to restart an already started EhCacheProvider. Use sessionFactory.close()  between repeated calls to buildSessionFactory. Using previously created EhCacheProvider. If this behaviour is required, consider using net.sf.ehcache.hibernate.SingletonEhCacheProvider.");
			return;
		}
		manager = new CacheManager();
		buildCache(CACHE_REGION_NAME);
	}

	public void stop() {
		if (manager != null) {
			manager.shutdown();
			manager = null;
		}
	}
	
	public void addProject(Project project) {
		if(cache == null) {
			return;
		}
		project.doDeserializeIfNeeded();
		cache.put(new Element(project.getId(), project));
	}

	public Project getProject(long projectId) {
		if(cache == null) {
			return null;
		}
		Element result = cache.get(projectId);
		if(result == null) {
			return null;
		} else {
			return (Project)result.getObjectValue();
		}
	}
	
	public void removeFromCache(long projectId) {
		if(cache == null) {
			return;
		}
		cache.remove(projectId);
	}
}
