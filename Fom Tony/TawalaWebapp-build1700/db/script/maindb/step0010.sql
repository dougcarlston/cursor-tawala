begin transaction;
    CREATE INDEX ix_submission_1 ON submission USING btree  (project_id, form);

    update schema_version set version = 10;

    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;