using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignpostInstantiate : MonoBehaviour {


    [Tooltip("If active, you can change the prefabs in runtime.")] public bool active;

    public enum poles {NA, one, two, three}
    public enum corners {NA, one, two, three }
    public enum signs {NA, one, two, three }

    private GameObject poleInUse;
    private GameObject cornerInUse;
    private GameObject signInUse;

    [Header ("Part Select")]
    public poles poleSelect;
    public corners cornerSelect;
    public signs signSelect;

    [Header("Part Prefabs")]
    public GameObject[] signPostParts;

	void Start () {
        switch (poleSelect)
        {
            case poles.NA:
                break;
            case poles.one:
                poleInUse = Instantiate(signPostParts[0], this.transform.position, Quaternion.identity) as GameObject;
                poleInUse.transform.SetParent(this.transform);
                break;
            case poles.two:
                poleInUse = Instantiate(signPostParts[1], this.transform.position, Quaternion.identity) as GameObject;
                poleInUse.transform.SetParent(this.transform);
                break;
            case poles.three:
                poleInUse = Instantiate(signPostParts[2], this.transform.position, Quaternion.identity) as GameObject;
                poleInUse.transform.SetParent(this.transform);
                break;
        }
        switch (cornerSelect)
        {
            case corners.NA:
                break;
            case corners.one:
                cornerInUse = Instantiate(signPostParts[3], this.transform.position + new Vector3(0f, 6f, 0f), Quaternion.identity);
                cornerInUse.transform.SetParent(this.transform);
                break;
            case corners.two:
                cornerInUse = Instantiate(signPostParts[4], this.transform.position + new Vector3(0f, 6f, 0f), Quaternion.identity);
                cornerInUse.transform.SetParent(this.transform);
                break;
            case corners.three:
                cornerInUse = Instantiate(signPostParts[5], this.transform.position + new Vector3(0f, 6f, 0f), Quaternion.identity);
                cornerInUse.transform.SetParent(this.transform);
                break;
        }
        switch (signSelect)
        {
            case signs.NA:
                break;
            case signs.one:
                signInUse = Instantiate(signPostParts[6], this.transform.position + new Vector3(-1.2f, 6.47f, 0f), Quaternion.identity);
                signInUse.transform.SetParent(this.transform);
                break;
            case signs.two:
                signInUse = Instantiate(signPostParts[7], this.transform.position + new Vector3(-1.23f, 6.47f, 0f), Quaternion.identity);
                signInUse.transform.SetParent(this.transform);
                break;
            case signs.three:
                signInUse = Instantiate(signPostParts[8], this.transform.position + new Vector3(-1.25f, 6.47f, 0f), Quaternion.identity);
                signInUse.transform.SetParent(this.transform);
                break;
        }
    }

    void Update()
    {
        if (active)
        {
            switch (poleSelect)
            {
                case poles.NA:
                    break;
                case poles.one:
                    Destroy(poleInUse.gameObject);
                    poleInUse = Instantiate(signPostParts[0], this.transform.position, Quaternion.identity) as GameObject;
                    poleInUse.transform.SetParent(this.transform);
                    break;
                case poles.two:
                    Destroy(poleInUse.gameObject);
                    poleInUse = Instantiate(signPostParts[1], this.transform.position, Quaternion.identity) as GameObject;
                    poleInUse.transform.SetParent(this.transform);
                    break;
                case poles.three:
                    Destroy(poleInUse.gameObject);
                    poleInUse = Instantiate(signPostParts[2], this.transform.position, Quaternion.identity) as GameObject;
                    poleInUse.transform.SetParent(this.transform);
                    break;
            }
            switch (cornerSelect)
            {
                case corners.NA:
                    break;
                case corners.one:
                    Destroy(cornerInUse.gameObject);
                    cornerInUse = Instantiate(signPostParts[3], this.transform.position + new Vector3(0f, 6f, 0f), Quaternion.identity);
                    cornerInUse.transform.SetParent(this.transform);
                    break;
                case corners.two:
                    Destroy(cornerInUse.gameObject);
                    cornerInUse = Instantiate(signPostParts[4], this.transform.position + new Vector3(0f, 6f, 0f), Quaternion.identity);
                    cornerInUse.transform.SetParent(this.transform);
                    break;
                case corners.three:
                    Destroy(cornerInUse.gameObject);
                    cornerInUse = Instantiate(signPostParts[5], this.transform.position + new Vector3(0f, 6f, 0f), Quaternion.identity);
                    cornerInUse.transform.SetParent(this.transform);
                    break;
            }
            switch (signSelect)
            {
                case signs.NA:
                    break;
                case signs.one:
                    Destroy(signInUse.gameObject);
                    signInUse = Instantiate(signPostParts[6], this.transform.position + new Vector3(-1.2f, 6.47f, 0f), Quaternion.identity);
                    signInUse.transform.SetParent(this.transform);
                    break;
                case signs.two:
                    Destroy(signInUse.gameObject);
                    signInUse = Instantiate(signPostParts[7], this.transform.position + new Vector3(-1.23f, 6.47f, 0f), Quaternion.identity);
                    signInUse.transform.SetParent(this.transform);
                    break;
                case signs.three:
                    Destroy(signInUse.gameObject);
                    signInUse = Instantiate(signPostParts[8], this.transform.position + new Vector3(-1.25f, 6.47f, 0f), Quaternion.identity);
                    signInUse.transform.SetParent(this.transform);
                    break;
            }
        }
    }
}
