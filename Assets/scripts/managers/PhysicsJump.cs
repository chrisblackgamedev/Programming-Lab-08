using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsJump : MonoBehaviour
{
    [SerializeField] float jumpForce = 10;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
