using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private Cell _mazeCellPrefab;

    [SerializeField]
    private Cell _mazeCellPrefab2;

    [SerializeField]
    private int _mazeWidth;
    [SerializeField]
    private int _mazeDepth;
    [SerializeField]
    private int _levels;

    [SerializeField]
    private int _pathLimit;

    private Cell[,] mazeGrid;

    private string mazeSeed;
    private int pathLimit;
    private Cell stairsCell;
    private int visitCounter;
    int roomindex;
    private List<GameObject> _spawnedMazeCells = new List<GameObject>();


    // To Start with fully functional Maze, switch IEnumerator for void and remove yield return
    IEnumerator Start()
    {
        roomindex = -1;
        stairsCell = null;
        mazeSeed = "D:" + _mazeWidth + "X" + _mazeDepth + "?";

        mazeGrid = new Cell[_mazeWidth, _mazeDepth];
        Cell cell = null;

        for (int y = 0; y < _levels; y++)
        {
            for (int x = 0; x < _mazeWidth; x++)
            {
                for (int z = 0; z < _mazeDepth; z++)
                {
                    if (y % 2 == 0)
                        cell = Instantiate(_mazeCellPrefab, new Vector3(x, y, z), Quaternion.identity);
                    else
                        cell = Instantiate(_mazeCellPrefab2, new Vector3(x, y, z), Quaternion.identity);

                    mazeGrid[x, z] = cell;
                    _spawnedMazeCells.Add(cell.gameObject);
                }
            }
            pathLimit = _pathLimit;
            if (stairsCell != null)
            {
                int x = (int)stairsCell.transform.position.x;
                int z = (int)stairsCell.transform.position.z;
                mazeGrid[x, z].ClearBottomWall();
            }
            yield return GenerateMaze(null, mazeGrid[0, 0], mazeGrid);
            PreparedRooms();
            GenerateMazeSeed();
        }

    }

    IEnumerator NewMap()
    {
        stairsCell = null;
        mazeSeed = "D:" + _mazeWidth + "X" + _mazeDepth + "?";

        mazeGrid = new Cell[_mazeWidth, _mazeDepth];
        Cell cell = null;

        for (int y = 0; y < _levels; y++)
        {
            for (int x = 0; x < _mazeWidth; x++)
            {
                for (int z = 0; z < _mazeDepth; z++)
                {
                    if (y % 2 == 0)
                        cell = Instantiate(_mazeCellPrefab, new Vector3(x, y, z), Quaternion.identity);
                    else
                        cell = Instantiate(_mazeCellPrefab2, new Vector3(x, y, z), Quaternion.identity);

                    mazeGrid[x, z] = cell;
                    _spawnedMazeCells.Add(cell.gameObject);
                }
            }
            pathLimit = _pathLimit;
            if (stairsCell != null)
            {
                int x = (int)stairsCell.transform.position.x;
                int z = (int)stairsCell.transform.position.z;
                mazeGrid[x, z].ClearBottomWall();
            }
            yield return GenerateMaze(null, mazeGrid[0, 0], mazeGrid);
            PreparedRooms();
            GenerateMazeSeed();
        }
    }

    private IEnumerator GenerateMaze(Cell previousCell, Cell currentCell, Cell[,] mazeGrid)
    {
        currentCell.Visit();
        visitCounter++;
        ClearWalls(previousCell, currentCell);

        yield return new WaitForSeconds(0.05f);

        Cell nextCell;

        do
        {

            nextCell = GetNextUnvisitedCell(currentCell, mazeGrid);

            if (nextCell != null && pathLimit >= 1)
            {
                yield return GenerateMaze(currentCell, nextCell, mazeGrid);

            }
            else
            {
                pathLimit--;

                if (pathLimit <= 0)
                {
                    if (pathLimit == 0)
                    {
                        stairsCell = currentCell;
                        stairsCell.ClearTopWall();
                    }
                    break;
                }
            }

        } while (nextCell != null); 
    }

    private Cell GetNextUnvisitedCell(Cell currentCell,Cell[,] mazeGrid)
    {
        var unvisitedCell = GetUnvisitedCell(currentCell, mazeGrid);

        return unvisitedCell.OrderBy(_  => Random.Range(1,10)).FirstOrDefault();

    }

    private IEnumerable<Cell> GetUnvisitedCell(Cell currentCell, Cell[,] mazeGrid)
    {
        int x = (int) currentCell.transform.position.x;
        int z = (int) currentCell.transform.position.z;

        if(x+1 < _mazeWidth)
        {
            var cellToRight = mazeGrid[x+1, z];

            if(cellToRight.IsVisited == false)
            {
                yield return cellToRight;
            }
        }
        if (x - 1 >= 0)
        {
            var cellToLeft = mazeGrid[x - 1, z];

            if (cellToLeft.IsVisited == false)
            {
                yield return cellToLeft;
            }
        }
        if (z + 1 < _mazeDepth)
        {
            var cellToFront = mazeGrid[x, z + 1];

            if (cellToFront.IsVisited == false)
            {
                yield return cellToFront;
            }
        }
        if (z - 1 >= 0)
        {
            var cellToBack = mazeGrid[x, z - 1];

            if (cellToBack.IsVisited == false)
            {
                yield return cellToBack;
            }
        }
    }

    private void ClearWalls(Cell previousCell, Cell currentCell)
    {
        if(previousCell == null) { return; }

        if(pathLimit == 0) { return; }

        if(previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }

        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }

    }

    private void CreateRoom(int x, int z, Cell[,] mazeGrid)
    {
        int xDistance = _mazeWidth - x;
        int zDistance = _mazeDepth - z;


        if (xDistance > 0 && zDistance > 0) 
        {
            int randomX = Random.Range(0, xDistance);
            int randomZ = Random.Range(0, zDistance);
            bool done = false;
            int endX = randomX + x - 1;
            int endZ = randomZ + z - 1;

            for(int i = randomX; i < endX; i++)
            {
                for(int j = randomZ; j < endZ; j++)
                {
                    mazeGrid[i, j].ClearRightWall();
                    mazeGrid[i, j].ClearFrontWall();
                    mazeGrid[i, j].Visit();

                    mazeGrid[i + 1, j].ClearLeftWall();
                    mazeGrid[i + 1, j].Visit();

                    mazeGrid[i, j + 1].ClearBackWall();
                    mazeGrid[i, j + 1].Visit();

                }
            }

            for(int i = endX; i > randomX; i--)
            {
                for (int j = endZ; j > randomZ; j--)
                {
                    mazeGrid[i, j].ClearLeftWall();
                    mazeGrid[i, j].ClearBackWall();
                    mazeGrid[i, j].Visit();

                    mazeGrid[i - 1, j].ClearRightWall();
                    mazeGrid[i - 1, j].Visit();

                    mazeGrid[i, j - 1].ClearFrontWall();
                    mazeGrid[i, j - 1].Visit();
                }
            }
        }
    }

    private void PreparedRooms()
    {
        switch (roomindex)   
        {
            case 0:
            CreateRoom(7, 4, mazeGrid);
            break;
            case 1:
            CreateRoom(7, 4, mazeGrid);
            CreateRoom(2, 3, mazeGrid);
            break;
            case 2:
            CreateRoom(2, 4, mazeGrid);
            CreateRoom(3, 3, mazeGrid);
            CreateRoom(4, 2, mazeGrid);
            break;
            default:
            break;
        }
    }

    public void GenerateMazeSeed()
    {
        foreach(Cell c in mazeGrid)
        {
            mazeSeed += c.getSeed().ToString() + "#";
        }
        mazeSeed = mazeSeed.Remove(mazeSeed.Length - 1);

        mazeSeed += "&";

    }
    public void ClearMaze()
    {
        foreach (GameObject obj in _spawnedMazeCells)
        {
            if (obj != null)
                Destroy(obj);
        }

        _spawnedMazeCells.Clear();
    }


    public void GenerateNewMaze()
    {
        ClearMaze();
        StartCoroutine(NewMap());
    }

    public void SetParameters(int width, int depth, int levels, int pathLimit)
    {
        _mazeWidth = width;
        _mazeDepth = depth;
        _levels = levels;
        _pathLimit = pathLimit;
    }
    public void SetRoomIndex(int index)
    {
        roomindex = index;
    }
}
