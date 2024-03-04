using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectIconsDictionary : MonoBehaviour
{
    public static EffectIconsDictionary Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [Header("Icons")]
    public GameObject HarvestIcon;
    public GameObject ActiveHarvestIcon;
    public GameObject RoyalBloodIcon;
    public GameObject RageIcon;
    public GameObject KnightlyCommandIcon;
    public GameObject NaturesBlessingIcon;

    // Utils
    public GameObject GetIconOfEffect(EffectType effectType)
    {
        switch (effectType)
        {
            case EffectType.Harvest:
                return Instantiate(HarvestIcon, Vector2.zero, Quaternion.identity);
            case EffectType.ActiveHarvest:
                return Instantiate(ActiveHarvestIcon, Vector2.zero, Quaternion.identity);
            case EffectType.RoyalBlood:
                return Instantiate(RoyalBloodIcon, Vector2.zero, Quaternion.identity);
            case EffectType.Rage:
                return Instantiate(RageIcon, Vector2.zero, Quaternion.identity);
            case EffectType.KnightlyCommand:
                return Instantiate(KnightlyCommandIcon, Vector2.zero, Quaternion.identity);
            case EffectType.NaturesBlessing:
                return Instantiate(NaturesBlessingIcon, Vector2.zero, Quaternion.identity);
            default:
                return null;
        }
    }
    public void DestroyIconOfEffect(GameObject Icon)
    {
        if (Icon != null)
        {
            Destroy(Icon);
        }
    }
}
