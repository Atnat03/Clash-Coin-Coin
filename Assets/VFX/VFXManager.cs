using System.Collections.Generic;
using UnityEngine;

public enum OneShotVFX
{
    TunnelSmoke, 
    MineExplosion
}

[System.Serializable]
public class VFXData
{
    public OneShotVFX type;
    public GameObject prefab;
    public float duration = 3f;
}

public class VFXManager : MonoBehaviour
{
    [SerializeField] private List<VFXData> vfxList = new();
    
    private Dictionary<OneShotVFX, VFXData> vfxDictionary;

    private void Awake()
    {
        vfxDictionary = new Dictionary<OneShotVFX, VFXData>();
        foreach (VFXData data in vfxList)
        {
            if (!vfxDictionary.ContainsKey(data.type))
            {
                vfxDictionary.Add(data.type, data);
            }
        }
    }

    // Que avec les positions
    public void PlayVFX(Vector3 position, OneShotVFX oneShotVFX)
    {
        if (vfxDictionary.TryGetValue(oneShotVFX, out VFXData data))
        {
            if (data.prefab != null)
            {
                GameObject vfx = Instantiate(data.prefab, position, Quaternion.identity);
                Destroy(vfx, data.duration);
            }
            else
            {
                Debug.LogWarning($"Prefab manquant pour {oneShotVFX}");
            }
        }
        else
        {
            Debug.LogWarning($"VFX non trouvé : {oneShotVFX}");
        }
    }
    
    // + Rotation
    public void PlayVFX(Vector3 position, OneShotVFX oneShotVFX, Quaternion rotation)
    {
        if (vfxDictionary.TryGetValue(oneShotVFX, out VFXData data))
        {
            if (data.prefab != null)
            {
                GameObject vfx = Instantiate(data.prefab, position, rotation);
                Destroy(vfx, data.duration);
            }
        }
    }
}