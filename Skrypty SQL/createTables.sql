

CREATE TABLE [dbo].[Uzytkownik] (
    [IdUzytkownika] INT           IDENTITY (1, 1) NOT NULL,
    [Login]         VARCHAR (50)  NOT NULL,
    [Haslo]         VARCHAR (200) NOT NULL,
    PRIMARY KEY CLUSTERED ([IdUzytkownika] ASC)
);
CREATE TABLE [dbo].[Admin] (
    [IdAdmina]      INT           IDENTITY (1, 1) NOT NULL,
    [Imie]          NVARCHAR (50) NOT NULL,
    [Nazwisko]      NVARCHAR (50) NOT NULL,
    [IdUzytkownika] INT           NULL,
    PRIMARY KEY CLUSTERED ([IdAdmina] ASC),
    CONSTRAINT [FK_Admin_Uzytkownik] FOREIGN KEY ([IdUzytkownika]) REFERENCES [dbo].[Uzytkownik] ([IdUzytkownika]) ON DELETE CASCADE
);
CREATE TABLE [dbo].[Lekarz] (
    [IdLekarza]        INT           IDENTITY (1, 1) NOT NULL,
    [Imie]             NVARCHAR (50) NOT NULL,
    [Nazwisko]         NVARCHAR (50) NOT NULL,
    [Adres]            NCHAR (50)    NOT NULL,
    [DataZatrudnienia] DATE          NOT NULL,
    [NumerTelefonu]    NCHAR (10)    NOT NULL,
    [IdUzytkownika]    INT           NULL,
    [DataRozpWizyt]    DATE          NULL,
    [DataZakWizyt]     DATE          NULL,
    PRIMARY KEY CLUSTERED ([IdLekarza] ASC),
    CONSTRAINT [FK_Lekarz_Uzytkownik] FOREIGN KEY ([IdUzytkownika]) REFERENCES [dbo].[Uzytkownik] ([IdUzytkownika]) ON DELETE CASCADE
);
CREATE TABLE [dbo].[DzienGodzinaPracyLekarza] (
    [IdDzienGodzina]   INT       IDENTITY (1, 1) NOT NULL,
    [DzienTygodnia]    INT       NOT NULL,
    [GodzinaRozp]      NCHAR (5) NOT NULL,
    [GodzinaZak]       NCHAR (5) NOT NULL,
    [CzasJednejWizyty] NCHAR (3) NOT NULL,
    [IdLekarza]        INT       NOT NULL,
    [IdGabinetu]       INT       NOT NULL,
    PRIMARY KEY CLUSTERED ([IdDzienGodzina] ASC),
    CONSTRAINT [FK_DzienGodzinaPracyLekarza_Lekarz] FOREIGN KEY ([IdLekarza]) REFERENCES [dbo].[Lekarz] ([IdLekarza]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[Pacjent] (
    [IdPacjenta]    INT           IDENTITY (1, 1) NOT NULL,
    [Imie]          NVARCHAR (50) NOT NULL,
    [Nazwisko]      NVARCHAR (50) NOT NULL,
    [Adres]         NVARCHAR (50) NOT NULL,
    [Pesel]         NVARCHAR (11) NOT NULL,
    [NumerTelefonu] NCHAR (10)    NOT NULL,
    [Zatwierdzono]  BIT           DEFAULT ((0)) NOT NULL,
    [Miasto]        VARCHAR (50)  NOT NULL,
    [KodPocztowy]   VARCHAR (50)  NOT NULL,
    [IdUzytkownika] INT           NULL,
    PRIMARY KEY CLUSTERED ([IdPacjenta] ASC),
    CONSTRAINT [FK_Pacjent_Uzytkownik] FOREIGN KEY ([IdUzytkownika]) REFERENCES [dbo].[Uzytkownik] ([IdUzytkownika]) ON DELETE CASCADE
);
CREATE TABLE [dbo].[Recepcjonista] (
    [IdRecepjonisty]   INT        IDENTITY (1, 1) NOT NULL,
    [Imie]             NCHAR (50) NOT NULL,
    [Nazwisko]         NCHAR (50) NOT NULL,
    [Adres]            NCHAR (50) NOT NULL,
    [DataZatrudnienia] DATE       NOT NULL,
    [NumerTelefonu]    NCHAR (10) NULL,
    [IdUzytkownika]    INT        NULL,
    PRIMARY KEY CLUSTERED ([IdRecepjonisty] ASC),
    CONSTRAINT [FK_Recepcjonista_Uzytkownik] FOREIGN KEY ([IdUzytkownika]) REFERENCES [dbo].[Uzytkownik] ([IdUzytkownika]) ON DELETE CASCADE
);
CREATE TABLE [dbo].[Rejestracja] (
    [IdRejestracji] INT       IDENTITY (1, 1) NOT NULL,
    [DataRozp]      DATETIME  NOT NULL,
    [DataZak]       DATETIME  NOT NULL,
    [CzyZajeta]     NCHAR (1) NOT NULL,
    [IdLekarza]     INT       NOT NULL,
    [IdPacjenta]    INT       NULL,
    PRIMARY KEY CLUSTERED ([IdRejestracji] ASC),
    CONSTRAINT [FK_Rejestracja_Pacjent] FOREIGN KEY ([IdPacjenta]) REFERENCES [dbo].[Pacjent] ([IdPacjenta]),
    CONSTRAINT [FK_Rejestracja_Lekarz] FOREIGN KEY ([IdLekarza]) REFERENCES [dbo].[Lekarz] ([IdLekarza])
);

CREATE TABLE [dbo].[Rola] (
    [IdRoli]    INT           IDENTITY (1, 1) NOT NULL,
    [NazwaRoli] VARCHAR (100) DEFAULT ('') NULL,
    [OpisRoli]  VARCHAR (500) DEFAULT ('') NULL,
    PRIMARY KEY CLUSTERED ([IdRoli] ASC)
);
CREATE TABLE [dbo].[RolaUzytkownika] (
    [IdRoliUzytkownika] INT IDENTITY (1, 1) NOT NULL,
    [IdRoli]            INT NOT NULL,
    [IdUzytkownika]     INT NOT NULL,
    PRIMARY KEY CLUSTERED ([IdRoliUzytkownika] ASC),
    CONSTRAINT [FK_RolaUzytkownika_Uzytkownik] FOREIGN KEY ([IdUzytkownika]) REFERENCES [dbo].[Uzytkownik] ([IdUzytkownika]) ON DELETE CASCADE,
    CONSTRAINT [FK_RolaUzytkownika_Rola] FOREIGN KEY ([IdRoli]) REFERENCES [dbo].[Rola] ([IdRoli])
);
CREATE TABLE [dbo].[Specjalizacja] (
    [IdSpecjalizacji]    INT        IDENTITY (1, 1) NOT NULL,
    [NazwaSpecjalizacji] NCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([IdSpecjalizacji] ASC),
    CONSTRAINT [FK_Specjalizacja_SpecjalizacjaLekarza] FOREIGN KEY ([IdSpecjalizacji]) REFERENCES [dbo].[Specjalizacja] ([IdSpecjalizacji])
);
CREATE TABLE [dbo].[SpecjalizacjaLekarza] (
    [IdSpecjalizacjiLekarza] INT IDENTITY (1, 1) NOT NULL,
    [IdSpecjalizacji]        INT NOT NULL,
    [IdLekarza]              INT NOT NULL,
    PRIMARY KEY CLUSTERED ([IdSpecjalizacjiLekarza] ASC),
    CONSTRAINT [FK_SpecjalizacjaLekarza_Specjalizacja] FOREIGN KEY ([IdSpecjalizacji]) REFERENCES [dbo].[Specjalizacja] ([IdSpecjalizacji]) ON DELETE CASCADE,
    CONSTRAINT [FK_SpecjalizacjaLekarza_Lekarz] FOREIGN KEY ([IdLekarza]) REFERENCES [dbo].[Lekarz] ([IdLekarza]) ON DELETE CASCADE
);
CREATE TABLE [dbo].[Wizyta] (
    [IdWizyty]      INT       IDENTITY (1, 1) NOT NULL,
    [Koszt]         NCHAR (3) NULL,
    [IdRejestracji] INT       NOT NULL,
    PRIMARY KEY CLUSTERED ([IdWizyty] ASC),
    CONSTRAINT [FK_Wizyta_Rejestracja] FOREIGN KEY ([IdRejestracji]) REFERENCES [dbo].[Rejestracja] ([IdRejestracji]) ON DELETE CASCADE
);

