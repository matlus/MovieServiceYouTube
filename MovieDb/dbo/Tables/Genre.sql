CREATE TABLE [dbo].[Genre] (
    [Id]    INT          IDENTITY (1, 1) NOT NULL,
    [Title] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Genre] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Genre]
    ON [dbo].[Genre]([Title] ASC);

