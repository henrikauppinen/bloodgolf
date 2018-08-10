using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CharController : MonoBehaviour {
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
