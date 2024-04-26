### Endpoints
#### /api/Room/Create

* HttpMethod: POST
* Params: *nothing*
* Returns: {"Create": Guid}

#### /api/Room/{id}/Connection

* HttpMethod: POST
* Params: Guid
* Returns: {"Error": "Room not found!"}
* Returns: {"Error": $"Room ready: {room.IsReady}"}
* Returns: {"Error": "Profile not found!"}
* Returns: {"Error": $"Welcome anonymous|known, {name}"}

#### /api/Room/{id}/Connection

* HttpMethod: GET
* Params: Guid
* Return is the same as the previous method
* WebSocket

### WebSocket
#### States

* Issuing a room token (If the user connected first or second)
* Authorization (If user reconnected; Next state may be skipped)
* Ship placement
* Have fun!

#### Issuing a room token
* Server: {"Token": String}

#### Authorization
* Client: {"Token": String}
* Server closes connection if token invalid
* Server goes to the next mode if the token is valid

#### Ship placement 

* Client: SeaBattleWeb.Models.ShipsDto
* Server: SeaBattleWeb.Models.NotificationDto
* Server: SeaBattleWeb.Models.NotificationDto

#### Have fun!

* Client: SeaBattleWeb.Models.ShipsDto
* Server: SeaBattleWeb.Models.ShipsDto
* Server: SeaBattleWeb.Models.PositionDto
