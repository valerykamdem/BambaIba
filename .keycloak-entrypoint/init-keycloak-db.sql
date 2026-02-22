-- Utilisateur pour l'application métier
CREATE USER bambaiba_user WITH PASSWORD 'bambaiba_pass';

-- Base métier
CREATE DATABASE bambaiba_db
    WITH OWNER = bambaiba_user
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.utf8'
    LC_CTYPE = 'en_US.utf8'
    TEMPLATE = template0;

-- Donner les droits
GRANT ALL PRIVILEGES ON DATABASE bambaiba_db TO bambaiba_user;


-- Utilisateur pour Keycloak
CREATE USER keycloak_user WITH PASSWORD 'keycloak_pass';

-- Base Keycloak
CREATE DATABASE bambaiba_idp_db
    WITH OWNER = keycloak_user
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.utf8'
    LC_CTYPE = 'en_US.utf8'
    TEMPLATE = template0;

-- Donner les droits
GRANT ALL PRIVILEGES ON DATABASE bambaiba_idp_db TO keycloak_user;
