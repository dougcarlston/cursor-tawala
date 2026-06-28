begin transaction;
	alter table submission add updated_dt timestamp;
	update schema_version set version = 62;
commit;