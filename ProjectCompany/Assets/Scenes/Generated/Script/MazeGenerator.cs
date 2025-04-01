using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private Cell mazeCellPrefab;

    [SerializeField]
    private Cell mazeCellPrefab2;

    [SerializeField]
    private int _mazeWidth;
    [SerializeField]
    private int _mazeDepth;
    [SerializeField]
    private int _levels;

    private Cell[,] mazeGrid;

    private string mazeSeed;
    // To Start with fully functional Maze, switch IEnumerator for void and remove yield return
    IEnumerator Start()
    {
        mazeSeed = "D:" + _mazeWidth + "X" + _mazeDepth + "???";
        mazeGrid = new Cell[_mazeWidth, _mazeDepth];
        for (int y = 0; y < _levels; y++)
        {
            for (int x = 0; x < _mazeWidth; x++)
            {
                for (int z = 0; z < _mazeDepth; z++)
                {
                    if(y % 2 == 0)
                        mazeGrid[x, z] = Instantiate(mazeCellPrefab, new Vector3(x, y, z), Quaternion.identity);
                    else
                        mazeGrid[x, z] = Instantiate(mazeCellPrefab2, new Vector3(x, y, z), Quaternion.identity);
                }
            }
            yield return GenerateMaze(null, mazeGrid[0, 0], mazeGrid);
            GenerateMazeSeed();
        }

    }

    private IEnumerator GenerateMaze(Cell previousCell, Cell currentCell, Cell[,] mazeGrid)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        yield return new WaitForSeconds(0.05f);

        Cell nextCell;

        do
        {

            nextCell = GetNextUnvisitedCell(currentCell, mazeGrid);

            if (nextCell != null)
            {
                yield return GenerateMaze(currentCell, nextCell, mazeGrid);
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

    public void GenerateMazeSeed()
    {
        foreach(Cell c in mazeGrid)
        {
            mazeSeed += c.getSeed().ToString() + "#";
        }
        mazeSeed.Remove(mazeSeed.Length - 1);
        mazeSeed += "&&&";

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
