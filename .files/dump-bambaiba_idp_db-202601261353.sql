--
-- PostgreSQL database cluster dump
--

-- Started on 2026-01-26 13:53:29

SET default_transaction_read_only = off;

SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;

--
-- Roles
--

CREATE ROLE keycloakuser;
ALTER ROLE keycloakuser WITH SUPERUSER INHERIT CREATEROLE CREATEDB LOGIN REPLICATION BYPASSRLS;

--
-- User Configurations
--








--
-- Databases
--

--
-- Database "template1" dump
--

\connect template1

--
-- PostgreSQL database dump
--

-- Dumped from database version 16.10
-- Dumped by pg_dump version 16.1

-- Started on 2026-01-26 13:53:29

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

-- Completed on 2026-01-26 13:53:32

--
-- PostgreSQL database dump complete
--

--
-- Database "bambaiba_idp_db" dump
--

--
-- PostgreSQL database dump
--

-- Dumped from database version 16.10
-- Dumped by pg_dump version 16.1

-- Started on 2026-01-26 13:53:33

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 4224 (class 1262 OID 16384)
-- Name: bambaiba_idp_db; Type: DATABASE; Schema: -; Owner: keycloakuser
--

CREATE DATABASE bambaiba_idp_db WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8';


ALTER DATABASE bambaiba_idp_db OWNER TO keycloakuser;

\connect bambaiba_idp_db

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 249 (class 1259 OID 25209)
-- Name: admin_event_entity; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.admin_event_entity (
    id character varying(36) NOT NULL,
    admin_event_time bigint,
    realm_id character varying(255),
    operation_type character varying(255),
    auth_realm_id character varying(255),
    auth_client_id character varying(255),
    auth_user_id character varying(255),
    ip_address character varying(255),
    resource_path character varying(2550),
    representation text,
    error character varying(255),
    resource_type character varying(64),
    details_json text
);


ALTER TABLE public.admin_event_entity OWNER TO keycloakuser;

--
-- TOC entry 276 (class 1259 OID 25652)
-- Name: associated_policy; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.associated_policy (
    policy_id character varying(36) NOT NULL,
    associated_policy_id character varying(36) NOT NULL
);


ALTER TABLE public.associated_policy OWNER TO keycloakuser;

--
-- TOC entry 252 (class 1259 OID 25224)
-- Name: authentication_execution; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.authentication_execution (
    id character varying(36) NOT NULL,
    alias character varying(255),
    authenticator character varying(36),
    realm_id character varying(36),
    flow_id character varying(36),
    requirement integer,
    priority integer,
    authenticator_flow boolean DEFAULT false NOT NULL,
    auth_flow_id character varying(36),
    auth_config character varying(36)
);


ALTER TABLE public.authentication_execution OWNER TO keycloakuser;

--
-- TOC entry 251 (class 1259 OID 25219)
-- Name: authentication_flow; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.authentication_flow (
    id character varying(36) NOT NULL,
    alias character varying(255),
    description character varying(255),
    realm_id character varying(36),
    provider_id character varying(36) DEFAULT 'basic-flow'::character varying NOT NULL,
    top_level boolean DEFAULT false NOT NULL,
    built_in boolean DEFAULT false NOT NULL
);


ALTER TABLE public.authentication_flow OWNER TO keycloakuser;

--
-- TOC entry 250 (class 1259 OID 25214)
-- Name: authenticator_config; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.authenticator_config (
    id character varying(36) NOT NULL,
    alias character varying(255),
    realm_id character varying(36)
);


ALTER TABLE public.authenticator_config OWNER TO keycloakuser;

--
-- TOC entry 253 (class 1259 OID 25229)
-- Name: authenticator_config_entry; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.authenticator_config_entry (
    authenticator_id character varying(36) NOT NULL,
    value text,
    name character varying(255) NOT NULL
);


ALTER TABLE public.authenticator_config_entry OWNER TO keycloakuser;

--
-- TOC entry 277 (class 1259 OID 25667)
-- Name: broker_link; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.broker_link (
    identity_provider character varying(255) NOT NULL,
    storage_provider_id character varying(255),
    realm_id character varying(36) NOT NULL,
    broker_user_id character varying(255),
    broker_username character varying(255),
    token text,
    user_id character varying(255) NOT NULL
);


ALTER TABLE public.broker_link OWNER TO keycloakuser;

--
-- TOC entry 217 (class 1259 OID 24590)
-- Name: client; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.client (
    id character varying(36) NOT NULL,
    enabled boolean DEFAULT false NOT NULL,
    full_scope_allowed boolean DEFAULT false NOT NULL,
    client_id character varying(255),
    not_before integer,
    public_client boolean DEFAULT false NOT NULL,
    secret character varying(255),
    base_url character varying(255),
    bearer_only boolean DEFAULT false NOT NULL,
    management_url character varying(255),
    surrogate_auth_required boolean DEFAULT false NOT NULL,
    realm_id character varying(36),
    protocol character varying(255),
    node_rereg_timeout integer DEFAULT 0,
    frontchannel_logout boolean DEFAULT false NOT NULL,
    consent_required boolean DEFAULT false NOT NULL,
    name character varying(255),
    service_accounts_enabled boolean DEFAULT false NOT NULL,
    client_authenticator_type character varying(255),
    root_url character varying(255),
    description character varying(255),
    registration_token character varying(255),
    standard_flow_enabled boolean DEFAULT true NOT NULL,
    implicit_flow_enabled boolean DEFAULT false NOT NULL,
    direct_access_grants_enabled boolean DEFAULT false NOT NULL,
    always_display_in_console boolean DEFAULT false NOT NULL
);


ALTER TABLE public.client OWNER TO keycloakuser;

--
-- TOC entry 236 (class 1259 OID 24948)
-- Name: client_attributes; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.client_attributes (
    client_id character varying(36) NOT NULL,
    name character varying(255) NOT NULL,
    value text
);


ALTER TABLE public.client_attributes OWNER TO keycloakuser;

--
-- TOC entry 288 (class 1259 OID 25917)
-- Name: client_auth_flow_bindings; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.client_auth_flow_bindings (
    client_id character varying(36) NOT NULL,
    flow_id character varying(36),
    binding_name character varying(255) NOT NULL
);


ALTER TABLE public.client_auth_flow_bindings OWNER TO keycloakuser;

--
-- TOC entry 287 (class 1259 OID 25791)
-- Name: client_initial_access; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.client_initial_access (
    id character varying(36) NOT NULL,
    realm_id character varying(36) NOT NULL,
    "timestamp" integer,
    expiration integer,
    count integer,
    remaining_count integer
);


ALTER TABLE public.client_initial_access OWNER TO keycloakuser;

--
-- TOC entry 237 (class 1259 OID 24958)
-- Name: client_node_registrations; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.client_node_registrations (
    client_id character varying(36) NOT NULL,
    value integer,
    name character varying(255) NOT NULL
);


ALTER TABLE public.client_node_registrations OWNER TO keycloakuser;

--
-- TOC entry 265 (class 1259 OID 25457)
-- Name: client_scope; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.client_scope (
    id character varying(36) NOT NULL,
    name character varying(255),
    realm_id character varying(36),
    description character varying(255),
    protocol character varying(255)
);


ALTER TABLE public.client_scope OWNER TO keycloakuser;

--
-- TOC entry 266 (class 1259 OID 25471)
-- Name: client_scope_attributes; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.client_scope_attributes (
    scope_id character varying(36) NOT NULL,
    value character varying(2048),
    name character varying(255) NOT NULL
);


ALTER TABLE public.client_scope_attributes OWNER TO keycloakuser;

--
-- TOC entry 289 (class 1259 OID 25958)
-- Name: client_scope_client; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.client_scope_client (
    client_id character varying(255) NOT NULL,
    scope_id character varying(255) NOT NULL,
    default_scope boolean DEFAULT false NOT NULL
);


ALTER TABLE public.client_scope_client OWNER TO keycloakuser;

--
-- TOC entry 267 (class 1259 OID 25476)
-- Name: client_scope_role_mapping; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.client_scope_role_mapping (
    scope_id character varying(36) NOT NULL,
    role_id character varying(36) NOT NULL
);


ALTER TABLE public.client_scope_role_mapping OWNER TO keycloakuser;

--
-- TOC entry 285 (class 1259 OID 25712)
-- Name: component; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.component (
    id character varying(36) NOT NULL,
    name character varying(255),
    parent_id character varying(36),
    provider_id character varying(36),
    provider_type character varying(255),
    realm_id character varying(36),
    sub_type character varying(255)
);


ALTER TABLE public.component OWNER TO keycloakuser;

--
-- TOC entry 284 (class 1259 OID 25707)
-- Name: component_config; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.component_config (
    id character varying(36) NOT NULL,
    component_id character varying(36) NOT NULL,
    name character varying(255) NOT NULL,
    value text
);


ALTER TABLE public.component_config OWNER TO keycloakuser;

--
-- TOC entry 218 (class 1259 OID 24609)
-- Name: composite_role; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.composite_role (
    composite character varying(36) NOT NULL,
    child_role character varying(36) NOT NULL
);


ALTER TABLE public.composite_role OWNER TO keycloakuser;

--
-- TOC entry 219 (class 1259 OID 24612)
-- Name: credential; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.credential (
    id character varying(36) NOT NULL,
    salt bytea,
    type character varying(255),
    user_id character varying(36),
    created_date bigint,
    user_label character varying(255),
    secret_data text,
    credential_data text,
    priority integer
);


ALTER TABLE public.credential OWNER TO keycloakuser;

--
-- TOC entry 216 (class 1259 OID 24582)
-- Name: databasechangelog; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.databasechangelog (
    id character varying(255) NOT NULL,
    author character varying(255) NOT NULL,
    filename character varying(255) NOT NULL,
    dateexecuted timestamp without time zone NOT NULL,
    orderexecuted integer NOT NULL,
    exectype character varying(10) NOT NULL,
    md5sum character varying(35),
    description character varying(255),
    comments character varying(255),
    tag character varying(255),
    liquibase character varying(20),
    contexts character varying(255),
    labels character varying(255),
    deployment_id character varying(10)
);


ALTER TABLE public.databasechangelog OWNER TO keycloakuser;

--
-- TOC entry 215 (class 1259 OID 24577)
-- Name: databasechangeloglock; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.databasechangeloglock (
    id integer NOT NULL,
    locked boolean NOT NULL,
    lockgranted timestamp without time zone,
    lockedby character varying(255)
);


ALTER TABLE public.databasechangeloglock OWNER TO keycloakuser;

--
-- TOC entry 290 (class 1259 OID 25974)
-- Name: default_client_scope; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.default_client_scope (
    realm_id character varying(36) NOT NULL,
    scope_id character varying(36) NOT NULL,
    default_scope boolean DEFAULT false NOT NULL
);


ALTER TABLE public.default_client_scope OWNER TO keycloakuser;

--
-- TOC entry 220 (class 1259 OID 24617)
-- Name: event_entity; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.event_entity (
    id character varying(36) NOT NULL,
    client_id character varying(255),
    details_json character varying(2550),
    error character varying(255),
    ip_address character varying(255),
    realm_id character varying(255),
    session_id character varying(255),
    event_time bigint,
    type character varying(255),
    user_id character varying(255),
    details_json_long_value text
);


ALTER TABLE public.event_entity OWNER TO keycloakuser;

--
-- TOC entry 278 (class 1259 OID 25672)
-- Name: fed_user_attribute; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.fed_user_attribute (
    id character varying(36) NOT NULL,
    name character varying(255) NOT NULL,
    user_id character varying(255) NOT NULL,
    realm_id character varying(36) NOT NULL,
    storage_provider_id character varying(36),
    value character varying(2024),
    long_value_hash bytea,
    long_value_hash_lower_case bytea,
    long_value text
);


ALTER TABLE public.fed_user_attribute OWNER TO keycloakuser;

--
-- TOC entry 279 (class 1259 OID 25677)
-- Name: fed_user_consent; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.fed_user_consent (
    id character varying(36) NOT NULL,
    client_id character varying(255),
    user_id character varying(255) NOT NULL,
    realm_id character varying(36) NOT NULL,
    storage_provider_id character varying(36),
    created_date bigint,
    last_updated_date bigint,
    client_storage_provider character varying(36),
    external_client_id character varying(255)
);


ALTER TABLE public.fed_user_consent OWNER TO keycloakuser;

--
-- TOC entry 292 (class 1259 OID 26000)
-- Name: fed_user_consent_cl_scope; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.fed_user_consent_cl_scope (
    user_consent_id character varying(36) NOT NULL,
    scope_id character varying(36) NOT NULL
);


ALTER TABLE public.fed_user_consent_cl_scope OWNER TO keycloakuser;

--
-- TOC entry 280 (class 1259 OID 25686)
-- Name: fed_user_credential; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.fed_user_credential (
    id character varying(36) NOT NULL,
    salt bytea,
    type character varying(255),
    created_date bigint,
    user_id character varying(255) NOT NULL,
    realm_id character varying(36) NOT NULL,
    storage_provider_id character varying(36),
    user_label character varying(255),
    secret_data text,
    credential_data text,
    priority integer
);


ALTER TABLE public.fed_user_credential OWNER TO keycloakuser;

--
-- TOC entry 281 (class 1259 OID 25695)
-- Name: fed_user_group_membership; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.fed_user_group_membership (
    group_id character varying(36) NOT NULL,
    user_id character varying(255) NOT NULL,
    realm_id character varying(36) NOT NULL,
    storage_provider_id character varying(36)
);


ALTER TABLE public.fed_user_group_membership OWNER TO keycloakuser;

--
-- TOC entry 282 (class 1259 OID 25698)
-- Name: fed_user_required_action; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.fed_user_required_action (
    required_action character varying(255) DEFAULT ' '::character varying NOT NULL,
    user_id character varying(255) NOT NULL,
    realm_id character varying(36) NOT NULL,
    storage_provider_id character varying(36)
);


ALTER TABLE public.fed_user_required_action OWNER TO keycloakuser;

--
-- TOC entry 283 (class 1259 OID 25704)
-- Name: fed_user_role_mapping; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.fed_user_role_mapping (
    role_id character varying(36) NOT NULL,
    user_id character varying(255) NOT NULL,
    realm_id character varying(36) NOT NULL,
    storage_provider_id character varying(36)
);


ALTER TABLE public.fed_user_role_mapping OWNER TO keycloakuser;

--
-- TOC entry 240 (class 1259 OID 24994)
-- Name: federated_identity; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.federated_identity (
    identity_provider character varying(255) NOT NULL,
    realm_id character varying(36),
    federated_user_id character varying(255),
    federated_username character varying(255),
    token text,
    user_id character varying(36) NOT NULL
);


ALTER TABLE public.federated_identity OWNER TO keycloakuser;

--
-- TOC entry 286 (class 1259 OID 25769)
-- Name: federated_user; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.federated_user (
    id character varying(255) NOT NULL,
    storage_provider_id character varying(255),
    realm_id character varying(36) NOT NULL
);


ALTER TABLE public.federated_user OWNER TO keycloakuser;

--
-- TOC entry 262 (class 1259 OID 25396)
-- Name: group_attribute; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.group_attribute (
    id character varying(36) DEFAULT 'sybase-needs-something-here'::character varying NOT NULL,
    name character varying(255) NOT NULL,
    value character varying(255),
    group_id character varying(36) NOT NULL
);


ALTER TABLE public.group_attribute OWNER TO keycloakuser;

--
-- TOC entry 261 (class 1259 OID 25393)
-- Name: group_role_mapping; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.group_role_mapping (
    role_id character varying(36) NOT NULL,
    group_id character varying(36) NOT NULL
);


ALTER TABLE public.group_role_mapping OWNER TO keycloakuser;

--
-- TOC entry 241 (class 1259 OID 24999)
-- Name: identity_provider; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.identity_provider (
    internal_id character varying(36) NOT NULL,
    enabled boolean DEFAULT false NOT NULL,
    provider_alias character varying(255),
    provider_id character varying(255),
    store_token boolean DEFAULT false NOT NULL,
    authenticate_by_default boolean DEFAULT false NOT NULL,
    realm_id character varying(36),
    add_token_role boolean DEFAULT true NOT NULL,
    trust_email boolean DEFAULT false NOT NULL,
    first_broker_login_flow_id character varying(36),
    post_broker_login_flow_id character varying(36),
    provider_display_name character varying(255),
    link_only boolean DEFAULT false NOT NULL,
    organization_id character varying(255),
    hide_on_login boolean DEFAULT false
);


ALTER TABLE public.identity_provider OWNER TO keycloakuser;

--
-- TOC entry 242 (class 1259 OID 25008)
-- Name: identity_provider_config; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.identity_provider_config (
    identity_provider_id character varying(36) NOT NULL,
    value text,
    name character varying(255) NOT NULL
);


ALTER TABLE public.identity_provider_config OWNER TO keycloakuser;

--
-- TOC entry 246 (class 1259 OID 25112)
-- Name: identity_provider_mapper; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.identity_provider_mapper (
    id character varying(36) NOT NULL,
    name character varying(255) NOT NULL,
    idp_alias character varying(255) NOT NULL,
    idp_mapper_name character varying(255) NOT NULL,
    realm_id character varying(36) NOT NULL
);


ALTER TABLE public.identity_provider_mapper OWNER TO keycloakuser;

--
-- TOC entry 247 (class 1259 OID 25117)
-- Name: idp_mapper_config; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.idp_mapper_config (
    idp_mapper_id character varying(36) NOT NULL,
    value text,
    name character varying(255) NOT NULL
);


ALTER TABLE public.idp_mapper_config OWNER TO keycloakuser;

--
-- TOC entry 301 (class 1259 OID 26202)
-- Name: jgroups_ping; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.jgroups_ping (
    address character varying(200) NOT NULL,
    name character varying(200),
    cluster_name character varying(200) NOT NULL,
    ip character varying(200) NOT NULL,
    coord boolean
);


ALTER TABLE public.jgroups_ping OWNER TO keycloakuser;

--
-- TOC entry 260 (class 1259 OID 25390)
-- Name: keycloak_group; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.keycloak_group (
    id character varying(36) NOT NULL,
    name character varying(255),
    parent_group character varying(36) NOT NULL,
    realm_id character varying(36),
    type integer DEFAULT 0 NOT NULL
);


ALTER TABLE public.keycloak_group OWNER TO keycloakuser;

--
-- TOC entry 221 (class 1259 OID 24625)
-- Name: keycloak_role; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.keycloak_role (
    id character varying(36) NOT NULL,
    client_realm_constraint character varying(255),
    client_role boolean DEFAULT false NOT NULL,
    description character varying(255),
    name character varying(255),
    realm_id character varying(255),
    client character varying(36),
    realm character varying(36)
);


ALTER TABLE public.keycloak_role OWNER TO keycloakuser;

--
-- TOC entry 245 (class 1259 OID 25109)
-- Name: migration_model; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.migration_model (
    id character varying(36) NOT NULL,
    version character varying(36),
    update_time bigint DEFAULT 0 NOT NULL
);


ALTER TABLE public.migration_model OWNER TO keycloakuser;

--
-- TOC entry 259 (class 1259 OID 25381)
-- Name: offline_client_session; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.offline_client_session (
    user_session_id character varying(36) NOT NULL,
    client_id character varying(255) NOT NULL,
    offline_flag character varying(4) NOT NULL,
    "timestamp" integer,
    data text,
    client_storage_provider character varying(36) DEFAULT 'local'::character varying NOT NULL,
    external_client_id character varying(255) DEFAULT 'local'::character varying NOT NULL,
    version integer DEFAULT 0
);


ALTER TABLE public.offline_client_session OWNER TO keycloakuser;

--
-- TOC entry 258 (class 1259 OID 25376)
-- Name: offline_user_session; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.offline_user_session (
    user_session_id character varying(36) NOT NULL,
    user_id character varying(255) NOT NULL,
    realm_id character varying(36) NOT NULL,
    created_on integer NOT NULL,
    offline_flag character varying(4) NOT NULL,
    data text,
    last_session_refresh integer DEFAULT 0 NOT NULL,
    broker_session_id character varying(1024),
    version integer DEFAULT 0
);


ALTER TABLE public.offline_user_session OWNER TO keycloakuser;

--
-- TOC entry 298 (class 1259 OID 26165)
-- Name: org; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.org (
    id character varying(255) NOT NULL,
    enabled boolean NOT NULL,
    realm_id character varying(255) NOT NULL,
    group_id character varying(255) NOT NULL,
    name character varying(255) NOT NULL,
    description character varying(4000),
    alias character varying(255) NOT NULL,
    redirect_url character varying(2048)
);


ALTER TABLE public.org OWNER TO keycloakuser;

--
-- TOC entry 299 (class 1259 OID 26176)
-- Name: org_domain; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.org_domain (
    id character varying(36) NOT NULL,
    name character varying(255) NOT NULL,
    verified boolean NOT NULL,
    org_id character varying(255) NOT NULL
);


ALTER TABLE public.org_domain OWNER TO keycloakuser;

--
-- TOC entry 272 (class 1259 OID 25595)
-- Name: policy_config; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.policy_config (
    policy_id character varying(36) NOT NULL,
    name character varying(255) NOT NULL,
    value text
);


ALTER TABLE public.policy_config OWNER TO keycloakuser;

--
-- TOC entry 238 (class 1259 OID 24983)
-- Name: protocol_mapper; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.protocol_mapper (
    id character varying(36) NOT NULL,
    name character varying(255) NOT NULL,
    protocol character varying(255) NOT NULL,
    protocol_mapper_name character varying(255) NOT NULL,
    client_id character varying(36),
    client_scope_id character varying(36)
);


ALTER TABLE public.protocol_mapper OWNER TO keycloakuser;

--
-- TOC entry 239 (class 1259 OID 24989)
-- Name: protocol_mapper_config; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.protocol_mapper_config (
    protocol_mapper_id character varying(36) NOT NULL,
    value text,
    name character varying(255) NOT NULL
);


ALTER TABLE public.protocol_mapper_config OWNER TO keycloakuser;

--
-- TOC entry 222 (class 1259 OID 24631)
-- Name: realm; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.realm (
    id character varying(36) NOT NULL,
    access_code_lifespan integer,
    user_action_lifespan integer,
    access_token_lifespan integer,
    account_theme character varying(255),
    admin_theme character varying(255),
    email_theme character varying(255),
    enabled boolean DEFAULT false NOT NULL,
    events_enabled boolean DEFAULT false NOT NULL,
    events_expiration bigint,
    login_theme character varying(255),
    name character varying(255),
    not_before integer,
    password_policy character varying(2550),
    registration_allowed boolean DEFAULT false NOT NULL,
    remember_me boolean DEFAULT false NOT NULL,
    reset_password_allowed boolean DEFAULT false NOT NULL,
    social boolean DEFAULT false NOT NULL,
    ssl_required character varying(255),
    sso_idle_timeout integer,
    sso_max_lifespan integer,
    update_profile_on_soc_login boolean DEFAULT false NOT NULL,
    verify_email boolean DEFAULT false NOT NULL,
    master_admin_client character varying(36),
    login_lifespan integer,
    internationalization_enabled boolean DEFAULT false NOT NULL,
    default_locale character varying(255),
    reg_email_as_username boolean DEFAULT false NOT NULL,
    admin_events_enabled boolean DEFAULT false NOT NULL,
    admin_events_details_enabled boolean DEFAULT false NOT NULL,
    edit_username_allowed boolean DEFAULT false NOT NULL,
    otp_policy_counter integer DEFAULT 0,
    otp_policy_window integer DEFAULT 1,
    otp_policy_period integer DEFAULT 30,
    otp_policy_digits integer DEFAULT 6,
    otp_policy_alg character varying(36) DEFAULT 'HmacSHA1'::character varying,
    otp_policy_type character varying(36) DEFAULT 'totp'::character varying,
    browser_flow character varying(36),
    registration_flow character varying(36),
    direct_grant_flow character varying(36),
    reset_credentials_flow character varying(36),
    client_auth_flow character varying(36),
    offline_session_idle_timeout integer DEFAULT 0,
    revoke_refresh_token boolean DEFAULT false NOT NULL,
    access_token_life_implicit integer DEFAULT 0,
    login_with_email_allowed boolean DEFAULT true NOT NULL,
    duplicate_emails_allowed boolean DEFAULT false NOT NULL,
    docker_auth_flow character varying(36),
    refresh_token_max_reuse integer DEFAULT 0,
    allow_user_managed_access boolean DEFAULT false NOT NULL,
    sso_max_lifespan_remember_me integer DEFAULT 0 NOT NULL,
    sso_idle_timeout_remember_me integer DEFAULT 0 NOT NULL,
    default_role character varying(255)
);


ALTER TABLE public.realm OWNER TO keycloakuser;

--
-- TOC entry 223 (class 1259 OID 24648)
-- Name: realm_attribute; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.realm_attribute (
    name character varying(255) NOT NULL,
    realm_id character varying(36) NOT NULL,
    value text
);


ALTER TABLE public.realm_attribute OWNER TO keycloakuser;

--
-- TOC entry 264 (class 1259 OID 25405)
-- Name: realm_default_groups; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.realm_default_groups (
    realm_id character varying(36) NOT NULL,
    group_id character varying(36) NOT NULL
);


ALTER TABLE public.realm_default_groups OWNER TO keycloakuser;

--
-- TOC entry 244 (class 1259 OID 25101)
-- Name: realm_enabled_event_types; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.realm_enabled_event_types (
    realm_id character varying(36) NOT NULL,
    value character varying(255) NOT NULL
);


ALTER TABLE public.realm_enabled_event_types OWNER TO keycloakuser;

--
-- TOC entry 224 (class 1259 OID 24656)
-- Name: realm_events_listeners; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.realm_events_listeners (
    realm_id character varying(36) NOT NULL,
    value character varying(255) NOT NULL
);


ALTER TABLE public.realm_events_listeners OWNER TO keycloakuser;

--
-- TOC entry 297 (class 1259 OID 26108)
-- Name: realm_localizations; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.realm_localizations (
    realm_id character varying(255) NOT NULL,
    locale character varying(255) NOT NULL,
    texts text NOT NULL
);


ALTER TABLE public.realm_localizations OWNER TO keycloakuser;

--
-- TOC entry 225 (class 1259 OID 24659)
-- Name: realm_required_credential; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.realm_required_credential (
    type character varying(255) NOT NULL,
    form_label character varying(255),
    input boolean DEFAULT false NOT NULL,
    secret boolean DEFAULT false NOT NULL,
    realm_id character varying(36) NOT NULL
);


ALTER TABLE public.realm_required_credential OWNER TO keycloakuser;

--
-- TOC entry 226 (class 1259 OID 24666)
-- Name: realm_smtp_config; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.realm_smtp_config (
    realm_id character varying(36) NOT NULL,
    value character varying(255),
    name character varying(255) NOT NULL
);


ALTER TABLE public.realm_smtp_config OWNER TO keycloakuser;

--
-- TOC entry 243 (class 1259 OID 25017)
-- Name: realm_supported_locales; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.realm_supported_locales (
    realm_id character varying(36) NOT NULL,
    value character varying(255) NOT NULL
);


ALTER TABLE public.realm_supported_locales OWNER TO keycloakuser;

--
-- TOC entry 227 (class 1259 OID 24676)
-- Name: redirect_uris; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.redirect_uris (
    client_id character varying(36) NOT NULL,
    value character varying(255) NOT NULL
);


ALTER TABLE public.redirect_uris OWNER TO keycloakuser;

--
-- TOC entry 257 (class 1259 OID 25340)
-- Name: required_action_config; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.required_action_config (
    required_action_id character varying(36) NOT NULL,
    value text,
    name character varying(255) NOT NULL
);


ALTER TABLE public.required_action_config OWNER TO keycloakuser;

--
-- TOC entry 256 (class 1259 OID 25333)
-- Name: required_action_provider; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.required_action_provider (
    id character varying(36) NOT NULL,
    alias character varying(255),
    name character varying(255),
    realm_id character varying(36),
    enabled boolean DEFAULT false NOT NULL,
    default_action boolean DEFAULT false NOT NULL,
    provider_id character varying(255),
    priority integer
);


ALTER TABLE public.required_action_provider OWNER TO keycloakuser;

--
-- TOC entry 294 (class 1259 OID 26039)
-- Name: resource_attribute; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.resource_attribute (
    id character varying(36) DEFAULT 'sybase-needs-something-here'::character varying NOT NULL,
    name character varying(255) NOT NULL,
    value character varying(255),
    resource_id character varying(36) NOT NULL
);


ALTER TABLE public.resource_attribute OWNER TO keycloakuser;

--
-- TOC entry 274 (class 1259 OID 25622)
-- Name: resource_policy; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.resource_policy (
    resource_id character varying(36) NOT NULL,
    policy_id character varying(36) NOT NULL
);


ALTER TABLE public.resource_policy OWNER TO keycloakuser;

--
-- TOC entry 273 (class 1259 OID 25607)
-- Name: resource_scope; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.resource_scope (
    resource_id character varying(36) NOT NULL,
    scope_id character varying(36) NOT NULL
);


ALTER TABLE public.resource_scope OWNER TO keycloakuser;

--
-- TOC entry 268 (class 1259 OID 25545)
-- Name: resource_server; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.resource_server (
    id character varying(36) NOT NULL,
    allow_rs_remote_mgmt boolean DEFAULT false NOT NULL,
    policy_enforce_mode smallint NOT NULL,
    decision_strategy smallint DEFAULT 1 NOT NULL
);


ALTER TABLE public.resource_server OWNER TO keycloakuser;

--
-- TOC entry 293 (class 1259 OID 26015)
-- Name: resource_server_perm_ticket; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.resource_server_perm_ticket (
    id character varying(36) NOT NULL,
    owner character varying(255) NOT NULL,
    requester character varying(255) NOT NULL,
    created_timestamp bigint NOT NULL,
    granted_timestamp bigint,
    resource_id character varying(36) NOT NULL,
    scope_id character varying(36),
    resource_server_id character varying(36) NOT NULL,
    policy_id character varying(36)
);


ALTER TABLE public.resource_server_perm_ticket OWNER TO keycloakuser;

--
-- TOC entry 271 (class 1259 OID 25581)
-- Name: resource_server_policy; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.resource_server_policy (
    id character varying(36) NOT NULL,
    name character varying(255) NOT NULL,
    description character varying(255),
    type character varying(255) NOT NULL,
    decision_strategy smallint,
    logic smallint,
    resource_server_id character varying(36) NOT NULL,
    owner character varying(255)
);


ALTER TABLE public.resource_server_policy OWNER TO keycloakuser;

--
-- TOC entry 269 (class 1259 OID 25553)
-- Name: resource_server_resource; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.resource_server_resource (
    id character varying(36) NOT NULL,
    name character varying(255) NOT NULL,
    type character varying(255),
    icon_uri character varying(255),
    owner character varying(255) NOT NULL,
    resource_server_id character varying(36) NOT NULL,
    owner_managed_access boolean DEFAULT false NOT NULL,
    display_name character varying(255)
);


ALTER TABLE public.resource_server_resource OWNER TO keycloakuser;

--
-- TOC entry 270 (class 1259 OID 25567)
-- Name: resource_server_scope; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.resource_server_scope (
    id character varying(36) NOT NULL,
    name character varying(255) NOT NULL,
    icon_uri character varying(255),
    resource_server_id character varying(36) NOT NULL,
    display_name character varying(255)
);


ALTER TABLE public.resource_server_scope OWNER TO keycloakuser;

--
-- TOC entry 295 (class 1259 OID 26057)
-- Name: resource_uris; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.resource_uris (
    resource_id character varying(36) NOT NULL,
    value character varying(255) NOT NULL
);


ALTER TABLE public.resource_uris OWNER TO keycloakuser;

--
-- TOC entry 300 (class 1259 OID 26193)
-- Name: revoked_token; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.revoked_token (
    id character varying(255) NOT NULL,
    expire bigint NOT NULL
);


ALTER TABLE public.revoked_token OWNER TO keycloakuser;

--
-- TOC entry 296 (class 1259 OID 26067)
-- Name: role_attribute; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.role_attribute (
    id character varying(36) NOT NULL,
    role_id character varying(36) NOT NULL,
    name character varying(255) NOT NULL,
    value character varying(255)
);


ALTER TABLE public.role_attribute OWNER TO keycloakuser;

--
-- TOC entry 228 (class 1259 OID 24679)
-- Name: scope_mapping; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.scope_mapping (
    client_id character varying(36) NOT NULL,
    role_id character varying(36) NOT NULL
);


ALTER TABLE public.scope_mapping OWNER TO keycloakuser;

--
-- TOC entry 275 (class 1259 OID 25637)
-- Name: scope_policy; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.scope_policy (
    scope_id character varying(36) NOT NULL,
    policy_id character varying(36) NOT NULL
);


ALTER TABLE public.scope_policy OWNER TO keycloakuser;

--
-- TOC entry 229 (class 1259 OID 24685)
-- Name: user_attribute; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.user_attribute (
    name character varying(255) NOT NULL,
    value character varying(255),
    user_id character varying(36) NOT NULL,
    id character varying(36) DEFAULT 'sybase-needs-something-here'::character varying NOT NULL,
    long_value_hash bytea,
    long_value_hash_lower_case bytea,
    long_value text
);


ALTER TABLE public.user_attribute OWNER TO keycloakuser;

--
-- TOC entry 248 (class 1259 OID 25122)
-- Name: user_consent; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.user_consent (
    id character varying(36) NOT NULL,
    client_id character varying(255),
    user_id character varying(36) NOT NULL,
    created_date bigint,
    last_updated_date bigint,
    client_storage_provider character varying(36),
    external_client_id character varying(255)
);


ALTER TABLE public.user_consent OWNER TO keycloakuser;

--
-- TOC entry 291 (class 1259 OID 25990)
-- Name: user_consent_client_scope; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.user_consent_client_scope (
    user_consent_id character varying(36) NOT NULL,
    scope_id character varying(36) NOT NULL
);


ALTER TABLE public.user_consent_client_scope OWNER TO keycloakuser;

--
-- TOC entry 230 (class 1259 OID 24690)
-- Name: user_entity; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.user_entity (
    id character varying(36) NOT NULL,
    email character varying(255),
    email_constraint character varying(255),
    email_verified boolean DEFAULT false NOT NULL,
    enabled boolean DEFAULT false NOT NULL,
    federation_link character varying(255),
    first_name character varying(255),
    last_name character varying(255),
    realm_id character varying(255),
    username character varying(255),
    created_timestamp bigint,
    service_account_client_link character varying(255),
    not_before integer DEFAULT 0 NOT NULL
);


ALTER TABLE public.user_entity OWNER TO keycloakuser;

--
-- TOC entry 231 (class 1259 OID 24698)
-- Name: user_federation_config; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.user_federation_config (
    user_federation_provider_id character varying(36) NOT NULL,
    value character varying(255),
    name character varying(255) NOT NULL
);


ALTER TABLE public.user_federation_config OWNER TO keycloakuser;

--
-- TOC entry 254 (class 1259 OID 25234)
-- Name: user_federation_mapper; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.user_federation_mapper (
    id character varying(36) NOT NULL,
    name character varying(255) NOT NULL,
    federation_provider_id character varying(36) NOT NULL,
    federation_mapper_type character varying(255) NOT NULL,
    realm_id character varying(36) NOT NULL
);


ALTER TABLE public.user_federation_mapper OWNER TO keycloakuser;

--
-- TOC entry 255 (class 1259 OID 25239)
-- Name: user_federation_mapper_config; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.user_federation_mapper_config (
    user_federation_mapper_id character varying(36) NOT NULL,
    value character varying(255),
    name character varying(255) NOT NULL
);


ALTER TABLE public.user_federation_mapper_config OWNER TO keycloakuser;

--
-- TOC entry 232 (class 1259 OID 24703)
-- Name: user_federation_provider; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.user_federation_provider (
    id character varying(36) NOT NULL,
    changed_sync_period integer,
    display_name character varying(255),
    full_sync_period integer,
    last_sync integer,
    priority integer,
    provider_name character varying(255),
    realm_id character varying(36)
);


ALTER TABLE public.user_federation_provider OWNER TO keycloakuser;

--
-- TOC entry 263 (class 1259 OID 25402)
-- Name: user_group_membership; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.user_group_membership (
    group_id character varying(36) NOT NULL,
    user_id character varying(36) NOT NULL,
    membership_type character varying(255) NOT NULL
);


ALTER TABLE public.user_group_membership OWNER TO keycloakuser;

--
-- TOC entry 233 (class 1259 OID 24708)
-- Name: user_required_action; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.user_required_action (
    user_id character varying(36) NOT NULL,
    required_action character varying(255) DEFAULT ' '::character varying NOT NULL
);


ALTER TABLE public.user_required_action OWNER TO keycloakuser;

--
-- TOC entry 234 (class 1259 OID 24711)
-- Name: user_role_mapping; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.user_role_mapping (
    role_id character varying(255) NOT NULL,
    user_id character varying(36) NOT NULL
);


ALTER TABLE public.user_role_mapping OWNER TO keycloakuser;

--
-- TOC entry 235 (class 1259 OID 24725)
-- Name: web_origins; Type: TABLE; Schema: public; Owner: keycloakuser
--

CREATE TABLE public.web_origins (
    client_id character varying(36) NOT NULL,
    value character varying(255) NOT NULL
);


ALTER TABLE public.web_origins OWNER TO keycloakuser;

--
-- TOC entry 4166 (class 0 OID 25209)
-- Dependencies: 249
-- Data for Name: admin_event_entity; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.admin_event_entity (id, admin_event_time, realm_id, operation_type, auth_realm_id, auth_client_id, auth_user_id, ip_address, resource_path, representation, error, resource_type, details_json) FROM stdin;
\.


--
-- TOC entry 4193 (class 0 OID 25652)
-- Dependencies: 276
-- Data for Name: associated_policy; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.associated_policy (policy_id, associated_policy_id) FROM stdin;
\.


--
-- TOC entry 4169 (class 0 OID 25224)
-- Dependencies: 252
-- Data for Name: authentication_execution; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.authentication_execution (id, alias, authenticator, realm_id, flow_id, requirement, priority, authenticator_flow, auth_flow_id, auth_config) FROM stdin;
7918c194-9712-455c-a1e5-c5c9fd230afd	\N	auth-cookie	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9e4ca14f-c769-445f-9823-0ee4e7f5995f	2	10	f	\N	\N
9bd1e444-7365-459a-9be1-34aabc20298b	\N	auth-spnego	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9e4ca14f-c769-445f-9823-0ee4e7f5995f	3	20	f	\N	\N
0232dbb4-88fa-498d-829f-2893306c8be0	\N	identity-provider-redirector	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9e4ca14f-c769-445f-9823-0ee4e7f5995f	2	25	f	\N	\N
ef6ac8d4-ae61-4694-8f3f-7bc950476d60	\N	\N	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9e4ca14f-c769-445f-9823-0ee4e7f5995f	2	30	t	baf3eba3-9636-44da-94ec-c589dcd0eeb8	\N
2c1ee7ad-ade8-4bf7-900c-768b2ad6d5fa	\N	auth-username-password-form	9ccc2b44-8d11-4694-87e4-8e194b225e1d	baf3eba3-9636-44da-94ec-c589dcd0eeb8	0	10	f	\N	\N
24d5d082-9036-4402-b740-1c2ff3215f3c	\N	\N	9ccc2b44-8d11-4694-87e4-8e194b225e1d	baf3eba3-9636-44da-94ec-c589dcd0eeb8	1	20	t	ead1e2d2-46ab-441a-8104-c9545d76a681	\N
83492eb9-40e6-4c14-b140-3648e5ce3464	\N	conditional-user-configured	9ccc2b44-8d11-4694-87e4-8e194b225e1d	ead1e2d2-46ab-441a-8104-c9545d76a681	0	10	f	\N	\N
8ee00e14-29f7-4393-b2e4-3cd8d252d66f	\N	auth-otp-form	9ccc2b44-8d11-4694-87e4-8e194b225e1d	ead1e2d2-46ab-441a-8104-c9545d76a681	0	20	f	\N	\N
6b451b98-7b97-4d3d-a8d7-5d846565d2b1	\N	direct-grant-validate-username	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8bbfc34c-d8d9-48c1-b79a-f955cb70b8bb	0	10	f	\N	\N
39958f6b-0ed6-4428-9437-e16c15123ddb	\N	direct-grant-validate-password	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8bbfc34c-d8d9-48c1-b79a-f955cb70b8bb	0	20	f	\N	\N
a80b2bd0-ee46-468c-904a-155a8ae05c81	\N	\N	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8bbfc34c-d8d9-48c1-b79a-f955cb70b8bb	1	30	t	87916976-0dfb-419b-be9b-a23a9f8b59f6	\N
d1abd7c2-f376-4db8-bc00-2ca59797161d	\N	conditional-user-configured	9ccc2b44-8d11-4694-87e4-8e194b225e1d	87916976-0dfb-419b-be9b-a23a9f8b59f6	0	10	f	\N	\N
683a217b-aa43-4883-959b-f822614e95f0	\N	direct-grant-validate-otp	9ccc2b44-8d11-4694-87e4-8e194b225e1d	87916976-0dfb-419b-be9b-a23a9f8b59f6	0	20	f	\N	\N
9eb3242e-af9f-4215-b0eb-061e8be1dcec	\N	registration-page-form	9ccc2b44-8d11-4694-87e4-8e194b225e1d	eeb5d5fc-01af-4213-a5a3-cbd285fb578b	0	10	t	2ffafffd-1a7c-48d6-8a4b-57bdd6d32579	\N
b3c0fe08-815a-49ea-8984-a50230e6908c	\N	registration-user-creation	9ccc2b44-8d11-4694-87e4-8e194b225e1d	2ffafffd-1a7c-48d6-8a4b-57bdd6d32579	0	20	f	\N	\N
872f0ea7-8dc7-476a-bdbf-51b1356e79ba	\N	registration-password-action	9ccc2b44-8d11-4694-87e4-8e194b225e1d	2ffafffd-1a7c-48d6-8a4b-57bdd6d32579	0	50	f	\N	\N
4593e9ff-b8b2-436a-91de-ec1b48549243	\N	registration-recaptcha-action	9ccc2b44-8d11-4694-87e4-8e194b225e1d	2ffafffd-1a7c-48d6-8a4b-57bdd6d32579	3	60	f	\N	\N
22de0a56-dd73-4359-98ca-8b4c0fe01486	\N	registration-terms-and-conditions	9ccc2b44-8d11-4694-87e4-8e194b225e1d	2ffafffd-1a7c-48d6-8a4b-57bdd6d32579	3	70	f	\N	\N
a65b7690-71be-4c17-84e4-d7455f8df762	\N	reset-credentials-choose-user	9ccc2b44-8d11-4694-87e4-8e194b225e1d	37d5edc9-7089-4b85-ac73-d99fb431baf4	0	10	f	\N	\N
5c4aab06-cb2a-4764-9e6a-b9bf4f736b16	\N	reset-credential-email	9ccc2b44-8d11-4694-87e4-8e194b225e1d	37d5edc9-7089-4b85-ac73-d99fb431baf4	0	20	f	\N	\N
457a4b18-9c4b-4dd1-b296-a39f5242e9d5	\N	reset-password	9ccc2b44-8d11-4694-87e4-8e194b225e1d	37d5edc9-7089-4b85-ac73-d99fb431baf4	0	30	f	\N	\N
63dfbcec-8f49-4a85-a0d0-3b7a446a05a4	\N	\N	9ccc2b44-8d11-4694-87e4-8e194b225e1d	37d5edc9-7089-4b85-ac73-d99fb431baf4	1	40	t	5cd838d0-366f-4727-86d3-c249445bb0b5	\N
8be99067-d8cd-452c-bca5-67ec04358280	\N	conditional-user-configured	9ccc2b44-8d11-4694-87e4-8e194b225e1d	5cd838d0-366f-4727-86d3-c249445bb0b5	0	10	f	\N	\N
17294db0-b362-4b9b-b823-88c072bb58c6	\N	reset-otp	9ccc2b44-8d11-4694-87e4-8e194b225e1d	5cd838d0-366f-4727-86d3-c249445bb0b5	0	20	f	\N	\N
f4e36a27-629d-42c8-9d28-61fd2a2a086b	\N	client-secret	9ccc2b44-8d11-4694-87e4-8e194b225e1d	f11b41ec-6388-4979-b2e9-7cf504fcb9ca	2	10	f	\N	\N
0b9db369-1d41-4ec7-91b9-f4c8474747f8	\N	client-jwt	9ccc2b44-8d11-4694-87e4-8e194b225e1d	f11b41ec-6388-4979-b2e9-7cf504fcb9ca	2	20	f	\N	\N
4eb369f7-e935-4cc4-adfa-166b8151c741	\N	client-secret-jwt	9ccc2b44-8d11-4694-87e4-8e194b225e1d	f11b41ec-6388-4979-b2e9-7cf504fcb9ca	2	30	f	\N	\N
571d07eb-8622-45a9-a0ba-af1e1674b962	\N	client-x509	9ccc2b44-8d11-4694-87e4-8e194b225e1d	f11b41ec-6388-4979-b2e9-7cf504fcb9ca	2	40	f	\N	\N
27072fba-7066-48f5-8e80-ea5c7b61601a	\N	idp-review-profile	9ccc2b44-8d11-4694-87e4-8e194b225e1d	a3f4378f-2789-4495-a910-8063b5661a03	0	10	f	\N	0d7c904a-3307-468c-9aca-890a8ed41c87
db306678-5711-4602-8689-664f92d4ae06	\N	\N	9ccc2b44-8d11-4694-87e4-8e194b225e1d	a3f4378f-2789-4495-a910-8063b5661a03	0	20	t	5f95c3c9-6993-4c4e-91b7-bb3f2d323f02	\N
2119b449-c56e-4bae-b718-99b8a1b22a94	\N	idp-create-user-if-unique	9ccc2b44-8d11-4694-87e4-8e194b225e1d	5f95c3c9-6993-4c4e-91b7-bb3f2d323f02	2	10	f	\N	c917c2bc-8e92-4e00-ba29-7148da34eb8c
f51661f1-db18-4f24-a319-6dffea329bfe	\N	\N	9ccc2b44-8d11-4694-87e4-8e194b225e1d	5f95c3c9-6993-4c4e-91b7-bb3f2d323f02	2	20	t	8d19efa0-309c-42b7-b8fc-993c180ab580	\N
c743e451-cec2-4a22-89fc-7cd819882528	\N	idp-confirm-link	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8d19efa0-309c-42b7-b8fc-993c180ab580	0	10	f	\N	\N
b42e1404-24eb-478c-85cc-0f91197ca304	\N	\N	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8d19efa0-309c-42b7-b8fc-993c180ab580	0	20	t	29f84f96-3f02-48ba-a10a-e8bfa9c109f4	\N
13a17bfe-21b9-4100-bb06-740b316af201	\N	idp-email-verification	9ccc2b44-8d11-4694-87e4-8e194b225e1d	29f84f96-3f02-48ba-a10a-e8bfa9c109f4	2	10	f	\N	\N
b08716df-211e-47ab-bbb6-3a605f33da4e	\N	\N	9ccc2b44-8d11-4694-87e4-8e194b225e1d	29f84f96-3f02-48ba-a10a-e8bfa9c109f4	2	20	t	75ee0ef1-dfc3-4b0f-9e36-ba9fc855d19f	\N
bc2908f6-b079-432e-b6dc-590860bcff8d	\N	idp-username-password-form	9ccc2b44-8d11-4694-87e4-8e194b225e1d	75ee0ef1-dfc3-4b0f-9e36-ba9fc855d19f	0	10	f	\N	\N
c84e402d-2557-4111-b490-5993484de084	\N	\N	9ccc2b44-8d11-4694-87e4-8e194b225e1d	75ee0ef1-dfc3-4b0f-9e36-ba9fc855d19f	1	20	t	801dc327-c0ce-409c-a616-4fc1ecbe9fb3	\N
65dde288-fb4f-49ce-a970-bcfca45e7608	\N	conditional-user-configured	9ccc2b44-8d11-4694-87e4-8e194b225e1d	801dc327-c0ce-409c-a616-4fc1ecbe9fb3	0	10	f	\N	\N
bc83615e-8a0c-4b4a-a6bd-5964c6a63dd4	\N	auth-otp-form	9ccc2b44-8d11-4694-87e4-8e194b225e1d	801dc327-c0ce-409c-a616-4fc1ecbe9fb3	0	20	f	\N	\N
3440617b-e984-4bde-9ed0-5a383328aaa8	\N	http-basic-authenticator	9ccc2b44-8d11-4694-87e4-8e194b225e1d	a25f235e-ed97-489f-8058-52b469fbaaf7	0	10	f	\N	\N
065e4c1d-5b14-4685-8971-5659a4a637d9	\N	docker-http-basic-authenticator	9ccc2b44-8d11-4694-87e4-8e194b225e1d	3ad6bc93-f13c-4f59-9680-33b92c2d92b8	0	10	f	\N	\N
165e10e4-863c-4021-a129-ff77b4b07c51	\N	auth-cookie	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	905d0947-4059-4fc3-a85f-30a710778cf1	2	10	f	\N	\N
e6f245ff-2216-422e-9525-3c12f4b83f8b	\N	auth-spnego	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	905d0947-4059-4fc3-a85f-30a710778cf1	3	20	f	\N	\N
c01e74a2-d26e-47bf-ac5c-cf1f71f252d7	\N	identity-provider-redirector	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	905d0947-4059-4fc3-a85f-30a710778cf1	2	25	f	\N	\N
dfd140d6-c67d-4216-b844-457c56794deb	\N	\N	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	905d0947-4059-4fc3-a85f-30a710778cf1	2	30	t	784063aa-84d0-4c40-9c41-b493bbda570e	\N
3c5e6246-aa36-4cd0-aaed-1459cd867a36	\N	auth-username-password-form	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	784063aa-84d0-4c40-9c41-b493bbda570e	0	10	f	\N	\N
1148fdfd-375d-43cf-876c-82d1233d26f8	\N	\N	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	784063aa-84d0-4c40-9c41-b493bbda570e	1	20	t	f6b82100-499e-4cce-8805-53626fa5dc7c	\N
5bb5ec42-8269-482d-b74f-da5080c2b8be	\N	conditional-user-configured	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f6b82100-499e-4cce-8805-53626fa5dc7c	0	10	f	\N	\N
c768648e-e432-495b-93e6-867d87bfbcba	\N	auth-otp-form	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f6b82100-499e-4cce-8805-53626fa5dc7c	0	20	f	\N	\N
8e205bfe-1f93-4856-8d7a-c93a23f60500	\N	\N	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	905d0947-4059-4fc3-a85f-30a710778cf1	2	26	t	3cd8cf0f-2c34-470b-83f1-8a14d486d5e3	\N
cac14545-455c-4b1c-8f97-26d1548815dd	\N	\N	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	3cd8cf0f-2c34-470b-83f1-8a14d486d5e3	1	10	t	1be4c33a-6cb0-4747-930f-dc905505e553	\N
699b4797-556f-42b9-bc32-34f2f784f4e6	\N	conditional-user-configured	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	1be4c33a-6cb0-4747-930f-dc905505e553	0	10	f	\N	\N
e2b9fb28-70cc-4793-907e-a2f6083a73fd	\N	organization	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	1be4c33a-6cb0-4747-930f-dc905505e553	2	20	f	\N	\N
d40c4e7b-1786-4eb1-b1e6-f6220f6b50dd	\N	direct-grant-validate-username	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	1fa01ed5-af2b-434e-a1b3-ab153785f0ef	0	10	f	\N	\N
b2eee87e-55bd-44f6-ae62-f872be4e34e0	\N	direct-grant-validate-password	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	1fa01ed5-af2b-434e-a1b3-ab153785f0ef	0	20	f	\N	\N
ff498c0f-344f-4b95-a082-18827eb3839d	\N	\N	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	1fa01ed5-af2b-434e-a1b3-ab153785f0ef	1	30	t	3f7a69fc-57c6-4b44-aa4b-6d14c6237dd8	\N
97deba1e-1269-406e-ba18-7c69778bae63	\N	conditional-user-configured	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	3f7a69fc-57c6-4b44-aa4b-6d14c6237dd8	0	10	f	\N	\N
eb349151-dafa-4fbe-a0e9-3c2db5997173	\N	direct-grant-validate-otp	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	3f7a69fc-57c6-4b44-aa4b-6d14c6237dd8	0	20	f	\N	\N
1ed62397-b212-4a10-a5fc-0e1b5ea6a7bb	\N	registration-page-form	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	e6a564ce-e81a-4447-a408-448dc4b24859	0	10	t	c22fae5b-a604-431a-ac7e-9afa8a7bfc6f	\N
2e661193-f534-4578-91fa-995c635dbd4b	\N	registration-user-creation	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	c22fae5b-a604-431a-ac7e-9afa8a7bfc6f	0	20	f	\N	\N
b6a3a086-d817-4fc3-9b07-7d8625a2b5c3	\N	registration-password-action	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	c22fae5b-a604-431a-ac7e-9afa8a7bfc6f	0	50	f	\N	\N
f7a38467-6687-4354-872d-397ad3809c94	\N	registration-recaptcha-action	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	c22fae5b-a604-431a-ac7e-9afa8a7bfc6f	3	60	f	\N	\N
71e43beb-63f1-4e75-ac00-22c25984aead	\N	registration-terms-and-conditions	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	c22fae5b-a604-431a-ac7e-9afa8a7bfc6f	3	70	f	\N	\N
42b0a3aa-675e-47cb-83e0-a14ce10dffe4	\N	reset-credentials-choose-user	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	0f75fe80-a603-4c1a-b430-4e6e306b45d0	0	10	f	\N	\N
cf97cfad-d26f-4041-b58d-a7f46f06dd31	\N	reset-credential-email	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	0f75fe80-a603-4c1a-b430-4e6e306b45d0	0	20	f	\N	\N
625e83dd-e7b5-4598-a798-c741534c5c88	\N	reset-password	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	0f75fe80-a603-4c1a-b430-4e6e306b45d0	0	30	f	\N	\N
313827fe-695d-48c2-a821-1a0da6053639	\N	\N	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	0f75fe80-a603-4c1a-b430-4e6e306b45d0	1	40	t	90d8d138-0c8c-4d94-bbd3-7ce9ab94c363	\N
9ede1a8d-aa96-4a05-8181-c5dc1deafdd1	\N	conditional-user-configured	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	90d8d138-0c8c-4d94-bbd3-7ce9ab94c363	0	10	f	\N	\N
16a90870-8d8c-4737-840c-7de7ac73eb9c	\N	reset-otp	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	90d8d138-0c8c-4d94-bbd3-7ce9ab94c363	0	20	f	\N	\N
58910165-e614-475b-8000-f196438b0fd5	\N	client-secret	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	c424c49c-aa15-4816-b9ef-efba43376b05	2	10	f	\N	\N
6f52faa5-2afe-4ffd-9989-5e46c141e4ef	\N	client-jwt	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	c424c49c-aa15-4816-b9ef-efba43376b05	2	20	f	\N	\N
5ae31014-b096-487a-a157-449ea14502eb	\N	client-secret-jwt	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	c424c49c-aa15-4816-b9ef-efba43376b05	2	30	f	\N	\N
c9c55380-97b7-4d92-b267-93259ed92c2a	\N	client-x509	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	c424c49c-aa15-4816-b9ef-efba43376b05	2	40	f	\N	\N
0a6cbe6b-1f39-45b6-bc88-f0ac04d38bd4	\N	idp-review-profile	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	d64f7b8b-da55-434a-93d6-45e763615e6a	0	10	f	\N	26eb4427-54e4-4b41-9033-f1a8761eac25
a6f5cbe6-7c71-4c55-906c-280eeaae65d5	\N	\N	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	d64f7b8b-da55-434a-93d6-45e763615e6a	0	20	t	ac40aca0-27c5-408d-93cf-74b43709c774	\N
cba51a5d-8e26-4aa0-88c0-7d11412b4d81	\N	idp-create-user-if-unique	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	ac40aca0-27c5-408d-93cf-74b43709c774	2	10	f	\N	ae747fe2-e10f-4c21-b3d5-2967ced346bb
7c38c5b3-d350-4960-955e-102abb36c1f3	\N	\N	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	ac40aca0-27c5-408d-93cf-74b43709c774	2	20	t	192eecbb-95f2-456e-8c9e-3dd6d5c0898e	\N
77c18192-bc70-4c76-96d4-bec0aa3e4bdf	\N	idp-confirm-link	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	192eecbb-95f2-456e-8c9e-3dd6d5c0898e	0	10	f	\N	\N
41c0c8db-15a2-4c3d-90eb-14d40e3246a1	\N	\N	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	192eecbb-95f2-456e-8c9e-3dd6d5c0898e	0	20	t	c13bf4dd-2c47-44bf-887e-8133566fac73	\N
b31b7195-8dc4-431b-85a0-35b5965c8dd0	\N	idp-email-verification	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	c13bf4dd-2c47-44bf-887e-8133566fac73	2	10	f	\N	\N
585d16ea-3b6c-4685-b17a-494e076418a8	\N	\N	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	c13bf4dd-2c47-44bf-887e-8133566fac73	2	20	t	1ed6507f-1a44-45b6-872f-134a9a2c323d	\N
e0a14a3e-9438-48e4-a303-7060622f0333	\N	idp-username-password-form	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	1ed6507f-1a44-45b6-872f-134a9a2c323d	0	10	f	\N	\N
23762881-552c-41a6-b2c6-e62c627c786c	\N	\N	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	1ed6507f-1a44-45b6-872f-134a9a2c323d	1	20	t	482d5253-9a56-4199-8bda-09e2f504ae0a	\N
ba7814f1-4310-417c-81b0-45bfc2f0e084	\N	conditional-user-configured	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	482d5253-9a56-4199-8bda-09e2f504ae0a	0	10	f	\N	\N
310548d8-c3df-4d11-9262-4a7c56a1e37b	\N	auth-otp-form	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	482d5253-9a56-4199-8bda-09e2f504ae0a	0	20	f	\N	\N
f0b6182d-0ecb-4aef-8dda-ca3c664ded49	\N	\N	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	d64f7b8b-da55-434a-93d6-45e763615e6a	1	50	t	4c2ad038-6e89-4763-b894-24116ef355ba	\N
06b2454a-c8c9-4bc5-98d6-aa5fac431717	\N	conditional-user-configured	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	4c2ad038-6e89-4763-b894-24116ef355ba	0	10	f	\N	\N
58878567-d3ca-4cd7-9a39-145b160950fe	\N	idp-add-organization-member	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	4c2ad038-6e89-4763-b894-24116ef355ba	0	20	f	\N	\N
9b5214a8-246f-4707-b8bd-aaefec5f2049	\N	http-basic-authenticator	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	822b04b3-fe84-4c4b-b109-39de7a844cd5	0	10	f	\N	\N
bf5fa28d-bf60-4699-843f-076a452fae10	\N	docker-http-basic-authenticator	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	5bf4fd2b-7ee7-4684-9d34-82bbac5b2483	0	10	f	\N	\N
\.


--
-- TOC entry 4168 (class 0 OID 25219)
-- Dependencies: 251
-- Data for Name: authentication_flow; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.authentication_flow (id, alias, description, realm_id, provider_id, top_level, built_in) FROM stdin;
9e4ca14f-c769-445f-9823-0ee4e7f5995f	browser	Browser based authentication	9ccc2b44-8d11-4694-87e4-8e194b225e1d	basic-flow	t	t
baf3eba3-9636-44da-94ec-c589dcd0eeb8	forms	Username, password, otp and other auth forms.	9ccc2b44-8d11-4694-87e4-8e194b225e1d	basic-flow	f	t
ead1e2d2-46ab-441a-8104-c9545d76a681	Browser - Conditional OTP	Flow to determine if the OTP is required for the authentication	9ccc2b44-8d11-4694-87e4-8e194b225e1d	basic-flow	f	t
8bbfc34c-d8d9-48c1-b79a-f955cb70b8bb	direct grant	OpenID Connect Resource Owner Grant	9ccc2b44-8d11-4694-87e4-8e194b225e1d	basic-flow	t	t
87916976-0dfb-419b-be9b-a23a9f8b59f6	Direct Grant - Conditional OTP	Flow to determine if the OTP is required for the authentication	9ccc2b44-8d11-4694-87e4-8e194b225e1d	basic-flow	f	t
eeb5d5fc-01af-4213-a5a3-cbd285fb578b	registration	Registration flow	9ccc2b44-8d11-4694-87e4-8e194b225e1d	basic-flow	t	t
2ffafffd-1a7c-48d6-8a4b-57bdd6d32579	registration form	Registration form	9ccc2b44-8d11-4694-87e4-8e194b225e1d	form-flow	f	t
37d5edc9-7089-4b85-ac73-d99fb431baf4	reset credentials	Reset credentials for a user if they forgot their password or something	9ccc2b44-8d11-4694-87e4-8e194b225e1d	basic-flow	t	t
5cd838d0-366f-4727-86d3-c249445bb0b5	Reset - Conditional OTP	Flow to determine if the OTP should be reset or not. Set to REQUIRED to force.	9ccc2b44-8d11-4694-87e4-8e194b225e1d	basic-flow	f	t
f11b41ec-6388-4979-b2e9-7cf504fcb9ca	clients	Base authentication for clients	9ccc2b44-8d11-4694-87e4-8e194b225e1d	client-flow	t	t
a3f4378f-2789-4495-a910-8063b5661a03	first broker login	Actions taken after first broker login with identity provider account, which is not yet linked to any Keycloak account	9ccc2b44-8d11-4694-87e4-8e194b225e1d	basic-flow	t	t
5f95c3c9-6993-4c4e-91b7-bb3f2d323f02	User creation or linking	Flow for the existing/non-existing user alternatives	9ccc2b44-8d11-4694-87e4-8e194b225e1d	basic-flow	f	t
8d19efa0-309c-42b7-b8fc-993c180ab580	Handle Existing Account	Handle what to do if there is existing account with same email/username like authenticated identity provider	9ccc2b44-8d11-4694-87e4-8e194b225e1d	basic-flow	f	t
29f84f96-3f02-48ba-a10a-e8bfa9c109f4	Account verification options	Method with which to verity the existing account	9ccc2b44-8d11-4694-87e4-8e194b225e1d	basic-flow	f	t
75ee0ef1-dfc3-4b0f-9e36-ba9fc855d19f	Verify Existing Account by Re-authentication	Reauthentication of existing account	9ccc2b44-8d11-4694-87e4-8e194b225e1d	basic-flow	f	t
801dc327-c0ce-409c-a616-4fc1ecbe9fb3	First broker login - Conditional OTP	Flow to determine if the OTP is required for the authentication	9ccc2b44-8d11-4694-87e4-8e194b225e1d	basic-flow	f	t
a25f235e-ed97-489f-8058-52b469fbaaf7	saml ecp	SAML ECP Profile Authentication Flow	9ccc2b44-8d11-4694-87e4-8e194b225e1d	basic-flow	t	t
3ad6bc93-f13c-4f59-9680-33b92c2d92b8	docker auth	Used by Docker clients to authenticate against the IDP	9ccc2b44-8d11-4694-87e4-8e194b225e1d	basic-flow	t	t
905d0947-4059-4fc3-a85f-30a710778cf1	browser	Browser based authentication	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	t	t
784063aa-84d0-4c40-9c41-b493bbda570e	forms	Username, password, otp and other auth forms.	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	f	t
f6b82100-499e-4cce-8805-53626fa5dc7c	Browser - Conditional OTP	Flow to determine if the OTP is required for the authentication	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	f	t
3cd8cf0f-2c34-470b-83f1-8a14d486d5e3	Organization	\N	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	f	t
1be4c33a-6cb0-4747-930f-dc905505e553	Browser - Conditional Organization	Flow to determine if the organization identity-first login is to be used	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	f	t
1fa01ed5-af2b-434e-a1b3-ab153785f0ef	direct grant	OpenID Connect Resource Owner Grant	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	t	t
3f7a69fc-57c6-4b44-aa4b-6d14c6237dd8	Direct Grant - Conditional OTP	Flow to determine if the OTP is required for the authentication	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	f	t
e6a564ce-e81a-4447-a408-448dc4b24859	registration	Registration flow	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	t	t
c22fae5b-a604-431a-ac7e-9afa8a7bfc6f	registration form	Registration form	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	form-flow	f	t
0f75fe80-a603-4c1a-b430-4e6e306b45d0	reset credentials	Reset credentials for a user if they forgot their password or something	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	t	t
90d8d138-0c8c-4d94-bbd3-7ce9ab94c363	Reset - Conditional OTP	Flow to determine if the OTP should be reset or not. Set to REQUIRED to force.	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	f	t
c424c49c-aa15-4816-b9ef-efba43376b05	clients	Base authentication for clients	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	client-flow	t	t
d64f7b8b-da55-434a-93d6-45e763615e6a	first broker login	Actions taken after first broker login with identity provider account, which is not yet linked to any Keycloak account	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	t	t
ac40aca0-27c5-408d-93cf-74b43709c774	User creation or linking	Flow for the existing/non-existing user alternatives	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	f	t
192eecbb-95f2-456e-8c9e-3dd6d5c0898e	Handle Existing Account	Handle what to do if there is existing account with same email/username like authenticated identity provider	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	f	t
c13bf4dd-2c47-44bf-887e-8133566fac73	Account verification options	Method with which to verity the existing account	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	f	t
1ed6507f-1a44-45b6-872f-134a9a2c323d	Verify Existing Account by Re-authentication	Reauthentication of existing account	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	f	t
482d5253-9a56-4199-8bda-09e2f504ae0a	First broker login - Conditional OTP	Flow to determine if the OTP is required for the authentication	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	f	t
4c2ad038-6e89-4763-b894-24116ef355ba	First Broker Login - Conditional Organization	Flow to determine if the authenticator that adds organization members is to be used	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	f	t
822b04b3-fe84-4c4b-b109-39de7a844cd5	saml ecp	SAML ECP Profile Authentication Flow	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	t	t
5bf4fd2b-7ee7-4684-9d34-82bbac5b2483	docker auth	Used by Docker clients to authenticate against the IDP	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	basic-flow	t	t
\.


--
-- TOC entry 4167 (class 0 OID 25214)
-- Dependencies: 250
-- Data for Name: authenticator_config; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.authenticator_config (id, alias, realm_id) FROM stdin;
0d7c904a-3307-468c-9aca-890a8ed41c87	review profile config	9ccc2b44-8d11-4694-87e4-8e194b225e1d
c917c2bc-8e92-4e00-ba29-7148da34eb8c	create unique user config	9ccc2b44-8d11-4694-87e4-8e194b225e1d
26eb4427-54e4-4b41-9033-f1a8761eac25	review profile config	22478864-d5ad-4829-b2bf-92ed9d9ca9c8
ae747fe2-e10f-4c21-b3d5-2967ced346bb	create unique user config	22478864-d5ad-4829-b2bf-92ed9d9ca9c8
\.


--
-- TOC entry 4170 (class 0 OID 25229)
-- Dependencies: 253
-- Data for Name: authenticator_config_entry; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.authenticator_config_entry (authenticator_id, value, name) FROM stdin;
0d7c904a-3307-468c-9aca-890a8ed41c87	missing	update.profile.on.first.login
c917c2bc-8e92-4e00-ba29-7148da34eb8c	false	require.password.update.after.registration
26eb4427-54e4-4b41-9033-f1a8761eac25	missing	update.profile.on.first.login
ae747fe2-e10f-4c21-b3d5-2967ced346bb	false	require.password.update.after.registration
\.


--
-- TOC entry 4194 (class 0 OID 25667)
-- Dependencies: 277
-- Data for Name: broker_link; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.broker_link (identity_provider, storage_provider_id, realm_id, broker_user_id, broker_username, token, user_id) FROM stdin;
\.


--
-- TOC entry 4134 (class 0 OID 24590)
-- Dependencies: 217
-- Data for Name: client; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.client (id, enabled, full_scope_allowed, client_id, not_before, public_client, secret, base_url, bearer_only, management_url, surrogate_auth_required, realm_id, protocol, node_rereg_timeout, frontchannel_logout, consent_required, name, service_accounts_enabled, client_authenticator_type, root_url, description, registration_token, standard_flow_enabled, implicit_flow_enabled, direct_access_grants_enabled, always_display_in_console) FROM stdin;
9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	f	master-realm	0	f	\N	\N	t	\N	f	9ccc2b44-8d11-4694-87e4-8e194b225e1d	\N	0	f	f	master Realm	f	client-secret	\N	\N	\N	t	f	f	f
80bd01e1-a0e7-45d4-9873-540a8217051d	t	f	account	0	t	\N	/realms/master/account/	f	\N	f	9ccc2b44-8d11-4694-87e4-8e194b225e1d	openid-connect	0	f	f	${client_account}	f	client-secret	${authBaseUrl}	\N	\N	t	f	f	f
9175a02f-cf18-40a3-b322-73ed00ce1729	t	f	account-console	0	t	\N	/realms/master/account/	f	\N	f	9ccc2b44-8d11-4694-87e4-8e194b225e1d	openid-connect	0	f	f	${client_account-console}	f	client-secret	${authBaseUrl}	\N	\N	t	f	f	f
f859cd3b-5020-4869-8d00-d688f2b7b3bb	t	f	broker	0	f	\N	\N	t	\N	f	9ccc2b44-8d11-4694-87e4-8e194b225e1d	openid-connect	0	f	f	${client_broker}	f	client-secret	\N	\N	\N	t	f	f	f
6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	t	t	security-admin-console	0	t	\N	/admin/master/console/	f	\N	f	9ccc2b44-8d11-4694-87e4-8e194b225e1d	openid-connect	0	f	f	${client_security-admin-console}	f	client-secret	${authAdminUrl}	\N	\N	t	f	f	f
d8e8e4b8-045c-4709-8563-a48bf3a66b3a	t	t	admin-cli	0	t	\N	\N	f	\N	f	9ccc2b44-8d11-4694-87e4-8e194b225e1d	openid-connect	0	f	f	${client_admin-cli}	f	client-secret	\N	\N	\N	f	f	t	f
f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	f	realm-management	0	f	\N	\N	t	\N	f	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	openid-connect	0	f	f	${client_realm-management}	f	client-secret	\N	\N	\N	t	f	f	f
157360ea-5728-4d05-877e-9bf20421e380	t	f	broker	0	f	\N	\N	t	\N	f	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	openid-connect	0	f	f	${client_broker}	f	client-secret	\N	\N	\N	t	f	f	f
32e509de-618c-43e7-a7fc-7dafe19d8bf8	t	t	admin-cli	0	t	\N	\N	f	\N	f	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	openid-connect	0	f	f	${client_admin-cli}	f	client-secret	\N	\N	\N	f	f	t	f
8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	f	bambaiba-realm	0	f	\N	\N	t	\N	f	9ccc2b44-8d11-4694-87e4-8e194b225e1d	\N	0	f	f	bambaiba-realm Realm	f	client-secret	\N	\N	\N	t	f	f	f
d8d4e759-1346-4d49-8cb2-8d96ba570f07	t	t	bambaiba-admin-client	0	f	X8wmErNGX4tl3sJyrtSIfqTan1i3agyh		f	http://localhost:18081	f	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	openid-connect	-1	t	f		t	client-secret			\N	f	f	t	f
7e903410-f31f-4bc8-8402-777e4538d2a1	t	t	bambaiba-auth-client	0	f	OKMIDc3yEjQtWNNfXnNoOSR5rZqJZZ3r		f	http://localhost:18081	f	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	openid-connect	-1	t	f		f	client-secret	http://localhost:18081		\N	t	f	t	f
88acef04-74b2-48a9-b20a-91e8a91a6071	t	t	bambaiba-api	0	t	\N		f		f	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	openid-connect	-1	t	f		f	client-secret			\N	t	f	t	f
862e68c6-d552-4480-9b9f-4d3a69704afa	t	t	security-admin-console	0	t	\N	/admin/bambaiba/console/	f	\N	f	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	openid-connect	0	f	f	${client_security-admin-console}	f	client-secret	${authAdminUrl}	\N	\N	t	f	f	f
32b64136-f72f-40be-8350-89e652c4d54f	t	f	account	0	t	\N	/realms/bambaiba/account/	f	\N	f	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	openid-connect	0	f	f	${client_account}	f	client-secret	${authBaseUrl}	\N	\N	t	f	f	f
660743a4-25c1-46dd-b9c1-a861be2a2d84	t	f	account-console	0	t	\N	/realms/bambaiba/account/	f	\N	f	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	openid-connect	0	f	f	${client_account-console}	f	client-secret	${authBaseUrl}	\N	\N	t	f	f	f
\.


--
-- TOC entry 4153 (class 0 OID 24948)
-- Dependencies: 236
-- Data for Name: client_attributes; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.client_attributes (client_id, name, value) FROM stdin;
80bd01e1-a0e7-45d4-9873-540a8217051d	post.logout.redirect.uris	+
9175a02f-cf18-40a3-b322-73ed00ce1729	post.logout.redirect.uris	+
9175a02f-cf18-40a3-b322-73ed00ce1729	pkce.code.challenge.method	S256
6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	post.logout.redirect.uris	+
6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	pkce.code.challenge.method	S256
6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	client.use.lightweight.access.token.enabled	true
d8e8e4b8-045c-4709-8563-a48bf3a66b3a	client.use.lightweight.access.token.enabled	true
32b64136-f72f-40be-8350-89e652c4d54f	post.logout.redirect.uris	+
660743a4-25c1-46dd-b9c1-a861be2a2d84	post.logout.redirect.uris	+
660743a4-25c1-46dd-b9c1-a861be2a2d84	pkce.code.challenge.method	S256
862e68c6-d552-4480-9b9f-4d3a69704afa	post.logout.redirect.uris	+
862e68c6-d552-4480-9b9f-4d3a69704afa	pkce.code.challenge.method	S256
862e68c6-d552-4480-9b9f-4d3a69704afa	client.use.lightweight.access.token.enabled	true
32e509de-618c-43e7-a7fc-7dafe19d8bf8	client.use.lightweight.access.token.enabled	true
d8d4e759-1346-4d49-8cb2-8d96ba570f07	client.secret.creation.time	1761221304
d8d4e759-1346-4d49-8cb2-8d96ba570f07	oauth2.device.authorization.grant.enabled	false
d8d4e759-1346-4d49-8cb2-8d96ba570f07	oidc.ciba.grant.enabled	false
d8d4e759-1346-4d49-8cb2-8d96ba570f07	backchannel.logout.session.required	true
d8d4e759-1346-4d49-8cb2-8d96ba570f07	backchannel.logout.revoke.offline.tokens	false
d8d4e759-1346-4d49-8cb2-8d96ba570f07	realm_client	false
d8d4e759-1346-4d49-8cb2-8d96ba570f07	display.on.consent.screen	false
d8d4e759-1346-4d49-8cb2-8d96ba570f07	frontchannel.logout.session.required	true
d8d4e759-1346-4d49-8cb2-8d96ba570f07	use.jwks.url	false
7e903410-f31f-4bc8-8402-777e4538d2a1	client.secret.creation.time	1761221443
7e903410-f31f-4bc8-8402-777e4538d2a1	oauth2.device.authorization.grant.enabled	false
7e903410-f31f-4bc8-8402-777e4538d2a1	oidc.ciba.grant.enabled	false
7e903410-f31f-4bc8-8402-777e4538d2a1	backchannel.logout.session.required	true
7e903410-f31f-4bc8-8402-777e4538d2a1	backchannel.logout.revoke.offline.tokens	false
88acef04-74b2-48a9-b20a-91e8a91a6071	oauth2.device.authorization.grant.enabled	false
88acef04-74b2-48a9-b20a-91e8a91a6071	oidc.ciba.grant.enabled	false
88acef04-74b2-48a9-b20a-91e8a91a6071	backchannel.logout.session.required	true
88acef04-74b2-48a9-b20a-91e8a91a6071	backchannel.logout.revoke.offline.tokens	false
88acef04-74b2-48a9-b20a-91e8a91a6071	realm_client	false
88acef04-74b2-48a9-b20a-91e8a91a6071	display.on.consent.screen	false
88acef04-74b2-48a9-b20a-91e8a91a6071	frontchannel.logout.session.required	true
\.


--
-- TOC entry 4205 (class 0 OID 25917)
-- Dependencies: 288
-- Data for Name: client_auth_flow_bindings; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.client_auth_flow_bindings (client_id, flow_id, binding_name) FROM stdin;
\.


--
-- TOC entry 4204 (class 0 OID 25791)
-- Dependencies: 287
-- Data for Name: client_initial_access; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.client_initial_access (id, realm_id, "timestamp", expiration, count, remaining_count) FROM stdin;
\.


--
-- TOC entry 4154 (class 0 OID 24958)
-- Dependencies: 237
-- Data for Name: client_node_registrations; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.client_node_registrations (client_id, value, name) FROM stdin;
\.


--
-- TOC entry 4182 (class 0 OID 25457)
-- Dependencies: 265
-- Data for Name: client_scope; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.client_scope (id, name, realm_id, description, protocol) FROM stdin;
ed2dd60a-b152-426b-bb35-923d5bcaacd5	offline_access	9ccc2b44-8d11-4694-87e4-8e194b225e1d	OpenID Connect built-in scope: offline_access	openid-connect
3447361c-98b8-4146-a2a8-d103e31a382e	role_list	9ccc2b44-8d11-4694-87e4-8e194b225e1d	SAML role list	saml
2354c9d6-9230-4ef8-9818-d56485529aae	saml_organization	9ccc2b44-8d11-4694-87e4-8e194b225e1d	Organization Membership	saml
f87e9ba6-df51-4a32-a0e4-8bd7fe00b685	profile	9ccc2b44-8d11-4694-87e4-8e194b225e1d	OpenID Connect built-in scope: profile	openid-connect
9477d6cb-3396-463f-807f-e7e05c77502b	email	9ccc2b44-8d11-4694-87e4-8e194b225e1d	OpenID Connect built-in scope: email	openid-connect
43597b3d-8978-4057-9084-9d511fd8e58e	address	9ccc2b44-8d11-4694-87e4-8e194b225e1d	OpenID Connect built-in scope: address	openid-connect
11daa3f1-9d5b-41b7-b68b-1d3d9f9124ed	phone	9ccc2b44-8d11-4694-87e4-8e194b225e1d	OpenID Connect built-in scope: phone	openid-connect
8e148a79-c784-465a-9c32-b340e03f5c0c	roles	9ccc2b44-8d11-4694-87e4-8e194b225e1d	OpenID Connect scope for add user roles to the access token	openid-connect
142e3661-dcf9-43be-a77d-278835a44ef7	web-origins	9ccc2b44-8d11-4694-87e4-8e194b225e1d	OpenID Connect scope for add allowed web origins to the access token	openid-connect
bc717a1c-2345-40e5-9d27-01579b8b2852	microprofile-jwt	9ccc2b44-8d11-4694-87e4-8e194b225e1d	Microprofile - JWT built-in scope	openid-connect
e9143869-1a5f-4f96-a55c-0e2b2d63b30b	acr	9ccc2b44-8d11-4694-87e4-8e194b225e1d	OpenID Connect scope for add acr (authentication context class reference) to the token	openid-connect
0486e99d-cae3-472e-98c6-e1da9bc9563a	basic	9ccc2b44-8d11-4694-87e4-8e194b225e1d	OpenID Connect scope for add all basic claims to the token	openid-connect
a98c98ef-d43c-442f-bf0b-6b035195c6e7	service_account	9ccc2b44-8d11-4694-87e4-8e194b225e1d	Specific scope for a client enabled for service accounts	openid-connect
f646375e-1c13-4dd9-a732-e328bf9f1a4d	organization	9ccc2b44-8d11-4694-87e4-8e194b225e1d	Additional claims about the organization a subject belongs to	openid-connect
a649a5b9-653d-4d5f-bdcf-f1659c51a313	offline_access	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	OpenID Connect built-in scope: offline_access	openid-connect
6f8b807e-fe47-446c-b699-a352acbd54f0	role_list	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	SAML role list	saml
d705b59a-33c0-42c0-8e04-8684529ed9c0	saml_organization	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	Organization Membership	saml
d33cf0c5-5f0c-45ae-8103-1fabd28670df	profile	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	OpenID Connect built-in scope: profile	openid-connect
8a3526d2-4888-4029-ad16-185e7e0cf9c0	email	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	OpenID Connect built-in scope: email	openid-connect
319da478-2b63-4964-b514-a9109e256736	address	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	OpenID Connect built-in scope: address	openid-connect
27b6c674-5fc7-402d-812a-43c50fbf5eb9	phone	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	OpenID Connect built-in scope: phone	openid-connect
0a460cd8-7178-4845-b532-c84568f3be21	roles	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	OpenID Connect scope for add user roles to the access token	openid-connect
38097740-c468-4967-9518-04b39f5289ec	web-origins	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	OpenID Connect scope for add allowed web origins to the access token	openid-connect
2c288200-e6eb-4b9b-851c-035f5b8db264	microprofile-jwt	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	Microprofile - JWT built-in scope	openid-connect
03a0470d-43a0-4262-a02f-c491715bcb56	acr	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	OpenID Connect scope for add acr (authentication context class reference) to the token	openid-connect
524331db-d0ab-4eaf-a3cb-b2e32aeb07bd	basic	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	OpenID Connect scope for add all basic claims to the token	openid-connect
148cf1d2-75c1-4066-988a-f02e2800de5a	service_account	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	Specific scope for a client enabled for service accounts	openid-connect
6bebdcdd-2fd2-4e70-b83c-61099e5fcd51	organization	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	Additional claims about the organization a subject belongs to	openid-connect
0b323a1f-3d9a-4569-97d7-cfb337e57a0c	bambaiba-api-audience	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	For BambaIba Api	openid-connect
\.


--
-- TOC entry 4183 (class 0 OID 25471)
-- Dependencies: 266
-- Data for Name: client_scope_attributes; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.client_scope_attributes (scope_id, value, name) FROM stdin;
ed2dd60a-b152-426b-bb35-923d5bcaacd5	true	display.on.consent.screen
ed2dd60a-b152-426b-bb35-923d5bcaacd5	${offlineAccessScopeConsentText}	consent.screen.text
3447361c-98b8-4146-a2a8-d103e31a382e	true	display.on.consent.screen
3447361c-98b8-4146-a2a8-d103e31a382e	${samlRoleListScopeConsentText}	consent.screen.text
2354c9d6-9230-4ef8-9818-d56485529aae	false	display.on.consent.screen
f87e9ba6-df51-4a32-a0e4-8bd7fe00b685	true	display.on.consent.screen
f87e9ba6-df51-4a32-a0e4-8bd7fe00b685	${profileScopeConsentText}	consent.screen.text
f87e9ba6-df51-4a32-a0e4-8bd7fe00b685	true	include.in.token.scope
9477d6cb-3396-463f-807f-e7e05c77502b	true	display.on.consent.screen
9477d6cb-3396-463f-807f-e7e05c77502b	${emailScopeConsentText}	consent.screen.text
9477d6cb-3396-463f-807f-e7e05c77502b	true	include.in.token.scope
43597b3d-8978-4057-9084-9d511fd8e58e	true	display.on.consent.screen
43597b3d-8978-4057-9084-9d511fd8e58e	${addressScopeConsentText}	consent.screen.text
43597b3d-8978-4057-9084-9d511fd8e58e	true	include.in.token.scope
11daa3f1-9d5b-41b7-b68b-1d3d9f9124ed	true	display.on.consent.screen
11daa3f1-9d5b-41b7-b68b-1d3d9f9124ed	${phoneScopeConsentText}	consent.screen.text
11daa3f1-9d5b-41b7-b68b-1d3d9f9124ed	true	include.in.token.scope
8e148a79-c784-465a-9c32-b340e03f5c0c	true	display.on.consent.screen
8e148a79-c784-465a-9c32-b340e03f5c0c	${rolesScopeConsentText}	consent.screen.text
8e148a79-c784-465a-9c32-b340e03f5c0c	false	include.in.token.scope
142e3661-dcf9-43be-a77d-278835a44ef7	false	display.on.consent.screen
142e3661-dcf9-43be-a77d-278835a44ef7		consent.screen.text
142e3661-dcf9-43be-a77d-278835a44ef7	false	include.in.token.scope
bc717a1c-2345-40e5-9d27-01579b8b2852	false	display.on.consent.screen
bc717a1c-2345-40e5-9d27-01579b8b2852	true	include.in.token.scope
e9143869-1a5f-4f96-a55c-0e2b2d63b30b	false	display.on.consent.screen
e9143869-1a5f-4f96-a55c-0e2b2d63b30b	false	include.in.token.scope
0486e99d-cae3-472e-98c6-e1da9bc9563a	false	display.on.consent.screen
0486e99d-cae3-472e-98c6-e1da9bc9563a	false	include.in.token.scope
a98c98ef-d43c-442f-bf0b-6b035195c6e7	false	display.on.consent.screen
a98c98ef-d43c-442f-bf0b-6b035195c6e7	false	include.in.token.scope
f646375e-1c13-4dd9-a732-e328bf9f1a4d	true	display.on.consent.screen
f646375e-1c13-4dd9-a732-e328bf9f1a4d	${organizationScopeConsentText}	consent.screen.text
f646375e-1c13-4dd9-a732-e328bf9f1a4d	true	include.in.token.scope
a649a5b9-653d-4d5f-bdcf-f1659c51a313	true	display.on.consent.screen
a649a5b9-653d-4d5f-bdcf-f1659c51a313	${offlineAccessScopeConsentText}	consent.screen.text
6f8b807e-fe47-446c-b699-a352acbd54f0	true	display.on.consent.screen
6f8b807e-fe47-446c-b699-a352acbd54f0	${samlRoleListScopeConsentText}	consent.screen.text
d705b59a-33c0-42c0-8e04-8684529ed9c0	false	display.on.consent.screen
d33cf0c5-5f0c-45ae-8103-1fabd28670df	true	display.on.consent.screen
d33cf0c5-5f0c-45ae-8103-1fabd28670df	${profileScopeConsentText}	consent.screen.text
d33cf0c5-5f0c-45ae-8103-1fabd28670df	true	include.in.token.scope
8a3526d2-4888-4029-ad16-185e7e0cf9c0	true	display.on.consent.screen
8a3526d2-4888-4029-ad16-185e7e0cf9c0	${emailScopeConsentText}	consent.screen.text
8a3526d2-4888-4029-ad16-185e7e0cf9c0	true	include.in.token.scope
319da478-2b63-4964-b514-a9109e256736	true	display.on.consent.screen
319da478-2b63-4964-b514-a9109e256736	${addressScopeConsentText}	consent.screen.text
319da478-2b63-4964-b514-a9109e256736	true	include.in.token.scope
27b6c674-5fc7-402d-812a-43c50fbf5eb9	true	display.on.consent.screen
27b6c674-5fc7-402d-812a-43c50fbf5eb9	${phoneScopeConsentText}	consent.screen.text
27b6c674-5fc7-402d-812a-43c50fbf5eb9	true	include.in.token.scope
0a460cd8-7178-4845-b532-c84568f3be21	true	display.on.consent.screen
0a460cd8-7178-4845-b532-c84568f3be21	${rolesScopeConsentText}	consent.screen.text
0a460cd8-7178-4845-b532-c84568f3be21	false	include.in.token.scope
38097740-c468-4967-9518-04b39f5289ec	false	display.on.consent.screen
38097740-c468-4967-9518-04b39f5289ec		consent.screen.text
38097740-c468-4967-9518-04b39f5289ec	false	include.in.token.scope
2c288200-e6eb-4b9b-851c-035f5b8db264	false	display.on.consent.screen
2c288200-e6eb-4b9b-851c-035f5b8db264	true	include.in.token.scope
03a0470d-43a0-4262-a02f-c491715bcb56	false	display.on.consent.screen
03a0470d-43a0-4262-a02f-c491715bcb56	false	include.in.token.scope
524331db-d0ab-4eaf-a3cb-b2e32aeb07bd	false	display.on.consent.screen
524331db-d0ab-4eaf-a3cb-b2e32aeb07bd	false	include.in.token.scope
148cf1d2-75c1-4066-988a-f02e2800de5a	false	display.on.consent.screen
148cf1d2-75c1-4066-988a-f02e2800de5a	false	include.in.token.scope
6bebdcdd-2fd2-4e70-b83c-61099e5fcd51	true	display.on.consent.screen
6bebdcdd-2fd2-4e70-b83c-61099e5fcd51	${organizationScopeConsentText}	consent.screen.text
6bebdcdd-2fd2-4e70-b83c-61099e5fcd51	true	include.in.token.scope
0b323a1f-3d9a-4569-97d7-cfb337e57a0c	true	display.on.consent.screen
0b323a1f-3d9a-4569-97d7-cfb337e57a0c		consent.screen.text
0b323a1f-3d9a-4569-97d7-cfb337e57a0c	true	include.in.token.scope
0b323a1f-3d9a-4569-97d7-cfb337e57a0c		gui.order
\.


--
-- TOC entry 4206 (class 0 OID 25958)
-- Dependencies: 289
-- Data for Name: client_scope_client; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.client_scope_client (client_id, scope_id, default_scope) FROM stdin;
80bd01e1-a0e7-45d4-9873-540a8217051d	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685	t
80bd01e1-a0e7-45d4-9873-540a8217051d	0486e99d-cae3-472e-98c6-e1da9bc9563a	t
80bd01e1-a0e7-45d4-9873-540a8217051d	142e3661-dcf9-43be-a77d-278835a44ef7	t
80bd01e1-a0e7-45d4-9873-540a8217051d	9477d6cb-3396-463f-807f-e7e05c77502b	t
80bd01e1-a0e7-45d4-9873-540a8217051d	8e148a79-c784-465a-9c32-b340e03f5c0c	t
80bd01e1-a0e7-45d4-9873-540a8217051d	e9143869-1a5f-4f96-a55c-0e2b2d63b30b	t
80bd01e1-a0e7-45d4-9873-540a8217051d	43597b3d-8978-4057-9084-9d511fd8e58e	f
80bd01e1-a0e7-45d4-9873-540a8217051d	bc717a1c-2345-40e5-9d27-01579b8b2852	f
80bd01e1-a0e7-45d4-9873-540a8217051d	ed2dd60a-b152-426b-bb35-923d5bcaacd5	f
80bd01e1-a0e7-45d4-9873-540a8217051d	f646375e-1c13-4dd9-a732-e328bf9f1a4d	f
80bd01e1-a0e7-45d4-9873-540a8217051d	11daa3f1-9d5b-41b7-b68b-1d3d9f9124ed	f
9175a02f-cf18-40a3-b322-73ed00ce1729	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685	t
9175a02f-cf18-40a3-b322-73ed00ce1729	0486e99d-cae3-472e-98c6-e1da9bc9563a	t
9175a02f-cf18-40a3-b322-73ed00ce1729	142e3661-dcf9-43be-a77d-278835a44ef7	t
9175a02f-cf18-40a3-b322-73ed00ce1729	9477d6cb-3396-463f-807f-e7e05c77502b	t
9175a02f-cf18-40a3-b322-73ed00ce1729	8e148a79-c784-465a-9c32-b340e03f5c0c	t
9175a02f-cf18-40a3-b322-73ed00ce1729	e9143869-1a5f-4f96-a55c-0e2b2d63b30b	t
9175a02f-cf18-40a3-b322-73ed00ce1729	43597b3d-8978-4057-9084-9d511fd8e58e	f
9175a02f-cf18-40a3-b322-73ed00ce1729	bc717a1c-2345-40e5-9d27-01579b8b2852	f
9175a02f-cf18-40a3-b322-73ed00ce1729	ed2dd60a-b152-426b-bb35-923d5bcaacd5	f
9175a02f-cf18-40a3-b322-73ed00ce1729	f646375e-1c13-4dd9-a732-e328bf9f1a4d	f
9175a02f-cf18-40a3-b322-73ed00ce1729	11daa3f1-9d5b-41b7-b68b-1d3d9f9124ed	f
d8e8e4b8-045c-4709-8563-a48bf3a66b3a	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685	t
d8e8e4b8-045c-4709-8563-a48bf3a66b3a	0486e99d-cae3-472e-98c6-e1da9bc9563a	t
d8e8e4b8-045c-4709-8563-a48bf3a66b3a	142e3661-dcf9-43be-a77d-278835a44ef7	t
d8e8e4b8-045c-4709-8563-a48bf3a66b3a	9477d6cb-3396-463f-807f-e7e05c77502b	t
d8e8e4b8-045c-4709-8563-a48bf3a66b3a	8e148a79-c784-465a-9c32-b340e03f5c0c	t
d8e8e4b8-045c-4709-8563-a48bf3a66b3a	e9143869-1a5f-4f96-a55c-0e2b2d63b30b	t
d8e8e4b8-045c-4709-8563-a48bf3a66b3a	43597b3d-8978-4057-9084-9d511fd8e58e	f
d8e8e4b8-045c-4709-8563-a48bf3a66b3a	bc717a1c-2345-40e5-9d27-01579b8b2852	f
d8e8e4b8-045c-4709-8563-a48bf3a66b3a	ed2dd60a-b152-426b-bb35-923d5bcaacd5	f
d8e8e4b8-045c-4709-8563-a48bf3a66b3a	f646375e-1c13-4dd9-a732-e328bf9f1a4d	f
d8e8e4b8-045c-4709-8563-a48bf3a66b3a	11daa3f1-9d5b-41b7-b68b-1d3d9f9124ed	f
f859cd3b-5020-4869-8d00-d688f2b7b3bb	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685	t
f859cd3b-5020-4869-8d00-d688f2b7b3bb	0486e99d-cae3-472e-98c6-e1da9bc9563a	t
f859cd3b-5020-4869-8d00-d688f2b7b3bb	142e3661-dcf9-43be-a77d-278835a44ef7	t
f859cd3b-5020-4869-8d00-d688f2b7b3bb	9477d6cb-3396-463f-807f-e7e05c77502b	t
f859cd3b-5020-4869-8d00-d688f2b7b3bb	8e148a79-c784-465a-9c32-b340e03f5c0c	t
f859cd3b-5020-4869-8d00-d688f2b7b3bb	e9143869-1a5f-4f96-a55c-0e2b2d63b30b	t
f859cd3b-5020-4869-8d00-d688f2b7b3bb	43597b3d-8978-4057-9084-9d511fd8e58e	f
f859cd3b-5020-4869-8d00-d688f2b7b3bb	bc717a1c-2345-40e5-9d27-01579b8b2852	f
f859cd3b-5020-4869-8d00-d688f2b7b3bb	ed2dd60a-b152-426b-bb35-923d5bcaacd5	f
f859cd3b-5020-4869-8d00-d688f2b7b3bb	f646375e-1c13-4dd9-a732-e328bf9f1a4d	f
f859cd3b-5020-4869-8d00-d688f2b7b3bb	11daa3f1-9d5b-41b7-b68b-1d3d9f9124ed	f
9f3e1992-eb18-490a-a7af-2ddc4d947be5	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685	t
9f3e1992-eb18-490a-a7af-2ddc4d947be5	0486e99d-cae3-472e-98c6-e1da9bc9563a	t
9f3e1992-eb18-490a-a7af-2ddc4d947be5	142e3661-dcf9-43be-a77d-278835a44ef7	t
9f3e1992-eb18-490a-a7af-2ddc4d947be5	9477d6cb-3396-463f-807f-e7e05c77502b	t
9f3e1992-eb18-490a-a7af-2ddc4d947be5	8e148a79-c784-465a-9c32-b340e03f5c0c	t
9f3e1992-eb18-490a-a7af-2ddc4d947be5	e9143869-1a5f-4f96-a55c-0e2b2d63b30b	t
9f3e1992-eb18-490a-a7af-2ddc4d947be5	43597b3d-8978-4057-9084-9d511fd8e58e	f
9f3e1992-eb18-490a-a7af-2ddc4d947be5	bc717a1c-2345-40e5-9d27-01579b8b2852	f
9f3e1992-eb18-490a-a7af-2ddc4d947be5	ed2dd60a-b152-426b-bb35-923d5bcaacd5	f
9f3e1992-eb18-490a-a7af-2ddc4d947be5	f646375e-1c13-4dd9-a732-e328bf9f1a4d	f
9f3e1992-eb18-490a-a7af-2ddc4d947be5	11daa3f1-9d5b-41b7-b68b-1d3d9f9124ed	f
6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685	t
6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	0486e99d-cae3-472e-98c6-e1da9bc9563a	t
6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	142e3661-dcf9-43be-a77d-278835a44ef7	t
6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	9477d6cb-3396-463f-807f-e7e05c77502b	t
6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	8e148a79-c784-465a-9c32-b340e03f5c0c	t
6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	e9143869-1a5f-4f96-a55c-0e2b2d63b30b	t
6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	43597b3d-8978-4057-9084-9d511fd8e58e	f
6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	bc717a1c-2345-40e5-9d27-01579b8b2852	f
6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	ed2dd60a-b152-426b-bb35-923d5bcaacd5	f
6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	f646375e-1c13-4dd9-a732-e328bf9f1a4d	f
6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	11daa3f1-9d5b-41b7-b68b-1d3d9f9124ed	f
32b64136-f72f-40be-8350-89e652c4d54f	0a460cd8-7178-4845-b532-c84568f3be21	t
32b64136-f72f-40be-8350-89e652c4d54f	d33cf0c5-5f0c-45ae-8103-1fabd28670df	t
32b64136-f72f-40be-8350-89e652c4d54f	8a3526d2-4888-4029-ad16-185e7e0cf9c0	t
32b64136-f72f-40be-8350-89e652c4d54f	524331db-d0ab-4eaf-a3cb-b2e32aeb07bd	t
32b64136-f72f-40be-8350-89e652c4d54f	38097740-c468-4967-9518-04b39f5289ec	t
32b64136-f72f-40be-8350-89e652c4d54f	03a0470d-43a0-4262-a02f-c491715bcb56	t
32b64136-f72f-40be-8350-89e652c4d54f	319da478-2b63-4964-b514-a9109e256736	f
32b64136-f72f-40be-8350-89e652c4d54f	6bebdcdd-2fd2-4e70-b83c-61099e5fcd51	f
32b64136-f72f-40be-8350-89e652c4d54f	27b6c674-5fc7-402d-812a-43c50fbf5eb9	f
32b64136-f72f-40be-8350-89e652c4d54f	2c288200-e6eb-4b9b-851c-035f5b8db264	f
32b64136-f72f-40be-8350-89e652c4d54f	a649a5b9-653d-4d5f-bdcf-f1659c51a313	f
660743a4-25c1-46dd-b9c1-a861be2a2d84	0a460cd8-7178-4845-b532-c84568f3be21	t
660743a4-25c1-46dd-b9c1-a861be2a2d84	d33cf0c5-5f0c-45ae-8103-1fabd28670df	t
660743a4-25c1-46dd-b9c1-a861be2a2d84	8a3526d2-4888-4029-ad16-185e7e0cf9c0	t
660743a4-25c1-46dd-b9c1-a861be2a2d84	524331db-d0ab-4eaf-a3cb-b2e32aeb07bd	t
660743a4-25c1-46dd-b9c1-a861be2a2d84	38097740-c468-4967-9518-04b39f5289ec	t
660743a4-25c1-46dd-b9c1-a861be2a2d84	03a0470d-43a0-4262-a02f-c491715bcb56	t
660743a4-25c1-46dd-b9c1-a861be2a2d84	319da478-2b63-4964-b514-a9109e256736	f
660743a4-25c1-46dd-b9c1-a861be2a2d84	6bebdcdd-2fd2-4e70-b83c-61099e5fcd51	f
660743a4-25c1-46dd-b9c1-a861be2a2d84	27b6c674-5fc7-402d-812a-43c50fbf5eb9	f
660743a4-25c1-46dd-b9c1-a861be2a2d84	2c288200-e6eb-4b9b-851c-035f5b8db264	f
660743a4-25c1-46dd-b9c1-a861be2a2d84	a649a5b9-653d-4d5f-bdcf-f1659c51a313	f
32e509de-618c-43e7-a7fc-7dafe19d8bf8	0a460cd8-7178-4845-b532-c84568f3be21	t
32e509de-618c-43e7-a7fc-7dafe19d8bf8	d33cf0c5-5f0c-45ae-8103-1fabd28670df	t
32e509de-618c-43e7-a7fc-7dafe19d8bf8	8a3526d2-4888-4029-ad16-185e7e0cf9c0	t
32e509de-618c-43e7-a7fc-7dafe19d8bf8	524331db-d0ab-4eaf-a3cb-b2e32aeb07bd	t
32e509de-618c-43e7-a7fc-7dafe19d8bf8	38097740-c468-4967-9518-04b39f5289ec	t
32e509de-618c-43e7-a7fc-7dafe19d8bf8	03a0470d-43a0-4262-a02f-c491715bcb56	t
32e509de-618c-43e7-a7fc-7dafe19d8bf8	319da478-2b63-4964-b514-a9109e256736	f
32e509de-618c-43e7-a7fc-7dafe19d8bf8	6bebdcdd-2fd2-4e70-b83c-61099e5fcd51	f
32e509de-618c-43e7-a7fc-7dafe19d8bf8	27b6c674-5fc7-402d-812a-43c50fbf5eb9	f
32e509de-618c-43e7-a7fc-7dafe19d8bf8	2c288200-e6eb-4b9b-851c-035f5b8db264	f
32e509de-618c-43e7-a7fc-7dafe19d8bf8	a649a5b9-653d-4d5f-bdcf-f1659c51a313	f
157360ea-5728-4d05-877e-9bf20421e380	0a460cd8-7178-4845-b532-c84568f3be21	t
157360ea-5728-4d05-877e-9bf20421e380	d33cf0c5-5f0c-45ae-8103-1fabd28670df	t
157360ea-5728-4d05-877e-9bf20421e380	8a3526d2-4888-4029-ad16-185e7e0cf9c0	t
157360ea-5728-4d05-877e-9bf20421e380	524331db-d0ab-4eaf-a3cb-b2e32aeb07bd	t
157360ea-5728-4d05-877e-9bf20421e380	38097740-c468-4967-9518-04b39f5289ec	t
157360ea-5728-4d05-877e-9bf20421e380	03a0470d-43a0-4262-a02f-c491715bcb56	t
157360ea-5728-4d05-877e-9bf20421e380	319da478-2b63-4964-b514-a9109e256736	f
157360ea-5728-4d05-877e-9bf20421e380	6bebdcdd-2fd2-4e70-b83c-61099e5fcd51	f
157360ea-5728-4d05-877e-9bf20421e380	27b6c674-5fc7-402d-812a-43c50fbf5eb9	f
157360ea-5728-4d05-877e-9bf20421e380	2c288200-e6eb-4b9b-851c-035f5b8db264	f
157360ea-5728-4d05-877e-9bf20421e380	a649a5b9-653d-4d5f-bdcf-f1659c51a313	f
f62fa0c1-357e-4d58-af34-9d108b6b24c0	0a460cd8-7178-4845-b532-c84568f3be21	t
f62fa0c1-357e-4d58-af34-9d108b6b24c0	d33cf0c5-5f0c-45ae-8103-1fabd28670df	t
f62fa0c1-357e-4d58-af34-9d108b6b24c0	8a3526d2-4888-4029-ad16-185e7e0cf9c0	t
f62fa0c1-357e-4d58-af34-9d108b6b24c0	524331db-d0ab-4eaf-a3cb-b2e32aeb07bd	t
f62fa0c1-357e-4d58-af34-9d108b6b24c0	38097740-c468-4967-9518-04b39f5289ec	t
f62fa0c1-357e-4d58-af34-9d108b6b24c0	03a0470d-43a0-4262-a02f-c491715bcb56	t
f62fa0c1-357e-4d58-af34-9d108b6b24c0	319da478-2b63-4964-b514-a9109e256736	f
f62fa0c1-357e-4d58-af34-9d108b6b24c0	6bebdcdd-2fd2-4e70-b83c-61099e5fcd51	f
f62fa0c1-357e-4d58-af34-9d108b6b24c0	27b6c674-5fc7-402d-812a-43c50fbf5eb9	f
f62fa0c1-357e-4d58-af34-9d108b6b24c0	2c288200-e6eb-4b9b-851c-035f5b8db264	f
f62fa0c1-357e-4d58-af34-9d108b6b24c0	a649a5b9-653d-4d5f-bdcf-f1659c51a313	f
862e68c6-d552-4480-9b9f-4d3a69704afa	0a460cd8-7178-4845-b532-c84568f3be21	t
862e68c6-d552-4480-9b9f-4d3a69704afa	d33cf0c5-5f0c-45ae-8103-1fabd28670df	t
862e68c6-d552-4480-9b9f-4d3a69704afa	8a3526d2-4888-4029-ad16-185e7e0cf9c0	t
862e68c6-d552-4480-9b9f-4d3a69704afa	524331db-d0ab-4eaf-a3cb-b2e32aeb07bd	t
862e68c6-d552-4480-9b9f-4d3a69704afa	38097740-c468-4967-9518-04b39f5289ec	t
862e68c6-d552-4480-9b9f-4d3a69704afa	03a0470d-43a0-4262-a02f-c491715bcb56	t
862e68c6-d552-4480-9b9f-4d3a69704afa	319da478-2b63-4964-b514-a9109e256736	f
862e68c6-d552-4480-9b9f-4d3a69704afa	6bebdcdd-2fd2-4e70-b83c-61099e5fcd51	f
862e68c6-d552-4480-9b9f-4d3a69704afa	27b6c674-5fc7-402d-812a-43c50fbf5eb9	f
862e68c6-d552-4480-9b9f-4d3a69704afa	2c288200-e6eb-4b9b-851c-035f5b8db264	f
862e68c6-d552-4480-9b9f-4d3a69704afa	a649a5b9-653d-4d5f-bdcf-f1659c51a313	f
d8d4e759-1346-4d49-8cb2-8d96ba570f07	0a460cd8-7178-4845-b532-c84568f3be21	t
d8d4e759-1346-4d49-8cb2-8d96ba570f07	d33cf0c5-5f0c-45ae-8103-1fabd28670df	t
d8d4e759-1346-4d49-8cb2-8d96ba570f07	8a3526d2-4888-4029-ad16-185e7e0cf9c0	t
d8d4e759-1346-4d49-8cb2-8d96ba570f07	524331db-d0ab-4eaf-a3cb-b2e32aeb07bd	t
d8d4e759-1346-4d49-8cb2-8d96ba570f07	38097740-c468-4967-9518-04b39f5289ec	t
d8d4e759-1346-4d49-8cb2-8d96ba570f07	03a0470d-43a0-4262-a02f-c491715bcb56	t
d8d4e759-1346-4d49-8cb2-8d96ba570f07	319da478-2b63-4964-b514-a9109e256736	f
d8d4e759-1346-4d49-8cb2-8d96ba570f07	6bebdcdd-2fd2-4e70-b83c-61099e5fcd51	f
d8d4e759-1346-4d49-8cb2-8d96ba570f07	27b6c674-5fc7-402d-812a-43c50fbf5eb9	f
d8d4e759-1346-4d49-8cb2-8d96ba570f07	2c288200-e6eb-4b9b-851c-035f5b8db264	f
d8d4e759-1346-4d49-8cb2-8d96ba570f07	a649a5b9-653d-4d5f-bdcf-f1659c51a313	f
d8d4e759-1346-4d49-8cb2-8d96ba570f07	148cf1d2-75c1-4066-988a-f02e2800de5a	t
7e903410-f31f-4bc8-8402-777e4538d2a1	0a460cd8-7178-4845-b532-c84568f3be21	t
7e903410-f31f-4bc8-8402-777e4538d2a1	d33cf0c5-5f0c-45ae-8103-1fabd28670df	t
7e903410-f31f-4bc8-8402-777e4538d2a1	8a3526d2-4888-4029-ad16-185e7e0cf9c0	t
7e903410-f31f-4bc8-8402-777e4538d2a1	524331db-d0ab-4eaf-a3cb-b2e32aeb07bd	t
7e903410-f31f-4bc8-8402-777e4538d2a1	38097740-c468-4967-9518-04b39f5289ec	t
7e903410-f31f-4bc8-8402-777e4538d2a1	03a0470d-43a0-4262-a02f-c491715bcb56	t
7e903410-f31f-4bc8-8402-777e4538d2a1	319da478-2b63-4964-b514-a9109e256736	f
7e903410-f31f-4bc8-8402-777e4538d2a1	6bebdcdd-2fd2-4e70-b83c-61099e5fcd51	f
7e903410-f31f-4bc8-8402-777e4538d2a1	27b6c674-5fc7-402d-812a-43c50fbf5eb9	f
7e903410-f31f-4bc8-8402-777e4538d2a1	2c288200-e6eb-4b9b-851c-035f5b8db264	f
7e903410-f31f-4bc8-8402-777e4538d2a1	a649a5b9-653d-4d5f-bdcf-f1659c51a313	f
7e903410-f31f-4bc8-8402-777e4538d2a1	0b323a1f-3d9a-4569-97d7-cfb337e57a0c	t
d8d4e759-1346-4d49-8cb2-8d96ba570f07	0b323a1f-3d9a-4569-97d7-cfb337e57a0c	t
88acef04-74b2-48a9-b20a-91e8a91a6071	0b323a1f-3d9a-4569-97d7-cfb337e57a0c	t
88acef04-74b2-48a9-b20a-91e8a91a6071	0a460cd8-7178-4845-b532-c84568f3be21	t
88acef04-74b2-48a9-b20a-91e8a91a6071	d33cf0c5-5f0c-45ae-8103-1fabd28670df	t
88acef04-74b2-48a9-b20a-91e8a91a6071	8a3526d2-4888-4029-ad16-185e7e0cf9c0	t
88acef04-74b2-48a9-b20a-91e8a91a6071	524331db-d0ab-4eaf-a3cb-b2e32aeb07bd	t
88acef04-74b2-48a9-b20a-91e8a91a6071	38097740-c468-4967-9518-04b39f5289ec	t
88acef04-74b2-48a9-b20a-91e8a91a6071	03a0470d-43a0-4262-a02f-c491715bcb56	t
88acef04-74b2-48a9-b20a-91e8a91a6071	319da478-2b63-4964-b514-a9109e256736	f
88acef04-74b2-48a9-b20a-91e8a91a6071	6bebdcdd-2fd2-4e70-b83c-61099e5fcd51	f
88acef04-74b2-48a9-b20a-91e8a91a6071	27b6c674-5fc7-402d-812a-43c50fbf5eb9	f
88acef04-74b2-48a9-b20a-91e8a91a6071	2c288200-e6eb-4b9b-851c-035f5b8db264	f
88acef04-74b2-48a9-b20a-91e8a91a6071	a649a5b9-653d-4d5f-bdcf-f1659c51a313	f
\.


--
-- TOC entry 4184 (class 0 OID 25476)
-- Dependencies: 267
-- Data for Name: client_scope_role_mapping; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.client_scope_role_mapping (scope_id, role_id) FROM stdin;
ed2dd60a-b152-426b-bb35-923d5bcaacd5	80bbb03d-dddd-4339-84b1-690dbf029de8
a649a5b9-653d-4d5f-bdcf-f1659c51a313	163f9ffa-1a43-48cd-8ad3-a918279550ae
\.


--
-- TOC entry 4202 (class 0 OID 25712)
-- Dependencies: 285
-- Data for Name: component; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.component (id, name, parent_id, provider_id, provider_type, realm_id, sub_type) FROM stdin;
aa49017c-2d6a-4431-a0bc-34bd9fbe2726	Trusted Hosts	9ccc2b44-8d11-4694-87e4-8e194b225e1d	trusted-hosts	org.keycloak.services.clientregistration.policy.ClientRegistrationPolicy	9ccc2b44-8d11-4694-87e4-8e194b225e1d	anonymous
7d11c4d6-ecb6-4a6c-9b7c-da9eb9ae8e8c	Consent Required	9ccc2b44-8d11-4694-87e4-8e194b225e1d	consent-required	org.keycloak.services.clientregistration.policy.ClientRegistrationPolicy	9ccc2b44-8d11-4694-87e4-8e194b225e1d	anonymous
18916dc5-28c4-4ae0-9d83-97eab8aaca59	Full Scope Disabled	9ccc2b44-8d11-4694-87e4-8e194b225e1d	scope	org.keycloak.services.clientregistration.policy.ClientRegistrationPolicy	9ccc2b44-8d11-4694-87e4-8e194b225e1d	anonymous
0ac79133-4b08-4ed0-b7e6-e4cbc12b441f	Max Clients Limit	9ccc2b44-8d11-4694-87e4-8e194b225e1d	max-clients	org.keycloak.services.clientregistration.policy.ClientRegistrationPolicy	9ccc2b44-8d11-4694-87e4-8e194b225e1d	anonymous
9487b0b5-ebe2-4d7f-a483-804a2196f488	Allowed Protocol Mapper Types	9ccc2b44-8d11-4694-87e4-8e194b225e1d	allowed-protocol-mappers	org.keycloak.services.clientregistration.policy.ClientRegistrationPolicy	9ccc2b44-8d11-4694-87e4-8e194b225e1d	anonymous
e73953ec-41e4-45a7-87d2-3c2422412574	Allowed Client Scopes	9ccc2b44-8d11-4694-87e4-8e194b225e1d	allowed-client-templates	org.keycloak.services.clientregistration.policy.ClientRegistrationPolicy	9ccc2b44-8d11-4694-87e4-8e194b225e1d	anonymous
406f9e8e-ea3a-4286-827e-e230e761cb80	Allowed Protocol Mapper Types	9ccc2b44-8d11-4694-87e4-8e194b225e1d	allowed-protocol-mappers	org.keycloak.services.clientregistration.policy.ClientRegistrationPolicy	9ccc2b44-8d11-4694-87e4-8e194b225e1d	authenticated
1d51ce8b-b319-4f59-b6e6-904795adca15	Allowed Client Scopes	9ccc2b44-8d11-4694-87e4-8e194b225e1d	allowed-client-templates	org.keycloak.services.clientregistration.policy.ClientRegistrationPolicy	9ccc2b44-8d11-4694-87e4-8e194b225e1d	authenticated
807851b6-c074-4d4d-a001-e5befa43b073	rsa-generated	9ccc2b44-8d11-4694-87e4-8e194b225e1d	rsa-generated	org.keycloak.keys.KeyProvider	9ccc2b44-8d11-4694-87e4-8e194b225e1d	\N
e4f15a41-0141-409a-b2d8-0602d8b160c2	rsa-enc-generated	9ccc2b44-8d11-4694-87e4-8e194b225e1d	rsa-enc-generated	org.keycloak.keys.KeyProvider	9ccc2b44-8d11-4694-87e4-8e194b225e1d	\N
c5a48332-40e8-42a4-998e-f5e2413786f9	hmac-generated-hs512	9ccc2b44-8d11-4694-87e4-8e194b225e1d	hmac-generated	org.keycloak.keys.KeyProvider	9ccc2b44-8d11-4694-87e4-8e194b225e1d	\N
c87cff2f-2060-4dfa-bd88-aaeb905bd6fc	aes-generated	9ccc2b44-8d11-4694-87e4-8e194b225e1d	aes-generated	org.keycloak.keys.KeyProvider	9ccc2b44-8d11-4694-87e4-8e194b225e1d	\N
c5b58635-88f8-432c-8a45-8f4cabd14904	\N	9ccc2b44-8d11-4694-87e4-8e194b225e1d	declarative-user-profile	org.keycloak.userprofile.UserProfileProvider	9ccc2b44-8d11-4694-87e4-8e194b225e1d	\N
56bc893c-0765-4c02-a81b-434000298b99	rsa-generated	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	rsa-generated	org.keycloak.keys.KeyProvider	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	\N
f68f8bbc-1359-42e0-b295-9d8611fdf6e9	rsa-enc-generated	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	rsa-enc-generated	org.keycloak.keys.KeyProvider	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	\N
cbb9f1a0-1f20-48b8-a62f-75d80a4cbb3f	hmac-generated-hs512	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	hmac-generated	org.keycloak.keys.KeyProvider	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	\N
642e25b1-3fb7-43c6-b9b6-a2532a2c5b02	aes-generated	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	aes-generated	org.keycloak.keys.KeyProvider	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	\N
036e5885-7f9e-4e1d-8ff6-c9e4ac35fc28	Trusted Hosts	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	trusted-hosts	org.keycloak.services.clientregistration.policy.ClientRegistrationPolicy	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	anonymous
15dceebb-f1f8-4c37-90b9-d2c9d8bed6cc	Consent Required	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	consent-required	org.keycloak.services.clientregistration.policy.ClientRegistrationPolicy	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	anonymous
eb8a99b2-e8f6-44e5-98e9-782ad61831e4	Full Scope Disabled	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	scope	org.keycloak.services.clientregistration.policy.ClientRegistrationPolicy	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	anonymous
bff1a4c2-921b-4bce-bf34-a52e93e9bdbe	Max Clients Limit	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	max-clients	org.keycloak.services.clientregistration.policy.ClientRegistrationPolicy	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	anonymous
8670777d-3f59-410b-ae49-497560d95e50	Allowed Protocol Mapper Types	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	allowed-protocol-mappers	org.keycloak.services.clientregistration.policy.ClientRegistrationPolicy	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	anonymous
4104e90f-6caf-478b-ab9a-c5825cb5dee3	Allowed Client Scopes	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	allowed-client-templates	org.keycloak.services.clientregistration.policy.ClientRegistrationPolicy	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	anonymous
25b2db25-222d-4e1d-a23d-7e2c8d011da7	Allowed Protocol Mapper Types	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	allowed-protocol-mappers	org.keycloak.services.clientregistration.policy.ClientRegistrationPolicy	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	authenticated
92be0202-5138-4cfe-ac42-bd543617885f	Allowed Client Scopes	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	allowed-client-templates	org.keycloak.services.clientregistration.policy.ClientRegistrationPolicy	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	authenticated
\.


--
-- TOC entry 4201 (class 0 OID 25707)
-- Dependencies: 284
-- Data for Name: component_config; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.component_config (id, component_id, name, value) FROM stdin;
bddb31f1-5ab5-41d9-bdce-d893b525b3a6	1d51ce8b-b319-4f59-b6e6-904795adca15	allow-default-scopes	true
4b0b332e-25f6-459d-aefb-612ae9f2d146	9487b0b5-ebe2-4d7f-a483-804a2196f488	allowed-protocol-mapper-types	saml-user-property-mapper
086f0da9-6f57-47f9-9d88-49e063f05fd1	9487b0b5-ebe2-4d7f-a483-804a2196f488	allowed-protocol-mapper-types	oidc-full-name-mapper
2b4f139a-6300-478e-a851-7995aee6090b	9487b0b5-ebe2-4d7f-a483-804a2196f488	allowed-protocol-mapper-types	oidc-address-mapper
f21c5f47-23eb-4dba-8845-6ff53447eb37	9487b0b5-ebe2-4d7f-a483-804a2196f488	allowed-protocol-mapper-types	oidc-sha256-pairwise-sub-mapper
cdbcf2c4-6f2d-4f35-a308-12a7e2157c10	9487b0b5-ebe2-4d7f-a483-804a2196f488	allowed-protocol-mapper-types	saml-role-list-mapper
f6d6c81f-74ff-46e9-9594-423a1adb26c9	9487b0b5-ebe2-4d7f-a483-804a2196f488	allowed-protocol-mapper-types	oidc-usermodel-property-mapper
13fe7ed9-695d-4436-ba13-442567ded8f7	9487b0b5-ebe2-4d7f-a483-804a2196f488	allowed-protocol-mapper-types	saml-user-attribute-mapper
115e1169-907b-47f3-9162-02b26dfb7e82	9487b0b5-ebe2-4d7f-a483-804a2196f488	allowed-protocol-mapper-types	oidc-usermodel-attribute-mapper
36cdfad0-7c94-4139-9901-afec1070b307	406f9e8e-ea3a-4286-827e-e230e761cb80	allowed-protocol-mapper-types	oidc-full-name-mapper
5d146eaf-a67f-4f6d-95ee-19bb83bc8c49	406f9e8e-ea3a-4286-827e-e230e761cb80	allowed-protocol-mapper-types	oidc-usermodel-attribute-mapper
dc5cba0e-5c98-48ef-a5f1-a1b49a1402f8	406f9e8e-ea3a-4286-827e-e230e761cb80	allowed-protocol-mapper-types	saml-user-property-mapper
34594ab0-9480-4e38-a7bb-3baa79a1925a	406f9e8e-ea3a-4286-827e-e230e761cb80	allowed-protocol-mapper-types	oidc-sha256-pairwise-sub-mapper
45c3a342-32c3-4f85-b775-5ec3a99a637d	406f9e8e-ea3a-4286-827e-e230e761cb80	allowed-protocol-mapper-types	oidc-usermodel-property-mapper
541159b8-4ad0-4747-b05a-847a82ceab3b	406f9e8e-ea3a-4286-827e-e230e761cb80	allowed-protocol-mapper-types	oidc-address-mapper
159f1c0d-1e58-49e8-8048-78300c4523b9	406f9e8e-ea3a-4286-827e-e230e761cb80	allowed-protocol-mapper-types	saml-role-list-mapper
898288fa-6fd6-4032-91c6-02f47df926b1	406f9e8e-ea3a-4286-827e-e230e761cb80	allowed-protocol-mapper-types	saml-user-attribute-mapper
0fd254e8-18ac-4fa0-bbe0-90f16a5f8d64	0ac79133-4b08-4ed0-b7e6-e4cbc12b441f	max-clients	200
d68c2d41-0712-4fd3-b324-844e4b317b8a	e73953ec-41e4-45a7-87d2-3c2422412574	allow-default-scopes	true
94ca84ca-553e-4a07-a57d-edcc66771dd5	aa49017c-2d6a-4431-a0bc-34bd9fbe2726	host-sending-registration-request-must-match	true
7126b5e7-06c6-4ebb-8295-4ab3e8cbc873	aa49017c-2d6a-4431-a0bc-34bd9fbe2726	client-uris-must-match	true
e092a082-5980-4a58-a673-0eb6a5726a56	e4f15a41-0141-409a-b2d8-0602d8b160c2	keyUse	ENC
ce6f476c-a899-46bb-ba16-ec1f2a0dfd87	e4f15a41-0141-409a-b2d8-0602d8b160c2	priority	100
21b65a28-97a5-4057-ae45-5e99b06824a4	e4f15a41-0141-409a-b2d8-0602d8b160c2	privateKey	MIIEowIBAAKCAQEAvCYnjwSWd7JlY0dIhhvLlMp4SVVxrScOSC9CNwVR2Oc1KxQGgsyJSlxH8xpdRC+UxWGKCf1xSKWpmplc6PV3pqIH6D5CI92jcLay+MRlMY23D1j5anFmGgF+lkHZ4HVkGaMnJl/+iGS7zWCwBq6XJ+y7Pv2TYHvuub+g00SsLuiCNlWUyd3oB1Qc2aGiaGy+kBF1ZB7p2cz88H5GyKDi6f7/wji4cAEknazoFNjScocgIBQxDVYGhD0YeCRk03sj9Puto9clF6PsbodQaWhG60giAdISNqZOa3cbhrg6miEpio9FTOpyqCX8zvFYQEY0WNWqyG1q9StR8LToIdYenQIDAQABAoIBACqoYOgvY1kMmKmnaAVF6ISZ2idQSMd4FygdFN9qJJovo5GahvZhXtA9928h/1reCdYtjPFCZcyRiNBdLKBBxSjBzFamxPKiRfPgbFf0GThgt8wyQUxVnQ9Q3u08zWPzYahhYhc0hP5SDnVcUlSVgX5jEToRHnit6JxzGsjDpWE14FLJTUYHO9wwahw8l4WZUmjakpxZXOkLGPEt5r9pCMEa6/n5UgiKkws43iEjZJdN7fhSr8msxR6Zyzwr/mNT8CDv4czLPTjYdvxX5dVyqVb/CHTlugy+dZv60IXcqO835rvc4qMKVX2P3li6G8YEpT3UjJVfQkwFRiEf1kwEBXkCgYEA91lHOJz87FFHsZ7CPOgTPyFm4e3SJ41mAi3bBoarzoM6jLjHezwrGydpQO4/HwWQL6IU2SzKefVbifTi+45WTwxWMuIsRaxnLIEuhYU3g+ovmSuW2Giw8bYdb/ZRadsKBmV2NoW8z8Ji5dWWPSd/cpRHkB6kn5cnbmNmnU/HXXUCgYEAwrrPvZYOj1e3LHd+TYdC1lp4VBqp/1IOuYbi4Oy76DPcjjVrFpYWHBtuQh221ddetoegLXC+6XaCCt8duHLXnMrFc8jNyogdMY4ZTicGmhmi5F3ZjljmmpL63AWJrWP5TUyrrhYeB9xWAsRnU1ABSrzuO/w7T+DpG7c1BNFWT4kCgYEApBUqhOjHc7D7oV2StrqxQdjTmh6cobZQLDPlLvAZetZc+cr+MccZRVsgnH0vjFGxc5fOwr7O1paoEd1M0dl7cdbaO8kD23hhvQk5JkPYLv8kcjSTTZ4tezr3TjWVjmZKwTF/eO3A8gWee57zmj/gcM7ipj1dsayg8L6p7MNmmp0CgYA38AYBBfwEvUPHKERF7RcMEE0Ei+xNhbrpHJBL4HVSDKPMzheHE3IifNa9c1+0KfvxHyCqLPsKgZp1mQYmmAbL6Yuy6PoEtfj85T7rrlY9KiQaycLXGz6TJKmfTo/wGQniZKXkikHyLB8hCF5OFmtolsEwxhIbrhYIeiZmzeOnUQKBgBCZzrvoMszSSrE9TLFuDRmi6KVFZPPKhyXaUo3fF1ZxMuKqXwb2DBWzahaozLWfZX/KZe0lqx7thf6o2UjFhr0BXDBgv8Z7rDJeV4IHNz1ZOjNNlBItirtDJ5LCSZfUDnzSB4NGXuC6cvVJxNBpmoVpLQ1wMausApTkxjUXwK8L
b2a9cd9f-82f6-40a0-8671-87ddaf3e15a2	e4f15a41-0141-409a-b2d8-0602d8b160c2	algorithm	RSA-OAEP
8f132a63-6c8d-40fe-b3b6-a40f01808aee	e4f15a41-0141-409a-b2d8-0602d8b160c2	certificate	MIICmzCCAYMCBgGaENjIxjANBgkqhkiG9w0BAQsFADARMQ8wDQYDVQQDDAZtYXN0ZXIwHhcNMjUxMDIzMTEzMjEzWhcNMzUxMDIzMTEzMzUzWjARMQ8wDQYDVQQDDAZtYXN0ZXIwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQC8JiePBJZ3smVjR0iGG8uUynhJVXGtJw5IL0I3BVHY5zUrFAaCzIlKXEfzGl1EL5TFYYoJ/XFIpamamVzo9XemogfoPkIj3aNwtrL4xGUxjbcPWPlqcWYaAX6WQdngdWQZoycmX/6IZLvNYLAGrpcn7Ls+/ZNge+65v6DTRKwu6II2VZTJ3egHVBzZoaJobL6QEXVkHunZzPzwfkbIoOLp/v/COLhwASSdrOgU2NJyhyAgFDENVgaEPRh4JGTTeyP0+62j1yUXo+xuh1BpaEbrSCIB0hI2pk5rdxuGuDqaISmKj0VM6nKoJfzO8VhARjRY1arIbWr1K1HwtOgh1h6dAgMBAAEwDQYJKoZIhvcNAQELBQADggEBAKKbzoc8A6JbWLPs7mVUzcHDQ6tNM7YyC/2Dvwi1GgLfB7eS0hJ2DU/7Na9waPeSdYMMXVcHTny3DbVbbY2ZOV0KpkGkN2kUaPPYRVo74frlo0MOwX7OWT+IHMTMfMwxOTcNT1zVtU2plqSI3UcOUfTA0MnJ7VeK+VNmTVpNzMB50oiRIoRWhpwVBSl0u7L7cmYxQuUPGQ9CsaA34S/YZfooOBDCotgPXpAde+hx2ev8t+fO6ihvshaJm/2FIbx2MOPdcP4Co5kLVos81zXnr7bg+5b6Z6K6AIPBMOEZGdw8rK7RXl/RVuQDJjMnSlAnO06yFpQ5qcrrRP2CWLdNwfM=
0a643336-7b70-496c-8b9e-c17bc6f964e4	c5b58635-88f8-432c-8a45-8f4cabd14904	kc.user.profile.config	{"attributes":[{"name":"username","displayName":"${username}","validations":{"length":{"min":3,"max":255},"username-prohibited-characters":{},"up-username-not-idn-homograph":{}},"permissions":{"view":["admin","user"],"edit":["admin","user"]},"multivalued":false},{"name":"email","displayName":"${email}","validations":{"email":{},"length":{"max":255}},"permissions":{"view":["admin","user"],"edit":["admin","user"]},"multivalued":false},{"name":"firstName","displayName":"${firstName}","validations":{"length":{"max":255},"person-name-prohibited-characters":{}},"permissions":{"view":["admin","user"],"edit":["admin","user"]},"multivalued":false},{"name":"lastName","displayName":"${lastName}","validations":{"length":{"max":255},"person-name-prohibited-characters":{}},"permissions":{"view":["admin","user"],"edit":["admin","user"]},"multivalued":false}],"groups":[{"name":"user-metadata","displayHeader":"User metadata","displayDescription":"Attributes, which refer to user metadata"}]}
98455bf6-a6a9-4411-8964-cd5d1820acea	807851b6-c074-4d4d-a001-e5befa43b073	certificate	MIICmzCCAYMCBgGaENjDtzANBgkqhkiG9w0BAQsFADARMQ8wDQYDVQQDDAZtYXN0ZXIwHhcNMjUxMDIzMTEzMjEyWhcNMzUxMDIzMTEzMzUyWjARMQ8wDQYDVQQDDAZtYXN0ZXIwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCt2aj1XU/tsfBbuyXoHxG+DcMCwZ8Je62YtM42wtdi8Bu2YEE003o4dkb/K0OP5x0fAsbvbcRTtqIaDJVo0qqgKDXBre2wSh80LPa6RL1w4DmvJJkCF0n83MyTEIX/gVgpQyTiuxhKnbDrdT4TzkgZ0yRb+EXO3JL3Y26+/rIvJuglKZUWZe8vgsF8OgjSXQ3deNQBDuyrSwA6/QV6gMYrc6bb4juDDe2ETBWz1plxwv1Qiu5lelE+xEp39yA1Ilux9NxYMQlvCVypUhD3QYXIOXZebix2FLPI0OmcBQoJIhyEQdIDIRUwycpeUF/sZ48hpdOWDNgePsp+vgmyp3UjAgMBAAEwDQYJKoZIhvcNAQELBQADggEBAEx7qqP/3TtCAsT0m3/r5wPhtUWqiiEYKOCh+3kSrNXIlh+HFuJWy9rbqL6YRUQ7gBkvw2pP9hp0DAhqsCM1Ex7YlSQc6X+DV22uqrQJHGCSOZXOTehf/fWVICBfsdATV5zg3534op20g7QJMvNX4BGQ1DGpU2eX0y5W5D5Bg3kZ8v2jktYeNrLnG7zp+59ykICN2dCVJyk5gYStSLZ/MXIkofPa/fIphul4tAv4FGb26TwQ0SqHQG3wSrDUVqVeQp4VD+8VTOH9j+Nfx+j4R/+azQIZihf2W4D+O5/frjamcAcJHRa87RZcbNWTIwuQ206V9UOJxRKjMfJejhmJ1fw=
0479e105-c50d-4bfe-a601-94fe5da4f8e7	807851b6-c074-4d4d-a001-e5befa43b073	priority	100
93d2c7cc-1c36-4880-91e0-5ca1f74a2abd	807851b6-c074-4d4d-a001-e5befa43b073	keyUse	SIG
642fecad-6d5c-40a9-a77a-b6bdeb986447	807851b6-c074-4d4d-a001-e5befa43b073	privateKey	MIIEowIBAAKCAQEArdmo9V1P7bHwW7sl6B8Rvg3DAsGfCXutmLTONsLXYvAbtmBBNNN6OHZG/ytDj+cdHwLG723EU7aiGgyVaNKqoCg1wa3tsEofNCz2ukS9cOA5rySZAhdJ/NzMkxCF/4FYKUMk4rsYSp2w63U+E85IGdMkW/hFztyS92Nuvv6yLyboJSmVFmXvL4LBfDoI0l0N3XjUAQ7sq0sAOv0FeoDGK3Om2+I7gw3thEwVs9aZccL9UIruZXpRPsRKd/cgNSJbsfTcWDEJbwlcqVIQ90GFyDl2Xm4sdhSzyNDpnAUKCSIchEHSAyEVMMnKXlBf7GePIaXTlgzYHj7Kfr4Jsqd1IwIDAQABAoIBAAFvQ0F1CIqiY2ZsyW+QjD4QA42DrzT9f4A/oMyGSXXJQHS4hgXMyihsHwuMjnZxlTYYJMlnZC9RSwFbI6f7daJIRzwkgmQXs7GszSReC9mHl7OjzCzBSQtYsu67sT6zSASxUmyk18ahoD4kJZU86j5YUA+9tzESdTed4+ld8n4a9kw/LxXla4kNBkk9uIbST6sRRf/k/uR46EuDLPSxDD9TGaLhl+8TJFp1IJhtQH9ehraOHPXbG2n2fkSPSdnqFUoePDXbi6XU+TPqoUPhDwmEXWPUdPrwfr3qTLqb34J7dNhItenlRi5RzY/whpvcuMPKYZhKphOfXCc2vFnlXSECgYEA4ZwaFy3rAVwDCzwU+kQ16z7OiKpscF9bmoSBQ9DPZGShPgZkk0rklmAiPYvkEuK+2cQGUhlTCSZ8VVR+D/rw+n/H89TZin9xxss6wrKJkpIIEXl9ONDSMpA6pzvY+cWxMjybcnrLvrZkNxgMlye9s9OHrXtk4QB7CnqUqxKE12ECgYEAxUSwWfSoC6zWafz6efl5CAph3Y+hKNaMclsOw2/D8IO7JT17FI2BmGJRa1T7ac1YsjVOTVCotLKq441E8rAy5/YIj5zWEeCr1eeHewKc1RHyNfI75LWYe4yi1Gqxe0HavSm7/I6G0KfHg/AQk9vl1QfbJ6wc03BCweNnTvniTwMCgYEAhVnwbfz9KCuLvQtBVwAUYr0X9N3PBMjIa1eg1EXLIAr+55t9vTYKUPv3oStbnvIZXrwoBETluMXfvcwYSzfHUjlPJceM4fkcdiVP0R8Yg+L/E90ccAW2CJ8JnCmawV/hsdDzDMKaRVqnd40me/3Iek8wphEGepv/Hh2AXHGwxWECgYAL2C76h70bTlplbMIhVsBUokL0Ca60hAVUzHDbfTd3loFDICjmuARjAOxAlI6v8Jt1v8ueyswGZX/rP803AStdUCF9df5IfoPyyP8SfLqr9HLlmFDOT7UDHhOcy3cynmai2wGljW081AYfvudP6XEoVFttS2FDtMbdW2kVjd1vvwKBgGGSk98Wy0QOrij1nnEAWcCPSgthBClfHW+vNaxFeOnM4TQEsLLZRSrLhbZNcuJIV+jRxs35hePsMCbn76LWP4yPxiaPJ8MDcsXDV3o3yY0Vra0bV73pKag4pJHViHomFepeoDgzoX+WFsju9iMVpcWk3LeTUEneMhhjOu20Vh3X
26eb5abb-b712-4d36-80d7-f7123466948f	c87cff2f-2060-4dfa-bd88-aaeb905bd6fc	kid	4f661bfd-07b9-406e-beed-2b9a2a52451a
e0142a28-1cbe-4660-be89-dc92ac704aeb	c87cff2f-2060-4dfa-bd88-aaeb905bd6fc	priority	100
8c6d4e31-80de-4093-9df0-933c0d28578c	c87cff2f-2060-4dfa-bd88-aaeb905bd6fc	secret	jslTDyXpjmT0NV0g5twChg
7e4556b8-3f48-4b41-88df-b97e1ac8ab76	c5a48332-40e8-42a4-998e-f5e2413786f9	kid	5b411799-895e-4246-a5b4-495622154539
bc76f7a0-fca5-4d60-aed9-e1009321e170	c5a48332-40e8-42a4-998e-f5e2413786f9	secret	zMl_AWhoV6XmuuYQ53tv37nuXtqQZVNDAIsnSVgtLa0VmGsBCATYoRpnbLx8GXtqqCz4okHFC2YH5SI7foCFS4oPIcbCL4TmX_fVHWanEh1I7fEIXjXdEz9w-A-TX4hE715YdL9poMiK0MBM_o3KnpGd1v9jZkOR0nHV2M9Vfew
dc6a2e2e-f1ab-4336-b6da-00eaa19fbb26	c5a48332-40e8-42a4-998e-f5e2413786f9	priority	100
08823d0f-6af2-4ad1-947f-bd4f89c766d8	c5a48332-40e8-42a4-998e-f5e2413786f9	algorithm	HS512
387eb2f6-f4b9-42b4-9c15-b635030a6ffd	56bc893c-0765-4c02-a81b-434000298b99	priority	100
7bda591f-0386-4e07-a8f7-ed1ea4844afb	56bc893c-0765-4c02-a81b-434000298b99	privateKey	MIIEogIBAAKCAQEAzc6g0EiFPN8/78iJknVzk542jGvyCcK28rTEIVNvuNROlgtgXWXxEybYQNuRABkuWFDZpHJ+HgjZLDC4HjYcuY2CYrooSxEr9NT2hdK4oxHIlJnzvQNNkXiw8//eM/DQCCuAkPWgswktQthz7w6lQbqse10PZ68NCcGJXAawkW+x8eflmieaDauhbXGGEM2IJXmeEbyzEBkDMR/lPzv/OIVn2X9Dmp8mQ8tzTG2XLKpsACCQVL6o4OL0vwnU295alCYi0Hhsj2+7+An0ID/A8DLYVzLkHeHrrYNxxsjmNvwJKFsI8T08d7+HL3uFV37m2mFSjE9BxX6x+RYLUk65wwIDAQABAoIBAAx7ludrLqGljNQ3XpjChPMUKozT00diRwRrkGXRELquLeIN6JdhooCd34n3h1DxNg1nQfwmcZ3SvZFflxtF/vIGMj1Yuy7qaLcd36H4SPijEH5q5Ay30ZGcN2dlQo318jg8ozynmUqiqNsgE9ARHFbjQqjs2Q1Z4bmQwosRYM5BUy08NUAFM0cFr5X8RyuydnbJWP8wKqK1P2/lFPKfV1WR3a+iIdMOa23wrIuUEhdmwcxuM0UbmecYoIGecCxgSLo+q7JgTIm7qkwEYTkBlXO1JdvYEq3Skr55R9wHnmhFYSUfPqMjYOZowouNmRtE8Xo3yF4WwTTB0ZdPwIddLmECgYEA8/0S4u3fnJCGiU3nGfPR6nJIhV/WztUga73QnU9hziweT5BJOrMkc2iLHQ0y7+MNRemAzaPZqZN12RCb3XCxM2+aPHapY6nFavkPRolU4x9AJx6AQ5DaT/goZwEKsUr8U8exR3b8iOloWUjVTfIDqonVRTkdlF12DZbLpKmuqtECgYEA1/BdD8vftA4/AmBWofSPy7tRkby1BXhaqtOO2z6ZFVxxMIiYSJYPDP8uZti5nLH0qK9CEvsorwMSU4Ckl9t3BBLf4mtTlKYFxBqQdoAhpLyfYiK9q7MCzlNl1xoD3LKQx9QtFodTT65vVUuPDawQNwXzxE0A59nU1w9yZ7792FMCgYBihAcCNgYaE4kfG6N6qSZTQb3ijKYRcaDboBMB0bGuK8Y5XgsTdo37Q3UYT+IHVmhfmD73yXbzr6Tf9oLY0M3b9O4UB92xbjVWUiZG1uC9rDqdEQW9QlnD/HFj1dnZaGul8HW2Y2boaUZG9s+Z32R3kZvZjYxSAcLPDdpALHtoYQKBgEOPGA6RInQa4OvFaFVTRonfGS4XdCs59GPtvIwrkYl1WpRsp/hNune+fq3+sFOt+XWtH2cA3UGkHF+zp8s7BolnvlQAcC3Zo4QvK3Nnsx7vRI5XgC/v2bjjrsEVio/J7jIW1+RsgkwzrvFoqmEErC4K1rFbxiTDqit+0bRTtOexAoGASSTPTSFmdzFG/gmnNTFCuKhkoaeacDQu4Q5dFMGdkHP9+sUad88XemEiJS2psuLXvZpXotrFszCozxNge+Klgk/T/lpbnHH3mT4KAtDJnL+ewgNQkMAcsC554tmAEVKjKCKfgA50KwWTzVinRhE/zG44g1jmtNmmsnQSh3Xg83s=
a94b0c85-efb7-44fc-bbd3-01baafebbd30	56bc893c-0765-4c02-a81b-434000298b99	keyUse	SIG
87a09b0a-f758-4fdf-9da6-f39b27db8c6c	56bc893c-0765-4c02-a81b-434000298b99	certificate	MIICqzCCAZMCBgGaEPOlPDANBgkqhkiG9w0BAQsFADAZMRcwFQYDVQQDDA5iYW1iYWliYS1yZWFsbTAeFw0yNTEwMjMxMjAxMzRaFw0zNTEwMjMxMjAzMTRaMBkxFzAVBgNVBAMMDmJhbWJhaWJhLXJlYWxtMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAzc6g0EiFPN8/78iJknVzk542jGvyCcK28rTEIVNvuNROlgtgXWXxEybYQNuRABkuWFDZpHJ+HgjZLDC4HjYcuY2CYrooSxEr9NT2hdK4oxHIlJnzvQNNkXiw8//eM/DQCCuAkPWgswktQthz7w6lQbqse10PZ68NCcGJXAawkW+x8eflmieaDauhbXGGEM2IJXmeEbyzEBkDMR/lPzv/OIVn2X9Dmp8mQ8tzTG2XLKpsACCQVL6o4OL0vwnU295alCYi0Hhsj2+7+An0ID/A8DLYVzLkHeHrrYNxxsjmNvwJKFsI8T08d7+HL3uFV37m2mFSjE9BxX6x+RYLUk65wwIDAQABMA0GCSqGSIb3DQEBCwUAA4IBAQC4GSPiu4M5fjL0Ou7etEmmGqauJ5lBWVvt8leow8cms3e7yBBj0fKnLjSVjx6GCuFkB8TTz4BLYOri4It91sVyOpSx6oBtGOK+PIwD9aOArxZzNmaenIMfAmlv3nPPoWXUBSjILp9i5n7YFdWNQtC93Xa6Mrrao6Of2U6jLBNV37TRjcbQyyT1B8XuQYphNxEb9Hdd04fPkwiTTJMu4+FhM0pZXiZEoOUf6vZuMG2VEhfp58oysue4IbeStVvDHuMVzaFDd8GBXLulAuyGU2XPfoHavrX6NG/RN1FbN9X77HCXUhXCzCA9StH19kA+TUHME9u8fXiyLrdNwVF/glmn
5d3ad769-4a2b-4bc3-bc8c-1e6d8dfbbd9d	f68f8bbc-1359-42e0-b295-9d8611fdf6e9	priority	100
9f534f62-92bf-41c4-9130-ea04380d619e	f68f8bbc-1359-42e0-b295-9d8611fdf6e9	privateKey	MIIEowIBAAKCAQEAqRLvMmfODmO8sXuFVPi8eFl7YDfutG0ndnnOa16gIg6FhNa9E0CtvN7/cQmVier4TeKviysOQqSJ28TSndYHkbYS9wOZaDVeZuBv9SNWxGSjJWqs94aziOhXRC4EzwaBvvxOOoFAlBPdV8fZhsui+iIaQObPVKH2jwa+xM81ss/9mK2qjjc31tmlHJNX0kdv6D1JSI3BYzI0xsQwKDiJ+hCQfvCwbooKSpwE3hid/0ussAJRqs0LM5m2Z1RgLevZlWSLmv8KS94j2QvBp5TjMTQM9PToI8keEI3OBOZgr7jYOO+ZMp+3MZy8UKPwwZIUD1MnlcXy/P6N9fr+Im1aGwIDAQABAoIBACVTLMMv72l10z5LSpALqW/YMUh4kJX8Uu3wzgUeCrP6CLfewDbz3GcJw9Ksjvq3iMGI/b/3bVqBAH4n4Wc+zMT6MQ2fGcrLnV+QxKaVep44qVulRTIP5qqewEUo857QjOwgvxRjXGJUUFUEpZ0Ab6ZQyQLy8Wp5pvjEQHEqXWxv6mH8btVqFZo2rWCCIRcS0+lWV2ncx64a/1+9cDJ4Pd7KLiVScLqGRGEoIH6A3P274x+4t3ukBqViVM/5UqZwrWy2+RJMAkE+yPyUL2gdMQYNYkjPxet4p7SHD+nAto4UJB5uxB4SwZH83/76FmPi6mGEVmZEZYyju267qkUQ5UECgYEA1mQE0A5Z29tY7rT09jB4DCKdyj/tkM5kfGFGUs8t4uBsSE4uCT5jmJ21oDD2QeM8pVSFcz1gzSHf4A4z1hFjXFUD76Cux+TbcL/F7ZXOBN7FbWNGveV06BQ/lriCARLpHf2Ng3n4vbvvvJgo1Ow2MW1Bbf/e0AH5e42Rqba4q6MCgYEAyeNbnxXA/sOVhzYDR+s1CxXqOSxdWV5UK5KiCGG9SZ78B14eM7vhqxXshYqAE8zqI4iTUbd20YsXS3Fvw1/IeVWIWrLziCqMdnRFZ2vvnaUyz5Z07dA5ryJ54KcwEF0io5vNDevF6Qa5ogPAKau4EUR0P3MH+vkoLQwQRlK8fykCgYEAsk4ZGpj07z+s5Usm+KIzFZSsuwsUH/0n4bJddH3O4gRYyoILE7EJ0GJKLmLKTN4bED4Nh1yi57imo4fsYLuxcZdbYK647pNOVqWIjiKrZ62HcYTBzynJY0OD52yo4w/BRiocyh5lXBe0OZwrSXrzT4RYiI3n2APsZG/Hl+jaChUCgYAd9te/adwmM+M6mUKtZvSNe6gy6inEvOJjQTU5Z0BP7//aHBcBeK5K1Uswmtjdl3lc4sVyyHcjLLC9as26wrmfgJlnVM4edKo6XpF6gyOiU0WSV/Ns5I3bN5O9SzMwCpK8k8BAg7DxICVnLYVU6f682Vsjm7AgiOB0pyi3N0pw2QKBgHrLaiXubjPo9M7NbkhcoYuHK7zVYBROFgTt+wtZV9Ge8k0q8QTepnWQY6TCG5dnXmpDQ9xv/9x3WhqsQgU2z9IdMTtZmH4+0cewhqAKrddBydtIz7xK19aBohFjVdqlfNJtdDH6Xg2g7pl9HROX74YqNblZDG4xAGf/Xkc/9JBD
b3e15d94-eac3-4b6e-99b1-723eb3bfa2fe	f68f8bbc-1359-42e0-b295-9d8611fdf6e9	certificate	MIICqzCCAZMCBgGaEPOrJTANBgkqhkiG9w0BAQsFADAZMRcwFQYDVQQDDA5iYW1iYWliYS1yZWFsbTAeFw0yNTEwMjMxMjAxMzVaFw0zNTEwMjMxMjAzMTVaMBkxFzAVBgNVBAMMDmJhbWJhaWJhLXJlYWxtMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqRLvMmfODmO8sXuFVPi8eFl7YDfutG0ndnnOa16gIg6FhNa9E0CtvN7/cQmVier4TeKviysOQqSJ28TSndYHkbYS9wOZaDVeZuBv9SNWxGSjJWqs94aziOhXRC4EzwaBvvxOOoFAlBPdV8fZhsui+iIaQObPVKH2jwa+xM81ss/9mK2qjjc31tmlHJNX0kdv6D1JSI3BYzI0xsQwKDiJ+hCQfvCwbooKSpwE3hid/0ussAJRqs0LM5m2Z1RgLevZlWSLmv8KS94j2QvBp5TjMTQM9PToI8keEI3OBOZgr7jYOO+ZMp+3MZy8UKPwwZIUD1MnlcXy/P6N9fr+Im1aGwIDAQABMA0GCSqGSIb3DQEBCwUAA4IBAQAp4dfPdsE0vjdJ7TjNv59jcQ6q6efJPG20CI8p8gD8p+qkDHtA7QAiUGCBs34m1s6YH37ddIp5YOZdhbXMOJ+ajleG3by3k5gb7fQRIrNY9H1jGWuqiDSZ/A6zCVqObmOdPtfGdlBLdA9Tgg370Zxaz/ItP+C4wxoR6lzCusiZxJPE174vRyVFE1sNrddyZprpPKuTIy3otNKRCbvZgrsjFWPZoJ9vK5I8Hl/36b7ydK+yRWyK3SlLiy5KOf/CkMriwuUMYeLy3hnynNXZNsiGZnSORLsc20NKXwjlF2RMXUc2CQcaahLujP6etu7RWO3bTNPbDEpZmGjulrttQ8KL
4ab2925b-a3f2-4c09-89d4-8b3c39118d4c	f68f8bbc-1359-42e0-b295-9d8611fdf6e9	keyUse	ENC
f9f9d462-17cd-43f6-a63b-f7b906701ae6	f68f8bbc-1359-42e0-b295-9d8611fdf6e9	algorithm	RSA-OAEP
c216338e-86a8-4a59-acbd-0f75b79655fc	cbb9f1a0-1f20-48b8-a62f-75d80a4cbb3f	priority	100
f9da0aa7-84e7-49d6-8e27-57d047a21cbd	cbb9f1a0-1f20-48b8-a62f-75d80a4cbb3f	algorithm	HS512
9ccd8645-112e-45c1-8afc-9bb20a1a7a54	cbb9f1a0-1f20-48b8-a62f-75d80a4cbb3f	secret	BQqB_ZBPdUrmwRPxPd0c821gYlyvB8tmyHOwSAige_xIxCVcueRmEnORMq_YvCosgdJOeBBbrASWBlL7oKpChYHe3CWOr7TobkPenI54qXWMAdLCVJi_TSIIv2qeekVBmAUSNG8-75zJpK7evXyTmMSTIVQ0JwmZL5gwDJs_gTU
ef5cef1c-6ac7-4b4d-9824-d3954b9ccafe	cbb9f1a0-1f20-48b8-a62f-75d80a4cbb3f	kid	cbf48735-b587-43e8-b21d-ccbb5ef7de5c
40f8d1b1-8b90-4622-a54b-6c7c50ad6e5c	642e25b1-3fb7-43c6-b9b6-a2532a2c5b02	kid	dd76f67b-4e0f-4d59-843f-a4924bdf7371
bd2d42b4-80e0-41c1-8f46-1fd0158a9694	642e25b1-3fb7-43c6-b9b6-a2532a2c5b02	priority	100
5d99984b-3f1d-4abf-aed5-50b95b3d08b8	642e25b1-3fb7-43c6-b9b6-a2532a2c5b02	secret	RDywBaci4h2wkni_t59O_w
dc1071bf-9d8a-461c-a102-b5972f86c32f	25b2db25-222d-4e1d-a23d-7e2c8d011da7	allowed-protocol-mapper-types	saml-role-list-mapper
8794a0f7-18ac-496c-b4c6-79a5f42c565d	25b2db25-222d-4e1d-a23d-7e2c8d011da7	allowed-protocol-mapper-types	saml-user-attribute-mapper
3f1e3deb-222d-4dfa-95e7-58db1b44b98c	25b2db25-222d-4e1d-a23d-7e2c8d011da7	allowed-protocol-mapper-types	oidc-address-mapper
88dc6ae0-3597-4c22-9fef-77299eafd2ed	25b2db25-222d-4e1d-a23d-7e2c8d011da7	allowed-protocol-mapper-types	oidc-full-name-mapper
f5a3a170-2c97-4bc6-9300-be7b6e7788e5	25b2db25-222d-4e1d-a23d-7e2c8d011da7	allowed-protocol-mapper-types	oidc-sha256-pairwise-sub-mapper
f764cae7-019e-4d60-8925-2715f94a8a70	25b2db25-222d-4e1d-a23d-7e2c8d011da7	allowed-protocol-mapper-types	oidc-usermodel-attribute-mapper
5e539915-9c6a-4fa1-9e49-6cf923ab2f60	25b2db25-222d-4e1d-a23d-7e2c8d011da7	allowed-protocol-mapper-types	saml-user-property-mapper
d80348db-3b75-4522-9ba4-439ec8581c8b	25b2db25-222d-4e1d-a23d-7e2c8d011da7	allowed-protocol-mapper-types	oidc-usermodel-property-mapper
870597ac-18f1-446c-ac8a-33d1a9ab1f7d	036e5885-7f9e-4e1d-8ff6-c9e4ac35fc28	client-uris-must-match	true
ad2c4789-4089-42e2-a1b2-cba072d55ee1	036e5885-7f9e-4e1d-8ff6-c9e4ac35fc28	host-sending-registration-request-must-match	true
e7d173d2-c0f6-45d9-b9ab-e7c12aba3484	bff1a4c2-921b-4bce-bf34-a52e93e9bdbe	max-clients	200
97abc801-671b-4ed4-851a-9b5b87912b1f	8670777d-3f59-410b-ae49-497560d95e50	allowed-protocol-mapper-types	oidc-address-mapper
26d553b8-8b5b-4f5b-a4d3-563b9ef9f8ed	8670777d-3f59-410b-ae49-497560d95e50	allowed-protocol-mapper-types	saml-user-property-mapper
96a2be3a-fd98-43f7-af90-37aa0fd31d9a	8670777d-3f59-410b-ae49-497560d95e50	allowed-protocol-mapper-types	oidc-usermodel-property-mapper
51dbb709-9c90-4a0c-9f71-b3298197c349	8670777d-3f59-410b-ae49-497560d95e50	allowed-protocol-mapper-types	saml-role-list-mapper
4d05a41c-7154-4fdc-9b69-f7dbfc8212d9	8670777d-3f59-410b-ae49-497560d95e50	allowed-protocol-mapper-types	saml-user-attribute-mapper
ed8e5cf1-d610-4d0d-bde3-db69f32b0889	8670777d-3f59-410b-ae49-497560d95e50	allowed-protocol-mapper-types	oidc-usermodel-attribute-mapper
b58a5867-3821-4eae-ad24-f2728210d985	8670777d-3f59-410b-ae49-497560d95e50	allowed-protocol-mapper-types	oidc-full-name-mapper
0ef2f009-e440-4831-a054-e43f18125159	8670777d-3f59-410b-ae49-497560d95e50	allowed-protocol-mapper-types	oidc-sha256-pairwise-sub-mapper
dcf8d2ae-83a5-4667-9d7c-f77b14136b3c	92be0202-5138-4cfe-ac42-bd543617885f	allow-default-scopes	true
c704a8a4-1c4b-4260-9cdf-4d0760fa6a9c	4104e90f-6caf-478b-ab9a-c5825cb5dee3	allow-default-scopes	true
\.


--
-- TOC entry 4135 (class 0 OID 24609)
-- Dependencies: 218
-- Data for Name: composite_role; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.composite_role (composite, child_role) FROM stdin;
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	dc53ea4d-3fcb-46ac-a814-df8415b9bb40
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	64cc72b0-840b-4dfe-8f4e-b63c713ee89f
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	380e1a65-8df9-436a-8de2-f4e1234cc8b2
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	4157d79f-00e3-4da9-915c-80008d3ba419
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	0ae04e8d-0a82-47de-af0b-d8115bfbd362
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	f47467eb-6bb9-4c13-9f1f-4c1d3d971818
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	5d2fbb55-64d9-49c8-a075-b5ab93c54099
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	87860229-fed9-491d-9148-43936b486420
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	356f1685-3619-41ae-92d3-e100d78f77d6
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	9d104f8e-c89c-434a-aac9-5ef19008b5cf
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	6fce7751-623a-4f50-baf3-98d5859544ed
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	ac5ff45d-649c-4845-9cf9-ddd596b5f265
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	b0918c3c-35f4-40cb-90f6-91cb3c1693e5
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	94d154d2-ec32-47ff-a94f-fbcdea2d83ab
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	0536112c-0684-4d3c-8bca-dcb371abc4df
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	5f4f4ea4-6492-40c5-b5ee-2792203858db
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	b3c278c3-1100-40f7-87e1-1fc5d5b6cfa1
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	022ac9ce-2ddd-4cdd-84d4-03193514f22e
0ae04e8d-0a82-47de-af0b-d8115bfbd362	5f4f4ea4-6492-40c5-b5ee-2792203858db
333d5b0e-b35b-447c-849f-6d1954882d54	390fff60-e27e-4c07-8950-30214e724306
4157d79f-00e3-4da9-915c-80008d3ba419	022ac9ce-2ddd-4cdd-84d4-03193514f22e
4157d79f-00e3-4da9-915c-80008d3ba419	0536112c-0684-4d3c-8bca-dcb371abc4df
333d5b0e-b35b-447c-849f-6d1954882d54	7c1f6e3d-2852-442c-81a0-0db59b56eea1
7c1f6e3d-2852-442c-81a0-0db59b56eea1	1b14b11f-7685-44ab-9d6e-740aef46a781
69cfe3f6-0cd9-44c6-90c5-1cd521be9fdd	aa4848f8-d34c-4199-99ba-ee75a4741a9d
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	abbf467d-4085-4b6d-b964-2202113c144f
333d5b0e-b35b-447c-849f-6d1954882d54	80bbb03d-dddd-4339-84b1-690dbf029de8
333d5b0e-b35b-447c-849f-6d1954882d54	0b7066ed-54b3-4ceb-814a-c7df9f5eba60
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	58e89a31-840e-49fc-8677-a26f2a9ed5a2
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	c7ba1b75-982f-43b7-bd10-8e0feef9c554
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	8efde795-9511-4eb3-abe9-b8468bc02983
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	6b3de988-e5d4-4c6f-b5d6-0490f9935949
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	5d5eb9ed-6c16-4ff4-9ab5-7a2e3730be41
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	e91b22e7-31af-41cc-947a-ea536fe8f03e
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	3a7c9cf0-68e4-4f14-b8cf-4aaea27bde21
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	222ff0cb-515e-42d4-a3f6-e195a33a58df
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	548365c0-c74f-401e-a49d-f27dde9b8fc3
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	3dee69c1-baf4-4083-95c9-e9e253c6e531
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	a0b7e6c7-5ba5-472b-af73-effdc48e268b
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	9f3a7329-42e6-4a17-8254-a9844ca68dc7
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	2248f8c9-e631-4c60-9cef-f30394594d45
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	2e93cba4-9c6e-4769-9a5f-3662b8824ddf
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	df44c961-8daa-4462-af0d-835543b505af
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	c3b1ccd2-a938-412b-b5e2-4eb2e1d4b7bd
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	7b211510-1bdf-4dad-81ab-9d6b7860e74a
6b3de988-e5d4-4c6f-b5d6-0490f9935949	df44c961-8daa-4462-af0d-835543b505af
8efde795-9511-4eb3-abe9-b8468bc02983	2e93cba4-9c6e-4769-9a5f-3662b8824ddf
8efde795-9511-4eb3-abe9-b8468bc02983	7b211510-1bdf-4dad-81ab-9d6b7860e74a
951a9506-6d8f-454a-8d91-99aa9a1e34ec	bef72c5d-9c9e-43eb-83ec-b2795d440d3b
951a9506-6d8f-454a-8d91-99aa9a1e34ec	c279ff02-28e0-4a86-a64a-f898ade088bb
951a9506-6d8f-454a-8d91-99aa9a1e34ec	f07d7887-ca58-42da-af92-2b818d9df8a2
951a9506-6d8f-454a-8d91-99aa9a1e34ec	dd1e4035-f356-4fb5-bdab-49c13aeaaf3b
951a9506-6d8f-454a-8d91-99aa9a1e34ec	de1eb8b9-4d6c-4165-858a-94ea47301ad0
951a9506-6d8f-454a-8d91-99aa9a1e34ec	e1816621-1c04-432f-8e17-a9c594952c31
951a9506-6d8f-454a-8d91-99aa9a1e34ec	0eb837e0-1e0d-4f25-8ed5-efc5ef872be1
951a9506-6d8f-454a-8d91-99aa9a1e34ec	2827a618-c764-4769-8ca2-b6e3463dbd71
951a9506-6d8f-454a-8d91-99aa9a1e34ec	966cb239-0efe-4e7a-a18c-a2e814e872ad
951a9506-6d8f-454a-8d91-99aa9a1e34ec	1e1bcb15-2939-4bbe-abb6-72aca08fc061
951a9506-6d8f-454a-8d91-99aa9a1e34ec	9abc4903-5fcc-452c-bd00-636d02a59b02
951a9506-6d8f-454a-8d91-99aa9a1e34ec	511e0b44-7570-4a99-9f0d-491ecc2b0510
951a9506-6d8f-454a-8d91-99aa9a1e34ec	566f768d-b842-4e5f-bd97-d21963abaaa1
951a9506-6d8f-454a-8d91-99aa9a1e34ec	6f1f8f24-ef5e-48c4-b903-487443d13235
951a9506-6d8f-454a-8d91-99aa9a1e34ec	f7001d0c-d197-406a-b9af-3dbcc1580f03
951a9506-6d8f-454a-8d91-99aa9a1e34ec	d2601b00-9729-473c-aadb-4cf8ff6f8676
951a9506-6d8f-454a-8d91-99aa9a1e34ec	cbefa965-534f-47e4-83d5-2da96b31260c
6337a65d-feea-4b7c-b5e2-6d122b88b571	0a4435de-49f4-4a96-a158-443c5ac3317d
dd1e4035-f356-4fb5-bdab-49c13aeaaf3b	f7001d0c-d197-406a-b9af-3dbcc1580f03
f07d7887-ca58-42da-af92-2b818d9df8a2	6f1f8f24-ef5e-48c4-b903-487443d13235
f07d7887-ca58-42da-af92-2b818d9df8a2	cbefa965-534f-47e4-83d5-2da96b31260c
6337a65d-feea-4b7c-b5e2-6d122b88b571	416f330a-0839-466e-a3f3-7baebb4a3e61
416f330a-0839-466e-a3f3-7baebb4a3e61	cfbb7550-b6a9-488f-ab57-f6240dd0cbf9
c5000d03-36d6-4c57-8170-6e9ce9454f83	d8aeb9b6-b2ef-4a59-95de-9058d9bcaa7a
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	f7199ff7-a9ef-4a27-a3b5-ae68a4f19ca5
951a9506-6d8f-454a-8d91-99aa9a1e34ec	1086d057-7584-4416-b18c-14201f82274a
6337a65d-feea-4b7c-b5e2-6d122b88b571	163f9ffa-1a43-48cd-8ad3-a918279550ae
6337a65d-feea-4b7c-b5e2-6d122b88b571	663e2268-8919-4289-9cdb-4ec4347d7e58
6337a65d-feea-4b7c-b5e2-6d122b88b571	966cb239-0efe-4e7a-a18c-a2e814e872ad
\.


--
-- TOC entry 4136 (class 0 OID 24612)
-- Dependencies: 219
-- Data for Name: credential; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.credential (id, salt, type, user_id, created_date, user_label, secret_data, credential_data, priority) FROM stdin;
19646cf0-95a0-4bc7-ac59-464b1552cd30	\N	password	416e44c2-0739-4968-b623-78f86e44b6b1	1761219237429	\N	{"value":"GoVYA9KFvFPCYRy021nCUxTFF1iDZjeyur1rCuiGiqU=","salt":"GH6RAylAjhJ1axZJ2Np67A==","additionalParameters":{}}	{"hashIterations":5,"algorithm":"argon2","additionalParameters":{"hashLength":["32"],"memory":["7168"],"type":["id"],"version":["1.3"],"parallelism":["1"]}}	10
429c1a6e-fdfa-42b3-9ad8-ee970ec5a646	\N	password	f1bd369f-b87c-4434-bdf7-e6cc125849d1	1761221100731	My password	{"value":"T1q+KcNYA6nATgpAbZpStY0WQDmmfamJOemKwind+DA=","salt":"KKmLgFmoruKL6KOcCSN5jA==","additionalParameters":{}}	{"hashIterations":5,"algorithm":"argon2","additionalParameters":{"hashLength":["32"],"memory":["7168"],"type":["id"],"version":["1.3"],"parallelism":["1"]}}	10
893c3e4f-e408-4ccb-a1d7-3d0c44ca9c45	\N	password	5339f261-b577-47ba-b3c7-fe335de2090f	1763545941126	\N	{"value":"SAv6X2Ykjbk43Fghx8zwLTO/wqcyRSR9f+knwPoWxlM=","salt":"95zGs2OO8BkhAYqJ40YRXA==","additionalParameters":{}}	{"hashIterations":5,"algorithm":"argon2","additionalParameters":{"hashLength":["32"],"memory":["7168"],"type":["id"],"version":["1.3"],"parallelism":["1"]}}	10
bc28eeff-f59c-45f9-8ddf-289590fc348b	\N	password	ddd2c493-94bc-47d8-b94e-b5f24188e8d4	1763546027845	\N	{"value":"P8yY7eaTiqWIa9rCQmeLxWg7fPFtgY242D6wY/AotUg=","salt":"echH95I0Yy5G6SkIO5hqBA==","additionalParameters":{}}	{"hashIterations":5,"algorithm":"argon2","additionalParameters":{"hashLength":["32"],"memory":["7168"],"type":["id"],"version":["1.3"],"parallelism":["1"]}}	10
a8c49994-4554-4ca5-bc83-8ab9c7d9cae6	\N	password	3ecb9173-ca70-49e6-a3df-1945943ead6e	1764684813198	My password	{"value":"Kqfv6GdsQj7G5a69SXK3zB0dFbRihzxoe9ClI3hRhvw=","salt":"E89FtwyyjEjHKdCF9CsHhQ==","additionalParameters":{}}	{"hashIterations":5,"algorithm":"argon2","additionalParameters":{"hashLength":["32"],"memory":["7168"],"type":["id"],"version":["1.3"],"parallelism":["1"]}}	10
002b7143-bc62-4516-9c7d-12a05a48ee03	\N	password	94557eed-86d5-4d01-bc8d-767f1a6bd89c	1764761712753	\N	{"value":"WjQf+hJWGxb70AuHdxTMKUMqIWI0BDb1gLdi98Unheg=","salt":"P9o5I1Y/SJvN4h5pwCT/1w==","additionalParameters":{}}	{"hashIterations":5,"algorithm":"argon2","additionalParameters":{"hashLength":["32"],"memory":["7168"],"type":["id"],"version":["1.3"],"parallelism":["1"]}}	10
\.


--
-- TOC entry 4133 (class 0 OID 24582)
-- Dependencies: 216
-- Data for Name: databasechangelog; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.databasechangelog (id, author, filename, dateexecuted, orderexecuted, exectype, md5sum, description, comments, tag, liquibase, contexts, labels, deployment_id) FROM stdin;
1.0.0.Final-KEYCLOAK-5461	sthorger@redhat.com	META-INF/jpa-changelog-1.0.0.Final.xml	2025-10-23 11:30:36.137568	1	EXECUTED	9:6f1016664e21e16d26517a4418f5e3df	createTable tableName=APPLICATION_DEFAULT_ROLES; createTable tableName=CLIENT; createTable tableName=CLIENT_SESSION; createTable tableName=CLIENT_SESSION_ROLE; createTable tableName=COMPOSITE_ROLE; createTable tableName=CREDENTIAL; createTable tab...		\N	4.29.1	\N	\N	1219031908
1.0.0.Final-KEYCLOAK-5461	sthorger@redhat.com	META-INF/db2-jpa-changelog-1.0.0.Final.xml	2025-10-23 11:30:36.343485	2	MARK_RAN	9:828775b1596a07d1200ba1d49e5e3941	createTable tableName=APPLICATION_DEFAULT_ROLES; createTable tableName=CLIENT; createTable tableName=CLIENT_SESSION; createTable tableName=CLIENT_SESSION_ROLE; createTable tableName=COMPOSITE_ROLE; createTable tableName=CREDENTIAL; createTable tab...		\N	4.29.1	\N	\N	1219031908
1.1.0.Beta1	sthorger@redhat.com	META-INF/jpa-changelog-1.1.0.Beta1.xml	2025-10-23 11:30:36.704906	3	EXECUTED	9:5f090e44a7d595883c1fb61f4b41fd38	delete tableName=CLIENT_SESSION_ROLE; delete tableName=CLIENT_SESSION; delete tableName=USER_SESSION; createTable tableName=CLIENT_ATTRIBUTES; createTable tableName=CLIENT_SESSION_NOTE; createTable tableName=APP_NODE_REGISTRATIONS; addColumn table...		\N	4.29.1	\N	\N	1219031908
1.1.0.Final	sthorger@redhat.com	META-INF/jpa-changelog-1.1.0.Final.xml	2025-10-23 11:30:36.743113	4	EXECUTED	9:c07e577387a3d2c04d1adc9aaad8730e	renameColumn newColumnName=EVENT_TIME, oldColumnName=TIME, tableName=EVENT_ENTITY		\N	4.29.1	\N	\N	1219031908
1.2.0.Beta1	psilva@redhat.com	META-INF/jpa-changelog-1.2.0.Beta1.xml	2025-10-23 11:30:37.803757	5	EXECUTED	9:b68ce996c655922dbcd2fe6b6ae72686	delete tableName=CLIENT_SESSION_ROLE; delete tableName=CLIENT_SESSION_NOTE; delete tableName=CLIENT_SESSION; delete tableName=USER_SESSION; createTable tableName=PROTOCOL_MAPPER; createTable tableName=PROTOCOL_MAPPER_CONFIG; createTable tableName=...		\N	4.29.1	\N	\N	1219031908
1.2.0.Beta1	psilva@redhat.com	META-INF/db2-jpa-changelog-1.2.0.Beta1.xml	2025-10-23 11:30:37.898053	6	MARK_RAN	9:543b5c9989f024fe35c6f6c5a97de88e	delete tableName=CLIENT_SESSION_ROLE; delete tableName=CLIENT_SESSION_NOTE; delete tableName=CLIENT_SESSION; delete tableName=USER_SESSION; createTable tableName=PROTOCOL_MAPPER; createTable tableName=PROTOCOL_MAPPER_CONFIG; createTable tableName=...		\N	4.29.1	\N	\N	1219031908
1.2.0.RC1	bburke@redhat.com	META-INF/jpa-changelog-1.2.0.CR1.xml	2025-10-23 11:30:38.785658	7	EXECUTED	9:765afebbe21cf5bbca048e632df38336	delete tableName=CLIENT_SESSION_ROLE; delete tableName=CLIENT_SESSION_NOTE; delete tableName=CLIENT_SESSION; delete tableName=USER_SESSION_NOTE; delete tableName=USER_SESSION; createTable tableName=MIGRATION_MODEL; createTable tableName=IDENTITY_P...		\N	4.29.1	\N	\N	1219031908
1.2.0.RC1	bburke@redhat.com	META-INF/db2-jpa-changelog-1.2.0.CR1.xml	2025-10-23 11:30:38.915194	8	MARK_RAN	9:db4a145ba11a6fdaefb397f6dbf829a1	delete tableName=CLIENT_SESSION_ROLE; delete tableName=CLIENT_SESSION_NOTE; delete tableName=CLIENT_SESSION; delete tableName=USER_SESSION_NOTE; delete tableName=USER_SESSION; createTable tableName=MIGRATION_MODEL; createTable tableName=IDENTITY_P...		\N	4.29.1	\N	\N	1219031908
1.2.0.Final	keycloak	META-INF/jpa-changelog-1.2.0.Final.xml	2025-10-23 11:30:39.001651	9	EXECUTED	9:9d05c7be10cdb873f8bcb41bc3a8ab23	update tableName=CLIENT; update tableName=CLIENT; update tableName=CLIENT		\N	4.29.1	\N	\N	1219031908
1.3.0	bburke@redhat.com	META-INF/jpa-changelog-1.3.0.xml	2025-10-23 11:30:39.904526	10	EXECUTED	9:18593702353128d53111f9b1ff0b82b8	delete tableName=CLIENT_SESSION_ROLE; delete tableName=CLIENT_SESSION_PROT_MAPPER; delete tableName=CLIENT_SESSION_NOTE; delete tableName=CLIENT_SESSION; delete tableName=USER_SESSION_NOTE; delete tableName=USER_SESSION; createTable tableName=ADMI...		\N	4.29.1	\N	\N	1219031908
1.4.0	bburke@redhat.com	META-INF/jpa-changelog-1.4.0.xml	2025-10-23 11:30:40.408272	11	EXECUTED	9:6122efe5f090e41a85c0f1c9e52cbb62	delete tableName=CLIENT_SESSION_AUTH_STATUS; delete tableName=CLIENT_SESSION_ROLE; delete tableName=CLIENT_SESSION_PROT_MAPPER; delete tableName=CLIENT_SESSION_NOTE; delete tableName=CLIENT_SESSION; delete tableName=USER_SESSION_NOTE; delete table...		\N	4.29.1	\N	\N	1219031908
1.4.0	bburke@redhat.com	META-INF/db2-jpa-changelog-1.4.0.xml	2025-10-23 11:30:40.447685	12	MARK_RAN	9:e1ff28bf7568451453f844c5d54bb0b5	delete tableName=CLIENT_SESSION_AUTH_STATUS; delete tableName=CLIENT_SESSION_ROLE; delete tableName=CLIENT_SESSION_PROT_MAPPER; delete tableName=CLIENT_SESSION_NOTE; delete tableName=CLIENT_SESSION; delete tableName=USER_SESSION_NOTE; delete table...		\N	4.29.1	\N	\N	1219031908
1.5.0	bburke@redhat.com	META-INF/jpa-changelog-1.5.0.xml	2025-10-23 11:30:40.630043	13	EXECUTED	9:7af32cd8957fbc069f796b61217483fd	delete tableName=CLIENT_SESSION_AUTH_STATUS; delete tableName=CLIENT_SESSION_ROLE; delete tableName=CLIENT_SESSION_PROT_MAPPER; delete tableName=CLIENT_SESSION_NOTE; delete tableName=CLIENT_SESSION; delete tableName=USER_SESSION_NOTE; delete table...		\N	4.29.1	\N	\N	1219031908
1.6.1_from15	mposolda@redhat.com	META-INF/jpa-changelog-1.6.1.xml	2025-10-23 11:30:40.860561	14	EXECUTED	9:6005e15e84714cd83226bf7879f54190	addColumn tableName=REALM; addColumn tableName=KEYCLOAK_ROLE; addColumn tableName=CLIENT; createTable tableName=OFFLINE_USER_SESSION; createTable tableName=OFFLINE_CLIENT_SESSION; addPrimaryKey constraintName=CONSTRAINT_OFFL_US_SES_PK2, tableName=...		\N	4.29.1	\N	\N	1219031908
1.6.1_from16-pre	mposolda@redhat.com	META-INF/jpa-changelog-1.6.1.xml	2025-10-23 11:30:40.871415	15	MARK_RAN	9:bf656f5a2b055d07f314431cae76f06c	delete tableName=OFFLINE_CLIENT_SESSION; delete tableName=OFFLINE_USER_SESSION		\N	4.29.1	\N	\N	1219031908
1.6.1_from16	mposolda@redhat.com	META-INF/jpa-changelog-1.6.1.xml	2025-10-23 11:30:40.898706	16	MARK_RAN	9:f8dadc9284440469dcf71e25ca6ab99b	dropPrimaryKey constraintName=CONSTRAINT_OFFLINE_US_SES_PK, tableName=OFFLINE_USER_SESSION; dropPrimaryKey constraintName=CONSTRAINT_OFFLINE_CL_SES_PK, tableName=OFFLINE_CLIENT_SESSION; addColumn tableName=OFFLINE_USER_SESSION; update tableName=OF...		\N	4.29.1	\N	\N	1219031908
1.6.1	mposolda@redhat.com	META-INF/jpa-changelog-1.6.1.xml	2025-10-23 11:30:40.917571	17	EXECUTED	9:d41d8cd98f00b204e9800998ecf8427e	empty		\N	4.29.1	\N	\N	1219031908
1.7.0	bburke@redhat.com	META-INF/jpa-changelog-1.7.0.xml	2025-10-23 11:30:41.236316	18	EXECUTED	9:3368ff0be4c2855ee2dd9ca813b38d8e	createTable tableName=KEYCLOAK_GROUP; createTable tableName=GROUP_ROLE_MAPPING; createTable tableName=GROUP_ATTRIBUTE; createTable tableName=USER_GROUP_MEMBERSHIP; createTable tableName=REALM_DEFAULT_GROUPS; addColumn tableName=IDENTITY_PROVIDER; ...		\N	4.29.1	\N	\N	1219031908
1.8.0	mposolda@redhat.com	META-INF/jpa-changelog-1.8.0.xml	2025-10-23 11:30:41.564478	19	EXECUTED	9:8ac2fb5dd030b24c0570a763ed75ed20	addColumn tableName=IDENTITY_PROVIDER; createTable tableName=CLIENT_TEMPLATE; createTable tableName=CLIENT_TEMPLATE_ATTRIBUTES; createTable tableName=TEMPLATE_SCOPE_MAPPING; dropNotNullConstraint columnName=CLIENT_ID, tableName=PROTOCOL_MAPPER; ad...		\N	4.29.1	\N	\N	1219031908
1.8.0-2	keycloak	META-INF/jpa-changelog-1.8.0.xml	2025-10-23 11:30:41.630907	20	EXECUTED	9:f91ddca9b19743db60e3057679810e6c	dropDefaultValue columnName=ALGORITHM, tableName=CREDENTIAL; update tableName=CREDENTIAL		\N	4.29.1	\N	\N	1219031908
1.8.0	mposolda@redhat.com	META-INF/db2-jpa-changelog-1.8.0.xml	2025-10-23 11:30:41.665824	21	MARK_RAN	9:831e82914316dc8a57dc09d755f23c51	addColumn tableName=IDENTITY_PROVIDER; createTable tableName=CLIENT_TEMPLATE; createTable tableName=CLIENT_TEMPLATE_ATTRIBUTES; createTable tableName=TEMPLATE_SCOPE_MAPPING; dropNotNullConstraint columnName=CLIENT_ID, tableName=PROTOCOL_MAPPER; ad...		\N	4.29.1	\N	\N	1219031908
1.8.0-2	keycloak	META-INF/db2-jpa-changelog-1.8.0.xml	2025-10-23 11:30:41.689811	22	MARK_RAN	9:f91ddca9b19743db60e3057679810e6c	dropDefaultValue columnName=ALGORITHM, tableName=CREDENTIAL; update tableName=CREDENTIAL		\N	4.29.1	\N	\N	1219031908
1.9.0	mposolda@redhat.com	META-INF/jpa-changelog-1.9.0.xml	2025-10-23 11:30:42.497236	23	EXECUTED	9:bc3d0f9e823a69dc21e23e94c7a94bb1	update tableName=REALM; update tableName=REALM; update tableName=REALM; update tableName=REALM; update tableName=CREDENTIAL; update tableName=CREDENTIAL; update tableName=CREDENTIAL; update tableName=REALM; update tableName=REALM; customChange; dr...		\N	4.29.1	\N	\N	1219031908
1.9.1	keycloak	META-INF/jpa-changelog-1.9.1.xml	2025-10-23 11:30:42.532975	24	EXECUTED	9:c9999da42f543575ab790e76439a2679	modifyDataType columnName=PRIVATE_KEY, tableName=REALM; modifyDataType columnName=PUBLIC_KEY, tableName=REALM; modifyDataType columnName=CERTIFICATE, tableName=REALM		\N	4.29.1	\N	\N	1219031908
1.9.1	keycloak	META-INF/db2-jpa-changelog-1.9.1.xml	2025-10-23 11:30:42.546407	25	MARK_RAN	9:0d6c65c6f58732d81569e77b10ba301d	modifyDataType columnName=PRIVATE_KEY, tableName=REALM; modifyDataType columnName=CERTIFICATE, tableName=REALM		\N	4.29.1	\N	\N	1219031908
1.9.2	keycloak	META-INF/jpa-changelog-1.9.2.xml	2025-10-23 11:30:46.555223	26	EXECUTED	9:fc576660fc016ae53d2d4778d84d86d0	createIndex indexName=IDX_USER_EMAIL, tableName=USER_ENTITY; createIndex indexName=IDX_USER_ROLE_MAPPING, tableName=USER_ROLE_MAPPING; createIndex indexName=IDX_USER_GROUP_MAPPING, tableName=USER_GROUP_MEMBERSHIP; createIndex indexName=IDX_USER_CO...		\N	4.29.1	\N	\N	1219031908
authz-2.0.0	psilva@redhat.com	META-INF/jpa-changelog-authz-2.0.0.xml	2025-10-23 11:30:47.126183	27	EXECUTED	9:43ed6b0da89ff77206289e87eaa9c024	createTable tableName=RESOURCE_SERVER; addPrimaryKey constraintName=CONSTRAINT_FARS, tableName=RESOURCE_SERVER; addUniqueConstraint constraintName=UK_AU8TT6T700S9V50BU18WS5HA6, tableName=RESOURCE_SERVER; createTable tableName=RESOURCE_SERVER_RESOU...		\N	4.29.1	\N	\N	1219031908
authz-2.5.1	psilva@redhat.com	META-INF/jpa-changelog-authz-2.5.1.xml	2025-10-23 11:30:47.146828	28	EXECUTED	9:44bae577f551b3738740281eceb4ea70	update tableName=RESOURCE_SERVER_POLICY		\N	4.29.1	\N	\N	1219031908
2.1.0-KEYCLOAK-5461	bburke@redhat.com	META-INF/jpa-changelog-2.1.0.xml	2025-10-23 11:30:47.767738	29	EXECUTED	9:bd88e1f833df0420b01e114533aee5e8	createTable tableName=BROKER_LINK; createTable tableName=FED_USER_ATTRIBUTE; createTable tableName=FED_USER_CONSENT; createTable tableName=FED_USER_CONSENT_ROLE; createTable tableName=FED_USER_CONSENT_PROT_MAPPER; createTable tableName=FED_USER_CR...		\N	4.29.1	\N	\N	1219031908
2.2.0	bburke@redhat.com	META-INF/jpa-changelog-2.2.0.xml	2025-10-23 11:30:47.84726	30	EXECUTED	9:a7022af5267f019d020edfe316ef4371	addColumn tableName=ADMIN_EVENT_ENTITY; createTable tableName=CREDENTIAL_ATTRIBUTE; createTable tableName=FED_CREDENTIAL_ATTRIBUTE; modifyDataType columnName=VALUE, tableName=CREDENTIAL; addForeignKeyConstraint baseTableName=FED_CREDENTIAL_ATTRIBU...		\N	4.29.1	\N	\N	1219031908
2.3.0	bburke@redhat.com	META-INF/jpa-changelog-2.3.0.xml	2025-10-23 11:30:47.944925	31	EXECUTED	9:fc155c394040654d6a79227e56f5e25a	createTable tableName=FEDERATED_USER; addPrimaryKey constraintName=CONSTR_FEDERATED_USER, tableName=FEDERATED_USER; dropDefaultValue columnName=TOTP, tableName=USER_ENTITY; dropColumn columnName=TOTP, tableName=USER_ENTITY; addColumn tableName=IDE...		\N	4.29.1	\N	\N	1219031908
2.4.0	bburke@redhat.com	META-INF/jpa-changelog-2.4.0.xml	2025-10-23 11:30:47.974512	32	EXECUTED	9:eac4ffb2a14795e5dc7b426063e54d88	customChange		\N	4.29.1	\N	\N	1219031908
2.5.0	bburke@redhat.com	META-INF/jpa-changelog-2.5.0.xml	2025-10-23 11:30:48.02223	33	EXECUTED	9:54937c05672568c4c64fc9524c1e9462	customChange; modifyDataType columnName=USER_ID, tableName=OFFLINE_USER_SESSION		\N	4.29.1	\N	\N	1219031908
2.5.0-unicode-oracle	hmlnarik@redhat.com	META-INF/jpa-changelog-2.5.0.xml	2025-10-23 11:30:48.041933	34	MARK_RAN	9:3a32bace77c84d7678d035a7f5a8084e	modifyDataType columnName=DESCRIPTION, tableName=AUTHENTICATION_FLOW; modifyDataType columnName=DESCRIPTION, tableName=CLIENT_TEMPLATE; modifyDataType columnName=DESCRIPTION, tableName=RESOURCE_SERVER_POLICY; modifyDataType columnName=DESCRIPTION,...		\N	4.29.1	\N	\N	1219031908
2.5.0-unicode-other-dbs	hmlnarik@redhat.com	META-INF/jpa-changelog-2.5.0.xml	2025-10-23 11:30:48.271619	35	EXECUTED	9:33d72168746f81f98ae3a1e8e0ca3554	modifyDataType columnName=DESCRIPTION, tableName=AUTHENTICATION_FLOW; modifyDataType columnName=DESCRIPTION, tableName=CLIENT_TEMPLATE; modifyDataType columnName=DESCRIPTION, tableName=RESOURCE_SERVER_POLICY; modifyDataType columnName=DESCRIPTION,...		\N	4.29.1	\N	\N	1219031908
2.5.0-duplicate-email-support	slawomir@dabek.name	META-INF/jpa-changelog-2.5.0.xml	2025-10-23 11:30:48.314919	36	EXECUTED	9:61b6d3d7a4c0e0024b0c839da283da0c	addColumn tableName=REALM		\N	4.29.1	\N	\N	1219031908
2.5.0-unique-group-names	hmlnarik@redhat.com	META-INF/jpa-changelog-2.5.0.xml	2025-10-23 11:30:48.364002	37	EXECUTED	9:8dcac7bdf7378e7d823cdfddebf72fda	addUniqueConstraint constraintName=SIBLING_NAMES, tableName=KEYCLOAK_GROUP		\N	4.29.1	\N	\N	1219031908
2.5.1	bburke@redhat.com	META-INF/jpa-changelog-2.5.1.xml	2025-10-23 11:30:48.398704	38	EXECUTED	9:a2b870802540cb3faa72098db5388af3	addColumn tableName=FED_USER_CONSENT		\N	4.29.1	\N	\N	1219031908
3.0.0	bburke@redhat.com	META-INF/jpa-changelog-3.0.0.xml	2025-10-23 11:30:48.427702	39	EXECUTED	9:132a67499ba24bcc54fb5cbdcfe7e4c0	addColumn tableName=IDENTITY_PROVIDER		\N	4.29.1	\N	\N	1219031908
3.2.0-fix	keycloak	META-INF/jpa-changelog-3.2.0.xml	2025-10-23 11:30:48.442753	40	MARK_RAN	9:938f894c032f5430f2b0fafb1a243462	addNotNullConstraint columnName=REALM_ID, tableName=CLIENT_INITIAL_ACCESS		\N	4.29.1	\N	\N	1219031908
3.2.0-fix-with-keycloak-5416	keycloak	META-INF/jpa-changelog-3.2.0.xml	2025-10-23 11:30:48.460921	41	MARK_RAN	9:845c332ff1874dc5d35974b0babf3006	dropIndex indexName=IDX_CLIENT_INIT_ACC_REALM, tableName=CLIENT_INITIAL_ACCESS; addNotNullConstraint columnName=REALM_ID, tableName=CLIENT_INITIAL_ACCESS; createIndex indexName=IDX_CLIENT_INIT_ACC_REALM, tableName=CLIENT_INITIAL_ACCESS		\N	4.29.1	\N	\N	1219031908
3.2.0-fix-offline-sessions	hmlnarik	META-INF/jpa-changelog-3.2.0.xml	2025-10-23 11:30:48.505046	42	EXECUTED	9:fc86359c079781adc577c5a217e4d04c	customChange		\N	4.29.1	\N	\N	1219031908
3.2.0-fixed	keycloak	META-INF/jpa-changelog-3.2.0.xml	2025-10-23 11:31:05.286553	43	EXECUTED	9:59a64800e3c0d09b825f8a3b444fa8f4	addColumn tableName=REALM; dropPrimaryKey constraintName=CONSTRAINT_OFFL_CL_SES_PK2, tableName=OFFLINE_CLIENT_SESSION; dropColumn columnName=CLIENT_SESSION_ID, tableName=OFFLINE_CLIENT_SESSION; addPrimaryKey constraintName=CONSTRAINT_OFFL_CL_SES_P...		\N	4.29.1	\N	\N	1219031908
3.3.0	keycloak	META-INF/jpa-changelog-3.3.0.xml	2025-10-23 11:31:05.315341	44	EXECUTED	9:d48d6da5c6ccf667807f633fe489ce88	addColumn tableName=USER_ENTITY		\N	4.29.1	\N	\N	1219031908
authz-3.4.0.CR1-resource-server-pk-change-part1	glavoie@gmail.com	META-INF/jpa-changelog-authz-3.4.0.CR1.xml	2025-10-23 11:31:05.367664	45	EXECUTED	9:dde36f7973e80d71fceee683bc5d2951	addColumn tableName=RESOURCE_SERVER_POLICY; addColumn tableName=RESOURCE_SERVER_RESOURCE; addColumn tableName=RESOURCE_SERVER_SCOPE		\N	4.29.1	\N	\N	1219031908
authz-3.4.0.CR1-resource-server-pk-change-part2-KEYCLOAK-6095	hmlnarik@redhat.com	META-INF/jpa-changelog-authz-3.4.0.CR1.xml	2025-10-23 11:31:05.401453	46	EXECUTED	9:b855e9b0a406b34fa323235a0cf4f640	customChange		\N	4.29.1	\N	\N	1219031908
authz-3.4.0.CR1-resource-server-pk-change-part3-fixed	glavoie@gmail.com	META-INF/jpa-changelog-authz-3.4.0.CR1.xml	2025-10-23 11:31:05.414071	47	MARK_RAN	9:51abbacd7b416c50c4421a8cabf7927e	dropIndex indexName=IDX_RES_SERV_POL_RES_SERV, tableName=RESOURCE_SERVER_POLICY; dropIndex indexName=IDX_RES_SRV_RES_RES_SRV, tableName=RESOURCE_SERVER_RESOURCE; dropIndex indexName=IDX_RES_SRV_SCOPE_RES_SRV, tableName=RESOURCE_SERVER_SCOPE		\N	4.29.1	\N	\N	1219031908
authz-3.4.0.CR1-resource-server-pk-change-part3-fixed-nodropindex	glavoie@gmail.com	META-INF/jpa-changelog-authz-3.4.0.CR1.xml	2025-10-23 11:31:08.11602	48	EXECUTED	9:bdc99e567b3398bac83263d375aad143	addNotNullConstraint columnName=RESOURCE_SERVER_CLIENT_ID, tableName=RESOURCE_SERVER_POLICY; addNotNullConstraint columnName=RESOURCE_SERVER_CLIENT_ID, tableName=RESOURCE_SERVER_RESOURCE; addNotNullConstraint columnName=RESOURCE_SERVER_CLIENT_ID, ...		\N	4.29.1	\N	\N	1219031908
authn-3.4.0.CR1-refresh-token-max-reuse	glavoie@gmail.com	META-INF/jpa-changelog-authz-3.4.0.CR1.xml	2025-10-23 11:31:08.638347	49	EXECUTED	9:d198654156881c46bfba39abd7769e69	addColumn tableName=REALM		\N	4.29.1	\N	\N	1219031908
3.4.0	keycloak	META-INF/jpa-changelog-3.4.0.xml	2025-10-23 11:31:09.579901	50	EXECUTED	9:cfdd8736332ccdd72c5256ccb42335db	addPrimaryKey constraintName=CONSTRAINT_REALM_DEFAULT_ROLES, tableName=REALM_DEFAULT_ROLES; addPrimaryKey constraintName=CONSTRAINT_COMPOSITE_ROLE, tableName=COMPOSITE_ROLE; addPrimaryKey constraintName=CONSTR_REALM_DEFAULT_GROUPS, tableName=REALM...		\N	4.29.1	\N	\N	1219031908
3.4.0-KEYCLOAK-5230	hmlnarik@redhat.com	META-INF/jpa-changelog-3.4.0.xml	2025-10-23 11:31:28.318115	51	EXECUTED	9:7c84de3d9bd84d7f077607c1a4dcb714	createIndex indexName=IDX_FU_ATTRIBUTE, tableName=FED_USER_ATTRIBUTE; createIndex indexName=IDX_FU_CONSENT, tableName=FED_USER_CONSENT; createIndex indexName=IDX_FU_CONSENT_RU, tableName=FED_USER_CONSENT; createIndex indexName=IDX_FU_CREDENTIAL, t...		\N	4.29.1	\N	\N	1219031908
3.4.1	psilva@redhat.com	META-INF/jpa-changelog-3.4.1.xml	2025-10-23 11:31:28.416826	52	EXECUTED	9:5a6bb36cbefb6a9d6928452c0852af2d	modifyDataType columnName=VALUE, tableName=CLIENT_ATTRIBUTES		\N	4.29.1	\N	\N	1219031908
3.4.2	keycloak	META-INF/jpa-changelog-3.4.2.xml	2025-10-23 11:31:28.551902	53	EXECUTED	9:8f23e334dbc59f82e0a328373ca6ced0	update tableName=REALM		\N	4.29.1	\N	\N	1219031908
3.4.2-KEYCLOAK-5172	mkanis@redhat.com	META-INF/jpa-changelog-3.4.2.xml	2025-10-23 11:31:28.627319	54	EXECUTED	9:9156214268f09d970cdf0e1564d866af	update tableName=CLIENT		\N	4.29.1	\N	\N	1219031908
4.0.0-KEYCLOAK-6335	bburke@redhat.com	META-INF/jpa-changelog-4.0.0.xml	2025-10-23 11:31:28.728733	55	EXECUTED	9:db806613b1ed154826c02610b7dbdf74	createTable tableName=CLIENT_AUTH_FLOW_BINDINGS; addPrimaryKey constraintName=C_CLI_FLOW_BIND, tableName=CLIENT_AUTH_FLOW_BINDINGS		\N	4.29.1	\N	\N	1219031908
4.0.0-CLEANUP-UNUSED-TABLE	bburke@redhat.com	META-INF/jpa-changelog-4.0.0.xml	2025-10-23 11:31:28.794793	56	EXECUTED	9:229a041fb72d5beac76bb94a5fa709de	dropTable tableName=CLIENT_IDENTITY_PROV_MAPPING		\N	4.29.1	\N	\N	1219031908
4.0.0-KEYCLOAK-6228	bburke@redhat.com	META-INF/jpa-changelog-4.0.0.xml	2025-10-23 11:31:30.989257	57	EXECUTED	9:079899dade9c1e683f26b2aa9ca6ff04	dropUniqueConstraint constraintName=UK_JKUWUVD56ONTGSUHOGM8UEWRT, tableName=USER_CONSENT; dropNotNullConstraint columnName=CLIENT_ID, tableName=USER_CONSENT; addColumn tableName=USER_CONSENT; addUniqueConstraint constraintName=UK_JKUWUVD56ONTGSUHO...		\N	4.29.1	\N	\N	1219031908
4.0.0-KEYCLOAK-5579-fixed	mposolda@redhat.com	META-INF/jpa-changelog-4.0.0.xml	2025-10-23 11:31:53.336068	58	EXECUTED	9:139b79bcbbfe903bb1c2d2a4dbf001d9	dropForeignKeyConstraint baseTableName=CLIENT_TEMPLATE_ATTRIBUTES, constraintName=FK_CL_TEMPL_ATTR_TEMPL; renameTable newTableName=CLIENT_SCOPE_ATTRIBUTES, oldTableName=CLIENT_TEMPLATE_ATTRIBUTES; renameColumn newColumnName=SCOPE_ID, oldColumnName...		\N	4.29.1	\N	\N	1219031908
authz-4.0.0.CR1	psilva@redhat.com	META-INF/jpa-changelog-authz-4.0.0.CR1.xml	2025-10-23 11:31:53.816658	59	EXECUTED	9:b55738ad889860c625ba2bf483495a04	createTable tableName=RESOURCE_SERVER_PERM_TICKET; addPrimaryKey constraintName=CONSTRAINT_FAPMT, tableName=RESOURCE_SERVER_PERM_TICKET; addForeignKeyConstraint baseTableName=RESOURCE_SERVER_PERM_TICKET, constraintName=FK_FRSRHO213XCX4WNKOG82SSPMT...		\N	4.29.1	\N	\N	1219031908
authz-4.0.0.Beta3	psilva@redhat.com	META-INF/jpa-changelog-authz-4.0.0.Beta3.xml	2025-10-23 11:31:53.929117	60	EXECUTED	9:e0057eac39aa8fc8e09ac6cfa4ae15fe	addColumn tableName=RESOURCE_SERVER_POLICY; addColumn tableName=RESOURCE_SERVER_PERM_TICKET; addForeignKeyConstraint baseTableName=RESOURCE_SERVER_PERM_TICKET, constraintName=FK_FRSRPO2128CX4WNKOG82SSRFY, referencedTableName=RESOURCE_SERVER_POLICY		\N	4.29.1	\N	\N	1219031908
authz-4.2.0.Final	mhajas@redhat.com	META-INF/jpa-changelog-authz-4.2.0.Final.xml	2025-10-23 11:31:54.071206	61	EXECUTED	9:42a33806f3a0443fe0e7feeec821326c	createTable tableName=RESOURCE_URIS; addForeignKeyConstraint baseTableName=RESOURCE_URIS, constraintName=FK_RESOURCE_SERVER_URIS, referencedTableName=RESOURCE_SERVER_RESOURCE; customChange; dropColumn columnName=URI, tableName=RESOURCE_SERVER_RESO...		\N	4.29.1	\N	\N	1219031908
authz-4.2.0.Final-KEYCLOAK-9944	hmlnarik@redhat.com	META-INF/jpa-changelog-authz-4.2.0.Final.xml	2025-10-23 11:31:54.16579	62	EXECUTED	9:9968206fca46eecc1f51db9c024bfe56	addPrimaryKey constraintName=CONSTRAINT_RESOUR_URIS_PK, tableName=RESOURCE_URIS		\N	4.29.1	\N	\N	1219031908
4.2.0-KEYCLOAK-6313	wadahiro@gmail.com	META-INF/jpa-changelog-4.2.0.xml	2025-10-23 11:31:54.231939	63	EXECUTED	9:92143a6daea0a3f3b8f598c97ce55c3d	addColumn tableName=REQUIRED_ACTION_PROVIDER		\N	4.29.1	\N	\N	1219031908
4.3.0-KEYCLOAK-7984	wadahiro@gmail.com	META-INF/jpa-changelog-4.3.0.xml	2025-10-23 11:31:54.268996	64	EXECUTED	9:82bab26a27195d889fb0429003b18f40	update tableName=REQUIRED_ACTION_PROVIDER		\N	4.29.1	\N	\N	1219031908
4.6.0-KEYCLOAK-7950	psilva@redhat.com	META-INF/jpa-changelog-4.6.0.xml	2025-10-23 11:31:54.311681	65	EXECUTED	9:e590c88ddc0b38b0ae4249bbfcb5abc3	update tableName=RESOURCE_SERVER_RESOURCE		\N	4.29.1	\N	\N	1219031908
4.6.0-KEYCLOAK-8377	keycloak	META-INF/jpa-changelog-4.6.0.xml	2025-10-23 11:31:56.460705	66	EXECUTED	9:5c1f475536118dbdc38d5d7977950cc0	createTable tableName=ROLE_ATTRIBUTE; addPrimaryKey constraintName=CONSTRAINT_ROLE_ATTRIBUTE_PK, tableName=ROLE_ATTRIBUTE; addForeignKeyConstraint baseTableName=ROLE_ATTRIBUTE, constraintName=FK_ROLE_ATTRIBUTE_ID, referencedTableName=KEYCLOAK_ROLE...		\N	4.29.1	\N	\N	1219031908
4.6.0-KEYCLOAK-8555	gideonray@gmail.com	META-INF/jpa-changelog-4.6.0.xml	2025-10-23 11:31:58.006847	67	EXECUTED	9:e7c9f5f9c4d67ccbbcc215440c718a17	createIndex indexName=IDX_COMPONENT_PROVIDER_TYPE, tableName=COMPONENT		\N	4.29.1	\N	\N	1219031908
4.7.0-KEYCLOAK-1267	sguilhen@redhat.com	META-INF/jpa-changelog-4.7.0.xml	2025-10-23 11:31:58.051422	68	EXECUTED	9:88e0bfdda924690d6f4e430c53447dd5	addColumn tableName=REALM		\N	4.29.1	\N	\N	1219031908
4.7.0-KEYCLOAK-7275	keycloak	META-INF/jpa-changelog-4.7.0.xml	2025-10-23 11:31:59.434615	69	EXECUTED	9:f53177f137e1c46b6a88c59ec1cb5218	renameColumn newColumnName=CREATED_ON, oldColumnName=LAST_SESSION_REFRESH, tableName=OFFLINE_USER_SESSION; addNotNullConstraint columnName=CREATED_ON, tableName=OFFLINE_USER_SESSION; addColumn tableName=OFFLINE_USER_SESSION; customChange; createIn...		\N	4.29.1	\N	\N	1219031908
4.8.0-KEYCLOAK-8835	sguilhen@redhat.com	META-INF/jpa-changelog-4.8.0.xml	2025-10-23 11:31:59.528881	70	EXECUTED	9:a74d33da4dc42a37ec27121580d1459f	addNotNullConstraint columnName=SSO_MAX_LIFESPAN_REMEMBER_ME, tableName=REALM; addNotNullConstraint columnName=SSO_IDLE_TIMEOUT_REMEMBER_ME, tableName=REALM		\N	4.29.1	\N	\N	1219031908
authz-7.0.0-KEYCLOAK-10443	psilva@redhat.com	META-INF/jpa-changelog-authz-7.0.0.xml	2025-10-23 11:31:59.594002	71	EXECUTED	9:fd4ade7b90c3b67fae0bfcfcb42dfb5f	addColumn tableName=RESOURCE_SERVER		\N	4.29.1	\N	\N	1219031908
8.0.0-adding-credential-columns	keycloak	META-INF/jpa-changelog-8.0.0.xml	2025-10-23 11:31:59.667231	72	EXECUTED	9:aa072ad090bbba210d8f18781b8cebf4	addColumn tableName=CREDENTIAL; addColumn tableName=FED_USER_CREDENTIAL		\N	4.29.1	\N	\N	1219031908
8.0.0-updating-credential-data-not-oracle-fixed	keycloak	META-INF/jpa-changelog-8.0.0.xml	2025-10-23 11:31:59.853201	73	EXECUTED	9:1ae6be29bab7c2aa376f6983b932be37	update tableName=CREDENTIAL; update tableName=CREDENTIAL; update tableName=CREDENTIAL; update tableName=FED_USER_CREDENTIAL; update tableName=FED_USER_CREDENTIAL; update tableName=FED_USER_CREDENTIAL		\N	4.29.1	\N	\N	1219031908
8.0.0-updating-credential-data-oracle-fixed	keycloak	META-INF/jpa-changelog-8.0.0.xml	2025-10-23 11:31:59.894162	74	MARK_RAN	9:14706f286953fc9a25286dbd8fb30d97	update tableName=CREDENTIAL; update tableName=CREDENTIAL; update tableName=CREDENTIAL; update tableName=FED_USER_CREDENTIAL; update tableName=FED_USER_CREDENTIAL; update tableName=FED_USER_CREDENTIAL		\N	4.29.1	\N	\N	1219031908
8.0.0-credential-cleanup-fixed	keycloak	META-INF/jpa-changelog-8.0.0.xml	2025-10-23 11:32:00.473401	75	EXECUTED	9:2b9cc12779be32c5b40e2e67711a218b	dropDefaultValue columnName=COUNTER, tableName=CREDENTIAL; dropDefaultValue columnName=DIGITS, tableName=CREDENTIAL; dropDefaultValue columnName=PERIOD, tableName=CREDENTIAL; dropDefaultValue columnName=ALGORITHM, tableName=CREDENTIAL; dropColumn ...		\N	4.29.1	\N	\N	1219031908
8.0.0-resource-tag-support	keycloak	META-INF/jpa-changelog-8.0.0.xml	2025-10-23 11:32:01.682686	76	EXECUTED	9:91fa186ce7a5af127a2d7a91ee083cc5	addColumn tableName=MIGRATION_MODEL; createIndex indexName=IDX_UPDATE_TIME, tableName=MIGRATION_MODEL		\N	4.29.1	\N	\N	1219031908
9.0.0-always-display-client	keycloak	META-INF/jpa-changelog-9.0.0.xml	2025-10-23 11:32:01.742253	77	EXECUTED	9:6335e5c94e83a2639ccd68dd24e2e5ad	addColumn tableName=CLIENT		\N	4.29.1	\N	\N	1219031908
9.0.0-drop-constraints-for-column-increase	keycloak	META-INF/jpa-changelog-9.0.0.xml	2025-10-23 11:32:01.757382	78	MARK_RAN	9:6bdb5658951e028bfe16fa0a8228b530	dropUniqueConstraint constraintName=UK_FRSR6T700S9V50BU18WS5PMT, tableName=RESOURCE_SERVER_PERM_TICKET; dropUniqueConstraint constraintName=UK_FRSR6T700S9V50BU18WS5HA6, tableName=RESOURCE_SERVER_RESOURCE; dropPrimaryKey constraintName=CONSTRAINT_O...		\N	4.29.1	\N	\N	1219031908
9.0.0-increase-column-size-federated-fk	keycloak	META-INF/jpa-changelog-9.0.0.xml	2025-10-23 11:32:02.626115	79	EXECUTED	9:d5bc15a64117ccad481ce8792d4c608f	modifyDataType columnName=CLIENT_ID, tableName=FED_USER_CONSENT; modifyDataType columnName=CLIENT_REALM_CONSTRAINT, tableName=KEYCLOAK_ROLE; modifyDataType columnName=OWNER, tableName=RESOURCE_SERVER_POLICY; modifyDataType columnName=CLIENT_ID, ta...		\N	4.29.1	\N	\N	1219031908
9.0.0-recreate-constraints-after-column-increase	keycloak	META-INF/jpa-changelog-9.0.0.xml	2025-10-23 11:32:02.657352	80	MARK_RAN	9:077cba51999515f4d3e7ad5619ab592c	addNotNullConstraint columnName=CLIENT_ID, tableName=OFFLINE_CLIENT_SESSION; addNotNullConstraint columnName=OWNER, tableName=RESOURCE_SERVER_PERM_TICKET; addNotNullConstraint columnName=REQUESTER, tableName=RESOURCE_SERVER_PERM_TICKET; addNotNull...		\N	4.29.1	\N	\N	1219031908
9.0.1-add-index-to-client.client_id	keycloak	META-INF/jpa-changelog-9.0.1.xml	2025-10-23 11:32:04.35819	81	EXECUTED	9:be969f08a163bf47c6b9e9ead8ac2afb	createIndex indexName=IDX_CLIENT_ID, tableName=CLIENT		\N	4.29.1	\N	\N	1219031908
9.0.1-KEYCLOAK-12579-drop-constraints	keycloak	META-INF/jpa-changelog-9.0.1.xml	2025-10-23 11:32:04.382146	82	MARK_RAN	9:6d3bb4408ba5a72f39bd8a0b301ec6e3	dropUniqueConstraint constraintName=SIBLING_NAMES, tableName=KEYCLOAK_GROUP		\N	4.29.1	\N	\N	1219031908
9.0.1-KEYCLOAK-12579-add-not-null-constraint	keycloak	META-INF/jpa-changelog-9.0.1.xml	2025-10-23 11:32:04.458385	83	EXECUTED	9:966bda61e46bebf3cc39518fbed52fa7	addNotNullConstraint columnName=PARENT_GROUP, tableName=KEYCLOAK_GROUP		\N	4.29.1	\N	\N	1219031908
9.0.1-KEYCLOAK-12579-recreate-constraints	keycloak	META-INF/jpa-changelog-9.0.1.xml	2025-10-23 11:32:04.479535	84	MARK_RAN	9:8dcac7bdf7378e7d823cdfddebf72fda	addUniqueConstraint constraintName=SIBLING_NAMES, tableName=KEYCLOAK_GROUP		\N	4.29.1	\N	\N	1219031908
9.0.1-add-index-to-events	keycloak	META-INF/jpa-changelog-9.0.1.xml	2025-10-23 11:32:05.331143	85	EXECUTED	9:7d93d602352a30c0c317e6a609b56599	createIndex indexName=IDX_EVENT_TIME, tableName=EVENT_ENTITY		\N	4.29.1	\N	\N	1219031908
map-remove-ri	keycloak	META-INF/jpa-changelog-11.0.0.xml	2025-10-23 11:32:05.459249	86	EXECUTED	9:71c5969e6cdd8d7b6f47cebc86d37627	dropForeignKeyConstraint baseTableName=REALM, constraintName=FK_TRAF444KK6QRKMS7N56AIWQ5Y; dropForeignKeyConstraint baseTableName=KEYCLOAK_ROLE, constraintName=FK_KJHO5LE2C0RAL09FL8CM9WFW9		\N	4.29.1	\N	\N	1219031908
map-remove-ri	keycloak	META-INF/jpa-changelog-12.0.0.xml	2025-10-23 11:32:05.640692	87	EXECUTED	9:a9ba7d47f065f041b7da856a81762021	dropForeignKeyConstraint baseTableName=REALM_DEFAULT_GROUPS, constraintName=FK_DEF_GROUPS_GROUP; dropForeignKeyConstraint baseTableName=REALM_DEFAULT_ROLES, constraintName=FK_H4WPD7W4HSOOLNI3H0SW7BTJE; dropForeignKeyConstraint baseTableName=CLIENT...		\N	4.29.1	\N	\N	1219031908
12.1.0-add-realm-localization-table	keycloak	META-INF/jpa-changelog-12.0.0.xml	2025-10-23 11:32:05.835077	88	EXECUTED	9:fffabce2bc01e1a8f5110d5278500065	createTable tableName=REALM_LOCALIZATIONS; addPrimaryKey tableName=REALM_LOCALIZATIONS		\N	4.29.1	\N	\N	1219031908
default-roles	keycloak	META-INF/jpa-changelog-13.0.0.xml	2025-10-23 11:32:06.007911	89	EXECUTED	9:fa8a5b5445e3857f4b010bafb5009957	addColumn tableName=REALM; customChange		\N	4.29.1	\N	\N	1219031908
default-roles-cleanup	keycloak	META-INF/jpa-changelog-13.0.0.xml	2025-10-23 11:32:06.121712	90	EXECUTED	9:67ac3241df9a8582d591c5ed87125f39	dropTable tableName=REALM_DEFAULT_ROLES; dropTable tableName=CLIENT_DEFAULT_ROLES		\N	4.29.1	\N	\N	1219031908
13.0.0-KEYCLOAK-16844	keycloak	META-INF/jpa-changelog-13.0.0.xml	2025-10-23 11:32:07.161346	91	EXECUTED	9:ad1194d66c937e3ffc82386c050ba089	createIndex indexName=IDX_OFFLINE_USS_PRELOAD, tableName=OFFLINE_USER_SESSION		\N	4.29.1	\N	\N	1219031908
map-remove-ri-13.0.0	keycloak	META-INF/jpa-changelog-13.0.0.xml	2025-10-23 11:32:07.220496	92	EXECUTED	9:d9be619d94af5a2f5d07b9f003543b91	dropForeignKeyConstraint baseTableName=DEFAULT_CLIENT_SCOPE, constraintName=FK_R_DEF_CLI_SCOPE_SCOPE; dropForeignKeyConstraint baseTableName=CLIENT_SCOPE_CLIENT, constraintName=FK_C_CLI_SCOPE_SCOPE; dropForeignKeyConstraint baseTableName=CLIENT_SC...		\N	4.29.1	\N	\N	1219031908
13.0.0-KEYCLOAK-17992-drop-constraints	keycloak	META-INF/jpa-changelog-13.0.0.xml	2025-10-23 11:32:07.239897	93	MARK_RAN	9:544d201116a0fcc5a5da0925fbbc3bde	dropPrimaryKey constraintName=C_CLI_SCOPE_BIND, tableName=CLIENT_SCOPE_CLIENT; dropIndex indexName=IDX_CLSCOPE_CL, tableName=CLIENT_SCOPE_CLIENT; dropIndex indexName=IDX_CL_CLSCOPE, tableName=CLIENT_SCOPE_CLIENT		\N	4.29.1	\N	\N	1219031908
13.0.0-increase-column-size-federated	keycloak	META-INF/jpa-changelog-13.0.0.xml	2025-10-23 11:32:07.316512	94	EXECUTED	9:43c0c1055b6761b4b3e89de76d612ccf	modifyDataType columnName=CLIENT_ID, tableName=CLIENT_SCOPE_CLIENT; modifyDataType columnName=SCOPE_ID, tableName=CLIENT_SCOPE_CLIENT		\N	4.29.1	\N	\N	1219031908
13.0.0-KEYCLOAK-17992-recreate-constraints	keycloak	META-INF/jpa-changelog-13.0.0.xml	2025-10-23 11:32:07.336926	95	MARK_RAN	9:8bd711fd0330f4fe980494ca43ab1139	addNotNullConstraint columnName=CLIENT_ID, tableName=CLIENT_SCOPE_CLIENT; addNotNullConstraint columnName=SCOPE_ID, tableName=CLIENT_SCOPE_CLIENT; addPrimaryKey constraintName=C_CLI_SCOPE_BIND, tableName=CLIENT_SCOPE_CLIENT; createIndex indexName=...		\N	4.29.1	\N	\N	1219031908
json-string-accomodation-fixed	keycloak	META-INF/jpa-changelog-13.0.0.xml	2025-10-23 11:32:07.415999	96	EXECUTED	9:e07d2bc0970c348bb06fb63b1f82ddbf	addColumn tableName=REALM_ATTRIBUTE; update tableName=REALM_ATTRIBUTE; dropColumn columnName=VALUE, tableName=REALM_ATTRIBUTE; renameColumn newColumnName=VALUE, oldColumnName=VALUE_NEW, tableName=REALM_ATTRIBUTE		\N	4.29.1	\N	\N	1219031908
14.0.0-KEYCLOAK-11019	keycloak	META-INF/jpa-changelog-14.0.0.xml	2025-10-23 11:32:11.451226	97	EXECUTED	9:24fb8611e97f29989bea412aa38d12b7	createIndex indexName=IDX_OFFLINE_CSS_PRELOAD, tableName=OFFLINE_CLIENT_SESSION; createIndex indexName=IDX_OFFLINE_USS_BY_USER, tableName=OFFLINE_USER_SESSION; createIndex indexName=IDX_OFFLINE_USS_BY_USERSESS, tableName=OFFLINE_USER_SESSION		\N	4.29.1	\N	\N	1219031908
14.0.0-KEYCLOAK-18286	keycloak	META-INF/jpa-changelog-14.0.0.xml	2025-10-23 11:32:11.470327	98	MARK_RAN	9:259f89014ce2506ee84740cbf7163aa7	createIndex indexName=IDX_CLIENT_ATT_BY_NAME_VALUE, tableName=CLIENT_ATTRIBUTES		\N	4.29.1	\N	\N	1219031908
14.0.0-KEYCLOAK-18286-revert	keycloak	META-INF/jpa-changelog-14.0.0.xml	2025-10-23 11:32:11.660272	99	MARK_RAN	9:04baaf56c116ed19951cbc2cca584022	dropIndex indexName=IDX_CLIENT_ATT_BY_NAME_VALUE, tableName=CLIENT_ATTRIBUTES		\N	4.29.1	\N	\N	1219031908
14.0.0-KEYCLOAK-18286-supported-dbs	keycloak	META-INF/jpa-changelog-14.0.0.xml	2025-10-23 11:32:13.36457	100	EXECUTED	9:60ca84a0f8c94ec8c3504a5a3bc88ee8	createIndex indexName=IDX_CLIENT_ATT_BY_NAME_VALUE, tableName=CLIENT_ATTRIBUTES		\N	4.29.1	\N	\N	1219031908
14.0.0-KEYCLOAK-18286-unsupported-dbs	keycloak	META-INF/jpa-changelog-14.0.0.xml	2025-10-23 11:32:13.382535	101	MARK_RAN	9:d3d977031d431db16e2c181ce49d73e9	createIndex indexName=IDX_CLIENT_ATT_BY_NAME_VALUE, tableName=CLIENT_ATTRIBUTES		\N	4.29.1	\N	\N	1219031908
KEYCLOAK-17267-add-index-to-user-attributes	keycloak	META-INF/jpa-changelog-14.0.0.xml	2025-10-23 11:32:15.817001	102	EXECUTED	9:0b305d8d1277f3a89a0a53a659ad274c	createIndex indexName=IDX_USER_ATTRIBUTE_NAME, tableName=USER_ATTRIBUTE		\N	4.29.1	\N	\N	1219031908
KEYCLOAK-18146-add-saml-art-binding-identifier	keycloak	META-INF/jpa-changelog-14.0.0.xml	2025-10-23 11:32:15.856439	103	EXECUTED	9:2c374ad2cdfe20e2905a84c8fac48460	customChange		\N	4.29.1	\N	\N	1219031908
15.0.0-KEYCLOAK-18467	keycloak	META-INF/jpa-changelog-15.0.0.xml	2025-10-23 11:32:15.947279	104	EXECUTED	9:47a760639ac597360a8219f5b768b4de	addColumn tableName=REALM_LOCALIZATIONS; update tableName=REALM_LOCALIZATIONS; dropColumn columnName=TEXTS, tableName=REALM_LOCALIZATIONS; renameColumn newColumnName=TEXTS, oldColumnName=TEXTS_NEW, tableName=REALM_LOCALIZATIONS; addNotNullConstrai...		\N	4.29.1	\N	\N	1219031908
17.0.0-9562	keycloak	META-INF/jpa-changelog-17.0.0.xml	2025-10-23 11:32:17.200224	105	EXECUTED	9:a6272f0576727dd8cad2522335f5d99e	createIndex indexName=IDX_USER_SERVICE_ACCOUNT, tableName=USER_ENTITY		\N	4.29.1	\N	\N	1219031908
18.0.0-10625-IDX_ADMIN_EVENT_TIME	keycloak	META-INF/jpa-changelog-18.0.0.xml	2025-10-23 11:32:23.953306	106	EXECUTED	9:015479dbd691d9cc8669282f4828c41d	createIndex indexName=IDX_ADMIN_EVENT_TIME, tableName=ADMIN_EVENT_ENTITY		\N	4.29.1	\N	\N	1219031908
18.0.15-30992-index-consent	keycloak	META-INF/jpa-changelog-18.0.15.xml	2025-10-23 11:32:28.304911	107	EXECUTED	9:80071ede7a05604b1f4906f3bf3b00f0	createIndex indexName=IDX_USCONSENT_SCOPE_ID, tableName=USER_CONSENT_CLIENT_SCOPE		\N	4.29.1	\N	\N	1219031908
19.0.0-10135	keycloak	META-INF/jpa-changelog-19.0.0.xml	2025-10-23 11:32:28.387068	108	EXECUTED	9:9518e495fdd22f78ad6425cc30630221	customChange		\N	4.29.1	\N	\N	1219031908
20.0.0-12964-supported-dbs	keycloak	META-INF/jpa-changelog-20.0.0.xml	2025-10-23 11:32:30.249809	109	EXECUTED	9:e5f243877199fd96bcc842f27a1656ac	createIndex indexName=IDX_GROUP_ATT_BY_NAME_VALUE, tableName=GROUP_ATTRIBUTE		\N	4.29.1	\N	\N	1219031908
20.0.0-12964-unsupported-dbs	keycloak	META-INF/jpa-changelog-20.0.0.xml	2025-10-23 11:32:30.268619	110	MARK_RAN	9:1a6fcaa85e20bdeae0a9ce49b41946a5	createIndex indexName=IDX_GROUP_ATT_BY_NAME_VALUE, tableName=GROUP_ATTRIBUTE		\N	4.29.1	\N	\N	1219031908
client-attributes-string-accomodation-fixed	keycloak	META-INF/jpa-changelog-20.0.0.xml	2025-10-23 11:32:30.480426	111	EXECUTED	9:3f332e13e90739ed0c35b0b25b7822ca	addColumn tableName=CLIENT_ATTRIBUTES; update tableName=CLIENT_ATTRIBUTES; dropColumn columnName=VALUE, tableName=CLIENT_ATTRIBUTES; renameColumn newColumnName=VALUE, oldColumnName=VALUE_NEW, tableName=CLIENT_ATTRIBUTES		\N	4.29.1	\N	\N	1219031908
21.0.2-17277	keycloak	META-INF/jpa-changelog-21.0.2.xml	2025-10-23 11:32:30.517961	112	EXECUTED	9:7ee1f7a3fb8f5588f171fb9a6ab623c0	customChange		\N	4.29.1	\N	\N	1219031908
21.1.0-19404	keycloak	META-INF/jpa-changelog-21.1.0.xml	2025-10-23 11:32:31.119895	113	EXECUTED	9:3d7e830b52f33676b9d64f7f2b2ea634	modifyDataType columnName=DECISION_STRATEGY, tableName=RESOURCE_SERVER_POLICY; modifyDataType columnName=LOGIC, tableName=RESOURCE_SERVER_POLICY; modifyDataType columnName=POLICY_ENFORCE_MODE, tableName=RESOURCE_SERVER		\N	4.29.1	\N	\N	1219031908
21.1.0-19404-2	keycloak	META-INF/jpa-changelog-21.1.0.xml	2025-10-23 11:32:31.148888	114	MARK_RAN	9:627d032e3ef2c06c0e1f73d2ae25c26c	addColumn tableName=RESOURCE_SERVER_POLICY; update tableName=RESOURCE_SERVER_POLICY; dropColumn columnName=DECISION_STRATEGY, tableName=RESOURCE_SERVER_POLICY; renameColumn newColumnName=DECISION_STRATEGY, oldColumnName=DECISION_STRATEGY_NEW, tabl...		\N	4.29.1	\N	\N	1219031908
22.0.0-17484-updated	keycloak	META-INF/jpa-changelog-22.0.0.xml	2025-10-23 11:32:31.260885	115	EXECUTED	9:90af0bfd30cafc17b9f4d6eccd92b8b3	customChange		\N	4.29.1	\N	\N	1219031908
22.0.5-24031	keycloak	META-INF/jpa-changelog-22.0.0.xml	2025-10-23 11:32:31.281279	116	MARK_RAN	9:a60d2d7b315ec2d3eba9e2f145f9df28	customChange		\N	4.29.1	\N	\N	1219031908
23.0.0-12062	keycloak	META-INF/jpa-changelog-23.0.0.xml	2025-10-23 11:32:31.446362	117	EXECUTED	9:2168fbe728fec46ae9baf15bf80927b8	addColumn tableName=COMPONENT_CONFIG; update tableName=COMPONENT_CONFIG; dropColumn columnName=VALUE, tableName=COMPONENT_CONFIG; renameColumn newColumnName=VALUE, oldColumnName=VALUE_NEW, tableName=COMPONENT_CONFIG		\N	4.29.1	\N	\N	1219031908
23.0.0-17258	keycloak	META-INF/jpa-changelog-23.0.0.xml	2025-10-23 11:32:31.519624	118	EXECUTED	9:36506d679a83bbfda85a27ea1864dca8	addColumn tableName=EVENT_ENTITY		\N	4.29.1	\N	\N	1219031908
24.0.0-9758	keycloak	META-INF/jpa-changelog-24.0.0.xml	2025-10-23 11:32:43.074971	119	EXECUTED	9:502c557a5189f600f0f445a9b49ebbce	addColumn tableName=USER_ATTRIBUTE; addColumn tableName=FED_USER_ATTRIBUTE; createIndex indexName=USER_ATTR_LONG_VALUES, tableName=USER_ATTRIBUTE; createIndex indexName=FED_USER_ATTR_LONG_VALUES, tableName=FED_USER_ATTRIBUTE; createIndex indexName...		\N	4.29.1	\N	\N	1219031908
24.0.0-9758-2	keycloak	META-INF/jpa-changelog-24.0.0.xml	2025-10-23 11:32:43.102225	120	EXECUTED	9:bf0fdee10afdf597a987adbf291db7b2	customChange		\N	4.29.1	\N	\N	1219031908
24.0.0-26618-drop-index-if-present	keycloak	META-INF/jpa-changelog-24.0.0.xml	2025-10-23 11:32:43.156195	121	MARK_RAN	9:04baaf56c116ed19951cbc2cca584022	dropIndex indexName=IDX_CLIENT_ATT_BY_NAME_VALUE, tableName=CLIENT_ATTRIBUTES		\N	4.29.1	\N	\N	1219031908
24.0.0-26618-reindex	keycloak	META-INF/jpa-changelog-24.0.0.xml	2025-10-23 11:32:44.097656	122	EXECUTED	9:08707c0f0db1cef6b352db03a60edc7f	createIndex indexName=IDX_CLIENT_ATT_BY_NAME_VALUE, tableName=CLIENT_ATTRIBUTES		\N	4.29.1	\N	\N	1219031908
24.0.2-27228	keycloak	META-INF/jpa-changelog-24.0.2.xml	2025-10-23 11:32:44.15117	123	EXECUTED	9:eaee11f6b8aa25d2cc6a84fb86fc6238	customChange		\N	4.29.1	\N	\N	1219031908
24.0.2-27967-drop-index-if-present	keycloak	META-INF/jpa-changelog-24.0.2.xml	2025-10-23 11:32:44.165303	124	MARK_RAN	9:04baaf56c116ed19951cbc2cca584022	dropIndex indexName=IDX_CLIENT_ATT_BY_NAME_VALUE, tableName=CLIENT_ATTRIBUTES		\N	4.29.1	\N	\N	1219031908
24.0.2-27967-reindex	keycloak	META-INF/jpa-changelog-24.0.2.xml	2025-10-23 11:32:44.221573	125	MARK_RAN	9:d3d977031d431db16e2c181ce49d73e9	createIndex indexName=IDX_CLIENT_ATT_BY_NAME_VALUE, tableName=CLIENT_ATTRIBUTES		\N	4.29.1	\N	\N	1219031908
25.0.0-28265-tables	keycloak	META-INF/jpa-changelog-25.0.0.xml	2025-10-23 11:32:44.296818	126	EXECUTED	9:deda2df035df23388af95bbd36c17cef	addColumn tableName=OFFLINE_USER_SESSION; addColumn tableName=OFFLINE_CLIENT_SESSION		\N	4.29.1	\N	\N	1219031908
25.0.0-28265-index-creation	keycloak	META-INF/jpa-changelog-25.0.0.xml	2025-10-23 11:32:47.987265	127	EXECUTED	9:3e96709818458ae49f3c679ae58d263a	createIndex indexName=IDX_OFFLINE_USS_BY_LAST_SESSION_REFRESH, tableName=OFFLINE_USER_SESSION		\N	4.29.1	\N	\N	1219031908
25.0.0-28265-index-cleanup-uss-createdon	keycloak	META-INF/jpa-changelog-25.0.0.xml	2025-10-23 11:32:49.207393	128	EXECUTED	9:78ab4fc129ed5e8265dbcc3485fba92f	dropIndex indexName=IDX_OFFLINE_USS_CREATEDON, tableName=OFFLINE_USER_SESSION		\N	4.29.1	\N	\N	1219031908
25.0.0-28265-index-cleanup-uss-preload	keycloak	META-INF/jpa-changelog-25.0.0.xml	2025-10-23 11:32:50.058923	129	EXECUTED	9:de5f7c1f7e10994ed8b62e621d20eaab	dropIndex indexName=IDX_OFFLINE_USS_PRELOAD, tableName=OFFLINE_USER_SESSION		\N	4.29.1	\N	\N	1219031908
25.0.0-28265-index-cleanup-uss-by-usersess	keycloak	META-INF/jpa-changelog-25.0.0.xml	2025-10-23 11:32:56.957836	130	EXECUTED	9:6eee220d024e38e89c799417ec33667f	dropIndex indexName=IDX_OFFLINE_USS_BY_USERSESS, tableName=OFFLINE_USER_SESSION		\N	4.29.1	\N	\N	1219031908
25.0.0-28265-index-cleanup-css-preload	keycloak	META-INF/jpa-changelog-25.0.0.xml	2025-10-23 11:32:59.734419	131	EXECUTED	9:5411d2fb2891d3e8d63ddb55dfa3c0c9	dropIndex indexName=IDX_OFFLINE_CSS_PRELOAD, tableName=OFFLINE_CLIENT_SESSION		\N	4.29.1	\N	\N	1219031908
25.0.0-28265-index-2-mysql	keycloak	META-INF/jpa-changelog-25.0.0.xml	2025-10-23 11:32:59.831367	132	MARK_RAN	9:b7ef76036d3126bb83c2423bf4d449d6	createIndex indexName=IDX_OFFLINE_USS_BY_BROKER_SESSION_ID, tableName=OFFLINE_USER_SESSION		\N	4.29.1	\N	\N	1219031908
25.0.0-28265-index-2-not-mysql	keycloak	META-INF/jpa-changelog-25.0.0.xml	2025-10-23 11:33:02.195823	133	EXECUTED	9:23396cf51ab8bc1ae6f0cac7f9f6fcf7	createIndex indexName=IDX_OFFLINE_USS_BY_BROKER_SESSION_ID, tableName=OFFLINE_USER_SESSION		\N	4.29.1	\N	\N	1219031908
25.0.0-org	keycloak	META-INF/jpa-changelog-25.0.0.xml	2025-10-23 11:33:03.104506	134	EXECUTED	9:5c859965c2c9b9c72136c360649af157	createTable tableName=ORG; addUniqueConstraint constraintName=UK_ORG_NAME, tableName=ORG; addUniqueConstraint constraintName=UK_ORG_GROUP, tableName=ORG; createTable tableName=ORG_DOMAIN		\N	4.29.1	\N	\N	1219031908
unique-consentuser	keycloak	META-INF/jpa-changelog-25.0.0.xml	2025-10-23 11:33:03.257627	135	EXECUTED	9:5857626a2ea8767e9a6c66bf3a2cb32f	customChange; dropUniqueConstraint constraintName=UK_JKUWUVD56ONTGSUHOGM8UEWRT, tableName=USER_CONSENT; addUniqueConstraint constraintName=UK_LOCAL_CONSENT, tableName=USER_CONSENT; addUniqueConstraint constraintName=UK_EXTERNAL_CONSENT, tableName=...		\N	4.29.1	\N	\N	1219031908
unique-consentuser-mysql	keycloak	META-INF/jpa-changelog-25.0.0.xml	2025-10-23 11:33:03.291529	136	MARK_RAN	9:b79478aad5adaa1bc428e31563f55e8e	customChange; dropUniqueConstraint constraintName=UK_JKUWUVD56ONTGSUHOGM8UEWRT, tableName=USER_CONSENT; addUniqueConstraint constraintName=UK_LOCAL_CONSENT, tableName=USER_CONSENT; addUniqueConstraint constraintName=UK_EXTERNAL_CONSENT, tableName=...		\N	4.29.1	\N	\N	1219031908
25.0.0-28861-index-creation	keycloak	META-INF/jpa-changelog-25.0.0.xml	2025-10-23 11:33:15.351581	137	EXECUTED	9:b9acb58ac958d9ada0fe12a5d4794ab1	createIndex indexName=IDX_PERM_TICKET_REQUESTER, tableName=RESOURCE_SERVER_PERM_TICKET; createIndex indexName=IDX_PERM_TICKET_OWNER, tableName=RESOURCE_SERVER_PERM_TICKET		\N	4.29.1	\N	\N	1219031908
26.0.0-org-alias	keycloak	META-INF/jpa-changelog-26.0.0.xml	2025-10-23 11:33:15.478936	138	EXECUTED	9:6ef7d63e4412b3c2d66ed179159886a4	addColumn tableName=ORG; update tableName=ORG; addNotNullConstraint columnName=ALIAS, tableName=ORG; addUniqueConstraint constraintName=UK_ORG_ALIAS, tableName=ORG		\N	4.29.1	\N	\N	1219031908
26.0.0-org-group	keycloak	META-INF/jpa-changelog-26.0.0.xml	2025-10-23 11:33:15.538123	139	EXECUTED	9:da8e8087d80ef2ace4f89d8c5b9ca223	addColumn tableName=KEYCLOAK_GROUP; update tableName=KEYCLOAK_GROUP; addNotNullConstraint columnName=TYPE, tableName=KEYCLOAK_GROUP; customChange		\N	4.29.1	\N	\N	1219031908
26.0.0-org-indexes	keycloak	META-INF/jpa-changelog-26.0.0.xml	2025-10-23 11:33:17.164517	140	EXECUTED	9:79b05dcd610a8c7f25ec05135eec0857	createIndex indexName=IDX_ORG_DOMAIN_ORG_ID, tableName=ORG_DOMAIN		\N	4.29.1	\N	\N	1219031908
26.0.0-org-group-membership	keycloak	META-INF/jpa-changelog-26.0.0.xml	2025-10-23 11:33:17.266368	141	EXECUTED	9:a6ace2ce583a421d89b01ba2a28dc2d4	addColumn tableName=USER_GROUP_MEMBERSHIP; update tableName=USER_GROUP_MEMBERSHIP; addNotNullConstraint columnName=MEMBERSHIP_TYPE, tableName=USER_GROUP_MEMBERSHIP		\N	4.29.1	\N	\N	1219031908
31296-persist-revoked-access-tokens	keycloak	META-INF/jpa-changelog-26.0.0.xml	2025-10-23 11:33:17.386286	142	EXECUTED	9:64ef94489d42a358e8304b0e245f0ed4	createTable tableName=REVOKED_TOKEN; addPrimaryKey constraintName=CONSTRAINT_RT, tableName=REVOKED_TOKEN		\N	4.29.1	\N	\N	1219031908
31725-index-persist-revoked-access-tokens	keycloak	META-INF/jpa-changelog-26.0.0.xml	2025-10-23 11:33:18.611159	143	EXECUTED	9:b994246ec2bf7c94da881e1d28782c7b	createIndex indexName=IDX_REV_TOKEN_ON_EXPIRE, tableName=REVOKED_TOKEN		\N	4.29.1	\N	\N	1219031908
26.0.0-idps-for-login	keycloak	META-INF/jpa-changelog-26.0.0.xml	2025-10-23 11:33:23.911322	144	EXECUTED	9:51f5fffadf986983d4bd59582c6c1604	addColumn tableName=IDENTITY_PROVIDER; createIndex indexName=IDX_IDP_REALM_ORG, tableName=IDENTITY_PROVIDER; createIndex indexName=IDX_IDP_FOR_LOGIN, tableName=IDENTITY_PROVIDER; customChange		\N	4.29.1	\N	\N	1219031908
26.0.0-32583-drop-redundant-index-on-client-session	keycloak	META-INF/jpa-changelog-26.0.0.xml	2025-10-23 11:33:24.768992	145	EXECUTED	9:24972d83bf27317a055d234187bb4af9	dropIndex indexName=IDX_US_SESS_ID_ON_CL_SESS, tableName=OFFLINE_CLIENT_SESSION		\N	4.29.1	\N	\N	1219031908
26.0.0.32582-remove-tables-user-session-user-session-note-and-client-session	keycloak	META-INF/jpa-changelog-26.0.0.xml	2025-10-23 11:33:25.029594	146	EXECUTED	9:febdc0f47f2ed241c59e60f58c3ceea5	dropTable tableName=CLIENT_SESSION_ROLE; dropTable tableName=CLIENT_SESSION_NOTE; dropTable tableName=CLIENT_SESSION_PROT_MAPPER; dropTable tableName=CLIENT_SESSION_AUTH_STATUS; dropTable tableName=CLIENT_USER_SESSION_NOTE; dropTable tableName=CLI...		\N	4.29.1	\N	\N	1219031908
26.0.0-33201-org-redirect-url	keycloak	META-INF/jpa-changelog-26.0.0.xml	2025-10-23 11:33:25.10291	147	EXECUTED	9:4d0e22b0ac68ebe9794fa9cb752ea660	addColumn tableName=ORG		\N	4.29.1	\N	\N	1219031908
29399-jdbc-ping-default	keycloak	META-INF/jpa-changelog-26.1.0.xml	2025-10-23 11:33:25.234552	148	EXECUTED	9:007dbe99d7203fca403b89d4edfdf21e	createTable tableName=JGROUPS_PING; addPrimaryKey constraintName=CONSTRAINT_JGROUPS_PING, tableName=JGROUPS_PING		\N	4.29.1	\N	\N	1219031908
26.1.0-34013	keycloak	META-INF/jpa-changelog-26.1.0.xml	2025-10-23 11:33:25.343003	149	EXECUTED	9:e6b686a15759aef99a6d758a5c4c6a26	addColumn tableName=ADMIN_EVENT_ENTITY		\N	4.29.1	\N	\N	1219031908
26.1.0-34380	keycloak	META-INF/jpa-changelog-26.1.0.xml	2025-10-23 11:33:25.384586	150	EXECUTED	9:ac8b9edb7c2b6c17a1c7a11fcf5ccf01	dropTable tableName=USERNAME_LOGIN_FAILURE		\N	4.29.1	\N	\N	1219031908
\.


--
-- TOC entry 4132 (class 0 OID 24577)
-- Dependencies: 215
-- Data for Name: databasechangeloglock; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.databasechangeloglock (id, locked, lockgranted, lockedby) FROM stdin;
1	f	\N	\N
1000	f	\N	\N
\.


--
-- TOC entry 4207 (class 0 OID 25974)
-- Dependencies: 290
-- Data for Name: default_client_scope; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.default_client_scope (realm_id, scope_id, default_scope) FROM stdin;
9ccc2b44-8d11-4694-87e4-8e194b225e1d	ed2dd60a-b152-426b-bb35-923d5bcaacd5	f
9ccc2b44-8d11-4694-87e4-8e194b225e1d	3447361c-98b8-4146-a2a8-d103e31a382e	t
9ccc2b44-8d11-4694-87e4-8e194b225e1d	2354c9d6-9230-4ef8-9818-d56485529aae	t
9ccc2b44-8d11-4694-87e4-8e194b225e1d	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685	t
9ccc2b44-8d11-4694-87e4-8e194b225e1d	9477d6cb-3396-463f-807f-e7e05c77502b	t
9ccc2b44-8d11-4694-87e4-8e194b225e1d	43597b3d-8978-4057-9084-9d511fd8e58e	f
9ccc2b44-8d11-4694-87e4-8e194b225e1d	11daa3f1-9d5b-41b7-b68b-1d3d9f9124ed	f
9ccc2b44-8d11-4694-87e4-8e194b225e1d	8e148a79-c784-465a-9c32-b340e03f5c0c	t
9ccc2b44-8d11-4694-87e4-8e194b225e1d	142e3661-dcf9-43be-a77d-278835a44ef7	t
9ccc2b44-8d11-4694-87e4-8e194b225e1d	bc717a1c-2345-40e5-9d27-01579b8b2852	f
9ccc2b44-8d11-4694-87e4-8e194b225e1d	e9143869-1a5f-4f96-a55c-0e2b2d63b30b	t
9ccc2b44-8d11-4694-87e4-8e194b225e1d	0486e99d-cae3-472e-98c6-e1da9bc9563a	t
9ccc2b44-8d11-4694-87e4-8e194b225e1d	f646375e-1c13-4dd9-a732-e328bf9f1a4d	f
22478864-d5ad-4829-b2bf-92ed9d9ca9c8	a649a5b9-653d-4d5f-bdcf-f1659c51a313	f
22478864-d5ad-4829-b2bf-92ed9d9ca9c8	6f8b807e-fe47-446c-b699-a352acbd54f0	t
22478864-d5ad-4829-b2bf-92ed9d9ca9c8	d705b59a-33c0-42c0-8e04-8684529ed9c0	t
22478864-d5ad-4829-b2bf-92ed9d9ca9c8	d33cf0c5-5f0c-45ae-8103-1fabd28670df	t
22478864-d5ad-4829-b2bf-92ed9d9ca9c8	8a3526d2-4888-4029-ad16-185e7e0cf9c0	t
22478864-d5ad-4829-b2bf-92ed9d9ca9c8	319da478-2b63-4964-b514-a9109e256736	f
22478864-d5ad-4829-b2bf-92ed9d9ca9c8	27b6c674-5fc7-402d-812a-43c50fbf5eb9	f
22478864-d5ad-4829-b2bf-92ed9d9ca9c8	0a460cd8-7178-4845-b532-c84568f3be21	t
22478864-d5ad-4829-b2bf-92ed9d9ca9c8	38097740-c468-4967-9518-04b39f5289ec	t
22478864-d5ad-4829-b2bf-92ed9d9ca9c8	2c288200-e6eb-4b9b-851c-035f5b8db264	f
22478864-d5ad-4829-b2bf-92ed9d9ca9c8	03a0470d-43a0-4262-a02f-c491715bcb56	t
22478864-d5ad-4829-b2bf-92ed9d9ca9c8	524331db-d0ab-4eaf-a3cb-b2e32aeb07bd	t
22478864-d5ad-4829-b2bf-92ed9d9ca9c8	6bebdcdd-2fd2-4e70-b83c-61099e5fcd51	f
22478864-d5ad-4829-b2bf-92ed9d9ca9c8	0b323a1f-3d9a-4569-97d7-cfb337e57a0c	t
\.


--
-- TOC entry 4137 (class 0 OID 24617)
-- Dependencies: 220
-- Data for Name: event_entity; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.event_entity (id, client_id, details_json, error, ip_address, realm_id, session_id, event_time, type, user_id, details_json_long_value) FROM stdin;
\.


--
-- TOC entry 4195 (class 0 OID 25672)
-- Dependencies: 278
-- Data for Name: fed_user_attribute; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.fed_user_attribute (id, name, user_id, realm_id, storage_provider_id, value, long_value_hash, long_value_hash_lower_case, long_value) FROM stdin;
\.


--
-- TOC entry 4196 (class 0 OID 25677)
-- Dependencies: 279
-- Data for Name: fed_user_consent; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.fed_user_consent (id, client_id, user_id, realm_id, storage_provider_id, created_date, last_updated_date, client_storage_provider, external_client_id) FROM stdin;
\.


--
-- TOC entry 4209 (class 0 OID 26000)
-- Dependencies: 292
-- Data for Name: fed_user_consent_cl_scope; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.fed_user_consent_cl_scope (user_consent_id, scope_id) FROM stdin;
\.


--
-- TOC entry 4197 (class 0 OID 25686)
-- Dependencies: 280
-- Data for Name: fed_user_credential; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.fed_user_credential (id, salt, type, created_date, user_id, realm_id, storage_provider_id, user_label, secret_data, credential_data, priority) FROM stdin;
\.


--
-- TOC entry 4198 (class 0 OID 25695)
-- Dependencies: 281
-- Data for Name: fed_user_group_membership; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.fed_user_group_membership (group_id, user_id, realm_id, storage_provider_id) FROM stdin;
\.


--
-- TOC entry 4199 (class 0 OID 25698)
-- Dependencies: 282
-- Data for Name: fed_user_required_action; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.fed_user_required_action (required_action, user_id, realm_id, storage_provider_id) FROM stdin;
\.


--
-- TOC entry 4200 (class 0 OID 25704)
-- Dependencies: 283
-- Data for Name: fed_user_role_mapping; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.fed_user_role_mapping (role_id, user_id, realm_id, storage_provider_id) FROM stdin;
\.


--
-- TOC entry 4157 (class 0 OID 24994)
-- Dependencies: 240
-- Data for Name: federated_identity; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.federated_identity (identity_provider, realm_id, federated_user_id, federated_username, token, user_id) FROM stdin;
\.


--
-- TOC entry 4203 (class 0 OID 25769)
-- Dependencies: 286
-- Data for Name: federated_user; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.federated_user (id, storage_provider_id, realm_id) FROM stdin;
\.


--
-- TOC entry 4179 (class 0 OID 25396)
-- Dependencies: 262
-- Data for Name: group_attribute; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.group_attribute (id, name, value, group_id) FROM stdin;
\.


--
-- TOC entry 4178 (class 0 OID 25393)
-- Dependencies: 261
-- Data for Name: group_role_mapping; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.group_role_mapping (role_id, group_id) FROM stdin;
\.


--
-- TOC entry 4158 (class 0 OID 24999)
-- Dependencies: 241
-- Data for Name: identity_provider; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.identity_provider (internal_id, enabled, provider_alias, provider_id, store_token, authenticate_by_default, realm_id, add_token_role, trust_email, first_broker_login_flow_id, post_broker_login_flow_id, provider_display_name, link_only, organization_id, hide_on_login) FROM stdin;
\.


--
-- TOC entry 4159 (class 0 OID 25008)
-- Dependencies: 242
-- Data for Name: identity_provider_config; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.identity_provider_config (identity_provider_id, value, name) FROM stdin;
\.


--
-- TOC entry 4163 (class 0 OID 25112)
-- Dependencies: 246
-- Data for Name: identity_provider_mapper; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.identity_provider_mapper (id, name, idp_alias, idp_mapper_name, realm_id) FROM stdin;
\.


--
-- TOC entry 4164 (class 0 OID 25117)
-- Dependencies: 247
-- Data for Name: idp_mapper_config; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.idp_mapper_config (idp_mapper_id, value, name) FROM stdin;
\.


--
-- TOC entry 4218 (class 0 OID 26202)
-- Dependencies: 301
-- Data for Name: jgroups_ping; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.jgroups_ping (address, name, cluster_name, ip, coord) FROM stdin;
\.


--
-- TOC entry 4177 (class 0 OID 25390)
-- Dependencies: 260
-- Data for Name: keycloak_group; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.keycloak_group (id, name, parent_group, realm_id, type) FROM stdin;
\.


--
-- TOC entry 4138 (class 0 OID 24625)
-- Dependencies: 221
-- Data for Name: keycloak_role; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.keycloak_role (id, client_realm_constraint, client_role, description, name, realm_id, client, realm) FROM stdin;
333d5b0e-b35b-447c-849f-6d1954882d54	9ccc2b44-8d11-4694-87e4-8e194b225e1d	f	${role_default-roles}	default-roles-master	9ccc2b44-8d11-4694-87e4-8e194b225e1d	\N	\N
dc53ea4d-3fcb-46ac-a814-df8415b9bb40	9ccc2b44-8d11-4694-87e4-8e194b225e1d	f	${role_create-realm}	create-realm	9ccc2b44-8d11-4694-87e4-8e194b225e1d	\N	\N
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	9ccc2b44-8d11-4694-87e4-8e194b225e1d	f	${role_admin}	admin	9ccc2b44-8d11-4694-87e4-8e194b225e1d	\N	\N
64cc72b0-840b-4dfe-8f4e-b63c713ee89f	9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	${role_create-client}	create-client	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9f3e1992-eb18-490a-a7af-2ddc4d947be5	\N
380e1a65-8df9-436a-8de2-f4e1234cc8b2	9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	${role_view-realm}	view-realm	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9f3e1992-eb18-490a-a7af-2ddc4d947be5	\N
4157d79f-00e3-4da9-915c-80008d3ba419	9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	${role_view-users}	view-users	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9f3e1992-eb18-490a-a7af-2ddc4d947be5	\N
0ae04e8d-0a82-47de-af0b-d8115bfbd362	9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	${role_view-clients}	view-clients	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9f3e1992-eb18-490a-a7af-2ddc4d947be5	\N
f47467eb-6bb9-4c13-9f1f-4c1d3d971818	9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	${role_view-events}	view-events	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9f3e1992-eb18-490a-a7af-2ddc4d947be5	\N
5d2fbb55-64d9-49c8-a075-b5ab93c54099	9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	${role_view-identity-providers}	view-identity-providers	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9f3e1992-eb18-490a-a7af-2ddc4d947be5	\N
87860229-fed9-491d-9148-43936b486420	9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	${role_view-authorization}	view-authorization	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9f3e1992-eb18-490a-a7af-2ddc4d947be5	\N
356f1685-3619-41ae-92d3-e100d78f77d6	9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	${role_manage-realm}	manage-realm	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9f3e1992-eb18-490a-a7af-2ddc4d947be5	\N
9d104f8e-c89c-434a-aac9-5ef19008b5cf	9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	${role_manage-users}	manage-users	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9f3e1992-eb18-490a-a7af-2ddc4d947be5	\N
6fce7751-623a-4f50-baf3-98d5859544ed	9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	${role_manage-clients}	manage-clients	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9f3e1992-eb18-490a-a7af-2ddc4d947be5	\N
ac5ff45d-649c-4845-9cf9-ddd596b5f265	9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	${role_manage-events}	manage-events	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9f3e1992-eb18-490a-a7af-2ddc4d947be5	\N
b0918c3c-35f4-40cb-90f6-91cb3c1693e5	9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	${role_manage-identity-providers}	manage-identity-providers	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9f3e1992-eb18-490a-a7af-2ddc4d947be5	\N
94d154d2-ec32-47ff-a94f-fbcdea2d83ab	9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	${role_manage-authorization}	manage-authorization	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9f3e1992-eb18-490a-a7af-2ddc4d947be5	\N
0536112c-0684-4d3c-8bca-dcb371abc4df	9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	${role_query-users}	query-users	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9f3e1992-eb18-490a-a7af-2ddc4d947be5	\N
5f4f4ea4-6492-40c5-b5ee-2792203858db	9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	${role_query-clients}	query-clients	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9f3e1992-eb18-490a-a7af-2ddc4d947be5	\N
b3c278c3-1100-40f7-87e1-1fc5d5b6cfa1	9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	${role_query-realms}	query-realms	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9f3e1992-eb18-490a-a7af-2ddc4d947be5	\N
022ac9ce-2ddd-4cdd-84d4-03193514f22e	9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	${role_query-groups}	query-groups	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9f3e1992-eb18-490a-a7af-2ddc4d947be5	\N
390fff60-e27e-4c07-8950-30214e724306	80bd01e1-a0e7-45d4-9873-540a8217051d	t	${role_view-profile}	view-profile	9ccc2b44-8d11-4694-87e4-8e194b225e1d	80bd01e1-a0e7-45d4-9873-540a8217051d	\N
7c1f6e3d-2852-442c-81a0-0db59b56eea1	80bd01e1-a0e7-45d4-9873-540a8217051d	t	${role_manage-account}	manage-account	9ccc2b44-8d11-4694-87e4-8e194b225e1d	80bd01e1-a0e7-45d4-9873-540a8217051d	\N
1b14b11f-7685-44ab-9d6e-740aef46a781	80bd01e1-a0e7-45d4-9873-540a8217051d	t	${role_manage-account-links}	manage-account-links	9ccc2b44-8d11-4694-87e4-8e194b225e1d	80bd01e1-a0e7-45d4-9873-540a8217051d	\N
61712afd-1efc-4bce-af49-4b5a3637d990	80bd01e1-a0e7-45d4-9873-540a8217051d	t	${role_view-applications}	view-applications	9ccc2b44-8d11-4694-87e4-8e194b225e1d	80bd01e1-a0e7-45d4-9873-540a8217051d	\N
aa4848f8-d34c-4199-99ba-ee75a4741a9d	80bd01e1-a0e7-45d4-9873-540a8217051d	t	${role_view-consent}	view-consent	9ccc2b44-8d11-4694-87e4-8e194b225e1d	80bd01e1-a0e7-45d4-9873-540a8217051d	\N
69cfe3f6-0cd9-44c6-90c5-1cd521be9fdd	80bd01e1-a0e7-45d4-9873-540a8217051d	t	${role_manage-consent}	manage-consent	9ccc2b44-8d11-4694-87e4-8e194b225e1d	80bd01e1-a0e7-45d4-9873-540a8217051d	\N
3b221f0d-f0bd-4980-a6c6-a01e62dbcc50	80bd01e1-a0e7-45d4-9873-540a8217051d	t	${role_view-groups}	view-groups	9ccc2b44-8d11-4694-87e4-8e194b225e1d	80bd01e1-a0e7-45d4-9873-540a8217051d	\N
26013cc2-2dfa-4509-ad7e-5671830d5b9f	80bd01e1-a0e7-45d4-9873-540a8217051d	t	${role_delete-account}	delete-account	9ccc2b44-8d11-4694-87e4-8e194b225e1d	80bd01e1-a0e7-45d4-9873-540a8217051d	\N
431d7153-703d-4918-b8ae-d1a1d45624a4	f859cd3b-5020-4869-8d00-d688f2b7b3bb	t	${role_read-token}	read-token	9ccc2b44-8d11-4694-87e4-8e194b225e1d	f859cd3b-5020-4869-8d00-d688f2b7b3bb	\N
abbf467d-4085-4b6d-b964-2202113c144f	9f3e1992-eb18-490a-a7af-2ddc4d947be5	t	${role_impersonation}	impersonation	9ccc2b44-8d11-4694-87e4-8e194b225e1d	9f3e1992-eb18-490a-a7af-2ddc4d947be5	\N
80bbb03d-dddd-4339-84b1-690dbf029de8	9ccc2b44-8d11-4694-87e4-8e194b225e1d	f	${role_offline-access}	offline_access	9ccc2b44-8d11-4694-87e4-8e194b225e1d	\N	\N
0b7066ed-54b3-4ceb-814a-c7df9f5eba60	9ccc2b44-8d11-4694-87e4-8e194b225e1d	f	${role_uma_authorization}	uma_authorization	9ccc2b44-8d11-4694-87e4-8e194b225e1d	\N	\N
6337a65d-feea-4b7c-b5e2-6d122b88b571	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f	${role_default-roles}	default-roles-bambaiba-realm	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	\N	\N
58e89a31-840e-49fc-8677-a26f2a9ed5a2	8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	${role_create-client}	create-client	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8675ed84-ae64-4ead-aaf4-913e1d7d3397	\N
c7ba1b75-982f-43b7-bd10-8e0feef9c554	8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	${role_view-realm}	view-realm	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8675ed84-ae64-4ead-aaf4-913e1d7d3397	\N
8efde795-9511-4eb3-abe9-b8468bc02983	8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	${role_view-users}	view-users	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8675ed84-ae64-4ead-aaf4-913e1d7d3397	\N
6b3de988-e5d4-4c6f-b5d6-0490f9935949	8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	${role_view-clients}	view-clients	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8675ed84-ae64-4ead-aaf4-913e1d7d3397	\N
5d5eb9ed-6c16-4ff4-9ab5-7a2e3730be41	8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	${role_view-events}	view-events	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8675ed84-ae64-4ead-aaf4-913e1d7d3397	\N
e91b22e7-31af-41cc-947a-ea536fe8f03e	8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	${role_view-identity-providers}	view-identity-providers	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8675ed84-ae64-4ead-aaf4-913e1d7d3397	\N
3a7c9cf0-68e4-4f14-b8cf-4aaea27bde21	8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	${role_view-authorization}	view-authorization	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8675ed84-ae64-4ead-aaf4-913e1d7d3397	\N
222ff0cb-515e-42d4-a3f6-e195a33a58df	8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	${role_manage-realm}	manage-realm	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8675ed84-ae64-4ead-aaf4-913e1d7d3397	\N
548365c0-c74f-401e-a49d-f27dde9b8fc3	8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	${role_manage-users}	manage-users	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8675ed84-ae64-4ead-aaf4-913e1d7d3397	\N
3dee69c1-baf4-4083-95c9-e9e253c6e531	8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	${role_manage-clients}	manage-clients	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8675ed84-ae64-4ead-aaf4-913e1d7d3397	\N
a0b7e6c7-5ba5-472b-af73-effdc48e268b	8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	${role_manage-events}	manage-events	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8675ed84-ae64-4ead-aaf4-913e1d7d3397	\N
9f3a7329-42e6-4a17-8254-a9844ca68dc7	8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	${role_manage-identity-providers}	manage-identity-providers	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8675ed84-ae64-4ead-aaf4-913e1d7d3397	\N
2248f8c9-e631-4c60-9cef-f30394594d45	8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	${role_manage-authorization}	manage-authorization	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8675ed84-ae64-4ead-aaf4-913e1d7d3397	\N
2e93cba4-9c6e-4769-9a5f-3662b8824ddf	8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	${role_query-users}	query-users	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8675ed84-ae64-4ead-aaf4-913e1d7d3397	\N
df44c961-8daa-4462-af0d-835543b505af	8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	${role_query-clients}	query-clients	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8675ed84-ae64-4ead-aaf4-913e1d7d3397	\N
c3b1ccd2-a938-412b-b5e2-4eb2e1d4b7bd	8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	${role_query-realms}	query-realms	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8675ed84-ae64-4ead-aaf4-913e1d7d3397	\N
7b211510-1bdf-4dad-81ab-9d6b7860e74a	8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	${role_query-groups}	query-groups	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8675ed84-ae64-4ead-aaf4-913e1d7d3397	\N
951a9506-6d8f-454a-8d91-99aa9a1e34ec	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_realm-admin}	realm-admin	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
bef72c5d-9c9e-43eb-83ec-b2795d440d3b	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_create-client}	create-client	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
c279ff02-28e0-4a86-a64a-f898ade088bb	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_view-realm}	view-realm	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
f07d7887-ca58-42da-af92-2b818d9df8a2	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_view-users}	view-users	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
dd1e4035-f356-4fb5-bdab-49c13aeaaf3b	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_view-clients}	view-clients	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
de1eb8b9-4d6c-4165-858a-94ea47301ad0	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_view-events}	view-events	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
e1816621-1c04-432f-8e17-a9c594952c31	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_view-identity-providers}	view-identity-providers	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
0eb837e0-1e0d-4f25-8ed5-efc5ef872be1	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_view-authorization}	view-authorization	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
2827a618-c764-4769-8ca2-b6e3463dbd71	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_manage-realm}	manage-realm	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
966cb239-0efe-4e7a-a18c-a2e814e872ad	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_manage-users}	manage-users	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
1e1bcb15-2939-4bbe-abb6-72aca08fc061	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_manage-clients}	manage-clients	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
9abc4903-5fcc-452c-bd00-636d02a59b02	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_manage-events}	manage-events	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
511e0b44-7570-4a99-9f0d-491ecc2b0510	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_manage-identity-providers}	manage-identity-providers	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
566f768d-b842-4e5f-bd97-d21963abaaa1	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_manage-authorization}	manage-authorization	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
6f1f8f24-ef5e-48c4-b903-487443d13235	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_query-users}	query-users	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
f7001d0c-d197-406a-b9af-3dbcc1580f03	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_query-clients}	query-clients	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
d2601b00-9729-473c-aadb-4cf8ff6f8676	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_query-realms}	query-realms	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
cbefa965-534f-47e4-83d5-2da96b31260c	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_query-groups}	query-groups	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
0a4435de-49f4-4a96-a158-443c5ac3317d	32b64136-f72f-40be-8350-89e652c4d54f	t	${role_view-profile}	view-profile	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	32b64136-f72f-40be-8350-89e652c4d54f	\N
416f330a-0839-466e-a3f3-7baebb4a3e61	32b64136-f72f-40be-8350-89e652c4d54f	t	${role_manage-account}	manage-account	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	32b64136-f72f-40be-8350-89e652c4d54f	\N
cfbb7550-b6a9-488f-ab57-f6240dd0cbf9	32b64136-f72f-40be-8350-89e652c4d54f	t	${role_manage-account-links}	manage-account-links	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	32b64136-f72f-40be-8350-89e652c4d54f	\N
3b7b1872-4869-4418-b6d7-ab38fd7e6340	32b64136-f72f-40be-8350-89e652c4d54f	t	${role_view-applications}	view-applications	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	32b64136-f72f-40be-8350-89e652c4d54f	\N
d8aeb9b6-b2ef-4a59-95de-9058d9bcaa7a	32b64136-f72f-40be-8350-89e652c4d54f	t	${role_view-consent}	view-consent	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	32b64136-f72f-40be-8350-89e652c4d54f	\N
c5000d03-36d6-4c57-8170-6e9ce9454f83	32b64136-f72f-40be-8350-89e652c4d54f	t	${role_manage-consent}	manage-consent	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	32b64136-f72f-40be-8350-89e652c4d54f	\N
de74335c-d20d-47a2-a8d0-267a731a5dd5	32b64136-f72f-40be-8350-89e652c4d54f	t	${role_view-groups}	view-groups	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	32b64136-f72f-40be-8350-89e652c4d54f	\N
237789ce-b0fa-4cd7-a76e-a83130220f25	32b64136-f72f-40be-8350-89e652c4d54f	t	${role_delete-account}	delete-account	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	32b64136-f72f-40be-8350-89e652c4d54f	\N
f7199ff7-a9ef-4a27-a3b5-ae68a4f19ca5	8675ed84-ae64-4ead-aaf4-913e1d7d3397	t	${role_impersonation}	impersonation	9ccc2b44-8d11-4694-87e4-8e194b225e1d	8675ed84-ae64-4ead-aaf4-913e1d7d3397	\N
1086d057-7584-4416-b18c-14201f82274a	f62fa0c1-357e-4d58-af34-9d108b6b24c0	t	${role_impersonation}	impersonation	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f62fa0c1-357e-4d58-af34-9d108b6b24c0	\N
3c7dbe71-64ed-4451-96f6-b0bff0aea3d8	157360ea-5728-4d05-877e-9bf20421e380	t	${role_read-token}	read-token	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	157360ea-5728-4d05-877e-9bf20421e380	\N
163f9ffa-1a43-48cd-8ad3-a918279550ae	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f	${role_offline-access}	offline_access	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	\N	\N
663e2268-8919-4289-9cdb-4ec4347d7e58	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f	${role_uma_authorization}	uma_authorization	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	\N	\N
\.


--
-- TOC entry 4162 (class 0 OID 25109)
-- Dependencies: 245
-- Data for Name: migration_model; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.migration_model (id, version, update_time) FROM stdin;
3ekjo	26.1.2	1761219216
\.


--
-- TOC entry 4176 (class 0 OID 25381)
-- Dependencies: 259
-- Data for Name: offline_client_session; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.offline_client_session (user_session_id, client_id, offline_flag, "timestamp", data, client_storage_provider, external_client_id, version) FROM stdin;
e2b219fe-1408-484f-80e5-3ac70a4eab32	6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	0	1769430821	{"authMethod":"openid-connect","redirectUri":"http://localhost:18081/admin/master/console/#/master","notes":{"clientId":"6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e","iss":"http://localhost:18081/realms/master","startedAt":"1769430107","response_type":"code","level-of-authentication":"-1","code_challenge_method":"S256","nonce":"cb592610-f475-4aa5-b5ca-173c6ab2cd50","response_mode":"query","scope":"openid","userSessionStartedAt":"1769430107","redirect_uri":"http://localhost:18081/admin/master/console/#/master","state":"1f30113b-a0d0-4792-9639-53497db85194","code_challenge":"SpukOecSsZiboIMRzK3inbCWNk97RIC7IIcg8Co77H0"}}	local	local	9
b31087fe-d516-4e67-9532-169fff233813	6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	0	1769429028	{"authMethod":"openid-connect","redirectUri":"http://localhost:18081/admin/master/console/#/master/clients/8675ed84-ae64-4ead-aaf4-913e1d7d3397/settings","notes":{"clientId":"6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e","iss":"http://localhost:18081/realms/master","startedAt":"1769429026","response_type":"code","level-of-authentication":"-1","code_challenge_method":"S256","nonce":"b40800fb-b242-436d-b2ba-1cef0a909899","response_mode":"query","scope":"openid","userSessionStartedAt":"1769429026","redirect_uri":"http://localhost:18081/admin/master/console/#/master/clients/8675ed84-ae64-4ead-aaf4-913e1d7d3397/settings","state":"0da678d1-48f8-4f59-b0ef-c8176a73af5a","code_challenge":"FBoBLq_X-t86KkxGnOxSfFtmF6IEnntXbSUd0xvgKss"}}	local	local	1
\.


--
-- TOC entry 4175 (class 0 OID 25376)
-- Dependencies: 258
-- Data for Name: offline_user_session; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.offline_user_session (user_session_id, user_id, realm_id, created_on, offline_flag, data, last_session_refresh, broker_session_id, version) FROM stdin;
b31087fe-d516-4e67-9532-169fff233813	f1bd369f-b87c-4434-bdf7-e6cc125849d1	9ccc2b44-8d11-4694-87e4-8e194b225e1d	1769429026	0	{"ipAddress":"172.18.0.1","authMethod":"openid-connect","rememberMe":false,"started":0,"notes":{"KC_DEVICE_NOTE":"eyJpcEFkZHJlc3MiOiIxNzIuMTguMC4xIiwib3MiOiJXaW5kb3dzIiwib3NWZXJzaW9uIjoiMTAiLCJicm93c2VyIjoiQ2hyb21lLzE0NC4wLjAiLCJkZXZpY2UiOiJPdGhlciIsImxhc3RBY2Nlc3MiOjAsIm1vYmlsZSI6ZmFsc2V9","AUTH_TIME":"1769429026","authenticators-completed":"{\\"2c1ee7ad-ade8-4bf7-900c-768b2ad6d5fa\\":1769429026}"},"state":"LOGGED_IN"}	1769429028	\N	1
e2b219fe-1408-484f-80e5-3ac70a4eab32	f1bd369f-b87c-4434-bdf7-e6cc125849d1	9ccc2b44-8d11-4694-87e4-8e194b225e1d	1769430107	0	{"ipAddress":"172.18.0.1","authMethod":"openid-connect","rememberMe":false,"started":0,"notes":{"KC_DEVICE_NOTE":"eyJpcEFkZHJlc3MiOiIxNzIuMTguMC4xIiwib3MiOiJXaW5kb3dzIiwib3NWZXJzaW9uIjoiMTAiLCJicm93c2VyIjoiRWRnZS8xNDQuMC4wIiwiZGV2aWNlIjoiT3RoZXIiLCJsYXN0QWNjZXNzIjowLCJtb2JpbGUiOmZhbHNlfQ==","AUTH_TIME":"1769430107","authenticators-completed":"{\\"2c1ee7ad-ade8-4bf7-900c-768b2ad6d5fa\\":1769430107}"},"state":"LOGGED_IN"}	1769430821	\N	9
\.


--
-- TOC entry 4215 (class 0 OID 26165)
-- Dependencies: 298
-- Data for Name: org; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.org (id, enabled, realm_id, group_id, name, description, alias, redirect_url) FROM stdin;
\.


--
-- TOC entry 4216 (class 0 OID 26176)
-- Dependencies: 299
-- Data for Name: org_domain; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.org_domain (id, name, verified, org_id) FROM stdin;
\.


--
-- TOC entry 4189 (class 0 OID 25595)
-- Dependencies: 272
-- Data for Name: policy_config; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.policy_config (policy_id, name, value) FROM stdin;
\.


--
-- TOC entry 4155 (class 0 OID 24983)
-- Dependencies: 238
-- Data for Name: protocol_mapper; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.protocol_mapper (id, name, protocol, protocol_mapper_name, client_id, client_scope_id) FROM stdin;
043e0872-ad49-4a27-b162-ff8b79da3d3e	audience resolve	openid-connect	oidc-audience-resolve-mapper	9175a02f-cf18-40a3-b322-73ed00ce1729	\N
fb3379d4-d55f-4365-9e51-ffe4cad42ab7	locale	openid-connect	oidc-usermodel-attribute-mapper	6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	\N
bbd13349-259d-40e3-82d9-7e025b1b8f67	role list	saml	saml-role-list-mapper	\N	3447361c-98b8-4146-a2a8-d103e31a382e
17d444f6-a5d9-4e9b-b4c9-cca2d0d02e41	organization	saml	saml-organization-membership-mapper	\N	2354c9d6-9230-4ef8-9818-d56485529aae
7806f04b-7d83-48d9-9e92-cfc70e7af129	full name	openid-connect	oidc-full-name-mapper	\N	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685
e7cff1ab-d714-4e87-9f59-114dc090ac48	family name	openid-connect	oidc-usermodel-attribute-mapper	\N	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685
b175af8d-7c62-4408-a2aa-de5d291bef1d	given name	openid-connect	oidc-usermodel-attribute-mapper	\N	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685
1bd7b2fb-e0d9-4a66-92f8-c70cb80e3eac	middle name	openid-connect	oidc-usermodel-attribute-mapper	\N	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685
7d774fbf-143a-4085-b959-289613141b7c	nickname	openid-connect	oidc-usermodel-attribute-mapper	\N	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685
124728de-5068-41d0-b69b-6f05e64b220e	username	openid-connect	oidc-usermodel-attribute-mapper	\N	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685
ad8fc644-3245-4ec4-b8e3-3b1b1572eefa	profile	openid-connect	oidc-usermodel-attribute-mapper	\N	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685
a43ca71e-295e-417b-94db-3a24fe84629d	picture	openid-connect	oidc-usermodel-attribute-mapper	\N	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685
a40d15f0-7dea-4395-9d27-bdf9c2bd01e4	website	openid-connect	oidc-usermodel-attribute-mapper	\N	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685
c0a306a9-1bd7-4d67-ba41-3d9acb35865d	gender	openid-connect	oidc-usermodel-attribute-mapper	\N	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685
8866dcca-f102-47c1-908c-51a6e8f14007	birthdate	openid-connect	oidc-usermodel-attribute-mapper	\N	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685
eb171c96-7443-433e-a35e-26c94cb32727	zoneinfo	openid-connect	oidc-usermodel-attribute-mapper	\N	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685
37c27c64-f72f-47bf-acae-435a676fa447	locale	openid-connect	oidc-usermodel-attribute-mapper	\N	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685
575ca26b-99b1-4dc7-8dbb-e9ee69458e6d	updated at	openid-connect	oidc-usermodel-attribute-mapper	\N	f87e9ba6-df51-4a32-a0e4-8bd7fe00b685
2ddb9c43-c9fd-4c9c-8667-dfaa3805b900	email	openid-connect	oidc-usermodel-attribute-mapper	\N	9477d6cb-3396-463f-807f-e7e05c77502b
36128397-81ab-4071-bb13-de7a2a2b18bd	email verified	openid-connect	oidc-usermodel-property-mapper	\N	9477d6cb-3396-463f-807f-e7e05c77502b
d12697cf-543e-49b5-9b94-d69c0cc14d87	address	openid-connect	oidc-address-mapper	\N	43597b3d-8978-4057-9084-9d511fd8e58e
86e87ba7-eb3d-457c-a21a-bf9bd7e807f1	phone number	openid-connect	oidc-usermodel-attribute-mapper	\N	11daa3f1-9d5b-41b7-b68b-1d3d9f9124ed
7049f7f7-3737-4e05-8cc2-ea95a9f28edf	phone number verified	openid-connect	oidc-usermodel-attribute-mapper	\N	11daa3f1-9d5b-41b7-b68b-1d3d9f9124ed
977d901c-cd00-4d66-b418-1997ee721f43	realm roles	openid-connect	oidc-usermodel-realm-role-mapper	\N	8e148a79-c784-465a-9c32-b340e03f5c0c
ce7051e3-1cde-4be6-9158-866bdce8f6be	client roles	openid-connect	oidc-usermodel-client-role-mapper	\N	8e148a79-c784-465a-9c32-b340e03f5c0c
8d9a8241-e859-4ec9-aeb9-366f64fc5b4e	audience resolve	openid-connect	oidc-audience-resolve-mapper	\N	8e148a79-c784-465a-9c32-b340e03f5c0c
36925b0f-92c2-4180-80d4-e9ae2029434d	allowed web origins	openid-connect	oidc-allowed-origins-mapper	\N	142e3661-dcf9-43be-a77d-278835a44ef7
751f9d76-99e0-4fe7-84e9-5c598e9a97aa	upn	openid-connect	oidc-usermodel-attribute-mapper	\N	bc717a1c-2345-40e5-9d27-01579b8b2852
d2d6548d-60d2-40c8-8013-b0574b77df11	groups	openid-connect	oidc-usermodel-realm-role-mapper	\N	bc717a1c-2345-40e5-9d27-01579b8b2852
96c2622b-db5e-4537-a3e0-40a1758145b8	acr loa level	openid-connect	oidc-acr-mapper	\N	e9143869-1a5f-4f96-a55c-0e2b2d63b30b
3aabe97f-cbc1-4bdb-ac51-9fe6963f5211	auth_time	openid-connect	oidc-usersessionmodel-note-mapper	\N	0486e99d-cae3-472e-98c6-e1da9bc9563a
0ec037b9-d897-461f-a59e-0579b9478090	sub	openid-connect	oidc-sub-mapper	\N	0486e99d-cae3-472e-98c6-e1da9bc9563a
f6907f92-ccca-4253-b90e-b0fa79f68e08	Client ID	openid-connect	oidc-usersessionmodel-note-mapper	\N	a98c98ef-d43c-442f-bf0b-6b035195c6e7
61a0f60d-9f30-433e-9616-d0e7f73dd6ef	Client Host	openid-connect	oidc-usersessionmodel-note-mapper	\N	a98c98ef-d43c-442f-bf0b-6b035195c6e7
b4914135-3088-4dd3-87ea-6243862f973e	Client IP Address	openid-connect	oidc-usersessionmodel-note-mapper	\N	a98c98ef-d43c-442f-bf0b-6b035195c6e7
63babe4b-a402-4e35-afed-57cad831d9b1	organization	openid-connect	oidc-organization-membership-mapper	\N	f646375e-1c13-4dd9-a732-e328bf9f1a4d
472b2493-87b7-4c61-9368-64fd0b41fef0	audience resolve	openid-connect	oidc-audience-resolve-mapper	660743a4-25c1-46dd-b9c1-a861be2a2d84	\N
0d031525-33bf-4372-865c-e817ae5c8f40	role list	saml	saml-role-list-mapper	\N	6f8b807e-fe47-446c-b699-a352acbd54f0
cf902d43-83c3-48f7-ae7e-e2294c282c76	organization	saml	saml-organization-membership-mapper	\N	d705b59a-33c0-42c0-8e04-8684529ed9c0
8c176027-1970-49f6-b4f9-ed136e4a6aa0	full name	openid-connect	oidc-full-name-mapper	\N	d33cf0c5-5f0c-45ae-8103-1fabd28670df
d40c931b-9de6-402d-a9d8-15e0c3d1e818	family name	openid-connect	oidc-usermodel-attribute-mapper	\N	d33cf0c5-5f0c-45ae-8103-1fabd28670df
d0ed7ce7-3bbd-4892-9474-c0861fa7ca9e	given name	openid-connect	oidc-usermodel-attribute-mapper	\N	d33cf0c5-5f0c-45ae-8103-1fabd28670df
fb809382-8aff-46f7-ad11-cf921f833068	middle name	openid-connect	oidc-usermodel-attribute-mapper	\N	d33cf0c5-5f0c-45ae-8103-1fabd28670df
bf2bb774-97a0-4149-bbe4-8076676f19f5	nickname	openid-connect	oidc-usermodel-attribute-mapper	\N	d33cf0c5-5f0c-45ae-8103-1fabd28670df
f8043730-bc96-4d12-965f-f592cc69d967	username	openid-connect	oidc-usermodel-attribute-mapper	\N	d33cf0c5-5f0c-45ae-8103-1fabd28670df
0a2645eb-7075-41f1-9e96-335015a9c640	profile	openid-connect	oidc-usermodel-attribute-mapper	\N	d33cf0c5-5f0c-45ae-8103-1fabd28670df
44e03518-4b15-4bdd-a053-5da57c2abd4b	picture	openid-connect	oidc-usermodel-attribute-mapper	\N	d33cf0c5-5f0c-45ae-8103-1fabd28670df
8f09a57c-0a75-4687-8f64-4f498d6eaf02	website	openid-connect	oidc-usermodel-attribute-mapper	\N	d33cf0c5-5f0c-45ae-8103-1fabd28670df
6c8375ed-46c5-4e54-bf3e-b63359c6d652	gender	openid-connect	oidc-usermodel-attribute-mapper	\N	d33cf0c5-5f0c-45ae-8103-1fabd28670df
16b8817c-3918-4efe-9b71-9ee9f172a1a8	birthdate	openid-connect	oidc-usermodel-attribute-mapper	\N	d33cf0c5-5f0c-45ae-8103-1fabd28670df
96bbf2cb-93cc-4c5f-aa1f-48d35e2c465d	zoneinfo	openid-connect	oidc-usermodel-attribute-mapper	\N	d33cf0c5-5f0c-45ae-8103-1fabd28670df
c6dc8a27-9cbb-4c0a-b789-efc6087a8969	locale	openid-connect	oidc-usermodel-attribute-mapper	\N	d33cf0c5-5f0c-45ae-8103-1fabd28670df
e1ee7e12-ac6c-43e8-9c4d-edb0accd799a	updated at	openid-connect	oidc-usermodel-attribute-mapper	\N	d33cf0c5-5f0c-45ae-8103-1fabd28670df
e0519651-be6d-4a58-ad20-6731b20cd993	email	openid-connect	oidc-usermodel-attribute-mapper	\N	8a3526d2-4888-4029-ad16-185e7e0cf9c0
f7e776fd-fdc6-4af2-a755-89fcbe1cb912	email verified	openid-connect	oidc-usermodel-property-mapper	\N	8a3526d2-4888-4029-ad16-185e7e0cf9c0
656f431f-a11c-475f-9359-0ef1a7a49b9c	address	openid-connect	oidc-address-mapper	\N	319da478-2b63-4964-b514-a9109e256736
2a8c3b33-15e8-421c-b186-e95ef4a21908	phone number	openid-connect	oidc-usermodel-attribute-mapper	\N	27b6c674-5fc7-402d-812a-43c50fbf5eb9
69c49ab4-d57e-47ee-b5f3-fc30d921cfe7	phone number verified	openid-connect	oidc-usermodel-attribute-mapper	\N	27b6c674-5fc7-402d-812a-43c50fbf5eb9
be610505-b9d2-493d-9135-212338257b2e	realm roles	openid-connect	oidc-usermodel-realm-role-mapper	\N	0a460cd8-7178-4845-b532-c84568f3be21
1f47b622-574f-491a-93b7-e324f7b41828	client roles	openid-connect	oidc-usermodel-client-role-mapper	\N	0a460cd8-7178-4845-b532-c84568f3be21
44e9e2b0-f300-46a9-9473-001dce537ea0	audience resolve	openid-connect	oidc-audience-resolve-mapper	\N	0a460cd8-7178-4845-b532-c84568f3be21
28eddf9e-235f-433d-bea6-a4d786a6b2ae	allowed web origins	openid-connect	oidc-allowed-origins-mapper	\N	38097740-c468-4967-9518-04b39f5289ec
99c5f74c-1b4f-4b32-bc1c-8da81f6a229a	upn	openid-connect	oidc-usermodel-attribute-mapper	\N	2c288200-e6eb-4b9b-851c-035f5b8db264
3167b204-ef95-425c-b3c3-133bb374badb	groups	openid-connect	oidc-usermodel-realm-role-mapper	\N	2c288200-e6eb-4b9b-851c-035f5b8db264
b0e402e4-cc42-4414-801e-204b2f9f9e4c	acr loa level	openid-connect	oidc-acr-mapper	\N	03a0470d-43a0-4262-a02f-c491715bcb56
fc8e99e5-f440-4640-93f8-b0675d65aca3	auth_time	openid-connect	oidc-usersessionmodel-note-mapper	\N	524331db-d0ab-4eaf-a3cb-b2e32aeb07bd
57ba6737-ba69-4632-8890-a1e1d38c562a	sub	openid-connect	oidc-sub-mapper	\N	524331db-d0ab-4eaf-a3cb-b2e32aeb07bd
fb49e089-d912-409f-9d86-b2309c4afa2f	Client ID	openid-connect	oidc-usersessionmodel-note-mapper	\N	148cf1d2-75c1-4066-988a-f02e2800de5a
f35e9a13-74ad-4506-b918-6ccf27b57586	Client Host	openid-connect	oidc-usersessionmodel-note-mapper	\N	148cf1d2-75c1-4066-988a-f02e2800de5a
2acb1b68-a197-4f02-bd63-ddfdf903a151	Client IP Address	openid-connect	oidc-usersessionmodel-note-mapper	\N	148cf1d2-75c1-4066-988a-f02e2800de5a
a0113ab2-1031-4b80-a5f9-d393331c4d02	organization	openid-connect	oidc-organization-membership-mapper	\N	6bebdcdd-2fd2-4e70-b83c-61099e5fcd51
c15447a3-1a10-4925-81ed-5823834b4d69	locale	openid-connect	oidc-usermodel-attribute-mapper	862e68c6-d552-4480-9b9f-4d3a69704afa	\N
e1b67e77-9ddd-4c0c-972d-5e52f6003df3	bambaiba-api	openid-connect	oidc-audience-mapper	\N	0b323a1f-3d9a-4569-97d7-cfb337e57a0c
\.


--
-- TOC entry 4156 (class 0 OID 24989)
-- Dependencies: 239
-- Data for Name: protocol_mapper_config; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.protocol_mapper_config (protocol_mapper_id, value, name) FROM stdin;
fb3379d4-d55f-4365-9e51-ffe4cad42ab7	true	introspection.token.claim
fb3379d4-d55f-4365-9e51-ffe4cad42ab7	true	userinfo.token.claim
fb3379d4-d55f-4365-9e51-ffe4cad42ab7	locale	user.attribute
fb3379d4-d55f-4365-9e51-ffe4cad42ab7	true	id.token.claim
fb3379d4-d55f-4365-9e51-ffe4cad42ab7	true	access.token.claim
fb3379d4-d55f-4365-9e51-ffe4cad42ab7	locale	claim.name
fb3379d4-d55f-4365-9e51-ffe4cad42ab7	String	jsonType.label
bbd13349-259d-40e3-82d9-7e025b1b8f67	false	single
bbd13349-259d-40e3-82d9-7e025b1b8f67	Basic	attribute.nameformat
bbd13349-259d-40e3-82d9-7e025b1b8f67	Role	attribute.name
124728de-5068-41d0-b69b-6f05e64b220e	true	introspection.token.claim
124728de-5068-41d0-b69b-6f05e64b220e	true	userinfo.token.claim
124728de-5068-41d0-b69b-6f05e64b220e	username	user.attribute
124728de-5068-41d0-b69b-6f05e64b220e	true	id.token.claim
124728de-5068-41d0-b69b-6f05e64b220e	true	access.token.claim
124728de-5068-41d0-b69b-6f05e64b220e	preferred_username	claim.name
124728de-5068-41d0-b69b-6f05e64b220e	String	jsonType.label
1bd7b2fb-e0d9-4a66-92f8-c70cb80e3eac	true	introspection.token.claim
1bd7b2fb-e0d9-4a66-92f8-c70cb80e3eac	true	userinfo.token.claim
1bd7b2fb-e0d9-4a66-92f8-c70cb80e3eac	middleName	user.attribute
1bd7b2fb-e0d9-4a66-92f8-c70cb80e3eac	true	id.token.claim
1bd7b2fb-e0d9-4a66-92f8-c70cb80e3eac	true	access.token.claim
1bd7b2fb-e0d9-4a66-92f8-c70cb80e3eac	middle_name	claim.name
1bd7b2fb-e0d9-4a66-92f8-c70cb80e3eac	String	jsonType.label
37c27c64-f72f-47bf-acae-435a676fa447	true	introspection.token.claim
37c27c64-f72f-47bf-acae-435a676fa447	true	userinfo.token.claim
37c27c64-f72f-47bf-acae-435a676fa447	locale	user.attribute
37c27c64-f72f-47bf-acae-435a676fa447	true	id.token.claim
37c27c64-f72f-47bf-acae-435a676fa447	true	access.token.claim
37c27c64-f72f-47bf-acae-435a676fa447	locale	claim.name
37c27c64-f72f-47bf-acae-435a676fa447	String	jsonType.label
575ca26b-99b1-4dc7-8dbb-e9ee69458e6d	true	introspection.token.claim
575ca26b-99b1-4dc7-8dbb-e9ee69458e6d	true	userinfo.token.claim
575ca26b-99b1-4dc7-8dbb-e9ee69458e6d	updatedAt	user.attribute
575ca26b-99b1-4dc7-8dbb-e9ee69458e6d	true	id.token.claim
575ca26b-99b1-4dc7-8dbb-e9ee69458e6d	true	access.token.claim
575ca26b-99b1-4dc7-8dbb-e9ee69458e6d	updated_at	claim.name
575ca26b-99b1-4dc7-8dbb-e9ee69458e6d	long	jsonType.label
7806f04b-7d83-48d9-9e92-cfc70e7af129	true	introspection.token.claim
7806f04b-7d83-48d9-9e92-cfc70e7af129	true	userinfo.token.claim
7806f04b-7d83-48d9-9e92-cfc70e7af129	true	id.token.claim
7806f04b-7d83-48d9-9e92-cfc70e7af129	true	access.token.claim
7d774fbf-143a-4085-b959-289613141b7c	true	introspection.token.claim
7d774fbf-143a-4085-b959-289613141b7c	true	userinfo.token.claim
7d774fbf-143a-4085-b959-289613141b7c	nickname	user.attribute
7d774fbf-143a-4085-b959-289613141b7c	true	id.token.claim
7d774fbf-143a-4085-b959-289613141b7c	true	access.token.claim
7d774fbf-143a-4085-b959-289613141b7c	nickname	claim.name
7d774fbf-143a-4085-b959-289613141b7c	String	jsonType.label
8866dcca-f102-47c1-908c-51a6e8f14007	true	introspection.token.claim
8866dcca-f102-47c1-908c-51a6e8f14007	true	userinfo.token.claim
8866dcca-f102-47c1-908c-51a6e8f14007	birthdate	user.attribute
8866dcca-f102-47c1-908c-51a6e8f14007	true	id.token.claim
8866dcca-f102-47c1-908c-51a6e8f14007	true	access.token.claim
8866dcca-f102-47c1-908c-51a6e8f14007	birthdate	claim.name
8866dcca-f102-47c1-908c-51a6e8f14007	String	jsonType.label
a40d15f0-7dea-4395-9d27-bdf9c2bd01e4	true	introspection.token.claim
a40d15f0-7dea-4395-9d27-bdf9c2bd01e4	true	userinfo.token.claim
a40d15f0-7dea-4395-9d27-bdf9c2bd01e4	website	user.attribute
a40d15f0-7dea-4395-9d27-bdf9c2bd01e4	true	id.token.claim
a40d15f0-7dea-4395-9d27-bdf9c2bd01e4	true	access.token.claim
a40d15f0-7dea-4395-9d27-bdf9c2bd01e4	website	claim.name
a40d15f0-7dea-4395-9d27-bdf9c2bd01e4	String	jsonType.label
a43ca71e-295e-417b-94db-3a24fe84629d	true	introspection.token.claim
a43ca71e-295e-417b-94db-3a24fe84629d	true	userinfo.token.claim
a43ca71e-295e-417b-94db-3a24fe84629d	picture	user.attribute
a43ca71e-295e-417b-94db-3a24fe84629d	true	id.token.claim
a43ca71e-295e-417b-94db-3a24fe84629d	true	access.token.claim
a43ca71e-295e-417b-94db-3a24fe84629d	picture	claim.name
a43ca71e-295e-417b-94db-3a24fe84629d	String	jsonType.label
ad8fc644-3245-4ec4-b8e3-3b1b1572eefa	true	introspection.token.claim
ad8fc644-3245-4ec4-b8e3-3b1b1572eefa	true	userinfo.token.claim
ad8fc644-3245-4ec4-b8e3-3b1b1572eefa	profile	user.attribute
ad8fc644-3245-4ec4-b8e3-3b1b1572eefa	true	id.token.claim
ad8fc644-3245-4ec4-b8e3-3b1b1572eefa	true	access.token.claim
ad8fc644-3245-4ec4-b8e3-3b1b1572eefa	profile	claim.name
ad8fc644-3245-4ec4-b8e3-3b1b1572eefa	String	jsonType.label
b175af8d-7c62-4408-a2aa-de5d291bef1d	true	introspection.token.claim
b175af8d-7c62-4408-a2aa-de5d291bef1d	true	userinfo.token.claim
b175af8d-7c62-4408-a2aa-de5d291bef1d	firstName	user.attribute
b175af8d-7c62-4408-a2aa-de5d291bef1d	true	id.token.claim
b175af8d-7c62-4408-a2aa-de5d291bef1d	true	access.token.claim
b175af8d-7c62-4408-a2aa-de5d291bef1d	given_name	claim.name
b175af8d-7c62-4408-a2aa-de5d291bef1d	String	jsonType.label
c0a306a9-1bd7-4d67-ba41-3d9acb35865d	true	introspection.token.claim
c0a306a9-1bd7-4d67-ba41-3d9acb35865d	true	userinfo.token.claim
c0a306a9-1bd7-4d67-ba41-3d9acb35865d	gender	user.attribute
c0a306a9-1bd7-4d67-ba41-3d9acb35865d	true	id.token.claim
c0a306a9-1bd7-4d67-ba41-3d9acb35865d	true	access.token.claim
c0a306a9-1bd7-4d67-ba41-3d9acb35865d	gender	claim.name
c0a306a9-1bd7-4d67-ba41-3d9acb35865d	String	jsonType.label
e7cff1ab-d714-4e87-9f59-114dc090ac48	true	introspection.token.claim
e7cff1ab-d714-4e87-9f59-114dc090ac48	true	userinfo.token.claim
e7cff1ab-d714-4e87-9f59-114dc090ac48	lastName	user.attribute
e7cff1ab-d714-4e87-9f59-114dc090ac48	true	id.token.claim
e7cff1ab-d714-4e87-9f59-114dc090ac48	true	access.token.claim
e7cff1ab-d714-4e87-9f59-114dc090ac48	family_name	claim.name
e7cff1ab-d714-4e87-9f59-114dc090ac48	String	jsonType.label
eb171c96-7443-433e-a35e-26c94cb32727	true	introspection.token.claim
eb171c96-7443-433e-a35e-26c94cb32727	true	userinfo.token.claim
eb171c96-7443-433e-a35e-26c94cb32727	zoneinfo	user.attribute
eb171c96-7443-433e-a35e-26c94cb32727	true	id.token.claim
eb171c96-7443-433e-a35e-26c94cb32727	true	access.token.claim
eb171c96-7443-433e-a35e-26c94cb32727	zoneinfo	claim.name
eb171c96-7443-433e-a35e-26c94cb32727	String	jsonType.label
2ddb9c43-c9fd-4c9c-8667-dfaa3805b900	true	introspection.token.claim
2ddb9c43-c9fd-4c9c-8667-dfaa3805b900	true	userinfo.token.claim
2ddb9c43-c9fd-4c9c-8667-dfaa3805b900	email	user.attribute
2ddb9c43-c9fd-4c9c-8667-dfaa3805b900	true	id.token.claim
2ddb9c43-c9fd-4c9c-8667-dfaa3805b900	true	access.token.claim
2ddb9c43-c9fd-4c9c-8667-dfaa3805b900	email	claim.name
2ddb9c43-c9fd-4c9c-8667-dfaa3805b900	String	jsonType.label
36128397-81ab-4071-bb13-de7a2a2b18bd	true	introspection.token.claim
36128397-81ab-4071-bb13-de7a2a2b18bd	true	userinfo.token.claim
36128397-81ab-4071-bb13-de7a2a2b18bd	emailVerified	user.attribute
36128397-81ab-4071-bb13-de7a2a2b18bd	true	id.token.claim
36128397-81ab-4071-bb13-de7a2a2b18bd	true	access.token.claim
36128397-81ab-4071-bb13-de7a2a2b18bd	email_verified	claim.name
36128397-81ab-4071-bb13-de7a2a2b18bd	boolean	jsonType.label
d12697cf-543e-49b5-9b94-d69c0cc14d87	formatted	user.attribute.formatted
d12697cf-543e-49b5-9b94-d69c0cc14d87	country	user.attribute.country
d12697cf-543e-49b5-9b94-d69c0cc14d87	true	introspection.token.claim
d12697cf-543e-49b5-9b94-d69c0cc14d87	postal_code	user.attribute.postal_code
d12697cf-543e-49b5-9b94-d69c0cc14d87	true	userinfo.token.claim
d12697cf-543e-49b5-9b94-d69c0cc14d87	street	user.attribute.street
d12697cf-543e-49b5-9b94-d69c0cc14d87	true	id.token.claim
d12697cf-543e-49b5-9b94-d69c0cc14d87	region	user.attribute.region
d12697cf-543e-49b5-9b94-d69c0cc14d87	true	access.token.claim
d12697cf-543e-49b5-9b94-d69c0cc14d87	locality	user.attribute.locality
7049f7f7-3737-4e05-8cc2-ea95a9f28edf	true	introspection.token.claim
7049f7f7-3737-4e05-8cc2-ea95a9f28edf	true	userinfo.token.claim
7049f7f7-3737-4e05-8cc2-ea95a9f28edf	phoneNumberVerified	user.attribute
7049f7f7-3737-4e05-8cc2-ea95a9f28edf	true	id.token.claim
7049f7f7-3737-4e05-8cc2-ea95a9f28edf	true	access.token.claim
7049f7f7-3737-4e05-8cc2-ea95a9f28edf	phone_number_verified	claim.name
7049f7f7-3737-4e05-8cc2-ea95a9f28edf	boolean	jsonType.label
86e87ba7-eb3d-457c-a21a-bf9bd7e807f1	true	introspection.token.claim
86e87ba7-eb3d-457c-a21a-bf9bd7e807f1	true	userinfo.token.claim
86e87ba7-eb3d-457c-a21a-bf9bd7e807f1	phoneNumber	user.attribute
86e87ba7-eb3d-457c-a21a-bf9bd7e807f1	true	id.token.claim
86e87ba7-eb3d-457c-a21a-bf9bd7e807f1	true	access.token.claim
86e87ba7-eb3d-457c-a21a-bf9bd7e807f1	phone_number	claim.name
86e87ba7-eb3d-457c-a21a-bf9bd7e807f1	String	jsonType.label
8d9a8241-e859-4ec9-aeb9-366f64fc5b4e	true	introspection.token.claim
8d9a8241-e859-4ec9-aeb9-366f64fc5b4e	true	access.token.claim
977d901c-cd00-4d66-b418-1997ee721f43	true	introspection.token.claim
977d901c-cd00-4d66-b418-1997ee721f43	true	multivalued
977d901c-cd00-4d66-b418-1997ee721f43	foo	user.attribute
977d901c-cd00-4d66-b418-1997ee721f43	true	access.token.claim
977d901c-cd00-4d66-b418-1997ee721f43	realm_access.roles	claim.name
977d901c-cd00-4d66-b418-1997ee721f43	String	jsonType.label
ce7051e3-1cde-4be6-9158-866bdce8f6be	true	introspection.token.claim
ce7051e3-1cde-4be6-9158-866bdce8f6be	true	multivalued
ce7051e3-1cde-4be6-9158-866bdce8f6be	foo	user.attribute
ce7051e3-1cde-4be6-9158-866bdce8f6be	true	access.token.claim
ce7051e3-1cde-4be6-9158-866bdce8f6be	resource_access.${client_id}.roles	claim.name
ce7051e3-1cde-4be6-9158-866bdce8f6be	String	jsonType.label
36925b0f-92c2-4180-80d4-e9ae2029434d	true	introspection.token.claim
36925b0f-92c2-4180-80d4-e9ae2029434d	true	access.token.claim
751f9d76-99e0-4fe7-84e9-5c598e9a97aa	true	introspection.token.claim
751f9d76-99e0-4fe7-84e9-5c598e9a97aa	true	userinfo.token.claim
751f9d76-99e0-4fe7-84e9-5c598e9a97aa	username	user.attribute
751f9d76-99e0-4fe7-84e9-5c598e9a97aa	true	id.token.claim
751f9d76-99e0-4fe7-84e9-5c598e9a97aa	true	access.token.claim
751f9d76-99e0-4fe7-84e9-5c598e9a97aa	upn	claim.name
751f9d76-99e0-4fe7-84e9-5c598e9a97aa	String	jsonType.label
d2d6548d-60d2-40c8-8013-b0574b77df11	true	introspection.token.claim
d2d6548d-60d2-40c8-8013-b0574b77df11	true	multivalued
d2d6548d-60d2-40c8-8013-b0574b77df11	foo	user.attribute
d2d6548d-60d2-40c8-8013-b0574b77df11	true	id.token.claim
d2d6548d-60d2-40c8-8013-b0574b77df11	true	access.token.claim
d2d6548d-60d2-40c8-8013-b0574b77df11	groups	claim.name
d2d6548d-60d2-40c8-8013-b0574b77df11	String	jsonType.label
96c2622b-db5e-4537-a3e0-40a1758145b8	true	introspection.token.claim
96c2622b-db5e-4537-a3e0-40a1758145b8	true	id.token.claim
96c2622b-db5e-4537-a3e0-40a1758145b8	true	access.token.claim
0ec037b9-d897-461f-a59e-0579b9478090	true	introspection.token.claim
0ec037b9-d897-461f-a59e-0579b9478090	true	access.token.claim
3aabe97f-cbc1-4bdb-ac51-9fe6963f5211	AUTH_TIME	user.session.note
3aabe97f-cbc1-4bdb-ac51-9fe6963f5211	true	introspection.token.claim
3aabe97f-cbc1-4bdb-ac51-9fe6963f5211	true	id.token.claim
3aabe97f-cbc1-4bdb-ac51-9fe6963f5211	true	access.token.claim
3aabe97f-cbc1-4bdb-ac51-9fe6963f5211	auth_time	claim.name
3aabe97f-cbc1-4bdb-ac51-9fe6963f5211	long	jsonType.label
61a0f60d-9f30-433e-9616-d0e7f73dd6ef	clientHost	user.session.note
61a0f60d-9f30-433e-9616-d0e7f73dd6ef	true	introspection.token.claim
61a0f60d-9f30-433e-9616-d0e7f73dd6ef	true	id.token.claim
61a0f60d-9f30-433e-9616-d0e7f73dd6ef	true	access.token.claim
61a0f60d-9f30-433e-9616-d0e7f73dd6ef	clientHost	claim.name
61a0f60d-9f30-433e-9616-d0e7f73dd6ef	String	jsonType.label
b4914135-3088-4dd3-87ea-6243862f973e	clientAddress	user.session.note
b4914135-3088-4dd3-87ea-6243862f973e	true	introspection.token.claim
b4914135-3088-4dd3-87ea-6243862f973e	true	id.token.claim
b4914135-3088-4dd3-87ea-6243862f973e	true	access.token.claim
b4914135-3088-4dd3-87ea-6243862f973e	clientAddress	claim.name
b4914135-3088-4dd3-87ea-6243862f973e	String	jsonType.label
f6907f92-ccca-4253-b90e-b0fa79f68e08	client_id	user.session.note
f6907f92-ccca-4253-b90e-b0fa79f68e08	true	introspection.token.claim
f6907f92-ccca-4253-b90e-b0fa79f68e08	true	id.token.claim
f6907f92-ccca-4253-b90e-b0fa79f68e08	true	access.token.claim
f6907f92-ccca-4253-b90e-b0fa79f68e08	client_id	claim.name
f6907f92-ccca-4253-b90e-b0fa79f68e08	String	jsonType.label
63babe4b-a402-4e35-afed-57cad831d9b1	true	introspection.token.claim
63babe4b-a402-4e35-afed-57cad831d9b1	true	multivalued
63babe4b-a402-4e35-afed-57cad831d9b1	true	id.token.claim
63babe4b-a402-4e35-afed-57cad831d9b1	true	access.token.claim
63babe4b-a402-4e35-afed-57cad831d9b1	organization	claim.name
63babe4b-a402-4e35-afed-57cad831d9b1	String	jsonType.label
0d031525-33bf-4372-865c-e817ae5c8f40	false	single
0d031525-33bf-4372-865c-e817ae5c8f40	Basic	attribute.nameformat
0d031525-33bf-4372-865c-e817ae5c8f40	Role	attribute.name
0a2645eb-7075-41f1-9e96-335015a9c640	true	introspection.token.claim
0a2645eb-7075-41f1-9e96-335015a9c640	true	userinfo.token.claim
0a2645eb-7075-41f1-9e96-335015a9c640	profile	user.attribute
0a2645eb-7075-41f1-9e96-335015a9c640	true	id.token.claim
0a2645eb-7075-41f1-9e96-335015a9c640	true	access.token.claim
0a2645eb-7075-41f1-9e96-335015a9c640	profile	claim.name
0a2645eb-7075-41f1-9e96-335015a9c640	String	jsonType.label
16b8817c-3918-4efe-9b71-9ee9f172a1a8	true	introspection.token.claim
16b8817c-3918-4efe-9b71-9ee9f172a1a8	true	userinfo.token.claim
16b8817c-3918-4efe-9b71-9ee9f172a1a8	birthdate	user.attribute
16b8817c-3918-4efe-9b71-9ee9f172a1a8	true	id.token.claim
16b8817c-3918-4efe-9b71-9ee9f172a1a8	true	access.token.claim
16b8817c-3918-4efe-9b71-9ee9f172a1a8	birthdate	claim.name
16b8817c-3918-4efe-9b71-9ee9f172a1a8	String	jsonType.label
44e03518-4b15-4bdd-a053-5da57c2abd4b	true	introspection.token.claim
44e03518-4b15-4bdd-a053-5da57c2abd4b	true	userinfo.token.claim
44e03518-4b15-4bdd-a053-5da57c2abd4b	picture	user.attribute
44e03518-4b15-4bdd-a053-5da57c2abd4b	true	id.token.claim
44e03518-4b15-4bdd-a053-5da57c2abd4b	true	access.token.claim
44e03518-4b15-4bdd-a053-5da57c2abd4b	picture	claim.name
44e03518-4b15-4bdd-a053-5da57c2abd4b	String	jsonType.label
6c8375ed-46c5-4e54-bf3e-b63359c6d652	true	introspection.token.claim
6c8375ed-46c5-4e54-bf3e-b63359c6d652	true	userinfo.token.claim
6c8375ed-46c5-4e54-bf3e-b63359c6d652	gender	user.attribute
6c8375ed-46c5-4e54-bf3e-b63359c6d652	true	id.token.claim
6c8375ed-46c5-4e54-bf3e-b63359c6d652	true	access.token.claim
6c8375ed-46c5-4e54-bf3e-b63359c6d652	gender	claim.name
6c8375ed-46c5-4e54-bf3e-b63359c6d652	String	jsonType.label
8c176027-1970-49f6-b4f9-ed136e4a6aa0	true	introspection.token.claim
8c176027-1970-49f6-b4f9-ed136e4a6aa0	true	userinfo.token.claim
8c176027-1970-49f6-b4f9-ed136e4a6aa0	true	id.token.claim
8c176027-1970-49f6-b4f9-ed136e4a6aa0	true	access.token.claim
8f09a57c-0a75-4687-8f64-4f498d6eaf02	true	introspection.token.claim
8f09a57c-0a75-4687-8f64-4f498d6eaf02	true	userinfo.token.claim
8f09a57c-0a75-4687-8f64-4f498d6eaf02	website	user.attribute
8f09a57c-0a75-4687-8f64-4f498d6eaf02	true	id.token.claim
8f09a57c-0a75-4687-8f64-4f498d6eaf02	true	access.token.claim
8f09a57c-0a75-4687-8f64-4f498d6eaf02	website	claim.name
8f09a57c-0a75-4687-8f64-4f498d6eaf02	String	jsonType.label
96bbf2cb-93cc-4c5f-aa1f-48d35e2c465d	true	introspection.token.claim
96bbf2cb-93cc-4c5f-aa1f-48d35e2c465d	true	userinfo.token.claim
96bbf2cb-93cc-4c5f-aa1f-48d35e2c465d	zoneinfo	user.attribute
96bbf2cb-93cc-4c5f-aa1f-48d35e2c465d	true	id.token.claim
96bbf2cb-93cc-4c5f-aa1f-48d35e2c465d	true	access.token.claim
96bbf2cb-93cc-4c5f-aa1f-48d35e2c465d	zoneinfo	claim.name
96bbf2cb-93cc-4c5f-aa1f-48d35e2c465d	String	jsonType.label
bf2bb774-97a0-4149-bbe4-8076676f19f5	true	introspection.token.claim
bf2bb774-97a0-4149-bbe4-8076676f19f5	true	userinfo.token.claim
bf2bb774-97a0-4149-bbe4-8076676f19f5	nickname	user.attribute
bf2bb774-97a0-4149-bbe4-8076676f19f5	true	id.token.claim
bf2bb774-97a0-4149-bbe4-8076676f19f5	true	access.token.claim
bf2bb774-97a0-4149-bbe4-8076676f19f5	nickname	claim.name
bf2bb774-97a0-4149-bbe4-8076676f19f5	String	jsonType.label
c6dc8a27-9cbb-4c0a-b789-efc6087a8969	true	introspection.token.claim
c6dc8a27-9cbb-4c0a-b789-efc6087a8969	true	userinfo.token.claim
c6dc8a27-9cbb-4c0a-b789-efc6087a8969	locale	user.attribute
c6dc8a27-9cbb-4c0a-b789-efc6087a8969	true	id.token.claim
c6dc8a27-9cbb-4c0a-b789-efc6087a8969	true	access.token.claim
c6dc8a27-9cbb-4c0a-b789-efc6087a8969	locale	claim.name
c6dc8a27-9cbb-4c0a-b789-efc6087a8969	String	jsonType.label
d0ed7ce7-3bbd-4892-9474-c0861fa7ca9e	true	introspection.token.claim
d0ed7ce7-3bbd-4892-9474-c0861fa7ca9e	true	userinfo.token.claim
d0ed7ce7-3bbd-4892-9474-c0861fa7ca9e	firstName	user.attribute
d0ed7ce7-3bbd-4892-9474-c0861fa7ca9e	true	id.token.claim
d0ed7ce7-3bbd-4892-9474-c0861fa7ca9e	true	access.token.claim
d0ed7ce7-3bbd-4892-9474-c0861fa7ca9e	given_name	claim.name
d0ed7ce7-3bbd-4892-9474-c0861fa7ca9e	String	jsonType.label
d40c931b-9de6-402d-a9d8-15e0c3d1e818	true	introspection.token.claim
d40c931b-9de6-402d-a9d8-15e0c3d1e818	true	userinfo.token.claim
d40c931b-9de6-402d-a9d8-15e0c3d1e818	lastName	user.attribute
d40c931b-9de6-402d-a9d8-15e0c3d1e818	true	id.token.claim
d40c931b-9de6-402d-a9d8-15e0c3d1e818	true	access.token.claim
d40c931b-9de6-402d-a9d8-15e0c3d1e818	family_name	claim.name
d40c931b-9de6-402d-a9d8-15e0c3d1e818	String	jsonType.label
e1ee7e12-ac6c-43e8-9c4d-edb0accd799a	true	introspection.token.claim
e1ee7e12-ac6c-43e8-9c4d-edb0accd799a	true	userinfo.token.claim
e1ee7e12-ac6c-43e8-9c4d-edb0accd799a	updatedAt	user.attribute
e1ee7e12-ac6c-43e8-9c4d-edb0accd799a	true	id.token.claim
e1ee7e12-ac6c-43e8-9c4d-edb0accd799a	true	access.token.claim
e1ee7e12-ac6c-43e8-9c4d-edb0accd799a	updated_at	claim.name
e1ee7e12-ac6c-43e8-9c4d-edb0accd799a	long	jsonType.label
f8043730-bc96-4d12-965f-f592cc69d967	true	introspection.token.claim
f8043730-bc96-4d12-965f-f592cc69d967	true	userinfo.token.claim
f8043730-bc96-4d12-965f-f592cc69d967	username	user.attribute
f8043730-bc96-4d12-965f-f592cc69d967	true	id.token.claim
f8043730-bc96-4d12-965f-f592cc69d967	true	access.token.claim
f8043730-bc96-4d12-965f-f592cc69d967	preferred_username	claim.name
f8043730-bc96-4d12-965f-f592cc69d967	String	jsonType.label
fb809382-8aff-46f7-ad11-cf921f833068	true	introspection.token.claim
fb809382-8aff-46f7-ad11-cf921f833068	true	userinfo.token.claim
fb809382-8aff-46f7-ad11-cf921f833068	middleName	user.attribute
fb809382-8aff-46f7-ad11-cf921f833068	true	id.token.claim
fb809382-8aff-46f7-ad11-cf921f833068	true	access.token.claim
fb809382-8aff-46f7-ad11-cf921f833068	middle_name	claim.name
fb809382-8aff-46f7-ad11-cf921f833068	String	jsonType.label
e0519651-be6d-4a58-ad20-6731b20cd993	true	introspection.token.claim
e0519651-be6d-4a58-ad20-6731b20cd993	true	userinfo.token.claim
e0519651-be6d-4a58-ad20-6731b20cd993	email	user.attribute
e0519651-be6d-4a58-ad20-6731b20cd993	true	id.token.claim
e0519651-be6d-4a58-ad20-6731b20cd993	true	access.token.claim
e0519651-be6d-4a58-ad20-6731b20cd993	email	claim.name
e0519651-be6d-4a58-ad20-6731b20cd993	String	jsonType.label
f7e776fd-fdc6-4af2-a755-89fcbe1cb912	true	introspection.token.claim
f7e776fd-fdc6-4af2-a755-89fcbe1cb912	true	userinfo.token.claim
f7e776fd-fdc6-4af2-a755-89fcbe1cb912	emailVerified	user.attribute
f7e776fd-fdc6-4af2-a755-89fcbe1cb912	true	id.token.claim
f7e776fd-fdc6-4af2-a755-89fcbe1cb912	true	access.token.claim
f7e776fd-fdc6-4af2-a755-89fcbe1cb912	email_verified	claim.name
f7e776fd-fdc6-4af2-a755-89fcbe1cb912	boolean	jsonType.label
656f431f-a11c-475f-9359-0ef1a7a49b9c	formatted	user.attribute.formatted
656f431f-a11c-475f-9359-0ef1a7a49b9c	country	user.attribute.country
656f431f-a11c-475f-9359-0ef1a7a49b9c	true	introspection.token.claim
656f431f-a11c-475f-9359-0ef1a7a49b9c	postal_code	user.attribute.postal_code
656f431f-a11c-475f-9359-0ef1a7a49b9c	true	userinfo.token.claim
656f431f-a11c-475f-9359-0ef1a7a49b9c	street	user.attribute.street
656f431f-a11c-475f-9359-0ef1a7a49b9c	true	id.token.claim
656f431f-a11c-475f-9359-0ef1a7a49b9c	region	user.attribute.region
656f431f-a11c-475f-9359-0ef1a7a49b9c	true	access.token.claim
656f431f-a11c-475f-9359-0ef1a7a49b9c	locality	user.attribute.locality
2a8c3b33-15e8-421c-b186-e95ef4a21908	true	introspection.token.claim
2a8c3b33-15e8-421c-b186-e95ef4a21908	true	userinfo.token.claim
2a8c3b33-15e8-421c-b186-e95ef4a21908	phoneNumber	user.attribute
2a8c3b33-15e8-421c-b186-e95ef4a21908	true	id.token.claim
2a8c3b33-15e8-421c-b186-e95ef4a21908	true	access.token.claim
2a8c3b33-15e8-421c-b186-e95ef4a21908	phone_number	claim.name
2a8c3b33-15e8-421c-b186-e95ef4a21908	String	jsonType.label
69c49ab4-d57e-47ee-b5f3-fc30d921cfe7	true	introspection.token.claim
69c49ab4-d57e-47ee-b5f3-fc30d921cfe7	true	userinfo.token.claim
69c49ab4-d57e-47ee-b5f3-fc30d921cfe7	phoneNumberVerified	user.attribute
69c49ab4-d57e-47ee-b5f3-fc30d921cfe7	true	id.token.claim
69c49ab4-d57e-47ee-b5f3-fc30d921cfe7	true	access.token.claim
69c49ab4-d57e-47ee-b5f3-fc30d921cfe7	phone_number_verified	claim.name
69c49ab4-d57e-47ee-b5f3-fc30d921cfe7	boolean	jsonType.label
1f47b622-574f-491a-93b7-e324f7b41828	true	introspection.token.claim
1f47b622-574f-491a-93b7-e324f7b41828	true	multivalued
1f47b622-574f-491a-93b7-e324f7b41828	foo	user.attribute
1f47b622-574f-491a-93b7-e324f7b41828	true	access.token.claim
1f47b622-574f-491a-93b7-e324f7b41828	resource_access.${client_id}.roles	claim.name
1f47b622-574f-491a-93b7-e324f7b41828	String	jsonType.label
44e9e2b0-f300-46a9-9473-001dce537ea0	true	introspection.token.claim
44e9e2b0-f300-46a9-9473-001dce537ea0	true	access.token.claim
be610505-b9d2-493d-9135-212338257b2e	true	introspection.token.claim
be610505-b9d2-493d-9135-212338257b2e	true	multivalued
be610505-b9d2-493d-9135-212338257b2e	foo	user.attribute
be610505-b9d2-493d-9135-212338257b2e	true	access.token.claim
be610505-b9d2-493d-9135-212338257b2e	realm_access.roles	claim.name
be610505-b9d2-493d-9135-212338257b2e	String	jsonType.label
28eddf9e-235f-433d-bea6-a4d786a6b2ae	true	introspection.token.claim
28eddf9e-235f-433d-bea6-a4d786a6b2ae	true	access.token.claim
3167b204-ef95-425c-b3c3-133bb374badb	true	introspection.token.claim
3167b204-ef95-425c-b3c3-133bb374badb	true	multivalued
3167b204-ef95-425c-b3c3-133bb374badb	foo	user.attribute
3167b204-ef95-425c-b3c3-133bb374badb	true	id.token.claim
3167b204-ef95-425c-b3c3-133bb374badb	true	access.token.claim
3167b204-ef95-425c-b3c3-133bb374badb	groups	claim.name
3167b204-ef95-425c-b3c3-133bb374badb	String	jsonType.label
99c5f74c-1b4f-4b32-bc1c-8da81f6a229a	true	introspection.token.claim
99c5f74c-1b4f-4b32-bc1c-8da81f6a229a	true	userinfo.token.claim
99c5f74c-1b4f-4b32-bc1c-8da81f6a229a	username	user.attribute
99c5f74c-1b4f-4b32-bc1c-8da81f6a229a	true	id.token.claim
99c5f74c-1b4f-4b32-bc1c-8da81f6a229a	true	access.token.claim
99c5f74c-1b4f-4b32-bc1c-8da81f6a229a	upn	claim.name
99c5f74c-1b4f-4b32-bc1c-8da81f6a229a	String	jsonType.label
b0e402e4-cc42-4414-801e-204b2f9f9e4c	true	introspection.token.claim
b0e402e4-cc42-4414-801e-204b2f9f9e4c	true	id.token.claim
b0e402e4-cc42-4414-801e-204b2f9f9e4c	true	access.token.claim
57ba6737-ba69-4632-8890-a1e1d38c562a	true	introspection.token.claim
57ba6737-ba69-4632-8890-a1e1d38c562a	true	access.token.claim
fc8e99e5-f440-4640-93f8-b0675d65aca3	AUTH_TIME	user.session.note
fc8e99e5-f440-4640-93f8-b0675d65aca3	true	introspection.token.claim
fc8e99e5-f440-4640-93f8-b0675d65aca3	true	id.token.claim
fc8e99e5-f440-4640-93f8-b0675d65aca3	true	access.token.claim
fc8e99e5-f440-4640-93f8-b0675d65aca3	auth_time	claim.name
fc8e99e5-f440-4640-93f8-b0675d65aca3	long	jsonType.label
2acb1b68-a197-4f02-bd63-ddfdf903a151	clientAddress	user.session.note
2acb1b68-a197-4f02-bd63-ddfdf903a151	true	introspection.token.claim
2acb1b68-a197-4f02-bd63-ddfdf903a151	true	id.token.claim
2acb1b68-a197-4f02-bd63-ddfdf903a151	true	access.token.claim
2acb1b68-a197-4f02-bd63-ddfdf903a151	clientAddress	claim.name
2acb1b68-a197-4f02-bd63-ddfdf903a151	String	jsonType.label
f35e9a13-74ad-4506-b918-6ccf27b57586	clientHost	user.session.note
f35e9a13-74ad-4506-b918-6ccf27b57586	true	introspection.token.claim
f35e9a13-74ad-4506-b918-6ccf27b57586	true	id.token.claim
f35e9a13-74ad-4506-b918-6ccf27b57586	true	access.token.claim
f35e9a13-74ad-4506-b918-6ccf27b57586	clientHost	claim.name
f35e9a13-74ad-4506-b918-6ccf27b57586	String	jsonType.label
fb49e089-d912-409f-9d86-b2309c4afa2f	client_id	user.session.note
fb49e089-d912-409f-9d86-b2309c4afa2f	true	introspection.token.claim
fb49e089-d912-409f-9d86-b2309c4afa2f	true	id.token.claim
fb49e089-d912-409f-9d86-b2309c4afa2f	true	access.token.claim
fb49e089-d912-409f-9d86-b2309c4afa2f	client_id	claim.name
fb49e089-d912-409f-9d86-b2309c4afa2f	String	jsonType.label
a0113ab2-1031-4b80-a5f9-d393331c4d02	true	introspection.token.claim
a0113ab2-1031-4b80-a5f9-d393331c4d02	true	multivalued
a0113ab2-1031-4b80-a5f9-d393331c4d02	true	id.token.claim
a0113ab2-1031-4b80-a5f9-d393331c4d02	true	access.token.claim
a0113ab2-1031-4b80-a5f9-d393331c4d02	organization	claim.name
a0113ab2-1031-4b80-a5f9-d393331c4d02	String	jsonType.label
c15447a3-1a10-4925-81ed-5823834b4d69	true	introspection.token.claim
c15447a3-1a10-4925-81ed-5823834b4d69	true	userinfo.token.claim
c15447a3-1a10-4925-81ed-5823834b4d69	locale	user.attribute
c15447a3-1a10-4925-81ed-5823834b4d69	true	id.token.claim
c15447a3-1a10-4925-81ed-5823834b4d69	true	access.token.claim
c15447a3-1a10-4925-81ed-5823834b4d69	locale	claim.name
c15447a3-1a10-4925-81ed-5823834b4d69	String	jsonType.label
e1b67e77-9ddd-4c0c-972d-5e52f6003df3	false	id.token.claim
e1b67e77-9ddd-4c0c-972d-5e52f6003df3	false	lightweight.claim
e1b67e77-9ddd-4c0c-972d-5e52f6003df3	true	access.token.claim
e1b67e77-9ddd-4c0c-972d-5e52f6003df3	true	introspection.token.claim
e1b67e77-9ddd-4c0c-972d-5e52f6003df3	false	userinfo.token.claim
e1b67e77-9ddd-4c0c-972d-5e52f6003df3	bambaiba-api	included.custom.audience
e1b67e77-9ddd-4c0c-972d-5e52f6003df3	bambaiba-api	included.client.audience
\.


--
-- TOC entry 4139 (class 0 OID 24631)
-- Dependencies: 222
-- Data for Name: realm; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.realm (id, access_code_lifespan, user_action_lifespan, access_token_lifespan, account_theme, admin_theme, email_theme, enabled, events_enabled, events_expiration, login_theme, name, not_before, password_policy, registration_allowed, remember_me, reset_password_allowed, social, ssl_required, sso_idle_timeout, sso_max_lifespan, update_profile_on_soc_login, verify_email, master_admin_client, login_lifespan, internationalization_enabled, default_locale, reg_email_as_username, admin_events_enabled, admin_events_details_enabled, edit_username_allowed, otp_policy_counter, otp_policy_window, otp_policy_period, otp_policy_digits, otp_policy_alg, otp_policy_type, browser_flow, registration_flow, direct_grant_flow, reset_credentials_flow, client_auth_flow, offline_session_idle_timeout, revoke_refresh_token, access_token_life_implicit, login_with_email_allowed, duplicate_emails_allowed, docker_auth_flow, refresh_token_max_reuse, allow_user_managed_access, sso_max_lifespan_remember_me, sso_idle_timeout_remember_me, default_role) FROM stdin;
9ccc2b44-8d11-4694-87e4-8e194b225e1d	60	300	60	\N	\N	\N	t	f	0	\N	master	0	\N	f	f	f	f	EXTERNAL	1800	36000	f	f	9f3e1992-eb18-490a-a7af-2ddc4d947be5	1800	f	\N	f	f	f	f	0	1	30	6	HmacSHA1	totp	9e4ca14f-c769-445f-9823-0ee4e7f5995f	eeb5d5fc-01af-4213-a5a3-cbd285fb578b	8bbfc34c-d8d9-48c1-b79a-f955cb70b8bb	37d5edc9-7089-4b85-ac73-d99fb431baf4	f11b41ec-6388-4979-b2e9-7cf504fcb9ca	2592000	f	900	t	f	3ad6bc93-f13c-4f59-9680-33b92c2d92b8	0	f	0	0	333d5b0e-b35b-447c-849f-6d1954882d54
22478864-d5ad-4829-b2bf-92ed9d9ca9c8	60	300	300	\N	\N	\N	t	f	0	\N	bambaiba	0	\N	f	f	f	f	EXTERNAL	1800	36000	f	f	8675ed84-ae64-4ead-aaf4-913e1d7d3397	1800	f	\N	f	f	f	f	0	1	30	6	HmacSHA1	totp	905d0947-4059-4fc3-a85f-30a710778cf1	e6a564ce-e81a-4447-a408-448dc4b24859	1fa01ed5-af2b-434e-a1b3-ab153785f0ef	0f75fe80-a603-4c1a-b430-4e6e306b45d0	c424c49c-aa15-4816-b9ef-efba43376b05	2592000	f	900	t	f	5bf4fd2b-7ee7-4684-9d34-82bbac5b2483	0	f	0	0	6337a65d-feea-4b7c-b5e2-6d122b88b571
\.


--
-- TOC entry 4140 (class 0 OID 24648)
-- Dependencies: 223
-- Data for Name: realm_attribute; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.realm_attribute (name, realm_id, value) FROM stdin;
_browser_header.contentSecurityPolicyReportOnly	9ccc2b44-8d11-4694-87e4-8e194b225e1d	
_browser_header.xContentTypeOptions	9ccc2b44-8d11-4694-87e4-8e194b225e1d	nosniff
_browser_header.referrerPolicy	9ccc2b44-8d11-4694-87e4-8e194b225e1d	no-referrer
_browser_header.xRobotsTag	9ccc2b44-8d11-4694-87e4-8e194b225e1d	none
_browser_header.xFrameOptions	9ccc2b44-8d11-4694-87e4-8e194b225e1d	SAMEORIGIN
_browser_header.contentSecurityPolicy	9ccc2b44-8d11-4694-87e4-8e194b225e1d	frame-src 'self'; frame-ancestors 'self'; object-src 'none';
_browser_header.xXSSProtection	9ccc2b44-8d11-4694-87e4-8e194b225e1d	1; mode=block
_browser_header.strictTransportSecurity	9ccc2b44-8d11-4694-87e4-8e194b225e1d	max-age=31536000; includeSubDomains
bruteForceProtected	9ccc2b44-8d11-4694-87e4-8e194b225e1d	false
permanentLockout	9ccc2b44-8d11-4694-87e4-8e194b225e1d	false
maxTemporaryLockouts	9ccc2b44-8d11-4694-87e4-8e194b225e1d	0
bruteForceStrategy	9ccc2b44-8d11-4694-87e4-8e194b225e1d	MULTIPLE
maxFailureWaitSeconds	9ccc2b44-8d11-4694-87e4-8e194b225e1d	900
minimumQuickLoginWaitSeconds	9ccc2b44-8d11-4694-87e4-8e194b225e1d	60
waitIncrementSeconds	9ccc2b44-8d11-4694-87e4-8e194b225e1d	60
quickLoginCheckMilliSeconds	9ccc2b44-8d11-4694-87e4-8e194b225e1d	1000
maxDeltaTimeSeconds	9ccc2b44-8d11-4694-87e4-8e194b225e1d	43200
failureFactor	9ccc2b44-8d11-4694-87e4-8e194b225e1d	30
realmReusableOtpCode	9ccc2b44-8d11-4694-87e4-8e194b225e1d	false
firstBrokerLoginFlowId	9ccc2b44-8d11-4694-87e4-8e194b225e1d	a3f4378f-2789-4495-a910-8063b5661a03
displayName	9ccc2b44-8d11-4694-87e4-8e194b225e1d	Keycloak
displayNameHtml	9ccc2b44-8d11-4694-87e4-8e194b225e1d	<div class="kc-logo-text"><span>Keycloak</span></div>
defaultSignatureAlgorithm	9ccc2b44-8d11-4694-87e4-8e194b225e1d	RS256
offlineSessionMaxLifespanEnabled	9ccc2b44-8d11-4694-87e4-8e194b225e1d	false
offlineSessionMaxLifespan	9ccc2b44-8d11-4694-87e4-8e194b225e1d	5184000
bruteForceProtected	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	false
permanentLockout	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	false
maxTemporaryLockouts	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	0
bruteForceStrategy	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	MULTIPLE
maxFailureWaitSeconds	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	900
minimumQuickLoginWaitSeconds	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	60
waitIncrementSeconds	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	60
quickLoginCheckMilliSeconds	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	1000
maxDeltaTimeSeconds	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	43200
failureFactor	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	30
realmReusableOtpCode	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	false
defaultSignatureAlgorithm	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	RS256
offlineSessionMaxLifespanEnabled	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	false
offlineSessionMaxLifespan	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	5184000
actionTokenGeneratedByAdminLifespan	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	43200
actionTokenGeneratedByUserLifespan	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	300
oauth2DeviceCodeLifespan	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	600
oauth2DevicePollingInterval	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	5
webAuthnPolicyRpEntityName	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	keycloak
webAuthnPolicySignatureAlgorithms	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	ES256,RS256
webAuthnPolicyRpId	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	
webAuthnPolicyAttestationConveyancePreference	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	not specified
webAuthnPolicyAuthenticatorAttachment	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	not specified
webAuthnPolicyRequireResidentKey	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	not specified
webAuthnPolicyUserVerificationRequirement	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	not specified
webAuthnPolicyCreateTimeout	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	0
webAuthnPolicyAvoidSameAuthenticatorRegister	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	false
webAuthnPolicyRpEntityNamePasswordless	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	keycloak
webAuthnPolicySignatureAlgorithmsPasswordless	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	ES256,RS256
webAuthnPolicyRpIdPasswordless	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	
webAuthnPolicyAttestationConveyancePreferencePasswordless	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	not specified
webAuthnPolicyAuthenticatorAttachmentPasswordless	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	not specified
webAuthnPolicyRequireResidentKeyPasswordless	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	not specified
webAuthnPolicyUserVerificationRequirementPasswordless	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	not specified
webAuthnPolicyCreateTimeoutPasswordless	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	0
webAuthnPolicyAvoidSameAuthenticatorRegisterPasswordless	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	false
cibaBackchannelTokenDeliveryMode	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	poll
cibaExpiresIn	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	120
cibaInterval	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	5
cibaAuthRequestedUserHint	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	login_hint
parRequestUriLifespan	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	60
firstBrokerLoginFlowId	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	d64f7b8b-da55-434a-93d6-45e763615e6a
frontendUrl	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	
acr.loa.map	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	{}
displayNameHtml	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	
organizationsEnabled	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	false
adminPermissionsEnabled	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	false
verifiableCredentialsEnabled	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	false
clientSessionIdleTimeout	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	0
clientSessionMaxLifespan	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	0
clientOfflineSessionIdleTimeout	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	0
clientOfflineSessionMaxLifespan	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	0
client-policies.profiles	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	{"profiles":[]}
client-policies.policies	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	{"policies":[]}
displayName	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	BambaIba
_browser_header.contentSecurityPolicyReportOnly	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	
_browser_header.xContentTypeOptions	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	nosniff
_browser_header.referrerPolicy	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	no-referrer
_browser_header.xRobotsTag	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	none
_browser_header.xFrameOptions	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	SAMEORIGIN
_browser_header.contentSecurityPolicy	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	frame-src 'self'; frame-ancestors 'self'; object-src 'none';
_browser_header.xXSSProtection	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	1; mode=block
_browser_header.strictTransportSecurity	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	max-age=31536000; includeSubDomains
\.


--
-- TOC entry 4181 (class 0 OID 25405)
-- Dependencies: 264
-- Data for Name: realm_default_groups; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.realm_default_groups (realm_id, group_id) FROM stdin;
\.


--
-- TOC entry 4161 (class 0 OID 25101)
-- Dependencies: 244
-- Data for Name: realm_enabled_event_types; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.realm_enabled_event_types (realm_id, value) FROM stdin;
\.


--
-- TOC entry 4141 (class 0 OID 24656)
-- Dependencies: 224
-- Data for Name: realm_events_listeners; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.realm_events_listeners (realm_id, value) FROM stdin;
9ccc2b44-8d11-4694-87e4-8e194b225e1d	jboss-logging
22478864-d5ad-4829-b2bf-92ed9d9ca9c8	jboss-logging
\.


--
-- TOC entry 4214 (class 0 OID 26108)
-- Dependencies: 297
-- Data for Name: realm_localizations; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.realm_localizations (realm_id, locale, texts) FROM stdin;
\.


--
-- TOC entry 4142 (class 0 OID 24659)
-- Dependencies: 225
-- Data for Name: realm_required_credential; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.realm_required_credential (type, form_label, input, secret, realm_id) FROM stdin;
password	password	t	t	9ccc2b44-8d11-4694-87e4-8e194b225e1d
password	password	t	t	22478864-d5ad-4829-b2bf-92ed9d9ca9c8
\.


--
-- TOC entry 4143 (class 0 OID 24666)
-- Dependencies: 226
-- Data for Name: realm_smtp_config; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.realm_smtp_config (realm_id, value, name) FROM stdin;
\.


--
-- TOC entry 4160 (class 0 OID 25017)
-- Dependencies: 243
-- Data for Name: realm_supported_locales; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.realm_supported_locales (realm_id, value) FROM stdin;
\.


--
-- TOC entry 4144 (class 0 OID 24676)
-- Dependencies: 227
-- Data for Name: redirect_uris; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.redirect_uris (client_id, value) FROM stdin;
80bd01e1-a0e7-45d4-9873-540a8217051d	/realms/master/account/*
9175a02f-cf18-40a3-b322-73ed00ce1729	/realms/master/account/*
6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	/admin/master/console/*
d8d4e759-1346-4d49-8cb2-8d96ba570f07	/*
7e903410-f31f-4bc8-8402-777e4538d2a1	http://localhost:18081/*
88acef04-74b2-48a9-b20a-91e8a91a6071	/*
862e68c6-d552-4480-9b9f-4d3a69704afa	/admin/bambaiba/console/*
32b64136-f72f-40be-8350-89e652c4d54f	/realms/bambaiba/account/*
660743a4-25c1-46dd-b9c1-a861be2a2d84	/realms/bambaiba/account/*
\.


--
-- TOC entry 4174 (class 0 OID 25340)
-- Dependencies: 257
-- Data for Name: required_action_config; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.required_action_config (required_action_id, value, name) FROM stdin;
\.


--
-- TOC entry 4173 (class 0 OID 25333)
-- Dependencies: 256
-- Data for Name: required_action_provider; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.required_action_provider (id, alias, name, realm_id, enabled, default_action, provider_id, priority) FROM stdin;
26341944-e536-455a-abcf-90e92a6e995b	VERIFY_EMAIL	Verify Email	9ccc2b44-8d11-4694-87e4-8e194b225e1d	t	f	VERIFY_EMAIL	50
c59e8332-3878-4b0c-87a2-3b84951e7783	UPDATE_PROFILE	Update Profile	9ccc2b44-8d11-4694-87e4-8e194b225e1d	t	f	UPDATE_PROFILE	40
e19d1d7b-a321-4465-bd37-dafa92e448e5	CONFIGURE_TOTP	Configure OTP	9ccc2b44-8d11-4694-87e4-8e194b225e1d	t	f	CONFIGURE_TOTP	10
9c27574b-e570-4c19-8e0d-01e1bc606841	UPDATE_PASSWORD	Update Password	9ccc2b44-8d11-4694-87e4-8e194b225e1d	t	f	UPDATE_PASSWORD	30
dc755600-7baa-470e-8847-4c71829d0f02	TERMS_AND_CONDITIONS	Terms and Conditions	9ccc2b44-8d11-4694-87e4-8e194b225e1d	f	f	TERMS_AND_CONDITIONS	20
ee370aac-a3d0-4c93-8ca7-e637b9de8dc2	delete_account	Delete Account	9ccc2b44-8d11-4694-87e4-8e194b225e1d	f	f	delete_account	60
13f811be-2a90-4394-a85b-68795c632bf9	delete_credential	Delete Credential	9ccc2b44-8d11-4694-87e4-8e194b225e1d	t	f	delete_credential	100
b7073e08-cf24-4326-b08e-1369c89d91e9	update_user_locale	Update User Locale	9ccc2b44-8d11-4694-87e4-8e194b225e1d	t	f	update_user_locale	1000
1533ef70-644b-465a-a4fb-b76cbd6c949f	webauthn-register	Webauthn Register	9ccc2b44-8d11-4694-87e4-8e194b225e1d	t	f	webauthn-register	70
79a974d4-68e8-4d97-baf5-eb7957223223	webauthn-register-passwordless	Webauthn Register Passwordless	9ccc2b44-8d11-4694-87e4-8e194b225e1d	t	f	webauthn-register-passwordless	80
70a5300f-17a2-4dbc-b842-08994de4d56b	VERIFY_PROFILE	Verify Profile	9ccc2b44-8d11-4694-87e4-8e194b225e1d	t	f	VERIFY_PROFILE	90
6f5c46ce-b329-4c08-8a5d-9c61cdf81af6	VERIFY_EMAIL	Verify Email	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	t	f	VERIFY_EMAIL	50
92c4e393-2ca8-4b21-905e-e4c060c8ae06	UPDATE_PROFILE	Update Profile	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	t	f	UPDATE_PROFILE	40
ee504c18-6016-43b9-b0ae-98c4c76729b1	CONFIGURE_TOTP	Configure OTP	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	t	f	CONFIGURE_TOTP	10
70f6008c-a899-4f52-a747-51e2354740ce	UPDATE_PASSWORD	Update Password	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	t	f	UPDATE_PASSWORD	30
245f6acb-4fe7-48ff-ae43-10b59c86691c	TERMS_AND_CONDITIONS	Terms and Conditions	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f	f	TERMS_AND_CONDITIONS	20
d77c73ad-8a3b-4a2a-98ce-fdf75ace8356	delete_account	Delete Account	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	f	f	delete_account	60
3399d45f-887a-43b3-abdc-1f28ef802c50	delete_credential	Delete Credential	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	t	f	delete_credential	100
1ebff633-aec1-4c70-bb75-d3fa95899e57	update_user_locale	Update User Locale	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	t	f	update_user_locale	1000
ec32828e-986f-4bcf-aa70-3ad70ce27762	webauthn-register	Webauthn Register	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	t	f	webauthn-register	70
37e6a0e9-a40d-4460-b4d1-609c4209fe78	webauthn-register-passwordless	Webauthn Register Passwordless	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	t	f	webauthn-register-passwordless	80
2a356cae-9605-452c-a956-98431d100eee	VERIFY_PROFILE	Verify Profile	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	t	f	VERIFY_PROFILE	90
\.


--
-- TOC entry 4211 (class 0 OID 26039)
-- Dependencies: 294
-- Data for Name: resource_attribute; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.resource_attribute (id, name, value, resource_id) FROM stdin;
\.


--
-- TOC entry 4191 (class 0 OID 25622)
-- Dependencies: 274
-- Data for Name: resource_policy; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.resource_policy (resource_id, policy_id) FROM stdin;
\.


--
-- TOC entry 4190 (class 0 OID 25607)
-- Dependencies: 273
-- Data for Name: resource_scope; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.resource_scope (resource_id, scope_id) FROM stdin;
\.


--
-- TOC entry 4185 (class 0 OID 25545)
-- Dependencies: 268
-- Data for Name: resource_server; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.resource_server (id, allow_rs_remote_mgmt, policy_enforce_mode, decision_strategy) FROM stdin;
\.


--
-- TOC entry 4210 (class 0 OID 26015)
-- Dependencies: 293
-- Data for Name: resource_server_perm_ticket; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.resource_server_perm_ticket (id, owner, requester, created_timestamp, granted_timestamp, resource_id, scope_id, resource_server_id, policy_id) FROM stdin;
\.


--
-- TOC entry 4188 (class 0 OID 25581)
-- Dependencies: 271
-- Data for Name: resource_server_policy; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.resource_server_policy (id, name, description, type, decision_strategy, logic, resource_server_id, owner) FROM stdin;
\.


--
-- TOC entry 4186 (class 0 OID 25553)
-- Dependencies: 269
-- Data for Name: resource_server_resource; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.resource_server_resource (id, name, type, icon_uri, owner, resource_server_id, owner_managed_access, display_name) FROM stdin;
\.


--
-- TOC entry 4187 (class 0 OID 25567)
-- Dependencies: 270
-- Data for Name: resource_server_scope; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.resource_server_scope (id, name, icon_uri, resource_server_id, display_name) FROM stdin;
\.


--
-- TOC entry 4212 (class 0 OID 26057)
-- Dependencies: 295
-- Data for Name: resource_uris; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.resource_uris (resource_id, value) FROM stdin;
\.


--
-- TOC entry 4217 (class 0 OID 26193)
-- Dependencies: 300
-- Data for Name: revoked_token; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.revoked_token (id, expire) FROM stdin;
\.


--
-- TOC entry 4213 (class 0 OID 26067)
-- Dependencies: 296
-- Data for Name: role_attribute; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.role_attribute (id, role_id, name, value) FROM stdin;
\.


--
-- TOC entry 4145 (class 0 OID 24679)
-- Dependencies: 228
-- Data for Name: scope_mapping; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.scope_mapping (client_id, role_id) FROM stdin;
9175a02f-cf18-40a3-b322-73ed00ce1729	7c1f6e3d-2852-442c-81a0-0db59b56eea1
9175a02f-cf18-40a3-b322-73ed00ce1729	3b221f0d-f0bd-4980-a6c6-a01e62dbcc50
660743a4-25c1-46dd-b9c1-a861be2a2d84	de74335c-d20d-47a2-a8d0-267a731a5dd5
660743a4-25c1-46dd-b9c1-a861be2a2d84	416f330a-0839-466e-a3f3-7baebb4a3e61
\.


--
-- TOC entry 4192 (class 0 OID 25637)
-- Dependencies: 275
-- Data for Name: scope_policy; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.scope_policy (scope_id, policy_id) FROM stdin;
\.


--
-- TOC entry 4146 (class 0 OID 24685)
-- Dependencies: 229
-- Data for Name: user_attribute; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.user_attribute (name, value, user_id, id, long_value_hash, long_value_hash_lower_case, long_value) FROM stdin;
is_temporary_admin	true	416e44c2-0739-4968-b623-78f86e44b6b1	c6a59944-0c8a-4144-ad55-147afa75500f	\N	\N	\N
\.


--
-- TOC entry 4165 (class 0 OID 25122)
-- Dependencies: 248
-- Data for Name: user_consent; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.user_consent (id, client_id, user_id, created_date, last_updated_date, client_storage_provider, external_client_id) FROM stdin;
\.


--
-- TOC entry 4208 (class 0 OID 25990)
-- Dependencies: 291
-- Data for Name: user_consent_client_scope; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.user_consent_client_scope (user_consent_id, scope_id) FROM stdin;
\.


--
-- TOC entry 4147 (class 0 OID 24690)
-- Dependencies: 230
-- Data for Name: user_entity; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.user_entity (id, email, email_constraint, email_verified, enabled, federation_link, first_name, last_name, realm_id, username, created_timestamp, service_account_client_link, not_before) FROM stdin;
416e44c2-0739-4968-b623-78f86e44b6b1	\N	0fbfd10d-986d-4a6e-b77d-9b6f06063022	f	t	\N	\N	\N	9ccc2b44-8d11-4694-87e4-8e194b225e1d	admin	1761219235377	\N	0
1e861589-92eb-461e-b844-9f07baa5c252	\N	96267c55-e9d5-4bad-b224-51a5e3f22928	f	t	\N	\N	\N	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	service-account-bambaiba-admin-client	1761221304182	d8d4e759-1346-4d49-8cb2-8d96ba570f07	0
f1bd369f-b87c-4434-bdf7-e6cc125849d1	clinsaure@gmail.com	clinsaure@gmail.com	t	t	\N	Kamdem	Kenmogne	9ccc2b44-8d11-4694-87e4-8e194b225e1d	bambaiba	1761221052389	\N	0
5339f261-b577-47ba-b3c7-fe335de2090f	user@user.com	user@user.com	t	t	\N	User	User	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	user@user.com	1763545929112	\N	0
ddd2c493-94bc-47d8-b94e-b5f24188e8d4	user1@user.com	user1@user.com	t	t	\N	User1	User1	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	user1@user.com	1763546025192	\N	0
3ecb9173-ca70-49e6-a3df-1945943ead6e	admin@bambaiba.com	admin@bambaiba.com	t	t	\N	Bamba	Iba	9ccc2b44-8d11-4694-87e4-8e194b225e1d	admin@bambaiba.com	1764684727104	\N	0
94557eed-86d5-4d01-bc8d-767f1a6bd89c	user2@user.com	user2@user.com	t	t	\N	User	User2	22478864-d5ad-4829-b2bf-92ed9d9ca9c8	user2@user.com	1764761712128	\N	0
\.


--
-- TOC entry 4148 (class 0 OID 24698)
-- Dependencies: 231
-- Data for Name: user_federation_config; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.user_federation_config (user_federation_provider_id, value, name) FROM stdin;
\.


--
-- TOC entry 4171 (class 0 OID 25234)
-- Dependencies: 254
-- Data for Name: user_federation_mapper; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.user_federation_mapper (id, name, federation_provider_id, federation_mapper_type, realm_id) FROM stdin;
\.


--
-- TOC entry 4172 (class 0 OID 25239)
-- Dependencies: 255
-- Data for Name: user_federation_mapper_config; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.user_federation_mapper_config (user_federation_mapper_id, value, name) FROM stdin;
\.


--
-- TOC entry 4149 (class 0 OID 24703)
-- Dependencies: 232
-- Data for Name: user_federation_provider; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.user_federation_provider (id, changed_sync_period, display_name, full_sync_period, last_sync, priority, provider_name, realm_id) FROM stdin;
\.


--
-- TOC entry 4180 (class 0 OID 25402)
-- Dependencies: 263
-- Data for Name: user_group_membership; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.user_group_membership (group_id, user_id, membership_type) FROM stdin;
\.


--
-- TOC entry 4150 (class 0 OID 24708)
-- Dependencies: 233
-- Data for Name: user_required_action; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.user_required_action (user_id, required_action) FROM stdin;
\.


--
-- TOC entry 4151 (class 0 OID 24711)
-- Dependencies: 234
-- Data for Name: user_role_mapping; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.user_role_mapping (role_id, user_id) FROM stdin;
333d5b0e-b35b-447c-849f-6d1954882d54	416e44c2-0739-4968-b623-78f86e44b6b1
e6bb89f1-c4a8-4448-ae69-1f07c1f17df3	416e44c2-0739-4968-b623-78f86e44b6b1
333d5b0e-b35b-447c-849f-6d1954882d54	f1bd369f-b87c-4434-bdf7-e6cc125849d1
4157d79f-00e3-4da9-915c-80008d3ba419	f1bd369f-b87c-4434-bdf7-e6cc125849d1
f47467eb-6bb9-4c13-9f1f-4c1d3d971818	f1bd369f-b87c-4434-bdf7-e6cc125849d1
380e1a65-8df9-436a-8de2-f4e1234cc8b2	f1bd369f-b87c-4434-bdf7-e6cc125849d1
0ae04e8d-0a82-47de-af0b-d8115bfbd362	f1bd369f-b87c-4434-bdf7-e6cc125849d1
b3c278c3-1100-40f7-87e1-1fc5d5b6cfa1	f1bd369f-b87c-4434-bdf7-e6cc125849d1
0536112c-0684-4d3c-8bca-dcb371abc4df	f1bd369f-b87c-4434-bdf7-e6cc125849d1
022ac9ce-2ddd-4cdd-84d4-03193514f22e	f1bd369f-b87c-4434-bdf7-e6cc125849d1
5d2fbb55-64d9-49c8-a075-b5ab93c54099	f1bd369f-b87c-4434-bdf7-e6cc125849d1
87860229-fed9-491d-9148-43936b486420	f1bd369f-b87c-4434-bdf7-e6cc125849d1
5f4f4ea4-6492-40c5-b5ee-2792203858db	f1bd369f-b87c-4434-bdf7-e6cc125849d1
356f1685-3619-41ae-92d3-e100d78f77d6	f1bd369f-b87c-4434-bdf7-e6cc125849d1
9d104f8e-c89c-434a-aac9-5ef19008b5cf	f1bd369f-b87c-4434-bdf7-e6cc125849d1
b0918c3c-35f4-40cb-90f6-91cb3c1693e5	f1bd369f-b87c-4434-bdf7-e6cc125849d1
ac5ff45d-649c-4845-9cf9-ddd596b5f265	f1bd369f-b87c-4434-bdf7-e6cc125849d1
64cc72b0-840b-4dfe-8f4e-b63c713ee89f	f1bd369f-b87c-4434-bdf7-e6cc125849d1
6fce7751-623a-4f50-baf3-98d5859544ed	f1bd369f-b87c-4434-bdf7-e6cc125849d1
abbf467d-4085-4b6d-b964-2202113c144f	f1bd369f-b87c-4434-bdf7-e6cc125849d1
431d7153-703d-4918-b8ae-d1a1d45624a4	f1bd369f-b87c-4434-bdf7-e6cc125849d1
94d154d2-ec32-47ff-a94f-fbcdea2d83ab	f1bd369f-b87c-4434-bdf7-e6cc125849d1
8efde795-9511-4eb3-abe9-b8468bc02983	f1bd369f-b87c-4434-bdf7-e6cc125849d1
c7ba1b75-982f-43b7-bd10-8e0feef9c554	f1bd369f-b87c-4434-bdf7-e6cc125849d1
3a7c9cf0-68e4-4f14-b8cf-4aaea27bde21	f1bd369f-b87c-4434-bdf7-e6cc125849d1
5d5eb9ed-6c16-4ff4-9ab5-7a2e3730be41	f1bd369f-b87c-4434-bdf7-e6cc125849d1
e91b22e7-31af-41cc-947a-ea536fe8f03e	f1bd369f-b87c-4434-bdf7-e6cc125849d1
6b3de988-e5d4-4c6f-b5d6-0490f9935949	f1bd369f-b87c-4434-bdf7-e6cc125849d1
2e93cba4-9c6e-4769-9a5f-3662b8824ddf	f1bd369f-b87c-4434-bdf7-e6cc125849d1
c3b1ccd2-a938-412b-b5e2-4eb2e1d4b7bd	f1bd369f-b87c-4434-bdf7-e6cc125849d1
df44c961-8daa-4462-af0d-835543b505af	f1bd369f-b87c-4434-bdf7-e6cc125849d1
7b211510-1bdf-4dad-81ab-9d6b7860e74a	f1bd369f-b87c-4434-bdf7-e6cc125849d1
548365c0-c74f-401e-a49d-f27dde9b8fc3	f1bd369f-b87c-4434-bdf7-e6cc125849d1
222ff0cb-515e-42d4-a3f6-e195a33a58df	f1bd369f-b87c-4434-bdf7-e6cc125849d1
9f3a7329-42e6-4a17-8254-a9844ca68dc7	f1bd369f-b87c-4434-bdf7-e6cc125849d1
3dee69c1-baf4-4083-95c9-e9e253c6e531	f1bd369f-b87c-4434-bdf7-e6cc125849d1
a0b7e6c7-5ba5-472b-af73-effdc48e268b	f1bd369f-b87c-4434-bdf7-e6cc125849d1
2248f8c9-e631-4c60-9cef-f30394594d45	f1bd369f-b87c-4434-bdf7-e6cc125849d1
f7199ff7-a9ef-4a27-a3b5-ae68a4f19ca5	f1bd369f-b87c-4434-bdf7-e6cc125849d1
390fff60-e27e-4c07-8950-30214e724306	f1bd369f-b87c-4434-bdf7-e6cc125849d1
58e89a31-840e-49fc-8677-a26f2a9ed5a2	f1bd369f-b87c-4434-bdf7-e6cc125849d1
3b221f0d-f0bd-4980-a6c6-a01e62dbcc50	f1bd369f-b87c-4434-bdf7-e6cc125849d1
aa4848f8-d34c-4199-99ba-ee75a4741a9d	f1bd369f-b87c-4434-bdf7-e6cc125849d1
61712afd-1efc-4bce-af49-4b5a3637d990	f1bd369f-b87c-4434-bdf7-e6cc125849d1
1b14b11f-7685-44ab-9d6e-740aef46a781	f1bd369f-b87c-4434-bdf7-e6cc125849d1
7c1f6e3d-2852-442c-81a0-0db59b56eea1	f1bd369f-b87c-4434-bdf7-e6cc125849d1
69cfe3f6-0cd9-44c6-90c5-1cd521be9fdd	f1bd369f-b87c-4434-bdf7-e6cc125849d1
26013cc2-2dfa-4509-ad7e-5671830d5b9f	f1bd369f-b87c-4434-bdf7-e6cc125849d1
6337a65d-feea-4b7c-b5e2-6d122b88b571	1e861589-92eb-461e-b844-9f07baa5c252
416f330a-0839-466e-a3f3-7baebb4a3e61	1e861589-92eb-461e-b844-9f07baa5c252
966cb239-0efe-4e7a-a18c-a2e814e872ad	1e861589-92eb-461e-b844-9f07baa5c252
6337a65d-feea-4b7c-b5e2-6d122b88b571	5339f261-b577-47ba-b3c7-fe335de2090f
6337a65d-feea-4b7c-b5e2-6d122b88b571	ddd2c493-94bc-47d8-b94e-b5f24188e8d4
333d5b0e-b35b-447c-849f-6d1954882d54	3ecb9173-ca70-49e6-a3df-1945943ead6e
ac5ff45d-649c-4845-9cf9-ddd596b5f265	3ecb9173-ca70-49e6-a3df-1945943ead6e
6fce7751-623a-4f50-baf3-98d5859544ed	3ecb9173-ca70-49e6-a3df-1945943ead6e
64cc72b0-840b-4dfe-8f4e-b63c713ee89f	3ecb9173-ca70-49e6-a3df-1945943ead6e
94d154d2-ec32-47ff-a94f-fbcdea2d83ab	3ecb9173-ca70-49e6-a3df-1945943ead6e
431d7153-703d-4918-b8ae-d1a1d45624a4	3ecb9173-ca70-49e6-a3df-1945943ead6e
abbf467d-4085-4b6d-b964-2202113c144f	3ecb9173-ca70-49e6-a3df-1945943ead6e
c7ba1b75-982f-43b7-bd10-8e0feef9c554	3ecb9173-ca70-49e6-a3df-1945943ead6e
8efde795-9511-4eb3-abe9-b8468bc02983	3ecb9173-ca70-49e6-a3df-1945943ead6e
e91b22e7-31af-41cc-947a-ea536fe8f03e	3ecb9173-ca70-49e6-a3df-1945943ead6e
5d5eb9ed-6c16-4ff4-9ab5-7a2e3730be41	3ecb9173-ca70-49e6-a3df-1945943ead6e
3a7c9cf0-68e4-4f14-b8cf-4aaea27bde21	3ecb9173-ca70-49e6-a3df-1945943ead6e
6b3de988-e5d4-4c6f-b5d6-0490f9935949	3ecb9173-ca70-49e6-a3df-1945943ead6e
c3b1ccd2-a938-412b-b5e2-4eb2e1d4b7bd	3ecb9173-ca70-49e6-a3df-1945943ead6e
2e93cba4-9c6e-4769-9a5f-3662b8824ddf	3ecb9173-ca70-49e6-a3df-1945943ead6e
7b211510-1bdf-4dad-81ab-9d6b7860e74a	3ecb9173-ca70-49e6-a3df-1945943ead6e
df44c961-8daa-4462-af0d-835543b505af	3ecb9173-ca70-49e6-a3df-1945943ead6e
9f3a7329-42e6-4a17-8254-a9844ca68dc7	3ecb9173-ca70-49e6-a3df-1945943ead6e
548365c0-c74f-401e-a49d-f27dde9b8fc3	3ecb9173-ca70-49e6-a3df-1945943ead6e
222ff0cb-515e-42d4-a3f6-e195a33a58df	3ecb9173-ca70-49e6-a3df-1945943ead6e
a0b7e6c7-5ba5-472b-af73-effdc48e268b	3ecb9173-ca70-49e6-a3df-1945943ead6e
69cfe3f6-0cd9-44c6-90c5-1cd521be9fdd	3ecb9173-ca70-49e6-a3df-1945943ead6e
3dee69c1-baf4-4083-95c9-e9e253c6e531	3ecb9173-ca70-49e6-a3df-1945943ead6e
3b221f0d-f0bd-4980-a6c6-a01e62dbcc50	3ecb9173-ca70-49e6-a3df-1945943ead6e
1b14b11f-7685-44ab-9d6e-740aef46a781	3ecb9173-ca70-49e6-a3df-1945943ead6e
f7199ff7-a9ef-4a27-a3b5-ae68a4f19ca5	3ecb9173-ca70-49e6-a3df-1945943ead6e
aa4848f8-d34c-4199-99ba-ee75a4741a9d	3ecb9173-ca70-49e6-a3df-1945943ead6e
2248f8c9-e631-4c60-9cef-f30394594d45	3ecb9173-ca70-49e6-a3df-1945943ead6e
61712afd-1efc-4bce-af49-4b5a3637d990	3ecb9173-ca70-49e6-a3df-1945943ead6e
390fff60-e27e-4c07-8950-30214e724306	3ecb9173-ca70-49e6-a3df-1945943ead6e
7c1f6e3d-2852-442c-81a0-0db59b56eea1	3ecb9173-ca70-49e6-a3df-1945943ead6e
58e89a31-840e-49fc-8677-a26f2a9ed5a2	3ecb9173-ca70-49e6-a3df-1945943ead6e
26013cc2-2dfa-4509-ad7e-5671830d5b9f	3ecb9173-ca70-49e6-a3df-1945943ead6e
6337a65d-feea-4b7c-b5e2-6d122b88b571	94557eed-86d5-4d01-bc8d-767f1a6bd89c
\.


--
-- TOC entry 4152 (class 0 OID 24725)
-- Dependencies: 235
-- Data for Name: web_origins; Type: TABLE DATA; Schema: public; Owner: keycloakuser
--

COPY public.web_origins (client_id, value) FROM stdin;
6844dd1b-0ae6-4e81-9ece-5f8b5dcb649e	+
862e68c6-d552-4480-9b9f-4d3a69704afa	+
d8d4e759-1346-4d49-8cb2-8d96ba570f07	/*
7e903410-f31f-4bc8-8402-777e4538d2a1	http://localhost:18081
88acef04-74b2-48a9-b20a-91e8a91a6071	/*
\.


--
-- TOC entry 3915 (class 2606 OID 26182)
-- Name: org_domain ORG_DOMAIN_pkey; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.org_domain
    ADD CONSTRAINT "ORG_DOMAIN_pkey" PRIMARY KEY (id, name);


--
-- TOC entry 3907 (class 2606 OID 26171)
-- Name: org ORG_pkey; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.org
    ADD CONSTRAINT "ORG_pkey" PRIMARY KEY (id);


--
-- TOC entry 3641 (class 2606 OID 26091)
-- Name: keycloak_role UK_J3RWUVD56ONTGSUHOGM184WW2-2; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.keycloak_role
    ADD CONSTRAINT "UK_J3RWUVD56ONTGSUHOGM184WW2-2" UNIQUE (name, client_realm_constraint);


--
-- TOC entry 3876 (class 2606 OID 25921)
-- Name: client_auth_flow_bindings c_cli_flow_bind; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.client_auth_flow_bindings
    ADD CONSTRAINT c_cli_flow_bind PRIMARY KEY (client_id, binding_name);


--
-- TOC entry 3878 (class 2606 OID 26120)
-- Name: client_scope_client c_cli_scope_bind; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.client_scope_client
    ADD CONSTRAINT c_cli_scope_bind PRIMARY KEY (client_id, scope_id);


--
-- TOC entry 3873 (class 2606 OID 25795)
-- Name: client_initial_access cnstr_client_init_acc_pk; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.client_initial_access
    ADD CONSTRAINT cnstr_client_init_acc_pk PRIMARY KEY (id);


--
-- TOC entry 3788 (class 2606 OID 25443)
-- Name: realm_default_groups con_group_id_def_groups; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.realm_default_groups
    ADD CONSTRAINT con_group_id_def_groups UNIQUE (group_id);


--
-- TOC entry 3836 (class 2606 OID 25718)
-- Name: broker_link constr_broker_link_pk; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.broker_link
    ADD CONSTRAINT constr_broker_link_pk PRIMARY KEY (identity_provider, user_id);


--
-- TOC entry 3864 (class 2606 OID 25738)
-- Name: component_config constr_component_config_pk; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.component_config
    ADD CONSTRAINT constr_component_config_pk PRIMARY KEY (id);


--
-- TOC entry 3867 (class 2606 OID 25736)
-- Name: component constr_component_pk; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.component
    ADD CONSTRAINT constr_component_pk PRIMARY KEY (id);


--
-- TOC entry 3856 (class 2606 OID 25734)
-- Name: fed_user_required_action constr_fed_required_action; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.fed_user_required_action
    ADD CONSTRAINT constr_fed_required_action PRIMARY KEY (required_action, user_id);


--
-- TOC entry 3838 (class 2606 OID 25720)
-- Name: fed_user_attribute constr_fed_user_attr_pk; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.fed_user_attribute
    ADD CONSTRAINT constr_fed_user_attr_pk PRIMARY KEY (id);


--
-- TOC entry 3843 (class 2606 OID 25722)
-- Name: fed_user_consent constr_fed_user_consent_pk; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.fed_user_consent
    ADD CONSTRAINT constr_fed_user_consent_pk PRIMARY KEY (id);


--
-- TOC entry 3848 (class 2606 OID 25728)
-- Name: fed_user_credential constr_fed_user_cred_pk; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.fed_user_credential
    ADD CONSTRAINT constr_fed_user_cred_pk PRIMARY KEY (id);


--
-- TOC entry 3852 (class 2606 OID 25730)
-- Name: fed_user_group_membership constr_fed_user_group; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.fed_user_group_membership
    ADD CONSTRAINT constr_fed_user_group PRIMARY KEY (group_id, user_id);


--
-- TOC entry 3860 (class 2606 OID 25732)
-- Name: fed_user_role_mapping constr_fed_user_role; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.fed_user_role_mapping
    ADD CONSTRAINT constr_fed_user_role PRIMARY KEY (role_id, user_id);


--
-- TOC entry 3871 (class 2606 OID 25775)
-- Name: federated_user constr_federated_user; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.federated_user
    ADD CONSTRAINT constr_federated_user PRIMARY KEY (id);


--
-- TOC entry 3790 (class 2606 OID 25879)
-- Name: realm_default_groups constr_realm_default_groups; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.realm_default_groups
    ADD CONSTRAINT constr_realm_default_groups PRIMARY KEY (realm_id, group_id);


--
-- TOC entry 3723 (class 2606 OID 25896)
-- Name: realm_enabled_event_types constr_realm_enabl_event_types; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.realm_enabled_event_types
    ADD CONSTRAINT constr_realm_enabl_event_types PRIMARY KEY (realm_id, value);


--
-- TOC entry 3655 (class 2606 OID 25898)
-- Name: realm_events_listeners constr_realm_events_listeners; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.realm_events_listeners
    ADD CONSTRAINT constr_realm_events_listeners PRIMARY KEY (realm_id, value);


--
-- TOC entry 3720 (class 2606 OID 25900)
-- Name: realm_supported_locales constr_realm_supported_locales; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.realm_supported_locales
    ADD CONSTRAINT constr_realm_supported_locales PRIMARY KEY (realm_id, value);


--
-- TOC entry 3711 (class 2606 OID 25029)
-- Name: identity_provider constraint_2b; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.identity_provider
    ADD CONSTRAINT constraint_2b PRIMARY KEY (internal_id);


--
-- TOC entry 3696 (class 2606 OID 24963)
-- Name: client_attributes constraint_3c; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.client_attributes
    ADD CONSTRAINT constraint_3c PRIMARY KEY (client_id, name);


--
-- TOC entry 3638 (class 2606 OID 24737)
-- Name: event_entity constraint_4; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.event_entity
    ADD CONSTRAINT constraint_4 PRIMARY KEY (id);


--
-- TOC entry 3707 (class 2606 OID 25031)
-- Name: federated_identity constraint_40; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.federated_identity
    ADD CONSTRAINT constraint_40 PRIMARY KEY (identity_provider, user_id);


--
-- TOC entry 3647 (class 2606 OID 24739)
-- Name: realm constraint_4a; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.realm
    ADD CONSTRAINT constraint_4a PRIMARY KEY (id);


--
-- TOC entry 3684 (class 2606 OID 24745)
-- Name: user_federation_provider constraint_5c; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_federation_provider
    ADD CONSTRAINT constraint_5c PRIMARY KEY (id);


--
-- TOC entry 3626 (class 2606 OID 24749)
-- Name: client constraint_7; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.client
    ADD CONSTRAINT constraint_7 PRIMARY KEY (id);


--
-- TOC entry 3665 (class 2606 OID 24753)
-- Name: scope_mapping constraint_81; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.scope_mapping
    ADD CONSTRAINT constraint_81 PRIMARY KEY (client_id, role_id);


--
-- TOC entry 3699 (class 2606 OID 24967)
-- Name: client_node_registrations constraint_84; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.client_node_registrations
    ADD CONSTRAINT constraint_84 PRIMARY KEY (client_id, name);


--
-- TOC entry 3652 (class 2606 OID 24755)
-- Name: realm_attribute constraint_9; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.realm_attribute
    ADD CONSTRAINT constraint_9 PRIMARY KEY (name, realm_id);


--
-- TOC entry 3658 (class 2606 OID 24757)
-- Name: realm_required_credential constraint_92; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.realm_required_credential
    ADD CONSTRAINT constraint_92 PRIMARY KEY (realm_id, type);


--
-- TOC entry 3643 (class 2606 OID 24759)
-- Name: keycloak_role constraint_a; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.keycloak_role
    ADD CONSTRAINT constraint_a PRIMARY KEY (id);


--
-- TOC entry 3741 (class 2606 OID 25883)
-- Name: admin_event_entity constraint_admin_event_entity; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.admin_event_entity
    ADD CONSTRAINT constraint_admin_event_entity PRIMARY KEY (id);


--
-- TOC entry 3754 (class 2606 OID 25260)
-- Name: authenticator_config_entry constraint_auth_cfg_pk; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.authenticator_config_entry
    ADD CONSTRAINT constraint_auth_cfg_pk PRIMARY KEY (authenticator_id, name);


--
-- TOC entry 3750 (class 2606 OID 25258)
-- Name: authentication_execution constraint_auth_exec_pk; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.authentication_execution
    ADD CONSTRAINT constraint_auth_exec_pk PRIMARY KEY (id);


--
-- TOC entry 3747 (class 2606 OID 25256)
-- Name: authentication_flow constraint_auth_flow_pk; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.authentication_flow
    ADD CONSTRAINT constraint_auth_flow_pk PRIMARY KEY (id);


--
-- TOC entry 3744 (class 2606 OID 25254)
-- Name: authenticator_config constraint_auth_pk; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.authenticator_config
    ADD CONSTRAINT constraint_auth_pk PRIMARY KEY (id);


--
-- TOC entry 3690 (class 2606 OID 24761)
-- Name: user_role_mapping constraint_c; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_role_mapping
    ADD CONSTRAINT constraint_c PRIMARY KEY (role_id, user_id);


--
-- TOC entry 3631 (class 2606 OID 25877)
-- Name: composite_role constraint_composite_role; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.composite_role
    ADD CONSTRAINT constraint_composite_role PRIMARY KEY (composite, child_role);


--
-- TOC entry 3718 (class 2606 OID 25033)
-- Name: identity_provider_config constraint_d; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.identity_provider_config
    ADD CONSTRAINT constraint_d PRIMARY KEY (identity_provider_id, name);


--
-- TOC entry 3822 (class 2606 OID 25601)
-- Name: policy_config constraint_dpc; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.policy_config
    ADD CONSTRAINT constraint_dpc PRIMARY KEY (policy_id, name);


--
-- TOC entry 3660 (class 2606 OID 24763)
-- Name: realm_smtp_config constraint_e; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.realm_smtp_config
    ADD CONSTRAINT constraint_e PRIMARY KEY (realm_id, name);


--
-- TOC entry 3635 (class 2606 OID 24765)
-- Name: credential constraint_f; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.credential
    ADD CONSTRAINT constraint_f PRIMARY KEY (id);


--
-- TOC entry 3682 (class 2606 OID 24767)
-- Name: user_federation_config constraint_f9; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_federation_config
    ADD CONSTRAINT constraint_f9 PRIMARY KEY (user_federation_provider_id, name);


--
-- TOC entry 3892 (class 2606 OID 26019)
-- Name: resource_server_perm_ticket constraint_fapmt; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_server_perm_ticket
    ADD CONSTRAINT constraint_fapmt PRIMARY KEY (id);


--
-- TOC entry 3807 (class 2606 OID 25559)
-- Name: resource_server_resource constraint_farsr; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_server_resource
    ADD CONSTRAINT constraint_farsr PRIMARY KEY (id);


--
-- TOC entry 3817 (class 2606 OID 25587)
-- Name: resource_server_policy constraint_farsrp; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_server_policy
    ADD CONSTRAINT constraint_farsrp PRIMARY KEY (id);


--
-- TOC entry 3833 (class 2606 OID 25656)
-- Name: associated_policy constraint_farsrpap; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.associated_policy
    ADD CONSTRAINT constraint_farsrpap PRIMARY KEY (policy_id, associated_policy_id);


--
-- TOC entry 3827 (class 2606 OID 25626)
-- Name: resource_policy constraint_farsrpp; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_policy
    ADD CONSTRAINT constraint_farsrpp PRIMARY KEY (resource_id, policy_id);


--
-- TOC entry 3812 (class 2606 OID 25573)
-- Name: resource_server_scope constraint_farsrs; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_server_scope
    ADD CONSTRAINT constraint_farsrs PRIMARY KEY (id);


--
-- TOC entry 3824 (class 2606 OID 25611)
-- Name: resource_scope constraint_farsrsp; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_scope
    ADD CONSTRAINT constraint_farsrsp PRIMARY KEY (resource_id, scope_id);


--
-- TOC entry 3830 (class 2606 OID 25641)
-- Name: scope_policy constraint_farsrsps; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.scope_policy
    ADD CONSTRAINT constraint_farsrsps PRIMARY KEY (scope_id, policy_id);


--
-- TOC entry 3674 (class 2606 OID 24769)
-- Name: user_entity constraint_fb; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_entity
    ADD CONSTRAINT constraint_fb PRIMARY KEY (id);


--
-- TOC entry 3760 (class 2606 OID 25268)
-- Name: user_federation_mapper_config constraint_fedmapper_cfg_pm; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_federation_mapper_config
    ADD CONSTRAINT constraint_fedmapper_cfg_pm PRIMARY KEY (user_federation_mapper_id, name);


--
-- TOC entry 3756 (class 2606 OID 25266)
-- Name: user_federation_mapper constraint_fedmapperpm; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_federation_mapper
    ADD CONSTRAINT constraint_fedmapperpm PRIMARY KEY (id);


--
-- TOC entry 3890 (class 2606 OID 26004)
-- Name: fed_user_consent_cl_scope constraint_fgrntcsnt_clsc_pm; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.fed_user_consent_cl_scope
    ADD CONSTRAINT constraint_fgrntcsnt_clsc_pm PRIMARY KEY (user_consent_id, scope_id);


--
-- TOC entry 3886 (class 2606 OID 25994)
-- Name: user_consent_client_scope constraint_grntcsnt_clsc_pm; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_consent_client_scope
    ADD CONSTRAINT constraint_grntcsnt_clsc_pm PRIMARY KEY (user_consent_id, scope_id);


--
-- TOC entry 3734 (class 2606 OID 25141)
-- Name: user_consent constraint_grntcsnt_pm; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_consent
    ADD CONSTRAINT constraint_grntcsnt_pm PRIMARY KEY (id);


--
-- TOC entry 3774 (class 2606 OID 25410)
-- Name: keycloak_group constraint_group; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.keycloak_group
    ADD CONSTRAINT constraint_group PRIMARY KEY (id);


--
-- TOC entry 3781 (class 2606 OID 25417)
-- Name: group_attribute constraint_group_attribute_pk; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.group_attribute
    ADD CONSTRAINT constraint_group_attribute_pk PRIMARY KEY (id);


--
-- TOC entry 3778 (class 2606 OID 25431)
-- Name: group_role_mapping constraint_group_role; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.group_role_mapping
    ADD CONSTRAINT constraint_group_role PRIMARY KEY (role_id, group_id);


--
-- TOC entry 3729 (class 2606 OID 25137)
-- Name: identity_provider_mapper constraint_idpm; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.identity_provider_mapper
    ADD CONSTRAINT constraint_idpm PRIMARY KEY (id);


--
-- TOC entry 3732 (class 2606 OID 25317)
-- Name: idp_mapper_config constraint_idpmconfig; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.idp_mapper_config
    ADD CONSTRAINT constraint_idpmconfig PRIMARY KEY (idp_mapper_id, name);


--
-- TOC entry 3921 (class 2606 OID 26208)
-- Name: jgroups_ping constraint_jgroups_ping; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.jgroups_ping
    ADD CONSTRAINT constraint_jgroups_ping PRIMARY KEY (address);


--
-- TOC entry 3726 (class 2606 OID 25135)
-- Name: migration_model constraint_migmod; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.migration_model
    ADD CONSTRAINT constraint_migmod PRIMARY KEY (id);


--
-- TOC entry 3772 (class 2606 OID 26097)
-- Name: offline_client_session constraint_offl_cl_ses_pk3; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.offline_client_session
    ADD CONSTRAINT constraint_offl_cl_ses_pk3 PRIMARY KEY (user_session_id, client_id, client_storage_provider, external_client_id, offline_flag);


--
-- TOC entry 3767 (class 2606 OID 25387)
-- Name: offline_user_session constraint_offl_us_ses_pk2; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.offline_user_session
    ADD CONSTRAINT constraint_offl_us_ses_pk2 PRIMARY KEY (user_session_id, offline_flag);


--
-- TOC entry 3701 (class 2606 OID 25027)
-- Name: protocol_mapper constraint_pcm; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.protocol_mapper
    ADD CONSTRAINT constraint_pcm PRIMARY KEY (id);


--
-- TOC entry 3705 (class 2606 OID 25310)
-- Name: protocol_mapper_config constraint_pmconfig; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.protocol_mapper_config
    ADD CONSTRAINT constraint_pmconfig PRIMARY KEY (protocol_mapper_id, name);


--
-- TOC entry 3662 (class 2606 OID 25902)
-- Name: redirect_uris constraint_redirect_uris; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.redirect_uris
    ADD CONSTRAINT constraint_redirect_uris PRIMARY KEY (client_id, value);


--
-- TOC entry 3765 (class 2606 OID 25350)
-- Name: required_action_config constraint_req_act_cfg_pk; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.required_action_config
    ADD CONSTRAINT constraint_req_act_cfg_pk PRIMARY KEY (required_action_id, name);


--
-- TOC entry 3762 (class 2606 OID 25348)
-- Name: required_action_provider constraint_req_act_prv_pk; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.required_action_provider
    ADD CONSTRAINT constraint_req_act_prv_pk PRIMARY KEY (id);


--
-- TOC entry 3687 (class 2606 OID 25262)
-- Name: user_required_action constraint_required_action; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_required_action
    ADD CONSTRAINT constraint_required_action PRIMARY KEY (required_action, user_id);


--
-- TOC entry 3900 (class 2606 OID 26066)
-- Name: resource_uris constraint_resour_uris_pk; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_uris
    ADD CONSTRAINT constraint_resour_uris_pk PRIMARY KEY (resource_id, value);


--
-- TOC entry 3902 (class 2606 OID 26073)
-- Name: role_attribute constraint_role_attribute_pk; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.role_attribute
    ADD CONSTRAINT constraint_role_attribute_pk PRIMARY KEY (id);


--
-- TOC entry 3918 (class 2606 OID 26197)
-- Name: revoked_token constraint_rt; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.revoked_token
    ADD CONSTRAINT constraint_rt PRIMARY KEY (id);


--
-- TOC entry 3668 (class 2606 OID 25346)
-- Name: user_attribute constraint_user_attribute_pk; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_attribute
    ADD CONSTRAINT constraint_user_attribute_pk PRIMARY KEY (id);


--
-- TOC entry 3785 (class 2606 OID 25424)
-- Name: user_group_membership constraint_user_group; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_group_membership
    ADD CONSTRAINT constraint_user_group PRIMARY KEY (group_id, user_id);


--
-- TOC entry 3693 (class 2606 OID 25904)
-- Name: web_origins constraint_web_origins; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.web_origins
    ADD CONSTRAINT constraint_web_origins PRIMARY KEY (client_id, value);


--
-- TOC entry 3624 (class 2606 OID 24581)
-- Name: databasechangeloglock databasechangeloglock_pkey; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.databasechangeloglock
    ADD CONSTRAINT databasechangeloglock_pkey PRIMARY KEY (id);


--
-- TOC entry 3799 (class 2606 OID 25527)
-- Name: client_scope_attributes pk_cl_tmpl_attr; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.client_scope_attributes
    ADD CONSTRAINT pk_cl_tmpl_attr PRIMARY KEY (scope_id, name);


--
-- TOC entry 3794 (class 2606 OID 25486)
-- Name: client_scope pk_cli_template; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.client_scope
    ADD CONSTRAINT pk_cli_template PRIMARY KEY (id);


--
-- TOC entry 3805 (class 2606 OID 25857)
-- Name: resource_server pk_resource_server; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_server
    ADD CONSTRAINT pk_resource_server PRIMARY KEY (id);


--
-- TOC entry 3803 (class 2606 OID 25515)
-- Name: client_scope_role_mapping pk_template_scope; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.client_scope_role_mapping
    ADD CONSTRAINT pk_template_scope PRIMARY KEY (scope_id, role_id);


--
-- TOC entry 3884 (class 2606 OID 25979)
-- Name: default_client_scope r_def_cli_scope_bind; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.default_client_scope
    ADD CONSTRAINT r_def_cli_scope_bind PRIMARY KEY (realm_id, scope_id);


--
-- TOC entry 3905 (class 2606 OID 26114)
-- Name: realm_localizations realm_localizations_pkey; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.realm_localizations
    ADD CONSTRAINT realm_localizations_pkey PRIMARY KEY (realm_id, locale);


--
-- TOC entry 3898 (class 2606 OID 26046)
-- Name: resource_attribute res_attr_pk; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_attribute
    ADD CONSTRAINT res_attr_pk PRIMARY KEY (id);


--
-- TOC entry 3776 (class 2606 OID 25787)
-- Name: keycloak_group sibling_names; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.keycloak_group
    ADD CONSTRAINT sibling_names UNIQUE (realm_id, parent_group, name);


--
-- TOC entry 3716 (class 2606 OID 25084)
-- Name: identity_provider uk_2daelwnibji49avxsrtuf6xj33; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.identity_provider
    ADD CONSTRAINT uk_2daelwnibji49avxsrtuf6xj33 UNIQUE (provider_alias, realm_id);


--
-- TOC entry 3629 (class 2606 OID 24773)
-- Name: client uk_b71cjlbenv945rb6gcon438at; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.client
    ADD CONSTRAINT uk_b71cjlbenv945rb6gcon438at UNIQUE (realm_id, client_id);


--
-- TOC entry 3796 (class 2606 OID 25932)
-- Name: client_scope uk_cli_scope; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.client_scope
    ADD CONSTRAINT uk_cli_scope UNIQUE (realm_id, name);


--
-- TOC entry 3678 (class 2606 OID 24777)
-- Name: user_entity uk_dykn684sl8up1crfei6eckhd7; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_entity
    ADD CONSTRAINT uk_dykn684sl8up1crfei6eckhd7 UNIQUE (realm_id, email_constraint);


--
-- TOC entry 3737 (class 2606 OID 26186)
-- Name: user_consent uk_external_consent; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_consent
    ADD CONSTRAINT uk_external_consent UNIQUE (client_storage_provider, external_client_id, user_id);


--
-- TOC entry 3810 (class 2606 OID 26105)
-- Name: resource_server_resource uk_frsr6t700s9v50bu18ws5ha6; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_server_resource
    ADD CONSTRAINT uk_frsr6t700s9v50bu18ws5ha6 UNIQUE (name, owner, resource_server_id);


--
-- TOC entry 3896 (class 2606 OID 26101)
-- Name: resource_server_perm_ticket uk_frsr6t700s9v50bu18ws5pmt; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_server_perm_ticket
    ADD CONSTRAINT uk_frsr6t700s9v50bu18ws5pmt UNIQUE (owner, requester, resource_server_id, resource_id, scope_id);


--
-- TOC entry 3820 (class 2606 OID 25848)
-- Name: resource_server_policy uk_frsrpt700s9v50bu18ws5ha6; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_server_policy
    ADD CONSTRAINT uk_frsrpt700s9v50bu18ws5ha6 UNIQUE (name, resource_server_id);


--
-- TOC entry 3815 (class 2606 OID 25852)
-- Name: resource_server_scope uk_frsrst700s9v50bu18ws5ha6; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_server_scope
    ADD CONSTRAINT uk_frsrst700s9v50bu18ws5ha6 UNIQUE (name, resource_server_id);


--
-- TOC entry 3739 (class 2606 OID 26184)
-- Name: user_consent uk_local_consent; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_consent
    ADD CONSTRAINT uk_local_consent UNIQUE (client_id, user_id);


--
-- TOC entry 3909 (class 2606 OID 26190)
-- Name: org uk_org_alias; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.org
    ADD CONSTRAINT uk_org_alias UNIQUE (realm_id, alias);


--
-- TOC entry 3911 (class 2606 OID 26175)
-- Name: org uk_org_group; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.org
    ADD CONSTRAINT uk_org_group UNIQUE (group_id);


--
-- TOC entry 3913 (class 2606 OID 26173)
-- Name: org uk_org_name; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.org
    ADD CONSTRAINT uk_org_name UNIQUE (realm_id, name);


--
-- TOC entry 3650 (class 2606 OID 24785)
-- Name: realm uk_orvsdmla56612eaefiq6wl5oi; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.realm
    ADD CONSTRAINT uk_orvsdmla56612eaefiq6wl5oi UNIQUE (name);


--
-- TOC entry 3680 (class 2606 OID 25777)
-- Name: user_entity uk_ru8tt6t700s9v50bu18ws5ha6; Type: CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_entity
    ADD CONSTRAINT uk_ru8tt6t700s9v50bu18ws5ha6 UNIQUE (realm_id, username);


--
-- TOC entry 3839 (class 1259 OID 26157)
-- Name: fed_user_attr_long_values; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX fed_user_attr_long_values ON public.fed_user_attribute USING btree (long_value_hash, name);


--
-- TOC entry 3840 (class 1259 OID 26159)
-- Name: fed_user_attr_long_values_lower_case; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX fed_user_attr_long_values_lower_case ON public.fed_user_attribute USING btree (long_value_hash_lower_case, name);


--
-- TOC entry 3742 (class 1259 OID 26133)
-- Name: idx_admin_event_time; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_admin_event_time ON public.admin_event_entity USING btree (realm_id, admin_event_time);


--
-- TOC entry 3834 (class 1259 OID 25801)
-- Name: idx_assoc_pol_assoc_pol_id; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_assoc_pol_assoc_pol_id ON public.associated_policy USING btree (associated_policy_id);


--
-- TOC entry 3745 (class 1259 OID 25805)
-- Name: idx_auth_config_realm; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_auth_config_realm ON public.authenticator_config USING btree (realm_id);


--
-- TOC entry 3751 (class 1259 OID 25803)
-- Name: idx_auth_exec_flow; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_auth_exec_flow ON public.authentication_execution USING btree (flow_id);


--
-- TOC entry 3752 (class 1259 OID 25802)
-- Name: idx_auth_exec_realm_flow; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_auth_exec_realm_flow ON public.authentication_execution USING btree (realm_id, flow_id);


--
-- TOC entry 3748 (class 1259 OID 25804)
-- Name: idx_auth_flow_realm; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_auth_flow_realm ON public.authentication_flow USING btree (realm_id);


--
-- TOC entry 3879 (class 1259 OID 26121)
-- Name: idx_cl_clscope; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_cl_clscope ON public.client_scope_client USING btree (scope_id);


--
-- TOC entry 3697 (class 1259 OID 26160)
-- Name: idx_client_att_by_name_value; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_client_att_by_name_value ON public.client_attributes USING btree (name, substr(value, 1, 255));


--
-- TOC entry 3627 (class 1259 OID 26106)
-- Name: idx_client_id; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_client_id ON public.client USING btree (client_id);


--
-- TOC entry 3874 (class 1259 OID 25845)
-- Name: idx_client_init_acc_realm; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_client_init_acc_realm ON public.client_initial_access USING btree (realm_id);


--
-- TOC entry 3797 (class 1259 OID 26009)
-- Name: idx_clscope_attrs; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_clscope_attrs ON public.client_scope_attributes USING btree (scope_id);


--
-- TOC entry 3880 (class 1259 OID 26118)
-- Name: idx_clscope_cl; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_clscope_cl ON public.client_scope_client USING btree (client_id);


--
-- TOC entry 3702 (class 1259 OID 26006)
-- Name: idx_clscope_protmap; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_clscope_protmap ON public.protocol_mapper USING btree (client_scope_id);


--
-- TOC entry 3800 (class 1259 OID 26007)
-- Name: idx_clscope_role; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_clscope_role ON public.client_scope_role_mapping USING btree (scope_id);


--
-- TOC entry 3865 (class 1259 OID 25811)
-- Name: idx_compo_config_compo; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_compo_config_compo ON public.component_config USING btree (component_id);


--
-- TOC entry 3868 (class 1259 OID 26080)
-- Name: idx_component_provider_type; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_component_provider_type ON public.component USING btree (provider_type);


--
-- TOC entry 3869 (class 1259 OID 25810)
-- Name: idx_component_realm; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_component_realm ON public.component USING btree (realm_id);


--
-- TOC entry 3632 (class 1259 OID 25812)
-- Name: idx_composite; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_composite ON public.composite_role USING btree (composite);


--
-- TOC entry 3633 (class 1259 OID 25813)
-- Name: idx_composite_child; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_composite_child ON public.composite_role USING btree (child_role);


--
-- TOC entry 3881 (class 1259 OID 26012)
-- Name: idx_defcls_realm; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_defcls_realm ON public.default_client_scope USING btree (realm_id);


--
-- TOC entry 3882 (class 1259 OID 26013)
-- Name: idx_defcls_scope; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_defcls_scope ON public.default_client_scope USING btree (scope_id);


--
-- TOC entry 3639 (class 1259 OID 26107)
-- Name: idx_event_time; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_event_time ON public.event_entity USING btree (realm_id, event_time);


--
-- TOC entry 3708 (class 1259 OID 25544)
-- Name: idx_fedidentity_feduser; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_fedidentity_feduser ON public.federated_identity USING btree (federated_user_id);


--
-- TOC entry 3709 (class 1259 OID 25543)
-- Name: idx_fedidentity_user; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_fedidentity_user ON public.federated_identity USING btree (user_id);


--
-- TOC entry 3841 (class 1259 OID 25905)
-- Name: idx_fu_attribute; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_fu_attribute ON public.fed_user_attribute USING btree (user_id, realm_id, name);


--
-- TOC entry 3844 (class 1259 OID 25926)
-- Name: idx_fu_cnsnt_ext; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_fu_cnsnt_ext ON public.fed_user_consent USING btree (user_id, client_storage_provider, external_client_id);


--
-- TOC entry 3845 (class 1259 OID 26089)
-- Name: idx_fu_consent; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_fu_consent ON public.fed_user_consent USING btree (user_id, client_id);


--
-- TOC entry 3846 (class 1259 OID 25908)
-- Name: idx_fu_consent_ru; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_fu_consent_ru ON public.fed_user_consent USING btree (realm_id, user_id);


--
-- TOC entry 3849 (class 1259 OID 25909)
-- Name: idx_fu_credential; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_fu_credential ON public.fed_user_credential USING btree (user_id, type);


--
-- TOC entry 3850 (class 1259 OID 25910)
-- Name: idx_fu_credential_ru; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_fu_credential_ru ON public.fed_user_credential USING btree (realm_id, user_id);


--
-- TOC entry 3853 (class 1259 OID 25911)
-- Name: idx_fu_group_membership; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_fu_group_membership ON public.fed_user_group_membership USING btree (user_id, group_id);


--
-- TOC entry 3854 (class 1259 OID 25912)
-- Name: idx_fu_group_membership_ru; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_fu_group_membership_ru ON public.fed_user_group_membership USING btree (realm_id, user_id);


--
-- TOC entry 3857 (class 1259 OID 25913)
-- Name: idx_fu_required_action; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_fu_required_action ON public.fed_user_required_action USING btree (user_id, required_action);


--
-- TOC entry 3858 (class 1259 OID 25914)
-- Name: idx_fu_required_action_ru; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_fu_required_action_ru ON public.fed_user_required_action USING btree (realm_id, user_id);


--
-- TOC entry 3861 (class 1259 OID 25915)
-- Name: idx_fu_role_mapping; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_fu_role_mapping ON public.fed_user_role_mapping USING btree (user_id, role_id);


--
-- TOC entry 3862 (class 1259 OID 25916)
-- Name: idx_fu_role_mapping_ru; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_fu_role_mapping_ru ON public.fed_user_role_mapping USING btree (realm_id, user_id);


--
-- TOC entry 3782 (class 1259 OID 26135)
-- Name: idx_group_att_by_name_value; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_group_att_by_name_value ON public.group_attribute USING btree (name, ((value)::character varying(250)));


--
-- TOC entry 3783 (class 1259 OID 25816)
-- Name: idx_group_attr_group; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_group_attr_group ON public.group_attribute USING btree (group_id);


--
-- TOC entry 3779 (class 1259 OID 25817)
-- Name: idx_group_role_mapp_group; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_group_role_mapp_group ON public.group_role_mapping USING btree (group_id);


--
-- TOC entry 3730 (class 1259 OID 25819)
-- Name: idx_id_prov_mapp_realm; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_id_prov_mapp_realm ON public.identity_provider_mapper USING btree (realm_id);


--
-- TOC entry 3712 (class 1259 OID 25818)
-- Name: idx_ident_prov_realm; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_ident_prov_realm ON public.identity_provider USING btree (realm_id);


--
-- TOC entry 3713 (class 1259 OID 26201)
-- Name: idx_idp_for_login; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_idp_for_login ON public.identity_provider USING btree (realm_id, enabled, link_only, hide_on_login, organization_id);


--
-- TOC entry 3714 (class 1259 OID 26200)
-- Name: idx_idp_realm_org; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_idp_realm_org ON public.identity_provider USING btree (realm_id, organization_id);


--
-- TOC entry 3644 (class 1259 OID 25820)
-- Name: idx_keycloak_role_client; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_keycloak_role_client ON public.keycloak_role USING btree (client);


--
-- TOC entry 3645 (class 1259 OID 25821)
-- Name: idx_keycloak_role_realm; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_keycloak_role_realm ON public.keycloak_role USING btree (realm);


--
-- TOC entry 3768 (class 1259 OID 26164)
-- Name: idx_offline_uss_by_broker_session_id; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_offline_uss_by_broker_session_id ON public.offline_user_session USING btree (broker_session_id, realm_id);


--
-- TOC entry 3769 (class 1259 OID 26163)
-- Name: idx_offline_uss_by_last_session_refresh; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_offline_uss_by_last_session_refresh ON public.offline_user_session USING btree (realm_id, offline_flag, last_session_refresh);


--
-- TOC entry 3770 (class 1259 OID 26125)
-- Name: idx_offline_uss_by_user; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_offline_uss_by_user ON public.offline_user_session USING btree (user_id, realm_id, offline_flag);


--
-- TOC entry 3916 (class 1259 OID 26192)
-- Name: idx_org_domain_org_id; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_org_domain_org_id ON public.org_domain USING btree (org_id);


--
-- TOC entry 3893 (class 1259 OID 26188)
-- Name: idx_perm_ticket_owner; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_perm_ticket_owner ON public.resource_server_perm_ticket USING btree (owner);


--
-- TOC entry 3894 (class 1259 OID 26187)
-- Name: idx_perm_ticket_requester; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_perm_ticket_requester ON public.resource_server_perm_ticket USING btree (requester);


--
-- TOC entry 3703 (class 1259 OID 25822)
-- Name: idx_protocol_mapper_client; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_protocol_mapper_client ON public.protocol_mapper USING btree (client_id);


--
-- TOC entry 3653 (class 1259 OID 25825)
-- Name: idx_realm_attr_realm; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_realm_attr_realm ON public.realm_attribute USING btree (realm_id);


--
-- TOC entry 3792 (class 1259 OID 26005)
-- Name: idx_realm_clscope; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_realm_clscope ON public.client_scope USING btree (realm_id);


--
-- TOC entry 3791 (class 1259 OID 25826)
-- Name: idx_realm_def_grp_realm; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_realm_def_grp_realm ON public.realm_default_groups USING btree (realm_id);


--
-- TOC entry 3656 (class 1259 OID 25829)
-- Name: idx_realm_evt_list_realm; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_realm_evt_list_realm ON public.realm_events_listeners USING btree (realm_id);


--
-- TOC entry 3724 (class 1259 OID 25828)
-- Name: idx_realm_evt_types_realm; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_realm_evt_types_realm ON public.realm_enabled_event_types USING btree (realm_id);


--
-- TOC entry 3648 (class 1259 OID 25824)
-- Name: idx_realm_master_adm_cli; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_realm_master_adm_cli ON public.realm USING btree (master_admin_client);


--
-- TOC entry 3721 (class 1259 OID 25830)
-- Name: idx_realm_supp_local_realm; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_realm_supp_local_realm ON public.realm_supported_locales USING btree (realm_id);


--
-- TOC entry 3663 (class 1259 OID 25831)
-- Name: idx_redir_uri_client; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_redir_uri_client ON public.redirect_uris USING btree (client_id);


--
-- TOC entry 3763 (class 1259 OID 25832)
-- Name: idx_req_act_prov_realm; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_req_act_prov_realm ON public.required_action_provider USING btree (realm_id);


--
-- TOC entry 3828 (class 1259 OID 25833)
-- Name: idx_res_policy_policy; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_res_policy_policy ON public.resource_policy USING btree (policy_id);


--
-- TOC entry 3825 (class 1259 OID 25834)
-- Name: idx_res_scope_scope; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_res_scope_scope ON public.resource_scope USING btree (scope_id);


--
-- TOC entry 3818 (class 1259 OID 25853)
-- Name: idx_res_serv_pol_res_serv; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_res_serv_pol_res_serv ON public.resource_server_policy USING btree (resource_server_id);


--
-- TOC entry 3808 (class 1259 OID 25854)
-- Name: idx_res_srv_res_res_srv; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_res_srv_res_res_srv ON public.resource_server_resource USING btree (resource_server_id);


--
-- TOC entry 3813 (class 1259 OID 25855)
-- Name: idx_res_srv_scope_res_srv; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_res_srv_scope_res_srv ON public.resource_server_scope USING btree (resource_server_id);


--
-- TOC entry 3919 (class 1259 OID 26198)
-- Name: idx_rev_token_on_expire; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_rev_token_on_expire ON public.revoked_token USING btree (expire);


--
-- TOC entry 3903 (class 1259 OID 26079)
-- Name: idx_role_attribute; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_role_attribute ON public.role_attribute USING btree (role_id);


--
-- TOC entry 3801 (class 1259 OID 26008)
-- Name: idx_role_clscope; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_role_clscope ON public.client_scope_role_mapping USING btree (role_id);


--
-- TOC entry 3666 (class 1259 OID 25838)
-- Name: idx_scope_mapping_role; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_scope_mapping_role ON public.scope_mapping USING btree (role_id);


--
-- TOC entry 3831 (class 1259 OID 25839)
-- Name: idx_scope_policy_policy; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_scope_policy_policy ON public.scope_policy USING btree (policy_id);


--
-- TOC entry 3727 (class 1259 OID 26087)
-- Name: idx_update_time; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_update_time ON public.migration_model USING btree (update_time);


--
-- TOC entry 3887 (class 1259 OID 26014)
-- Name: idx_usconsent_clscope; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_usconsent_clscope ON public.user_consent_client_scope USING btree (user_consent_id);


--
-- TOC entry 3888 (class 1259 OID 26134)
-- Name: idx_usconsent_scope_id; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_usconsent_scope_id ON public.user_consent_client_scope USING btree (scope_id);


--
-- TOC entry 3669 (class 1259 OID 25540)
-- Name: idx_user_attribute; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_user_attribute ON public.user_attribute USING btree (user_id);


--
-- TOC entry 3670 (class 1259 OID 26131)
-- Name: idx_user_attribute_name; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_user_attribute_name ON public.user_attribute USING btree (name, value);


--
-- TOC entry 3735 (class 1259 OID 25537)
-- Name: idx_user_consent; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_user_consent ON public.user_consent USING btree (user_id);


--
-- TOC entry 3636 (class 1259 OID 25541)
-- Name: idx_user_credential; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_user_credential ON public.credential USING btree (user_id);


--
-- TOC entry 3675 (class 1259 OID 25534)
-- Name: idx_user_email; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_user_email ON public.user_entity USING btree (email);


--
-- TOC entry 3786 (class 1259 OID 25536)
-- Name: idx_user_group_mapping; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_user_group_mapping ON public.user_group_membership USING btree (user_id);


--
-- TOC entry 3688 (class 1259 OID 25542)
-- Name: idx_user_reqactions; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_user_reqactions ON public.user_required_action USING btree (user_id);


--
-- TOC entry 3691 (class 1259 OID 25535)
-- Name: idx_user_role_mapping; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_user_role_mapping ON public.user_role_mapping USING btree (user_id);


--
-- TOC entry 3676 (class 1259 OID 26132)
-- Name: idx_user_service_account; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_user_service_account ON public.user_entity USING btree (realm_id, service_account_client_link);


--
-- TOC entry 3757 (class 1259 OID 25841)
-- Name: idx_usr_fed_map_fed_prv; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_usr_fed_map_fed_prv ON public.user_federation_mapper USING btree (federation_provider_id);


--
-- TOC entry 3758 (class 1259 OID 25842)
-- Name: idx_usr_fed_map_realm; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_usr_fed_map_realm ON public.user_federation_mapper USING btree (realm_id);


--
-- TOC entry 3685 (class 1259 OID 25843)
-- Name: idx_usr_fed_prv_realm; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_usr_fed_prv_realm ON public.user_federation_provider USING btree (realm_id);


--
-- TOC entry 3694 (class 1259 OID 25844)
-- Name: idx_web_orig_client; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX idx_web_orig_client ON public.web_origins USING btree (client_id);


--
-- TOC entry 3671 (class 1259 OID 26156)
-- Name: user_attr_long_values; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX user_attr_long_values ON public.user_attribute USING btree (long_value_hash, name);


--
-- TOC entry 3672 (class 1259 OID 26158)
-- Name: user_attr_long_values_lower_case; Type: INDEX; Schema: public; Owner: keycloakuser
--

CREATE INDEX user_attr_long_values_lower_case ON public.user_attribute USING btree (long_value_hash_lower_case, name);


--
-- TOC entry 3944 (class 2606 OID 25038)
-- Name: identity_provider fk2b4ebc52ae5c3b34; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.identity_provider
    ADD CONSTRAINT fk2b4ebc52ae5c3b34 FOREIGN KEY (realm_id) REFERENCES public.realm(id);


--
-- TOC entry 3938 (class 2606 OID 24968)
-- Name: client_attributes fk3c47c64beacca966; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.client_attributes
    ADD CONSTRAINT fk3c47c64beacca966 FOREIGN KEY (client_id) REFERENCES public.client(id);


--
-- TOC entry 3943 (class 2606 OID 25048)
-- Name: federated_identity fk404288b92ef007a6; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.federated_identity
    ADD CONSTRAINT fk404288b92ef007a6 FOREIGN KEY (user_id) REFERENCES public.user_entity(id);


--
-- TOC entry 3939 (class 2606 OID 25195)
-- Name: client_node_registrations fk4129723ba992f594; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.client_node_registrations
    ADD CONSTRAINT fk4129723ba992f594 FOREIGN KEY (client_id) REFERENCES public.client(id);


--
-- TOC entry 3930 (class 2606 OID 24793)
-- Name: redirect_uris fk_1burs8pb4ouj97h5wuppahv9f; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.redirect_uris
    ADD CONSTRAINT fk_1burs8pb4ouj97h5wuppahv9f FOREIGN KEY (client_id) REFERENCES public.client(id);


--
-- TOC entry 3934 (class 2606 OID 24798)
-- Name: user_federation_provider fk_1fj32f6ptolw2qy60cd8n01e8; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_federation_provider
    ADD CONSTRAINT fk_1fj32f6ptolw2qy60cd8n01e8 FOREIGN KEY (realm_id) REFERENCES public.realm(id);


--
-- TOC entry 3928 (class 2606 OID 24808)
-- Name: realm_required_credential fk_5hg65lybevavkqfki3kponh9v; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.realm_required_credential
    ADD CONSTRAINT fk_5hg65lybevavkqfki3kponh9v FOREIGN KEY (realm_id) REFERENCES public.realm(id);


--
-- TOC entry 3986 (class 2606 OID 26047)
-- Name: resource_attribute fk_5hrm2vlf9ql5fu022kqepovbr; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_attribute
    ADD CONSTRAINT fk_5hrm2vlf9ql5fu022kqepovbr FOREIGN KEY (resource_id) REFERENCES public.resource_server_resource(id);


--
-- TOC entry 3932 (class 2606 OID 24813)
-- Name: user_attribute fk_5hrm2vlf9ql5fu043kqepovbr; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_attribute
    ADD CONSTRAINT fk_5hrm2vlf9ql5fu043kqepovbr FOREIGN KEY (user_id) REFERENCES public.user_entity(id);


--
-- TOC entry 3935 (class 2606 OID 24823)
-- Name: user_required_action fk_6qj3w1jw9cvafhe19bwsiuvmd; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_required_action
    ADD CONSTRAINT fk_6qj3w1jw9cvafhe19bwsiuvmd FOREIGN KEY (user_id) REFERENCES public.user_entity(id);


--
-- TOC entry 3925 (class 2606 OID 24828)
-- Name: keycloak_role fk_6vyqfe4cn4wlq8r6kt5vdsj5c; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.keycloak_role
    ADD CONSTRAINT fk_6vyqfe4cn4wlq8r6kt5vdsj5c FOREIGN KEY (realm) REFERENCES public.realm(id);


--
-- TOC entry 3929 (class 2606 OID 24833)
-- Name: realm_smtp_config fk_70ej8xdxgxd0b9hh6180irr0o; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.realm_smtp_config
    ADD CONSTRAINT fk_70ej8xdxgxd0b9hh6180irr0o FOREIGN KEY (realm_id) REFERENCES public.realm(id);


--
-- TOC entry 3926 (class 2606 OID 24848)
-- Name: realm_attribute fk_8shxd6l3e9atqukacxgpffptw; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.realm_attribute
    ADD CONSTRAINT fk_8shxd6l3e9atqukacxgpffptw FOREIGN KEY (realm_id) REFERENCES public.realm(id);


--
-- TOC entry 3922 (class 2606 OID 24853)
-- Name: composite_role fk_a63wvekftu8jo1pnj81e7mce2; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.composite_role
    ADD CONSTRAINT fk_a63wvekftu8jo1pnj81e7mce2 FOREIGN KEY (composite) REFERENCES public.keycloak_role(id);


--
-- TOC entry 3953 (class 2606 OID 25289)
-- Name: authentication_execution fk_auth_exec_flow; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.authentication_execution
    ADD CONSTRAINT fk_auth_exec_flow FOREIGN KEY (flow_id) REFERENCES public.authentication_flow(id);


--
-- TOC entry 3954 (class 2606 OID 25284)
-- Name: authentication_execution fk_auth_exec_realm; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.authentication_execution
    ADD CONSTRAINT fk_auth_exec_realm FOREIGN KEY (realm_id) REFERENCES public.realm(id);


--
-- TOC entry 3952 (class 2606 OID 25279)
-- Name: authentication_flow fk_auth_flow_realm; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.authentication_flow
    ADD CONSTRAINT fk_auth_flow_realm FOREIGN KEY (realm_id) REFERENCES public.realm(id);


--
-- TOC entry 3951 (class 2606 OID 25274)
-- Name: authenticator_config fk_auth_realm; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.authenticator_config
    ADD CONSTRAINT fk_auth_realm FOREIGN KEY (realm_id) REFERENCES public.realm(id);


--
-- TOC entry 3936 (class 2606 OID 24863)
-- Name: user_role_mapping fk_c4fqv34p1mbylloxang7b1q3l; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_role_mapping
    ADD CONSTRAINT fk_c4fqv34p1mbylloxang7b1q3l FOREIGN KEY (user_id) REFERENCES public.user_entity(id);


--
-- TOC entry 3963 (class 2606 OID 25953)
-- Name: client_scope_attributes fk_cl_scope_attr_scope; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.client_scope_attributes
    ADD CONSTRAINT fk_cl_scope_attr_scope FOREIGN KEY (scope_id) REFERENCES public.client_scope(id);


--
-- TOC entry 3964 (class 2606 OID 25943)
-- Name: client_scope_role_mapping fk_cl_scope_rm_scope; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.client_scope_role_mapping
    ADD CONSTRAINT fk_cl_scope_rm_scope FOREIGN KEY (scope_id) REFERENCES public.client_scope(id);


--
-- TOC entry 3940 (class 2606 OID 25938)
-- Name: protocol_mapper fk_cli_scope_mapper; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.protocol_mapper
    ADD CONSTRAINT fk_cli_scope_mapper FOREIGN KEY (client_scope_id) REFERENCES public.client_scope(id);


--
-- TOC entry 3979 (class 2606 OID 25796)
-- Name: client_initial_access fk_client_init_acc_realm; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.client_initial_access
    ADD CONSTRAINT fk_client_init_acc_realm FOREIGN KEY (realm_id) REFERENCES public.realm(id);


--
-- TOC entry 3977 (class 2606 OID 25744)
-- Name: component_config fk_component_config; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.component_config
    ADD CONSTRAINT fk_component_config FOREIGN KEY (component_id) REFERENCES public.component(id);


--
-- TOC entry 3978 (class 2606 OID 25739)
-- Name: component fk_component_realm; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.component
    ADD CONSTRAINT fk_component_realm FOREIGN KEY (realm_id) REFERENCES public.realm(id);


--
-- TOC entry 3962 (class 2606 OID 25444)
-- Name: realm_default_groups fk_def_groups_realm; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.realm_default_groups
    ADD CONSTRAINT fk_def_groups_realm FOREIGN KEY (realm_id) REFERENCES public.realm(id);


--
-- TOC entry 3957 (class 2606 OID 25304)
-- Name: user_federation_mapper_config fk_fedmapper_cfg; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_federation_mapper_config
    ADD CONSTRAINT fk_fedmapper_cfg FOREIGN KEY (user_federation_mapper_id) REFERENCES public.user_federation_mapper(id);


--
-- TOC entry 3955 (class 2606 OID 25299)
-- Name: user_federation_mapper fk_fedmapperpm_fedprv; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_federation_mapper
    ADD CONSTRAINT fk_fedmapperpm_fedprv FOREIGN KEY (federation_provider_id) REFERENCES public.user_federation_provider(id);


--
-- TOC entry 3956 (class 2606 OID 25294)
-- Name: user_federation_mapper fk_fedmapperpm_realm; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_federation_mapper
    ADD CONSTRAINT fk_fedmapperpm_realm FOREIGN KEY (realm_id) REFERENCES public.realm(id);


--
-- TOC entry 3975 (class 2606 OID 25662)
-- Name: associated_policy fk_frsr5s213xcx4wnkog82ssrfy; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.associated_policy
    ADD CONSTRAINT fk_frsr5s213xcx4wnkog82ssrfy FOREIGN KEY (associated_policy_id) REFERENCES public.resource_server_policy(id);


--
-- TOC entry 3973 (class 2606 OID 25647)
-- Name: scope_policy fk_frsrasp13xcx4wnkog82ssrfy; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.scope_policy
    ADD CONSTRAINT fk_frsrasp13xcx4wnkog82ssrfy FOREIGN KEY (policy_id) REFERENCES public.resource_server_policy(id);


--
-- TOC entry 3982 (class 2606 OID 26020)
-- Name: resource_server_perm_ticket fk_frsrho213xcx4wnkog82sspmt; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_server_perm_ticket
    ADD CONSTRAINT fk_frsrho213xcx4wnkog82sspmt FOREIGN KEY (resource_server_id) REFERENCES public.resource_server(id);


--
-- TOC entry 3965 (class 2606 OID 25863)
-- Name: resource_server_resource fk_frsrho213xcx4wnkog82ssrfy; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_server_resource
    ADD CONSTRAINT fk_frsrho213xcx4wnkog82ssrfy FOREIGN KEY (resource_server_id) REFERENCES public.resource_server(id);


--
-- TOC entry 3983 (class 2606 OID 26025)
-- Name: resource_server_perm_ticket fk_frsrho213xcx4wnkog83sspmt; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_server_perm_ticket
    ADD CONSTRAINT fk_frsrho213xcx4wnkog83sspmt FOREIGN KEY (resource_id) REFERENCES public.resource_server_resource(id);


--
-- TOC entry 3984 (class 2606 OID 26030)
-- Name: resource_server_perm_ticket fk_frsrho213xcx4wnkog84sspmt; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_server_perm_ticket
    ADD CONSTRAINT fk_frsrho213xcx4wnkog84sspmt FOREIGN KEY (scope_id) REFERENCES public.resource_server_scope(id);


--
-- TOC entry 3976 (class 2606 OID 25657)
-- Name: associated_policy fk_frsrpas14xcx4wnkog82ssrfy; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.associated_policy
    ADD CONSTRAINT fk_frsrpas14xcx4wnkog82ssrfy FOREIGN KEY (policy_id) REFERENCES public.resource_server_policy(id);


--
-- TOC entry 3974 (class 2606 OID 25642)
-- Name: scope_policy fk_frsrpass3xcx4wnkog82ssrfy; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.scope_policy
    ADD CONSTRAINT fk_frsrpass3xcx4wnkog82ssrfy FOREIGN KEY (scope_id) REFERENCES public.resource_server_scope(id);


--
-- TOC entry 3985 (class 2606 OID 26052)
-- Name: resource_server_perm_ticket fk_frsrpo2128cx4wnkog82ssrfy; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_server_perm_ticket
    ADD CONSTRAINT fk_frsrpo2128cx4wnkog82ssrfy FOREIGN KEY (policy_id) REFERENCES public.resource_server_policy(id);


--
-- TOC entry 3967 (class 2606 OID 25858)
-- Name: resource_server_policy fk_frsrpo213xcx4wnkog82ssrfy; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_server_policy
    ADD CONSTRAINT fk_frsrpo213xcx4wnkog82ssrfy FOREIGN KEY (resource_server_id) REFERENCES public.resource_server(id);


--
-- TOC entry 3969 (class 2606 OID 25612)
-- Name: resource_scope fk_frsrpos13xcx4wnkog82ssrfy; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_scope
    ADD CONSTRAINT fk_frsrpos13xcx4wnkog82ssrfy FOREIGN KEY (resource_id) REFERENCES public.resource_server_resource(id);


--
-- TOC entry 3971 (class 2606 OID 25627)
-- Name: resource_policy fk_frsrpos53xcx4wnkog82ssrfy; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_policy
    ADD CONSTRAINT fk_frsrpos53xcx4wnkog82ssrfy FOREIGN KEY (resource_id) REFERENCES public.resource_server_resource(id);


--
-- TOC entry 3972 (class 2606 OID 25632)
-- Name: resource_policy fk_frsrpp213xcx4wnkog82ssrfy; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_policy
    ADD CONSTRAINT fk_frsrpp213xcx4wnkog82ssrfy FOREIGN KEY (policy_id) REFERENCES public.resource_server_policy(id);


--
-- TOC entry 3970 (class 2606 OID 25617)
-- Name: resource_scope fk_frsrps213xcx4wnkog82ssrfy; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_scope
    ADD CONSTRAINT fk_frsrps213xcx4wnkog82ssrfy FOREIGN KEY (scope_id) REFERENCES public.resource_server_scope(id);


--
-- TOC entry 3966 (class 2606 OID 25868)
-- Name: resource_server_scope fk_frsrso213xcx4wnkog82ssrfy; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_server_scope
    ADD CONSTRAINT fk_frsrso213xcx4wnkog82ssrfy FOREIGN KEY (resource_server_id) REFERENCES public.resource_server(id);


--
-- TOC entry 3923 (class 2606 OID 24878)
-- Name: composite_role fk_gr7thllb9lu8q4vqa4524jjy8; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.composite_role
    ADD CONSTRAINT fk_gr7thllb9lu8q4vqa4524jjy8 FOREIGN KEY (child_role) REFERENCES public.keycloak_role(id);


--
-- TOC entry 3981 (class 2606 OID 25995)
-- Name: user_consent_client_scope fk_grntcsnt_clsc_usc; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_consent_client_scope
    ADD CONSTRAINT fk_grntcsnt_clsc_usc FOREIGN KEY (user_consent_id) REFERENCES public.user_consent(id);


--
-- TOC entry 3950 (class 2606 OID 25158)
-- Name: user_consent fk_grntcsnt_user; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_consent
    ADD CONSTRAINT fk_grntcsnt_user FOREIGN KEY (user_id) REFERENCES public.user_entity(id);


--
-- TOC entry 3960 (class 2606 OID 25418)
-- Name: group_attribute fk_group_attribute_group; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.group_attribute
    ADD CONSTRAINT fk_group_attribute_group FOREIGN KEY (group_id) REFERENCES public.keycloak_group(id);


--
-- TOC entry 3959 (class 2606 OID 25432)
-- Name: group_role_mapping fk_group_role_group; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.group_role_mapping
    ADD CONSTRAINT fk_group_role_group FOREIGN KEY (group_id) REFERENCES public.keycloak_group(id);


--
-- TOC entry 3947 (class 2606 OID 25104)
-- Name: realm_enabled_event_types fk_h846o4h0w8epx5nwedrf5y69j; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.realm_enabled_event_types
    ADD CONSTRAINT fk_h846o4h0w8epx5nwedrf5y69j FOREIGN KEY (realm_id) REFERENCES public.realm(id);


--
-- TOC entry 3927 (class 2606 OID 24888)
-- Name: realm_events_listeners fk_h846o4h0w8epx5nxev9f5y69j; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.realm_events_listeners
    ADD CONSTRAINT fk_h846o4h0w8epx5nxev9f5y69j FOREIGN KEY (realm_id) REFERENCES public.realm(id);


--
-- TOC entry 3948 (class 2606 OID 25148)
-- Name: identity_provider_mapper fk_idpm_realm; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.identity_provider_mapper
    ADD CONSTRAINT fk_idpm_realm FOREIGN KEY (realm_id) REFERENCES public.realm(id);


--
-- TOC entry 3949 (class 2606 OID 25318)
-- Name: idp_mapper_config fk_idpmconfig; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.idp_mapper_config
    ADD CONSTRAINT fk_idpmconfig FOREIGN KEY (idp_mapper_id) REFERENCES public.identity_provider_mapper(id);


--
-- TOC entry 3937 (class 2606 OID 24898)
-- Name: web_origins fk_lojpho213xcx4wnkog82ssrfy; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.web_origins
    ADD CONSTRAINT fk_lojpho213xcx4wnkog82ssrfy FOREIGN KEY (client_id) REFERENCES public.client(id);


--
-- TOC entry 3931 (class 2606 OID 24908)
-- Name: scope_mapping fk_ouse064plmlr732lxjcn1q5f1; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.scope_mapping
    ADD CONSTRAINT fk_ouse064plmlr732lxjcn1q5f1 FOREIGN KEY (client_id) REFERENCES public.client(id);


--
-- TOC entry 3941 (class 2606 OID 25043)
-- Name: protocol_mapper fk_pcm_realm; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.protocol_mapper
    ADD CONSTRAINT fk_pcm_realm FOREIGN KEY (client_id) REFERENCES public.client(id);


--
-- TOC entry 3924 (class 2606 OID 24923)
-- Name: credential fk_pfyr0glasqyl0dei3kl69r6v0; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.credential
    ADD CONSTRAINT fk_pfyr0glasqyl0dei3kl69r6v0 FOREIGN KEY (user_id) REFERENCES public.user_entity(id);


--
-- TOC entry 3942 (class 2606 OID 25311)
-- Name: protocol_mapper_config fk_pmconfig; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.protocol_mapper_config
    ADD CONSTRAINT fk_pmconfig FOREIGN KEY (protocol_mapper_id) REFERENCES public.protocol_mapper(id);


--
-- TOC entry 3980 (class 2606 OID 25980)
-- Name: default_client_scope fk_r_def_cli_scope_realm; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.default_client_scope
    ADD CONSTRAINT fk_r_def_cli_scope_realm FOREIGN KEY (realm_id) REFERENCES public.realm(id);


--
-- TOC entry 3958 (class 2606 OID 25353)
-- Name: required_action_provider fk_req_act_realm; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.required_action_provider
    ADD CONSTRAINT fk_req_act_realm FOREIGN KEY (realm_id) REFERENCES public.realm(id);


--
-- TOC entry 3987 (class 2606 OID 26060)
-- Name: resource_uris fk_resource_server_uris; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.resource_uris
    ADD CONSTRAINT fk_resource_server_uris FOREIGN KEY (resource_id) REFERENCES public.resource_server_resource(id);


--
-- TOC entry 3988 (class 2606 OID 26074)
-- Name: role_attribute fk_role_attribute_id; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.role_attribute
    ADD CONSTRAINT fk_role_attribute_id FOREIGN KEY (role_id) REFERENCES public.keycloak_role(id);


--
-- TOC entry 3946 (class 2606 OID 25073)
-- Name: realm_supported_locales fk_supported_locales_realm; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.realm_supported_locales
    ADD CONSTRAINT fk_supported_locales_realm FOREIGN KEY (realm_id) REFERENCES public.realm(id);


--
-- TOC entry 3933 (class 2606 OID 24943)
-- Name: user_federation_config fk_t13hpu1j94r2ebpekr39x5eu5; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_federation_config
    ADD CONSTRAINT fk_t13hpu1j94r2ebpekr39x5eu5 FOREIGN KEY (user_federation_provider_id) REFERENCES public.user_federation_provider(id);


--
-- TOC entry 3961 (class 2606 OID 25425)
-- Name: user_group_membership fk_user_group_user; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.user_group_membership
    ADD CONSTRAINT fk_user_group_user FOREIGN KEY (user_id) REFERENCES public.user_entity(id);


--
-- TOC entry 3968 (class 2606 OID 25602)
-- Name: policy_config fkdc34197cf864c4e43; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.policy_config
    ADD CONSTRAINT fkdc34197cf864c4e43 FOREIGN KEY (policy_id) REFERENCES public.resource_server_policy(id);


--
-- TOC entry 3945 (class 2606 OID 25053)
-- Name: identity_provider_config fkdc4897cf864c4e43; Type: FK CONSTRAINT; Schema: public; Owner: keycloakuser
--

ALTER TABLE ONLY public.identity_provider_config
    ADD CONSTRAINT fkdc4897cf864c4e43 FOREIGN KEY (identity_provider_id) REFERENCES public.identity_provider(internal_id);


-- Completed on 2026-01-26 13:53:37

--
-- PostgreSQL database dump complete
--

-- Completed on 2026-01-26 13:53:37

--
-- PostgreSQL database cluster dump complete
--

