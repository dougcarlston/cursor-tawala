    create table lib_category (
        category_id int8 not null,
        name varchar(40) not null,
        description varchar(60),
        read_only bool not null,
        project_count int4 not null,
        version int4 not null,
        parent_category_id int8,
        primary key (category_id)
    );

    create table lib_comment (
        comment_id int8 not null,
        user_id varchar(20) not null,
        comment_text text not null,
        created_dt timestamp not null,
        deleted bool not null,
        primary key (comment_id)
    );

    create table lib_event (
        eventtype varchar(30) not null,
        lib_event_id int8 not null,
        created_dt timestamp not null,
        user_id varchar(20) not null,
        lib_project_id int8,
        lib_comment_id int8,
        comment_by_id varchar(20),
        comment_dt timestamp,
        prev_category_id int8,
        prev_string_value varchar(255),
        new_string_value varchar(255),
        prev_desc text,
        new_desc text,
        version_number int4,
        lib_version_id int8,
        version_user_id varchar(20),
        primary key (lib_event_id)
    );

    create table lib_project (
        lib_project_id int8 not null,
        author_id varchar(20) not null,
        name varchar(60) not null unique,
        short_desc varchar(60) not null,
        long_desc text not null,
        next_version int4 not null,
        rating int4 not null,
        deleted bool not null,
        download_count int4 not null,
        comment_count int4 not null,
        created_dt timestamp not null,
        version int4 not null,
        category_id int8 not null,
        primary key (lib_project_id)
    );

    create table lib_project_comment (
        lib_project_id int8 not null,
        comment_id int8 not null,
        unique (comment_id)
    );

    create table lib_project_rating (
        project_rating_id int8 not null,
        user_id varchar(20) not null,
        note varchar(100),
        created_dt timestamp not null,
        value int4 not null,
        primary key (project_rating_id)
    );

    create table lib_project_rating_map (
        lib_project_id int8 not null,
        project_rating_id int8 not null,
        key varchar(255),
        primary key (lib_project_id, key),
        unique (project_rating_id)
    );

    create table lib_project_user_downloaded (
        lib_project_id int8 not null,
        user_id_and_version varchar(30) not null,
        primary key (lib_project_id, user_id_and_version)
    );

    create table lib_project_version (
        lib_project_version_id int8 not null,
        version_number int4 not null,
        user_id varchar(20) not null,
        description text not null,
        project_definition text not null,
        deleted bool not null,
        created_dt timestamp not null,
        primary key (lib_project_version_id)
    );

    create table lib_project_version_map (
        lib_project_id int8 not null,
        lib_project_version_id int8 not null,
        unique (lib_project_version_id)
    );

    alter table lib_category 
        add constraint FK_lib_category_1 
        foreign key (parent_category_id) 
        references lib_category;

    alter table lib_project 
        add constraint FK_lib_project_1
        foreign key (category_id) 
        references lib_category;

    alter table lib_project_comment 
        add constraint FK_lib_project_comment_1
        foreign key (comment_id) 
        references lib_comment;

    alter table lib_project_comment 
        add constraint FK_lib_project_comment_2
        foreign key (lib_project_id) 
        references lib_project;

    alter table lib_project_rating_map 
        add constraint FK_lib_project_rating_map_1
        foreign key (project_rating_id) 
        references lib_project_rating;

    alter table lib_project_rating_map 
        add constraint FK_lib_project_rating_map_2
        foreign key (lib_project_id) 
        references lib_project;

    alter table lib_project_user_downloaded 
        add constraint FK_lib_project_user_downloaded_1 
        foreign key (lib_project_id) 
        references lib_project;

    alter table lib_project_version_map 
        add constraint FK_lib_project_version_map_1
        foreign key (lib_project_version_id) 
        references lib_project_version;

    alter table lib_project_version_map 
        add constraint FK_lib_project_version_map_2
        foreign key (lib_project_id) 
        references lib_project;

    create sequence seq_lib_category_id;

    create sequence seq_lib_comment_id;

    create sequence seq_lib_event_id;

    create sequence seq_lib_project_id;

    create sequence seq_lib_project_rating_id;

    create sequence seq_lib_project_version_id;

    create table schema_version	(
		version int8 NOT NULL
    );

    INSERT INTO schema_version VALUES (1);

    select acl_admin.grant_on_visible('tawala_user', 'ALL');
