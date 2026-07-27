begin transaction;
    create table users (
	user_id int8 not null, 
	user_name varchar(20) not null unique, 
	first_name varchar(20) not null, 
	last_name varchar(30) not null, 
	password varchar(40) not null, 
	email varchar(50) not null, 
	email_valid_token varchar(40), 
	registration_dt timestamp not null, 
	email_valid_dt timestamp, 
	admin bool not null, 
	status varchar(20) not null, 
	primary key (user_id));

    create sequence seq_user_id;

    create table project (
	project_id int8 not null, 
	user_id int8 not null,
	name varchar(100) not null, 
	major_version int4 not null, 
	minor_version int4 not null, 
	project_definition text not null, 
	theme_path varchar(60), 
	lib_project_id int8, 
	lib_version_number int4, 
	created_dt timestamp not null, 
	primary key (project_id));

    alter table project add constraint fk_project_1 foreign key (user_id) references users on delete cascade;
    alter table project add constraint uk_project_1 unique (user_id, name);

    create sequence seq_project_id;

    create table submission (
	submission_id int8 not null, 
	contents text not null, 
	form varchar(120) not null, 
	created_dt timestamp not null, 
	project_id int8 not null, 
	primary key (submission_id));

    alter table submission add constraint fk_submission_1 foreign key (project_id) references project on delete cascade;
   
    create sequence seq_submission_id;

    update schema_version set version = 2;

    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;