using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectData
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable]
public class MoveData
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 Endposition;
    public int Speed;
}

[System.Serializable]
public class SceneData
{
    public List<ObjectData> objects;
    public List<MoveData> moves;
}
