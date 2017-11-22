using UnityEngine;
using UnityEngine.UI;

public class AutoSelectButton : MonoBehaviour
{
    private Button button;
    private void Start()
    {
    }

    private void OnEnable()
    {
<<<<<<< HEAD
		if (!button)
			button = GetComponent<Button>();
        button.Select();
=======
        if (button != null)
            button.Select();
>>>>>>> 5a9aa5827aa39d4f5177d66f8ae54c6c081d88d3
    }
}
