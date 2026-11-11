using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private ObjectManager objectManager;
    [SerializeField] private ObjectController objectController;
    [SerializeField] private PhysicsManager physicsManager;
    [SerializeField] private GameStateManager gameStateManager;

    public ObjectManager ObjectManager => objectManager;
    public ObjectController ObjectController => objectController;
    public PhysicsManager PhysicsManager => physicsManager;
    public GameStateManager GameStateManager => gameStateManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}