begin transaction; 
    alter table users add shared_storage_project_id int8;

    ALTER TABLE users
	ADD CONSTRAINT fk_users_1 FOREIGN KEY (shared_storage_project_id)
      REFERENCES project (project_id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE;

    update schema_version set version = 31;
commit;