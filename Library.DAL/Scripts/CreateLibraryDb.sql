IF OBJECT_ID('dbo.Reviews', 'U') IS NOT NULL DROP TABLE dbo.Reviews;
IF OBJECT_ID('dbo.BookAuthors', 'U') IS NOT NULL DROP TABLE dbo.BookAuthors;
IF OBJECT_ID('dbo.Books', 'U') IS NOT NULL DROP TABLE dbo.Books;
IF OBJECT_ID('dbo.Authors', 'U') IS NOT NULL DROP TABLE dbo.Authors;
GO

CREATE TABLE Authors (
    Id      INT IDENTITY(1,1) PRIMARY KEY,
    Name    NVARCHAR(100) NOT NULL,
    Surname NVARCHAR(100) NOT NULL
);

CREATE TABLE Books (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    Title            NVARCHAR(200) NOT NULL,
    Publication_Year  INT NOT NULL
);

CREATE TABLE BookAuthors (
    Book_Id   INT NOT NULL REFERENCES Books(Id)   ON DELETE CASCADE,
    Author_Id INT NOT NULL REFERENCES Authors(Id) ON DELETE CASCADE,
    PRIMARY KEY (Book_Id, Author_Id)
);

CREATE TABLE Reviews (
    Id     INT IDENTITY(1,1) PRIMARY KEY,
    Book_Id INT NOT NULL REFERENCES Books(Id) ON DELETE CASCADE,
    Score  INT NOT NULL CHECK (Score BETWEEN 1 AND 10)
);
GO