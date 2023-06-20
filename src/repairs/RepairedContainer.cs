using System.Collections.Generic;

using UnityEngine;

namespace BetterBases
{
    public struct RepairedContainer
    {
        public string guid;
        public string path;
        public string scene;
        public Vector3 position;

        public RepairedContainer(string scene, string path, Vector3 position, string guid)
        {
            this.scene = scene;
            this.path = path;
            this.position = position;
            this.guid = guid;
        }
    }

    public class RepairedContainers : Il2CppSystem.Object
    {
        public List<RepairedContainer> containers = new List<RepairedContainer>();

        public void AddRepairedContainer(RepairedContainer repairedContainer)
        {
            this.containers.RemoveAll(value => Vector3.Distance(value.position, repairedContainer.position) < 0.01f);
            this.containers.Add(repairedContainer);
        }

        public void Clear()
        {
            this.containers.Clear();
        }
    }
}