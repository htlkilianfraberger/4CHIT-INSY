select * from scottnew.emps;

create user 'e'@'%' IDENTIFIED BY 'e';
grant select on scottnew.emps to 'e'@'%';
grant insert on scottnew.emps to 'e'@'%';
flush privileges;

create user 'e1'@'172.17.0.1' IDENTIFIED BY 'e';
grant select on scottnew.emps to 'e1'@'172.17.0.1';
flush privileges;

DROP USER 'e'@'%';
DROP USER 'e1'@'172.17.0.1';
FLUSH PRIVILEGES;

-- Rollen erstellen
CREATE ROLE read_emps;
CREATE ROLE read_depts;
CREATE ROLE write_emps;
CREATE ROLE admin_role;

-- Berechtigungen für read_emps zuweisen
GRANT SELECT ON scottnew.emps TO read_emps;

-- Berechtigungen für read_depts zuweisen
GRANT SELECT ON scottnew.depts TO read_depts;

-- Berechtigungen für write_emps zuweisen
GRANT INSERT, UPDATE, DELETE ON scottnew.emps TO write_emps;

-- Die Admin-Rolle erhält alle anderen Rollen zugewiesen
GRANT read_emps, read_depts, write_emps TO admin_role;

FLUSH PRIVILEGES;

-- Benutzer erstellen (bitte sichere Passwörter verwenden!)
CREATE USER 'readonly_user'@'%' IDENTIFIED BY 'securepass1';
CREATE USER 'writer_user'@'%' IDENTIFIED BY 'securepass2';
CREATE USER 'app_admin'@'172.17.0.1' IDENTIFIED BY 'securepass3';

-- Rollen den Benutzern zuweisen
GRANT read_emps, read_depts TO 'readonly_user'@'%';
GRANT read_emps, write_emps TO 'writer_user'@'%';
GRANT admin_role TO 'app_admin'@'172.17.0.1';

-- Wichtig: Damit der Benutzer die Rolle verwenden kann, muss sie standardmäßig aktiviert werden
SET DEFAULT ROLE ALL TO 'readonly_user'@'%', 'writer_user'@'%', 'app_admin'@'172.17.0.1';

FLUSH PRIVILEGES;
