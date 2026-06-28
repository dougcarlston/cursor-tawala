begin transaction;
    create table project_version (
        project_version_id int8 not null,
        user_project_id int8,
        project_id int8 not null,
        version_number int4 not null,
        description text not null,
        deleted bool not null,
        created_dt timestamp not null,
        primary key (project_version_id)
    );

    alter table project_version 
        add constraint fk_project_version_1
        foreign key (project_id) 
        references project;

    alter table project_version 
        add constraint fk_project_version_2
        foreign key (user_project_id) 
        references user_project on delete cascade;

    alter table project_version 
	add constraint uk_project_version_1 unique (project_id);

    alter table project_version 
	add constraint uk_project_version_2 unique (user_project_id, version_number);
  
    ALTER TABLE user_project ADD COLUMN version int4;
    update user_project set version = 1;
    ALTER TABLE user_project ALTER COLUMN version SET NOT NULL;

    ALTER TABLE user_project ADD COLUMN next_version int4;
    update user_project set next_version = 2;
    ALTER TABLE user_project ALTER COLUMN next_version SET NOT NULL;


    create sequence seq_project_version_id;

    update schema_version set version = 8;

    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;