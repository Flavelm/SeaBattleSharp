### WebSocket (SignalR)
#### States

* Issuing a room token
* Ship placement
* Have fun!

#### Issuing a room token (/create)
* Client on server: GenerateToken(string name)
* Server on client: setToken(string token)

#### Ship placement (Auth needed, /play)

* Client: SeaBattleWeb.Models.ShipsDto
* Server: SeaBattleWeb.Models.NotificationDto
* Server: SeaBattleWeb.Models.NotificationDto

#### Have fun! (Auth needed, /play)

* Client: SeaBattleWeb.Models.ShipsDto
* Server: SeaBattleWeb.Models.ShipsDto
* Server: SeaBattleWeb.Models.PositionDto
