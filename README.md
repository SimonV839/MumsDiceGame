# MumsDiceGame
 Server and client that allow two players to play a game of dice

##Aborted:
See EfDbFirstConsole project. - In memory databases do not support foreign key integrity or stored procedures.
Will look at authentication before carrying on with this.

##Phase 1
Dummy services will be added which should later be turned into real services.

###References
https://www.youtube.com/watch?v=C_bYPn-OTtw&t=1394s

###Basic structure 
Users sign in (reception area).
Users register for a match (lounge area).
Users are paired and play a match (table area).
Users either play again or enter the lounge area.

###Services
Sign in service - users sign in. Their name will be unique for the session. They must maintain their source
	sign in with a name - check name unique
	sign out with a name - check source is as expected
	OnExpired

Register for match service - users request to play someone
	get list of available players
	register for match any available
	register for match specific oponent
	unregister for match
	OnOponentMatched
	OnOponentAvailablityhChanged

PlayerStates
	Watching
	Avaliable
	WaitingForSpecific
	WaitingForAny
	Playing

### Database / Db First Entity Framework
see: project ShipCrewsEFDatabaseFirst
See https://puresourcecode.com/dotnet/net-core/creating-a-model-for-an-existing-database-in-entity-framework-core/

nuget Microsoft.EntityFrameworkCore.Tools and Microsoft.EntityFrameworkCore.SqlServer
restart vs
Scaffold-DbContext "server=(local)\SQLEXPRESS;database=MumsDiceGame;Integrated Security=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models

For conversion to in memory, see: https://www.red-gate.com/simple-talk/databases/sql-server/t-sql-programming-sql-server/converting-database-memory-oltp/
limitations: My conclusion is that In-Memory OLTP does not support migrations in most cases. When I mention migration, I mean tables, referential integrity, check constraint and so on. It is good for new projects where you expect many transactions, but migrating an existing database is not properly supported.

The Microsoft.EntityFrameworkCore.InMemory provider is primarily designed for testing purposes and does not enforce foreign key constraints like a relational database would. This means that it allows you to save data that would violate referential integrity constraints in a relational database1.




###Steps
Blazor Web App
	Auto(Server and WebAssembly)
	Per page/component
[This creates the server and the .client]

Add Logging to the server (this allows the client to log too)

Add Hub
	ISignInService
	reception.razor
	Nuget: Microsoft.AspNetCore.SignalR.Client

