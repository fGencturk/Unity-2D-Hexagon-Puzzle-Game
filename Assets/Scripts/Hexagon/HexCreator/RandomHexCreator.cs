using UnityEngine;

namespace Hexagon.HexCreator
{
    public abstract class RandomHexCreator : MonoBehaviour
    {
        public abstract Hex CreateHexGameObject(Vector2Int indexes);
    }
}