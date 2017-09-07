using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    private static CameraManager _instance;

    public static CameraManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CameraManager>();
            }
            return _instance;
        }
    }
    public CameraMovement playerCamera;
}
