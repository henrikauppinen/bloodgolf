using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    Animator animator;
    GameObject ball;
    private GameObject arrow;

    public float WalkingSpeed = 5.0f;
    public int PlayerID = 1;
    public float MaximumShootDistance = 2f;
    public float MaximumBallSpeedForShoot = 5;
    private List<Vector3> shotStarts = new List<Vector3>();


    bool shootPhase = false;
    float charge = 0.0f;
    private readonly float maxCharge = 10;
    private string xAxis;
    private string yAxis;
    private string shootKey;
    private string altKey;

    private void Start()
    {
        name = "Player-" + PlayerID;
        animator = GetComponent<Animator>();
        SpawnBall();
        xAxis = string.Format("X{0}", PlayerID);
        yAxis = string.Format("Y{0}", PlayerID);
        shootKey = string.Format("Shoot{0}", PlayerID);
        altKey = string.Format("Alt{0}", PlayerID);
        var cameraController = Camera.main.GetComponent<CameraController>();
        if(cameraController)
        {
            cameraController.UpdateObjectLists();
        }
    }

    private void SpawnBall()
    {
        ball = Instantiate(Resources.Load<GameObject>("Ball"));
        ball.tag = "Ball";
        ball.name = "Ball-" + PlayerID;
        var ballPos = transform.position + transform.rotation * Vector3.forward * 5;
        ballPos.y = transform.position.y + 5;
        shotStarts.Add(ballPos);
        ball.transform.position = ballPos;

        var playerColor = Constants.PlayerColors[PlayerID - 1];
        var mr = ball.GetComponent<MeshRenderer>();
        var mat = new Material(mr.material)
        {
            color = Color.Lerp(Color.white, playerColor, .5f)
        };
        mr.material = mat;
        var tr = ball.GetComponent<TrailRenderer>();
        var cg = new Gradient
        {
            mode = GradientMode.Blend
        };
        cg.SetKeys(
            new[]
            {
                new GradientColorKey(playerColor, 0),
                new GradientColorKey(playerColor, 1.0f),
            },
            new[] {
                new GradientAlphaKey(.3f, 0),
                new GradientAlphaKey(1f, .5f),
                new GradientAlphaKey(0f, 1.0f),
            }
        );
        tr.colorGradient = cg;
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
        float horizontalInput = Input.GetAxis(xAxis) * Time.deltaTime * WalkingSpeed;
        float verticalInput = Input.GetAxis(yAxis) * Time.deltaTime * WalkingSpeed;
        bool shootInput = Input.GetAxis(shootKey) > 0.5;

        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);

        transform.Translate(movement, Space.World);

        bool isMoving = movement != Vector3.zero;

        if (isMoving)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(movement),
                800 * Time.deltaTime
            );
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
                Shoot();
            }
        }
        else
        {
            shootPhase = false;
            charge = 0;
        }

        updateArrow();
        CheckBallDeath();
    }

    void updateArrow()
    {
        if (IsInRangeForShoot() || shootPhase)
        {
            if (!arrow)
            {
                arrow = Instantiate<GameObject>(Resources.Load<GameObject>("AimArrow"));
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
            var pos = Camera.main.WorldToScreenPoint(transform.position);

            var chargeP = charge / maxCharge;
            var oldColor = GUI.color;
            GUI.Box(new Rect(pos.x, Screen.height - pos.y, chargeBarWidth, 30), "");
            GUI.color = Color.Lerp(Color.yellow, Color.red, chargeP);
            GUI.Box(new Rect(pos.x, Screen.height - pos.y, chargeP * chargeBarWidth, 30), string.Format("{0}%", Mathf.Floor(chargeP * 100)));
            GUI.color = oldColor;
        }
    }

    private void Shoot()
    {
        shotStarts.Add(ball.transform.position);
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        Vector3 normalDirection = GetBallVector();
        ballRb.AddForce(normalDirection * charge * 100);
        charge = 0.0f;
    }

    private Vector3 GetBallVector()
    {
        Vector3 direction = ball.transform.position - transform.position;
        float distance = direction.magnitude;
        Vector3 normalDirection = direction / distance;
        normalDirection.y = 1.0f;
        return normalDirection;
    }

    private void CheckBallDeath()
    {
        if(ball.transform.position.y < -5)
        {
            Debug.Log(string.Format("Player {0}'s ball died :(", PlayerID));
            ResetBall();
        }
    }

    private void ResetBall()
    {
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        ballRb.velocity = Vector3.up * 5;
        ballRb.angularVelocity = Vector3.zero;
        ball.transform.position = shotStarts[shotStarts.Count - 1];
    }
}
