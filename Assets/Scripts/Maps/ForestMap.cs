using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestMap : MonoBehaviour
{
    // 1 = •Ç
    // 0 = “¹
    public static int[,] mapGrid = new int[15, 15]
    {
        { 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1 },
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 0, 1 },
        { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1 },
        { 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
        { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0 },
        { 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0 },
        { 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0 },
        { 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0 },
        { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
        { 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1 },
        { 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1 },
    };
    public static int GetForestMap(int x, int y)
    {
        return mapGrid[x, y];
    }

    public static void SetForestMap(int x, int y, int value)
    {
        mapGrid[x, y] = value;
    }
}
