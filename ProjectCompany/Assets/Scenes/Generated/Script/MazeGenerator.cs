using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private Cell mazeCellPrefab;

    [SerializeField]
    private int _mazeWidth;
    [SerializeField]
    private int _mazeDepth;

    private Cell[,] _mazeGrid;
    // To Start with fully functional Maze, switch IEnumerator for void and remove yield return
    IEnumerator Start()
    {
        _mazeGrid = new Cell[_mazeWidth, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                _mazeGrid[x, z] = Instantiate(mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity);
            }
        }

        yield return GenerateMaze(null, _mazeGrid[0, 0]);
        
    }

    private IEnumerator GenerateMaze(Cell previousCell, Cell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        yield return new WaitForSeconds(0.05f);

        Cell nextCell;

        do
        {

            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                yield return GenerateMaze(currentCell, nextCell);
            }

        } while (nextCell != null); 
    }

    private Cell GetNextUnvisitedCell(Cell currentCell)
    {
        var unvisitedCell = GetUnvisitedCell(currentCell);

        return unvisitedCell.OrderBy(_  => Random.Range(1,10)).FirstOrDefault();

    }

    private IEnumerable<Cell> GetUnvisitedCell(Cell currentCell)
    {
        int x = (int) currentCell.transform.position.x;
        int z = (int) currentCell.transform.position.z;

        if(x+1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x+1, z];

            if(cellToRight.IsVisited == false)
            {
                yield return cellToRight;
            }
        }
        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];

            if (cellToLeft.IsVisited == false)
            {
                yield return cellToLeft;
            }
        }
        if (z + 1 < _mazeDepth)
        {
            var cellToFront = _mazeGrid[x, z + 1];

            if (cellToFront.IsVisited == false)
            {
                yield return cellToFront;
            }
        }
        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
