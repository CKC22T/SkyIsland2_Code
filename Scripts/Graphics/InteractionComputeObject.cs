using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Olympus
{
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
    public class InteractionComputeObject : MonoBehaviour
    {
        #region Internals

        #region path
        private readonly string encodedBufferPath = "Textures/EncodedInteractionBuffer";
        private readonly string encodedPreviousBufferPath = "Textures/EncodedPreviousInteractionBuffer";
        private readonly string deltaBufferPath = "Textures/DeltaInteractionBuffer";
        private readonly string trailBufferPath = "Textures/TrailInteractionBuffer";
        private readonly string backBufferPath = "Textures/TrailBackInteractionBuffer";

        #endregion

        #region shaderInfo
        /// <summary>
        /// 64 bits long
        /// </summary>
        public struct ObjectData
        {
            public ObjectData(Vector3 direction, float strength, Vector3 position, int method, float radius)
            {
                Direction = direction;
                Strength = strength;
                Position = position;
                Method = method;
                Radius = radius;
            }

            public Vector3 Direction; // 0000 0000 0000
            public float Strength; // 00 ~ 11
            public Vector3 Position; // 0000 0000 0000 0000 0000 0000 0000 0000 ~ 1111 1111 1111 1111 1111 1111 1111 1111
            public int Method; // 00 ~ 11
            public float Radius;
        }

        public struct ObjectRawData
        {
            public Vector3 Direction;
            public Vector3 Position;
        }
        struct VegetationInstance
        {
            public Vector3 worldPosition;
            public int prototypeIndex;
        };

        public struct VegetationParticleInfo
        {
            public Vector3 position;
            public float scale;
            public int prototypeIndex;
        }

        public struct InternalObjectData
        {
            public InternalObjectData(InteractableObject target)
            {
                Target = target;
                Position = Vector3.zero;
            }
            public InteractableObject Target;
            public Vector3 Position;
        }
        private int objectStride;
        private int objectRawStride;
        private int vegetationInstanceStride;

        VegetationParticleInfo[] overlapResults = new VegetationParticleInfo[32];
        private Queue<DamageEffectLevelEvent> preInteractionQueue = new();
        private Queue<DamageEffectLevelEvent> postInteractionQueue = new();
        public Queue<DamageEffectLevelEvent> InteractionQueue { get { return preInteractionQueue; } }
        private List<ObjectData> objectDatas = new();
        private List<ObjectRawData> objectRawDatas = new();
        private List<ObjectRawData> objectRawDatasPrevious = new();
        #endregion

        #region volume
        [SerializeField, TabGroup("Debug")] private Rigidbody volumeRigidbody;
        [SerializeField, TabGroup("Debug")] private BoxCollider volumeCollider;
        #endregion

        private ComputeShader shader;
        private RenderTexture encodedBuffer;
        private RenderTexture encodedPreviousBuffer;
        private RenderTexture trailBuffer;
        private RenderTexture backBuffer;
        private RenderTexture vegetationBuffer;
        private ComputeBuffer particleBuffer;

        private RenderTexture deltaBuffer;
        public RenderTexture EncodedTexture { get { return encodedBuffer; } }
        public RenderTexture EncodedPreviousTexture { get { return encodedPreviousBuffer; } }
        public RenderTexture DeltaTexture { get { return deltaBuffer; } }
        public RenderTexture TrailTexture { get { return trailBuffer; } }
        public RenderTexture BackbufferTexture { get { return backBuffer; } }
        public RenderTexture VegetationTexture { get { return vegetationBuffer; } }

        public ComputeBuffer ParticleBuffer { get { return particleBuffer; } }

        [SerializeField] GameObject[] filteredDetailPrefabs;

        private int resolution;
        private readonly int threadGroup = 32;
        protected enum ResolutionType
        {
            Small = 256,
            Medium = 512,
            Large = 1024,
            VeryLarge = 2048,
            Massive = 4096,
        }

        private int suppressCount = 0;

        private ComputeBuffer objectsBuffer;
        private ComputeBuffer objectsRawBuffer;
        private ComputeBuffer objectsRawBufferPrevious;
        //private ComputeBuffer vegetationInstanceBuffer;
        private List<ComputeBuffer> vegetationInstanceBuffers;

        static Dictionary<Terrain, List<KeyValuePair<int, GameObject>>> terrainPrototypeList = new();

        static Vector2Int GetResolutionFromType(ResolutionType type)
        {
            Vector2Int resolution = new Vector2Int((int)type, (int)type);

            return resolution;
        }

        #endregion

        #region Parameters
        [SerializeField, TabGroup("System"), LabelText("맵 해상도")] ResolutionType resolutionType = ResolutionType.Large;
        [SerializeField, TabGroup("System"), LabelText("시스템 크기")] float systemSize;
        [SerializeField, TabGroup("System"), LabelText("시스템 복원 속도")] float fadeRate;
        [SerializeField, TabGroup("System"), LabelText("시스템 적용 대상 목록")] List<InteractableObject> systemObjects = new();
        public List<InteractableObject> SystemObjects { get { return systemObjects; } }
        [SerializeField, TabGroup("System"), LabelText("중심점 갱신 쉐이더 목록")] List<Material> systemMaterials = new();
        [SerializeField, TabGroup("System"), LabelText("파티클 이펙트")] GameObject particlePrefab;
        public GameObject ParticlePrefab { get { return particlePrefab; } }

        #endregion

        void RegenerateTextureBuffer(string path, out RenderTexture target, RenderTextureFormat format)
        {
            target = Resources.Load(path) as RenderTexture;
            target.Release();
            target.enableRandomWrite = true;
            target.format = format;
            target.width = resolution;
            target.height = resolution;
            LogUtil.Assert(target.Create() == true);
        }
        protected virtual bool Init()
        {
            volumeCollider = GetComponent<BoxCollider>();
            volumeRigidbody = GetComponent<Rigidbody>();

            LogUtil.Assert(volumeCollider != null);
            LogUtil.Assert(volumeRigidbody != null);

            volumeRigidbody.isKinematic = true;
            volumeCollider.isTrigger = true;

            shader = Resources.Load("Shaders/GeneralInteractiveSystem/InteractiveComputeShader") as ComputeShader;
            LogUtil.Assert(shader != null);

            resolution = (int)resolutionType;

            for (int i = 0; i < systemObjects.Count; i++)
            {
                objectRawDatas.Add(new ObjectRawData());
                objectRawDatasPrevious.Add(new ObjectRawData());
            }

            RegenerateTextureBuffer(encodedBufferPath, out encodedBuffer, RenderTextureFormat.ARGBFloat);
            RegenerateTextureBuffer(encodedPreviousBufferPath, out encodedPreviousBuffer, RenderTextureFormat.ARGBFloat);
            RegenerateTextureBuffer(deltaBufferPath, out deltaBuffer, RenderTextureFormat.ARGBFloat);
            RegenerateTextureBuffer(trailBufferPath, out trailBuffer, RenderTextureFormat.ARGBHalf);
            RegenerateTextureBuffer(backBufferPath, out backBuffer, RenderTextureFormat.RGFloat);

            if (encodedBuffer == null)
            {
                return false;
            }
            if (encodedPreviousBuffer == null)
            {
                return false;
            }

            objectStride = Marshal.SizeOf<ObjectData>();
            objectRawStride = Marshal.SizeOf<ObjectRawData>();
            vegetationInstanceStride = Marshal.SizeOf<VegetationInstance>();

            objectsBuffer = new ComputeBuffer(1, objectStride);
            objectsRawBuffer = new ComputeBuffer(1, objectRawStride);
            objectsRawBufferPrevious = new ComputeBuffer(1, objectRawStride);
            RegenerateBuffer<ObjectData>(ref objectsBuffer, systemObjects.Count, objectStride);
            RegenerateBuffer<ObjectRawData>(ref objectsRawBuffer, systemObjects.Count, objectRawStride);
            RegenerateBuffer<VegetationParticleInfo>(ref particleBuffer, 32, Marshal.SizeOf<VegetationParticleInfo>());
            Vector3Int dispatchSize = new Vector3Int(resolution / threadGroup, resolution / threadGroup, systemObjects.Count);

            int initKernel = shader.FindKernel("InitializeInteractionData");

            shader.SetTexture(initKernel, "trailBuffer", trailBuffer);
            shader.SetTexture(initKernel, "backBuffer", backBuffer);
            shader.Dispatch(initKernel, dispatchSize.x, dispatchSize.y, 1);

            return true;
        }

        void RegenerateBuffer<T>(ref ComputeBuffer buffer, int count, int stride)
        {
            if (buffer == null)
            {
                buffer = new(count, stride);
            }

            if (count == 0 || buffer.count == 0)
            {
                buffer = null;
                return;
            }

            ComputeBuffer tempBuffer;

            tempBuffer = new ComputeBuffer(count, stride, ComputeBufferType.Default);

            int bufferSize = 0;
            if (buffer.count < count)
            {
                bufferSize = buffer.count;
            }
            else
            {
                bufferSize = count;
            }

            T[] tempArray = new T[bufferSize];

            buffer.GetData(tempArray);
            tempBuffer.SetData(tempArray);

            if (buffer?.IsValid() == true)
            {
                buffer?.Dispose();
                buffer?.Release();
                buffer = null;
            }

            buffer = new ComputeBuffer(count, stride);

            tempBuffer.GetData(tempArray);
            buffer.SetData(tempArray, 0, 0, tempArray.Length);

            tempBuffer.Dispose();
            tempBuffer.Release();
            tempBuffer = null;

            return;
        }

        void PreDispatch(List<ObjectData> objectDatas)
        {
            for (int i = 0; i < systemObjects.Count; i++)
            {
                if (systemObjects[i] == null)
                {
                    systemObjects.RemoveAt(i);
                    continue;
                }
                InteractableObject instance = systemObjects[i];
                Transform instanceTransform = systemObjects[i].transform;
                ObjectData instanceData = new ObjectData(instanceTransform.forward, 1.0f, instanceTransform.position, (int)instance.Type, instance.Radius);
                objectDatas.Add(instanceData);

                ObjectRawData rawInstanceData = new ObjectRawData();
                rawInstanceData.Position = instanceTransform.position;
                rawInstanceData.Direction = instanceTransform.forward;

                var temp = objectRawDatas[i];
                temp.Position = instanceTransform.position;
                temp.Direction = instanceTransform.forward;

                objectRawDatas[i] = temp;

                if (instance.transform == PlayerController.Instance.PlayerEntity.transform)
                {
                    LogUtil.Log(instance.transform.forward);
                }
            }

            if (objectsBuffer.count < objectDatas.Count)
            {
                RegenerateBuffer<ObjectData>(ref objectsBuffer, objectDatas.Count, objectStride);
            }

            objectsBuffer.SetData(objectDatas);

            shader.SetInt("_Width", (int)resolution);
            shader.SetInt("_Height", (int)resolution);
            shader.SetInt("_SystemSize", (int)systemSize);
            shader.SetVector("_CenterPosition", transform.position);
            shader.SetFloat("_DeltaTime", Time.deltaTime);
            shader.SetFloat("_FadeRate", fadeRate);
        }

        void PostDispatch()
        {
            for (int i = 0; i < systemObjects.Count; i++)
            {
                if (systemObjects[i] == null)
                {
                    systemObjects.RemoveAt(i);
                    continue;
                }

                Transform instance = systemObjects[i].transform;

                var temp = objectRawDatasPrevious[i];
                temp.Position = instance.position;
                temp.Direction = instance.forward;

                objectRawDatasPrevious[i] = temp;
            }

            objectDatas.Clear();
            shader.SetVector("_PreviousCenterPosition", gameObject.transform.position);
        }

        protected virtual void Draw()
        {
            Vector3Int dispatchSize = new Vector3Int(resolution / threadGroup, resolution / threadGroup, systemObjects.Count);

            int clearKernel = shader.FindKernel("ClearInteractionData");



            //for (int i = 0; i < overlapResults.Length; i++)
            //{
            //    overlapResults[i] = 0;
            //}

            shader.SetBuffer(clearKernel, "input", objectsBuffer);

            shader.SetTexture(clearKernel, "encodedResult", encodedBuffer);
            shader.SetTexture(clearKernel, "encodedResultPrevious", encodedPreviousBuffer);
            shader.SetTexture(clearKernel, "deltaBuffer", deltaBuffer);
            shader.SetTexture(clearKernel, "trailBuffer", trailBuffer);
            shader.SetTexture(clearKernel, "backBuffer", backBuffer);
            shader.SetBuffer(clearKernel, "vegetationParticleBuffer", particleBuffer);

            shader.Dispatch(clearKernel, dispatchSize.x, dispatchSize.y, 1);

            shader.SetBuffer(0, "input", objectsBuffer);
            shader.SetTexture(0, "encodedResult", encodedBuffer);
            shader.SetTexture(0, "encodedResultPrevious", encodedPreviousBuffer);
            shader.SetTexture(0, "deltaBuffer", deltaBuffer);
            shader.SetTexture(0, "trailBuffer", trailBuffer);
            shader.SetTexture(0, "backBuffer", backBuffer);
            shader.Dispatch(0, dispatchSize.x, dispatchSize.y, dispatchSize.z);

            //PostProcessPendingInteractionQueue();


        }

        private void Awake()
        {
            Init();
            //GenerateVegetationMap(resolution, resolution);
        }

        private void Update()
        {
            volumeCollider.size = new Vector3(systemSize, systemSize, systemSize);

            for (int i = 0; i < systemMaterials.Count; i++)
            {
                systemMaterials[i].SetVector("_CenterPosition", transform.position);
            }
            PreProcessPendingInteractionQueue();

            if (systemObjects[0].UpdateSupressing == true)
            {
                if (suppressCount >= systemObjects[0].UpdateInterval)
                {
                    PreDispatch(objectDatas);

                    Draw();

                    PostDispatch();

                    suppressCount = 0;
                }

                suppressCount++;
            }
            else
            {
                PreDispatch(objectDatas);

                Draw();

                PostDispatch();

            }


        }

        readonly string vegetationMapPath = "Textures/VegetationBuffer";

        void GenerateVegetationMap(int width, int height)
        {
            ComputeShader targetCS = Resources.Load<ComputeShader>("Shaders/GeneralInteractiveSystem/VegetationMapGeneration");
            Debug.Assert(targetCS != null);

            var terrains = GameObject.FindObjectsOfType<Terrain>();

            List<DetailedObjectInstance[]> detailMeshInstances = new();
            List<VegetationInstance> vegetationInstances = new();

            int detailMeshCount = 0;
            for (int i = 0; i < terrains.Length; i++)
            {
                detailMeshInstances.Add(DetailedObjectInstance.ExportObjects(this, terrains[i]));
                detailMeshCount += detailMeshInstances[i].Length;

                terrainPrototypeList.Add(terrains[i], new());
                List<KeyValuePair<int, GameObject>> pairList = terrainPrototypeList[terrains[i]];

                foreach (var detail in detailMeshInstances[i])
                {
                    VegetationInstance inst = new();
                    inst.prototypeIndex = detail.PrototypePair.Key;
                    inst.worldPosition = detail.Position;

                    pairList.Add(detail.PrototypePair);

                    vegetationInstances.Add(inst);
                }
            }

            vegetationBuffer = Resources.Load<RenderTexture>(vegetationMapPath);
            if (vegetationBuffer == null)
            {
                return;
            }

            vegetationBuffer.Release();
            vegetationBuffer.width = width;
            vegetationBuffer.height = height;
            vegetationBuffer.format = RenderTextureFormat.ARGBHalf;
            vegetationBuffer.enableRandomWrite = true;

            if (vegetationBuffer.Create() == false)
            {
                Debug.LogError("Failed to create vegetation map");
                return;
            }

            int instancePerChunk = 8;
            int chunkSize = vegetationInstanceStride * instancePerChunk;
            int totalSize = detailMeshCount * vegetationInstanceStride;
            int chunkCount = totalSize / chunkSize;

            List<List<VegetationInstance>> chunkVegetationInstances = new List<List<VegetationInstance>>(chunkCount);

            for (int i = 0; i < chunkCount; i++)
            {
                chunkVegetationInstances.Add(new List<VegetationInstance>());
                for (int j = 0; j < instancePerChunk; j++)
                {
                    int globalIndex = (i * instancePerChunk) + j;
                    chunkVegetationInstances[i].Add(vegetationInstances[globalIndex]);
                }
            }

            vegetationInstanceBuffers = new List<ComputeBuffer>();

            for (int i = 0; i < chunkCount; i++)
            {
                ComputeBuffer tempBuffer = new ComputeBuffer(instancePerChunk, vegetationInstanceStride);

                RegenerateBuffer<VegetationInstance>(ref tempBuffer, instancePerChunk, vegetationInstanceStride);

                vegetationInstanceBuffers.Add(tempBuffer);
                vegetationInstanceBuffers[i].SetData<VegetationInstance>(chunkVegetationInstances[i]);

                targetCS.SetTexture(0, "vegetationMap", vegetationBuffer);
                targetCS.SetBuffer(0, "vegetationInstances", vegetationInstanceBuffers[i]);
                targetCS.SetVector("_CenterPosition", transform.position);
                targetCS.SetInt("_Resolution", resolution);
                targetCS.SetInt("_InstanceCount", vegetationInstanceBuffers[i].count);
                uint dispatchXSize, dispatchYSize, dispatchZSize;

                targetCS.GetKernelThreadGroupSizes(0, out dispatchXSize, out dispatchYSize, out dispatchZSize);
            //    targetCS.Dispatch(0, resolution / (int)dispatchXSize, resolution / (int)dispatchYSize, vegetationInstanceBuffers[i].count / (int)dispatchZSize);
            }
            for (int i = 0; i < chunkCount; i++)
            {
                vegetationInstanceBuffers[i].Dispose();
            }

        }

        void PreProcessPendingInteractionQueue()
        {
            while (preInteractionQueue.Count > 0)
            {

                var instance = preInteractionQueue.Dequeue();

                instance.EffectOnVegetation();

                postInteractionQueue.Enqueue(instance);
            }
        }

        void PostProcessPendingInteractionQueue()
        {
            while (postInteractionQueue.Count > 0)
            {
                var instance = postInteractionQueue.Dequeue();

                instance.ParticleIntegration();

            }

        }

        public VegetationParticleInfo RequestOverlapTest(int requestId)
        {

            int overlapKernel = shader.FindKernel("OverlapTest");
            uint dispatchXSize, dispatchYSize, dispatchZSize;

            overlapResults[requestId] = new VegetationParticleInfo();

            particleBuffer.SetData(overlapResults);

            shader.GetKernelThreadGroupSizes(overlapKernel, out dispatchXSize, out dispatchYSize, out dispatchZSize);

            shader.SetTexture(overlapKernel, "encodedResult", encodedBuffer);
            shader.SetTexture(overlapKernel, "vegetationMap", vegetationBuffer);
            shader.SetBuffer(overlapKernel, "vegetationParticleBuffer", particleBuffer);
        //    shader.Dispatch(overlapKernel, resolution / (int)dispatchXSize, resolution / (int)dispatchYSize, requestId + 1);

            particleBuffer.GetData(overlapResults);

            return overlapResults[requestId];
        }

        public void AddSystemObject(InteractableObject instance)
        {
            systemObjects.Add(instance);
            objectRawDatas.Add(new ObjectRawData());
            objectRawDatasPrevious.Add(new ObjectRawData());

            PreDispatch(objectDatas);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer.Equals(LayerData.LAYER_ENEMY))
            {
                InteractableObject interactComponent;
                if (other.gameObject.TryGetComponent<InteractableObject>(out interactComponent) == false)
                {
                    interactComponent = other.gameObject.AddComponent<InteractableObject>();
                }

                systemObjects.Add(interactComponent);
                objectRawDatas.Add(new ObjectRawData());
                objectRawDatasPrevious.Add(new ObjectRawData());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer.Equals(LayerData.LAYER_ENEMY))
            {
                InteractableObject interactComponent;
                if (other.gameObject.TryGetComponent<InteractableObject>(out interactComponent) == true)
                {
                    if (systemObjects.Contains(interactComponent) == true)
                    {
                        systemObjects.Remove(interactComponent);
                    }
                }

            }
        }


#if UNITY_EDITOR

        private void OnGUI()
        {
            //            GUI.DrawTexture()
        }
        private void OnDrawGizmos()
        {
            if (volumeCollider != null)
            {
                volumeCollider.size = new Vector3(systemSize, systemSize, systemSize);
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(transform.position, volumeCollider.size);
            }
            else
            {
                volumeCollider = GetComponent<BoxCollider>();
            }
        }

#endif

        [System.Serializable]
        public class DetailedObjectInstance
        {
            public Vector3 Position;
            public Vector3 Scale;
            public KeyValuePair<int, GameObject> PrototypePair;

            public static DetailedObjectInstance[] ExportObjects(InteractionComputeObject root, Terrain terrain)
            {
                List<DetailInstanceTransform[]> detailTransforms = new();
                List<DetailedObjectInstance> output = new List<DetailedObjectInstance>();

                TerrainData data = terrain.terrainData;
                if (terrain.detailObjectDensity != 0)
                {

                    int detailWidth = data.detailWidth;
                    int detailHeight = data.detailHeight;


                    float delatilWToTerrainW = data.size.x / (float)detailWidth;
                    float delatilHToTerrainH = data.size.z / (float)detailHeight;

                    Vector3 mapPosition = terrain.transform.position;

                    float targetDentisty = 0;
                    if (terrain.detailObjectDensity != 1)
                    {
                        targetDentisty = (1.0f / (1f - terrain.detailObjectDensity));
                    }

                    DetailPrototype[] details = data.detailPrototypes;
                    for (int i = 0; i < details.Length; i++)
                    {
                        if (root.filteredDetailPrefabs.Contains(details[i].prototype) == true)
                        {
                            continue;
                        }
                        KeyValuePair<int, GameObject> pair = new KeyValuePair<int, GameObject>(i, details[i].prototype);

                        float minWidth = details[i].minWidth;
                        float maxWidth = details[i].maxWidth;

                        float minHeight = details[i].minHeight;
                        float maxHeight = details[i].maxHeight;

                        int[,] map = data.GetDetailLayer(0, 0, data.detailWidth, data.detailHeight, 0);

                        List<Vector3> grasses = new List<Vector3>();

                        int patchCount = data.detailResolution / data.detailResolutionPerPatch;
                        for (int y = 0; y < patchCount; y++)
                        {
                            for (int x = 0; x < patchCount; x++)
                            {
                                Bounds bound;
                                var instanceTransforms = terrain.terrainData.ComputeDetailInstanceTransforms(x, y, i, terrain.detailObjectDensity, out bound);

                                detailTransforms.Add(instanceTransforms);
                            }
                        }

                        foreach (var list in detailTransforms)
                        {
                            foreach (var inst in list)
                            {
                                DetailedObjectInstance e = new DetailedObjectInstance();

                                e.Position = new Vector3(inst.posX, inst.posY, inst.posZ) + mapPosition;
                                e.Scale = new Vector3(inst.scaleXZ, inst.scaleY, inst.scaleXZ);
                                e.PrototypePair = new KeyValuePair<int, GameObject>(pair.Key, pair.Value);
                                output.Add(e);
                            }
                        }
                    }
                }

                return output.ToArray();
            }
        }
    }
}