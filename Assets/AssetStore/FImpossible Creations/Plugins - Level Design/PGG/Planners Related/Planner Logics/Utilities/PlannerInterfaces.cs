using FIMSpace.Generating.Planning.PlannerNodes;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.Generating.Planning
{
    public interface IPlanNodesContainer 
    {
        List<PlannerRuleBase> Procedures { get; }
        List<PlannerRuleBase> PostProcedures { get; }
        List<FieldVariable> Variables { get; }
        ScriptableObject ScrObj { get; }
        FieldPlanner.LocalVariables GraphLocalVariables { get; }
    }
}
