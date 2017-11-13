using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoSelectButton : MonoBehaviour
{
    private Button button;
    private void Start()
    {
        button = GetComponent<Button>();
        Debug.Assert(button != null, "AutoSelectButton can't find button!");
    }

    private void OnEnable()
    {
        button.Select();
    }
}
