-- U-Bahn Datenbankinitialisierung
-- Einmalig als root ausfuehren (z.B. in HeidiSQL oder MySQL Workbench)

CREATE DATABASE IF NOT EXISTS UBahn
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

CREATE USER IF NOT EXISTS 'admin'@'localhost' IDENTIFIED BY 'sml12345';

GRANT ALL PRIVILEGES ON UBahn.* TO 'admin'@'localhost';

FLUSH PRIVILEGES;
