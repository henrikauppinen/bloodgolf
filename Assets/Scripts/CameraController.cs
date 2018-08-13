using UnityEngine;

public class CameraController : MonoBehaviour
{
    private new Camera camera;

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        UpdatePositionAndRotation();
    }

    void UpdatePositionAndRotation(bool snap = false)
    {
        Bounds bbox = GetActionBounds();
        float distance = Mathf.Clamp(bbox.size.magnitude, 10, 30);
        float height = Mathf.Min(distance, 40);
        float angle = Mathf.PI / -2;
        var pos = bbox.center;
        pos += new Vector3(Mathf.Cos(angle) * distance, height, Mathf.Sin(angle) * distance);
        var rot = Quaternion.LookRotation(bbox.center - camera.transform.position);

        if (snap)
        {
            camera.transform.position = pos;
            camera.transform.rotation = rot;
        }
        else
        {
            camera.transform.position = Vector3.MoveTowards(camera.transform.position, pos, 10 * Time.deltaTime);
            camera.transform.rotation = Quaternion.RotateTowards(camera.transform.rotation, rot, 70 * Time.deltaTime);
        }
    }

    void OnDrawGizmos()
    {
        var bbox = GetActionBounds();
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(bbox.center, bbox.size);
    }

    private Bounds GetActionBounds()
    {
        var bbox = new Bounds();
        var first = true;

        if (GameController.instance == null) return bbox;

        foreach (var player in GameController.instance.Players)
        {
            if (first)
            {
                bbox = new Bounds(player.transform.position, Vector3.zero);
                first = false;
            }
            else
            {
                bbox.Encapsulate(player.transform.position);
            }
        }
        /*
        float maxBallDistance = 150;
        var playerCenter = bbox.center;
        foreach (var ball in balls)
        {
            if ((ball.transform.position - playerCenter).magnitude < maxBallDistance)
            {
                bbox.Encapsulate(ball.transform.position);
            }
        }
        */

        return bbox;
    }
}
