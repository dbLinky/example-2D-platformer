using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;

    public float startTime = 0f;
    public float timer = 0f;
    public float holdTime = 2.0f;
    public float cameraZPos = -10f;
    public float cameraXOffset = 5f;
    public float cameraYOffset = 1f;

    public float horizontalSpeed = 2f;
    public float verticalSpeed = 10f;

    //Private
    private Transform _camera;
    private Player _playerController;



    // Start is called before the first frame updat
    void Start()
    {
        if (!player) //If no Player
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        _playerController = player.GetComponent<Player>();
        _camera = Camera.main.transform;

        //Camera starting position
        _camera.position = new Vector3(
            player.transform.position.x + cameraXOffset,
            player.transform.position.y + cameraYOffset,
            player.transform.position.z + cameraZPos
            );



    }

    // Update is called once per frame
    void Update()
    {

        if (_playerController.isFacingRight) //Adjust camera to player direction
        {
            _camera.position = new Vector3(
                Mathf.Lerp(_camera.position.x, player.transform.position.x + cameraXOffset, horizontalSpeed * Time.deltaTime),
                Mathf.Lerp(_camera.position.y, player.transform.position.y + cameraYOffset, verticalSpeed * Time.deltaTime),
                cameraZPos
                );
        }
        else
        {
            _camera.position = new Vector3(
               Mathf.Lerp(_camera.position.x, player.transform.position.x - cameraXOffset, horizontalSpeed * Time.deltaTime),
               Mathf.Lerp(_camera.position.y, player.transform.position.y + cameraYOffset, verticalSpeed * Time.deltaTime),
               cameraZPos
               );
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            timer += Time.deltaTime;

            // Once the timer float has added on the required holdTime, changes the bool (for a single trigger), and calls the function
            if (timer > (startTime + holdTime))
            {
                if (Input.GetKey(KeyCode.W))
                {
                    ButtonHeldUp();
                }
                if (Input.GetKey(KeyCode.S))
                {
                    ButtonHeldDown();
                }

            }
        }
        else
        {
            timer = 0;
        }

    }

    void ButtonHeldUp()
    {

        _camera.position = new Vector3(
               _camera.position.x,
               Mathf.Lerp(_camera.position.y, 5.5f + _camera.position.y + cameraYOffset, verticalSpeed * Time.deltaTime),
               cameraZPos
               );
    }

    void ButtonHeldDown()
    {

        _camera.position = new Vector3(
               _camera.position.x,
               Mathf.Lerp(_camera.position.y, _camera.position.y - 6f - cameraYOffset, verticalSpeed * Time.deltaTime),
               cameraZPos
               );
    }



/* Different camera style, if needed for the future. 
public Controller2D target;
public float verticalOffset;
public float lookAheadDstX;
public float lookSmoothTimeX;
public float verticalSmoothTime;
public Vector2 focusAreaSize;

FocusArea focusArea;

float currentLookAheadX;
float targetLookAheadX;
float lookAheadDirX;
float smoothLookVelocityX;
float smoothVelocityY;

bool lookAheadStopped;

void Start()
{
    focusArea = new FocusArea(target.collider.bounds, focusAreaSize);
}

void LateUpdate()
{
    focusArea.Update(target.collider.bounds);

    Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;

    if (focusArea.velocity.x != 0)
    {
        lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
        if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0)
        {
            lookAheadStopped = false;
            targetLookAheadX = lookAheadDirX * lookAheadDstX;
        }
        else
        {
            if (!lookAheadStopped)
            {
                lookAheadStopped = true;
                targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
            }
        }
    }


    currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

    focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
    focusPosition += Vector2.right * currentLookAheadX;
    transform.position = (Vector3)focusPosition + Vector3.forward * -10;
}

void OnDrawGizmos()
{
    Gizmos.color = new Color(1, 0, 0, .5f);
    Gizmos.DrawCube(focusArea.centre, focusAreaSize);
}

struct FocusArea
{
    public Vector2 centre;
    public Vector2 velocity;
    float left, right;
    float top, bottom;


    public FocusArea(Bounds targetBounds, Vector2 size)
    {
        left = targetBounds.center.x - size.x / 2;
        right = targetBounds.center.x + size.x / 2;
        bottom = targetBounds.min.y;
        top = targetBounds.min.y + size.y;

        velocity = Vector2.zero;
        centre = new Vector2((left + right) / 2, (top + bottom) / 2);
    }

    public void Update(Bounds targetBounds)
    {
        float shiftX = 0;
        if (targetBounds.min.x < left)
        {
            shiftX = targetBounds.min.x - left;
        }
        else if (targetBounds.max.x > right)
        {
            shiftX = targetBounds.max.x - right;
        }
        left += shiftX;
        right += shiftX;

        float shiftY = 0;
        if (targetBounds.min.y < bottom)
        {
            shiftY = targetBounds.min.y - bottom;
        }
        else if (targetBounds.max.y > top)
        {
            shiftY = targetBounds.max.y - top;
        }
        top += shiftY;
        bottom += shiftY;
        centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        velocity = new Vector2(shiftX, shiftY);
    }
*/


}
