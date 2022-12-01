using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.Graph
{
    public abstract partial class FGraphDrawerBase
    {

        /// <summary> Used for auto-nodes to choose create menu. Can be overrided for custom path. </summary>
        public virtual string GetNodesNamespace { get { return GetBaseNodeType.Namespace; } }
        /// <summary> Base type of the nodes used in this graph. It's namespace will be used for auto-node menu </summary>
        public abstract Type GetBaseNodeType { get; }


        protected List<Type> types = null;
        public List<Type> GetAndSetupNodeTypesByNamespace(string initialNamespace = "YourNamespace.Graph.Nodes")
        {
            if (types == null) types = GetNodeTypesByNamespace(initialNamespace);
            return types;
        }

        /// <summary>
        /// GetAndSetupNodeTypesByNamespace for some optimization
        /// </summary>
        public List<Type> GetNodeTypesByNamespace(string initialNamespace = "YourNamespace.Graph.Nodes")
        {
            List<Type> types = new List<Type>();

            foreach (Type t in GetDerivedTypes(typeof(FGraph_NodeBase)))
            {
                if (t == typeof(FGraph_NodeBase)) continue; // Ignore base class
                if (t.Namespace.StartsWith(initialNamespace) == false) continue; // Ignore other namespace nodes
                string path = GetPathNameForGenericMenu(t, GetNodesNamespace);
                if (string.IsNullOrEmpty(path)) continue;
                types.Add(t);
            }

            return types;
        }

        public virtual void FillGenericMenuWithAutomaticDetectedNodesByNamespace(GenericMenu menu)
        {
            var nodes = GetNodesByNamespace();

            for (int i = 0; i < nodes.Count; i++)
            {
                var nNode = nodes[i].node;
                menu.AddItem(new GUIContent(nodes[i].name), false, () =>
                {
                    AddNewNodeToPreset(nNode);
                });
            }
        }

        private List<NodeRef> _assemblyNodes = new List<NodeRef>();
        public virtual List<NodeRef> GetNodesByNamespace()
        {
            for (int a = 0; a < _assemblyNodes.Count; a++) { if (_assemblyNodes[a].node == null) { _assemblyNodes.Clear(); break; } }
            if (_assemblyNodes.Count > 0) return _assemblyNodes;

            List<Type> types = GetAndSetupNodeTypesByNamespace(GetNodesNamespace);

            for (int i = 0; i < types.Count; i++)
            {
                Type type = types[i];

                if (type == typeof(FGraph_NodeBase)) continue;
                string path = GetPathNameForGenericMenu(type, GetNodesNamespace);
                if (string.IsNullOrEmpty(path)) continue;

                string name = path;
                FGraph_NodeBase node = ScriptableObject.CreateInstance(type) as FGraph_NodeBase;

                if (string.IsNullOrEmpty(node.EditorCustomMenuPath()) == false)
                {
                    name = node.EditorCustomMenuPath()+"/"+node.GetDisplayName();
                }
                else
                {
                    if (node != null) name = path.Replace(node.GetType().Name, "") + node.GetDisplayName();
                    name = System.Text.RegularExpressions.Regex.Replace(name, "(\\B[A-Z])", " $1");
                }

                _assemblyNodes.Add(new NodeRef() { name = name, node = node });
            }

            return _assemblyNodes;
        }


        #region Reflection helper methods

        public struct NodeRef
        {
            public string name;
            public FGraph_NodeBase node;
        }

        public static List<Type> GetDerivedTypes(Type baseType)
        {
            List<Type> types = new List<System.Type>();
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                try { types.AddRange(assembly.GetTypes().Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t)).ToArray()); }
                catch (ReflectionTypeLoadException) { }
            }

            return types;
        }

        public virtual string GetPathNameForGenericMenu(Type type, string initialNamespace)
        {
            string name = type.ToString();
            name = name.Replace(initialNamespace + ".", "");
            return name.Replace('.', '/');
        }

        #endregion


    }
}