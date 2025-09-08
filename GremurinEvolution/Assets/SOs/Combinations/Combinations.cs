using System.Collections.Generic;
using System;
using UnityEngine;

public class Combinations : MonoBehaviour
{
  

    [Serializable]
    public struct ElementCombination
    {
        [Tooltip("Primer elemento de la combinación")]
        public ElementsEnum.ElementType elementoA;

        [Tooltip("Segundo elemento de la combinación")]
        public ElementsEnum.ElementType elementoB;

        [Tooltip("Nombre del resultado (p. ej. “Ébano”)")]
        public ElementsEnum.ElementType resultado;
    }


    [CreateAssetMenu(
        fileName = "ElementCombinationsDB",
        menuName = "Element Combinations",
        order = 1)]
    public class ElementCombinationDatabase : ScriptableObject
    {
        [Tooltip("Lista de todas las combinaciones posibles")]
        public List<ElementCombination> combos = new List<ElementCombination>();
    }

}
