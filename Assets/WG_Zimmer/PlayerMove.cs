using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed;

    public float groundDrag;

    public float playerHeigt;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horivontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb_body;
    // Start is called before the first frame update
    void Start()
    {
        rb_body = GetComponent<Rigidbody>();
        rb_body.freezeRotation = true;
        
    }

    // Update is called once per frame
    private void PlayerInput(){
        horivontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }
    void Update()
    {
        PlayerInput();

        grounded = Physics.Raycast(transform.position, Vector3.down,playerHeigt * 0.5f + 0.2f,whatIsGround);

        if(grounded){
            rb_body.drag = groundDrag;
        }else{
            rb_body.drag = 0;
        }
    }

    private void FixedUpdate(){
        MovePlayer();
        SpeedControll();
    }

    private void MovePlayer(){
        moveDirection = orientation.forward * verticalInput + orientation.right * horivontalInput;

        rb_body.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControll(){
        Vector3 flatVel = new Vector3(rb_body.velocity.x,0f,rb_body.velocity.z);

        if(flatVel.magnitude > moveSpeed){
            Vector3 limiter = flatVel.normalized * moveSpeed;
            rb_body.velocity = new Vector3(limiter.x,0f,limiter.z);
        }
    }
}
