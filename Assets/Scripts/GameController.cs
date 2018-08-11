using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public GameObject[] Balls {get; private set;}
    public GameObject[] Players { get; private set; }

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
        Balls = new GameObject[]{ };
        Players = new GameObject[] { };
    }

	public void OnDestroy()
	{
        if(instance == this) {
            instance = null;
        }
	}


    public void UpdateObjectLists()
    {
        Balls = GameObject.FindGameObjectsWithTag("Ball");
        Players = GameObject.FindGameObjectsWithTag("Player");
    }
}
