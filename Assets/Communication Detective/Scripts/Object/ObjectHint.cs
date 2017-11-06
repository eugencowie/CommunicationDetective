using UnityEditor;
using UnityEngine;

public class ObjectHintData
{
    public string Name = "";
    public string Hint = "";
    public string Image = "";

    public ObjectHintData(string name, string hint, string image)
    {
        Name = name;
        Hint = hint;
        Image = image;
    }

    public ObjectHintData(ObjectHint hint)
        : this(hint.Name, hint.Hint, AssetDatabase.GetAssetPath(hint.Image).Replace("Assets/Resources/", "").Replace(".png", ""))
    {
    }
}

public class ObjectHint : MonoBehaviour
{
    public string Name = "";
    public string Hint = "";
    public Sprite Image = null;

    public ObjectHint() { }

    public ObjectHint(ObjectHintData data)
    {
        Name = data.Name;
        Hint = data.Hint;
        Image = Resources.Load<Sprite>(data.Image);
    }
}
