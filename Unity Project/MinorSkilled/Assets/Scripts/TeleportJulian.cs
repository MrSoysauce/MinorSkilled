using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportJulian : MonoBehaviour {

    public GameObject receiver;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.localPosition = receiver.transform.localPosition;
        }
    }
}
