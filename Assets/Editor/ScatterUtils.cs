using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class ScatterUtils {

    public static void RandomizeYRotation(GameObject[] objects)
    {
        Undo.RecordObjects(objects, "Randomize Y Rotation");
        foreach (var obj in objects)
        {
            var angles = obj.transform.rotation.eulerAngles;
            angles.y = Random.Range(0, 360);
            obj.transform.rotation = Quaternion.Euler(angles);
        }
    }

    public static void RandomizeAllRotation(GameObject[] objects)
    {
        Undo.RecordObjects(objects, "Randomize All Rotation");
        foreach (var obj in objects)
        {
            obj.transform.rotation = Random.rotation;
        }
    }

    public static void PlaceObjectsOnGround(GameObject[] objects, float maxGroundDist = 100, float yOffset = 0)
    {
        Undo.RecordObjects(Selection.gameObjects, "Place On Ground");
        foreach (var obj in objects)
        {
            PlaceObjectOnGround(obj, maxGroundDist, yOffset);
        }
    }

    public static void PlaceObjectOnGround(GameObject obj, float maxGroundDist, float yOffset)
    {
        var hitInfo = new RaycastHit();
        var didHit = Physics.Raycast(new Ray(obj.transform.position, Vector3.down), out hitInfo, maxGroundDist);
        if (didHit)
        {
            var rend = obj.GetComponent<Renderer>();
            var objectHeight = rend.bounds.extents.y;
            var pos = obj.transform.position;
            pos.y = hitInfo.point.y + yOffset + objectHeight;
            obj.transform.position = pos;
        }
    }

    public static List<GameObject> ScatterOnObject(
        GameObject targetObject,
        GameObject templateObject,
        int count
    )
    {
        var rend = targetObject.GetComponent<Renderer>();
        var y = rend.bounds.max.y;
        var newObjects = new List<GameObject>();
        while(newObjects.Count < count)
        {
            var x = Random.Range(rend.bounds.min.x, rend.bounds.max.x);
            var z = Random.Range(rend.bounds.min.z, rend.bounds.max.z);
            var pos = new Vector3(x, y, z);
            var hitInfo = new RaycastHit();
            var didHit = Physics.Raycast(new Ray(pos, Vector3.down), out hitInfo);
            if (!didHit) continue;
            if (hitInfo.collider.gameObject != targetObject) continue;

            var newInstance = GameObject.Instantiate(templateObject, pos, Quaternion.identity);
            newInstance.name = System.String.Format("{0}-{1}", templateObject.name, newObjects.Count + 1);
            newObjects.Add(newInstance);
        }
        return newObjects;
    }
}
