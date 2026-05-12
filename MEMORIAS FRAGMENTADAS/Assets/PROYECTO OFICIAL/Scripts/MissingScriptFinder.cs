using UnityEngine;

public class MissingScriptFinder : MonoBehaviour
{
    void Start()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>(true);

        foreach (GameObject obj in allObjects)
        {
            Component[] components = obj.GetComponents<Component>();

            foreach (Component component in components)
            {
                if (component == null)
                {
                    Debug.LogWarning("MISSING SCRIPT EN: " + GetPath(obj.transform), obj);
                }
            }
        }
    }

    string GetPath(Transform t)
    {
        string path = t.name;

        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }

        return path;
    }
}