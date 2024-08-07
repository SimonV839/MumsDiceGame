       USE MumsDiceGame
       
	   ---------------------------------------------------
       -- Create a new table called 'GameUsers' in schema 'dbo'
       -- Drop the table if it already exists
       IF OBJECT_ID('dbo.GameUsers', 'U') IS NOT NULL
           DROP TABLE dbo.GameUsers
       GO
       
       -- Create the table in the specified schema
       CREATE TABLE dbo.GameUsers (
           GameUserId INT NOT NULL IDENTITY(1,1) PRIMARY KEY, -- primary key column
           Name VARCHAR(12) UNIQUE
		          );
       GO

	   ---------------------------------------------------
       -- Create a new table called 'GameRequests' in schema 'dbo'
       -- Drop the table if it already exists
       IF OBJECT_ID('dbo.GameRequests', 'U') IS NOT NULL
           DROP TABLE dbo.GameRequests
       GO
       
       -- Create the table in the specified schema
       CREATE TABLE dbo.GameRequests (
           GameRequestId INT NOT NULL IDENTITY(1,1) PRIMARY KEY, -- primary key column
           GameUserId INT FOREIGN KEY (GameUserId) REFERENCES GameUsers(GameUserId),
           OpponentName CHAR(12) NULL
		          );
       GO

	   ---------------------------------------------------
       -- Create a new table called 'DiceGames' in schema 'dbo'
       -- Drop the table if it already exists
       IF OBJECT_ID('dbo.DiceGames', 'U') IS NOT NULL
           DROP TABLE dbo.DiceGames
       GO
       
       -- Create the table in the specified schema
       CREATE TABLE dbo.DiceGames (
           DiceGameId INT NOT NULL IDENTITY(1,1) PRIMARY KEY, -- primary key column
           Player1Id INT FOREIGN KEY (Player1Id) REFERENCES GameUsers(GameUserId),
           Player2Id INT FOREIGN KEY (Player2Id) REFERENCES GameUsers(GameUserId),
		   IsPlayer1Turn BIT,
		   IsAborted BIT
		          );
       GO


	   ---------------------------------------------------
       -- Update table called 'GameUsers' in schema 'dbo' with foreign keys of newly created tables
	   ALTER TABLE GameUsers ADD 
	       GameRequestId INT  NULL,
		   -- SIMON: note 'ON DELETE SET NULL,'
		   FOREIGN KEY (GameRequestId) REFERENCES GameRequests(GameRequestId) ON DELETE SET NULL,
	       DiceGameId INT  NULL,
		   FOREIGN KEY (DiceGameId ) REFERENCES DiceGames(DiceGameId) ON DELETE SET NULL,
		   constraint chk_notBothMatchAndRequest check (not (GameRequestId is not null and DiceGameId is not null))

		GO

	   ---------------------------------------------------
       -- Create stored procedure InsertGameRequest
		DROP PROCEDURE InsertGameRequest;
		GO

	   CREATE PROCEDURE InsertGameRequest
	       @gameUserId INT,
	       @opponentName CHAR(12) NULL
	   AS
	   BEGIN
		SET NOCOUNT ON;
			BEGIN TRY
				INSERT INTO dbo.GameRequests (GameUserId, OpponentName)
				VALUES (@gameUserId, @opponentName);

				UPDATE dbo.GameUsers
				SET [GameRequestId]   = SCOPE_IDENTITY()
				WHERE [GameUserId] = @gameUserId;
			END TRY
			BEGIN CATCH
				DECLARE @ErrorMessage NVARCHAR(2048);
				DECLARE @ErrorSeverity INT;
				DECLARE @ErrorState INT;
 
				SELECT
				ERROR_NUMBER()    AS [ErrorNumber],
				ERROR_SEVERITY()  AS [ErrorSeverity],
				ERROR_STATE()     AS [ErrorState],
				ERROR_PROCEDURE() AS [ErrorProcedure],
				ERROR_LINE()      AS [ErrorLine],
				ERROR_MESSAGE()   AS [ErrorMessage]
				INTO #Error_Log;
 
				RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
			END CATCH;
	   END

	   GO

	   ---------------------------------------------------
       -- Create stored procedure InsertDiceGame
		DROP PROCEDURE InsertDiceGame;
		GO

	   CREATE PROCEDURE InsertDiceGame
	       @player1Id INT,
		   @player2Id INT
	   AS
	   BEGIN
		SET NOCOUNT ON;
			BEGIN TRY
				DECLARE @matchRequest1 INT;
				DECLARE @matchRequest2 INT;
				DECLARE @name1 CHAR(12);
				DECLARE @name2 CHAR(12);
				SELECT @name1 = Name, @matchRequest1 = GameRequestId FROM dbo.GameUsers WHERE GameUserId = @player1Id;
				SELECT @name2 = Name, @matchRequest2 = GameRequestId FROM dbo.GameUsers WHERE GameUserId = @player2Id;

				IF (@name1 = null OR @name2 = null) 
				BEGIN
					THROW 51000, 'A DiceGame requires both GameUsers to be present.', 1;
				END

				IF (@matchRequest1 IS NOT null)
				BEGIN
					BEGIN TRY
						DELETE FROM dbo.GameRequests WHERE GameRequestId = @matchRequest1;
					END TRY
					BEGIN CATCH
						THROW 51000, 'Could not delete the request associated with the DiceGame.', 1;
					END CATCH
				END

				IF (@matchRequest2 IS NOT null)
				BEGIN
					BEGIN TRY
						DELETE FROM dbo.GameRequests WHERE GameRequestId = @matchRequest2;
					END TRY
					BEGIN CATCH
						THROW 51000, 'Could not delete the request associated with the DiceGame.', 1;
					END CATCH
				END

				DELETE FROM dbo.GameRequests 
					WHERE GameUserId = @player1Id OR GameUserId = @player2Id OR
						OpponentName = @name1 OR OpponentName = @name2;

				INSERT INTO dbo.DiceGames(Player1Id, Player2Id, IsPlayer1Turn, IsAborted)
					VALUES (@player1Id, @player2Id, 1, 0);

				UPDATE GameUsers
					SET DiceGameId = SCOPE_IDENTITY() 
					WHERE GameUserId IN (@player1Id, @player2Id)

			END TRY
			BEGIN CATCH
				DECLARE @ErrorMessage NVARCHAR(2048);
				DECLARE @ErrorSeverity INT;
				DECLARE @ErrorState INT;
 
				SELECT
				ERROR_NUMBER()    AS [ErrorNumber],
				ERROR_SEVERITY()  AS [ErrorSeverity],
				ERROR_STATE()     AS [ErrorState],
				ERROR_PROCEDURE() AS [ErrorProcedure],
				ERROR_LINE()      AS [ErrorLine],
				ERROR_MESSAGE()   AS [ErrorMessage]
				INTO #Error_Log;
 
				RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
			END CATCH;
	   END

	   GO




	   -- Populate the GameUsers table
	   INSERT INTO dbo.GameUsers 
			([Name], [GameRequestId], [DiceGameId])
		VALUES
			('user1', null, null),
			('user2', null, null);
		
		GO

	   -- Populate the GameUsers table
	   EXEC InsertGameRequest @gameUserId = 1, @opponentName = null;
	   SELECT @@ERROR

	   EXEC InsertDiceGame @player1Id = 1, @player2Id = 2;
	   SELECT @@ERROR
	
		GO


		select * from GameUsers;
		select * from GameRequests;
		select * from DiceGames;

		delete from GameRequests;
		delete from DiceGames;


