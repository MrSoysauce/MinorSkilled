using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAlpahFade : MonoBehaviour
{

    [Tooltip("Object that will fade out on trigger.")]
    public GameObject objectToFade;
    [Tooltip("The speed to transition between current and target alpha.")]
    [Range(0f, 1f)]
    public float fadeSpeed = 0.5f;
    [Tooltip("The target alpha (value between 0 and 1).")]
    public float alphaAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (objectToFade != null)
            {
                MakeTransparent(objectToFade);
                iTween.FadeTo(objectToFade, alphaAmount, fadeSpeed);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (objectToFade != null)
            {
                MakeOpaque(objectToFade);
                iTween.FadeTo(objectToFade, alphaAmount, fadeSpeed);
            }
        }
    }

    void MakeTransparent(GameObject objectToFade)
    {
        Material m1 = objectToFade.GetComponent<Renderer>().material;
        m1.SetFloat("_Mode", 2);
        m1.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        m1.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m1.SetInt("_ZWrite", 0);
        m1.DisableKeyword("_ALPHATEST_ON");
        m1.EnableKeyword("_ALPHABLEND_ON");
        m1.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        m1.renderQueue = 3000;
    }

    void MakeOpaque(GameObject objectToFade)
    {
        Material m1 = objectToFade.GetComponent<Renderer>().material;
        m1.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        m1.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        m1.SetInt("_ZWrite", 1);
        m1.DisableKeyword("_ALPHATEST_ON");
        m1.DisableKeyword("_ALPHABLEND_ON");
        m1.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        m1.renderQueue = -1;
    }

}
