using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class StoneBridge : MonoBehaviour
    {
        [System.Serializable]
        public struct TransformInfo
        {
            public Vector3 position;
            public Vector3 localPosition;
            public Quaternion localRotation;
            public Vector3 localScale;

            public void SetTransformInfo(Transform transform)
            {
                position = transform.position;
                localPosition = transform.localPosition;
                localRotation = transform.localRotation;
                localScale = transform.localScale;
            }
        }

        public TransformInfo destination;
        public TransformInfo startPoint;
        public float moveSpeed = 3.0f;
        public bool IsDestroy = false;
        public bool isAutoOff = true;
        public float autoOffTime = 1.0f;

        [SerializeField, ReadOnly, TabGroup("Debug")] private Rigidbody rigidbody;
        private Coroutine moveCoroutine = null;

        #region Offsets
        [SerializeField, LabelText("Position Min"), TabGroup("Offset")] private Vector3 positionOffsetMin;
        [SerializeField, LabelText("Position Max"), TabGroup("Offset")] private Vector3 positionOffsetMax;
        [SerializeField, LabelText("Rotation Min"), TabGroup("Offset")] private Vector3 rotationOffsetMin;
        [SerializeField, LabelText("Rotation Max"), TabGroup("Offset")] private Vector3 rotationOffsetMax;
        [SerializeField, LabelText("Scale Min"), TabGroup("Offset")] private Vector3 scaleOffsetMin;
        [SerializeField, LabelText("Scale Max"), TabGroup("Offset")] private Vector3 scaleOffsetMax;
        [SerializeField, LabelText("Broken Power"), TabGroup("Offset")] private float offsetSize;
        #endregion

        [Button(Name = "End 위치 설정")]
        public void SetDestination()
        {
            destination.SetTransformInfo(transform);
        }

        [Button(Name = "Start 위치 설정")]
        public void SetStartPoint()
        {
            startPoint.SetTransformInfo(transform);
        }

        private void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

        private void setOffsetMinMax(ref Vector3 min, ref Vector3 max)
        {
            if (min.x > max.x)
            {
                Swap(ref min.x, ref max.x);
            }
            if (min.y > max.y)
            {
                Swap(ref min.y, ref max.y);
            }
            if (min.z > max.z)
            {
                Swap(ref min.z, ref max.z);
            }
        }

        [Button(Name = "Start 위치 랜덤 설정")]
        public void SetRandomStartPoint()
        {
            setOffsetMinMax(ref positionOffsetMin, ref positionOffsetMax);
            setOffsetMinMax(ref rotationOffsetMin, ref rotationOffsetMax);
            setOffsetMinMax(ref scaleOffsetMin, ref scaleOffsetMax);


            //Offset Random StartingPoint Setting
            startPoint = destination;
            float x = Random.Range(positionOffsetMin.x, positionOffsetMax.x);
            float y = Random.Range(positionOffsetMin.y, positionOffsetMax.y);
            float z = Random.Range(positionOffsetMin.z, positionOffsetMax.z);
            startPoint.localPosition += new Vector3(x, y, z);

            Vector3 angle = startPoint.localRotation.eulerAngles;
            x = Random.Range(rotationOffsetMin.x, rotationOffsetMax.x);
            y = Random.Range(rotationOffsetMin.y, rotationOffsetMax.y);
            z = Random.Range(rotationOffsetMin.z, rotationOffsetMax.z);
            startPoint.localRotation = Quaternion.Euler(angle + new Vector3(x, y, z));


            x = Random.Range(scaleOffsetMin.x, scaleOffsetMax.x);
            y = Random.Range(scaleOffsetMin.y, scaleOffsetMax.y);
            z = Random.Range(scaleOffsetMin.z, scaleOffsetMax.z);
            startPoint.localScale = new Vector3(x, y, z);
        }

        [Button(Name = "End 위치 랜덤 설정")]
        public void SetRandomEndPoint()
        {
            setOffsetMinMax(ref positionOffsetMin, ref positionOffsetMax);
            setOffsetMinMax(ref rotationOffsetMin, ref rotationOffsetMax);
            setOffsetMinMax(ref scaleOffsetMin, ref scaleOffsetMax);


            //Offset Random StartingPoint Setting
            destination = startPoint;
            float x = Random.Range(positionOffsetMin.x, positionOffsetMax.x);
            float y = Random.Range(positionOffsetMin.y, positionOffsetMax.y);
            float z = Random.Range(positionOffsetMin.z, positionOffsetMax.z);
            destination.localPosition += new Vector3(x, y, z);

            Vector3 angle = destination.localRotation.eulerAngles;
            x = Random.Range(rotationOffsetMin.x, rotationOffsetMax.x);
            y = Random.Range(rotationOffsetMin.y, rotationOffsetMax.y);
            z = Random.Range(rotationOffsetMin.z, rotationOffsetMax.z);
            destination.localRotation = Quaternion.Euler(angle + new Vector3(x, y, z));


            x = Random.Range(scaleOffsetMin.x, scaleOffsetMax.x);
            y = Random.Range(scaleOffsetMin.y, scaleOffsetMax.y);
            z = Random.Range(scaleOffsetMin.z, scaleOffsetMax.z);
            destination.localScale = new Vector3(x, y, z);
        }

        [Button(Name = "End 위치 확인")]
        public void SetPositionToDestination()
        {
            transform.localPosition = destination.localPosition;
            transform.localRotation = destination.localRotation;
            transform.localScale = destination.localScale;
        }

        [Button(Name = "Start 위치 확인")]
        public void SetPositionToStartPoint()
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            transform.localPosition = startPoint.localPosition;
            transform.localRotation = startPoint.localRotation;
            transform.localScale = startPoint.localScale;
        }

        [Button(Name = "다리 연출 재생")]
        public void TestBridgeOn()
        {
            BridgeOn();
        }

        [Button(Name = "다리 연출 되돌리기")]
        public void TestBridgeOff()
        {
            BridgeOff();
        }

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        public void BridgeOn()
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }

            moveCoroutine = StartCoroutine(OnMoving());
        }

        public virtual IEnumerator OnMoving()
        {
            while ((destination.localPosition - transform.localPosition).magnitude > 0.05f)
            {
                //SoundManager.Instance.PlaySound("Bridge_On(029)", false);
                transform.localPosition = Vector3.Lerp(transform.localPosition, destination.localPosition, Time.deltaTime * moveSpeed);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, destination.localRotation, Time.deltaTime * moveSpeed);
                transform.localScale = Vector3.Lerp(transform.localScale, destination.localScale, Time.deltaTime * moveSpeed);
                yield return new WaitForEndOfFrame();
            }

            transform.localPosition = destination.localPosition;
            transform.localRotation = destination.localRotation;
            transform.localScale = destination.localScale;

            if (isAutoOff)
            {
                yield return new WaitForSeconds(autoOffTime);
                //SoundManager.Instance.StopSound("Bridge_On(029)");
                BridgeOff();
            }
            moveCoroutine = null;
        }

        public void BridgeOff()
        {

            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            moveCoroutine = StartCoroutine(OffMoving());

        }

        public virtual IEnumerator OffMoving()
        {
            moveCoroutine = null;
            while ((startPoint.localPosition - transform.localPosition).magnitude > 0.01f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, startPoint.localPosition, Time.fixedDeltaTime * moveSpeed);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, startPoint.localRotation, Time.fixedDeltaTime * moveSpeed);
                transform.localScale = Vector3.Lerp(transform.localScale, startPoint.localScale, Time.fixedDeltaTime * moveSpeed);
             
                if(moveCoroutine != null)
                {
                    break;
                }
                
                yield return new WaitForFixedUpdate();
            }

            transform.localPosition = startPoint.localPosition;
            transform.localRotation = startPoint.localRotation;
            transform.localScale = startPoint.localScale;
        }

        public void BridgeBreak()
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            moveCoroutine = StartCoroutine(moveing());

            IEnumerator moveing()
            {
                //rigidbody.isKinematic = false;
                //rigidbody.useGravity = true;

                float x = Random.Range(-offsetSize, offsetSize);
                float y = Random.Range(0, offsetSize);
                float z = Random.Range(-offsetSize, offsetSize);
                rigidbody.AddForce(new Vector3(x, y, z));
                rigidbody.AddTorque(new Vector3(x, y, z) * 0.1f);

                if (IsDestroy)
                {
                    yield return new WaitForSeconds(5.0f);
                    gameObject.SetActive(false);
                }
            }
        }
    }
}