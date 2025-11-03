using System.Collections.Generic;
using UnityEngine;

namespace TapVerse.Gameplay.Definitions
{
    [CreateAssetMenu(menuName = "TapVerse/Generator Catalog")]
    public class GeneratorCatalog : ScriptableObject
    {
        public List<GeneratorDefinition> Generators = new List<GeneratorDefinition>();
    }
}
