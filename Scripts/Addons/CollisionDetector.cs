using Olympus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.ShortcutManagement;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

[ExecuteInEditMode()]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CollisionDetector : MonoBehaviour
{
    [SerializeField, TabGroup("Collider")] float Radius;
    [SerializeField, TabGroup("Collider")] float Height;
    [SerializeField, TabGroup("Rendering")] Camera drawCamera;

    [SerializeField, TabGroup("Info"), DisableIf("@true")] GameObject groundObject;
    [SerializeField, TabGroup("Info"), DisableIf("@true")] float groundDistance;

    [FoldoutGroup("Simulation Properties"), SerializeField, DisableIf("@true")] bool Control;

    [SerializeField, DisableIf("@true"), ShowIf("@IsGizmoOn == false"), LabelText("Gizmo Is Currently Hidden!")]
    bool IsGizmoOn = false;

    [SerializeField, TabGroup("Rendering")] bool UseSolid;
    [SerializeField, TabGroup("Rendering")] float CapsuleThickness;

    private Renderer detectorRenderer;
    private MeshFilter detectorFilter;
    private int layerMask = 0;
    private float yOffset;
    private readonly float threshold = 0.001f;

    void DrawGizmos()
    {
        yOffset = (Height / 2.0f);
        Vector3 head = transform.position + (transform.up * (yOffset - 0.5f - threshold));
        Vector3 foot = transform.position + (-transform.up * (yOffset - 0.5f - threshold));

        if (Physics.CheckCapsule(head, foot, Radius, layerMask, QueryTriggerInteraction.Ignore))
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
            if (UseSolid == false)
            {
                DrawWireCapsule(transform.position, transform.rotation, Radius, Height, Color.red, CapsuleThickness);
                detectorRenderer.enabled = false;
            }
            else
            {
                detectorRenderer.enabled = true;
                detectorRenderer.sharedMaterial.color = new Color(1, 0, 0, 0.5f);
            }

        }
        else
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
            if (UseSolid == false)
            {
                DrawWireCapsule(transform.position, transform.rotation, Radius, Height, Color.green, CapsuleThickness);
                detectorRenderer.enabled = false;
            }
            else
            {
                detectorRenderer.enabled = true;
                detectorRenderer.sharedMaterial.color = new Color(0, 1, 0, 0.5f);
            }

        }

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, float.PositiveInfinity, layerMask, QueryTriggerInteraction.Ignore) == true)
        {
            Handles.color = Color.magenta;
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            GUI.contentColor = Color.magenta;

            labelStyle.fontSize = 16;
            labelStyle.fontStyle = FontStyle.Bold;

            Handles.DrawLine(transform.position - (transform.up * yOffset), hitInfo.point, 10.0f);

            Vector3 labelPosition = transform.position;
            Vector3 labelWorldPosition = Vector3.zero;

            groundObject = hitInfo.collider.gameObject;
            groundDistance = hitInfo.distance;
           // Handles.Label(labelPosition, "Ground Distance: " + (hitInfo.distance - yOffset), labelStyle);
           // labelPosition.y += 1;
           // Handles.Label(labelPosition, "Ground Object: " + hitInfo.collider.gameObject.name, labelStyle);

            Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
        }
        else
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
        }

    }

    private void OnEnable()
    {
        ResetDetector();
    }

    private void OnDrawGizmos()
    {
        IsGizmoOn = true;

        DrawGizmos();

        if (Control == true)
        {
            transform.position = drawCamera.transform.position;
        }
    }

    [Button]
    void GroundSnap()
    {
        RaycastHit hitInfo;
        Ray r = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(r, out hitInfo, float.PositiveInfinity, layerMask, QueryTriggerInteraction.Ignore) == true)
        {
            transform.position = hitInfo.point + (transform.up * yOffset);
        }
        else
        {
            r = new Ray(transform.position, Vector3.up);
            if (Physics.Raycast(r, out hitInfo, float.PositiveInfinity, layerMask, QueryTriggerInteraction.Ignore) == true)
            {
                transform.position = hitInfo.point + (transform.up * yOffset);
            }
        }
    }

    [Button]
    void DetectorControl()
    {
        Control = !Control;
    }

    [Button]
    void ResetDetector()
    {
        if (detectorRenderer == null)
        {
            detectorRenderer = GetComponent<MeshRenderer>();
        }
        detectorRenderer.sharedMaterial = null;

        if (detectorFilter == null)
        {
            detectorFilter = GetComponent<MeshFilter>();
            GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            detectorFilter.sharedMesh = capsule.GetComponent<MeshFilter>().sharedMesh;
            DestroyImmediate(capsule);
        }
        if (detectorRenderer.sharedMaterial == null)
        {
            Shader shader = Shader.Find("Unlit/Color");
            detectorRenderer.sharedMaterial = new Material(shader);
            detectorRenderer.sharedMaterial.SetFloat("_Ztest", 0.0f);
        }

        LogUtil.Assert(detectorRenderer != null);
        LogUtil.Assert(detectorRenderer.sharedMaterial != null);
        LogUtil.Assert(detectorFilter != null);
        LogUtil.Assert(detectorFilter.sharedMesh != null);

        layerMask = (1 << LayerMask.NameToLayer("ObjectForPlayer"));
        layerMask |= (1 << LayerMask.NameToLayer("Enemy"));
        layerMask |= (1 << LayerMask.NameToLayer("Platform"));
        layerMask |= (1 << LayerMask.NameToLayer("Default"));

        if (Radius == 0.0f)
        {
            Radius = 0.5f;
        }
        if(Height == 0.0f)
        {
            Height = 2.0f;
        }
        if(CapsuleThickness == 0.0f)
        {
            CapsuleThickness = 5.0f;
        }

        var sceneCameras = SceneView.GetAllSceneCameras();
        if (sceneCameras.Length == 0)
        {
            return;
        }

        drawCamera = sceneCameras[0];
        LogUtil.Assert(drawCamera != null);

        IsGizmoOn = false;
    }

    #region static
    /// <summary>
    /// √‚√≥: https://answers.unity.com/questions/56063/draw-capsule-gizmo.html?childToView=1476302#answer-1476302
    /// </summary>
    /// <param name="_pos"></param>
    /// <param name="_rot"></param>
    /// <param name="_radius"></param>
    /// <param name="_height"></param>
    /// <param name="_color"></param>
    public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color _color = default(Color), float thickness = 1.0f)
    {
        if (_color != default(Color))
            Handles.color = _color;
        Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);
        using (new Handles.DrawingScope(angleMatrix))
        {
            var pointOffset = (_height - (_radius * 2)) / 2;

            //draw sideways
            //Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;

            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius, thickness);
            Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius), thickness);
            Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius), thickness);
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius, thickness);
            //draw frontways
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius, thickness);
            Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0), thickness);
            Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0), thickness);
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius, thickness);
            //draw center
            Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius, thickness);
            Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius, thickness);

        }
    }

    #endregion

    #region Input
#if UNITY_EDITOR
    private void OnValidate()
    {
        SceneView.duringSceneGui += OnListenInput;
    }

    private void Update()
    {
        OnListenInput(SceneView.currentDrawingSceneView);
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnListenInput;
    }

    private void OnListenInput(SceneView sceneView)
    {
        UnityEngine.Event e = UnityEngine.Event.current;

        if (e == null)
        {
            return;
        }

        switch (e.type)
        {
            case EventType.KeyDown:
                if (UnityEngine.Event.current.keyCode == KeyCode.F1)
                {
                    GroundSnap();
                }
                else if (UnityEngine.Event.current.keyCode == KeyCode.F2)
                {
                    DetectorControl();
                }
                break;
        }
    }
#endif

    #endregion



}

#endif