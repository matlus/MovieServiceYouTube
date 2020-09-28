CREATE TABLE [dbo].[Movie] (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [Title]    VARCHAR (50)  NOT NULL,
    [Year]     INT           NOT NULL,
    [ImageUrl] VARCHAR (200) NOT NULL,
    CONSTRAINT [PK_Movie] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Movie]
    ON [dbo].[Movie]([Title] ASC);

