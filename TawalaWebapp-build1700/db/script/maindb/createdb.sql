CREATE USER tawala_admin PASSWORD 'tawala';
CREATE USER tawala_user PASSWORD 'tawala';

CREATE Group users
  USER tawala_user;

CREATE DATABASE tawala
  WITH OWNER = tawala_admin
       ENCODING = 'UTF8'
       TABLESPACE = pg_default
	;
