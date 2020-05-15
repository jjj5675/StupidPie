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

    public static void AllCharacterReleaseControl(bool resetValuse = true)
    {
        Character character;
        for (int i = 0; i < Instance.m_PlayableCharacterCache.Count; i++)
        {
            Instance.m_PlayableCharacterCache.TryGetValue((PlayerBehaviour.PlayableCharacter)i, out character);
            character.playerInput.ReleaseControl(resetValuse);
        }
    }

    public static void AllCharacterGainControl()
    {
        Character character;
        for(int i=0; i<Instance.m_PlayableCharacterCache.Count; i++)
        {
            Instance.m_PlayableCharacterCache.TryGetValue((PlayerBehaviour.PlayableCharacter)i, out character);
            character.playerInput.GainControl();
        }
    }

    public static void AllCharacterTeleport()
    {
        Character character;
        Vector3 seriLocation = CellController.Instance.lastEnteringDestination.seriLocation.transform.position;
        Vector3 iresLocation = CellController.Instance.lastEnteringDestination.iresLocation.transform.position;

        Instance.m_PlayableCharacterCache.TryGetValue(PlayerBehaviour.PlayableCharacter.SERI, out character);
        GameObjectTeleporter.Teleport(character.playerBehaviour.gameObject, seriLocation);

        Instance.m_PlayableCharacterCache.TryGetValue(PlayerBehaviour.PlayableCharacter.IRES, out character);
        GameObjectTeleporter.Teleport(character.playerBehaviour.gameObject, iresLocation);
    }
}
