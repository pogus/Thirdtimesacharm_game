using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovementbefore : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    Animator animator;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float z;
    Vector3 velocity;

    private bool IsGrounded;
   // private bool IsJumping;


    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        //checking if we hit the ground to reset our falling velocity, otherwise we will fall faster the next time
        IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (IsGrounded && velocity.y < 0)
        {
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsJumping", false);
            //IsJumping = false;
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        //right is the red Axis, foward is the blue axis
        Vector3 move = transform.right * x + transform.forward * z;
        
        controller.Move(move * speed * Time.deltaTime);

        animator.SetFloat("speedwalk", z);
        //check if the player is on the ground so he can jump
        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            //the equation for jumping
            animator.SetBool("IsJumping", true);
            //IsJumping = true;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        animator.SetBool("IsGrounded", IsGrounded);
        controller.Move(velocity * Time.deltaTime);
        animator.SetBool("IsFalling", true);
    }
}