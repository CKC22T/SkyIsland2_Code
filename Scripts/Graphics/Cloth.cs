using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Olympus
{
    public class Cloth : MonoBehaviour
    {
        public struct VertexAnimationInfo
        {
            public float ClosestDistance;
        }
        public struct ControlPoint
        {
            Vector3 Velocity;
            Vector3 Rotation;

            Vector3 BoundMin;
            Vector3 BoundMax;

            Matrix4x4 WorldMatrix;
            Matrix4x4 InverseWorldMatrix;
        }
        struct VirtualControlPointConnection
        {
            public int From;
            public int To;

            public float Stiffness;
            public float Threshold;
            public float Stress;
        };

        public struct AABB
        {
            public AABB(Vector3 minimum, Vector3 maximum, Vector3 cent,
                Matrix4x4 inverseMat, Matrix4x4 worldMat, Matrix4x4 rot, Matrix4x4 invRot)
            {
                min = minimum;
                max = maximum;
                center = cent;
                inverseWorldMatrix = inverseMat;
                worldMatrix = worldMat;
                rotationMatrix = rot;
                inverseRotationMatrix = invRot;
            }
            public Vector3 min;
            public Vector3 max;
            public Vector3 center;
            public Matrix4x4 inverseWorldMatrix;
            public Matrix4x4 worldMatrix;
            public Matrix4x4 rotationMatrix;
            public Matrix4x4 inverseRotationMatrix;
        }

        class ComputeBufferResource
        {
            public ComputeBufferResource(int count, int stride, string name)
            {
                buffer = new ComputeBuffer(count, stride);
                bufferId = Shader.PropertyToID(name);
            }

            public bool IsValid {
                get { return buffer != null; }
            }

            public void Reset()
            {
                buffer?.Dispose();
                buffer = null;
            }

            public ComputeBuffer buffer;
            public int bufferId;
            public int stride;
            public int count;
        }

        public int segments;
        public int applySegments;

        int kernelVCPGeneration;
        int kernelVCPSimulation;
        int kernelVertexAnimation;

     //   [StructLayout(LayoutKind.Sequential, Pack = 16)]
        public struct GlobalControlPointProperties
        {
            public Vector4 MinimumPointInWorldSpace;
            public Vector4 MaximumPointInWorldSpace;
            public Matrix4x4 InverseTransform;
            public Matrix4x4 InverseTranslation;
            public Matrix4x4 InverseRotation;
            public Matrix4x4 WorldMatrix;
            public int Segments;
            public int CollisionCount;
            public int _unused1;
            public int _unused2;
            public float DeltaTime;
            public float _unused3;
            public float _unused4;
            public float _unused5;
        }

        struct ShaderCollider
        {
            public Vector4 Position;
            public Vector4 Offset;
            public Vector4 Scale;
            public Vector4 Rotation;
            public Matrix4x4 Transform;
            public Matrix4x4 InversedTransform;

            public float Radius;
            public float Height;
            public Vector3 Size;
            public int Type;
        };

        private Vector3 previousPosition;
        private Vector3 delta;
        private Vector4[] updatedVCPPositions;
        private VirtualControlPointConnection[] updatedVCPConnections;
        private Vector3[] updatedVertices;

        ControlPoint[] massPointResult;
        List<GlobalControlPointProperties> gcpp = new(1);

        private Material mufflerMaterial;
        private ComputeShader vcpCS;
        private ComputeShader animationCS;
        // private ComputeBuffer massPointBuffer;
        //private ComputeBuffer verticesBuffer;
        private ComputeBuffer sceneCollisionBuffer;
        private ComputeBufferResource rawControlPointBuffer;
        private ComputeBufferResource controlPointPropertyBuffer;
        private ComputeBufferResource controlPointEdgeBuffer;
        private ComputeBufferResource gcppBuffer;
        private ComputeBufferResource collisionBuffer;
        private ComputeBufferResource animationInfoBuffer;
        private ComputeBufferResource vertexBuffer;

        [SerializeField] private List<Collider> collisions = new();
        private Dictionary<Collider, ShaderCollider> colliderData = new();
        private MeshFilter targetFilter;

        private List<VertexAnimationInfo> animInfos = new();

        private Mesh meshBuffer;
        GlobalControlPointProperties gcppRaw = new();

        private int physicsPropertiesStride = -1;
        private int vertexStride = -1;
        private int controlPointPropertyStride = -1;
        private int aabbStride = -1;
        private int gcppStride = -1;
        private int edgeStride = -1;

        Vector3 meshBoundMinimum;
        Vector3 meshBoundMaximum;

        private void GenerateVCP()
        {
            gcppRaw.DeltaTime = Time.deltaTime;
            gcppRaw.Segments = segments;
            gcppRaw.MinimumPointInWorldSpace = meshBoundMinimum;
            gcppRaw.MaximumPointInWorldSpace = meshBoundMaximum;
            gcppRaw.WorldMatrix = transform.localToWorldMatrix.transpose;
            gcppRaw.InverseTransform = transform.worldToLocalMatrix.transpose;

            gcpp[0] = gcppRaw;

            vcpCS.SetBuffer(kernelVCPGeneration, rawControlPointBuffer.bufferId, rawControlPointBuffer.buffer);
            vcpCS.SetBuffer(kernelVCPGeneration, controlPointPropertyBuffer.bufferId, controlPointPropertyBuffer.buffer);
            vcpCS.SetBuffer(kernelVCPGeneration, controlPointEdgeBuffer.bufferId, controlPointEdgeBuffer.buffer);

            vcpCS.Dispatch(kernelVCPGeneration, 16, 1, 1);
            rawControlPointBuffer.buffer.GetData(updatedVCPPositions);
            controlPointEdgeBuffer.buffer.GetData(updatedVCPConnections);
        }

        private void SimulateVCP()
        {
            rawControlPointBuffer.buffer.SetData(updatedVCPPositions);

            vcpCS.Dispatch(kernelVCPSimulation, 16, 16, 1);
            rawControlPointBuffer.buffer.GetData(updatedVCPPositions);
            controlPointEdgeBuffer.buffer.GetData(updatedVCPConnections);
        }

        private void AnimateVertices()
        {
            animationCS.SetBuffer(kernelVertexAnimation, animationInfoBuffer.bufferId, animationInfoBuffer.buffer);
            animationCS.SetBuffer(kernelVertexAnimation, vertexBuffer.bufferId, vertexBuffer.buffer);
            animationCS.SetBuffer(kernelVertexAnimation, rawControlPointBuffer.bufferId, rawControlPointBuffer.buffer);

            animationCS.Dispatch(kernelVertexAnimation, 16, 1, 1);

            vertexBuffer.buffer.GetData(updatedVertices);

            meshBuffer.vertices = updatedVertices;
        }

        void UpdateSubresources()
        {
            gcppRaw.DeltaTime = Time.deltaTime;
            gcppRaw.Segments = applySegments;
            gcppRaw.MinimumPointInWorldSpace = meshBoundMinimum;
            gcppRaw.MaximumPointInWorldSpace = meshBoundMaximum;
            gcppRaw.WorldMatrix = transform.localToWorldMatrix.transpose;
            
            gcpp[0] = gcppRaw;

            gcppBuffer.buffer.SetData(gcpp);
         //   rawControlPointBuffer.buffer.SetData(updatedPositions);
         //   controlPointEdgeBuffer.buffer.SetData(updatedConnections);

            vcpCS.SetConstantBuffer(gcppBuffer.bufferId, gcppBuffer.buffer, 0, gcppStride);

            vcpCS.SetBuffer(kernelVCPSimulation, rawControlPointBuffer.bufferId, rawControlPointBuffer.buffer);
            vcpCS.SetBuffer(kernelVCPSimulation, controlPointPropertyBuffer.bufferId, controlPointPropertyBuffer.buffer);
            vcpCS.SetBuffer(kernelVCPSimulation, controlPointEdgeBuffer.bufferId, controlPointEdgeBuffer.buffer);
            vcpCS.SetBuffer(kernelVCPSimulation, collisionBuffer.bufferId, collisionBuffer.buffer);
        }

        void PostUpdateSubresources()
        {
            gcppRaw.InverseTransform = transform.worldToLocalMatrix.transpose;
            gcpp[0] = gcppRaw;
        }


        void ValidateSegments()
        {
            if (segments != applySegments)
            {
                applySegments = segments;
                updatedVCPPositions = new Vector4[applySegments];
                updatedVCPConnections = new VirtualControlPointConnection[applySegments - 1];

                rawControlPointBuffer?.Reset();
                rawControlPointBuffer = new ComputeBufferResource(applySegments, vertexStride, "ControlPoints");

                controlPointPropertyBuffer?.Reset();
                controlPointPropertyBuffer = new ComputeBufferResource(applySegments, controlPointPropertyStride, "ControlPointsProperties");

                controlPointEdgeBuffer?.Reset();
                controlPointEdgeBuffer = new ComputeBufferResource(applySegments - 1, edgeStride, "ControlPointEdges");


                GenerateVCP();
            }

        }

        void ApplicationInit()
        {
            targetFilter = GetComponent<MeshFilter>();

            meshBuffer = targetFilter.mesh;

            meshBoundMinimum = meshBuffer.bounds.min;
            meshBoundMaximum = meshBuffer.bounds.max;

            collisionBuffer = new ComputeBufferResource(64, Marshal.SizeOf<ShaderCollider>(), "SceneCollisions");

            animInfos = new List<VertexAnimationInfo>(meshBuffer.vertexCount);
            animationInfoBuffer = new ComputeBufferResource(meshBuffer.vertexCount, Marshal.SizeOf<VertexAnimationInfo>(), "vertexAnimationInfos");
            vertexBuffer = new ComputeBufferResource(meshBuffer.vertexCount, Marshal.SizeOf<Vector3>(), "outVertices");
            updatedVertices = meshBuffer.vertices;
        }

        void UpdateCollisions()
        {
            foreach(var c in collisions)
            {
                Type type = c.GetType();
                ShaderCollider shaderCollider = colliderData[c];

                shaderCollider.Position = c.transform.position;
                shaderCollider.Transform = c.transform.localToWorldMatrix;
                shaderCollider.InversedTransform = c.transform.worldToLocalMatrix;
                shaderCollider.Rotation = c.transform.rotation.eulerAngles;
                shaderCollider.Scale = c.transform.localScale;
                if(type == typeof(SphereCollider))
                {
                    shaderCollider.Type = 0;
                    SphereCollider collider = c as SphereCollider;
                    shaderCollider.Radius = collider.radius;
                    shaderCollider.Offset = collider.center;

                }
                else if(type == typeof(BoxCollider))
                {
                    shaderCollider.Type = 1;
                    BoxCollider collider = c as BoxCollider;
                    shaderCollider.Size = collider.size;
                    shaderCollider.Offset = collider.center;
                }
                else if(type == typeof(CapsuleCollider))
                {
                    shaderCollider.Type = 2;
                    CapsuleCollider collider = c as CapsuleCollider;
                    shaderCollider.Height = collider.height;
                    shaderCollider.Radius = collider.radius;
                    shaderCollider.Offset = collider.center;
                }
                else
                {
                    shaderCollider.Type = 3;
                    MeshCollider collider = c as MeshCollider;
                    LogUtil.LogError("Not supported yet.");
                    // mesh, not supported yet.
                }

                colliderData[c] = shaderCollider;
            }

            gcppRaw.CollisionCount = collisions.Count;
            collisionBuffer.buffer.SetData(colliderData.Values.ToArray());
        }

        void Start()
        {
            ApplicationInit();

            vcpCS = Instantiate(Resources.Load("Shaders/MufflerVirtualControlPointCS")) as ComputeShader;
            animationCS = Instantiate(Resources.Load("Shaders/MufflerAnimationCS")) as ComputeShader;

            gcppStride = Marshal.SizeOf<GlobalControlPointProperties>();
            vertexStride = Marshal.SizeOf<Vector4>();
            controlPointPropertyStride = Marshal.SizeOf<ControlPoint>();
            edgeStride = Marshal.SizeOf<VirtualControlPointConnection>();

            kernelVCPGeneration = vcpCS.FindKernel("GenerateVCP");
            kernelVCPSimulation = vcpCS.FindKernel("SimulateVCP");
            kernelVertexAnimation = animationCS.FindKernel("KernelVertexAnimation");

            gcpp.Add(new GlobalControlPointProperties());

            ValidateSegments();

            gcppBuffer = new ComputeBufferResource(1, gcppStride, "VCPProperties");

            UpdateSubresources();
            vcpCS.SetBuffer(kernelVCPGeneration, rawControlPointBuffer.bufferId, rawControlPointBuffer.buffer);
            vcpCS.SetBuffer(kernelVCPGeneration, controlPointPropertyBuffer.bufferId, controlPointPropertyBuffer.buffer);
            vcpCS.SetBuffer(kernelVCPGeneration, controlPointEdgeBuffer.bufferId, controlPointEdgeBuffer.buffer);

            GenerateVCP();
            PostUpdateSubresources();
        }

        private void OnDestroy()
        {
            controlPointPropertyBuffer.Reset();
            rawControlPointBuffer.Reset();
            gcppBuffer.Reset();
        }

        void Update()
        {
            ValidateSegments();

            UpdateSubresources();
            UpdateCollisions();

            vcpCS.SetBuffer(kernelVCPSimulation, rawControlPointBuffer.bufferId, rawControlPointBuffer.buffer);
            vcpCS.SetBuffer(kernelVCPSimulation, controlPointPropertyBuffer.bufferId, controlPointPropertyBuffer.buffer);
            vcpCS.SetBuffer(kernelVCPSimulation, controlPointEdgeBuffer.bufferId, controlPointEdgeBuffer.buffer);

            SimulateVCP();
            AnimateVertices();

            PostUpdateSubresources();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < applySegments; i++)
            {
                Gizmos.DrawWireSphere(updatedVCPPositions[i], 0.25f);
            }

            Gizmos.color = Color.cyan;
            for (int i = 0; i < applySegments - 1; i++)
            {
                int from = updatedVCPConnections[i].From;
                int to = updatedVCPConnections[i].To;

                Handles.DrawLine(updatedVCPPositions[from], updatedVCPPositions[to], 3.0f); 
            }
        }
#endif

        private void OnTriggerEnter(Collider other)
        {
            if(collisions.Contains(other) == true)
            {
                return;
            }

            collisions.Add(other);
            colliderData.Add(other, new ShaderCollider());

            var type = other.GetType();

            LogUtil.Log(type.ToString());

        }
        private void OnTriggerExit(Collider other)
        {
            if (collisions.Contains(other) == false)
            {
                return;
            }

            collisions.Remove(other);
            colliderData.Remove(other);
        }
    }
}