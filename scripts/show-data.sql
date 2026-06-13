-- Zeigt Struktur und Inhalt aller Tabellen der UBahn-Datenbank.
-- Verwendung (bereits in der DB eingeloggt, nach 'USE UBahn;'):
--   SOURCE C:/Pfad/zu/scripts/show-data.sql;
-- oder Inhalt direkt in die mysql-Konsole einfuegen.

SELECT '========== STRUKTUR: Linien ==========' AS '';
DESCRIBE Linien;
SELECT '========== DATEN: Linien ==========' AS '';
SELECT * FROM Linien;

SELECT '========== STRUKTUR: Stationen ==========' AS '';
DESCRIBE Stationen;
SELECT '========== DATEN: Stationen ==========' AS '';
SELECT * FROM Stationen ORDER BY LinieId, Position;

SELECT '========== STRUKTUR: Fahrzeiten ==========' AS '';
DESCRIBE Fahrzeiten;
SELECT '========== DATEN: Fahrzeiten ==========' AS '';
SELECT * FROM Fahrzeiten;
