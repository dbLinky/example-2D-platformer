using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{

    //Player control parameters
    public float moveSpeed = 6;
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    float timeToWallUnstick;
    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    public float stompSpeed = 1.5f;
    float velocityXSmoothing;
    float jumpRememberTime = 2;
    float jumpPressedRemember = 0f;
    public float fHorizontalDampingWhenStopping;
    public float fHorizontalDampingWhenTurnning;
    public float fHorizontalDampingBasic;


    //Player ability toggles
    public bool canDoubleJump = true;
    public bool canWallDoubleJump = true;
    public bool canWallJump = true;
    public bool canStomp = true;



    //player state variables
    public bool isGrounded;
    public bool isJumping;
    public bool isFacingRight;
    public bool doubleJumped;
    public bool wallDoubleJumped;
    public bool wallJumped;
    public bool isWallRunning;
    public bool isSlopeSliding;
    public bool wallSliding;
    public int wallDirX;
    public bool isStomped;


    //variables
    Controller2D controller;
    Vector2 directionalInput;
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    Vector3 velocity;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    private CameraShake _shake;

    // Start is called before the first frame update
    void Start()
    {
        //Settings
        controller = GetComponent<Controller2D>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        _shake = GameObject.FindGameObjectWithTag("ScreenShake").GetComponent<CameraShake>();

        isFacingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        SetDirectionalInput(directionalInput);
        CalculateVelocity();
        HandleWallSliding();
        controller.Move(velocity * Time.deltaTime, directionalInput);
        jumpPressedRemember -= Time.deltaTime;


        //direction detection
        if (directionalInput.x < 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            isFacingRight = false; //ForCamera
        }
        else if (directionalInput.x > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            isFacingRight = true; //ForCamera
        }

        //Gravity + OnGround?
        if (controller.collisions.below)
        {
            isGrounded = true;
            velocity.y = 0;
        } else if (controller.collisions.above)
        {
            velocity.y = 0;
        }

        /* //HorizontalVelocity 'Acceleretion' if needed.
        float fHorizontalVelocity = velocity.x;
        fHorizontalVelocity += Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.01f)
        {
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenStopping, Time.deltaTime * 10f);

        }
        else if (Mathf.Sign(Input.GetAxisRaw("Horizontal")) != Mathf.Sign(fHorizontalVelocity))
        {
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenTurnning, Time.deltaTime * 10f);
        }
        else
        {
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 10f);
        }

        velocity = new Vector2(fHorizontalVelocity, velocity.y); */



        if (isGrounded) //Player is on the ground
        {
            isJumping = false;
            doubleJumped = false;
            canStomp = true;
            isStomped = false;


            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = maxJumpVelocity;
                isJumping = true;
                isWallRunning = true;
                isGrounded = false;
                canDoubleJump = true;
            }


        }
        else //Player is in the air
        {
            jumpPressedRemember -= Time.deltaTime;
            if (Input.GetButtonUp("Jump")) //Half Jump
            {
                if (velocity.y > minJumpVelocity)
                {
                    velocity.y = minJumpVelocity;
                }

            }

            if (Input.GetButtonDown("Jump")) //Double jump
            {
                if (canDoubleJump)
                {
                    if (!doubleJumped)
                    {
                        velocity.y = maxJumpVelocity;
                        doubleJumped = true;
                        canDoubleJump = false;
                    }
                }
                if (canWallDoubleJump) //Need to fix walldoublejump issue
                {
                    if (!wallDoubleJumped)
                    {
                        velocity.y = maxJumpVelocity;
                        wallDoubleJumped = true;
                        canWallDoubleJump = false;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.C)) // Air Stomp
            {
                if (canStomp)
                {
                    velocity.y = stompSpeed * gravity;
                    canStomp = false;
                    isStomped = true;
                    _shake.CamShake();
                }
            }
        }

    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }


    void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;
            canDoubleJump = true;
            doubleJumped = false;
            isJumping = false;
            isGrounded = false;
            wallDoubleJumped = false;
            canWallDoubleJump = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                if (wallDirX == -1 && directionalInput.x == 0)
                {
                    velocity.x = -1f;
                }
                else if (wallDirX == 1 && directionalInput.x == 0)
                {
                    velocity.x = 1f;
                }

                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }

            if (canWallJump) //WallJumping
            {
                if (Input.GetButtonDown("Jump") && wallJumped == false && isGrounded == false)
                {
                    if (Input.GetKey(KeyCode.LeftArrow) && wallSliding)
                    {
                        if (wallDirX == -1)
                        {
                            velocity.x = maxJumpVelocity * moveSpeed * 0.1f;
                        }
                        velocity.y = maxJumpVelocity;
                        //transform.eulerAngles = new Vector3(0, 0, 0);

                        if (Input.GetButtonUp("Jump")) //Half Jump
                        {
                            if (velocity.x > minJumpVelocity)
                            {
                                velocity.x = minJumpVelocity;
                            }
                        }

                        //isWallRunning = true;

                    }
                    else if (Input.GetKey(KeyCode.RightArrow) && wallSliding)
                    {
                        if (wallDirX == 1)
                        {
                            velocity.x = -(maxJumpVelocity * moveSpeed * 0.1f);
                        }
                        velocity.y = maxJumpVelocity;
                        //transform.eulerAngles = new Vector3(0, 180, 0);

                        if (Input.GetButtonUp("Jump")) //Half Jump
                        {
                            if (velocity.x > minJumpVelocity)
                            {
                                velocity.x = minJumpVelocity;
                            }
                        }

                        //isWallRunning = true;
                    }

                    StartCoroutine(WallJumpWaiter());

                }
            }

        }

    }



    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }

    IEnumerator WallJumpWaiter()
    {
        wallJumped = true;
        yield return new WaitForSeconds(0.5f);
        wallJumped = false;
    }
}