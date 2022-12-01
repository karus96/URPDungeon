#if UNITY_EDITOR
using UnityEditor;
using FIMSpace.FEditor;
#endif
using UnityEngine;

namespace FIMSpace.Generating.Rules.Modelling
{
    public class SR_TileDesigner : SpawnRuleBase, ISpawnProcedureType
    {
        public override string TitleName() { return "Tile Designer"; }
        public override string Tooltip() { return "Generating object to generate with tile designer"; }
        public EProcedureType Type { get { return EProcedureType.Event; } }
        //public EProcedureType Type { get { return EProcedureType.Coded; } }

        //public bool ReplaceSelectedSpawn = false;
        [HideInInspector] public TileDesign Design;
        private GameObject generatedDesign = null;



        public override void PreGenerateResetRule(FGenGraph<FieldCell, FGenPoint> grid, FieldSetup preset, FieldSpawner callFrom)
        {
            if (callFrom.TemporaryPrefabOverride != null)
            {
                return;
            }

            if (generatedDesign) { FGenerators.DestroyObject(generatedDesign); }

            Design.FullGenerateStack();
            generatedDesign = Design.GeneratePrefab();
            generatedDesign.transform.position = new Vector3(10000, -10000, 10000);
            generatedDesign.hideFlags = HideFlags.HideAndDontSave;

            callFrom.SetTemporaryPrefabToSpawn(generatedDesign);
        }


        #region Editor GUI

#if UNITY_EDITOR
        public override void NodeBody(SerializedObject so)
        {
            if (Design != null)
            {
                if (Design.DesignName == "New Tile")
                {
                    Design.DesignName = OwnerSpawner.Name;
                    EditorUtility.SetDirty(this);
                }
            }

            EditorGUILayout.HelpBox(" Replacing object to spawn with Tile Design", MessageType.Info);
            base.NodeBody(so);

            GUILayout.Space(6);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("  Open Tile Designer", FGUI_Resources.Tex_GearSetup), FGUI_Resources.ButtonStyle, GUILayout.Height(24)))
            {
                TileDesignerWindow.Init(Design, this);
            }

            if (Design.TileMeshes.Count == 1)
            {
                if (GUILayout.Button("Quick Edit", FGUI_Resources.ButtonStyle, GUILayout.Width(72), GUILayout.Height(24)))
                {
                    TileDesignerWindow.Init(Design, this, true);
                }
            }

            GUILayout.EndHorizontal();

            if (Design._LatestGen_Meshes > 0)
            {
                EditorGUILayout.LabelField("Target mesh tris: " + Design._LatestGen_Tris, EditorStyles.centeredGreyMiniLabel);
            }

            GUILayout.Space(6);
        }

        //public override void NodeFooter(SerializedObject so, FieldModification mod)
        //{
        //    if (ReplaceSelectedSpawn)
        //    {
        //        EditorGUILayout.HelpBox("All other spawners using same 'To Spawn' will generate Tile Design Object", MessageType.None);
        //    }

        //    base.NodeFooter(so, mod);
        //}

#endif

        #endregion

    }
}