CREATE PROCEDURE dbo.spCreateTeamsForLeague   
    @LeagueId int
AS   
	SELECT 
		0 as UserId
		, lt.TeamId
		, lt.Teamname
		, lt.ShortCode
		, lt.Mascot
		, lt.Division 
		, @LeagueId as LeagueId
	INTO dbo.Teams
	FROM Landing.Teams lt;
GO  


