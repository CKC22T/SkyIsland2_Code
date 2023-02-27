using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Olympus
{
    public class AlbumUI : UIBase
    {
        public Image[] AlbumImages;
        private List<Dictionary<Texture2D, Sprite>> spriteCache = new();
        private Dictionary<Texture2D, Sprite> mainSpriteCache = new();

        private List<Texture2D> imageBuffer = new();
        [SerializeField] private Sprite placeholderSprite;
        [SerializeField] private TextMeshProUGUI pageIndexTextLeft;
        [SerializeField] private TextMeshProUGUI pageIndexTextRight;
        [SerializeField] private AlbumImageElementUI[] elements;
        [SerializeField] private Image mainImage;
        int selectedIndex = 0;
        int pageIndex = 0;

        private Coroutine loadCoroutine = null;
        
        public int selectedImageIndex = 0;
        private new void Awake()
        {
            base.Awake();
            spriteCache.Add(new());
            elements = GetComponentsInChildren<AlbumImageElementUI>();
        }

        public void ResetPage()
        {
            for (int i = 0; i < AlbumImages.Length;i++)
            {
                Transform logoObject = AlbumImages[i].transform.GetChild(0);
                elements[i].Setup(i, -1);

                AlbumImages[i].sprite = placeholderSprite;
                AlbumImages[i].color = new Color(0.6313726f, 0.4823529f, 0.345098f);
                logoObject.gameObject.SetActive(true);
            }
        }

        IEnumerator loadAsync()
        {
            yield return null;


        }

        public void NextPage()
        {
            pageIndex++;
            pageIndex = Mathf.Clamp(pageIndex, 0, (imageBuffer.Count / 12));

            if(spriteCache.Count <= pageIndex)
            {
                spriteCache.Add(new());
            }
            ResetPage();
            if (loadCoroutine != null)
            {
                StopCoroutine(loadCoroutine);
            }

            loadCoroutine = StartCoroutine(UpdatePage());
        }

        public void PreviousPage()
        {
            pageIndex--;
            pageIndex = Mathf.Clamp(pageIndex, 0, (imageBuffer.Count / 12));
            ResetPage();

            if(loadCoroutine != null)
            {
                StopCoroutine(loadCoroutine);
                loadCoroutine = null;
            }

            loadCoroutine = StartCoroutine(UpdatePage());
        }

        IEnumerator LoadImagesFromDisk()
        {
            string documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string screenshotFolder = documentPath + "/My Games/SkyIslands/Screenshots";

            if(Directory.Exists(screenshotFolder) == false)
            {
                Directory.CreateDirectory(screenshotFolder);
                yield return null;
            }
            else
            {
                var imagePathes = Directory.GetFiles(screenshotFolder);
                foreach(var path in imagePathes)
                {
                    byte[] rawImage = File.ReadAllBytes(path);

                    Texture2D loadTexture = new Texture2D(1, 1);

                    if(loadTexture.LoadImage(rawImage) == true)
                    {
                        imageBuffer.Add(loadTexture);
                    }

                    yield return null;
                }


                yield return null;
            }
        }

        IEnumerator UpdatePage()
        {
            int offsetIndex = pageIndex * 12;
            int pageNumber = (pageIndex * 2) + 1;

            pageIndexTextLeft.text = pageNumber.ToString();
            pageIndexTextRight.text = (pageNumber + 1).ToString();

            if (imageBuffer == null)
            {
                yield return null;
            }

            for (int i = 0; i < AlbumImages.Length; i++)
            {
                int imageIndex = i + offsetIndex;

                if (imageIndex >= imageBuffer.Count)
                {
                    break;
                }


                Sprite sprite = null;
                if (spriteCache[pageIndex].ContainsKey(imageBuffer[imageIndex]) == false)
                {
                    sprite = Sprite.Create(imageBuffer[imageIndex], new Rect(0, 0, imageBuffer[imageIndex].width, imageBuffer[imageIndex].height), Vector2.zero);
                }
                else
                {
                    sprite = spriteCache[pageIndex].GetValueOrDefault(imageBuffer[imageIndex]);
                }

                elements[i].Setup(i, imageIndex);
                AlbumImages[i].sprite = sprite;
                Transform logoObject = AlbumImages[i].transform.GetChild(0);
                logoObject.gameObject.SetActive(false);

                yield return null;
            }
        }

        public override void Show(UnityAction callback = null)
        {
            base.Show(callback);
            StartCoroutine(LoadImagesFromDisk());
            StartCoroutine(UpdatePage());
        }

        public override void Hide(UnityAction callback = null)
        {
            base.Hide(callback);
        }

        public void HideUI()
        {
            UIManager.Hide(Id);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(selectedImageIndex != -1)
                {
                    selectedImageIndex = -1;
                }
                else
                {
                    UIManager.Hide(Id);
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SoundManager.Instance.PlaySound("UI_turn(033)");
                if (selectedIndex + 1 < AlbumImages.Length)
                {
                    selectedIndex++;
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SoundManager.Instance.PlaySound("UI_turn(033)");
                if (selectedIndex - 1 >= 0)
                {
                    selectedIndex--;
                }
            }

            if(selectedImageIndex != -1)
            {
                mainImage.gameObject.SetActive(true);
                if(mainSpriteCache.ContainsKey(imageBuffer[selectedImageIndex]) == false)
                {
                    var sprite = Sprite.Create(imageBuffer[selectedImageIndex], new Rect(0, 0, imageBuffer[selectedImageIndex].width, imageBuffer[selectedImageIndex].height), Vector2.zero);
                    mainSpriteCache.Add(imageBuffer[selectedImageIndex], sprite);
                }

                mainImage.sprite = mainSpriteCache[imageBuffer[selectedImageIndex]];
            }
            else
            {
                mainImage.gameObject.SetActive(false);
            }
        }
    }
}