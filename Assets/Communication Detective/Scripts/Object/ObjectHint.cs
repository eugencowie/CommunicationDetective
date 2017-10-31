using UnityEngine;

public class ObjectHintData
{
    public string Name = "";
    public string Hint = "";

    public ObjectHintData(string name, string hint)
    {
        Name = name;
        Hint = hint;
    }
}

public class ObjectHint : MonoBehaviour
{
    public string Name = "";
    public string Hint = "";
}
