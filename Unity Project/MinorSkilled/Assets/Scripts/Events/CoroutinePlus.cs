using System;
using System.Collections;
using UnityEngine;

public class CoroutinePlus
{
    public Coroutine coroutine;
    public Action onCoroutineStop;
    private static GameObject tempGameObject = null;

    CoroutinePlus()
    {
        tempGameObject = new GameObject();
    }

    public static void StopCoroutine(CoroutinePlus coroutinePlus)
    {
        if (tempGameObject == null) tempGameObject = new GameObject();
        MonoBehaviourCheat.Instance.StopCoroutine(coroutinePlus.coroutine);
        if (coroutinePlus.onCoroutineStop != null) coroutinePlus.onCoroutineStop.Invoke();
    }

    public static CoroutinePlus StartCoroutine(IEnumerator routine)
    {
        if (tempGameObject == null) tempGameObject = new GameObject();
        CoroutinePlus output = new CoroutinePlus();
        output.coroutine = MonoBehaviourCheat.Instance.StartCoroutine(routine);
        return output;
    }

    public static CoroutinePlus StartCoroutine(IEnumerator routine, Action onRoutineStop)
    {
        if (tempGameObject == null) tempGameObject = new GameObject();
        CoroutinePlus output = new CoroutinePlus();
        output.coroutine = MonoBehaviourCheat.Instance.StartCoroutine(routine);
        output.onCoroutineStop = onRoutineStop;
        return output;
    }
}

public class MonoBehaviourCheat : Singleton<MonoBehaviourCheat>
{
}
