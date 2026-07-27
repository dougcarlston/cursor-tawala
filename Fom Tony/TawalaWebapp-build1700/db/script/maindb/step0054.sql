begin transaction;

	alter table user_theme add header_image_id int8;

    alter table user_theme 
        add constraint FK_user_theme_user_image 
        foreign key (header_image_id) 
        references user_image;

	update schema_version set version = 54;
commit;