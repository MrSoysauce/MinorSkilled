using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BreakableBlock : MonoBehaviour
{
    [SerializeField] private float impactBreakForce;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision c)
    {
        if (!c.gameObject.CompareTag("Player"))
        {
            if (c.relativeVelocity.magnitude > impactBreakForce)
                Destroy(this.gameObject);
        }
    }
}
