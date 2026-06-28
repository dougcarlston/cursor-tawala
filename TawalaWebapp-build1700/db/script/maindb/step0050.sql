begin transaction;
	create sequence seq_email_id;
	
	create table email (
        email_id int8 not null,
        email_type varchar(30) not null,
        state varchar(20) not null,
        from_address varchar(500) not null,
        to_address varchar(1000),
        cc_address varchar(1000),
        subject varchar(1000) not null,
        type varchar(20) not null,
	body oid,
	compressed bool,
        error_reason varchar(1000),
        cust_error_reason varchar(1000),
	create_dt timestamp not null,
	sent_dt timestamp,
        user_project_id int8,
        primary key (email_id)
    );

	CREATE INDEX ix_email_state ON email (state);
	CREATE INDEX ix_email_user_project_id ON email (user_project_id);

	select acl_admin.grant_on_visible('tawala_user', 'ALL');
	
	update schema_version set version = 50;
commit;