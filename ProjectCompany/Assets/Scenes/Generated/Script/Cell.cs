using Newtonsoft.Json.Linq;
using NUnit.Framework.Internal;
using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField]
    private GameObject _leftWall;
    [SerializeField]
    private GameObject _rightWall;
    [SerializeField]
    private GameObject _frontWall;
    [SerializeField]
    private GameObject _backWall;
    [SerializeField]
    private GameObject _topWall;
    [SerializeField]
    private GameObject _bottomWall;
    [SerializeField]
    private GameObject _unvisitedBlock;

    private int[] wallBinary;

    private int seed;

    public Cell()
    {
        int[] wallStructure = new int[6] { 1, 1, 1, 1, 1, 1 };
        wallBinary = wallStructure;
        setSeed();
    }

    public Cell(int Seed)
    {
        String walls = Convert.ToString(Seed, 2);
        wallBinary = new int[6];
        wallBinary[0] = walls[0];
        wallBinary[1] = walls[1];
        wallBinary[2] = walls[2];
        wallBinary[3] = walls[3];
        wallBinary[4] = walls[4];
        wallBinary[5] = walls[5];
        setSeed();
        GenerateCell();
    }

    public void GenerateCell()
    {
        if (wallBinary[0] == 0)
        {
            ClearLeftWall();
        }
        if (wallBinary[1] == 0)
        {
            ClearRightWall();
        }
        if (wallBinary[2] == 0)
        {
            ClearFrontWall();
        }
        if (wallBinary[3] == 0)
        {
            ClearBackWall();
        }
        if (wallBinary[4] == 0)
        {
            ClearTopWall();
        }
        if (wallBinary[5] == 0)
        {
            ClearBottomWall();
        }
    }

    public bool IsVisited { get; private set; }

    public void Visit()
    {
        IsVisited = true;
        _unvisitedBlock.SetActive(false);
    }

    public int getSeed()
    {
        setSeed();
        return seed;
    }

    public void setSeed()
    {
        string values = string.Join("", wallBinary);
        seed = Convert.ToInt32(values, 2);
    }

    public void ClearLeftWall()
    {
        _leftWall.SetActive(false);
        wallBinary[0] = 0;
        setSeed();
    }

    public void ClearRightWall()
    {
        _rightWall.SetActive(false);
        wallBinary[1] = 0;
        setSeed();
    }

    public void ClearFrontWall()
    {
        _frontWall.SetActive(false);
        wallBinary[2] = 0;
        setSeed();
    }

    public void ClearBackWall()
    { 
        _backWall.SetActive(false);
        wallBinary[3] = 0;
        setSeed();
    }

    public void ClearTopWall()
    {
        _topWall.SetActive(false);
        wallBinary[4] = 0;
        setSeed();
    }

    public void ClearBottomWall()
    { 
        _bottomWall.SetActive(false);
        wallBinary[5] = 0;
        setSeed();
    }



}
