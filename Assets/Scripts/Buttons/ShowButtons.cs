using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowButtons : MonoBehaviour
{
    public GameObject buttons;

    public void ShowOtherButtons()
    {
        if (!buttons.activeSelf) buttons.SetActive(true);
        else buttons.SetActive(false);
    }
}
