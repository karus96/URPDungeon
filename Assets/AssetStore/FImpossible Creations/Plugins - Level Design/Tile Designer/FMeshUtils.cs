using Parabox.CSG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.Generating
{
    public static class FMeshUtils
    {

        public static void SmoothMeshNormals(Mesh m, float hard)
        {
            var verts = m.vertices;
            var triangles = m.triangles;

            Vector3[] normals = new Vector3[verts.Length];

            List<Vector3>[] vertexNormals = new List<Vector3>[verts.Length];

            for (int i = 0; i < vertexNormals.Length; i++)
            {
                vertexNormals[i] = new List<Vector3>();
            }

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 currNormal = Vector3.Cross(
                    (verts[triangles[i + 1]] - verts[triangles[i]]).normalized,
                    (verts[triangles[i + 2]] - verts[triangles[i]]).normalized);

                vertexNormals[triangles[i]].Add(currNormal);
                vertexNormals[triangles[i + 1]].Add(currNormal);
                vertexNormals[triangles[i + 2]].Add(currNormal);
            }

            for (int i = 0; i < vertexNormals.Length; i++)
            {
                normals[i] = Vector3.zero;

                float numNormals = vertexNormals[i].Count;
                for (int j = 0; j < numNormals; j++)
                {
                    normals[i] += vertexNormals[i][j];
                }

                normals[i] /= numNormals;

                if (hard > 0.05f)
                {
                    if (normals[i].sqrMagnitude > Mathf.Epsilon)
                    {
                        Quaternion look = Quaternion.LookRotation(normals[i]);
                        Vector3 sm = look.eulerAngles;
                        sm = FVectorMethods.FlattenVector(sm, hard * 90f);
                        normals[i] = Quaternion.Euler(sm) * Vector3.forward;
                    }
                }
            }

            m.normals = normals;
        }


        public static Mesh MeshesOperation(Mesh combined, Mesh removeCombination, Parabox.CSG.CSG.BooleanOp operation)
        {
            if (operation == Parabox.CSG.CSG.BooleanOp.None) return combined;

            Material defMat = new Material(Shader.Find("Diffuse"));
            Model result;

            if (operation == CSG.BooleanOp.Intersection)
                result = CSG.Intersect(combined, defMat, Matrix4x4.identity, removeCombination, defMat, Matrix4x4.identity, true);
            else if (operation == CSG.BooleanOp.Subtraction)
                result = CSG.Subtract(combined, defMat, Matrix4x4.identity, removeCombination, defMat, Matrix4x4.identity, true);
            else //if (operation == CSG.BooleanOp.Union)
                result = CSG.Union(combined, defMat, Matrix4x4.identity, removeCombination, defMat, Matrix4x4.identity, true);


            return result.mesh;
        }


        public static Mesh AdjustOrigin(Mesh m, TileMeshSetup.EOrigin origin)
        {
            m.RecalculateBounds();

            if (origin == TileMeshSetup.EOrigin.Unchanged) return m;
            else if (origin == TileMeshSetup.EOrigin.Center)
            {
                Vector3 off = -m.bounds.center;
                var verts = m.vertices;

                // Center Offset
                for (int v = 0; v < verts.Length; v++) verts[v] += off;


                m.SetVerticesUnity2018(verts);
            }
            else if (origin == TileMeshSetup.EOrigin.BottomCenter)
            {
                Vector3 off = new Vector3(-m.bounds.center.x, -m.bounds.min.y, -m.bounds.center.z);

                var verts = m.vertices;
                for (int v = 0; v < verts.Length; v++) verts[v] += off;

                m.SetVerticesUnity2018(verts);
            }
            else if (origin == TileMeshSetup.EOrigin.TopCenter)
            {
                Vector3 off = new Vector3(-m.bounds.center.x, -m.bounds.max.y, -m.bounds.center.z);

                var verts = m.vertices;
                for (int v = 0; v < verts.Length; v++) verts[v] += off;

                m.SetVerticesUnity2018(verts);
            }
            else if (origin == TileMeshSetup.EOrigin.BottomLeft)
            {
                Vector3 off = new Vector3(-m.bounds.min.x, -m.bounds.min.y, -m.bounds.min.z);

                var verts = m.vertices;
                for (int v = 0; v < verts.Length; v++) verts[v] += off;

                m.SetVerticesUnity2018(verts);
            }
            else if (origin == TileMeshSetup.EOrigin.BottomCenterBack)
            {
                Vector3 off = new Vector3(-m.bounds.center.x, -m.bounds.min.y, -m.bounds.min.z);

                var verts = m.vertices;
                for (int v = 0; v < verts.Length; v++) verts[v] += off;

                m.SetVerticesUnity2018(verts);
            }
            else if (origin == TileMeshSetup.EOrigin.BottomCenterFront)
            {
                Vector3 off = new Vector3(-m.bounds.center.x, -m.bounds.min.y, -m.bounds.max.z);

                var verts = m.vertices;
                for (int v = 0; v < verts.Length; v++) verts[v] += off;

                m.SetVerticesUnity2018(verts);
            }

            return m;
        }


        public static void SetVerticesUnity2018(this Mesh m, Vector3[] verts)
        {
#if UNITY_2019_4_OR_NEWER
            m.SetVertices(verts);
#else
            m.vertices = verts;
#endif
        }

    }
}