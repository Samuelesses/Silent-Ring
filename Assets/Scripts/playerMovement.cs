using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float mouseSensitivity = 2f;
    public float groundCheckDistance = 0.4f;
    public LayerMask groundMask;

    [Header("References")]
    public Transform cameraTransform;
    public ParticleSystem landingParticlePrefab;

    private Rigidbody rb;
    private float xRotation = 0f;
    private bool isGrounded;
    private bool wasGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        LookAround();
        GroundCheck();

        if (!wasGrounded && isGrounded && landingParticlePrefab != null)
        {
            SpawnLandingParticle();
        }
        wasGrounded = isGrounded;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move = move.normalized * moveSpeed;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = move.x;
        velocity.z = move.z;
        rb.linearVelocity = velocity;
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.position + Vector3.down * 0.5f, groundCheckDistance, groundMask);
    }

    void SpawnLandingParticle()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        if (Physics.Raycast(origin, Vector3.down, out hit, 2f, groundMask))
        {
            ParticleSystem particle = Instantiate(landingParticlePrefab, hit.point, Quaternion.identity);
            particle.Play();
            Destroy(particle.gameObject, particle.main.duration + particle.main.startLifetime.constantMax);
        }
        else
        {
            ParticleSystem particle = Instantiate(landingParticlePrefab, transform.position + Vector3.down * 0.5f, Quaternion.identity);
            particle.Play();
            Destroy(particle.gameObject, particle.main.duration + particle.main.startLifetime.constantMax);
        }
    }
}
