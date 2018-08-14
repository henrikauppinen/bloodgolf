using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController : MonoBehaviour {
    private void SpawnBall()
    {
        Ball = Instantiate(Resources.Load<GameObject>("Ball"));
        Ball.tag = "Ball";
        Ball.name = "Ball-" + PlayerID;
        var ballPos = transform.position + transform.rotation * Vector3.forward * 5;
        ballPos.y = transform.position.y + 5;
        shotStarts.Add(ballPos);
        Ball.transform.position = ballPos;

        var mr = Ball.GetComponent<MeshRenderer>();
        var mat = new Material(mr.material)
        {
            color = Color.Lerp(Color.white, PlayerColor, .5f)
        };
        mr.material = mat;
        var tr = Ball.GetComponent<TrailRenderer>();
        var cg = new Gradient
        {
            mode = GradientMode.Blend
        };
        cg.SetKeys(
            new[]
            {
                new GradientColorKey(PlayerColor, 0),
                new GradientColorKey(PlayerColor, 1.0f),
            },
            new[] {
                new GradientAlphaKey(.3f, 0),
                new GradientAlphaKey(1f, .5f),
                new GradientAlphaKey(0f, 1.0f),
            }
        );
        tr.colorGradient = cg;
    }

    private Vector3 GetBallVector()
    {
        Vector3 direction = Ball.transform.position - transform.position;
        float distance = direction.magnitude;
        Vector3 normalDirection = direction / distance;
        normalDirection.y = 1.0f;
        return normalDirection;
    }

    private void CheckBallDeath()
    {
        if (Ball.transform.position.y < -5)
        {
            Debug.Log(string.Format("Player {0}'s ball died :(", PlayerID));
            ResetBall();
        }
    }

    private void ResetBall()
    {
        Rigidbody ballRb = Ball.GetComponent<Rigidbody>();
        ballRb.velocity = Vector3.up * 5;
        ballRb.angularVelocity = Vector3.zero;
        Ball.transform.position = shotStarts[shotStarts.Count - 1];
    }
}
