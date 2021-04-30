using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    #region Variables

    public PlayerController player;

    private class GameBoardInstances
    {
        public GameBoardInstances()
        {
            _gameBoardInstances = new GameObject[8, 8];
            gameBoardCircles = new GameObject[8, 8];
        }

        public GameObject this[int i, int j]
        {
            get
            {
                if (i >= 0 && i <= 7 && j >= 0 && j <= 7)
                    return _gameBoardInstances[i, j];

                return null;
            }

            set
            {
                if (i >= 0 && i <= 7 && j >= 0 && j <= 7)
                    _gameBoardInstances[i, j] = value;
                else
                    Debug.LogError("[GameBoardInstances] Failed to set new value; index is out of bounds.");
            }
        }

        public GameObject[,] _gameBoardInstances;
        public GameObject[,] gameBoardCircles;

        public void ActivateCircles(List<Cell> cellsForCircles)
        {
            foreach (var cell in cellsForCircles)
            {
                gameBoardCircles[cell.x, cell.y].SetActive(true);
            }
        }

        public void DeactivateCircles()
        {
            foreach (var circle in gameBoardCircles)
            {
                circle.SetActive(false);
            }
        }
    }

    GameBoardInstances gameBoardInstances = new GameBoardInstances();

    private const float CELLSIZEX = 0.99f;
    private const float CELLSIZEY = 0.98f;

    private class PieceSelected
    {
        public PieceSelected(GameObject piece, Cell cell, List<Cell> availableTurns, bool isWhite)
        {
            instance = piece;
            this.cell = cell;
            this.availableTurns = availableTurns;
            this.isWhite = isWhite;
        }

        public GameObject instance;
        public Cell cell;
        public List<Cell> availableTurns;
        public bool isWhite;

        public void Reset()
        {
            instance = null;
            cell = null;
            availableTurns = null;
            isWhite = false;
        }
    }
    private PieceSelected pieceSelected = new PieceSelected(null, null, null, false);
    private bool isWhiteTurn = true;

    

    [SerializeField]  GameObject whitePawnPrefab;
    [SerializeField]  GameObject whiteKnightPrefab;
    [SerializeField]  GameObject whiteBishopPrefab;
    [SerializeField]  GameObject whiteRookPrefab;
    [SerializeField]  GameObject whiteQueenPrefab;
    [SerializeField]  GameObject whiteKingPrefab;

    [SerializeField]  GameObject blackPawnPrefab;
    [SerializeField]  GameObject blackKnightPrefab;
    [SerializeField]  GameObject blackBishopPrefab;
    [SerializeField]  GameObject blackRookPrefab;
    [SerializeField]  GameObject blackQueenPrefab;
    [SerializeField]  GameObject blackKingPrefab;

    [SerializeField] GameObject circlePrefab;

    private class Cell
    {
        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Cell thisCell, Cell otherCell) => (thisCell.x == otherCell.x && thisCell.y == otherCell.y);
        public static bool operator !=(Cell thisCell, Cell otherCell) => !(thisCell == otherCell);

        public int x;
        public int y;

        public override bool Equals(object obj)
        {
            if (obj is Cell)
            {
                return this == (Cell)obj;
            }

            return false;
        }

        public override string ToString()
        {
            return $"Cell ({x}, {y})";
        }

        public override int GetHashCode()
        {
            int hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }
    }

    #endregion

    // 0 - clear, 1 - pawn, 2 - knight, 3 - bishop, 4 - rook, 5 - queen, 6 - king

    #region Initializations
    void Start()
    {
        ResetBoard();
    }

    void ResetBoard()
    {
        int[,] newGameBoard = 
            {
                {14, 12, 13, 16, 15, 13, 12, 14},
                {11, 11, 11, 11, 11, 11, 11, 11},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {21, 21, 21, 21, 21, 21, 21, 21},
                {24, 22, 23, 26, 25, 23, 22, 24},
            };

        SpawnBoard(newGameBoard);
    }

    private void SpawnBoard(int[,] gameBoard)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (gameBoard[i, j] != 0)
                {
                    GameObject prefab = GetPrefabByIndex(gameBoard[i, j]);
                    gameBoardInstances[i, j] = Instantiate(prefab, GetCellCenter(new Cell(i, j)), prefab.transform.rotation, transform);
                }

                gameBoardInstances.gameBoardCircles[i, j] = Instantiate(circlePrefab, GetCellCenter(new Cell(i, j)) + Vector3.up / 20, circlePrefab.transform.rotation, transform);
                gameBoardInstances.gameBoardCircles[i, j].SetActive(false);
            }
        }
    }

    private GameObject GetPrefabByIndex(int index)
    {
        if (index / 10 == 1)
        {
            switch (index % 10)
            {
                case 1:
                    return whitePawnPrefab;
                case 2:
                    return whiteKnightPrefab;
                case 3:
                    return whiteBishopPrefab;
                case 4:
                    return whiteRookPrefab;
                case 5:
                    return whiteQueenPrefab;
                case 6:
                    return whiteKingPrefab;
                default:
                    Debug.LogError("[GetPrefabByIndex] Error with getting a prefab.");
                    return whitePawnPrefab;
            }
        }
        else
        {
            switch (index % 10)
            {
                case 1:
                    return blackPawnPrefab;
                case 2:
                    return blackKnightPrefab;
                case 3:
                    return blackBishopPrefab;
                case 4:
                    return blackRookPrefab;
                case 5:
                    return blackQueenPrefab;
                case 6:
                    return blackKingPrefab;
                default:
                    Debug.LogError("[GetPrefabByIndex] Error with getting a prefab.");
                    return blackPawnPrefab;
            }
        }
        
    }
    #endregion

    #region CheckTurnAvailability

    private List<Cell> GetAvailableTurns()
    {
        if ((!isWhiteTurn && pieceSelected.isWhite) || (isWhiteTurn && !pieceSelected.isWhite))
            return new List<Cell>();

        switch (pieceSelected.instance.name.Split(' ', '(', ')')[1])
        {
            case "Pawn":
                if (pieceSelected.isWhite)
                    return GetWhitePawnTurnAvailable();             
                else
                    return GetBlackPawnTurnAvailable();

            case "Knight":
                return GetKnightTurnAvailable();

            case "Bishop":

                break;
            case "Rook":

                break;
            case "Queen":

                break;
            case "King":

                break;
            default:
                Debug.LogError("[CheckIfTurnIsAvailable] Unknown piece name");
                break;
        }
        return new List<Cell>();
    }

    private List<Cell> GetWhitePawnTurnAvailable()
    {
        var cellList = new List<Cell>();

        if (IsCorrectCell(pieceSelected.cell.x + 1, pieceSelected.cell.y)) 
            cellList.Add(new Cell(pieceSelected.cell.x + 1, pieceSelected.cell.y));

        if (pieceSelected.cell.x == 1 && IsCorrectCell(pieceSelected.cell.x + 2, pieceSelected.cell.y))
            cellList.Add(new Cell(pieceSelected.cell.x + 2, pieceSelected.cell.y));

        if (gameBoardInstances[pieceSelected.cell.x + 1, pieceSelected.cell.y + 1]?.CompareTag("Black") ?? false)
            cellList.Add(new Cell(pieceSelected.cell.x + 1, pieceSelected.cell.y + 1));

        if (gameBoardInstances[pieceSelected.cell.x + 1, pieceSelected.cell.y - 1]?.CompareTag("Black") ?? false)
            cellList.Add(new Cell(pieceSelected.cell.x + 1, pieceSelected.cell.y - 1));

        return cellList;
    }

    private List<Cell> GetBlackPawnTurnAvailable()
    {
        var cellList = new List<Cell>();

        if (IsCorrectCell(pieceSelected.cell.x - 1, pieceSelected.cell.y))
            cellList.Add(new Cell(pieceSelected.cell.x - 1, pieceSelected.cell.y));

        if (pieceSelected.cell.x == 6 && IsCorrectCell(pieceSelected.cell.x - 2, pieceSelected.cell.y))
            cellList.Add(new Cell(pieceSelected.cell.x - 2, pieceSelected.cell.y));

        if (gameBoardInstances[pieceSelected.cell.x - 1, pieceSelected.cell.y + 1]?.CompareTag("White") ?? false)
            cellList.Add(new Cell(pieceSelected.cell.x - 1, pieceSelected.cell.y + 1));

        if (gameBoardInstances[pieceSelected.cell.x - 1, pieceSelected.cell.y - 1]?.CompareTag("White") ?? false)
            cellList.Add(new Cell(pieceSelected.cell.x - 1, pieceSelected.cell.y - 1));

        return cellList;
    }

    private List<Cell> GetKnightTurnAvailable()
    {
        var cellList = new List<Cell>();

        if (IsCorrectCell(pieceSelected.cell.x + 2, pieceSelected.cell.y + 1))
            cellList.Add(new Cell(pieceSelected.cell.x + 2, pieceSelected.cell.y + 1));

        if (IsCorrectCell(pieceSelected.cell.x + 2, pieceSelected.cell.y - 1))
            cellList.Add(new Cell(pieceSelected.cell.x + 2, pieceSelected.cell.y - 1));

        if (IsCorrectCell(pieceSelected.cell.x + 1, pieceSelected.cell.y - 2))
            cellList.Add(new Cell(pieceSelected.cell.x + 1, pieceSelected.cell.y - 2));

        if (IsCorrectCell(pieceSelected.cell.x - 1, pieceSelected.cell.y - 2))
            cellList.Add(new Cell(pieceSelected.cell.x - 1, pieceSelected.cell.y - 2));

        if (IsCorrectCell(pieceSelected.cell.x - 2, pieceSelected.cell.y - 1))
            cellList.Add(new Cell(pieceSelected.cell.x - 2, pieceSelected.cell.y - 1));

        if (IsCorrectCell(pieceSelected.cell.x - 2, pieceSelected.cell.y + 1))
            cellList.Add(new Cell(pieceSelected.cell.x - 2, pieceSelected.cell.y + 1));

        if (IsCorrectCell(pieceSelected.cell.x - 1, pieceSelected.cell.y + 2))
            cellList.Add(new Cell(pieceSelected.cell.x - 1, pieceSelected.cell.y + 2));

        if (IsCorrectCell(pieceSelected.cell.x + 1, pieceSelected.cell.y + 2))
            cellList.Add(new Cell(pieceSelected.cell.x + 1, pieceSelected.cell.y + 2));

        return cellList;
    }

    private bool IsCorrectCell(int x, int y) 
    {
        if (!(x >= 0 && x <= 7 && y >= 0 && y <= 7))
            return false;

        if (gameBoardInstances[x, y] == null)
        {
            return true;
        }
        else
        {
            if ((gameBoardInstances[x, y].CompareTag("White") && !pieceSelected.isWhite) ||
                (gameBoardInstances[x, y].CompareTag("Black") && pieceSelected.isWhite))
            {
                return true;
            }   
        }

        return false;
    }
        
    #endregion


    private Vector3 GetCellCenter(Cell cell)
    {
        return new Vector3((cell.x + 0.5f) * CELLSIZEX, 0, (cell.y + 0.5f) * -CELLSIZEY);
    }

    public void InputCell(Vector3 worldCoords)
    {
        int x = Mathf.FloorToInt(worldCoords.x / CELLSIZEX);
        int y = Mathf.FloorToInt(-worldCoords.z / CELLSIZEY);
        if (0 <= x && x <= 7 && 0 <= y && y <= 7)
        {
            var cell = new Cell(x, y);

            if (pieceSelected.instance != null)
                MakeTurn(cell);
            else
                SelectCell(cell);
        }
            
    }

    private void MakeTurn(Cell cell)
    {
        if(IsAvaiableTurn(cell))
        {
            pieceSelected.instance.transform.Translate(GetCellCenter(cell) - pieceSelected.instance.transform.position);
            if (gameBoardInstances[cell.x, cell.y] != null)
                gameBoardInstances[cell.x, cell.y].SetActive(false); // TODO: take a piece in a portal

            // check for check and mate

            gameBoardInstances[cell.x, cell.y] = pieceSelected.instance;
            pieceSelected.Reset();
            gameBoardInstances.DeactivateCircles();
            isWhiteTurn = !isWhiteTurn;

            player.RotateCamera(isWhiteTurn);
        }
    }

    private bool IsAvaiableTurn(Cell cell)
    {
        return pieceSelected.availableTurns.Contains(cell); 
    }

    private void SelectCell(Cell cell)
    {
        if (gameBoardInstances[cell.x, cell.y] == null)
            return;

        pieceSelected.instance = gameBoardInstances[cell.x, cell.y];
        pieceSelected.cell = cell;
        pieceSelected.isWhite = pieceSelected.instance.CompareTag("White");
        pieceSelected.availableTurns = GetAvailableTurns();
        

        if (pieceSelected.availableTurns.Count == 0)
        {
            pieceSelected.Reset();
            return;
        }
            
        gameBoardInstances[cell.x, cell.y] = null;
        gameBoardInstances.ActivateCircles(pieceSelected.availableTurns);
    }
}
