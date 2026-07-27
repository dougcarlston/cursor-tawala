begin transaction;
    alter table user_theme 
        add constraint FK_user_theme_users 
        foreign key (user_id) 
        references users on delete cascade;

    create sequence seq_user_image_id;

    create table user_image (
        user_image_id int8 not null,
        user_id int8 not null,
        image_data oid not null,
	content_type varchar(30) not null,
        suffix varchar(20) not null,
	size int8 not null,
        created_dt timestamp not null,
        primary key (user_image_id)
    );

	alter table user_image 
        add constraint FK_user_image_users 
        foreign key (user_id) 
        references users on delete cascade;
	
	select acl_admin.grant_on_visible('tawala_user', 'ALL');
	update schema_version set version = 53;
commit;