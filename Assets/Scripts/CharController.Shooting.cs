using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CharController : MonoBehaviour {

    private List<Vector3> shotStarts = new List<Vector3>();
    private AudioSource swingSoundSource;
    private AudioSource shootSoundSource;
    private float shootModeStartTime;
    private float shootAngleDeg;
    private float shootStartDst;
    float charge = 0.0f;

    public float MaximumShootDistance = 2f;
    public float MaximumBallSpeedForShoot = 5;

    private void InitializeShooting() {
        swingSoundSource = gameObject.AddComponent<AudioSource>();
        swingSoundSource.clip = Resources.Load<AudioClip>("Sounds/SwingPower");
        swingSoundSource.loop = true;
        shootSoundSource = gameObject.AddComponent<AudioSource>();
        shootSoundSource.clip = Resources.Load<AudioClip>("Sounds/Shoot");
    }

    private bool HandleShootMoving() {
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
        bool isMoving = Mathf.Abs(angleDelta) > 0;
        return isMoving;
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

    private void UpdateShooting() {
        float w = ((Time.time - shootModeStartTime) * 3) / Mathf.PI * 2;
        float chargeWave = -Mathf.Cos(w);
        charge = Mathf.Clamp01(chargeWave * 0.5f + 0.5f);
        swingSoundSource.pitch = 1.0f + chargeWave * .5f;

        bool isMoving = HandleShootMoving();
        animator.SetBool("isWalking", isMoving);
        bool shootInputPressed = Input.GetButtonUp(shootKey);
        if(shootInputPressed) {
            Shoot();
        }
    }

    private void EnterShootMode() {
        charMode = CharMode.Shoot;
        shootModeStartTime = Time.time;
        var ballPos2d = new Vector2(ball.transform.position.x, ball.transform.position.z);
        var playerPos2d = new Vector2(transform.position.x, transform.position.z);
        var delta2d = playerPos2d - ballPos2d;
        shootAngleDeg = Mathf.Rad2Deg * Mathf.Atan2(delta2d.y, delta2d.x) + 180;
        shootStartDst = delta2d.magnitude;
        swingSoundSource.Play();
    }


    private void Shoot()
    {
        if(charMode != CharMode.Shoot) {
            Debug.LogWarning("Attempted to shoot out of shoot mode");
            return;
        }
        charMode = CharMode.Walk;
        swingSoundSource.Stop();
        if (charge < 0.03)
        {
            return;
        }
        shootSoundSource.Play();
        shotStarts.Add(ball.transform.position);
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        Vector3 normalDirection = GetBallVector();
        ballRb.AddForce(normalDirection * charge * 1000);
    }

    private void DrawShootGUI() {
        var chargeBarWidth = (Screen.width / 5);
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
}
