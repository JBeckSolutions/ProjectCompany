using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GenerateFromSeed : MonoBehaviour
{
    [SerializeField]
    private Cell _mazeCellPrefab;

    [SerializeField]
    private string _seed;

    private int mazeWidth;
    private int mazeDepth;
    private string [] levels;

    private Cell[,] mazeGrid;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        decode(_seed);

        for(int i = 0; i < levels.Length; i++)
        {
            Level(levels[i], i);
        }
    }

    private void decode(string seed)
    {
        string[] split = seed.Split('?');
        split[0] = split[0].Remove(0, 2);
        string[] dimension = split[0].Split('X', StringSplitOptions.RemoveEmptyEntries);
        mazeWidth = int.Parse(dimension[0]);
        mazeDepth = int.Parse(dimension[1]);

        levels = split[1].Split("&", StringSplitOptions.RemoveEmptyEntries);

    }
    private void Level(string code, int y)
    {
        string[] cells = code.Split('#', StringSplitOptions.RemoveEmptyEntries);
        int[] cellcodes = new int[cells.Length];

        for (int i = 0; i < cells.Length; i++)
        {
            cellcodes[i] = int.Parse(cells[i]);
        }

        int counter = 0;
        mazeGrid = new Cell[mazeWidth, mazeDepth];

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeDepth; z++)
            {
                Cell cell = Instantiate(_mazeCellPrefab, new Vector3(x, y, z), Quaternion.identity);
                cell.Init(cellcodes[counter]);
                if(cellcodes[counter] < 63)
                {
                    cell.Visit();
                }
                mazeGrid[x, z] = cell;
                counter++;
            }
        }

    }
}
