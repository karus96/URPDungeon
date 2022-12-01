using FIMSpace.Graph;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FIMSpace.Generating.Planning.PlannerNodes.Math.Values
{

    public class PR_GetRandom : PlannerRuleBase
    {
        public override string GetDisplayName(float maxWidth = 120) { return "Get Random"; }
        public override string GetNodeTooltipDescription { get { return "Get random value, it can be number or vectors"; } }

        public override Color GetNodeColor() { return new Color(0.3f, 0.5f, 0.75f, 0.9f); }
        public override Vector2 NodeSize { get { return new Vector2(172, _EditorFoldout ? 102 : 82); } }
        public override bool DrawInputConnector { get { return false; } }
        public override bool DrawOutputConnector { get { return false; } }
        public override bool IsFoldable { get { return true; } }

        public override EPlannerNodeType NodeType { get { return EPlannerNodeType.Math; } }

        public enum EType
        {
            Number, Int, Vector3NoY, Vector3, Vector2
        }


        [HideInInspector] public Vector2 FromTo = new Vector2(0f, 2f);
        [HideInInspector] public EType RandomOut = EType.Number;
        [HideInInspector] [Port(EPortPinType.Output, true)] public FloatPort RandomF;
        [HideInInspector] [Port(EPortPinType.Output, true)] public PGGVector3Port RandomV3;
        //[HideInInspector][Port(EPortPinType.Output, EPortPinDisplay.JustPort)] public FloatPort Random;

        public override void Prepare(PlanGenerationPrint print)
        {
            base.Prepare(print);

            if (RandomOut == EType.Number)
            {
                RandomF.Value = FGenerators.GetRandom(FromTo.x, FromTo.y);
            }
            else if (RandomOut == EType.Int)
            {
                RandomF.Value = FGenerators.GetRandom(Mathf.RoundToInt(FromTo.x), Mathf.RoundToInt(FromTo.y));
            }
            else if (RandomOut == EType.Vector2)
            {
                RandomV3.Value.x = FGenerators.GetRandom(FromTo.x, FromTo.y);
                RandomV3.Value.y = FGenerators.GetRandom(FromTo.x, FromTo.y);
            }
            else if (RandomOut == EType.Vector3)
            {
                RandomV3.Value.x = FGenerators.GetRandom(FromTo.x, FromTo.y);
                RandomV3.Value.y = FGenerators.GetRandom(FromTo.x, FromTo.y);
                RandomV3.Value.z = FGenerators.GetRandom(FromTo.x, FromTo.y);
            }
            else if (RandomOut == EType.Vector3NoY)
            {
                RandomV3.Value.x = FGenerators.GetRandom(FromTo.x, FromTo.y);
                RandomV3.Value.y = 0;
                RandomV3.Value.z = FGenerators.GetRandom(FromTo.x, FromTo.y);
            }
        }

#if UNITY_EDITOR
        SerializedProperty sp = null;
        public override void Editor_OnNodeBodyGUI(ScriptableObject setup)
        {
            if (sp == null) sp = baseSerializedObject.FindProperty("FromTo");
            SerializedProperty s = sp.Copy();

            GUILayout.BeginHorizontal();

            if (RandomOut == EType.Number)
            {
                RandomV3.AllowDragWire = false;
                RandomF.AllowDragWire = true;
            }
            else
            {
                RandomF.AllowDragWire = false;
                RandomV3.AllowDragWire = true;
            }

            EditorGUIUtility.labelWidth = 4;
            Vector2 nVal = s.vector2Value;
            nVal.x = EditorGUILayout.FloatField(" ", s.vector2Value.x, GUILayout.Width(33));
            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.LabelField(" to ", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(16));
            EditorGUIUtility.labelWidth = 3;
            nVal.y = EditorGUILayout.FloatField(" ", s.vector2Value.y, GUILayout.Width(33));
            EditorGUIUtility.labelWidth = 0;

            s.vector2Value = nVal;

            SerializedProperty spp;
            if (RandomOut == EType.Number)
            {
                spp = baseSerializedObject.FindProperty("RandomF");
            }
            else
            {
                spp = baseSerializedObject.FindProperty("RandomV3");
            }

            GUILayout.Space(-27);
            EditorGUILayout.PropertyField(spp, GUIContent.none);
            GUILayout.EndHorizontal();
            //GUILayout.Space(-21);
            //EditorGUILayout.PropertyField(s, GUIContent.none);

            if (_EditorFoldout)
            {
                s.Next(false);
                EditorGUILayout.PropertyField(s, GUIContent.none);
            }
        }

        public override void Editor_OnAdditionalInspectorGUI()
        {
            EditorGUILayout.LabelField("Debugging:", EditorStyles.helpBox);
            if (RandomOut == EType.Number)
            {
                EditorGUILayout.LabelField("Out Value: " + RandomF.Value);
            }
            else if (RandomOut == EType.Vector3)
            {
                EditorGUILayout.LabelField("Out Value: " + RandomV3.Value);
            }
        }

#endif

    }
}