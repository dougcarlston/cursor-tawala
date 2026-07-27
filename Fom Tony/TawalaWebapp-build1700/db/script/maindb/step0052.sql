begin transaction;
	create sequence seq_user_theme_id;
	
	create table user_theme (
        user_theme_id int8 not null,
        created_dt timestamp not null,
        updated_dt timestamp not null,
        name varchar(40) not null,
        parent_theme varchar(40) not null,
        style_definitions text not null,
        user_id int8 not null,
        primary key (user_theme_id),
        unique (user_id, name)
    );
	

	CREATE INDEX ix_user_theme_user_id ON user_theme (user_id);

	select acl_admin.grant_on_visible('tawala_user', 'ALL');
	update schema_version set version = 52;
commit;