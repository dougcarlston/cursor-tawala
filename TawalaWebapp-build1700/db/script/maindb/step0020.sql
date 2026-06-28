begin transaction;  
    create sequence seq_domain_id start with 1;

    create table domain (
        domain_id int8 not null,
        name varchar(120) not null unique,
        title varchar(120) not null,
        subtitle varchar(120) not null,
        desc_caption varchar(120) not null,
        description text not null,
        suggestion text not null,
        primary key (domain_id)
    );

    create table domain_lib_project (
        domain_id int8 not null,
        lib_project_id int8 not null,
        index int4 not null,
        primary key (domain_id, index)
    );

    alter table domain_lib_project 
        add constraint FK_domain_lib_project_1 
        foreign key (domain_id) 
        references domain on delete cascade;

    alter table domain_lib_project 
        add constraint FK_domain_lib_project_2 
        foreign key (lib_project_id) 
        references lib_project on delete cascade;
	
    update schema_version set version = 20;
    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;