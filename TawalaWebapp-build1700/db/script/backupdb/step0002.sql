begin transaction;
    create sequence seq_user_project_backup_id;

    create table user_project_backup (
        user_project_backup_id int8 not null,
        user_id int8 not null,
        user_project_id int8 not null,
        project_name varchar(100) not null,
        project_version_number int4 not null,
        record_count int4 not null,
        backup_size int4 not null,
        create_dt timestamp not null,
        backup_data oid,
        primary key (user_project_backup_id)
	);

	update schema_version set version = 2;
	select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;