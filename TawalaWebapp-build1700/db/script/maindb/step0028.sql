begin transaction; 
    create table project_library (
        project_library_id int,
        description varchar(100),
        primary key (project_library_id)
    );

	--- Create predefined categories
    insert into project_library values (1, 'System Library');
    insert into project_library values (2, 'Community Library');
    insert into project_library values (3, 'Under Construction Library');

	--- Add the key to the category table
    alter table lib_category add project_library_id int;
    
	--- Assign the top level categories to the right libraries
    update lib_category set project_library_id = 1 where parent_category_id is null and name = 'Tawala Systems Library';
    update lib_category set project_library_id = 1 where parent_category_id is null and name = 'Other';
    update lib_category set project_library_id = 2 where parent_category_id is null and name = 'User Community Library';
    update lib_category set project_library_id = 3 where parent_category_id is null and name = 'Under Construction';
 
	--- Any other top level category goes to the community library
    update lib_category set project_library_id = 2 where parent_category_id is null;

	--- Progressively assign the library to the next level of categories. We shouldn't have more than 5.
    update lib_category set project_library_id = 
	(select o.project_library_id from lib_category o where o.category_id = lib_category.parent_category_id) 
	where project_library_id is null;

    update lib_category set project_library_id = 
	(select o.project_library_id from lib_category o where o.category_id = lib_category.parent_category_id) 
	where project_library_id is null;
  
    update lib_category set project_library_id = 
	(select o.project_library_id from lib_category o where o.category_id = lib_category.parent_category_id) 
	where project_library_id is null;
  
    update lib_category set project_library_id = 
	(select o.project_library_id from lib_category o where o.category_id = lib_category.parent_category_id) 
	where project_library_id is null;
  
    update lib_category set project_library_id = 
	(select o.project_library_id from lib_category o where o.category_id = lib_category.parent_category_id) 
	where project_library_id is null;
  
    update lib_category set project_library_id = 
	(select o.project_library_id from lib_category o where o.category_id = lib_category.parent_category_id) 
	where project_library_id is null;
  
    alter table lib_category alter project_library_id set not null;

    alter table lib_category 
        add constraint FK_lib_category_2 
        foreign key (project_library_id) 
        references project_library;


    alter table lib_project drop under_construction;
    alter table lib_project drop vetted;

    update schema_version set version = 28;
    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;