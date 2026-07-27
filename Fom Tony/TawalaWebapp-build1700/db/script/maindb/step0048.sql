begin transaction;
    create sequence seq_user_project_backup_schedule_id;

    create table user_project_backup_schedule (
        user_project_backup_schedule_id int8 not null,
        schedule_type varchar(30) not null,
        next_dt timestamp not null,
        created_dt timestamp not null,
        hour int4,
        minute int4,
        user_project_id bigint not null,
        primary key (user_project_backup_schedule_id)
    );

    alter table user_project_backup_schedule 
        add constraint FK_user_project_backup_schedule_user_project
        foreign key (user_project_id) 
        references user_project on delete cascade;

	update schema_version set version = 48;
	select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;