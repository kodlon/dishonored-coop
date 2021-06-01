using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    public LayerMask groundMask;

    [SerializeField] private float speed = 12f;
    [SerializeField] private float sprintSpeed = 20f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float groundDistance = 0.4f;

    private Vector3 velocity;
    private bool isGrounded;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position + new Vector3(0f, -1.829f, 0f), groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(Input.GetButton("Sprint") && isGrounded ? move * sprintSpeed * Time.deltaTime : move * speed * Time.deltaTime);

        // if (Input.GetButton("Sprint")) { controller.Move(move * sprintSpeed * Time.deltaTime); }
        // else { controller.Move(move * speed * Time.deltaTime); }
        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
