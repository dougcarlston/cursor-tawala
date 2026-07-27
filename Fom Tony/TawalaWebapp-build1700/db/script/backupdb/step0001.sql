create table schema_version	(
	version int8 NOT NULL
);

INSERT INTO schema_version VALUES (1);

select acl_admin.grant_on_visible('tawala_user', 'ALL');
