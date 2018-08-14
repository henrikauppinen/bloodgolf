using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject[] Balls { get; private set; }
    public GameObject[] Players { get; private set; }
    public PlayerController[] PlayerControllers { get; private set; }
    private GUISkin guiSkin;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Balls = new GameObject[] { };
        Players = new GameObject[] { };
        guiSkin = Resources.Load<GUISkin>("BloodSkin");
    }

    public void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void UpdateObjectLists()
    {
        Balls = GameObject.FindGameObjectsWithTag("Ball");
        Players = GameObject.FindGameObjectsWithTag("Player");
        PlayerControllers = new PlayerController[Players.Length];
        for (var i = 0; i < Players.Length; i++)
        {
            PlayerControllers[i] = Players[i].GetComponent<PlayerController>();
        }
    }

    public void OnGUI()
    {
        GUI.skin = guiSkin;
        var playerLabelStyle = new GUIStyle(GUI.skin.label);
        playerLabelStyle.alignment = TextAnchor.LowerCenter;
        playerLabelStyle.fontSize = 25;
        foreach (var pc in PlayerControllers)
        {
            var playerAreaX0 = (pc.PlayerID - 1) / 4.0f * Screen.width;
            var playerAreaW = Screen.width / 4;
            var playerAreaX1 = playerAreaX0 + playerAreaW;
            var playerAreaRect = new Rect(
                playerAreaX0,
                Screen.height - 100,
                playerAreaW,
                95
            );
            GUI.contentColor = pc.PlayerColor;
            GUI.Label(
                playerAreaRect,
                string.Format("Player {0}\nShots: {1} - Health: {2}", pc.PlayerID, pc.NumShots, pc.Health),
                playerLabelStyle
            );
            DrawBallTrackerGUI(pc.Ball, pc);
        }
    }


    private void DrawBallTrackerGUI(GameObject ball, PlayerController player)
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
            Debug.Log(string.Format("{0}: {1}", player.PlayerID, ballScreenPos));
            var onScreenXPadding = Screen.width * .06f;
            var onScreenYPadding = Screen.height * .06f;
            var onScreenX = Mathf.Clamp(ballScreenPos.x, onScreenXPadding, Screen.width - onScreenXPadding);
            var onScreenY = Mathf.Clamp(ballScreenPos.y, onScreenYPadding, Screen.height - onScreenYPadding);
            var distanceVec = (ball.transform.position - transform.position);
            distanceVec.y = 0;  // ground distance only
            var distance = distanceVec.magnitude;
            GUI.color = player.PlayerColor;
            GUI.Label(new Rect(onScreenX, onScreenY, 30, 30), string.Format("{0}m", Mathf.Round(distance)));
        }
    }

}
