using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "ListaDeCacas")]
public class ListaDeCacas : ScriptableObject
{

    

    [System.Serializable]
    public class CacaData 
    {
        public int poopLevel;
        public float poopValue = 1f;
        public float baseCooldown = 3f;
        public AnimationCurve poopRateCurve;
    }

    public List<CacaData> listaDeCacas;
}



