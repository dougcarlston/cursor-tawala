package com.tawala.web.admin;

import java.lang.annotation.Annotation;
import java.lang.reflect.Field;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.hibernate.annotations.Cache;
import org.hibernate.annotations.CacheConcurrencyStrategy;
import org.hibernate.stat.CollectionStatistics;
import org.hibernate.stat.EntityStatistics;
import org.hibernate.stat.QueryStatistics;
import org.hibernate.stat.SecondLevelCacheStatistics;
import org.hibernate.stat.Statistics;
import org.springframework.orm.hibernate3.HibernateTemplate;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.hibernate.TawalaSessionFactory;

public class ViewHibernateStatisticsController implements Controller {
	public static final String ENABLE_STATICTICS_PARAMETER = "enable_statistics";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		Statistics statistics = TawalaSessionFactory.MAIN.getFactory()
				.getStatistics();

		String enableStaticstics = request
				.getParameter(ENABLE_STATICTICS_PARAMETER);

		if (enableStaticstics != null) {
			boolean requestedEnabledStatistics = Boolean
					.parseBoolean(enableStaticstics);
			if (requestedEnabledStatistics != statistics.isStatisticsEnabled()) {
				statistics.setStatisticsEnabled(requestedEnabledStatistics);
			}
		}

		ModelAndView result = new ModelAndView("admin.hibernate.staticstics");
		result.addObject("statistics", statistics);
		result.addObject("classInfo", getClassStatistics(statistics));
		result.addObject("collectionsStats",
				getCollectionStatistics(statistics));
		result.addObject("queriesStats", getQueryStatistics(statistics));

		return result;
	}

	private List<QueryStats> getQueryStatistics(Statistics statistics)
			throws ClassNotFoundException, NoSuchFieldException {
		List<QueryStats> queryStats = new ArrayList<QueryStats>();
		String[] queries = statistics.getQueries();
		for (int i = 0; i < queries.length; i++) {
			QueryStats queryStatistics = new QueryStats();
			queryStatistics.queryName = queries[i];
			queryStatistics.statistics = statistics
					.getQueryStatistics(queryStatistics.queryName);

			HibernateTemplate template = TawalaSessionFactory.MAIN
					.getCustomTemplate(queryStatistics.queryName);
			queryStatistics.cached = template.isCacheQueries();
			if (template.isCacheQueries()) {
				if (template.getQueryCacheRegion() != null) {
					queryStatistics.cacheRegion = template.getQueryCacheRegion();
					queryStatistics.secondLevelCacheStatistics = statistics.getSecondLevelCacheStatistics(template.getQueryCacheRegion());
				} else {
					//--- TODO: this is not correct. There are two hardcoded region names.
					queryStatistics.secondLevelCacheStatistics = statistics.getSecondLevelCacheStatistics(queryStatistics.queryName);
				}
			}

			queryStats.add(queryStatistics);
		}

		Collections.sort(queryStats);

		return queryStats;
	}

	@SuppressWarnings("unchecked")
	private List<CollectionStats> getCollectionStatistics(Statistics statistics)
			throws ClassNotFoundException, NoSuchFieldException {
		List<CollectionStats> collectionsStats = new ArrayList<CollectionStats>();
		for (int i = 0; i < statistics.getCollectionRoleNames().length; i++) {
			CollectionStats collectionStats = new CollectionStats();
			collectionStats.roleName = statistics.getCollectionRoleNames()[i];
			collectionStats.collectionStatistics = statistics
					.getCollectionStatistics(collectionStats.roleName);

			int lastDot = collectionStats.roleName.lastIndexOf('.');
			String className = collectionStats.roleName.substring(0, lastDot);
			String collectionName = collectionStats.roleName
					.substring(lastDot + 1);

			Class clazz = Class.forName(className);
			Field field = clazz.getDeclaredField(collectionName);

			Annotation annotation = field.getAnnotation(Cache.class);
			collectionStats.cached = annotation != null;
			if (annotation != null) {
				Cache cache = (Cache) annotation;
				collectionStats.cacheConcurrencyStrategy = cache.usage();
				collectionStats.secondLevelCacheStatistics = statistics
						.getSecondLevelCacheStatistics(cache.region());
				collectionStats.cacheRegion = cache.region();
			}

			collectionsStats.add(collectionStats);
		}

		Collections.sort(collectionsStats);

		return collectionsStats;
	}

	@SuppressWarnings("unchecked")
	private List<ClassStatistics> getClassStatistics(Statistics statistics) {
		List<ClassStatistics> classInfo = new ArrayList<ClassStatistics>();
		Class[] persistentClasses = TawalaSessionFactory.MAIN.getClasses();
		for (int i = 0; i < persistentClasses.length; i++) {
			Class persistentClass = persistentClasses[i];

			ClassStatistics classStatistics = new ClassStatistics();
			classStatistics.clazz = persistentClass;
			classStatistics.entityStatistics = statistics
					.getEntityStatistics(persistentClass.getName());

			Annotation annotation = persistentClass.getAnnotation(Cache.class);
			classStatistics.cached = annotation != null;
			if (annotation != null) {
				Cache cache = (Cache) annotation;
				classStatistics.cacheConcurrencyStrategy = cache.usage();
				classStatistics.secondLevelCacheStatistics = statistics
						.getSecondLevelCacheStatistics(cache.region());
				classStatistics.cacheRegion = cache.region();
			}

			classInfo.add(classStatistics);
		}

		Collections.sort(classInfo);
		return classInfo;
	}

	public static class QueryStats implements Comparable<QueryStats> {
		public String queryName;
		public QueryStatistics statistics;
		public String cacheRegion;
		public SecondLevelCacheStatistics secondLevelCacheStatistics;
		boolean cached;

		public String getCacheRegion() {
			return cacheRegion;
		}

		public boolean isCached() {
			return cached;
		}

		public String getQueryName() {
			return queryName;
		}

		public SecondLevelCacheStatistics getSecondLevelCacheStatistics() {
			return secondLevelCacheStatistics;
		}

		public QueryStatistics getStatistics() {
			return statistics;
		}

		public int compareTo(QueryStats o) {
			return queryName.compareTo(o.queryName);
		}
	}

	public static class CollectionStats implements Comparable<CollectionStats> {
		public SecondLevelCacheStatistics secondLevelCacheStatistics;
		public CacheConcurrencyStrategy cacheConcurrencyStrategy;
		String roleName;
		CollectionStatistics collectionStatistics;
		boolean cached;
		String cacheRegion;

		public String getCacheRegion() {
			return cacheRegion;
		}

		public CollectionStatistics getCollectionStatistics() {
			return collectionStatistics;
		}

		public String getRoleName() {
			return roleName;
		}

		public int compareTo(CollectionStats o) {
			return roleName.compareTo(o.roleName);
		}

		public CacheConcurrencyStrategy getCacheConcurrencyStrategy() {
			return cacheConcurrencyStrategy;
		}

		public boolean isCached() {
			return cached;
		}

		public SecondLevelCacheStatistics getSecondLevelCacheStatistics() {
			return secondLevelCacheStatistics;
		}

	}

	public static class ClassStatistics implements Comparable<ClassStatistics> {
		@SuppressWarnings("unchecked")
		Class clazz;
		EntityStatistics entityStatistics;
		boolean cached;
		CacheConcurrencyStrategy cacheConcurrencyStrategy;
		SecondLevelCacheStatistics secondLevelCacheStatistics;
		String cacheRegion;

		public String getCacheRegion() {
			return cacheRegion;
		}

		public CacheConcurrencyStrategy getCacheConcurrencyStrategy() {
			return cacheConcurrencyStrategy;
		}

		public boolean isCached() {
			return cached;
		}

		@SuppressWarnings("unchecked")
		public Class getClazz() {
			return clazz;
		}

		public EntityStatistics getEntityStatistics() {
			return entityStatistics;
		}

		public SecondLevelCacheStatistics getSecondLevelCacheStatistics() {
			return secondLevelCacheStatistics;
		}

		public int compareTo(ClassStatistics o) {
			return this.getClazz().getName().compareTo(o.getClazz().getName());
		}
	}
}
