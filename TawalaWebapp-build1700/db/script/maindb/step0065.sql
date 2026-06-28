begin transaction;
    create sequence seq_project_group_id;

    create table project_group (
        project_group_id int8 not null,
        user_id int8 not null,
        name varchar(200) not null,
        primary key (project_group_id),
        unique (user_id, name)
    );

    create table project_group_member (
        project_group_id int8 not null,
        user_project_id int8 not null
    );
    
    alter table project_group_member 
        add constraint FK_project_group_member_project_group_id 
        foreign key (project_group_id) 
        references project_group ON DELETE CASCADE;

    alter table project_group_member 
        add constraint FK_project_group_member_user_project_id 
        foreign key (user_project_id) 
        references user_project ON DELETE CASCADE;
        
    alter table project_group 
        add constraint FK_project_group_user_id 
        foreign key (user_id) 
        references users ON DELETE CASCADE;

	update schema_version set version = 65;

    select acl_admin.grant_on_visible('tawala_user', 'ALL');

commit;