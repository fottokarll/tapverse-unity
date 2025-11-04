using System.Collections.Generic;
using UnityEngine;

namespace Tapverse.Gameplay.Definitions
{
    [CreateAssetMenu(menuName = "Tapverse/Generator Catalog")]
    public class GeneratorCatalog : ScriptableObject
    {
        public List<GeneratorDefinition> Generators = new List<GeneratorDefinition>();
    }
}
