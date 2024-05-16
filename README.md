### WebSocket (SignalR)
#### States

* Issuing a room token
* Ship placement
* Have fun!

#### Issuing a room token (/create)
* Client on server: GenerateToken(string name)
* Server on client: setToken(string token)

#### Ship placement (Auth needed, /play)

* Client on server: Join(string roomId, FieldDto fieldDto) or Create(FieldDto fieldDto)
* Server on client: SetStatus(OpponentJoin or OpponentReady or GameStarted)

#### Have fun! (Auth needed, /play)

* Client on server: Shot(int x, int y) or Notify()
* Server on client: Positions(PositionDto)
