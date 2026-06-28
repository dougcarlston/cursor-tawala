begin transaction; 
    create table user_project_link (
        user_project_link_id varchar(20) not null,
        is_authenticated bool not null,
        authentication_token text,
        user_project_id int8 not null,
	created_dt timestamp not null default now(),
        primary key (user_project_link_id)
    );

    alter table user_project_link 
        add constraint FK_project_link_1 
        foreign key (user_project_id) 
        references user_project on delete cascade;

    insert into user_project_link (
       select random_id, false, null, up.user_project_id, up.created_dt 
		from project p, user_project up where p.project_id = up.project_id);

    alter table project drop random_id;

    alter table user_project_link add use_once bool;
    update user_project_link set use_once = false;
    alter table user_project_link alter use_once set not null;
    
    alter table user_project add unique_random_id varchar(20);
    update user_project set unique_random_id = 
	(select user_project_link_id from user_project_link l where l.user_project_id = user_project.user_project_id);
    alter table user_project alter unique_random_id set not null;

    update schema_version set version = 27;
    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;