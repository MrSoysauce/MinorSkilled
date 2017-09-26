using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy5 : Enemy2
{
    [SerializeField] private float descendSpeed;
    [SerializeField] private Transform descendEnd;
    private Vector3 startDescend;

    private bool hanging = true;

    protected override void Start()
    {
        startDescend = transform.position;
        base.Start();
    }

    public void StartFalling()
    {
        StartCoroutine(fall());
    }

    protected override void Update()
    {
        animator.SetBool("Hanging", hanging);

        if (!hanging)
            base.Update();
    }

    private IEnumerator fall()
    {
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * descendSpeed;
            transform.position = Vector3.Lerp(startDescend, descendEnd.position, t);

            inRangeCheck();
            if (attached)
            {
                hanging = false;
                visuals.transform.localRotation = Quaternion.identity;
                yield break;
            }

            yield return null;
        }

        visuals.transform.localRotation = Quaternion.identity;

        hanging = false;
    }
}
