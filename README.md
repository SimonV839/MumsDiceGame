# MumsDiceGame
 Server and client that allow two players to play a game of dice

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

###Steps
Blazor Web App
	Auto(Server and WebAssembly)
	Per page/component
[This creates the server and the .client]

Add Logging to the server (attempts to add logging to client fail)

