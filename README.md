### SeaBattle

### Packet types: <br />
> 000 - Buffer error <br />
> 100 - GET_ROOM <br />
> 101 - Ping <br />

### Cells types: <br />
> 0 - SHIP <br />
> 1 - DEAD_SHIP <br />
> 2 - MISS <br />
> 3 - NONE <br />

### to the client <br />
  * Server message <br />
    > {"Type": 0, "Message": "Buffer dropped"};

### Get room <br />
  * Client request <br />
    > 100:{"Player": [0 or 1]}; <br />
  * Server response <br />
    > {"Type": 100, "FieldOne": [[{"Type": &Cell type&}, {"Type": &Cell type&}..]..],"FieldTwo": [[{"Type": &Cell type&}, {"Type": &Cell type&}..]..]}; <br />

### Ping <br />
  * Client request <br />
    > 101:{"Time": &unix time&}; <br />
  * Server response <br />
    > {"Type": 101, "Ping": 10};  <br />
