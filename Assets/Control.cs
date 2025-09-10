using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Control : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0, moveZ).normalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        if (movement != Vector3.zero)
            rb.rotation = Quaternion.LookRotation(movement);
    }
}