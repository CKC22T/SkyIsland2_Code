using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.UI;

namespace Olympus
{
    public class AlignmentHelper : EditorWindow
    {
        private List<Transform> transforms = new();
        private List<string> selectedObjectNames;
        public float distance = 1.0f;

        private bool objectListFoldout = false;
        private Vector2 objectListScrollPosition;

        int selectedAxis = 0;

        private Vector3 highestPosition;
        private Vector3 lowestPosition;

        private int[] gridSize = new int[3];

        enum AxisFilter
        {
            AXIS_X = 1,
            AXIS_Y = 2,
            AXIS_Z = 4,
        }

        private AxisFilter axisFilter;

        [MenuItem("Window/Alignment Helper")]
        static void CreateWindow()
        {
            AlignmentHelper instance = EditorWindow.GetWindow<AlignmentHelper>();
            LogUtil.Log("AlignmentHelper Instance: " + instance);
        }

        Rect mainRect;

        private Vector3 GetHighest(GameObject[] objects, AxisFilter axis)
        {
            Vector3 highest = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < objects.Length; i++)
            {
                Vector3 objectPos = objects[i].transform.position;
                float compare = float.MinValue;
                float target = float.MinValue;
                switch (axis)
                {
                    case AxisFilter.AXIS_X:
                        target = objectPos.x;
                        compare = highest.x;
                        break;

                    case AxisFilter.AXIS_Y:
                        target = objectPos.y;
                        compare = highest.y;

                        break;

                    case AxisFilter.AXIS_Z:
                        target = objectPos.z;
                        compare = highest.z;
                        break;
                }

                if (target > compare)
                {
                    highest = objectPos;
                }
            }

            return highest;
        }

        private Vector3 GetLowest(GameObject[] objects, AxisFilter axis)
        {
            Vector3 lowest = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            for (int i = 0; i < objects.Length; i++)
            {
                Vector3 objectPos = objects[i].transform.position;
                float compare = float.MaxValue;
                float target = float.MaxValue;
                switch (axis)
                {
                    case AxisFilter.AXIS_X:
                        target = objectPos.x;
                        compare = lowest.x;
                        break;

                    case AxisFilter.AXIS_Y:
                        target = objectPos.y;
                        compare = lowest.y;

                        break;

                    case AxisFilter.AXIS_Z:
                        target = objectPos.z;
                        compare = lowest.z;
                        break;
                }

                if (target < compare)
                {
                    lowest = objectPos;
                }
            }

            return lowest;
        }

        private void OnSelectionChange()
        {
            transforms.Clear();
            foreach (var i in Selection.gameObjects)
            {
                transforms.Add(i.gameObject.transform);
                selectedObjectNames.Add(i.gameObject.name);
            }

            Repaint();

        }

        private void OnInspectorUpdate()
        {
            highestPosition = GetHighest(Selection.gameObjects, axisFilter);
            lowestPosition = GetLowest(Selection.gameObjects, axisFilter);


            //for (int i = 0; i < transforms.Count; i++)
            //{
            //    //olor = Color.green;
            //  //  Debug.DrawLine(lowestPosition, highestPosition, Color.green);
            //    //newPosition = new Vector3(lowest.x + (distance * i), lowest.y, lowest.z);
            //}
        }

        private void OnGUI()
        {
            var objects = Selection.gameObjects;
            GUIContent[] gridLabels = { new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z") };

            mainRect = EditorGUILayout.GetControlRect();


            EditorGUI.LabelField(mainRect, "Distance per Object");
            mainRect.y += 20;
            distance = EditorGUI.Slider(mainRect, distance, -100.0f, 100.0f);

            mainRect.y += 20;

            EditorGUI.LabelField(mainRect, "Grid Size");
            mainRect.y += 20;

            EditorGUI.MultiIntField(mainRect, gridLabels, gridSize);
            mainRect.y += 20;

            switch (selectedAxis)
            {
                case 0:
                    axisFilter = AxisFilter.AXIS_X;
                    break;

                case 1:
                    axisFilter = AxisFilter.AXIS_Y;
                    break;

                case 2:
                    axisFilter = AxisFilter.AXIS_Z;
                    break;
            }

            LogUtil.Log("Selected Axis: " + selectedAxis);


            Rect autoAlignButtonRect = mainRect;
            autoAlignButtonRect.height = 20;
            autoAlignButtonRect.y += 20;

            EditorGUI.LabelField(autoAlignButtonRect, "Alignment Options");
            autoAlignButtonRect.width = 160;
            autoAlignButtonRect.y += 20;

            if (GUI.Button(autoAlignButtonRect, "Auto-Align"))
            {
                Undo.RegisterCompleteObjectUndo(transforms.ToArray(), "Objects Auto-Alignment");

                Vector3 end = GetHighest(objects, axisFilter);
                Vector3 start = GetLowest(objects, axisFilter);
                Vector3 origin = start;
                float dist = Vector3.Distance(start, end);
                float factor = dist / (objects.Length - 1);

                for (int i = 0; i < objects.Length; i++)
                {
                    LogUtil.Log(objects[i].name);
                    if (selectedAxis == 0)
                    {
                        origin = new Vector3(start.x + (factor * i), start.y, start.z);
                    }
                    else if (selectedAxis == 1)
                    {
                        origin = new Vector3(start.x, start.y + (factor * i), start.z);
                    }
                    else if (selectedAxis == 2)
                    {
                        origin = new Vector3(start.x, start.y, start.z + (factor * i));
                    }

                    objects[i].transform.position = origin;
                }

                Undo.FlushUndoRecordObjects();

            }

            mainRect.y += 60;

            Rect autoDistanceRect = autoAlignButtonRect;
            autoDistanceRect.y += 20;

            if (GUI.Button(autoDistanceRect, "Auto-Distancing"))
            {
                Undo.RegisterCompleteObjectUndo(transforms.ToArray(), "Objects Auto-Distancing");

                Vector3 highest = GetHighest(objects, axisFilter);
                Vector3 lowest = GetLowest(objects, axisFilter);
                Vector3 newPosition = Vector3.zero;

                for (int i = 0; i < objects.Length; i++)
                {
                    if (selectedAxis == 0)
                    {
                        newPosition = new Vector3(lowest.x + (distance * i), lowest.y, lowest.z);
                    }
                    else if (selectedAxis == 1)
                    {

                    }
                    else if (selectedAxis == 2)
                    {

                    }

                    objects[i].transform.position = newPosition;
                }
                Undo.FlushUndoRecordObjects();
            }

            Rect spreadAlignButtonRect = autoDistanceRect;
            spreadAlignButtonRect.y += 20;

            if (GUI.Button(spreadAlignButtonRect, "Align by 3-Axis Grid"))
            {
                Undo.RegisterCompleteObjectUndo(transforms.ToArray(), "Objects 3-Axis Alignment");

                int xSize = gridSize[0];
                int ySize = gridSize[1];
                int zSize = gridSize[2];
                int index = 0;

                System.Action gridIteration = () =>
                {
                    for (int z = 0; z < zSize; z++)
                    {
                        for (int y = 0; y < ySize; y++)
                        {
                            for (int x = 0; x < xSize; x++)
                            {
                                if (index >= objects.Length)
                                {
                                    return;
                                }

                                Vector3 newPosition = new Vector3(x * distance, y * distance, z * distance);

                                objects[index].transform.position = newPosition;

                                index++;
                            }
                        }
                    }
                };

                gridIteration();

                Undo.FlushUndoRecordObjects();
            }

            spreadAlignButtonRect.y += 20;
            objectListFoldout = EditorGUI.BeginFoldoutHeaderGroup(spreadAlignButtonRect, objectListFoldout, "Selected Objects");

            if (objectListFoldout == true)
            {
                //objectListScrollPosition = new Vector2(spreadAlignButtonRect.width, 20 * objects.Length);
                objectListScrollPosition = EditorGUILayout.BeginScrollView(objectListScrollPosition, GUILayout.Width(spreadAlignButtonRect.width), GUILayout.Height(20 * objects.Length));

                for (int i = 0; i < objects.Length; i++)
                {
                    EditorGUI.indentLevel++;
                    spreadAlignButtonRect.y += 20;
                    EditorGUI.LabelField(spreadAlignButtonRect, objects[i].name);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndScrollView();
            }


            //  EditorGUI.LabelField(spreadAlignButtonRect, );

            EditorGUI.EndFoldoutHeaderGroup();
            //      EditorGUI.Popup(spreadAlignButtonRect, 0, selectedObjectNames.ToArray());

        }
    }
}

#endif