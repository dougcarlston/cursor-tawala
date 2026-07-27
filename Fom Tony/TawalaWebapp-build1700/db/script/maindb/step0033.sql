begin transaction; 
	create table access_ticket (
		access_ticket_id char(40) not null,
		user_id int8 not null,
		created_dt timestamp not null, 
		last_used_dt timestamp, 
		primary key (access_ticket_id)
    );
	
	alter table access_ticket add constraint fk_access_ticket_1 foreign key (user_id) references users on delete cascade;
	
	select acl_admin.grant_on_visible('tawala_user', 'ALL');
	
    update schema_version set version = 33;
commit;