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
    }

    private void OnEnable()
    {
		if (!button)
			button = GetComponent<Button>();
        button.Select();
    }
}
