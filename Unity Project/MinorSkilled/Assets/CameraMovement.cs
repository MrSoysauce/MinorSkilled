using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

//public enum CameraState { Default, Moving, NewLoc, Static }
public class CameraMovement : MonoBehaviour
{
    private Transform currentCameraTrans;
    [SerializeField] private GameObject playertarget;

    [Header("-Transforms-")]
    [SerializeField] private Vector3 offset;
    private Transform cameraDefaultTrans;
    private Transform cameraDefaultTarget;

    //public CameraState _cameraState;
    //public CameraState cameraState {
    //    get
    //    {
    //        return _cameraState;
    //    }
    //    set
    //    {
    //        _cameraState = value;

    //        switch (_cameraState)
    //        {
    //            case CameraState.Default:
    //                currentCameraTrans = cameraTransDefault;
    //                SetCameraTarget(cameraTargetDefault);
    //                break;
    //            case CameraState.Static:
    //                //currentCameraTrans.position = panTarget.position + (offset + new Vector3(0f, 1f, -2f));
    //                //SetCameraTarget(panTarget.transform);
    //                break;
    //            case CameraState.Moving:
    //                //Moving towards the NewLoc
    //                break;
    //            case CameraState.NewLoc:
    //                //This is new offset until triggered otherwise
    //                break;
    //        }
    //    } 
    //}

    [HideInInspector] public Transform currentCameraTarget;

    void Start()
    {
        cameraDefaultTrans = playertarget.transform;
        cameraDefaultTarget = playertarget.transform;

        transform.position = playertarget.transform.position + offset;

        cameraDefaultTarget.position = playertarget.transform.position;
        //cameraState = CameraState.Default;

        currentCameraTrans = cameraDefaultTrans;
        SetCameraTarget(cameraDefaultTarget);
    }

    void Update()
    {
        //if (cameraState != CameraState.Static)
        //{
            cameraDefaultTrans.position = playertarget.transform.position + offset;
            CameraFollowPlayer();
        //}
    }

    void CameraFollowPlayer()
    {
        transform.position = currentCameraTrans.position;
        transform.forward = (currentCameraTarget.position - transform.position).normalized;
    }

    public void SetCameraTarget(Transform target)
    {
        currentCameraTarget = target;
    }
}
