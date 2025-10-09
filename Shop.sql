CREATE DATABASE IF NOT EXISTS shop;
USE shop;

DROP TABLE IF EXISTS taxes;
CREATE TABLE taxes (
                       tax_class INT PRIMARY KEY,
                       rate FLOAT -- Prozentwert (10, 13, 20)
);

INSERT INTO taxes (tax_class, rate) VALUES
                                        (1, 10),
                                        (2, 13),
                                        (3, 20);

SELECT * FROM taxes;


DROP TABLE IF EXISTS articles;
CREATE TABLE articles (
                          id INT PRIMARY KEY,
                          description VARCHAR(20),
                          netto FLOAT,
                          tax_class INT
);

-- Artikel einfügen inkl. Steuerklasse
INSERT INTO articles (id, description, netto, tax_class) VALUES
                                                             (1, 'Banana', 0.5, 1),
                                                             (2, 'Tee', 2.5, 2),
                                                             (3, 'Bread', 4.5, 3);

select * from articles;
select id, description, ROUND(netto * 1.2, 2) as brutto from articles;
select id, description, Mwst(netto, tax_class) as brutto from articles;

drop function if exists Mwst;
CREATE FUNCTION Mwst(net FLOAT, tax_class INT)
    RETURNS FLOAT -- float rundet automatisch auf 7 Nachkommastellen mit RETURNS
    DETERMINISTIC
BEGIN
    DECLARE rate FLOAT;
    DECLARE brutto FLOAT;

    SELECT t.rate INTO rate
    FROM taxes t
    WHERE t.tax_class = tax_class;

    SET brutto = net * (1 + rate/100);

    RETURN ROUND(brutto, 2);
END;
