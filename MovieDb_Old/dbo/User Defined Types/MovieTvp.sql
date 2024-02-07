CREATE TYPE [dbo].[MovieTvp] AS TABLE (
    [Title]    VARCHAR (50)  NOT NULL,
    [Genre]    VARCHAR (50)  NOT NULL,
    [Year]     INT           NOT NULL,
    [ImageUrl] VARCHAR (200) NOT NULL,
    PRIMARY KEY CLUSTERED ([Title] ASC));

