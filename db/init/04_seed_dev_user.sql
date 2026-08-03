-- Dev Designer user for Java backend (/client API) when using docker-compose Postgres.
-- Password: dev  (MD5 hash matches com.tawala.domain.User.hashPassword)
-- Safe to re-run: uses ON CONFLICT DO NOTHING.
--
-- email is deliberately @localhost (see designer@localhost too). Process Send with no
-- explicit From uses the project owner as Reply-To; Resend rejects @localhost, so
-- Email.buildSafeReplyTo omits Reply-To for these users (SMTP From stays server-owned).

INSERT INTO users (
    user_id,
    user_name,
    normal_user_name,
    first_name,
    last_name,
    password,
    email,
    registration_dt,
    admin,
    status,
    password_reset,
    suspended,
    enable_ads
) VALUES (
    nextval('seq_user_id'),
    'dev',
    'dev',
    'Dev',
    'Designer',
    'MD5:53mJ7SF1jngzGyDkd/xVgg==',
    'dev@localhost',
    NOW(),
    false,
    'REGISTERED',
    false,
    false,
    true
) ON CONFLICT (user_name) DO NOTHING;

INSERT INTO users (
    user_id,
    user_name,
    normal_user_name,
    first_name,
    last_name,
    password,
    email,
    registration_dt,
    admin,
    status,
    password_reset,
    suspended,
    enable_ads
) VALUES (
    nextval('seq_user_id'),
    'designer',
    'designer',
    'Project',
    'Designer',
    'MD5:53mJ7SF1jngzGyDkd/xVgg==',
    'designer@localhost',
    NOW(),
    false,
    'REGISTERED',
    false,
    false,
    true
) ON CONFLICT (user_name) DO NOTHING;
