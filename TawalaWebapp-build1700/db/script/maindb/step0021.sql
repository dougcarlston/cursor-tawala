begin transaction;  
    alter table domain add column solutions_caption varchar(120);
    update domain set solutions_caption = ' ';
    ALTER TABLE domain ALTER COLUMN solutions_caption SET NOT NULL;

    alter table lib_project add column under_construction boolean;
    update lib_project set under_construction = false;
    ALTER TABLE lib_project ALTER COLUMN under_construction SET NOT NULL;
	
    update schema_version set version = 21;
commit;