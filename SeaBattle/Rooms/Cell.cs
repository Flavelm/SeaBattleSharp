namespace SeaBattle.Rooms;

public struct Cell
{
    public CellType Type { get; set; }
    
    public Cell(CellType type)
    {
        Type = type;
    }
}

public enum CellType
{
    SHIP, DEAD_SHIP, MISS, NONE
}