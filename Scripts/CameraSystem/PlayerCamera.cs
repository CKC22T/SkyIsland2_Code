using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using Mono.Cecil;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Olympus
{
    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : LocalSingletonBase<PlayerCamera>
    {


        [System.Serializable]
        public class CameraShakeDescriptor
        {
            public AnimationCurve attenuationCurve;

            public float duration;
            public float magnitude;
            public float frequency;
        }

        [SerializeField] private CameraStateData defaultState;
        public CameraStateData SelectedState;
        public int sequenceIndex { get; private set; }
        [SerializeField, Sirenix.OdinInspector.ReadOnly] private int sequenceIndexDebug;

        public List<SerializablePair<Vector3, Vector3>> shiftOffsets = new();

        public Transform trackingTarget;
        public Transform[] TrackingObjectList;

        public AnimationCurve offsetInterpolationCurve;
        public AnimationCurve rotationalInterpolationCurve;
        public AnimationCurve fieldOfViewInterpolationCurve;
        public AnimationCurve shiftInterpolationCurve;

        public float cameraZoomSmoothness = 2.0f;
        public float scrollSensitivty = 0.5f;

        public const float cameraResponseScale = 100.0f;

        [Range(0.0f, 1.0f)]
        public float cameraResponse = 0.5f * cameraResponseScale;

        private Camera cameraInstance;

        private Vector3 previousTargetPosition;

        private float scrollValue = 1;
        private float scrollIntegral = 0;

        private Vector3 targetDelta;

        private DepthOfField dofInstance;
        public Volume globalVolume = null;

        public bool IsCameraShifted { get; private set; }
        public bool VelocityTracking;
        public int SelectedShiftIndex { get; set; }


        public SerializableDictionary<string, CameraShakeDescriptor> cameraShakeList = new();

        public void SetState(CameraStateData data, int sequence = 0)
        {
            SelectedState = data;
            sequenceIndex = sequenceIndex;
        }
        public void ReleaseState()
        {
            SelectedState = null;
            sequenceIndex = 0;
        }

        public new void Awake()
        {
            base.Awake();
            cameraInstance = Camera.main;
            VelocityTracking = true;

            //defaultState = Resources.Load("GameData/CameraStates/Default") as CameraStateData;

        }

        public void Start()
        {
            globalVolume.profile.TryGet<DepthOfField>(out dofInstance);
        }

        private void OnEnable()
        {
            //cameraShakeList.AddRange(cameraShakeListInternal.ToArray());
            //cameraShakeListInternal.Clear();

            if(dofInstance != null)
            {
                dofInstance.focalLength.value = 300.0f;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            PlayerCameraExtension.instance?.GenerateWaves();
        }

#endif
        private void OnDestroy()
        {
            //cameraShakeList.Clear();
            //cameraShakeList.AddRange(cameraShakeListInternal.ToArray());
        }

        public void Tracking(float delta)
        {
            if (defaultState.sequences[0].target == null)
            {
                defaultState.sequences[0].target = PlayerController.Instance.PlayerEntity.transform;
            }
            if (defaultState.sequences[1].target == null)
            {
                defaultState.sequences[1].target = PlayerController.Instance.PlayerEntity.transform;
            }

            if (cameraInstance == null)
            {
                cameraInstance = Camera.main;
            }
            Transform cameraTransform = cameraInstance.transform;

            targetDelta = trackingTarget.position - previousTargetPosition;

            float derivative = (Mathf.Clamp(Input.mouseScrollDelta.y, -1, 1) * delta) * scrollSensitivty;

            scrollIntegral -= derivative;

            scrollValue += scrollIntegral / cameraZoomSmoothness;
            scrollValue = Mathf.Clamp01(scrollValue);

            float offsetInterpolationFactor = offsetInterpolationCurve.Evaluate(scrollValue);
            float rotationalInterpolationFactor = rotationalInterpolationCurve.Evaluate(scrollValue);
            float fieldOfViewInterpolationFactor = fieldOfViewInterpolationCurve.Evaluate(scrollValue);
            float shiftInterpolationFactor = shiftInterpolationCurve.Evaluate(scrollValue);

            CameraStateData.Sequence nearest = defaultState.sequences[0];
            CameraStateData.Sequence farthest = defaultState.sequences[1];

            Vector3 offset = Vector3.Lerp(nearest.offset, farthest.offset, offsetInterpolationFactor);

            if (IsCameraShifted == true)
            {
                Vector3 shift = Vector3.Lerp(shiftOffsets[SelectedShiftIndex].Left, shiftOffsets[SelectedShiftIndex].Right, shiftInterpolationFactor);
                offset += shift;
            }

            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(cameraTransform.rotation);
            offset = rotationMatrix.MultiplyPoint(offset);

            Vector3 resultPosition = offset;
            resultPosition += trackingTarget.position;

            Quaternion begin = Quaternion.Euler(nearest.eulerRotation);
            Quaternion end = Quaternion.Euler(farthest.eulerRotation);
            Quaternion targetRot = Quaternion.Slerp(begin, end, rotationalInterpolationFactor);

            float targetFov = Mathf.Lerp(nearest.fieldOfView, farthest.fieldOfView, fieldOfViewInterpolationFactor);

            float response = delta * (cameraResponse * cameraResponseScale);

            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.position, resultPosition, response);
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRot, response);
            cameraInstance.fieldOfView = Mathf.Lerp(cameraInstance.fieldOfView, targetFov, response);

            scrollIntegral = Mathf.Lerp(scrollIntegral, 0.0f, (1.0f / cameraZoomSmoothness));
            float targetDistance = Vector3.Distance(transform.position, trackingTarget.position);

            if (Mathf.Abs(scrollIntegral) < Mathf.Epsilon)
            {
                scrollIntegral = 0.0f;
            }

            previousTargetPosition = trackingTarget.position;

            if (dofInstance != null)
            {
                dofInstance.focusDistance.value = targetDistance;
            //    dofInstance.focalLength.value = 360.0f / (Mathf.Deg2Rad * cameraInstance.fieldOfView * 2);
            }
        }

        public void PlaySequence(float delta, CameraStateData data, int sequenceIndex)
        {
            if (cameraInstance == null)
            {
                cameraInstance = Camera.main;
            }

            Transform cameraTransform = cameraInstance.transform;
            CameraStateData.Sequence sequence = data.sequences[sequenceIndex];
            float targetDistance = Vector3.Distance(transform.position, sequence.target.position);

            if (sequence.sequenceType == CameraStateData.SequenceType.Time)
            {
                if (sequence.useAdvanced == true)
                {
                    float response = delta * (sequence.response * cameraResponseScale);

                    float normalizedTime = sequence.sequenceTimer.Current / sequence.sequenceTimer.Target;

                    float xEvaluation = sequence.xOffsetCurve.Evaluate(normalizedTime);
                    float yEvaluation = sequence.yOffsetCurve.Evaluate(normalizedTime);
                    float zEvaluation = sequence.zOffsetCurve.Evaluate(normalizedTime);

                    float rotationEvaluation = sequence.rotationCurve.Evaluate(normalizedTime);

                    Quaternion initialRot = Quaternion.Euler(sequence.InitialRotaion);
                    Quaternion endRot = Quaternion.Euler(sequence.EndRotation);
                    Quaternion rot = Quaternion.SlerpUnclamped(initialRot, endRot, rotationEvaluation);

                    Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rot);

                    Vector3 initialOffset = sequence.InitialPosition;
                    initialOffset = rotationMatrix.MultiplyPoint(initialOffset);

                    Vector3 endOffset = sequence.EndPosition;
                    endOffset = rotationMatrix.MultiplyPoint(endOffset);

                    float x = Mathf.LerpUnclamped(initialOffset.x, endOffset.x, xEvaluation);
                    float y = Mathf.LerpUnclamped(initialOffset.y, endOffset.y, yEvaluation);
                    float z = Mathf.LerpUnclamped(initialOffset.z, endOffset.z, zEvaluation);

                    Vector3 resultPosition = new Vector3(x, y, z);

                    float targetFov = sequence.fieldOfView;

                    cameraTransform.localPosition = resultPosition; //Vector3.Lerp(sequence.InitialPosition, sequence.EndPosition, response);
                    cameraTransform.rotation = rot;
                    cameraInstance.fieldOfView = Mathf.Lerp(cameraInstance.fieldOfView, targetFov, response);

                }
            }
            else
            {
                float response = delta * (sequence.response * cameraResponseScale);

                Vector3 offset = sequence.offset;
                Matrix4x4 rotationMatrix = Matrix4x4.Rotate(cameraTransform.rotation);
                offset = rotationMatrix.MultiplyPoint(offset);

                Vector3 resultPosition = offset;
                resultPosition += sequence.target.position;
                cameraTransform.localPosition = Vector3.Lerp(cameraTransform.position, resultPosition, response);

                Quaternion targetRot = Quaternion.Euler(sequence.eulerRotation);

                float targetFov = sequence.fieldOfView;

                cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRot, response);
                cameraInstance.fieldOfView = Mathf.Lerp(cameraInstance.fieldOfView, targetFov, response);
            }

            sequenceIndexDebug = sequenceIndex;

            if (dofInstance != null)
            {
                dofInstance.focusDistance.value = targetDistance;
            }
        }

        public void SetCameraShift(bool flag, int index = 0)
        {
            IsCameraShifted = flag;

            if (flag == false)
            {
                return;
            }

            SelectedShiftIndex = index;
        }

        private void LateUpdate()
        {
            if (trackingTarget == null)
            {
                return;
            }

            if (SelectedState != null)
            {
                CameraStateData.Sequence currentSequence = SelectedState.sequences[sequenceIndex];
                switch (currentSequence.sequenceType)
                {
                    case CameraStateData.SequenceType.Input:
                        if (Input.GetKeyDown(currentSequence.keyCode))
                        {
                            if (sequenceIndex < SelectedState.sequences.Length)
                            {
                                sequenceIndex++;
                            }
                        }
                        break;
                    case CameraStateData.SequenceType.Time:
                        DeltaTimer timer = currentSequence.sequenceTimer;
                        if (currentSequence.IsDirty == false)
                        {
                            currentSequence.InitialPosition = transform.position;
                            currentSequence.InitialRotaion = transform.rotation.eulerAngles;
                            currentSequence.EndPosition = currentSequence.target.position + currentSequence.offset;
                            currentSequence.EndRotation = currentSequence.eulerRotation;

                            currentSequence.IsDirty = true;
                        }


                        if (timer.IsDone == false)
                        {
                            timer.Tick();
                        }
                        else
                        {
                            timer.Reset();
                            sequenceIndex++;
                        }
                        break;
                    case CameraStateData.SequenceType.Volume:

                        break;
                }

                if (sequenceIndex >= SelectedState.sequences.Length)
                {
                    ReleaseState();
                }
            }

            if (VelocityTracking == false)
            {
                if (SelectedState == null)
                {
                    Tracking(Time.deltaTime);
                }
                else
                {
                    PlaySequence(Time.deltaTime, SelectedState, sequenceIndex);
                }
            }
        }

        private void FixedUpdate()
        {
            if (trackingTarget == null)
            {
                return;
            }

            if (VelocityTracking == true)
            {
                if (SelectedState == null)
                {
                    Tracking(Time.fixedDeltaTime);
                }
                else
                {
                    PlaySequence(Time.fixedDeltaTime, SelectedState, sequenceIndex);
                }
            }
        }
        public void Shake(string name)
        {
            CameraShakeDescriptor descriptor;
            if (cameraShakeList.TryGetValue(name, out descriptor) == false)
            {
                LogUtil.LogError("Invalid argument, no such descriptor was found, " + name);
                return;
            }
            else
            {
                StartCoroutine("ShakeCamera", descriptor);
            }
        }

        float SampleWave(ref CameraShakeDescriptor descriptor, float elapsedTime)
        {
            float sampled = 0.0f;
            float normalizedTime = elapsedTime / descriptor.duration;
            float attenuation = descriptor.attenuationCurve.Evaluate(normalizedTime);

            sampled = Mathf.Sin(elapsedTime * descriptor.frequency) * (descriptor.magnitude * (descriptor.duration - elapsedTime)); //Mathf.Sin(elapsed * frequency) / elapsed * (1.0f / magnitude);

            if (float.IsNaN(sampled) == true)
            {
                sampled = 0.0f;
            }

            return sampled;
        }

        public IEnumerator ShakeCamera(CameraShakeDescriptor descriptor)
        {
            Vector3 originalPos = transform.localPosition;

            float elapsed = Mathf.Epsilon;

            while (elapsed < descriptor.duration)
            {
                float waveSample = SampleWave(ref descriptor, elapsed);
                float attenuationPower = descriptor.attenuationCurve.Evaluate(elapsed);

                float x = Random.Range(-1.0f, 1.0f) * waveSample;
                float y = Random.Range(-1.0f, 1.0f) * waveSample;
                float z = Random.Range(-1.0f, 1.0f) * waveSample;

                float pitch = Random.Range(-1.0f, 1.0f) * waveSample;
                float yaw = Random.Range(-1.0f, 1.0f) * waveSample;
                float roll = Random.Range(-1.0f, 1.0f) * waveSample;

                float fovDifferntial = Random.Range(0.0f, 1.0f) * waveSample;

                Vector3 originalRotation = transform.rotation.eulerAngles;
                Quaternion targetRotation = Quaternion.Euler(new Vector3(originalRotation.x + pitch, originalRotation.y + yaw, originalRotation.z + roll));

                // Vector3 targetOffset = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z);
                //transform.localPosition = targetOffset;
                transform.localRotation = targetRotation;
                // cameraInstance.fieldOfView = cameraInstance.fieldOfView + fovDifferntial;

                elapsed += Time.deltaTime;

                yield return null;
            }

            yield return null;
        }

        private void OnTriggerEnter(Collider other)
        {
            Volume targetVolume;
            other.TryGetComponent<Volume>(out targetVolume);
            if (targetVolume != null)
            {
                targetVolume.profile.TryGet<DepthOfField>(out dofInstance);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            globalVolume.profile.TryGet<DepthOfField>(out dofInstance);
        }

    }
}