using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Il2Cpp;

namespace BetterBases
{
    public class ModBreakDownSaveData : BreakDownSaveData
    {
        public Vector3 m_Rotation;
        public Vector3 m_CurrentPosition;
    }

    public class ModBreakDownSaveProxy
    {
        public float[] m_Position;
        public float[] m_CurrentPosition;
        public float[] m_Rotation;
        public bool m_HasBeenBrokenDown;
        public string m_Guid;

        public static implicit operator BreakDownSaveData(ModBreakDownSaveProxy saveProxy)
        {
            BreakDownSaveData saveData = new BreakDownSaveData();
            saveData.m_Guid = saveProxy.m_Guid;
            saveData.m_HasBeenBrokenDown = saveProxy.m_HasBeenBrokenDown;
            saveData.m_Position = new Vector3(saveProxy.m_Position[0], saveProxy.m_Position[1], saveProxy.m_Position[2]);

            return saveData;
        }
        
    }
}
