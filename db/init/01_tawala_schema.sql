-- Tawala platform — consolidated PostgreSQL schema (legacy build-1700, schema_version 68)
-- Consolidated from TawalaWebapp-build1700/db/script/maindb/step0001..step0068
--
-- Compatible with legacy Java/Hibernate entity mappings.
-- Modernizations: timestamptz, explicit grants in 99_grants.sql (no acl_admin helpers).

BEGIN;

-- Large-object triggers (email.body OID, etc.) require lo_manage() from the lo extension
CREATE EXTENSION IF NOT EXISTS lo;

CREATE SEQUENCE seq_user_id;
CREATE SEQUENCE seq_project_id;
CREATE SEQUENCE seq_submission_id;
CREATE SEQUENCE seq_user_project_id;
CREATE SEQUENCE seq_project_version_id;
CREATE SEQUENCE seq_lib_category_id;
CREATE SEQUENCE seq_lib_comment_id;
CREATE SEQUENCE seq_lib_event_id;
CREATE SEQUENCE seq_lib_project_id;
CREATE SEQUENCE seq_lib_project_rating_id;
CREATE SEQUENCE seq_lib_project_version_id;
CREATE SEQUENCE seq_domain_id START WITH 1;
CREATE SEQUENCE seq_suggestion_id;
CREATE SEQUENCE seq_notification_request_id;
CREATE SEQUENCE seq_visitor_id;
CREATE SEQUENCE seq_event_id;
CREATE SEQUENCE seq_email_id;
CREATE SEQUENCE seq_user_theme_id;
CREATE SEQUENCE seq_project_invoice_event_id;
CREATE SEQUENCE seq_user_project_backup_schedule_id;
CREATE SEQUENCE seq_project_group_id;

CREATE TABLE schema_version (
    version BIGINT NOT NULL
);

INSERT INTO schema_version (version) VALUES (68);

CREATE TABLE project_library (
    project_library_id INTEGER NOT NULL,
    description        VARCHAR(100),
    show_cloned_count  BOOLEAN NOT NULL,
    PRIMARY KEY (project_library_id)
);

INSERT INTO project_library (project_library_id, description, show_cloned_count) VALUES
    (1, 'System Library', false),
    (2, 'Community Library', false),
    (3, 'Under Construction Library', false);

CREATE TABLE lib_category (
    category_id         BIGINT NOT NULL,
    name                VARCHAR(40) NOT NULL,
    description         VARCHAR(60),
    read_only           BOOLEAN NOT NULL,
    project_count       INTEGER NOT NULL,
    parent_category_id  BIGINT,
    project_library_id  INTEGER NOT NULL,
    PRIMARY KEY (category_id),
    CONSTRAINT fk_lib_category_parent
        FOREIGN KEY (parent_category_id) REFERENCES lib_category,
    CONSTRAINT fk_lib_category_library
        FOREIGN KEY (project_library_id) REFERENCES project_library
);

CREATE TABLE lib_comment (
    comment_id    BIGINT NOT NULL,
    user_id       VARCHAR(20) NOT NULL,
    comment_text  TEXT NOT NULL,
    created_dt    TIMESTAMPTZ NOT NULL,
    deleted       BOOLEAN NOT NULL,
    PRIMARY KEY (comment_id)
);

CREATE TABLE lib_project (
    lib_project_id    BIGINT NOT NULL,
    author_id         VARCHAR(20) NOT NULL,
    name              VARCHAR(60) NOT NULL UNIQUE,
    short_desc        VARCHAR(60) NOT NULL,
    long_desc         TEXT NOT NULL,
    next_version      INTEGER NOT NULL,
    rating            INTEGER NOT NULL,
    deleted           BOOLEAN NOT NULL,
    download_count    INTEGER NOT NULL,
    comment_count     INTEGER NOT NULL,
    created_dt        TIMESTAMPTZ NOT NULL,
    testdrive_count   INTEGER NOT NULL,
    last_updated_dt   TIMESTAMPTZ NOT NULL,
    featured          BOOLEAN NOT NULL,
    featured_order    INTEGER,
    icon_url          VARCHAR(100),
    video_url         VARCHAR(100),
    snapshot_tile     VARCHAR(100),
    clone_count       INTEGER NOT NULL,
    category_id       BIGINT NOT NULL,
    PRIMARY KEY (lib_project_id),
    CONSTRAINT fk_lib_project_category
        FOREIGN KEY (category_id) REFERENCES lib_category
);

CREATE TABLE lib_project_rating (
    project_rating_id BIGINT NOT NULL,
    user_id           VARCHAR(20) NOT NULL,
    note              VARCHAR(100),
    created_dt        TIMESTAMPTZ NOT NULL,
    value             INTEGER NOT NULL,
    PRIMARY KEY (project_rating_id)
);

CREATE TABLE lib_project_rating_map (
    lib_project_id      BIGINT NOT NULL,
    project_rating_id   BIGINT NOT NULL,
    key                 VARCHAR(255),
    PRIMARY KEY (lib_project_id, key),
    UNIQUE (project_rating_id),
    CONSTRAINT fk_lib_project_rating_map_rating
        FOREIGN KEY (project_rating_id) REFERENCES lib_project_rating,
    CONSTRAINT fk_lib_project_rating_map_project
        FOREIGN KEY (lib_project_id) REFERENCES lib_project
);

CREATE TABLE lib_project_user_downloaded (
    lib_project_id       BIGINT NOT NULL,
    user_id_and_version  VARCHAR(60) NOT NULL,
    PRIMARY KEY (lib_project_id, user_id_and_version),
    CONSTRAINT fk_lib_project_user_downloaded
        FOREIGN KEY (lib_project_id) REFERENCES lib_project
);

CREATE TABLE lib_project_comment (
    lib_project_id BIGINT NOT NULL,
    comment_id     BIGINT NOT NULL,
    UNIQUE (comment_id),
    CONSTRAINT fk_lib_project_comment_comment
        FOREIGN KEY (comment_id) REFERENCES lib_comment,
    CONSTRAINT fk_lib_project_comment_project
        FOREIGN KEY (lib_project_id) REFERENCES lib_project
);

CREATE TABLE lib_event (
    eventtype           VARCHAR(30) NOT NULL,
    lib_event_id        BIGINT NOT NULL,
    created_dt          TIMESTAMPTZ NOT NULL,
    user_id             VARCHAR(20) NOT NULL,
    lib_project_id      BIGINT,
    lib_comment_id      BIGINT,
    comment_by_id       VARCHAR(20),
    comment_dt          TIMESTAMPTZ,
    prev_category_id    BIGINT,
    prev_string_value   VARCHAR(255),
    new_string_value    VARCHAR(255),
    prev_desc           TEXT,
    new_desc            TEXT,
    version_number      INTEGER,
    lib_version_id      BIGINT,
    version_user_id     VARCHAR(20),
    category_id         BIGINT,
    new_category_id     BIGINT,
    category_name       VARCHAR(255),
    PRIMARY KEY (lib_event_id)
);

CREATE TABLE users (
    user_id                    BIGINT NOT NULL,
    user_name                  VARCHAR(40) NOT NULL UNIQUE,
    normal_user_name           VARCHAR(40) NOT NULL UNIQUE,
    first_name                 VARCHAR(20),
    last_name                  VARCHAR(30),
    password                   VARCHAR(40) NOT NULL,
    email                      VARCHAR(100),
    email_valid_token          VARCHAR(40),
    registration_dt            TIMESTAMPTZ NOT NULL,
    email_valid_dt             TIMESTAMPTZ,
    admin                      BOOLEAN NOT NULL,
    status                     VARCHAR(20) NOT NULL,
    password_reset             BOOLEAN NOT NULL,
    suspended                  BOOLEAN NOT NULL,
    last_logged_in_dt          TIMESTAMPTZ,
    visitor_id                 BIGINT,
    original_domain            VARCHAR(60),
    shared_storage_project_id  BIGINT,
    enable_ads                 BOOLEAN NOT NULL,
    paypal_account             VARCHAR(50),
    PRIMARY KEY (user_id)
);

CREATE TABLE role (
    role_id      VARCHAR(50) NOT NULL,
    description  VARCHAR(255),
    PRIMARY KEY (role_id)
);

CREATE TABLE user_role (
    user_id  BIGINT NOT NULL,
    role_id  VARCHAR(50) NOT NULL,
    PRIMARY KEY (user_id, role_id),
    CONSTRAINT fk_user_role_role
        FOREIGN KEY (role_id) REFERENCES role ON DELETE CASCADE,
    CONSTRAINT fk_user_role_users
        FOREIGN KEY (user_id) REFERENCES users ON DELETE CASCADE
);

CREATE TABLE access_ticket (
    access_ticket_id  CHAR(40) NOT NULL,
    user_id           BIGINT NOT NULL,
    created_dt        TIMESTAMPTZ NOT NULL,
    last_used_dt      TIMESTAMPTZ,
    PRIMARY KEY (access_ticket_id),
    CONSTRAINT fk_access_ticket_users
        FOREIGN KEY (user_id) REFERENCES users ON DELETE CASCADE
);

CREATE TABLE visitor (
    visitor_id    BIGINT NOT NULL,
    referrer      VARCHAR(300),
    created_dt    TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    landed_on     VARCHAR(100) NOT NULL,
    remote_host   VARCHAR(100),
    user_agent    VARCHAR(100),
    PRIMARY KEY (visitor_id)
);

CREATE TABLE event (
    event_id     BIGINT NOT NULL,
    visitor_id   BIGINT,
    user_id      BIGINT,
    type         VARCHAR(40) NOT NULL,
    param1       VARCHAR(300),
    created_dt   TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (event_id)
);

CREATE TABLE project (
    project_id           BIGINT NOT NULL,
    major_version        INTEGER NOT NULL,
    minor_version        INTEGER NOT NULL,
    project_definition   TEXT NOT NULL,
    theme_path           VARCHAR(60),
    created_dt           TIMESTAMPTZ NOT NULL,
    designer_version     VARCHAR(10),
    properties           TEXT,
    PRIMARY KEY (project_id)
);

CREATE TABLE user_project (
    user_project_id                  BIGINT NOT NULL,
    name                             VARCHAR(100) NOT NULL,
    lib_project_id                   BIGINT,
    lib_version_number               INTEGER,
    created_dt                       TIMESTAMPTZ NOT NULL,
    last_updated_dt                  TIMESTAMPTZ NOT NULL,
    user_id                          BIGINT NOT NULL,
    project_id                       BIGINT NOT NULL,
    deployed_version_id              BIGINT,
    version                          INTEGER NOT NULL,
    next_version                     INTEGER NOT NULL,
    original_lib_project_version_id  BIGINT,
    unique_random_id                 VARCHAR(20) NOT NULL,
    offline                          BOOLEAN NOT NULL,
    reg_start_dt                     DATE,
    reg_close_dt                     DATE,
    reg_closed                       BOOLEAN,
    reg_fee                          NUMERIC(6, 2),
    reg_invoice_dt                   DATE,
    project_type                     VARCHAR(20),
    po_number                        VARCHAR(20),
    invoice_number                   VARCHAR(20),
    ysl_league_id                    VARCHAR(20),
    ysl_last_updated                 TIMESTAMPTZ,
    roster_template_id               VARCHAR(40),
    require_ssl                      BOOLEAN NOT NULL,
    PRIMARY KEY (user_project_id),
    UNIQUE (user_id, project_id),
    UNIQUE (ysl_league_id),
    CONSTRAINT fk_user_project_user
        FOREIGN KEY (user_id) REFERENCES users ON DELETE CASCADE,
    CONSTRAINT fk_user_project_project
        FOREIGN KEY (project_id) REFERENCES project ON DELETE CASCADE
);

CREATE TABLE project_version (
    project_version_id  BIGINT NOT NULL,
    user_project_id     BIGINT,
    project_id          BIGINT NOT NULL,
    version_number      INTEGER NOT NULL,
    description         TEXT NOT NULL,
    deleted             BOOLEAN NOT NULL,
    created_dt          TIMESTAMPTZ NOT NULL,
    PRIMARY KEY (project_version_id),
    UNIQUE (project_id),
    UNIQUE (user_project_id, version_number),
    CONSTRAINT fk_project_version_project
        FOREIGN KEY (project_id) REFERENCES project,
    CONSTRAINT fk_project_version_user_project
        FOREIGN KEY (user_project_id) REFERENCES user_project ON DELETE CASCADE
);

ALTER TABLE user_project
    ADD CONSTRAINT fk_user_project_deployed_version
        FOREIGN KEY (deployed_version_id) REFERENCES project_version;

ALTER TABLE users
    ADD CONSTRAINT fk_users_shared_storage
        FOREIGN KEY (shared_storage_project_id) REFERENCES project;

CREATE TABLE lib_project_version (
    lib_project_version_id  BIGINT NOT NULL,
    version_number          INTEGER NOT NULL,
    user_id                 VARCHAR(20) NOT NULL,
    description             TEXT NOT NULL,
    deleted                 BOOLEAN NOT NULL,
    created_dt              TIMESTAMPTZ NOT NULL,
    project_id              BIGINT NOT NULL,
    lib_project_id          BIGINT NOT NULL,
    PRIMARY KEY (lib_project_version_id),
    CONSTRAINT fk_lib_project_version_project
        FOREIGN KEY (project_id) REFERENCES project,
    CONSTRAINT fk_lib_project_version_lib_project
        FOREIGN KEY (lib_project_id) REFERENCES lib_project ON DELETE CASCADE
);

CREATE TABLE submission (
    submission_id  BIGINT NOT NULL,
    contents       TEXT NOT NULL,
    form           VARCHAR(120) NOT NULL,
    created_dt     TIMESTAMPTZ NOT NULL,
    updated_dt     TIMESTAMPTZ,
    project_id     BIGINT NOT NULL,
    PRIMARY KEY (submission_id),
    CONSTRAINT fk_submission_project
        FOREIGN KEY (project_id) REFERENCES project ON DELETE CASCADE
);

CREATE INDEX ix_submission_1 ON submission (project_id, form);

CREATE TABLE user_project_link (
    user_project_link_id  VARCHAR(20) NOT NULL,
    is_authenticated      BOOLEAN NOT NULL,
    authentication_token  TEXT,
    user_project_id       BIGINT NOT NULL,
    created_dt            TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    use_once              BOOLEAN NOT NULL,
    PRIMARY KEY (user_project_link_id),
    CONSTRAINT fk_user_project_link_user_project
        FOREIGN KEY (user_project_id) REFERENCES user_project ON DELETE CASCADE
);

CREATE INDEX user_project_link_authentication_token
    ON user_project_link (authentication_token);

CREATE TABLE user_file (
    user_file_id      VARCHAR(15) NOT NULL,
    user_id           BIGINT NOT NULL,
    image_data        OID NOT NULL,
    content_type      VARCHAR(100) NOT NULL,
    file_name         VARCHAR(200) NOT NULL,
    size              BIGINT NOT NULL,
    created_dt        TIMESTAMPTZ NOT NULL,
    user_project_id   BIGINT,
    PRIMARY KEY (user_file_id),
    CONSTRAINT fk_user_file_users
        FOREIGN KEY (user_id) REFERENCES users ON DELETE CASCADE,
    CONSTRAINT fk_user_file_user_project
        FOREIGN KEY (user_project_id) REFERENCES user_project ON DELETE CASCADE
);

CREATE TABLE user_theme (
    user_theme_id       BIGINT NOT NULL,
    created_dt          TIMESTAMPTZ NOT NULL,
    updated_dt          TIMESTAMPTZ NOT NULL,
    name                VARCHAR(40) NOT NULL,
    parent_theme        VARCHAR(40) NOT NULL,
    style_definitions   TEXT NOT NULL,
    user_id             BIGINT NOT NULL,
    header_image_id     VARCHAR(15),
    PRIMARY KEY (user_theme_id),
    UNIQUE (user_id, name),
    CONSTRAINT fk_user_theme_users
        FOREIGN KEY (user_id) REFERENCES users ON DELETE CASCADE,
    CONSTRAINT fk_user_theme_user_file
        FOREIGN KEY (header_image_id) REFERENCES user_file
);

CREATE INDEX ix_user_theme_user_id ON user_theme (user_id);

CREATE TABLE user_project_backup_schedule (
    user_project_backup_schedule_id  BIGINT NOT NULL,
    schedule_type                    VARCHAR(30) NOT NULL,
    next_dt                          TIMESTAMPTZ NOT NULL,
    created_dt                       TIMESTAMPTZ NOT NULL,
    hour                             INTEGER,
    minute                           INTEGER,
    user_project_id                  BIGINT NOT NULL,
    PRIMARY KEY (user_project_backup_schedule_id),
    CONSTRAINT fk_user_project_backup_schedule_user_project
        FOREIGN KEY (user_project_id) REFERENCES user_project ON DELETE CASCADE
);

CREATE INDEX ix_user_project_backup_schedule_next_dt
    ON user_project_backup_schedule (next_dt);

CREATE TABLE email (
    email_id          BIGINT NOT NULL,
    email_type        VARCHAR(30) NOT NULL,
    state             VARCHAR(20) NOT NULL,
    from_address      VARCHAR(500) NOT NULL,
    to_address        VARCHAR(1000),
    cc_address        VARCHAR(1000),
    subject           VARCHAR(1000) NOT NULL,
    type              VARCHAR(20) NOT NULL,
    body              OID,
    compressed        BOOLEAN,
    error_reason      VARCHAR(1000),
    cust_error_reason VARCHAR(1000),
    create_dt         TIMESTAMPTZ NOT NULL,
    sent_dt           TIMESTAMPTZ,
    user_project_id   BIGINT,
    orig_size         INTEGER,
    compressed_size   INTEGER,
    PRIMARY KEY (email_id)
);

CREATE INDEX ix_email_state ON email (state);
CREATE INDEX ix_email_user_project_id ON email (user_project_id);
CREATE INDEX ix_email_create_dt ON email (create_dt);

CREATE TRIGGER t_email
    BEFORE DELETE ON email
    FOR EACH ROW EXECUTE FUNCTION lo_manage(body);

CREATE TABLE project_group (
    project_group_id  BIGINT NOT NULL,
    user_id           BIGINT NOT NULL,
    name              VARCHAR(200) NOT NULL,
    PRIMARY KEY (project_group_id),
    UNIQUE (user_id, name),
    CONSTRAINT fk_project_group_user
        FOREIGN KEY (user_id) REFERENCES users ON DELETE CASCADE
);

CREATE TABLE project_group_member (
    project_group_id   BIGINT NOT NULL,
    user_project_id    BIGINT NOT NULL,
    CONSTRAINT fk_project_group_member_group
        FOREIGN KEY (project_group_id) REFERENCES project_group ON DELETE CASCADE,
    CONSTRAINT fk_project_group_member_user_project
        FOREIGN KEY (user_project_id) REFERENCES user_project ON DELETE CASCADE
);

CREATE TABLE domain (
    domain_id          BIGINT NOT NULL,
    name               VARCHAR(120) NOT NULL UNIQUE,
    title              VARCHAR(120) NOT NULL,
    subtitle           VARCHAR(120) NOT NULL,
    desc_caption       VARCHAR(120) NOT NULL,
    description        TEXT NOT NULL,
    suggestion         TEXT NOT NULL,
    display_name       VARCHAR(60) NOT NULL,
    solutions_caption  VARCHAR(120) NOT NULL,
    PRIMARY KEY (domain_id)
);

CREATE TABLE domain_lib_project (
    domain_id       BIGINT NOT NULL,
    lib_project_id  BIGINT NOT NULL,
    index           INTEGER NOT NULL,
    PRIMARY KEY (domain_id, index),
    CONSTRAINT fk_domain_lib_project_domain
        FOREIGN KEY (domain_id) REFERENCES domain ON DELETE CASCADE,
    CONSTRAINT fk_domain_lib_project_lib_project
        FOREIGN KEY (lib_project_id) REFERENCES lib_project ON DELETE CASCADE
);

CREATE TABLE suggestion (
    suggestion_id  BIGINT NOT NULL,
    domain_name    VARCHAR(60),
    suggestion     TEXT NOT NULL,
    created_dt     TIMESTAMPTZ NOT NULL,
    PRIMARY KEY (suggestion_id)
);

CREATE TABLE notification_request (
    notification_request_id  BIGINT NOT NULL,
    domain_name              VARCHAR(60) NOT NULL,
    email                    VARCHAR(120) NOT NULL,
    created_dt               TIMESTAMPTZ NOT NULL,
    PRIMARY KEY (notification_request_id)
);

CREATE TABLE project_invoice (
    project_invoice_id        VARCHAR(20) NOT NULL,
    status                    VARCHAR(20) NOT NULL,
    invoice_amount            NUMERIC(19, 2) NOT NULL,
    status_field_name         VARCHAR(100) NOT NULL,
    paid_amount_field_name    VARCHAR(100),
    invoice_dt                TIMESTAMPTZ NOT NULL,
    paid_amount               NUMERIC(19, 2),
    paid_dt                   TIMESTAMPTZ,
    submission_id             BIGINT,
    user_id                   BIGINT,
    user_project_id           BIGINT,
    PRIMARY KEY (project_invoice_id),
    CONSTRAINT fk_project_invoice_submission
        FOREIGN KEY (submission_id) REFERENCES submission,
    CONSTRAINT fk_project_invoice_user
        FOREIGN KEY (user_id) REFERENCES users,
    CONSTRAINT fk_project_invoice_user_project
        FOREIGN KEY (user_project_id) REFERENCES user_project
);

CREATE TABLE project_invoice_event (
    notification_request_id  BIGINT NOT NULL,
    status                   VARCHAR(25) NOT NULL,
    payload                  TEXT NOT NULL,
    event_dt                 TIMESTAMPTZ NOT NULL,
    created_dt               TIMESTAMPTZ NOT NULL,
    project_invoice_id       VARCHAR(20) NOT NULL,
    PRIMARY KEY (notification_request_id),
    CONSTRAINT fk_project_invoice_event_invoice
        FOREIGN KEY (project_invoice_id) REFERENCES project_invoice
);

CREATE INDEX ix_user_project_project_type ON user_project (project_type);

COMMIT;
