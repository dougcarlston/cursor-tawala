begin transaction;
    create sequence seq_project_invoice_event_id;

    create table project_invoice_event (
        notification_request_id int8 not null,
        status varchar(25) not null,
        payload text not null,
        event_dt timestamp not null,
        created_dt timestamp not null,
        project_invoice_id varchar(20) not null,
        primary key (notification_request_id)
    );

    alter table project_invoice_event 
        add constraint FK_project_invoice_event_project_invoice 
        foreign key (project_invoice_id) 
        references project_invoice;

	update schema_version set version = 47;
	select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;