using MelonLoader;
using UnityEngine;

namespace BetterBases
{
    [RegisterTypeInIl2Cpp]
    class MoveableObject : MonoBehaviour
    {
        public Vector3 m_OriginalPosition;
        public Vector3 m_CurrentPosition;

        public MoveableObject(System.IntPtr intPtr) : base(intPtr) { }
    }
}
