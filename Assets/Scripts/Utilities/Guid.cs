using UnityEngine;

[ExecuteAlways]
public class Guid : MonoBehaviour
{
    public string ModuleGuid;

    private void Awake()
    {
        if (ModuleGuid == null)
            ModuleGuid = System.Guid.NewGuid().ToString();
    }

    
}
