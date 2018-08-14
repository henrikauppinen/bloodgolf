using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    public Color PlayerColor { get; private set; }
    private Animator animator;
    public GameObject Ball { get; private set; }
    private GameObject arrow;
    public int PlayerID = 1;

    public int Health
    {
        get;
        private set;
    }

    enum CharMode
    {
        Walk,
        Shoot,
    };

    private CharMode charMode = CharMode.Walk;

    private string xAxis;
    private string yAxis;
    private string shootKey;
    private string altKey;

    private void Start()
    {
        Health = 100;
        name = "Player-" + PlayerID;
        PlayerColor = Constants.PlayerColors[PlayerID - 1];
        animator = GetComponent<Animator>();
        xAxis = string.Format("X{0}", PlayerID);
        yAxis = string.Format("Y{0}", PlayerID);
        shootKey = string.Format("Shoot{0}", PlayerID);
        altKey = string.Format("Alt{0}", PlayerID);
        InitializeShooting();
        SpawnBall();
        GameManager.instance.UpdateObjectLists();
    }

    void Update()
    {
        switch (charMode)
        {
            case CharMode.Walk:
                UpdateWalking();
                break;
            case CharMode.Shoot:
                UpdateShooting();
                break;
        }
        UpdateArrow();
        CheckBallDeath();
    }

    private void UpdateArrow()
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
            arrow.transform.position = Ball.transform.position + vec * .2f;
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
        if (charMode == CharMode.Shoot)
        {
            DrawShootGUI();
        }
    }

}
