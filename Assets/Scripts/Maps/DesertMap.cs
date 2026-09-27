using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertMap : MonoBehaviour
{
    // 1 = •Ç
    // 0 = “¹
    public static int[,] mapGrid = new int[15, 15]
    {
        { 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1 },
        { 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1 },
        { 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1 },
        { 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1 },
        { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 1 },
        { 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
        { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
        { 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0 },
        { 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 0, 0 },
    };
    public static int GetDesertMap(int x, int y)
    {
        return mapGrid[x, y];
    }

    public static void SetDesertMap(int x, int y, int value)
    {
        mapGrid[x, y] = value;
    }
}
