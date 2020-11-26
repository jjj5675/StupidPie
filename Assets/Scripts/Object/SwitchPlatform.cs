using UnityEngine;
using UnityEngine.Events;

public class SwitchPlatform : Platform
{
    public float switchChangeDuration;
    public float whiteSpriteExitTime;
    public EdgeCollider2D edgeCollider;
    public LayerMask overlapColliderMask;
    public UnityEvent OnEabled;
    public UnityEvent OnDisabled;
    public Sprite enableWhiteSprite;
    public Sprite disableWhiteSprite;

    protected ContactFilter2D m_OverlapCharacterContactFilter;
    protected BoxCollider2D m_Box;
    protected float m_CurrentChangeDuration = 0;
    protected bool m_IsPrevCollision;
    protected bool m_CanChangeSwitch = true;
    protected SpriteRenderer m_SpriteRenderer;
    protected Sprite m_CurrentWhiteSprite;
    protected Sprite m_StartingSprite;
    protected AudioSource[] switchAudio;

    void Awake()
    {
        m_Box = GetComponent<BoxCollider2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        if(transform.childCount!=0)
        switchAudio = transform.GetChild(0).GetComponents<AudioSource>();
    }

    protected override void Initialise()
    {
        if (edgeCollider == null)
        {
            edgeCollider = GetComponent<EdgeCollider2D>();
        }

        edgeCollider.enabled = false;

        m_OverlapCharacterContactFilter.layerMask = overlapColliderMask;
        m_OverlapCharacterContactFilter.useLayerMask = true;
        m_OverlapCharacterContactFilter.useTriggers = false;

        if (!isMovingAtStart)
        {
            OnEabled.Invoke();

            var spriteTemp = enableWhiteSprite;
            enableWhiteSprite = disableWhiteSprite;
            disableWhiteSprite = spriteTemp;

            m_IsPrevCollision = true;
            m_Box.isTrigger = true;
        }
        else
        {
            m_IsPrevCollision = false;
        }

        m_CurrentWhiteSprite = enableWhiteSprite;
        m_StartingSprite = m_SpriteRenderer.sprite;
    }

    public override void ResetPlatform()
    {
        if (!isMovingAtStart)
        {
            m_IsPrevCollision = true;
            m_Box.isTrigger = true;
        }
        else
        {
            m_IsPrevCollision = false;
            m_Box.isTrigger = false;
        }

        m_CanChangeSwitch = true;
        m_CurrentChangeDuration = 0f;
        m_SpriteRenderer.sprite = m_StartingSprite;
        m_CurrentWhiteSprite = enableWhiteSprite;
    }

    public void EnableOnSwitch()
    {
        m_CanChangeSwitch = true;
    }

    public void DisableOnSwitch()
    {
        m_CanChangeSwitch = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_CanChangeSwitch)
        {
            return;
        }

        m_CurrentChangeDuration += Time.deltaTime;

        if (switchChangeDuration <= m_CurrentChangeDuration)
        {
            if (m_CurrentWhiteSprite != m_SpriteRenderer.sprite)
            {
                m_SpriteRenderer.sprite = m_CurrentWhiteSprite;
                m_IsPrevCollision = !m_IsPrevCollision;
                
                if(switchAudio!=null && switchAudio[0].gameObject.activeInHierarchy)
                   switchAudio[0].Play();
            }

            if (switchChangeDuration + whiteSpriteExitTime <= m_CurrentChangeDuration)
            {
                m_CurrentChangeDuration = 0;

                if (switchAudio != null && switchAudio[1].gameObject.activeInHierarchy)
                    switchAudio[1].Play();
                if (m_IsPrevCollision)
                {
                    OnEabled.Invoke();
                }
                else
                {
                    OnDisabled.Invoke();
                }

                if (m_Box.isTrigger)
                {
                    SearchOverlapCharacter(edgeCollider, m_OverlapCharacterContactFilter, 5);
                }

                m_Box.isTrigger = m_IsPrevCollision;
                m_CurrentWhiteSprite = m_CurrentWhiteSprite == enableWhiteSprite ? disableWhiteSprite : enableWhiteSprite;

            }
        }
    }
}
