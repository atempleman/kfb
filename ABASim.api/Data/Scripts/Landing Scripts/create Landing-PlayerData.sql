CREATE TABLE Landing_PlayerData (
    [SeasonId] INT,
    [PlayerId] INT,
    [FirstName] NVARCHAR(12),
    [Surname] NVARCHAR(18),
    [PGPosition] INT,
    [SGPosition] INT,
    [SFPosition] INT,
    [PFPosition] INT,
    [Cposition] NVARCHAR(2),
    [Age] INT,
    [TwoPointTendancy] INT,
    [ThreePointTendnacy] INT,
    [PassTendancy] INT,
    [FoulTendancy] INT,
    [TurnoverTendancy] INT,
    [TwoRating] INT,
    [ThreeRating] INT,
    [FTRating] INT,
    [ORebRating] INT,
    [DRebRating] INT,
    [AssitRating] INT,
    [PassAssistRating] INT,
    [StealRating] INT,
    [BlockRating] INT,
    [UsageRating] INT,
    [StaminaRating] INT,
    [ORPMRating] INT,
    [DRPMRating] INT,
    [FoulingRating] INT
);


CREATE TABLE Landing.RetiredPlayers (
	SeasonId INT,
	PlayerId INT
);

CREATE TABLE Landing.DraftPlayers (
	SeasonId INT,
	PlayerId INT
);