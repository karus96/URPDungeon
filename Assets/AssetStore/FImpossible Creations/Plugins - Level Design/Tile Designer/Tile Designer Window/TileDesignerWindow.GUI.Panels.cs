#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using FIMSpace.FEditor;
using static FIMSpace.Generating.TileMeshSetup;
using System.Collections.Generic;

namespace FIMSpace.Generating
{
    public partial class TileDesignerWindow
    {
        List<CurvePoint> _editorPoints = new List<CurvePoint>();
        List<CurvePoint> _editorPoints2 = new List<CurvePoint>();

        List<MeshShapePoint> previewShape { get { return s.previewShape; } }
        List<MeshShapePoint> previewShape2 { get { return s.previewShape2; } }

        int _selectedTileMesh = 0;


        #region Setup GUI

        int _generated_sceneObj = 0;
        int _generated_prefabObj = 0;
        int _generated_meshFile = 0;
        TileDesign _generated_sceneObjParentRef = null;
        GameObject _generated_sceneObjRef = null;

        void DrawTileLeftArrowButton()
        {
            if (EditedDesign.TileMeshes.Count > 1)
                if (GUILayout.Button(FGUI_Resources.Tex_ArrowLeft, GUILayout.Width(22))) { _selectedTileMesh -= 1; RefreshSelectedTileMesh(true); shapeChanged = true; shapeEndChanging = true; }
        }

        void DrawTileRightArrowButton()
        {
            if (EditedDesign.TileMeshes.Count > 1)
                if (GUILayout.Button(FGUI_Resources.Tex_ArrowRight, GUILayout.Width(22))) { _selectedTileMesh += 1; RefreshSelectedTileMesh(true); shapeChanged = true; shapeEndChanging = true; }
        }

        void RefreshSelectedTileMesh(bool setEditedDesign)
        {
            if (_selectedTileMesh >= EditedDesign.TileMeshes.Count) _selectedTileMesh = 0;
            if (_selectedTileMesh < 0) _selectedTileMesh = EditedDesign.TileMeshes.Count - 1;
            if (setEditedDesign) EditedTileSetup = EditedDesign.TileMeshes[_selectedTileMesh];
        }

        void DesignGenericMenu()
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Export design setup to the project file"), false, () =>
            {
                TileDesignPreset gen = (TileDesignPreset)FGeneratingUtilities.GenerateScriptable(CreateInstance<TileDesignPreset>(), "Tile Design");

                if (gen)
                {
                    gen.Designs.Add(new TileDesign());
                    gen.Designs[0].PasteEverythingFrom(EditedDesign);
                    Init(gen.Designs[0], gen);
                }
            });

            menu.AddItem(new GUIContent("Copy all design setup"), false, () => { TileDesign._CopyFrom = EditedDesign; });
            if (TileDesign._CopyFrom != null)
                menu.AddItem(new GUIContent("Paste - replace all setup with " + TileDesign._CopyFrom.DesignName + " parameters"), false, () => { EditedDesign.PasteEverythingFrom(TileDesign._CopyFrom); });

            menu.ShowAsContext();
        }

        void GUISetup()
        {
            Color preC = GUI.color;

            if (editingTempDesign || ToDirty == null)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.HelpBox("Editing design which is not saved in the project, save to new file if you want to keep it!", MessageType.None);

                if (GUILayout.Button("Save", GUILayout.Width(48)))
                {
                    TileDesignPreset gen = (TileDesignPreset)FGeneratingUtilities.GenerateScriptable(CreateInstance<TileDesignPreset>(), "Tile Design");

                    if (gen)
                    {
                        gen.Designs.Add(new TileDesign());
                        gen.Designs[0].PasteEverythingFrom(EditedDesign);
                        Init(gen.Designs[0], gen);
                    }
                }

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(4);
            }

            if (ToDirty != null)
                if (ToDirty.GetType() == typeof(TileDesignPreset))
                {
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField("", ToDirty, typeof(TileDesignPreset), false);
                    GUI.enabled = true;
                }

            if (GUILayout.Button("Tile Designer", FGUI_Resources.HeaderStyle)) { DesignGenericMenu(); }
            if (GUILayout.Button("Generate single wall/column/architecture tile for spawning", EditorStyles.centeredGreyMiniLabel)) { DesignGenericMenu(); }
            //GUILayout.Label("Generate single wall/column/architecture tile for spawning", EditorStyles.centeredGreyMiniLabel);

            var d = EditedDesign;

            GUILayout.Space(4);
            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxBlankStyle);

            d.DesignName = EditorGUILayout.TextField("Whole Design Name:", d.DesignName);
            d.DefaultMaterial = (Material)EditorGUILayout.ObjectField("Default Material:", d.DefaultMaterial, typeof(Material), false);

            EditorGUILayout.EndVertical();


            FGUI_Inspector.DrawUILine(0.3f, 0.5f);

            GUILayout.Space(4);


            #region Meshes Foldout

            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);

            if (d.TileMeshes.Count == 0)
            {
                d.TileMeshes.Add(new TileMeshSetup());
            }


            bool wasFolded = _foldout_meshSetup;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(FGUI_Resources.GetFoldSimbolTex(_foldout_meshSetup, true), EditorStyles.label, GUILayout.Height(19), GUILayout.Width(20))) _foldout_meshSetup = !_foldout_meshSetup;
            if (GUILayout.Button(new GUIContent("  Tile Editor Meshes (" + d.TileMeshes.Count + ")", EditorGUIUtility.IconContent("Mesh Icon").image), EditorStyles.label, GUILayout.Height(19))) _foldout_meshSetup = !_foldout_meshSetup;

            if (_foldout_meshSetup)
            {

                EditorGUILayout.BeginHorizontal(GUILayout.Height(19));
                //if (GUILayout.Button(FGUI_Resources.Tex_ArrowLeft, GUILayout.Width(22))) { _selectedTileMesh -= 1; }
                DrawTileLeftArrowButton();

                GUILayout.Label((_selectedTileMesh + 1) + "/" + d.TileMeshes.Count, EditorStyles.centeredGreyMiniLabel, GUILayout.Height(19));

                DrawTileRightArrowButton();
                //if (GUILayout.Button(FGUI_Resources.Tex_ArrowRight, GUILayout.Width(22))) { _selectedTileMesh += 1; }
                EditorGUILayout.EndHorizontal();



                GUILayout.Space(12);
                if (GUILayout.Button("+", FGUI_Resources.ButtonStyle, GUILayout.Height(18), GUILayout.Width(22))) { d.TileMeshes.Add(new TileMeshSetup("Tile Mesh " + (d.TileMeshes.Count + 1))); _selectedTileMesh += 1; }
                GUILayout.Space(4);
            }
            else
            {
                if (GUILayout.Button(new GUIContent(" Go", FGUI_Resources.Tex_GearSetup), EditorStyles.label, GUILayout.Width(50), GUILayout.Height(19)))
                {
                    EditedTileSetup = d.TileMeshes[_selectedTileMesh];
                    RefreshDefaultCurve(EditedTileSetup.GenTechnique);
                    GenerateTileMeshSetupMesh();

                    EditorMode = EMode.MeshEditor;
                    shapeChanged = true;
                }
            }

            EditorGUILayout.EndHorizontal();

            if (_foldout_meshSetup)
            {
                RefreshSelectedTileMesh(false);
                var sel = d.TileMeshes[_selectedTileMesh];
                int toRemove = -1;

                GUILayout.Space(4);

                EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxBlankStyle);

                GUILayout.BeginHorizontal();
                sel.Name = EditorGUILayout.TextField("Name:", sel.Name);

                GUI.enabled = d.TileMeshes.Count > 1;
                GUI.backgroundColor = new Color(1f, 0.6f, 0.6f, 1f);
                if (GUILayout.Button(FGUI_Resources.GUIC_Remove, FGUI_Resources.ButtonStyle, GUILayout.Width(25), GUILayout.Height(19))) { toRemove = _selectedTileMesh; }
                GUI.backgroundColor = Color.white;
                GUI.enabled = true;

                GUILayout.EndHorizontal();

                GUILayout.Space(3);

                if (EditedTileSetup != null)
                if (EditedTileSetup.GenTechnique == EMeshGenerator.CustomMesh)
                {
                    EditedTileSetup.CustomMesh = (Mesh)EditorGUILayout.ObjectField("Custom Mesh:", EditedTileSetup.CustomMesh, typeof(Mesh), false);
                }

                GUILayout.BeginHorizontal();
                sel.Material = (Material)EditorGUILayout.ObjectField("Override Material:", sel.Material, typeof(Material), false);
                if (GUILayout.Button(FGUI_Resources.GUIC_Info, EditorStyles.label, GUILayout.Width(17))) { EditorUtility.DisplayDialog("Material Info", "If you override materials, then there will be generated multiple meshes instead of single mesh which is more optimal!\nSo it's better to leave this field empty if it's not neccessary to use multiple materials.", "Ok"); }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Space(6);
                GUI.backgroundColor = new Color(0.7f, 1f, 0.7f, 1f);

                if (GUILayout.Button(new GUIContent("  Switch to Tile Editor", FGUI_Resources.Tex_GearSetup), FGUI_Resources.ButtonStyle, GUILayout.Height(24)))
                {
                    EditedTileSetup = sel;
                    EditorMode = EMode.MeshEditor;
                    shapeChanged = true;
                }

                GUI.backgroundColor = Color.white;
                GUILayout.Space(6);
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                EditorGUILayout.EndVertical();

                if (toRemove != -1)
                {
                    d.TileMeshes.RemoveAt(toRemove);
                    _selectedTileMesh -= 1;
                }

                if (wasFolded != _foldout_meshSetup)
                {
                    if (EditedTileSetup == null) EditedTileSetup = sel;

                    if (EditedTileSetup != null)
                    {
                        if (EditedTileSetup.LatestGeneratedMesh == null)
                        {
                            RefreshDefaultCurve(EditedTileSetup.GenTechnique);
                            GenerateTileMeshSetupMesh();
                        }
                        else
                        {
                            TileMeshUpdatePreview();
                        }
                    }

                    shapeChanged = true;
                }
            }

            EditorGUILayout.EndVertical();

            #endregion


            GUILayout.Space(3);
            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxBlankStyle);


            #region Object Settings Foldout

            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(FGUI_Resources.GetFoldSimbolTex(_foldout_gameObjectSetup, true), EditorStyles.label, GUILayout.Height(19), GUILayout.Width(20))) _foldout_gameObjectSetup = !_foldout_gameObjectSetup;
            if (GUILayout.Button(new GUIContent(" Target Parameters", EditorGUIUtility.IconContent("Prefab Icon").image), EditorStyles.label, GUILayout.Height(19))) _foldout_gameObjectSetup = !_foldout_gameObjectSetup;

            if (_foldout_gameObjectSetup)
            {
                #region Backup Code

                //EditorGUILayout.BeginHorizontal(GUILayout.Height(19));
                //if (GUILayout.Button(FGUI_Resources.Tex_ArrowLeft, GUILayout.Width(22))) { }

                //GUILayout.Label("Base", EditorStyles.centeredGreyMiniLabel, GUILayout.Height(19));

                //if (GUILayout.Button(FGUI_Resources.Tex_ArrowRight, GUILayout.Width(22))) { }
                //EditorGUILayout.EndHorizontal();

                //GUILayout.Space(12);
                //if (GUILayout.Button("+", FGUI_Resources.ButtonStyle, GUILayout.Height(18), GUILayout.Width(22))) { }
                //GUILayout.Space(4);
                #endregion

                if (TileDesign._copyGameObjectSetFrom != null && TileDesign._copyGameObjectSetFrom != EditedDesign)
                {
                    if (DrawPasteButton()) { TileDesign.PasteGameObjectParameters(TileDesign._copyGameObjectSetFrom, EditedDesign); TileDesign._copyGameObjectSetFrom = null; }
                }

                if (DrawCopyButton()) { EditedDesign.CopyGameObjectParameters(); }
            }


            EditorGUILayout.EndHorizontal();

            if (_foldout_gameObjectSetup)
            {
                GUILayout.Space(2);

                EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxBlankStyle);
                EditorGUIUtility.labelWidth = 102;

                d.Static = EditorGUILayout.Toggle("Static:", d.Static);
                Rect paramRect = GUILayoutUtility.GetLastRect();
                paramRect.position += new Vector2(0, EditorGUIUtility.singleLineHeight + 2);
                d.Tag = EditorGUI.TagField(paramRect, "Tag:", d.Tag);
                paramRect.position += new Vector2(0, EditorGUIUtility.singleLineHeight + 2);
                d.Layer = EditorGUI.LayerField(paramRect, "Layer:", d.Layer);
                GUILayout.Space((EditorGUIUtility.singleLineHeight + 2) * 2);

                GUILayout.Space(8);


                EditorGUILayout.LabelField(new GUIContent("  Attach Components:", EditorGUIUtility.IconContent("cs Script Icon").image));

                #region Attach components menu



                EditorGUI.BeginChangeCheck();

                int attachRemove = -1;
                for (int i = 0; i < d._editor_ToAttach.Count; i++)
                {
                    if (GUI_DisplayAttachable(d._editor_ToAttach, i)) attachRemove = i;
                }

                if (EditorGUI.EndChangeCheck()) { d.Editor_SyncToAttach(); _SetDirty(); }

                if (attachRemove >= 0)
                {
                    d._editor_ToAttach.RemoveAt(attachRemove);
                    d.Editor_SyncToAttach();
                }

                GUILayout.Space(4);
                GUILayout.BeginHorizontal();
                GUILayout.Space(18);
                if (GUILayout.Button(" + Add MonoBehaviour to Attach +", FGUI_Resources.ButtonStyle, GUILayout.Height(16)))
                {
                    d._editor_ToAttach.Add(null);
                    d.Editor_SyncToAttach();
                }
                GUILayout.Space(18);
                GUILayout.EndHorizontal();
                GUILayout.Space(4);

                #endregion


                GUILayout.Space(8);
                EditorGUILayout.LabelField(new GUIContent("  Send Messages:", EditorGUIUtility.IconContent("EventSystem Icon").image));

                #region Send Messages Menu

                int messageRemove = -1;
                for (int i = 0; i < d.SendMessages.Count; i++)
                {
                    if (GUI_DisplaySendMessageHelper(d.SendMessages, i)) messageRemove = i;
                }

                if (messageRemove >= 0)
                {
                    d.SendMessages.RemoveAt(messageRemove);
                }

                GUILayout.Space(4);
                GUILayout.BeginHorizontal();
                GUILayout.Space(18);
                if (GUILayout.Button(" + Add Message To Send +", FGUI_Resources.ButtonStyle, GUILayout.Height(16)))
                {
                    d.SendMessages.Add(new TileDesign.SendMessageHelper());
                }
                GUILayout.Space(18);
                GUILayout.EndHorizontal();
                GUILayout.Space(4);

                #endregion


                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();

            #endregion


            GUILayout.Space(10);


            #region Collider Settings Foldout

            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(FGUI_Resources.GetFoldSimbolTex(_foldout_colliderSetup, true), EditorStyles.label, GUILayout.Height(19), GUILayout.Width(20))) _foldout_colliderSetup = !_foldout_colliderSetup;
            if (GUILayout.Button(new GUIContent(" Collider Parameters", EditorGUIUtility.IconContent("SphereCollider Icon").image), EditorStyles.label, GUILayout.Height(19))) _foldout_colliderSetup = !_foldout_colliderSetup;

            if (_foldout_colliderSetup)
            {
                #region Backup Code

                //EditorGUILayout.BeginHorizontal(GUILayout.Height(19));
                //if (GUILayout.Button(FGUI_Resources.Tex_ArrowLeft, GUILayout.Width(22))) { }

                //GUILayout.Label("Base", EditorStyles.centeredGreyMiniLabel, GUILayout.Height(19));

                //if (GUILayout.Button(FGUI_Resources.Tex_ArrowRight, GUILayout.Width(22))) { }
                //EditorGUILayout.EndHorizontal();

                //GUILayout.Space(12);
                //if (GUILayout.Button("+", FGUI_Resources.ButtonStyle, GUILayout.Height(18), GUILayout.Width(22))) { }
                //GUILayout.Space(4);
                #endregion

                if (TileDesign._copyColliderSetFrom != null && TileDesign._copyColliderSetFrom != EditedDesign)
                {
                    if (DrawPasteButton()) { TileDesign.PasteColliderParameters(TileDesign._copyColliderSetFrom, EditedDesign); TileDesign._copyGameObjectSetFrom = null; }
                }

                if (DrawCopyButton()) { EditedDesign.CopyColliderParameters(); }
            }

            EditorGUILayout.EndHorizontal();

            if (_foldout_colliderSetup)
            {
                GUILayout.Space(2);

                EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxBlankStyle);

                d.AddRigidbody = EditorGUILayout.Toggle("Add Rigidbody", d.AddRigidbody);
                if (d.AddRigidbody)
                {
                    EditorGUI.indentLevel++;
                    d.IsKinematic = EditorGUILayout.Toggle("Kinematic", d.IsKinematic);
                    d.RigidbodyMass = EditorGUILayout.FloatField("Rigidbody Mass", d.RigidbodyMass);
                    EditorGUI.indentLevel--;
                }

                GUILayout.Space(8);

                d.ColliderMode = (TileDesign.EColliderMode)EditorGUILayout.EnumPopup("Collision Type:", d.ColliderMode);

                if (d.ColliderMode != TileDesign.EColliderMode.None)
                {
                    d.CollidersMaterial = (PhysicMaterial)EditorGUILayout.ObjectField("Colliders Material", d.CollidersMaterial, typeof(PhysicMaterial), false);
                    GUILayout.Space(4);

                    if (d.ColliderMode == TileDesign.EColliderMode.BoundingBox || d.ColliderMode == TileDesign.EColliderMode.MultipleBoundingBoxes || d.ColliderMode == TileDesign.EColliderMode.SphereCollider)
                    {
                        d.ScaleColliders = EditorGUILayout.Slider("Scale Colliders:", d.ScaleColliders, 0.5f, 2f);
                    }
                    else if (d.ColliderMode != TileDesign.EColliderMode.None)
                    {
                        d.ConvexCollider = EditorGUILayout.Toggle("Convex: ", d.ConvexCollider);
                    }
                }

                GUILayout.Space(4);

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();

            #endregion


            GUILayout.Space(10);


            #region Finalization Settings Foldout

            wasFolded = _foldout_finalizeSetup;

            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(FGUI_Resources.GetFoldSimbolTex(_foldout_finalizeSetup, true), EditorStyles.label, GUILayout.Height(19), GUILayout.Width(20))) _foldout_finalizeSetup = !_foldout_finalizeSetup;
            if (GUILayout.Button(new GUIContent(" Finalize Tile", FGUI_Resources.Tex_Tweaks), EditorStyles.label, GUILayout.Height(19))) _foldout_finalizeSetup = !_foldout_finalizeSetup;

            if (!_foldout_finalizeSetup)
            {
                if (GUILayout.Button(new GUIContent(" Go", FGUI_Resources.Tex_Extension), EditorStyles.label, GUILayout.Width(50), GUILayout.Height(19))) EditorMode = EMode.Combine;
            }

            EditorGUILayout.EndHorizontal();

            if (_foldout_finalizeSetup)
            {
                GUILayout.Space(2);

                EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxBlankStyle);

                GUILayout.Space(2);

                EditorGUILayout.HelpBox("In the finalize stage, you can create final model out of multiple meshes prepared in Tile Editor", MessageType.Info);

                GUILayout.Space(4);

                if (GUILayout.Button(new GUIContent("  Switch to Combiner", FGUI_Resources.Tex_Extension), FGUI_Resources.ButtonStyle, GUILayout.Height(22))) { EditorMode = EMode.Combine; }

                GUILayout.Space(8);

                EditorGUILayout.EndVertical();

                if (_foldout_finalizeSetup != wasFolded)
                {
                    if (EditedDesign.IsSomethingGenerated == false || _comb_autoRefresh)
                    {
                        GenerateCombinedMesh();
                    }
                }
            }

            EditorGUILayout.EndVertical();

            #endregion


            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            //if (GUILayout.Button(new GUIContent("  Re-Generate Meshes", FGUI_Resources.Tex_Expose), GUILayout.Height(30)))
            //{
            //    d.FullGenerateStack();
            //}

            if (GUILayout.Button(new GUIContent("  Generate Prefab", EditorGUIUtility.IconContent("Prefab Icon").image), GUILayout.Height(30), GUILayout.Width(160)))
            {
                if (_generated_prefabObj < 1)
                    EditorUtility.DisplayDialog("Generating Prefab", "Algorithm will generate prefab and place it inside provided project directory, it will also generate unity .mesh file as sub-asset in the prefab file. (mesh needs to be saved in file in order to appear in other game scenes than current one)", "Ok");

                #region Generating Prefab

                string path = UnityEditor.EditorUtility.SaveFilePanelInProject("Generate Prefab File", d.DesignName, "prefab", "Enter name of file");

                if (!string.IsNullOrEmpty(path))
                {

                    GameObject pf = d.GeneratePrefab();

                    bool success;
                    GameObject newPf = PrefabUtility.SaveAsPrefabAsset(pf, path, out success);

                    if (success)
                    {
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        for (int i = 0; i < d.LatestGeneratedMeshes.Count; i++)
                        {
                            if (d.LatestGeneratedMeshes[i] == null) continue;
                            AssetDatabase.AddObjectToAsset(d.LatestGeneratedMeshes[i], newPf);
                        }

                        if (d._UsedCombinedCollisionMesh != null)
                        {
                            AssetDatabase.AddObjectToAsset(d._UsedCombinedCollisionMesh, newPf);
                        }

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        var subAssets = AssetDatabase.LoadAllAssetsAtPath(path);

                        List<MeshFilter> filters = FTransformMethods.FindComponentsInAllChildren<MeshFilter>(newPf.transform, true);

                        for (int f = 0; f < filters.Count; f++)
                        {
                            MeshFilter filt = filters[f];

                            for (int i = 0; i < subAssets.Length; i++)
                            {
                                Mesh m = subAssets[i] as Mesh;

                                if (m)
                                {
                                    if (m.name == d.LatestGeneratedMeshes[f].name)
                                        if (m.vertexCount == d.LatestGeneratedMeshes[f].vertexCount)
                                        {
                                            filt.sharedMesh = m;
                                            break;
                                        }
                                }

                            }
                        }



                        if (d._UsedCombinedCollisionMesh != null)
                        {
                            MeshCollider meshColl = newPf.GetComponent<MeshCollider>();

                            for (int i = 0; i < subAssets.Length; i++)
                            {
                                Mesh msh = subAssets[i] as Mesh;

                                if (msh)
                                {
                                    if (msh.name == d._UsedCombinedCollisionMesh.name)
                                        if (msh.vertexCount == d._UsedCombinedCollisionMesh.vertexCount)
                                        {
                                            meshColl.sharedMesh = msh;
                                            break;
                                        }
                                }
                            }
                        }
                        else
                        {

                            List<MeshCollider> meshColls = FTransformMethods.FindComponentsInAllChildren<MeshCollider>(newPf.transform, true);
                            for (int m = 0; m < meshColls.Count; m++)
                            {
                                MeshCollider mshCols = meshColls[m];

                                for (int i = 0; i < subAssets.Length; i++)
                                {
                                    Mesh msh = subAssets[i] as Mesh;

                                    if (msh)
                                    {
                                        if (msh.name == d.LatestGeneratedMeshes[m].name)
                                            if (msh.vertexCount == d.LatestGeneratedMeshes[m].vertexCount)
                                            {
                                                mshCols.sharedMesh = msh;
                                                break;
                                            }
                                    }

                                }

                            }

                        }



                        EditorUtility.SetDirty(newPf);
                        FGenerators.DestroyObject(pf);
                        EditorGUIUtility.PingObject(newPf);
                    }
                    else
                    {
                        FGenerators.DestroyObject(pf);
                        UnityEngine.Debug.LogError("Something went wrong when generating prefab!");
                    }

                }

                _generated_prefabObj++;

                #endregion

            }

            if (GUILayout.Button(new GUIContent(" Scene Object", EditorGUIUtility.IconContent("SceneAsset Icon").image), GUILayout.Height(30), GUILayout.Width(128)))
            {
                if (_generated_sceneObj < 3)
                    EditorUtility.DisplayDialog("Generating Scene Object", "Algorithm will create tile designer object on the scene, the meshes will be stored inside scene file, so creating prefab will lost meshes references!", "Ok, I will not create prefab from this scene object");

                GenerateSceneObject();

                _generated_sceneObj += 1;
            }

            if (GUILayout.Button(new GUIContent(" Export", EditorGUIUtility.IconContent("Mesh Icon").image, "Export unity mesh file"), GUILayout.Height(30), GUILayout.Width(82)))
            {
                if (_generated_meshFile < 1)
                    EditorUtility.DisplayDialog("Generating Mesh File", "With mesh file you should be able to do further changes to the model with other plugins or use other plugins to export it as .fbx file", "Ok");

                #region Generating Mesh file

                string path = UnityEditor.EditorUtility.SaveFilePanelInProject("Generate Mesh File", d.DesignName, "asset", "Enter name of file");

                if (!string.IsNullOrEmpty(path))
                {
                    d.FullGenerateStack();

                    for (int m = 0; m < d.LatestGeneratedMeshes.Count; m++)
                    {
                        if (m == 0)
                        {
                            AssetDatabase.CreateAsset(d.LatestGeneratedMeshes[0], path);
                        }
                        else
                        {
                            AssetDatabase.CreateAsset(d.LatestGeneratedMeshes[m], path.Replace(".asset", (m + 1).ToString() + ".asset"));
                        }
                    }

                    AssetDatabase.SaveAssets();
                    var toPing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                    if (toPing) EditorGUIUtility.PingObject(toPing);
                }


                _generated_meshFile++;

                #endregion
            }

            EditorGUILayout.LabelField("If you recompile some scripts,close\ntile designer window and open it again", EditorStyles.centeredGreyMiniLabel, GUILayout.Height(28));

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(4);

            GUI.enabled = false;

            if (d._LatestGen_Meshes == 0)
            {
                EditorGUILayout.LabelField("Nothing Generated Yet");
            }
            else
            {
                EditorGUILayout.LabelField("Separated Meshes: " + d._LatestGen_Meshes);
                EditorGUILayout.LabelField("Vertices: " + d._LatestGen_Vertices + "    Tris: " + d._LatestGen_Tris);
                EditorGUILayout.LabelField("Bounds Size: " + d._LatestGen_Bounds.size);
            }

            GUI.enabled = true;
            GUILayout.Space(4);

            EditorGUILayout.EndVertical();


            if (position.height < 430) return;

            Rect rLowRect = position;

            float heightRatio = Mathf.InverseLerp(450, 550, position.height);
            GUI.color = new Color(1f, 1f, 1f, 0.25f + heightRatio * 0.6f);

            float smaller = 40f - Mathf.Min(40f, (heightRatio * 1.65f) * 40f);
            float previewSize = 110f - smaller;
            rLowRect.position = new Vector2(position.width - previewSize, position.height - (previewSize + 4));
            rLowRect.size = new Vector2(previewSize - 3, previewSize);


            if (_foldout_finalizeSetup)
            {
                var selDes = EditedDesign;
                if (selDes != null)
                {
                    if (selDes.IsSomethingGenerated == false)
                    {
                        GenerateCombinedMesh();
                    }

                    if (selDes.IsSomethingGenerated)
                    {
                        RefreshCombinedMeshPreviewDisplayer();

                        if (combinedMeshDisplay != null)
                        {
                            combinedMeshDisplay.UpdateMesh(selDes);
                            CombinePreviewDisplay(rLowRect);
                        }

                        //GUI.Label(rLowRect, "Full Preview");
                    }
                }
            }
            else
            {
                var selTile = d.TileMeshes[_selectedTileMesh];
                if (selTile != null)
                    if (selTile.LatestGeneratedMesh != null)
                        if (_foldout_meshSetup)
                        {
                            TilePreviewDisplay(rLowRect, selTile.LatestGeneratedMesh, selTile.Material);
                            //GUI.Label(rLowRect, d.TileMeshes[_selectedTileMesh].Name);
                        }
            }


            GUI.color = preC;
        }

        private void _SetDirty()
        {
            if (ToDirty != null) EditorUtility.SetDirty(ToDirty);
        }

        private void GenerateSceneObject()
        {
            var d = EditedDesign;

            d.FullGenerateStack();

            Vector3 preScenePos = Vector3.zero;

            if (SceneMeshIsSame())
            {
                preScenePos = _generated_sceneObjRef.transform.position;
                FGenerators.DestroyObject(_generated_sceneObjRef);
            }

            _generated_sceneObjRef = d.GeneratePrefab();
            _generated_sceneObjRef.transform.position = preScenePos;
            _generated_sceneObjParentRef = d;

            if (SceneView.lastActiveSceneView)
            {
                Selection.activeObject = _generated_sceneObjRef;
                SceneView.lastActiveSceneView.FrameSelected();
            }
        }

        bool SceneMeshIsSame()
        {
            if (_generated_sceneObjRef)
                if (_generated_sceneObjParentRef == EditedDesign)
                    return true;

            return false;
        }

        #endregion



        #region Combine GUI

        bool _comb_changed = false;
        int _comb_selectedTileMesh = 0;
        int _comb_selectedMeshCopy = 0;
        bool _comb_autoRefresh = true;
        bool _comb_switchedFromComb = false;

        TileMeshSetup CombRefreshSelTileMeshRef()
        {
            if (_comb_selectedTileMesh < 0) _comb_selectedTileMesh = EditedDesign.TileMeshes.Count - 1;
            if (_comb_selectedTileMesh >= EditedDesign.TileMeshes.Count) _comb_selectedTileMesh = 0;
            return EditedDesign.TileMeshes[_comb_selectedTileMesh];
        }

        void GUICombiner()
        {

            //FGUI_Inspector.DrawUILine(0.3f, 0.5f);
            TileDesign d = EditedDesign;
            if (d.TileMeshes.Count == 0) d.TileMeshes.Add(new TileMeshSetup());

            GUILayout.Label("Tile Combine Mode", FGUI_Resources.HeaderStyle);
            GUILayout.Label("Arrange meshes, remove / join vertices", EditorStyles.centeredGreyMiniLabel);

            FGUI_Inspector.DrawUILine(0.3f, 0.5f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(16);
            if (GUILayout.Button(new GUIContent("  Back to Setup", FGUI_Resources.Tex_Sliders), FGUI_Resources.ButtonStyle, GUILayout.Height(18))) { EditorMode = EMode.Setup; }
            GUILayout.Space(16);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(4);

            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxBlankStyle);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent(" Meshes (" + d.TileMeshes.Count + ")", EditorGUIUtility.IconContent("Mesh Icon").image), EditorStyles.label, GUILayout.Height(19))) _foldout_meshSetup = !_foldout_meshSetup;

            EditorGUILayout.BeginHorizontal(GUILayout.Height(19));
            if (GUILayout.Button(FGUI_Resources.Tex_ArrowLeft, GUILayout.Width(22))) { _comb_selectedTileMesh += 1; _comb_changed = true; }

            GUILayout.Label((_comb_selectedTileMesh + 1) + "/" + d.TileMeshes.Count, EditorStyles.centeredGreyMiniLabel, GUILayout.Height(19));

            if (GUILayout.Button(FGUI_Resources.Tex_ArrowRight, GUILayout.Width(22))) { _comb_selectedTileMesh -= 1; _comb_changed = true; }
            EditorGUILayout.EndHorizontal();


            TileMeshSetup t = CombRefreshSelTileMeshRef();

            GUILayout.Space(12);
            EditorGUIUtility.labelWidth = 60;

            if (t.Copies <= 0) t.Copies = 1;
            t.Copies = EditorGUILayout.IntField("Copies: ", t.Copies, GUILayout.Width(90));
            t.AdjustCopiesCount();

            if (_comb_selectedMeshCopy < 0) _comb_selectedMeshCopy = t.Instances.Count - 1;
            if (_comb_selectedMeshCopy >= t.Instances.Count) _comb_selectedMeshCopy = 0;

            var inst = t.Instances[_comb_selectedMeshCopy];

            EditorGUIUtility.fieldWidth = 0;
            EditorGUIUtility.labelWidth = 0;
            GUILayout.Space(4);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(8);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("[" + (_comb_selectedTileMesh + 1) + "] Instance Copy:", GUILayout.Width(140));

            if (GUILayout.Button(FGUI_Resources.Tex_ArrowLeft, GUILayout.Width(22)))
            {
                _comb_selectedMeshCopy -= 1;

                if (_comb_selectedMeshCopy < 0)
                {
                    _comb_selectedTileMesh -= 1;
                    t = CombRefreshSelTileMeshRef();
                    _comb_selectedMeshCopy = t.Copies - 1;
                }
            }

            if (GUILayout.Button(FGUI_Resources.Tex_ArrowRight, GUILayout.Width(22)))
            {
                _comb_selectedMeshCopy += 1;

                if (_comb_selectedMeshCopy >= t.Copies)
                {
                    _comb_selectedTileMesh += 1;
                    t = CombRefreshSelTileMeshRef();
                    _comb_selectedMeshCopy = 0;
                }
            }

            _comb_selectedMeshCopy = EditorGUILayout.IntPopup(_comb_selectedMeshCopy, GetCopiesNameList(t.Instances, t.Name), GetCopiesIDList(t.Instances));


            EditorGUI.BeginChangeCheck();
            inst.Enabled = EditorGUILayout.Toggle(inst.Enabled, GUILayout.Width(16));
            if (EditorGUI.EndChangeCheck()) _comb_changed = true;


            if (GUILayout.Button(new GUIContent(" Go To " + t.Name, FGUI_Resources.Tex_GearSetup), FGUI_Resources.ButtonStyle, GUILayout.Width(140), GUILayout.Height(18)))
            {
                EditedTileSetup = t;
                _comb_switchedFromComb = true;
                EditorMode = EMode.MeshEditor;
            }

            if (GUILayout.Button(" + ", FGUI_Resources.ButtonStyle, GUILayout.Height(18), GUILayout.Width(20))) { t.Copies += 1; _comb_changed = true;  t.AdjustCopiesCount(); _comb_selectedMeshCopy += 1; }

            GUI.enabled = t.Copies > 1;
            if (GUILayout.Button(FGUI_Resources.GUIC_Remove, FGUI_Resources.ButtonStyle, GUILayout.Height(18), GUILayout.Width(20))) { t.Copies -= 1; _comb_changed = true; t.AdjustCopiesCount(); }
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(8);

            /*MeshMode = (EMeshMode)*/

            EditorGUI.BeginChangeCheck();

            inst.Position = EditorGUILayout.Vector3Field("Position:", inst.Position);
            inst.Rotation = EditorGUILayout.Vector3Field("Rotation:", inst.Rotation);
            inst.Scale = EditorGUILayout.Vector3Field("Scale:", inst.Scale);
            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            inst.MeshMode = (TileMeshSetup.TileMeshCombineInstance.EMeshMode)EditorGUILayout.EnumPopup("Mode:", inst.MeshMode);
            GUILayout.Space(14);

            if (inst.MeshMode == TileMeshCombineInstance.EMeshMode.Default)
            {
                if (/*EditedDesign.ColliderMode == TileDesign.EColliderMode.MeshColliders || */EditedDesign.ColliderMode == TileDesign.EColliderMode.CombinedMeshCollider)
                {
                    EditorGUIUtility.labelWidth = 27;
                    inst.UseInCollider = EditorGUILayout.Toggle(new GUIContent(EditorGUIUtility.IconContent("MeshCollider Icon").image, "Include this mesh as collider mesh"), inst.UseInCollider, GUILayout.Width(54));
                }
            }

            EditorGUIUtility.labelWidth = 120;
            inst.OverrideMaterial = (Material)EditorGUILayout.ObjectField("Override Material:", inst.OverrideMaterial, typeof(Material), false);
            EditorGUIUtility.labelWidth = 0;
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck()) { _comb_changed = true; }


            EditorGUILayout.EndVertical();

            FGUI_Inspector.DrawUILine(0.3f, 0.5f, 1, 6);

            EditorGUILayout.BeginHorizontal(FGUI_Resources.BGInBoxBlankStyle);

            EditorGUIUtility.labelWidth = 100;
            _comb_autoRefresh = EditorGUILayout.Toggle("Auto Refresh:", _comb_autoRefresh, GUILayout.Width(130));
            EditorGUIUtility.labelWidth = 0;

            GUILayout.Space(12);

            if (_comb_autoRefresh)
                if (_comb_changed)
                {
                    _comb_changed = false;
                    GenerateCombinedMesh();
                    if (_generated_sceneObjRef != null) if (SceneMeshIsSame()) GenerateSceneObject();
                }

            if (GUILayout.Button(new GUIContent("  Generate Final Mesh", FGUI_Resources.Tex_Expose), FGUI_Resources.ButtonStyle, GUILayout.Height(20)))
            {
                GenerateCombinedMesh();
            }

            if (GUILayout.Button(new GUIContent("  Scene Mesh", EditorGUIUtility.IconContent("SceneAsset Icon").image), FGUI_Resources.ButtonStyle, GUILayout.Width(120), GUILayout.Height(20)))
            {
                GenerateSceneObject();
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(4);

            Rect editorRect = GUILayoutUtility.GetLastRect();
            editorRect.position += new Vector2(8, 8);
            editorRect.width = position.width - 16;
            editorRect.height = position.height - (editorRect.y + 8);

            GUI.Box(editorRect, GUIContent.none, FGUI_Resources.BGInBoxStyle);

            CombinePreviewDisplay(editorRect);
        }

        static Mesh combinePreviewMesh = null;
        void CombinePreviewDisplay(Rect? rect)
        {
            RefreshCombinedMeshPreviewDisplayer();

            if (combinedMeshDisplay == null)
            {
                if (rect == null) return;
            }

            if (combinedMeshDisplay != null)
            {
                combinedMeshDisplay.UpdateMesh(EditedDesign);
                if (rect == null) return;
                combinedMeshDisplay.OnInteractivePreviewGUI(rect.Value, EditorStyles.textArea);
            }
        }

        void RefreshCombinedMeshPreviewDisplayer()
        {
            if (combinedMeshDisplay == null)
            {
                combinedMeshDisplay = (TilePreviewWindow)Editor.CreateEditor(generatedMesh, typeof(TilePreviewWindow));
            }
        }

        /// <summary> Combine Preview Mesh</summary>
        public static Mesh CPMesh
        {
            get { if (combinePreviewMesh == null) combinePreviewMesh = new Mesh(); return combinePreviewMesh; }
            set { SetMeshFromTo(value, ref combinePreviewMesh); }
        }


        static void SetMeshFromTo(Mesh from, ref Mesh to)
        {
            if (from == null) { return; }
            if (to == null) { to = new Mesh(); return; }

#if UNITY_2019_4_OR_NEWER
            to.SetVertices(from.vertices);
            to.SetNormals(from.normals);
            to.bounds = from.bounds;
            to.SetColors(from.colors);
            to.SetUVs(0, from.uv);
#else
            to.vertices=(from.vertices);
            to.normals=(from.normals);
            to.bounds = from.bounds;
            to.colors =(from.colors);
            to.uv = (from.uv);
#endif
        }

        TilePreviewWindow combinedMeshDisplay = null;

        void GenerateCombinedMesh()
        {
            EditedDesign.FullGenerateStack();

            if (EditedDesign.IsSomethingGenerated)
            {
                CombinePreviewDisplay(null);
            }
        }

#region Copies GUI popup helper

        private int[] _CopiesIds = null;
        object _CopiesLast = null;
        public int[] GetCopiesIDList<T>(List<T> elems, bool forceRefresh = false)
        {
            if (Event.current != null) if (Event.current.type == EventType.MouseDown) forceRefresh = true;

            if (forceRefresh || _CopiesIds == null || _CopiesIds.Length != elems.Count)
            {
                _CopiesIds = new int[elems.Count];
                for (int i = 0; i < elems.Count; i++)
                {
                    _CopiesIds[i] = i;
                }
            }

            return _CopiesIds;
        }

        private string[] _CopiesNames = null;
        public string[] GetCopiesNameList<T>(List<T> elems, string predicate = "", bool forceRefresh = false)
        {
            if (elems != _CopiesLast)
            {
                _CopiesLast = elems;
                forceRefresh = true;
            }

            if (Event.current != null) if (Event.current.type == EventType.MouseDown) forceRefresh = true;

            if (forceRefresh || _CopiesNames == null || _CopiesNames.Length != elems.Count)
            {
                _CopiesNames = new string[elems.Count];
                for (int i = 0; i < elems.Count; i++)
                {
                    _CopiesNames[i] = predicate + " " + i;
                }
            }
            return _CopiesNames;
        }




#endregion



#endregion



#region Tile Mesh GUIs


#region Lathe


        List<CurvePoint> lathe_points { get { return s._lathe_points; } }


        public void GUI_LatheTopPanel()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 60;
            width = EditorGUILayout.FloatField("Width:", width);
            GUILayout.Space(16);
            height = EditorGUILayout.FloatField("Height:", height);

            GUILayout.Space(16);
            EditorGUIUtility.labelWidth = 68;
            EditorGUI.BeginChangeCheck();
            s._lathe_fillAngle = EditorGUILayout.IntSlider("Fill Angle:", s._lathe_fillAngle, 1, 360);


            EditorGUIUtility.labelWidth = 0;

            EditorGUILayout.EndHorizontal();


            EditorGUIUtility.labelWidth = 112;
            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();


            s._lathe_xSubdivCount = EditorGUILayout.IntSlider("Subdivisions (X):", s._lathe_xSubdivCount, 3, 64);

            GUILayout.Space(8);

            EditorGUILayout.BeginHorizontal();

            s._lathe_ySubdivLimit = EditorGUILayout.Slider("Isoparm Limit (Y):", s._lathe_ySubdivLimit, 30, 1);
            s.SubdivMode = (ESubdivideCompute)EditorGUILayout.EnumPopup(s.SubdivMode, GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = 90;

            if (EditorGUI.EndChangeCheck()) { shapeChanged = true; }
        }

        public static List<CurvePoint> _copyCurveRef = null;
        void DrawLatheEditor(Rect editorRect, Rect displayArea)
        {
            displayArea.position = Vector2.zero;
            editorRect.position = Vector2.zero;

            Rect lRect = displayArea;
            lRect.width = displayArea.width / 2;

            DrawLatheEditorLeft(lRect);

            DrawAxisInRect(lRect, "Y", "X");


            GUI.Box(lRect, GUIContent.none, FGUI_Resources.BGInBoxStyleH);

            Rect rRect = displayArea;
            rRect.width = displayArea.width / 2;
            rRect.position += new Vector2(displayArea.width / 2, 0);

            DrawCurveOptionsButton(rRect, lathe_points);
            //if (DrawCurveButton(rRect, EditorGUIUtility.IconContent("TreeEditor.Duplicate").image)) { _copyCurveRef = lathe_points; }
            //DrawPasteCurveButton(rRect, lathe_points);

            DrawLatheEditorRight(rRect);

            Rect rMenu = editorRect;
            rMenu.position = new Vector2(editorRect.width - 240, 0);
            rMenu.width = 240;
            DrawCurvePointMenu(rMenu);

            Rect previewRect = displayArea;
            previewRect.position += new Vector2(displayArea.width * 1.175f, 0f);
            previewRect.size *= 0.7f;
            previewRect.position += new Vector2(0f, previewRect.height * 0.2f);

            DrawLathePreviewShape(previewRect);

            _latestEditorDisplayRect = _editorDisplayRect1;
            UpdateCurveInputEvents(lRect, displayArea, lathe_points);
        }



        private void DrawLathePreviewShape(Rect previewRect)
        {
            if (previewShape.Count == 0) return;


            Rect headerR = previewRect;
            headerR.height = 32;
            headerR.position -= new Vector2(0, 40);
            GUI.Label(headerR, "Subdivisions Preview", EditorStyles.centeredGreyMiniLabel);

            headerR = previewRect;
            headerR.height = 42;
            headerR.position += new Vector2(0, previewRect.height + 2);
            float totalVerts = (previewShape.Count) * (s._lathe_xSubdivCount + 2);
            float totalPoly = (previewShape.Count - 1) * s._lathe_xSubdivCount;
            GUI.Label(headerR, "Y Subdivs: " + previewShape.Count + "\nCalculated Vertices: " + totalVerts + "\nPredicted Tris: " + totalPoly * 2 + " Poly: " + totalPoly, EditorStyles.centeredGreyMiniLabel);


            GUI.BeginGroup(previewRect, FGUI_Resources.BGInBoxStyle);

            Color preH = Handles.color;
            Color preG = GUI.color;

            float width = previewRect.width / 2f;
            float height = previewRect.height;

            Handles.color = new Color(0.8f, 0.8f, 0.8f, 0.7f);
            GUI.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);

            DrawSubdivsPreviewLines(previewShape, width, height, true);

            Handles.color = preH;
            GUI.color = preG;
            GUI.EndGroup();
        }


        void DrawLatheEditorRight(Rect r)
        {
            GUI.Box(r, GUIContent.none, FGUI_Resources.BGInBoxStyleH);

            SetDisplayRect2(r);
            DrawCurves(r, lathe_points, true, true);
        }


        void RefreshDefaultCurve(EMeshGenerator? type = null)
        {
            if (type == null)
            {
                if (EditedTileSetup == null) return;
                type = EditedTileSetup.GenTechnique;
            }

            if (type.Value == EMeshGenerator.Lathe)
            {
                if (lathe_points.Count == 0)
                {
                    lathe_points.Add(new CurvePoint(0.5f, 0.05f, true));
                    lathe_points.Add(new CurvePoint(0.7f, 0.175f, true));
                    lathe_points.Add(new CurvePoint(0.7f, 0.8f, true));
                    lathe_points.Add(new CurvePoint(0.45f, 1f, true));
                    shapeChanged = true;
                }
            }
            else if (type.Value == EMeshGenerator.Loft)
            {
                if (_loft_depth.Count == 0)
                {
                    _loft_depth.Add(new CurvePoint(0.45f, 0f, true));
                    _loft_depth.Add(new CurvePoint(0.45f, .4f, true));
                    _loft_depth.Add(new CurvePoint(0.2f, .4f, true));
                    _loft_depth.Add(new CurvePoint(0.2f, 1f, true));
                    shapeChanged = true;
                }

                if (_loft_distribute.Count == 0)
                {
                    _loft_distribute.Add(new CurvePoint(0.0f, 0.5f, true));
                    _loft_distribute.Add(new CurvePoint(1f, 0.5f, true));
                    shapeChanged = true;
                }
            }
            else if (type.Value == EMeshGenerator.Extrude)
            {
                if (_extrude_curve.Count == 0)
                {
                    //_extrude_curve.Add(new CurvePoint(1f, .8f, true));
                    _extrude_curve.Add(new CurvePoint(0.625f, .8f, true));
                    _extrude_curve.Add(new CurvePoint(0.625f, .3f, true));
                    _extrude_curve.Add(new CurvePoint(0.7f, .215f, true));
                    _extrude_curve.Add(new CurvePoint(0.8f, .2f, true));
                    //_extrude_curve.Add(new CurvePoint(1f, .2f, true));
                    shapeChanged = true;
                }
            }
        }


        void DrawLatheEditorLeft(Rect r)
        {
            SetDisplayRect(r);

#region If lathe list is empty generate example shape

            if (lathe_points.Count == 0)
            {
                RefreshDefaultCurve(EMeshGenerator.Lathe);
            }

#endregion

            GUI.Box(r, GUIContent.none, FGUI_Resources.BGInBoxStyleH);
            DrawCurves(r, lathe_points, false, false);

            bool changed = DisplaySplineHandler(r, lathe_points, null);
            if (changed) shapeChanged = true;
        }

#endregion




#region Loft


        List<CurvePoint> _loft_depth { get { return s._loft_depth; } }
        List<CurvePoint> _loft_distribute { get { return s._loft_distribute; } }



        public void GUI_LoftTopPanel()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 80;
            width = EditorGUILayout.FloatField("Distr Width:", width);
            GUILayout.Space(16);
            EditorGUIUtility.labelWidth = 60;
            height = EditorGUILayout.FloatField("Height:", height);
            GUILayout.Space(16);
            EditorGUIUtility.labelWidth = 80;
            depth = EditorGUILayout.FloatField("Distr Depth:", depth);

            EditorGUI.BeginChangeCheck();

            GUILayout.Space(16);
            EditorGUIUtility.labelWidth = 90;
            s._loftDepthCurveWidener = EditorGUILayout.FloatField("Depth Width:", s._loftDepthCurveWidener, GUILayout.Width(130));
            s._loftDepthCurveWidener = Mathf.Clamp(s._loftDepthCurveWidener, 0.25f, 1f);

            EditorGUIUtility.labelWidth = 0;

            EditorGUILayout.EndHorizontal();


            EditorGUIUtility.labelWidth = 142;
            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();


            s._loft_DepthSubdivLimit = EditorGUILayout.Slider("Depth Subdiv Limit:", s._loft_DepthSubdivLimit, 30, 1);
            if (GenTechnique == EMeshGenerator.Lathe) s._loft_DepthSubdivLimit = Mathf.FloorToInt(s._loft_DepthSubdivLimit);

            GUILayout.Space(8);

            EditorGUILayout.BeginHorizontal();

            s._loft_DistribSubdivLimit = EditorGUILayout.Slider("Distrubute Subdiv Limit:", s._loft_DistribSubdivLimit, 30, 1);
            s.SubdivMode = (ESubdivideCompute)EditorGUILayout.EnumPopup(s.SubdivMode, GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = 90;

            if (EditorGUI.EndChangeCheck()) { shapeChanged = true; }

        }


        void DrawLoftEditor(Rect editorRect, Rect displayArea)
        {
            displayArea.position = Vector2.zero;
            editorRect.position = Vector2.zero;


#region If null lists then generating example shapes

            RefreshDefaultCurve(EMeshGenerator.Loft);

#endregion

            Rect lRect = displayArea;
            lRect.position += new Vector2(0, displayArea.height * 0.045f);
            lRect.size *= 0.9f;
            lRect.width *= s._loftDepthCurveWidener;
            DrawLoftDepthRect(lRect);
            DrawCurves(lRect, _loft_depth, false, false);
            /*if ( lRect.Contains(mousePos))*/
            UpdateCurveInputEvents(lRect, displayArea, _loft_depth);

            Rect rRect = displayArea;
            rRect.position += new Vector2(displayArea.width * s._loftDepthCurveWidener, displayArea.height * 0.045f);
            rRect.size *= 0.9f;
            DrawLoftDistributeRect(rRect);
            DrawCurves(rRect, _loft_distribute, false, true);
            /*if (rRect.Contains(mousePos))*/
            UpdateCurveInputEvents(rRect, displayArea, _loft_distribute);

            DrawCurveOptionsButton(lRect, _loft_depth);
            DrawCurveOptionsButton(rRect, _loft_distribute);

            //if (DrawCurveButton(lRect, EditorGUIUtility.IconContent("TreeEditor.Duplicate").image)) { _copyCurveRef = _loft_depth; }
            //DrawPasteCurveButton(lRect, _loft_depth);

            //if (DrawCurveButton(rRect, EditorGUIUtility.IconContent("TreeEditor.Duplicate").image)) { _copyCurveRef = _loft_distribute; }
            //DrawPasteCurveButton(rRect, _loft_distribute);

            // Curve point handlers must be called in a row because of WindowsBegin()
            BeginWindows();
            _latestEditorDisplayRect = _editorDisplayRect1;
            bool changed = DisplaySplineHandler(lRect, _loft_depth, false);
            _latestEditorDisplayRect = _editorDisplayRect2;
            bool changed2 = DisplaySplineHandler(rRect, _loft_distribute, true);
            EndWindows();

            if (changed || changed2)
            {
                shapeChanged = true;
                repaintRequest = true;
            }

            Rect rMenu = editorRect;
            rMenu.position = new Vector2(editorRect.width - 240, 0);
            rMenu.width = 240;
            DrawCurvePointMenu(rMenu);

            Rect previewRect = displayArea;
            previewRect.position += new Vector2(displayArea.width * (1f + s._loftDepthCurveWidener), 0f);
            previewRect.size *= 0.7f;
            previewRect.position += new Vector2(0f, previewRect.height * 0.2f);

            Rect pRect = previewRect;
            pRect.width *= s._loftDepthCurveWidener;
            previewRect.position += new Vector2(pRect.width, 0);
            DrawLoftPreviewView(pRect, previewRect);
        }

        void DrawLoftDepthRect(Rect r)
        {
            SetDisplayRect(r);
            GUI.Box(r, GUIContent.none, FGUI_Resources.BGInBoxStyleH);

            DrawHeaderLabelOnRect(r, "Depth");
            DrawAxisInRect(r, "Y", "Z");
        }

        void DrawLoftDistributeRect(Rect r)
        {
            SetDisplayRect2(r);
            GUI.Box(r, GUIContent.none, FGUI_Resources.BGInBoxStyleH);

            DrawHeaderLabelOnRect(r, "Distribute");
            DrawAxisInRect(r, "Z", "X");
        }

        void DrawLoftPreviewView(Rect pRect1, Rect pRect2)
        {
            if (previewShape.Count == 0 && previewShape2.Count == 0) return;

            Color preH = Handles.color;
            Color preG = GUI.color;


            Rect prevRect = pRect1;
            prevRect.width += pRect2.width;

            Rect headerR = prevRect;
            headerR.height = 32;
            headerR.position -= new Vector2(0, 40);
            GUI.Label(headerR, "Subdivisions Preview", EditorStyles.centeredGreyMiniLabel);

            headerR = prevRect;
            headerR.height = 42;
            headerR.position += new Vector2(0, pRect1.height + 2);

            int teoVertsC = previewShape.Count + previewShape2.Count;
            float totalVerts = (previewShape.Count * previewShape2.Count);
            float totalPoly = Mathf.Round(((previewShape.Count + 2) * (previewShape2.Count)) / 2f);
            GUI.Label(headerR, "Subdivs: " + teoVertsC + "\nCalculated Vertices: " + totalVerts + "\nPredicted Tris: " + totalPoly * 2 + " Poly: " + totalPoly, EditorStyles.centeredGreyMiniLabel);


            Handles.color = new Color(0.8f, 0.8f, 0.8f, 0.7f);
            GUI.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);


            GUI.BeginGroup(pRect1, FGUI_Resources.BGInBoxStyle);
            DrawSubdivsPreviewLines(previewShape, pRect1.width, pRect1.height, false);
            GUI.EndGroup();


            GUI.BeginGroup(pRect2, FGUI_Resources.BGInBoxStyle);
            DrawSubdivsPreviewLines(previewShape2, pRect2.width, pRect2.height, false);
            GUI.EndGroup();

            Handles.color = preH;
            GUI.color = preG;
        }



#endregion



#region Extrude

        List<CurvePoint> _extrude_curve { get { return s._extrude_curve; } }


        public void GUI_ExtrudeTopPanel()
        {

            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 60;
            width = EditorGUILayout.FloatField("Width:", width);
            GUILayout.Space(16);
            height = EditorGUILayout.FloatField("Height:", height);

            GUILayout.Space(16);
            s.depth = EditorGUILayout.FloatField("Depth:", s.depth);

            EditorGUIUtility.labelWidth = 0;

            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();

            EditorGUIUtility.labelWidth = 102;
            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();

            //extrudeMirror = EditorGUILayout.Toggle("Mirror:", extrudeMirror);
            EditorGUIUtility.labelWidth = 72;
            s._extrudeFrontCap = EditorGUILayout.Toggle("Front Cap:", s._extrudeFrontCap, GUILayout.Width(102));
            EditorGUIUtility.labelWidth = 42;
            s._extrudeBackCap = EditorGUILayout.Toggle("Back:", s._extrudeBackCap, GUILayout.Width(82));
            EditorGUIUtility.labelWidth = 102;


            GUILayout.Space(8);

            EditorGUILayout.BeginHorizontal();

            s._extrude_SubdivLimit = EditorGUILayout.Slider("Isoparm Limit(Y):", s._extrude_SubdivLimit, 30, 1);
            s.SubdivMode = (ESubdivideCompute)EditorGUILayout.EnumPopup(s.SubdivMode, GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = 90;
            if (EditorGUI.EndChangeCheck()) { shapeChanged = true; }
        }


        void DrawExtrudeEditor(Rect editorRect, Rect displayArea)
        {
            displayArea.position = Vector2.zero;
            editorRect.position = Vector2.zero;

#region If null lists then generating example shapes

            if (_extrude_curve.Count == 0)
            {
                RefreshDefaultCurve(EMeshGenerator.Extrude);
            }

#endregion

            Rect lRect = displayArea;
            lRect.width = displayArea.width / 2;

            GUI.Box(displayArea, GUIContent.none, FGUI_Resources.BGInBoxStyleH);
            //DrawHeaderLabelOnRect(displayArea, "Symmetry Shape");

            DrawAxisInRect(lRect, "Y", "X");
            SetDisplayRect(lRect);
            GUI.Box(lRect, GUIContent.none, FGUI_Resources.BGInBoxStyleH);

            DrawCurves(lRect, _extrude_curve, false, false);
            UpdateCurveInputEvents(lRect, displayArea, _extrude_curve);

            DrawCurveOptionsButton(lRect, _extrude_curve);
            //if (DrawCurveButton(lRect, EditorGUIUtility.IconContent("TreeEditor.Duplicate").image)) { _copyCurveRef = _extrude_curve; }
            //DrawPasteCurveButton(lRect, _extrude_curve);

            Rect rRect = lRect;
            rRect.position += new Vector2(rRect.width, 0);
            DrawCurves(rRect, _extrude_curve, true, false);

            bool changed = DisplaySplineHandler(lRect, _extrude_curve, null);

            if (changed)
            {
                shapeChanged = true;
                repaintRequest = true;
            }

            Rect rMenu = editorRect;
            rMenu.position = new Vector2(editorRect.width - 240, 0);
            rMenu.width = 240;
            DrawCurvePointMenu(rMenu);

            Rect previewRect = displayArea;
            previewRect.position += new Vector2(displayArea.width * 1.175f, 0f);
            previewRect.size *= 0.7f;
            previewRect.position += new Vector2(0f, previewRect.height * 0.2f);
            DrawExtrudePreviewShape(previewRect);
        }


        private void DrawExtrudePreviewShape(Rect previewRect)
        {
            if (_extrude_curve.Count == 0) return;


            Rect headerR = previewRect;
            headerR.height = 32;
            headerR.position -= new Vector2(0, 40);
            GUI.Label(headerR, "Subdivisions Preview", EditorStyles.centeredGreyMiniLabel);

            headerR = previewRect;
            headerR.height = 42;
            headerR.position += new Vector2(0, previewRect.height + 2);
            float totalPoly = (previewShape.Count - 2) * 4 - 2;
            if (!s._extrudeFrontCap) totalPoly -= (previewShape.Count - 2);
            if (!s._extrudeBackCap) totalPoly -= (previewShape.Count - 2);
            float totalVerts = (previewShape.Count - 2) * 4;
            GUI.Label(headerR, "Vertices: " + totalVerts + "\nPredicted Tris: " + totalPoly * 2 + " Poly: " + totalPoly, EditorStyles.centeredGreyMiniLabel);


            GUI.BeginGroup(previewRect, FGUI_Resources.BGInBoxStyle);

            Color preH = Handles.color;
            Color preG = GUI.color;

            float width = previewRect.width / 2f;
            float height = previewRect.height;

            Handles.color = new Color(0.8f, 0.8f, 0.8f, 0.7f);
            GUI.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);

            DrawSubdivsPreviewLines(previewShape, width, height, true);

            Handles.color = preH;
            GUI.color = preG;
            GUI.EndGroup();
        }






#endregion


#region Sweep

        public void GUI_SweepTopPanel()
        {

            GUILayout.Space(16);

            EditorGUILayout.HelpBox(" Sweep is not implemented yet!", MessageType.Info);

            GUILayout.Space(16);

        }

        public void DrawSweepEditor(Rect editorRect, Rect displayArea)
        {

            //EditorGUILayout.BeginHorizontal();
            //EditorGUIUtility.labelWidth = 60;
            //width = EditorGUILayout.FloatField("Width:", width);
            //GUILayout.Space(16);
            //height = EditorGUILayout.FloatField("Height:", height);

            //GUILayout.Space(16);
            //s.depth = EditorGUILayout.FloatField("Depth:", s.depth);

            //EditorGUIUtility.labelWidth = 0;

            //EditorGUILayout.EndHorizontal();


        }

#endregion




#region Custom Mesh

        public void GUI_CustomMeshTopPanel()
        {
            GUILayout.Space(8);

            EditorGUILayout.HelpBox("Use custom mesh or other tile mesh to join it with the tile design in the 'Combiner' menu.", MessageType.Info);

            GUILayout.Space(8);

            EditedTileSetup.CustomMesh = (Mesh)EditorGUILayout.ObjectField("Custom Mesh:", EditedTileSetup.CustomMesh, typeof(Mesh), false);
            GUILayout.Space(4);
            EditedTileSetup.Material = (Material)EditorGUILayout.ObjectField("Material:", EditedTileSetup.Material, typeof(Material), false);
            GUILayout.Space(8);

        }

        void DrawCustomMeshEditor(Rect editorRect, Rect displayArea)
        {
        }



#endregion


#endregion



#region GUI Helpers



        void DrawAxisInRect(Rect r, string up, string right)
        {
#region Axis display

            Vector2 axisOrig = new Vector2(r.x + 12, r.y + r.height - 23f);
            Rect axisR = r;
            axisR.position = axisOrig + new Vector2(12, 4);
            axisR.size = new Vector2(20, 20);
            GUI.Label(axisR, right, EditorStyles.centeredGreyMiniLabel);

            axisR = r;
            axisR.position = axisOrig + new Vector2(-10, -20);
            axisR.size = new Vector2(20, 20);
            GUI.Label(axisR, up, EditorStyles.centeredGreyMiniLabel);

            Color preH = Handles.color;
            Handles.color = _curveCol * 0.8f;
            axisOrig -= new Vector2(2, -15);
            Handles.DrawLine(axisOrig, axisOrig + new Vector2(15, 0));
            Handles.DrawLine(axisOrig, axisOrig + new Vector2(0, -15));
            Handles.color = preH;

#endregion
        }


        bool DrawCurveButton(Rect r, Texture icon, string tooltip = "", float rOffset = 0f)
        {
            Vector2 buttOrig = new Vector2(r.x + r.width - 24 - rOffset, r.y + r.height - 27f);
            Rect buttonR = r;
            buttonR.position = buttOrig;
            buttonR.size = new Vector2(20, 20);
            if (GUI.Button(buttonR, new GUIContent(icon, tooltip), FGUI_Resources.ButtonStyle)) { return true; }
            return false;
        }

        void DrawCurveOptionsButton(Rect r, List<CurvePoint> curve, float rOffset = 0f)
        {
            Vector2 buttOrig = new Vector2(r.x + r.width - 24 - rOffset, r.y + r.height - 23f);
            Rect buttonR = r;
            buttonR.position = buttOrig;
            buttonR.size = new Vector2(16, 16);

            if (GUI.Button(buttonR, FGUI_Resources.GUIC_More, EditorStyles.label))
            {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Copy Curve"), false, () => { _copyCurveRef = curve; });

                if (_copyCurveRef != null && _copyCurveRef != curve)
                {
                    menu.AddItem(GUIContent.none, false, () => { });
                    menu.AddItem(new GUIContent("Paste Curve"), false, () =>
                    {
                        CurvePoint.CopyListFromTo(_copyCurveRef, curve);
                    });
                }

                menu.AddItem(GUIContent.none, false, () => { });
                menu.AddItem(new GUIContent("Clear Curve"), false, () => { curve.Clear(); });

                menu.ShowAsContext();
            }
        }


        void DrawHeaderLabelOnRect(Rect r, string label)
        {
            Rect labelR = r;
            labelR.height = 24;
            labelR.position -= new Vector2(0, 23);
            GUI.Label(labelR, label, EditorStyles.centeredGreyMiniLabel);
        }



#endregion


    }
}
#endif