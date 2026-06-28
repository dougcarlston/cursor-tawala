begin transaction;

    create table role (
        role_id varchar(50) not null,
        description varchar(255),
        primary key (role_id)
    );

    create table user_role (
        user_id int8 not null,
        role_id varchar(50) not null,
        primary key (user_id, role_id)
    );

    alter table user_role 
        add constraint FK_user_role_role 
        foreign key (role_id) 
        references role on delete cascade;

    alter table user_role 
        add constraint FK_user_role_users 
        foreign key (user_id) 
        references users on delete cascade;

select acl_admin.grant_on_visible('tawala_user', 'ALL');

update schema_version set version = 59;

commit;