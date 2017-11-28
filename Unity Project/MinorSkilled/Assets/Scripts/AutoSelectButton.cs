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
		if (!button)
			button = GetComponent<Button>();

        if (button != null)
            button.Select();
    }
}
