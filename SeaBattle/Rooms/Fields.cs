namespace SeaBattle.Rooms;

public class Fields
{
    private const int FieldLength = 10;

    public Cell[,] FirstField { get; private set; } = new Cell[FieldLength, FieldLength];
    public Cell[,] SecondField { get; private set; } = new Cell[FieldLength, FieldLength];

    public Fields()
    {
        for (int I = 0; I < FieldLength; I++)
        {
            for (int J = 0; J < FieldLength; J++)
            {
                FirstField[I, J] = new Cell(CellType.SHIP);
                SecondField[I, J] = new Cell(CellType.SHIP);
            }
        }
    }
    
    public Cell[,] GetSecondWithoutShips()
    {
        return GetWithoutShips(SecondField);
    }

    public Cell[,] GetFirstWithoutShips()
    {
        return GetWithoutShips(FirstField);
    }

    public Cell[,] GetWithoutShips(Cell[,] list)
    {
        Cell[,] Return = new Cell[FieldLength, FieldLength];
        
        for (int I = 0; I < FieldLength; I++)
        {
            for (int J = 0; J < FieldLength; J++)
            {
                Cell Cell = list[I, J];
                Cell.Type = Cell.Type == CellType.SHIP ? CellType.NONE : Cell.Type;
                Return[I, J] = Cell;
            }
        }

        return Return;
    }
}