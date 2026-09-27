using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    // »”™
    private static bool _getWeapon = false; // •Ší“üè
    private static bool _getKeyD = false; // Î”Â“üè

    // ƒ_ƒ“ƒWƒ‡ƒ“
    private static bool _getfire = false; // ¼–¾“üè
    private static bool _monster = false; // “GŒ‚”j
    private static bool _gameOver = false; // “G‚É•‰‚¯‚½
    private static bool _gameClear = false; // ƒQ[ƒ€ƒNƒŠƒA

    // X
    private static bool _rain = true; // ‰J
    private static bool _getKeyF = false; // Î”Â“üè


    public static bool GetWeapon
    {
        get { return _getWeapon; }
        set { _getWeapon = value; }
    }
    public static bool GetKeyD
    {
        get { return _getKeyD; }
        set { _getKeyD = value; }
    }
    public static bool GetFire
    {
        get { return _getfire; }
        set { _getfire = value; }
    }
    public static bool Monster
    {
        get { return _monster; }
        set { _monster = value; }
    }
    public static bool GameOverFlag
    {
        get { return _gameOver; }
        set { _gameOver = value; }
    }
    public static bool GameClearFlag
    {
        get { return _gameClear; }
        set { _gameClear = value; }
    }
    public static bool Raining
    {
        get { return _rain; }
        set { _rain = value; }
    }
    public static bool GetKeyF
    {
        get { return _getKeyF; }
        set { _getKeyF = value; }
    }
}
