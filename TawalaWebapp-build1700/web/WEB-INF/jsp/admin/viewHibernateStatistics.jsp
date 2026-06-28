<%@ page contentType="text/html" %>
<jsp:directive.page import="com.tawala.web.admin.ViewHibernateStatisticsController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<div class="section">    
	
	<h3>Enable/Disable Statistics Collection Mode</h3>
	
	<p>In production environment under normal circumstances this mode should be disabled.</p>
	<form id="changeStatisticsMode" method="post">
	    <div>
		<input type="radio" name="<%= ViewHibernateStatisticsController.ENABLE_STATICTICS_PARAMETER %>" value="true" <c:if test="${statistics.statisticsEnabled}">checked</c:if> /> Enable
		<input type="radio" name="<%= ViewHibernateStatisticsController.ENABLE_STATICTICS_PARAMETER %>" value="false" <c:if test="${!statistics.statisticsEnabled}">checked</c:if>/> Disable
	    <input type="submit" value="Change"/> 
		</div>
	</form>
	<br />	
	<a href="${urls.adminViewHibernateStatistics}">[refresh this page]</a>
	<br />
	<ul class="hMenu">
		<li><a href="#GlobalStats">[Global Statistics]</a></li>
		<li><a href="#ClassStats">[Class Statistics]</a></li>
		<li><a href="#CollectionStats">[Collection Statistics]</a></li>
		<li><a href="#QueryStats">[Query Statistics]</a></li>
	</ul>
	<br />
	<h3 id="GlobalStats">Global Statistics</h3>
	<table class="list striped ruler">
		<thead>
			<tr>
				<th>Description</th>
				<th>Value</th>
			</tr>
		</thead>
		<tbody class="left">
			<tr>
				<td>Entity Delete Count</td>
				<td>${statistics.entityDeleteCount}</td>
			</tr>
			<tr>
				<td>Entity Insert Count</td>
				<td>${statistics.entityInsertCount}</td>
			</tr>
			<tr>
				<td>Entity Load Count</td>
				<td>${statistics.entityLoadCount}</td>
			</tr>
			<tr>
				<td>Entity Fetch Count</td>
				<td>${statistics.entityFetchCount}</td>
			</tr>
			<tr>
				<td>Entity Update Count</td>
				<td>${statistics.entityUpdateCount}</td>
			</tr>
			<tr>
				<td>Query Execution Count</td>
				<td>${statistics.queryExecutionCount}</td>
			</tr>
			<tr>
				<td>Query Execution Max Time Query String</td>
				<td>${statistics.queryExecutionMaxTimeQueryString}</td>
			</tr>
			<tr>
				<td>Query Cache Hit Count</td>
				<td>${statistics.queryCacheHitCount}</td>
			</tr>
			<tr>
				<td>Query Cache Miss Count</td>
				<td>${statistics.queryCacheMissCount}</td>
			</tr>
			<tr>
				<td>Query Cache Put Count</td>
				<td>${statistics.queryCachePutCount}</td>
			</tr>
			<tr>
				<td>Flush Count</td>
				<td>${statistics.flushCount}</td>
			</tr>
			<tr>
				<td>Connect Count</td>
				<td>${statistics.connectCount}</td>
			</tr>
			<tr>
				<td>Second Level Cache Hit Count</td>
				<td>${statistics.secondLevelCacheHitCount}</td>
			</tr>
			<tr>
				<td>Second Level Cache Miss Count</td>
				<td>${statistics.secondLevelCacheMissCount}</td>
			</tr>
			<tr>
				<td>Second Level Cache Put Count</td>
				<td>${statistics.secondLevelCachePutCount}</td>
			</tr>
			<tr>
				<td>Session Close Count</td>
				<td>${statistics.sessionCloseCount}</td>
			</tr>
			<tr>
				<td>Session Open Count</td>
				<td>${statistics.sessionOpenCount}</td>
			</tr>
			<tr>
				<td>Collection Load Count</td>
				<td>${statistics.collectionLoadCount}</td>
			</tr>
			<tr>
				<td>Collection Fetch Count</td>
				<td>${statistics.collectionFetchCount}</td>
			</tr>
			<tr>
				<td>Collection Update Count</td>
				<td>${statistics.collectionUpdateCount}</td>
			</tr>
			<tr>
				<td>Collection Remove Count</td>
				<td>${statistics.collectionRemoveCount}</td>
			</tr>
			<tr>
				<td>Collection Recreate Count</td>
				<td>${statistics.collectionRecreateCount}</td>
			</tr>
			<tr>
				<td>Start Time</td>
				<td>${statistics.startTime}</td>
			</tr>
			<tr>
				<td>Successful Transaction Count</td>
				<td>${statistics.successfulTransactionCount}</td>
			</tr>
			<tr>
				<td>Transaction Count</td>
				<td>${statistics.transactionCount}</td>
			</tr>
			<tr>
				<td>Prepared Statement Count</td>
				<td>${statistics.prepareStatementCount}</td>
			</tr>
			<tr>
				<td>Close Statement Count</td>
				<td>${statistics.closeStatementCount}</td>
			</tr>
			<tr>
				<td>Optimistic Failure Count</td>
				<td>${statistics.optimisticFailureCount}</td>
			</tr>
		</tbody>
	</table>
</div>
<br />	
<div class="section">    
	<h3 id="ClassStats">Class Statistics</h3>
	<table class="list striped ruler">
		<thead>
			<tr>
				<th>Class Stats</th>
				<th>Cache Stats</th>
			</tr>
		</thead>
		<tbody class="left">
			<c:forEach var="classStatistics" items="${classInfo}">
			<tr>
				<td style="border-bottom: thin; border-bottom-style: dashed; border-bottom-color: gray" colspan="2"><font color="blue">${classStatistics.clazz.name}</font></td>
			</tr>
			<tr>
				<td>loadCount: ${classStatistics.entityStatistics.loadCount}<br />
					updateCount: ${classStatistics.entityStatistics.updateCount}<br />
					insertCount: ${classStatistics.entityStatistics.insertCount}<br />
					deleteCount: ${classStatistics.entityStatistics.deleteCount}<br />
					fetchCount: ${classStatistics.entityStatistics.fetchCount}<br />
					optimisticLockFailureCount: ${classStatistics.entityStatistics.optimisticFailureCount}
				</td>
				<td >
				<c:choose>
					<c:when test="${classStatistics.cached}">
					Region: ${classStatistics.cacheRegion}<br />
					Strategy: ${classStatistics.cacheConcurrencyStrategy}<br />
					hitCount: ${classStatistics.secondLevelCacheStatistics.hitCount}<br />
					missCount: ${classStatistics.secondLevelCacheStatistics.missCount}<br />
					putCount: ${classStatistics.secondLevelCacheStatistics.putCount}<br />
					elementCountInMemory: ${classStatistics.secondLevelCacheStatistics.elementCountInMemory}<br />
					elementCountOnDisk: ${classStatistics.secondLevelCacheStatistics.elementCountOnDisk}<br />
					sizeInMemory: ${classStatistics.secondLevelCacheStatistics.sizeInMemory}
					</c:when>
					<c:otherwise><span class="red">Not cached</span>
					</c:otherwise>
				</c:choose>
				</td>
			</tr>
			</c:forEach>
		</tbody>
	</table>
</div>
<br />	
<div class="section">    
	<h3 id="CollectionStats">Collection Statistics</h3>
	<table class="list striped ruler left" >
		<thead>
			<tr>
				<th>Collection Stats</th>
				<th>Cache Stats</th>
			</tr>
		</thead>
		<tbody class="left">
			<c:forEach var="collectionStats" items="${collectionsStats}">
			<c:set var="stats" value="${collectionStats.collectionStatistics}"/>
			<tr>
				<td stlye="border-bottom: thin; border-bottom-style: dashed; border-bottom-color: gray" colspan="2"><font color="blue">${collectionStats.roleName}</font></td>
			</tr>
			<tr>
				<td>loadCount: ${stats.loadCount}<br />
					updateCount: ${stats.updateCount}<br />
					recreateCount: ${stats.recreateCount}<br />
					removeCount: ${stats.removeCount}<br />
					fetchCount: ${stats.fetchCount}<br />
				</td>
				<td>
				<c:choose>
					<c:when test="${collectionStats.cached}">
					Region: ${collectionStats.cacheRegion}<br />
					Strategy: ${collectionStats.cacheConcurrencyStrategy}<br />
					hitCount: ${collectionStats.secondLevelCacheStatistics.hitCount}<br />
					missCount: ${collectionStats.secondLevelCacheStatistics.missCount}<br />
					putCount: ${collectionStats.secondLevelCacheStatistics.putCount}<br />
					elementCountInMemory: ${collectionStats.secondLevelCacheStatistics.elementCountInMemory}<br />
					elementCountOnDisk: ${collectionStats.secondLevelCacheStatistics.elementCountOnDisk}<br />
					sizeInMemory: ${collectionStats.secondLevelCacheStatistics.sizeInMemory}
					</c:when>
					<c:otherwise><span class="red">Not cached</span>
					</c:otherwise>
				</c:choose>
				</td>
			</tr>
			</c:forEach>
		</tbody>
	</table>
</div>

<br />	
<div class="section">    
	<h3 id="QueryStats">Query Statistics</h3>
	<table class="list striped ruler left" >
		<thead>
			<tr>
				<th>Query Stats</th>
				<th>Cache Stats</th>
			</tr>
		</thead>
		<tbody class="left">
			<c:forEach var="queryStats" items="${queriesStats}">
			<c:set var="stats" value="${queryStats.statistics}"/>
			<tr>
				<td stlye="border-bottom: thin; border-bottom-style: dashed; border-bottom-color: gray" colspan="2"><font color="blue">${queryStats.queryName}</font></td>
			</tr>
			<tr>
				<td>cacheHitCount: ${stats.cacheHitCount}<br />
					cacheMissCount: ${stats.cacheMissCount}<br />
					cachePutCount: ${stats.cachePutCount}<br />
					executionCount: ${stats.executionCount}<br />
					executionRowCount: ${stats.executionRowCount}<br />
					executionAvgTime: ${stats.executionAvgTime}<br />
					executionMaxTime: ${stats.executionMaxTime}<br />
					executionMinTime: ${stats.executionMinTime}<br />
				</td>
				<td>
				<c:choose>
					<c:when test="${queryStats.cached}">
					Region: ${queryStats.cacheRegion}<br />
					hitCount: ${queryStats.secondLevelCacheStatistics.hitCount}<br />
					missCount: ${queryStats.secondLevelCacheStatistics.missCount}<br />
					putCount: ${queryStats.secondLevelCacheStatistics.putCount}<br />
					elementCountInMemory: ${queryStats.secondLevelCacheStatistics.elementCountInMemory}<br />
					elementCountOnDisk: ${queryStats.secondLevelCacheStatistics.elementCountOnDisk}<br />
					sizeInMemory: ${queryStats.secondLevelCacheStatistics.sizeInMemory}
					</c:when>
					<c:otherwise><span class="red">Not cached</span>
					</c:otherwise>
				</c:choose>
				</td>
			</tr>
			</c:forEach>
		</tbody>
	</table>
</div>
