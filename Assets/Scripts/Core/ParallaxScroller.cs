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
    float m_OriginPosition;
    Vector2 screenBounds;

    private void Awake()
    {
        camera = Camera.main;
        float cameraHeight = 2 * camera.orthographicSize;
        screenBounds = new Vector2(cameraHeight * camera.aspect, cameraHeight);
    }

    private void Start()
    {
        //카메라의 움직임이 늦기 때문에 cell position을 처음 위치로 사용
        m_OldPosition = SceneController.Instance.rootCell.transform.position.x;
        m_OriginPosition = m_OldPosition;
        CreateLayers();
    }

    void CreateLayers()
    {
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            parallaxLayers[i].layerTransform.position = new Vector3(m_OldPosition, parallaxLayers[i].layerTransform.position.y, parallaxLayers[i].layerTransform.position.z);
            float spriteWidth = parallaxLayers[i].layerTransform.GetComponent<SpriteRenderer>().bounds.size.x;

            //int childsNeeded = (int)Mathf.Ceil(screenBounds.x * 2 / spriteWidth) + 1;
            GameObject clone = Instantiate(parallaxLayers[i].layerTransform.gameObject);

            for (int k = 0; k < 2; k++)
            {
                // 현재 c의 로컬, 월드 포지션은 parallaxLayers[i].layerTransform과 같다.
                GameObject c = Instantiate(clone);

                // c의 로컬만 변경
                c.transform.SetParent(parallaxLayers[i].layerTransform);

                //c의 로컬에 월드 포지션값을 주면 값이 로컬로 변환되버림
                // ex) c의 localPosition = (-1.5, 12.5)일 때 0을 주면 로컬값이 아닌 월드값으로 변경되서 로컬로 들어간다.
                //ex) 로컬값 0을 주고싶었는데  월드값으로 변경된 0 -> -200이 들어간다.
                //그래서 로컬은 -200정도에 월드포지션은 0이 나온다.
                //따라서 position이아니라 localPosition을 확실히 지정해서 변경해야한다.
                //c.transform.position = new Vector3(spriteWidth * k, parallaxLayers[i].layerTransform.position.y, parallaxLayers[i].layerTransform.position.z);
                c.transform.localPosition = new Vector3(spriteWidth * k, 0, 0);
                c.name = parallaxLayers[i].layerTransform.name + k;
            }

            Destroy(clone);
            Destroy(parallaxLayers[i].layerTransform.GetComponent<SpriteRenderer>());

            //이미지의 절반 크기를 구하기
            parallaxLayers[i].halfSpriteWidth = spriteWidth * 0.5f;
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
                //월드값이 나오므로 상관없이 진행가능
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
            parallaxLayers[i].layerTransform.position = new Vector3(m_OriginPosition, parallaxLayers[i].layerTransform.position.y, parallaxLayers[i].layerTransform.position.z);

            for (int k = 0; k < 2; k++)
            {
                parallaxLayers[i].layerTransform.GetChild(k).transform.localPosition =
                    new Vector3(parallaxLayers[i].halfSpriteWidth * 2 * k, 0, 0);
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
            Debug.Log(factor);
            SetLayerPostion(factor);
        }
        else
        {
            m_OldPosition = camera.transform.position.x;
        }
    }
}
