using UnityEngine;

[CreateAssetMenu(menuName = "CacaData")]
public class CacaData : ScriptableObject
{
    public int poopLevel;
    public float poopValue = 1f;
    public float baseCooldown = 3f;
    public AnimationCurve poopRateCurve;
}