using FIMSpace.Graph;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FIMSpace.Generating.Planning.PlannerNodes.Field.Access
{

    public class PR_GetFieldSelector : PlannerRuleBase
    {
        public override string GetDisplayName(float maxWidth = 120) { return wasCreated ? "Get Field" : "Get Field Planner from Selector"; }
        public override string GetNodeTooltipDescription { get { return "Getting choosed field index number or unfold to get field port"; } }
        public override EPlannerNodeType NodeType { get { return EPlannerNodeType.ReadData; } }
        public override Color GetNodeColor() { return new Color(1.0f, 0.75f, 0.25f, 0.9f); }
        public override Vector2 NodeSize { get { return new Vector2(165, _EditorFoldout ? 104 : 84); } }
        public override bool IsFoldable { get { return true; } }
        public override bool DrawInputConnector { get { return false; } }
        public override bool DrawOutputConnector { get { return false; } }

        [Port(EPortPinType.Output, true)] public IntPort PlannerID;
        [HideInInspector] [Port(EPortPinType.Output, EPortValueDisplay.HideValue)] public PGGPlannerPort Planner;

        public override void OnStartReadingNode()
        {
            Planner.UniquePlannerID = PlannerID.Value;
        }

#if UNITY_EDITOR
        SerializedProperty sp = null;
        public override void Editor_OnNodeBodyGUI(ScriptableObject setup)
        {
            base.Editor_OnNodeBodyGUI(setup);

            if (ParentPlanner != null)
            {
                GUILayout.Space(-20);
                UnityEditor.EditorGUI.BeginChangeCheck();
                PlannerID.Value = EditorGUILayout.IntPopup(PlannerID.Value, ParentPlanner.GetPlannersNameList(), ParentPlanner.GetPlannersIDList(), GUILayout.Width(NodeSize.x - 80));
                if (UnityEditor.EditorGUI.EndChangeCheck()) _editorForceChanged = true;
            }

            if (_EditorFoldout)
            {
                if (sp == null) sp = baseSerializedObject.FindProperty("Planner");
                EditorGUILayout.PropertyField(sp, true);
            }

        }
#endif

    }
}