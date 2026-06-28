begin transaction;
	alter table lib_project_version add column lib_project_id int8;

	update lib_project_version set lib_project_id = (SELECT lib_project_id FROM lib_project_version_map x WHERE x.lib_project_version_id = lib_project_version.lib_project_version_id);
	delete from lib_project_version where not exists (select * from lib_project y where lib_project_version.lib_project_id = y.lib_project_id);

	alter table lib_project_version alter column lib_project_id SET NOT NULL;

	alter table lib_project_version 
		add constraint FK_lib_project_version_lib_project
		foreign key (lib_project_id) 
		references lib_project on delete cascade;

	drop table lib_project_version_map;

	update schema_version set version = 55;
commit;