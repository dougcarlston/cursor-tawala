begin transaction;  
    ALTER TABLE users ALTER COLUMN first_name DROP NOT NULL;
    ALTER TABLE users ALTER COLUMN last_name DROP NOT NULL;
    ALTER TABLE users ALTER COLUMN email DROP NOT NULL;
    ALTER TABLE users ADD COLUMN suspended bool;
    UPDATE users set suspended = false;
    ALTER TABLE users ALTER COLUMN suspended SET NOT NULL;
    UPDATE users SET suspended = true, status = 'REGISTERED' WHERE status = 'SUSPENDED' OR status = 'HOLD';

    update schema_version set version = 17;

    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;