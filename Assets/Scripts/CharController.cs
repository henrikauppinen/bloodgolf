using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{

    Animator animator;
    GameObject ball;
    public GameObject ArrowPrefab;
    private GameObject arrow;
    Camera camera;

    [SerializeField] float speed = 5.0f;

    public float MaximumShootDistance = 2f;
    public float MaximumBallSpeedForShoot = 5;


    bool shootPhase = false;
    float charge = 0.0f;
    private readonly float maxCharge = 10;

    private void Start()
    {
        animator = GetComponent<Animator>();
        ball = GameObject.FindWithTag("Ball");
        camera = Camera.main;
    }

    private bool IsInRangeForShoot()
    {
        var distSqr = (transform.position - ball.transform.position).sqrMagnitude;
        if (distSqr > MaximumShootDistance * MaximumShootDistance)  // Too far away
        {
            return false;
        }
        var rb = ball.GetComponent<Rigidbody>();
        if (rb.velocity.sqrMagnitude > MaximumBallSpeedForShoot * MaximumBallSpeedForShoot)  // Ball rolling
        {
            return false;
        }
        return true;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float verticalInput = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        bool shootInput = Input.GetKey(KeyCode.Space);

        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);

        transform.Translate(movement, Space.World);

        bool isMoving = movement != Vector3.zero;

        if (isMoving)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }

        animator.SetBool("isWalking", isMoving);
        animator.SetBool("isShooting", shootPhase);
        if (IsInRangeForShoot())
        {
            if (shootInput)
            {
                shootPhase = true;
                charge = Mathf.Clamp(charge + Time.deltaTime * 5.0f, 0, maxCharge);
            }


            if (!shootInput && shootPhase)
            {
                shootPhase = false;
                shoot();
            }
        }
        else
        {
            shootPhase = false;
            charge = 0;
        }

        updateCameraPosition(transform.position, ball.transform.position);
        updateArrow();
    }

    void updateArrow()
    {
        if (IsInRangeForShoot() || shootPhase)
        {
            if (!arrow)
            {
                arrow = Instantiate<GameObject>(ArrowPrefab);
                Debug.Log(arrow);
            }
            arrow.SetActive(true);
            var vec = GetBallVector();
            vec.y = 0;
            arrow.transform.position = ball.transform.position + vec * .2f;
            arrow.transform.rotation = Quaternion.LookRotation(vec);
        }
        else
        {
            if (arrow)
            {
                arrow.SetActive(false);
            }
        }
    }

    void OnGUI()
    {
        var chargeBarWidth = (Screen.width / 5);
        if (shootPhase)
        {
            var pos = camera.WorldToScreenPoint(transform.position);

            var chargeP = charge / maxCharge;
            var oldColor = GUI.color;
            GUI.Box(new Rect(pos.x, Screen.height - pos.y, chargeBarWidth, 30), "");
            GUI.color = Color.Lerp(Color.yellow, Color.red, chargeP);
            GUI.Box(new Rect(pos.x, Screen.height - pos.y, chargeP * chargeBarWidth, 30), string.Format("{0}%", Mathf.Floor(chargeP * 100)));
            GUI.color = oldColor;
        }
    }

    void shoot()
    {
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        Vector3 normalDirection = GetBallVector();
        ballRb.AddForce(normalDirection * charge * 100);
        charge = 0.0f;
    }

    private Vector3 GetBallVector()
    {
        Transform ballT = ball.GetComponent<Transform>();
        Vector3 direction = ballT.position - transform.position;
        float distance = direction.magnitude;
        Vector3 normalDirection = direction / distance;
        normalDirection.y = 1.0f;
        return normalDirection;
    }

    void updateCameraPosition(Vector3 playerPosition, Vector3 ballPosition)
    {

        float cameraDistance = 10 + Mathf.Abs(ballPosition.x - playerPosition.x);
        Vector3 newCameraPosition = new Vector3(getXBetween(playerPosition, ballPosition), cameraDistance - 3, transform.position.z - cameraDistance);
        camera.transform.position = newCameraPosition;
    }

    float getXBetween(Vector3 a, Vector3 b)
    {
        return a.x + (b.x - a.x) / 2;
    }
}
