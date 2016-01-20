INSERT INTO Rola values
('Administrator','admin'),
('Recepcjonista','rec'),
('Lekarz','lek'),
('Pacjent','pac')

INSERT INTO Admin
           (Imie, Nazwisko)
     VALUES

           ('Grzegorz', 'Kolarz')
		   
INSERT INTO Uzytkownik
VALUES
('admin1','admin123')
DECLARE @IdUz bigint = 0;

SELECT @IdUz = IdUzytkownika from Uzytkownik u where u.Login = 'admin1';

UPDATE Admin
SET IdUzytkownika = @IdUz
WHERE Imie = 'Grzegorz' and Nazwisko = 'Kolarz'

DECLARE @IdRoli bigint = 0;
Select @IdRoli = r.IdRoli from Rola r where r.NazwaRoli = 'Administrator';

Insert into RolaUzytkownika values
(@IdRoli, @IdUz)
