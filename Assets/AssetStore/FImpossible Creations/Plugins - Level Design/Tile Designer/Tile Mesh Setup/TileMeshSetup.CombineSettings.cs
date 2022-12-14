using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.Generating
{
    public partial class TileMeshSetup
    {
        public int Copies = 1;
        [SerializeField] private List<TileMeshCombineInstance> _instances = new List<TileMeshCombineInstance>();
        public List<TileMeshCombineInstance> Instances
        {
            get
            {
                CheckInstances();
                return _instances;
            }
        }

        internal void CheckInstances()
        {
            if (Copies < 1) Copies = 1;
            if (_instances == null) _instances = new List<TileMeshCombineInstance>();
            if (_instances.Count == 0) _instances.Add(new TileMeshCombineInstance());
        }

        [System.Serializable]
        public class TileMeshCombineInstance
        {
            public bool Enabled = true;

            public enum EMeshMode { Default, JustCollider, Remove }
            public EMeshMode MeshMode = EMeshMode.Default;

            public Vector3 Position = Vector3.zero;
            public Vector3 Rotation = Vector3.zero;
            public Vector3 Scale = Vector3.one;
            public Material OverrideMaterial = null;
            public bool UseInCollider = true;
            [NonSerialized] public TileMeshSetup _BakeParent = null;

            public bool MeshModeApplyToAll = true;
            public List<int> MeshModeApplyMasks = new List<int>();

            internal Matrix4x4 GenerateMatrix()
            {
                return Matrix4x4.TRS(Position, Quaternion.Euler(Rotation), Scale);
            }

            internal TileMeshCombineInstance Copy()
            {
                return (TileMeshCombineInstance)MemberwiseClone();
            }
        }

    }
}