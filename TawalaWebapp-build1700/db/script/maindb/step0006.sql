begin transaction;
    create table user_project (
        user_project_id int8 not null,
        name varchar(100) not null,
        lib_project_id int8,
        lib_version_number int4,
        created_dt timestamp not null,
        user_id int8 not null,
        project_id int8 not null,
        primary key (user_project_id),
        unique (user_id, project_id)
    );

    alter table user_project 
        add constraint FK_user_project_1 
        foreign key (user_id) 
        references users ON DELETE CASCADE;

    alter table user_project 
        add constraint FK_user_project_2 
        foreign key (project_id) 
        references project ON DELETE CASCADE;

    create sequence seq_user_project_id;

    CREATE OR REPLACE FUNCTION convert_user_project() RETURNS void AS $$
	DECLARE 
		project_record RECORD;
        BEGIN
		FOR project_record IN select project_id, user_id, name, lib_project_id, lib_version_number, created_dt from project LOOP
			insert into user_project (user_project_id, project_id, user_id, name, lib_project_id, lib_version_number, created_dt) VALUES
				( nextval('seq_user_project_id'), project_record.project_id, 
				project_record.user_id, project_record.name, project_record.lib_project_id, project_record.lib_version_number, project_record.created_dt);
		END LOOP;
		RETURN;
        END;
    $$ LANGUAGE plpgsql;

    SELECT convert_user_project();
    DROP FUNCTION convert_user_project();
    /* insert into user_project (user_project_id, project_id, user_id, name, lib_project_id, lib_version_number, created_dt)
      (select nextval('seq_user_project_id'), project_id, user_id, name, lib_project_id, lib_version_number, created_dt from project);
	*/

    alter table project drop user_id, drop name, drop lib_project_id, drop lib_version_number;

    alter table project add column temp_id int8;

    select setval('seq_project_id', (select max(project_id) + 1 from project));

    CREATE OR REPLACE FUNCTION convert_version() RETURNS void AS $$
	DECLARE 
		project_record RECORD;
        BEGIN

    FOR project_record IN select project_definition, created_dt, lib_project_version_id from lib_project_version LOOP
	insert into project VALUES
	( nextval('seq_project_id'), 1, 3, project_record.project_definition, 'default', project_record.created_dt, project_record.lib_project_version_id);
    END LOOP;
		RETURN;
        END;
    $$ LANGUAGE plpgsql;

    SELECT convert_version();
    DROP FUNCTION convert_version();
    /*
	insert into project (select nextval('seq_project_id'), 1, 3, project_definition, 'default', created_dt, lib_project_version_id from lib_project_version);
	*/

    alter table lib_project_version add column project_id int8;

    update lib_project_version set project_id = (select project_id from project where temp_id = lib_project_version.lib_project_version_id);

    alter table lib_project_version alter column project_id set not null;

    alter table project drop column temp_id;

    alter table lib_project_version drop column project_definition;

    alter table lib_project_version 
        add constraint FK_lib_project_version_2 
        foreign key (project_id) 
        references project;

    update schema_version set version = 6;

    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;