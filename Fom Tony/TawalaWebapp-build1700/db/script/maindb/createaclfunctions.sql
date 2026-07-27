-- $Id: createaclfunctions.sql 3906 2008-06-05 03:51:03Z sergei $
--
-- dba utility functions for access control / admin
--
-- Portions Copyright (c) 2004, Afilias Canada Corporation
--
-- Permission to use, copy, modify, and distribute this software and its
-- documentation for any purpose, without fee, and without a written agreement
-- is hereby granted, provided that the above copyright notice and this
-- paragraph and the following two paragraphs appear in all copies.
-- 
-- IN NO EVENT SHALL AFILIAS CANADA CORPORATION BE LIABLE TO ANY PARTY FOR
-- DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES, INCLUDING
-- LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
-- DOCUMENTATION, EVEN IF AFILIAS CANADA CORPORATION HAS BEEN ADVISED OF THE
-- POSSIBILITY OF SUCH DAMAGE.
-- 
-- AFILIAS CANADA CORPORATION SPECIFICALLY DISCLAIMS ANY WARRANTIES,
-- INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
-- AND FITNESS FOR A PARTICULAR PURPOSE.  THE SOFTWARE PROVIDED HEREUNDER IS
-- ON AN "AS IS" BASIS, AND THE UNIVERSITY OF CALIFORNIA HAS NO OBLIGATIONS TO
-- PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.

--DROP SCHEMA acl_admin CASCADE;
CREATE SCHEMA acl_admin;
COMMENT ON SCHEMA acl_admin
IS 'Functions and aggregates to facilitate access control administration.';

CREATE OR REPLACE FUNCTION acl_admin.comma_append (text, text)
RETURNS TEXT CALLED ON NULL INPUT VOLATILE
AS 'SELECT COALESCE($1 || '','' || $2, $2)::text'
LANGUAGE 'sql';
COMMENT ON FUNCTION acl_admin.comma_append (text, text)
IS 'Function to append text with commas between them. If given a null first parameter, will return the second parameter without prepending a comma.';

CREATE AGGREGATE acl_admin.comma_list (
    BASETYPE = text,
    STYPE = text,
    SFUNC = acl_admin.comma_append
);
COMMENT ON AGGREGATE acl_admin.comma_list (text)
IS 'Aggregate to join a list of text values into commas.
Used by members and groups functions below.';

CREATE OR REPLACE FUNCTION acl_admin.username_to_userid (text)
RETURNS oid STRICT VOLATILE AS
'SELECT usesysid FROM pg_catalog.pg_user u WHERE u.usename = $1'
LANGUAGE 'sql';
COMMENT ON FUNCTION acl_admin.username_to_userid (text)
IS 'Utility function to translate username to userid.';

CREATE OR REPLACE FUNCTION acl_admin.members(text)
RETURNS TEXT STRICT VOLATILE AS 
'SELECT acl_admin.comma_list(u.usename)
FROM pg_catalog.pg_user u,
    pg_catalog.pg_group g
WHERE g.groname = $1
  AND u.usesysid = ANY(g.grolist)'
LANGUAGE 'sql';
COMMENT ON FUNCTION acl_admin.members (text)
IS 'Given a group name, return a comma separated list of users in that group.';

CREATE OR REPLACE FUNCTION acl_admin.groups(text)
RETURNS TEXT STRICT VOLATILE AS
'SELECT acl_admin.comma_list(g.groname)
FROM pg_catalog.pg_group g
WHERE acl_admin.username_to_userid($1) = ANY(g.grolist)'
LANGUAGE 'sql';
COMMENT ON FUNCTION acl_admin.groups(text)
IS 'Given a username, return a comma separated list of groups to which the user belongs.';

CREATE OR REPLACE VIEW acl_admin.schema_and_relation_view AS 
SELECT n.oid  AS schema_oid,
    c.oid     AS relation_oid,
    n.nspname AS schema_name,
    c.relname AS relation_name    
FROM pg_catalog.pg_class c, pg_catalog.pg_namespace n
WHERE c.relnamespace = n.oid
  AND NOT c.relkind = 'i'
  AND NOT n.nspname LIKE 'pg_%' 
  AND NOT n.nspname = 'information_schema';
COMMENT ON VIEW acl_admin.schema_and_relation_view
IS 'A view of all existing schemas and relations within them (excluding indices).';


CREATE OR REPLACE FUNCTION acl_admin.grant_on_all(text, text)
RETURNS TEXT STRICT VOLATILE AS '
DECLARE
    usr ALIAS FOR $1;
    prv ALIAS FOR $2;
    rel record;
    sql text;
BEGIN
    FOR rel IN SELECT pg_catalog.quote_ident(schema_name) AS schema_name,
                      pg_catalog.quote_ident(relation_name) AS relation_name
        FROM acl_admin.schema_and_relation_view
    LOOP sql := ''GRANT '' || prv || '' ON '' || rel.schema_name || ''.'' || rel.relation_name || '' TO '' || usr;
        RAISE NOTICE ''%'', sql;
        EXECUTE sql;
    END LOOP;
    RETURN ''OK'';
END;
' LANGUAGE 'plpgsql';
COMMENT ON FUNCTION acl_admin.grant_on_all(text,text)
IS 'Given a user name and privilidge, execute GRANT privilidge ON all relations in all schemas TO user.';

CREATE OR REPLACE FUNCTION acl_admin.grant_on_all(text)
RETURNS TEXT STRICT VOLATILE AS
'SELECT acl_admin.grant_on_all($1, ''ALL''::text);'
LANGUAGE 'sql';
COMMENT ON FUNCTION acl_admin.grant_on_all(text)
IS 'Given a user name, execute GRANT ALL ON all relations in all schemas TO user.';

CREATE OR REPLACE FUNCTION acl_admin.grant_on_visible(text,text)
RETURNS TEXT STRICT VOLATILE AS '
DECLARE
    usr ALIAS FOR $1;
    prv ALIAS FOR $2;
    rel record;
    sql text;
BEGIN
    FOR rel IN SELECT pg_catalog.quote_ident(schema_name) AS schema_name,
                      pg_catalog.quote_ident(relation_name) AS relation_name
        FROM acl_admin.schema_and_relation_view
    WHERE pg_catalog.pg_table_is_visible(relation_oid)
    LOOP sql := ''GRANT '' || prv || '' ON '' || rel.schema_name || ''.'' || rel.relation_name || '' TO '' || usr;
        RAISE NOTICE ''%'', sql;
        EXECUTE sql;
    END LOOP;
    RETURN ''OK'';
END;
' LANGUAGE 'plpgsql';
COMMENT ON FUNCTION acl_admin.grant_on_visible(text,text)
IS 'Given a user name and privilidge, execute GRANT privilidge ON all relations in visible schemas TO user.';

CREATE OR REPLACE FUNCTION acl_admin.revoke_on_visible(text,text)
RETURNS TEXT STRICT VOLATILE AS '
DECLARE
    usr ALIAS FOR $1;
    prv ALIAS FOR $2;
    rel record;
    sql text;
BEGIN
    FOR rel IN SELECT pg_catalog.quote_ident(schema_name) AS schema_name,
                      pg_catalog.quote_ident(relation_name) AS relation_name
        FROM acl_admin.schema_and_relation_view
    WHERE pg_catalog.pg_table_is_visible(relation_oid)
    LOOP sql := ''REVOKE '' || prv || '' ON '' || rel.schema_name || ''.'' || rel.relation_name || '' FROM '' || usr;
        RAISE NOTICE ''%'', sql;
        EXECUTE sql;
    END LOOP;
    RETURN ''OK'';
END;
' LANGUAGE 'plpgsql';
COMMENT ON FUNCTION acl_admin.revoke_on_visible(text,text)
IS 'Given a user name and privilidge, execute REVOKE privilidge ON all relations in visible schemas FROM user.';

CREATE OR REPLACE FUNCTION acl_admin.grant_on_visible(text)
RETURNS TEXT STRICT VOLATILE AS
'SELECT acl_admin.grant_on_visible($1, ''ALL''::text)'
LANGUAGE 'sql';
COMMENT ON FUNCTION acl_admin.grant_on_visible(text)
IS 'Given a user name and privilidge, execute GRANT ALL ON all relations in visible schemas TO user.';

CREATE OR REPLACE FUNCTION acl_admin.revoke_on_visible(text)
RETURNS TEXT STRICT VOLATILE AS
'SELECT acl_admin.revoke_on_visible($1, ''ALL''::text)'
LANGUAGE 'sql';
COMMENT ON FUNCTION acl_admin.revoke_on_visible(text)
IS 'Given a user name and privilidge, execute REVOKE ALL ON all relations in visible schemas FROM user.';

CREATE OR REPLACE FUNCTION acl_admin.revoke_on_all(text, text)
RETURNS TEXT STRICT VOLATILE AS '
DECLARE
    usr ALIAS FOR $1;
    prv ALIAS FOR $2;
    rel record;
    sql text;
BEGIN
    FOR rel IN SELECT pg_catalog.quote_ident(schema_name) AS schema_name,
                      pg_catalog.quote_ident(relation_name) AS relation_name
        FROM acl_admin.schema_and_relation_view
    LOOP sql := ''REVOKE '' || prv || '' ON '' || rel.schema_name || ''.'' || rel.relation_name || '' FROM '' || usr;
        RAISE NOTICE ''%'', sql;
        EXECUTE sql;
    END LOOP;
    RETURN ''OK'';
END;
' LANGUAGE 'plpgsql';
COMMENT ON FUNCTION acl_admin.revoke_on_all(text,text)
IS 'Given a user name and privilidge, execute REVOKE privilidge ON all relations in all schemas FROM user.';

CREATE OR REPLACE FUNCTION acl_admin.revoke_on_all(text)
RETURNS TEXT STRICT VOLATILE AS
'SELECT acl_admin.revoke_on_all($1, ''ALL''::text);'
LANGUAGE 'sql';
COMMENT ON FUNCTION acl_admin.revoke_on_all(text)
IS 'Given a user name, execute REVOKE ALL ON all relations in all schemas FROM user.';
 
CREATE OR REPLACE FUNCTION acl_admin.chown_all(text)
RETURNS TEXT STRICT VOLATILE AS '
DECLARE
    usr ALIAS FOR $1;
    rel record;
    sql text;
BEGIN
    FOR rel IN SELECT pg_catalog.quote_ident(schema_name) AS schema_name,
                      pg_catalog.quote_ident(relation_name) AS relation_name
        FROM acl_admin.schema_and_relation_view
    LOOP sql := ''ALTER TABLE '' || rel.schema_name || ''.'' || rel.relation_name || '' OWNER TO '' || usr;
        RAISE NOTICE ''%'', sql;
        EXECUTE sql;
    END LOOP;
    RETURN ''OK'';
END;
' LANGUAGE 'plpgsql';
COMMENT ON FUNCTION acl_admin.chown_all(text)
IS 'Given a user name, execute ALTER TABLE on all relations in all schemas OWNER TO user.';