using UnityEngine;
using System.Collections;

public class CameraPosChange : MonoBehaviour {

    private CameraMovement playerCamera;

    void Start() {
        playerCamera = CameraManager.instance.playerCamera;
    }

    void OnTriggerEnter(Collider other) {
        if (other.GetComponent<PlayerController>()) {
            //Move smoothly towards new position
        }
    }
}
