using FIMSpace.Graph;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FIMSpace.Generating.Planning.PlannerNodes.Logic
{

    public class PR_EqualIfSwitchExecution : PlannerRuleBase
    {
        public override string GetDisplayName(float maxWidth = 120) { return wasCreated ? "If => Execute A or B" : "If true\\false => Execute A or B"; }
        public override Color GetNodeColor() { return new Color(0.3f, 0.8f, 0.55f, 0.9f); }
        public override Vector2 NodeSize { get { return new Vector2(180, 92); } }
        public override bool DrawInputConnector { get { return true; } }
        public override bool DrawOutputConnector { get { return true; } }
        public override int OutputConnectorsCount { get { return 2; } }
        public override int HotOutputConnectionIndex { get { return 1; } }
        public override int AllowedOutputConnectionIndex { get { return outputId; } }

        public override string GetOutputHelperText(int outputId = 0)
        {
            if (outputId == 0) return "False/Null";
            return "True";
        }

        public override EPlannerNodeType NodeType { get { return EPlannerNodeType.Logic; } }

        [Port(EPortPinType.Input, EPortNameDisplay.Default, EPortValueDisplay.HideValue, 1)] public BoolPort FalseOrTrue;

        int outputId = 0;

        public override void Execute(PlanGenerationPrint print, PlannerResult newResult)
        {
            FalseOrTrue.TriggerReadPort(true);
            int targetId;
            if (FalseOrTrue.GetInputValue) targetId = 1; else targetId = 0;
            outputId = targetId;
        }

#if UNITY_EDITOR
        public override void Editor_OnAdditionalInspectorGUI()
        {
            EditorGUILayout.LabelField("Debugging:", EditorStyles.helpBox);
            GUILayout.Label("Input Value: " + FalseOrTrue.GetPortValueSafe);
            GUILayout.Label("Is null?: " + FGenerators.CheckIfIsNull(FalseOrTrue.GetPortValueSafe) );
        }
#endif

    }
}