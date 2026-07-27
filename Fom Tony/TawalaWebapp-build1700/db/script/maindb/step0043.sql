begin transaction; 
	create table project_invoice (
		project_invoice_id bigint not null,
		status varchar(20) not null,
		invoice_amount numeric(19, 2) not null,
		status_field_name varchar(40) not null,
		invoice_dt timestamp not null,
		paid_amount numeric(19, 2),
		paid_dt timestamp,
		submission_id bigint,
		user_id bigint,
		user_project_id bigint,
		primary key (project_invoice_id)
	    );
    
	create sequence project_invoice_id;
    
	update schema_version set version = 43;
	
	select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;