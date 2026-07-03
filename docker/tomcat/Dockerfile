# Tawala Java backend — Tomcat 7 + JDK 8 (Spring 2.x / Hibernate 3 era).
# Expects ROOT.war in the repo root (run TawalaWebapp-build1700/scripts/build-root-war.sh first).

FROM tomcat:7-jdk8

RUN rm -rf /usr/local/tomcat/webapps/*

COPY docker/tomcat/entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

COPY ROOT.war /tmp/ROOT.war
COPY docker/tomcat/dbconnection.properties /tmp/patch/dbconnection.properties
COPY docker/tomcat/jbpm.hibernate.properties /tmp/patch/jbpm.hibernate.properties
COPY docker/tomcat/lucene-local-setup.properties /tmp/patch/lucene-local-setup.properties
COPY docker/tomcat/userproject.properties /tmp/patch/userproject.properties
COPY docker/tomcat/sports-dashboard.properties /tmp/patch/sports-dashboard.properties
COPY docker/tomcat/css /tmp/patch/css
COPY docker/tomcat/images /tmp/patch/images

RUN mkdir -p /tmp/ROOT \
 && unzip -q /tmp/ROOT.war -d /tmp/ROOT \
 && rm -f /tmp/ROOT/WEB-INF/lib/postgresql-8.1*.jar \
 && cp /tmp/patch/*.properties /tmp/ROOT/WEB-INF/classes/ \
 && mkdir -p /tmp/ROOT/css/project /tmp/ROOT/images \
 && cp -r /tmp/patch/css/project /tmp/ROOT/css/ \
 && cp -r /tmp/patch/images/silk /tmp/ROOT/images/ \
 && cp /tmp/patch/images/submit-progress.gif /tmp/ROOT/images/ \
 && cd /tmp/ROOT && jar cf /usr/local/tomcat/webapps/ROOT.war . \
 && rm -rf /tmp/ROOT /tmp/ROOT.war /tmp/patch

ENV CATALINA_OPTS="-Dlucene.use.in.process=true -Xms256m -Xmx1024m"

EXPOSE 8080

HEALTHCHECK --interval=15s --timeout=10s --start-period=120s --retries=8 \
  CMD curl -fsS http://localhost:8080/ >/dev/null || exit 1

ENTRYPOINT ["/entrypoint.sh"]
