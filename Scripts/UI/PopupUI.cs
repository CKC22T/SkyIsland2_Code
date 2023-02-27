using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Olympus
{
    public class PopupUI : UIBase
    {
        public Image mainFrame;
        public Image[] sideFrames;
        public Image[] sideFrameBuffer;
        private int selectedIndex = 0;
        private Rect sideFrameSize;
        public RectTransform Anchor;

        public GameObject photoGroup;
        public GameObject mapGroup;

        public SerializableDictionary<IslandType, GameObject> mapObjects;
        public TextMeshProUGUI mapIslandText;
        public TextMeshProUGUI mapOlympusNameText;
        public GameObject mapNameObject;

        private Dictionary<Texture2D, Sprite> cachedSprites = new();

        Coroutine photoLoadRoutine = null;

        private new void Awake()
        {
            base.Awake();

            sideFrameBuffer = sideFrames;

            for (int i = 0; i < sideFrameBuffer.Length; i++)
            {
                for (int j = 0; j < sideFrameBuffer.Length; j++)
                {
                    Transform root = sideFrameBuffer[j].transform.parent;
                    float dist0 = Vector2.Distance(root.position, sideFrameBuffer[j].transform.position);
                    float dist1 = Vector2.Distance(root.position, sideFrameBuffer[i].transform.position);

                    if (dist0 > dist1)
                    {
                        Vector3 temp = sideFrameBuffer[j].transform.position;
                        sideFrameBuffer[j].transform.position = sideFrameBuffer[i].transform.position;
                        sideFrameBuffer[i].transform.position = temp;
                    }
                }
            }

            sideFrames = sideFrameBuffer;
        }

        public override void Show(UnityAction callback = null)
        {
            InitMap();



            base.Show(callback);
            PlayerController.Instance.PopupShift(true);

            mapNameObject.SetActive(GameDataManager.Instance.stageIslandType != IslandType.Boss);

            if(photoLoadRoutine != null)
            {
                StopCoroutine(photoLoadRoutine);
                photoLoadRoutine = null;
            }

            photoLoadRoutine = StartCoroutine(loadPhotos());
        }

        IEnumerator loadPhotos()
        {
            List<Texture2D> photos = GameManager.Instance.AlbumBuffer;

            Sprite mainSprite = null;

            if (photos.Count == 0)
            {
                yield return null;
            }

            if(photos.Count > 0)
            {
                if (cachedSprites.TryGetValue(photos[selectedIndex], out mainSprite) == false)
                {
                    mainSprite = Sprite.Create(photos[selectedIndex], new Rect(0, 0, photos[selectedIndex].width, photos[selectedIndex].height), new Vector2(0.0f, 0.0f));

                    cachedSprites.Add(photos[selectedIndex], mainSprite);
                }
            }

            mainFrame.sprite = mainSprite;

            for (int i = 0; i < sideFrames.Length; i++)
            {
                if (i >= photos.Count)
                {
                    break;
                }

                Sprite cache;

                if (cachedSprites.ContainsKey(photos[i]) == false)
                {
                    cache = Sprite.Create(photos[i], new Rect(0, 0, photos[i].width, photos[i].height), new Vector2(0.0f, 0.0f));

                    cachedSprites.Add(photos[i], cache);
                }
                else
                {
                    cache = cachedSprites[photos[i]];
                }

                sideFrames[i].sprite = cache;

                yield return new WaitForEndOfFrame();
            }

            yield return null;
        }

        public void selectPhoto()
        {
            GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;


            for (int i = 0; i < 100; i++)
            {
                if(cachedSprites.Values.Count <= i)
                {
                    break;
                }

                if (sideFrames[i].gameObject == button)
                {
                    selectedIndex = i;
                    float offset = (-122.9f * selectedIndex) + (-122.9f * (selectedIndex / 100.0f));
                    Anchor.transform.localPosition = new Vector2(offset, 0.0f);
                }
            }
        }

        public override void Hide(UnityAction callback = null)
        {
            Animate(true, () =>
            {
                base.Hide(callback);
                PlayerController.Instance.PopupShift(false);
            });
        }

        private void Update()
        {
            List<Texture2D> photos = GameManager.Instance.AlbumBuffer;

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                UIManager.Hide(UIList.PopupUI);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (selectedIndex + 1 < sideFrames.Length && selectedIndex + 1 < photos.Count)
                {
                    selectedIndex++;

                    float offset = (-122.9f * selectedIndex) + (-122.9f * (selectedIndex / 100.0f));

                    Anchor.transform.localPosition = new Vector2(offset, 0.0f);
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (selectedIndex - 1 >= 0)
                {
                    selectedIndex--;

                    float offset = (-122.9f * selectedIndex) + (-122.9f * (selectedIndex / 100.0f));


                    Anchor.transform.localPosition = new Vector2(offset, 0.0f);
                }
            }
            Sprite mainSprite;

            if (selectedIndex < sideFrames.Length && selectedIndex >= 0)
            {
                if (selectedIndex < photos.Count)
                {
                    if (cachedSprites.TryGetValue(photos[selectedIndex], out mainSprite) == true)
                    {
                        mainFrame.sprite = mainSprite;
                        LogUtil.Log("Sprite Size:" + mainSprite.rect.width + ", " + mainSprite.rect.height);
                    }
                }

            }

        }

        public void InitMap()
        {
            IslandType islandType = GameDataManager.Instance.stageIslandType;
            foreach (var kv in mapObjects)
            {
                kv.Value.SetActive(kv.Key == islandType);
            }
            mapIslandText.text = TextTable.Instance.Get(IslandNameInfo.ISLAND_NAME[islandType]);
            mapOlympusNameText.text = IslandNameInfo.OLYMPUS_NAME[islandType];
        }

        public void ShowPhoto()
        {
            photoGroup.SetActive(true);
            mapGroup.SetActive(false);
        }

        public void ShowMap()
        {
            photoGroup.SetActive(false);
            mapGroup.SetActive(true);
        }
    }
}