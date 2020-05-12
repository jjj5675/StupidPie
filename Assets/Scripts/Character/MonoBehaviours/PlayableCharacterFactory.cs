using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayableCharacterFactory : MonoBehaviour
{
    //OnEable, Disable고려할것

    private static PlayableCharacterFactory s_Instance;
    public static PlayableCharacterFactory Instance
    {
        get
        {
            if (s_Instance != null)
            {
                return s_Instance;
            }

            s_Instance = FindObjectOfType<PlayableCharacterFactory>();

            if (s_Instance != null)
            {
                return s_Instance;
            }

            Create();

            return s_Instance;
        }
        set
        {
            s_Instance = value;
        }
    }

    static void Create()
    {
        GameObject gameObject = new GameObject("PlayableCharacterFactory");
        s_Instance = gameObject.AddComponent<PlayableCharacterFactory>();
    }

     public class Character
    {
        public PlayerBehaviour playerBehaviour;
        public PlayerInput playerInput;
        public Collider2D playerCollider;
    }

    Dictionary<PlayerBehaviour.PlayableCharacter, Character> m_PlayableCharacterCache = new Dictionary<PlayerBehaviour.PlayableCharacter, Character>();

    void Awake()
    {
        if(Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }

    public static void Initialise(PlayerBehaviour newBehaviour, PlayerInput newInput, Collider2D collider)
    {
        Character newCharacter = new Character
        {
            playerBehaviour = newBehaviour,
            playerInput = newInput,
            playerCollider = collider
        };

        Instance.m_PlayableCharacterCache.Add(newBehaviour.playableCharacter, newCharacter);
    }

    public static PlayerBehaviour TryGetBehaviour(PlayerBehaviour.PlayableCharacter playableCharacter)
    {
        Character character;
        Instance.m_PlayableCharacterCache.TryGetValue(playableCharacter, out character);
        return character.playerBehaviour;
    }

    public static PlayerInput TryGetInput(PlayerBehaviour.PlayableCharacter playableCharacter)
    {
        Character character;
        Instance.m_PlayableCharacterCache.TryGetValue(playableCharacter, out character);
        return character.playerInput;
    }
    public static Collider2D TryGetCollider(PlayerBehaviour.PlayableCharacter playableCharacter)
    {
        Character character;
        Instance.m_PlayableCharacterCache.TryGetValue(playableCharacter, out character);
        return character.playerCollider;
    }
}
