using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;

public class GrassFieldGenerator : EditorWindow
{

    static GrassFieldGenerator instance;

    private float[] fieldSize = new float[2];
    private GUIContent[] fieldLabels = { new GUIContent("X"), new GUIContent("Z") };

    private int[] gridSize = new int[2];

    private List<Vector3> positions = new();
    private List<Vector3> normals = new();
    private List<Vector2> uvs = new();
    private List<int> indices = new();

    private Material grassMaterial;

    [MenuItem("Window/Grass Field Generator")]
    static void CreateWindow()
    {
        instance = EditorWindow.GetWindow<GrassFieldGenerator>();
    }
    private void OnGUI()
    {
        Rect rect = EditorGUILayout.GetControlRect();

        EditorGUI.LabelField(rect, "Field Size");
        rect.y += 20;

        EditorGUI.MultiFloatField(rect, fieldLabels, fieldSize);
        rect.y += 20;

        EditorGUI.LabelField(rect, "Grid Size");
        rect.y += 20;

        EditorGUI.MultiIntField(rect, fieldLabels, gridSize);
        rect.y += 20;

        if (GUI.Button(rect, "Create New Grass Field"))
        {

            Mesh meshBuffer = new();

            GameObject field = new GameObject();
            Undo.RegisterCreatedObjectUndo(field, "Create New Grass Field");

            positions.Clear();
            indices.Clear();
            normals.Clear();
            uvs.Clear();

            float xFieldSize = fieldSize[0] + 1;
            float zFieldSize = fieldSize[1] + 1;

            int xGridSize = gridSize[0] + 1;
            int zGridSize = gridSize[1] + 1;

            float distPerBlockX = xFieldSize / (float)xGridSize;
            float distPerBlockZ = zFieldSize / (float)zGridSize;

            float uvPerBlockX = 1.0f / (float)xGridSize;
            float uvPerBlockZ = 1.0f / (float)zGridSize;

            MeshRenderer renderer = field.AddComponent<MeshRenderer>();
            MeshFilter filter = field.AddComponent<MeshFilter>();

            float errorCorrectionX = 0.5f / xFieldSize;
            float errorCorrectionZ = 0.5f / zFieldSize;

            float xPos = -(xFieldSize / 2.0f) + 1.0f;
            float zPos = -(zFieldSize / 2.0f);

            float xUV = 0.0f;
            float zUV = 0.0f;


            int index = 0;
            for (int z = 0; z < zGridSize; z++)
            {
                xPos = -(xFieldSize / 2) + 1;

                for (int x = 0; x < xGridSize; x++)
                {
                    Vector3 position = new Vector3(xPos, 0.0f, zPos);
                    Vector2 uv = new Vector2(xUV, zUV);
                    Vector3 normal = new Vector3(0.0f, 1.0f, 0.0f);

                    positions.Add(position);
                    normals.Add(normal);
                    uvs.Add(uv);

                    xPos += distPerBlockX;
                    xUV += uvPerBlockX;

                    if (x < xGridSize - 1 && z < zGridSize - 1)
                    {
                        indices.Add(index);
                        indices.Add(index + 1);
                        indices.Add(index + xGridSize + 1);

                        indices.Add(index);
                        indices.Add(index + xGridSize + 1);
                        indices.Add(index + xGridSize);
                    }
                    
                    index++;

                }
                zPos += distPerBlockZ;
                zUV += uvPerBlockZ;
            }

            meshBuffer.SetVertices(positions);
            meshBuffer.SetIndices(indices, MeshTopology.Triangles, 0);
            meshBuffer.SetNormals(normals);
            meshBuffer.uv = uvs.ToArray();

            Material mat = Resources.Load("Materials/GrassField") as Material;

            renderer.material = mat;

            filter.mesh = meshBuffer;

            Undo.FlushUndoRecordObjects();
        }

    }
}

#endif