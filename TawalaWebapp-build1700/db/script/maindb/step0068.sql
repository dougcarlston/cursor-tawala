begin transaction;
	CREATE TRIGGER t_email BEFORE DELETE ON email
	     FOR EACH ROW EXECUTE PROCEDURE lo_manage(body);
 	 
	update schema_version set version = 68;
commit;