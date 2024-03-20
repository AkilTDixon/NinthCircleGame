using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Mime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


/* todo
    ability to turn and even move in mid-air
    too fast, too slippery
    still get stuck
 */

public class PlayerMovement : MonoBehaviour {

    [SerializeField] private PlayerInfoScript playerInfo;

    [Header("Movement")]
    private float moveSpeed;
    [SerializeField] private float walkSpeed;


    public float groundDrag;
    public Transform orientation;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    public float airMultiplier;
    private bool readyToJump = true;


    [SerializeField] private float dashForce;
    [SerializeField] private float dashPeriod;
    [SerializeField] private float dashCooldown;
    private bool readyToDash = true;
    private float basePeriod;

    // Slope
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool isOnSlope = false;
    private bool jumpingFromSlope = false;
/*    
    [Header("Camera")]
    [SerializeField] Transform camera;
    [SerializeField] private KeyCode switchCameraKey = KeyCode.C; 
    private bool isFirstPerson = true;*/
    
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;
    // private bool grounded2; // new method
    [SerializeField] Transform groundCheck;
    [SerializeField] private float groundCheckRadius;

/*    [Header("Keybinds")] 
    public KeyCode jumpKey = KeyCode.Space;  //todo eliminate this.  shpuld use Unity mapping
    public KeyCode dashKey= KeyCode.LeftShift;*/

    [Header("Debug")] 
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Transform groundCheckDebugSphere = null;
    private bool showGroundCheckDebugSphere;
    
    public MovementState state;
    public enum MovementState {
        walking,
        dashing,
        air
    }

    private float horizontalInput;
    private float verticalInput;

    private Vector3 movedDirection;
    public Rigidbody rb;

    private bool Dead = false;
    private bool ChargeActive = false;

    private float elapsedTimeSinceLastFootstep;
    
    void Start() {

        basePeriod = dashPeriod;
        dashPeriod = 0;
        //Akil
        GameEvents.current.onSkipItemInfo += PlayerReviveEvent;
        GameEvents.current.onItemPickup += PlayerDeathEvent; 
        GameEvents.current.onPlayerDeath += PlayerDeathEvent;
        GameEvents.current.onPlayerRevive += PlayerReviveEvent;
        GameEvents.current.onChargeActivated += ChargeActivatedEvent;
        GameEvents.current.onChargeDeactivated += ChargeDeactivatedEvent;
        //


        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        showGroundCheckDebugSphere = (groundCheckDebugSphere != null);
        elapsedTimeSinceLastFootstep = 0.0f;

        
        
    }

    // Update is called once per frame
    void Update() {

        if (PlayerInfoScript.Instance.onIntro || PlayerInfoScript.Instance.finalBossKilled) {
            rb.isKinematic = true; //rb.isKinematic set true once by PlayerInfoScript when sequence ended
        }
        else {
            
            // Player has control
            
            grounded = IsGrounded();
            MyInput();
            StateHandler();
            if (!ChargeActive)
                SpeedControl();
            if (grounded)
                rb.drag = groundDrag;
            else rb.drag = 0;
        }
        
        

        updateDebugInfo();
    }

    private void FixedUpdate() {
        elapsedTimeSinceLastFootstep += Time.deltaTime;

        if (!Dead)
        {
            if (!ChargeActive)
                MovePlayer();
            else
                ForcedCharge();
        }
    }

    private bool IsGrounded() {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    private bool isSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0f;
        }
        return false; //didn't hit anything
    }

    private Vector3 getSlopeMoveDirection() {
        return Vector3.ProjectOnPlane(movedDirection, slopeHit.normal).normalized;
    }

    private void MyInput() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if (!ChargeActive)
        {
            if (Input.GetKey(PlayerInfoScript.Instance.jumpKey) && readyToJump && grounded)
            {
                readyToJump = false;
                Jump();
                Invoke(nameof(ResetJump), jumpCooldown); // enables holding down the space to jump continuously.
            }
            if (Input.GetKey(PlayerInfoScript.Instance.dashKey) && readyToDash)
            {
                float cooldown = dashCooldown;
                if (state == MovementState.air)
                    cooldown += 3f;
                readyToDash = false;
                Dash();
                Invoke(nameof(ResetDash), cooldown);

            }
/*            if (Input.GetKeyDown(switchCameraKey))
            {
                toggleCameraPerspective();
            }*/
        }
    }
    

    /*private void toggleCameraPerspective() {
        
        isFirstPerson = !isFirstPerson;
        Debug.Log("Toggle Camera. IsFirstPerson=" + isFirstPerson);
        float zpos = 0f;
        if (!isFirstPerson) {
            zpos = 2f;
        }   
        camera.localPosition = new Vector3(camera.localPosition.x, camera.localPosition.y, zpos);
    }*/
    private void ForcedCharge()
    {
        verticalInput = 1f;
        float chargeSpeed = 35f;
        movedDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        movedDirection.y = 0f;
        isOnSlope = isSlope();
        if (isOnSlope && !jumpingFromSlope)
        {
            rb.AddForce(getSlopeMoveDirection() * chargeSpeed * 20f, ForceMode.Force);
            if (rb.velocity.y > 0) //player moving up
                rb.AddForce(Vector3.down * 80f, ForceMode.Force); // prevent slope bobbing
        }

        // on ground
        if (grounded)
        {
            rb.AddForce(movedDirection.normalized * chargeSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            // in air
            rb.AddForce(movedDirection.normalized * chargeSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        rb.useGravity = !isOnSlope; // prevent sliding down

    }
    private void MovePlayer() {
        movedDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        isOnSlope = isSlope();
        if (isOnSlope && !jumpingFromSlope) {
            rb.AddForce(getSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
            if (rb.velocity.y > 0) //player moving up
                rb.AddForce(Vector3.down * 80f, ForceMode.Force); // prevent slope bobbing
        }
        
        // on ground
        if (grounded) {
            rb.AddForce(movedDirection.normalized * moveSpeed * 10f, ForceMode.Force);

            if (isTimeToTakeAfootStep() && (movedDirection[0] != 0 || movedDirection[1] != 0 || movedDirection[2] != 0)) {
                GameEvents.current.playerFootstep();
            }
        }
        else if (!grounded) {
            // in air
            rb.AddForce(movedDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        rb.useGravity = !isOnSlope; // prevent sliding down

    }

    private void SpeedControl() {
        if (isOnSlope) {
            // on slope.  limit speed on all the axes
            if (rb.velocity.magnitude > moveSpeed) 
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        else {
            if (dashPeriod <= 0f)
            {
                // on ground (or air).  Only limit speed on X and Z axes
                Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                if (flatVel.magnitude > moveSpeed)
                {
                    Vector3 limitedVel = flatVel.normalized * moveSpeed;
                    rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
                }
            }
            
        }
        
        
    }

    private void Jump() {
        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);  // make sure we always jump the same height
        rb.AddForce(transform.up * jumpForce * playerInfo.GetJumpForce(), ForceMode.Impulse); //only apply force once.
        jumpingFromSlope = true;
    }
    

    private void ResetJump() {
        readyToJump = true;
        jumpingFromSlope = false;
    }
    private void Dash()
    {
        GameEvents.current.PlayerDash();
        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        Vector3 f = movedDirection * dashForce;
        if (grounded)
            f *= 1.5f;
        if (PlayerInfoScript.Instance.speedBuffAdjustAbsolute != 0f)
            f /= 2.0f;
        rb.AddForce(f, ForceMode.Impulse); //only apply force once.
        if (dashPeriod <= 0f)
            StartCoroutine(DashTime());
        
    }
    private void ResetDash()
    {
        readyToDash = true;
    }
    private IEnumerator DashTime()
    {
        dashPeriod = basePeriod;
        yield return new WaitForSeconds(dashPeriod);
        dashPeriod = 0f;
    }
    private void StateHandler() {
        if (grounded && Input.GetKeyDown(PlayerInfoScript.Instance.dashKey)) {
            state = MovementState.dashing;          
        } else if (grounded) {
            state = MovementState.walking;
            moveSpeed = (walkSpeed + playerInfo.speedBuffAdjustAbsolute) * playerInfo.GetSpeed();
        } else {
            state = MovementState.air;
        }
        // make sure buffs don't make us go backwards
        if (moveSpeed < 0)
            moveSpeed = 0f;
    }

    private void updateDebugInfo() {
        string s = "Speed :" + String.Format("{0:#,0.00}", rb.velocity.magnitude) +" ";
        s += " Grounded: " + grounded;
        s += " Slope: " + isOnSlope;
        text.text = s;

        if (showGroundCheckDebugSphere) {
            groundCheckDebugSphere.localScale = new Vector3(groundCheckRadius/2,groundCheckRadius/2, groundCheckRadius/2);
        }

    }

    public void ChargeActivatedEvent(int unused)
    {
        ChargeActive = true;
    }
    public void ChargeDeactivatedEvent()
    {
        ChargeActive = false;
    }

    //Akil
    public void PlayerDeathEvent()
    {
        Dead = true;
    }
    public void PlayerReviveEvent()
    {
        Dead = false;
    }

    private bool isTimeToTakeAfootStep() {
        if ((elapsedTimeSinceLastFootstep * walkSpeed * 0.5) >= 1){
            elapsedTimeSinceLastFootstep = 0;
            return true;
        }
        return false;
    }

}
