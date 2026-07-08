USE LibraryDB;
GO

-- =============================================
-- CONFIGURABLE SEED PARAMETERS
-- =============================================
DECLARE @NumAuthors        INT = 50;   -- Change number of authors
DECLARE @NumBooks          INT = 100;  -- Change number of books  
DECLARE @NumReviews        INT = 500;  -- Change number of reviews
DECLARE @MaxAuthorsPerBook INT = 5;    -- Max authors per book

-- =============================================
-- Clear existing data
-- =============================================
DELETE FROM bookauthors;
DELETE FROM reviews;
DELETE FROM books;
DELETE FROM authors;

-- Reset IDENTITY seeds
DBCC CHECKIDENT ('authors', RESEED, 0);
DBCC CHECKIDENT ('books', RESEED, 0);
DBCC CHECKIDENT ('reviews', RESEED, 0);
GO

-- =============================================
-- Seed Authors
-- =============================================
DECLARE @NumAuthors2        INT = 50;   -- duplicate for this batch
DECLARE @author_start       INT = 1;

SET IDENTITY_INSERT authors ON;

DECLARE @names TABLE (name NVARCHAR(100));
INSERT INTO @names VALUES
('James'),('Mary'),('Robert'),('Patricia'),('John'),('Jennifer'),('Michael'),
('Linda'),('William'),('Elizabeth'),('David'),('Barbara'),('Richard'),('Susan'),
('Joseph'),('Jessica'),('Thomas'),('Charles'),('Karen'),('Christopher');

DECLARE @surnames TABLE (surname NVARCHAR(100));
INSERT INTO @surnames VALUES
('Smith'),('Johnson'),('Williams'),('Brown'),('Jones'),('Garcia'),('Miller'),
('Davis'),('Rodriguez'),('Martinez'),('Hernandez'),('Lopez'),('Gonzalez'),
('Wilson'),('Anderson'),('Thomas'),('Taylor'),('Moore'),('Jackson'),('Martin');

;WITH AuthorGen AS (
    SELECT 
        n.name,
        s.surname,
        ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
    FROM (SELECT name, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn FROM @names) n
    CROSS JOIN (SELECT surname, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn FROM @surnames) s
)
INSERT INTO authors (id, name, surname)
SELECT 
    @author_start + rn - 1,
    name,
    surname
FROM AuthorGen
WHERE rn <= @NumAuthors2;

SET IDENTITY_INSERT authors OFF;
GO

-- =============================================
-- Seed Books
-- =============================================
DECLARE @NumBooks2   INT = 100;
DECLARE @book_start  INT = 1;

SET IDENTITY_INSERT books ON;

;WITH numbers AS (
    SELECT TOP (@NumBooks2) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
    FROM sys.all_objects
)
INSERT INTO books (id, title, publication_year)
SELECT
    @book_start + n - 1,
    CONCAT('Book Title ', @book_start + n - 1),
    1980 + ABS(CHECKSUM(NEWID())) % 45
FROM numbers;

SET IDENTITY_INSERT books OFF;
GO

-- =============================================
-- Seed BookAuthors (max @MaxAuthorsPerBook per book)
-- =============================================
DECLARE @MaxAuthorsPerBook2 INT = 5;

;WITH random_assign AS (
    SELECT
        b.id AS book_id,
        a.id AS author_id,
        ROW_NUMBER() OVER (PARTITION BY b.id ORDER BY NEWID()) AS rn,
        1 + ABS(CHECKSUM(NEWID())) % @MaxAuthorsPerBook2 AS take_count
    FROM books b
    CROSS JOIN authors a
)
INSERT INTO bookauthors (author_id, book_id)
SELECT DISTINCT author_id, book_id
FROM random_assign
WHERE rn <= take_count;
GO

-- =============================================
-- Seed Reviews
-- =============================================
DECLARE @NumReviews2   INT = 500;
DECLARE @review_start  INT = 1;

SET IDENTITY_INSERT reviews ON;

;WITH numbers AS (
    SELECT TOP (@NumReviews2) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
    FROM sys.all_objects a
    CROSS JOIN sys.all_objects b
)
INSERT INTO reviews (id, score, book_id)
SELECT
    @review_start + n.n - 1,
    1 + ABS(CHECKSUM(NEWID())) % 10,
    (SELECT TOP 1 id FROM books ORDER BY CHECKSUM(NEWID(), n.n))
FROM numbers n
OPTION (MAXRECURSION 0);

SET IDENTITY_INSERT reviews OFF;
GO

-- Summary
SELECT 
    (SELECT COUNT(*) FROM authors)     AS TotalAuthors,
    (SELECT COUNT(*) FROM books)       AS TotalBooks,
    (SELECT COUNT(*) FROM bookauthors) AS BookAuthorLinks,
    (SELECT COUNT(*) FROM reviews)     AS TotalReviews;