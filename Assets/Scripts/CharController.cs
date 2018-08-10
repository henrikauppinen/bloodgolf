using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    private Color playerColor;
    Animator animator;
    GameObject ball;
    private GameObject arrow;
    private AudioSource swingSoundSource;
    private AudioSource shootSoundSource;

    public float WalkingSpeed = 5.0f;
    public int PlayerID = 1;
    public float MaximumShootDistance = 2f;
    public float MaximumBallSpeedForShoot = 5;
    private List<Vector3> shotStarts = new List<Vector3>();

    enum CharMode
    {
        Walk,
        Shoot,
    };

    private CharMode charMode = CharMode.Walk;
    private float shootModeStartTime;
    private float shootAngleDeg;
    private float shootStartDst;
    float charge = 0.0f;
    private string xAxis;
    private string yAxis;
    private string shootKey;
    private string altKey;

    private void Start()
    {
        name = "Player-" + PlayerID;
        playerColor = Constants.PlayerColors[PlayerID - 1];
        animator = GetComponent<Animator>();
        swingSoundSource = gameObject.AddComponent<AudioSource>();
        swingSoundSource.clip = Resources.Load<AudioClip>("Sounds/SwingPower");
        swingSoundSource.loop = true;
        shootSoundSource = gameObject.AddComponent<AudioSource>();
        shootSoundSource.clip = Resources.Load<AudioClip>("Sounds/Shoot");
        SpawnBall();
        xAxis = string.Format("X{0}", PlayerID);
        yAxis = string.Format("Y{0}", PlayerID);
        shootKey = string.Format("Shoot{0}", PlayerID);
        altKey = string.Format("Alt{0}", PlayerID);
        var cameraController = Camera.main.GetComponent<CameraController>();
        if (cameraController)
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
        bool shootInputPressed = Input.GetButtonUp(shootKey);

        bool isMoving = false;
        if (charMode == CharMode.Walk)
        {
            var movement = new Vector3(horizontalInput, 0, verticalInput);
            transform.Translate(movement, Space.World);
            isMoving = movement != Vector3.zero;
            if (isMoving)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.LookRotation(movement),
                    800 * Time.deltaTime
                );
            }
        } else if(charMode == CharMode.Shoot) {
            var angleDelta = Input.GetAxis(xAxis) * Time.deltaTime * -180;
            shootAngleDeg += angleDelta;
            var shootAngleRad = Mathf.Deg2Rad * shootAngleDeg;
            var dst = -Mathf.Min(shootStartDst, MaximumShootDistance * .95f);
            var newPos = new Vector3(
                ball.transform.position.x + Mathf.Cos(shootAngleRad) * dst,
                transform.position.y,
                ball.transform.position.z + Mathf.Sin(shootAngleRad) * dst
            );
            var oldPos = transform.position;
            transform.position = Vector3.MoveTowards(
                transform.position,
                newPos,
                Time.deltaTime * WalkingSpeed
            );
            var posDelta2d = ball.transform.position - transform.position;
            posDelta2d.y = 0;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(posDelta2d),
                200 * Time.deltaTime
            );
            isMoving = Mathf.Abs(angleDelta) > 0;
        }

        animator.SetBool("isWalking", isMoving);
        //animator.SetBool("isShooting", charMode == CharMode.Shoot);
        if(charMode == CharMode.Shoot) {
            float w = ((Time.time - shootModeStartTime) * 3) / Mathf.PI * 2;
            float chargeWave = -Mathf.Cos(w);
            charge = Mathf.Clamp01(chargeWave * 0.5f + 0.5f);
            swingSoundSource.pitch = 1.0f + chargeWave * .5f;
        }
        if (IsInRangeForShoot())
        {
            if (shootInputPressed)
            {
                if(charMode == CharMode.Walk) {
                    charMode = CharMode.Shoot;
                    shootModeStartTime = Time.time;
                    var ballPos2d = new Vector2(ball.transform.position.x, ball.transform.position.z);
                    var playerPos2d = new Vector2(transform.position.x, transform.position.z);
                    var delta2d = playerPos2d - ballPos2d;
                    shootAngleDeg = Mathf.Rad2Deg * Mathf.Atan2(delta2d.y, delta2d.x) + 180;
                    shootStartDst = delta2d.magnitude;
                    swingSoundSource.Play();
                } else if(charMode == CharMode.Shoot) {
                    Shoot();
                }
            }
        }
        else
        {
            if (charMode == CharMode.Shoot)
            {
                charMode = CharMode.Walk;
                charge = 0;
                swingSoundSource.Stop();
            }
        }

        updateArrow();
        CheckBallDeath();
    }

    void updateArrow()
    {
        if (IsInRangeForShoot() || charMode == CharMode.Shoot)
        {
            if (!arrow)
            {
                arrow = Instantiate<GameObject>(Resources.Load<GameObject>("AimArrow"));
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
        if (charMode == CharMode.Shoot)
        {
            var pos = Camera.main.WorldToScreenPoint(transform.position);
            var chargeP = charge;
            var oldColor = GUI.color;
            GUI.Box(new Rect(pos.x, Screen.height - pos.y, chargeBarWidth, 30), "");
            if (chargeP > 0)
            {
                GUI.color = Color.Lerp(Color.yellow, Color.red, chargeP);
                GUI.Box(new Rect(pos.x, Screen.height - pos.y, chargeP * chargeBarWidth, 30), string.Format("{0}%", Mathf.Floor(chargeP * 100)));
            }
            GUI.color = oldColor;
        }
        DrawBallTrackerGUI();
    }

    private void DrawBallTrackerGUI()
    {
        var ballScreenPos = Camera.main.WorldToScreenPoint(ball.transform.position);
        ballScreenPos.y = Screen.height - ballScreenPos.y;
        var offScreenXMargin = Screen.width * -.01;
        var offScreenYMargin = Screen.height * -.01;
        if (
            ballScreenPos.x < -offScreenXMargin || ballScreenPos.x > Screen.width + offScreenXMargin ||
            ballScreenPos.y < -offScreenYMargin || ballScreenPos.y > Screen.height + offScreenYMargin
        )
        {
            var onScreenXPadding = Screen.width * .06f;
            var onScreenYPadding = Screen.height * .06f;
            var onScreenX = Mathf.Clamp(ballScreenPos.x, onScreenXPadding, Screen.width - onScreenXPadding);
            var onScreenY = Mathf.Clamp(ballScreenPos.y, onScreenYPadding, Screen.height - onScreenYPadding);
            var distanceVec = (ball.transform.position - transform.position);
            distanceVec.y = 0;  // ground distance only
            var distance = distanceVec.magnitude;
            GUI.color = playerColor;
            GUI.Label(new Rect(onScreenX, onScreenY, 30, 30), string.Format("{0}m", Mathf.Round(distance)));
        }
    }

    private void Shoot()
    {
        charMode = CharMode.Walk;
        swingSoundSource.Stop();
        if(charge < 0.03) {
            return;
        }
        shootSoundSource.Play();
        shotStarts.Add(ball.transform.position);
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        Vector3 normalDirection = GetBallVector();
        ballRb.AddForce(normalDirection * charge * 1000);

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
        if (ball.transform.position.y < -5)
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
