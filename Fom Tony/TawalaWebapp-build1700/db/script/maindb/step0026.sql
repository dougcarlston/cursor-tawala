begin transaction; 
	create table visitor (
        visitor_id int8 not null,
        referrer varchar(300),
        created_dt timestamp not null default current_timestamp,
        primary key (visitor_id) );

	alter table visitor add landed_on varchar(100) not null;
	alter table visitor add remote_host varchar(100);
	alter table visitor add user_agent varchar(100);

	create sequence seq_visitor_id; 

	create table event (
        event_id int8 not null,
        visitor_id int8,
        user_id int8,
        type varchar(40) not null,
        param1 varchar(300),
	created_dt timestamp not null default current_timestamp,
        primary key (event_id)
	);

	create sequence seq_event_id; 

	alter table users add visitor_id int8;

    update schema_version set version = 26;
    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;