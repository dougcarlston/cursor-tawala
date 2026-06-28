begin transaction; 
	create table suggestion (
        suggestion_id int8 not null,
        domain_name varchar(60),
        suggestion text not null,
        created_dt timestamp not null,
        primary key (suggestion_id)
    );
    
    create table notification_request (
        notification_request_id int8 not null,
        domain_name varchar(60) not null,
        email varchar(120) not null,
        created_dt timestamp not null,
        primary key (notification_request_id)
    );
    
	create sequence seq_suggestion_id; 
	create sequence seq_notification_request_id;
	
    update schema_version set version = 22;
    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;