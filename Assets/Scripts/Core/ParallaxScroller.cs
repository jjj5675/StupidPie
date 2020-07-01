using System;
using UnityEngine;

public class ParallaxScroller : MonoBehaviour
{
    [Serializable]
    public class ParallaxLayer
    {
        public Transform layerTransform;

        [Range(0f, 10f)]
        public float speed;

        [HideInInspector]
        public float halfSpriteWidth;
    }

    public Camera camera;
    public ScreenManager screenManager;
    public ParallaxLayer[] parallaxLayers;

    float m_OldPosition;
    Vector2 screenBounds;

    private void Awake()
    {
        camera = Camera.main;
        float cameraHeight = 2 * camera.orthographicSize;
        screenBounds = new Vector2(cameraHeight * camera.aspect, cameraHeight);

        m_OldPosition = camera.transform.position.x;
        transform.position = new Vector3(camera.transform.position.x, transform.position.y, transform.position.z);
        CreateLayers();
    }

    void CreateLayers()
    {
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            float spriteWidth = parallaxLayers[i].layerTransform.GetComponent<SpriteRenderer>().bounds.size.x;

            //int childsNeeded = (int)Mathf.Ceil(screenBounds.x * 2 / spriteWidth) + 1;
            GameObject clone = Instantiate(parallaxLayers[i].layerTransform.gameObject);

            for (int k = 0; k < 2; k++)
            {
                GameObject c = Instantiate(clone);
                c.transform.SetParent(parallaxLayers[i].layerTransform);
                c.transform.position = new Vector3(spriteWidth * k, parallaxLayers[i].layerTransform.position.y, parallaxLayers[i].layerTransform.position.z);
                c.name = parallaxLayers[i].layerTransform.name + k;
            }

            Destroy(clone);
            Destroy(parallaxLayers[i].layerTransform.GetComponent<SpriteRenderer>());

            //이미지의 절반 크기를 구하기
            parallaxLayers[i].halfSpriteWidth = parallaxLayers[i].layerTransform.GetChild(0).GetComponent<SpriteRenderer>().sprite.bounds.extents.x;
        }
    }

    void SetLayerPostion(float factor)
    {
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            GameObject currentChild = parallaxLayers[i].layerTransform.GetChild(0).gameObject;
            GameObject nextChild = parallaxLayers[i].layerTransform.GetChild(1).gameObject;

            if (factor < 0)
            {
                if (camera.transform.position.x + screenBounds.x * 0.5f > nextChild.transform.position.x + parallaxLayers[i].halfSpriteWidth)
                {
                    currentChild.transform.SetAsLastSibling();
                    currentChild.transform.position = new Vector3(nextChild.transform.position.x + parallaxLayers[i].halfSpriteWidth * 2, nextChild.transform.position.y, nextChild.transform.position.z);
                }
            }
            else if (factor > 0)
            {
                if (camera.transform.position.x - screenBounds.x * 0.5f < currentChild.transform.position.x - parallaxLayers[i].halfSpriteWidth)
                {
                    nextChild.transform.SetAsFirstSibling();
                    nextChild.transform.position = new Vector3(currentChild.transform.position.x - parallaxLayers[i].halfSpriteWidth * 2, currentChild.transform.position.y, currentChild.transform.position.z);
                }
            }
        }
    }

    public void Initialize()
    {
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            for (int k = 0; k < 2; k++)
            {
                parallaxLayers[i].layerTransform.GetChild(k).transform.position = 
                    new Vector3(parallaxLayers[i].halfSpriteWidth * 2f * k, parallaxLayers[i].layerTransform.position.y, parallaxLayers[i].layerTransform.position.z);
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (screenManager.autoCameraSetup.CellChanging)
        {
            float delta = m_OldPosition - camera.transform.position.x;
            float factor = Math.Sign(delta);

            for (int i = 0; i < parallaxLayers.Length; i++)
            {
                Vector2 newPosition = parallaxLayers[i].layerTransform.position;

                newPosition.x += factor * parallaxLayers[i].speed * Time.deltaTime;

                parallaxLayers[i].layerTransform.position = newPosition;
            }

            SetLayerPostion(factor);
        }
        else
        {
            m_OldPosition = camera.transform.position.x;
        }
    }
}
