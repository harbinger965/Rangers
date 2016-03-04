using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Assets.Scripts.Level;
using Assets.Scripts.Player;
using Assets.Scripts.Timers;
using Assets.Scripts.Util;

namespace Assets.Scripts.Data
{
    /// <summary>
    /// Manager to control everything as the top of the pyramid
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public string settingsName = "Stock";
        /// <summary>
        /// Use a singleton instance to make sure there is only one
        /// </summary>
        public static GameManager instance;

        // List of all the controllers of the players
        private List<Controller> controllers;

        // List of all the types of tokens available
        [SerializeField]
        private List<GameObject> allTokens;
        private Dictionary<Enums.Tokens, Enums.Frequency> tokens;

        [SerializeField]
        private GameObject playerPrefab;

        // Match timer (optional)
        private Timer matchTimer;

        // Current game settings to abide by
        private GameSettings currentGameSettings;
        // CustomColor object for efficient color getting
        private CustomColor customColor;

        private int numDead = 0;

        // Sets up singleton instance. Will remain if one does not already exist in scene
        void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            controllers = new List<Controller>();
            customColor = new CustomColor();
        }

        void Start()
        {
            // Call Init
            InitializeMatch();
        }

		void OnLevelWasLoaded(int level)
		{
			// Reinitialize when restarting a match.
			controllers = new List<Controller>();
			numDead = 0;
			InitializeMatch();
		}

        /// <summary>
        /// Initializes the match when one is started
        /// </summary>
        private void InitializeMatch()
        {
            // Find all the players
            Controller[] findControllers = FindObjectsOfType<Controller>();
            for (int i = 0; i < findControllers.Length; i++)
            {
                controllers.Add(findControllers[i]);
            }
            // Load the last settings used
            //currentGameSettings = LoadManager.LoadGameSettings(GameSettings.persistentExtension);
            currentGameSettings = LoadManager.LoadGameSettingsXML(settingsName);

            // Initialize the tokens
            TokenSpawner.instance.Init(currentGameSettings.EnabledTokens);

            for (int i = 0; i < controllers.Count; i++)
            {
                controllers[i].LifeComponent.Lives = currentGameSettings.StockLimit;
            }

            // Set up a timer that counts up for targets
            if (currentGameSettings.Type.Equals(Enums.GameType.Target))
            {
                currentGameSettings.TargetsInLevel = FindObjectsOfType<Target>().Length;
                matchTimer = gameObject.AddComponent<Timer>();
                matchTimer.Initialize(Mathf.Infinity, "Match Timer");
            }
            // All other games will have a countdown timer
            else
            {
                // If the timer is enabled in that game type
                if (currentGameSettings.TimeLimitEnabled)
                {
                    matchTimer = gameObject.AddComponent<CountdownTimer>();
                    matchTimer.Initialize(currentGameSettings.TimeLimit, "Match Timer");
                    ((CountdownTimer)matchTimer).TimeOut += new CountdownTimer.TimerEvent(TimeUp);
                }
            }
        }
        
        /// <summary>
        /// Timer that will end the match
        /// </summary>
        /// <param name="t">The timer that timed out</param>
        private void TimeUp(CountdownTimer t)
        {
            Debug.Log("Time is up");
        }

        /// <summary>
        /// Handles a target being destroyed and checks to see if the game is over
        /// </summary>
        /// <param name="fromPlayer">The player that hit the target</param>
        public void TargetDestroyed(PlayerID fromPlayer)
        {
            if (--currentGameSettings.TargetsInLevel <= 0) GameOver();
        }

        /// <summary>
        /// Handles what happens when the game is over
        /// </summary>
        private void GameOver()
        {
            //Debug.Log("Match concluded");
            CountdownTimer.CreateTimer(gameObject, 3f, "GameOver", ResetLevel);
        }

        /// <summary>
        /// Resets the scene.
        /// </summary>
        private void ResetLevel(CountdownTimer t)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// What happens when a player is killed.
        /// </summary>
        /// <param name="killed">The id of the player that is killed</param>
        /// <param name="killedBy">The id of the player that is the killer</param>
        public void PlayerKilled(PlayerID killed, PlayerID killedBy = PlayerID.None)
        {
            // Find those two players
            Controller victim = controllers.Find(x => x.ID.Equals(killed));
            Controller killer = controllers.Find(x => x.ID.Equals(killedBy));
            // Increment killer score provided there is a killer that is not self
            if(killer != null) killer.LifeComponent.kills++;
            
            // Check if the game is over based on gametype
            if(currentGameSettings.Type.Equals(Enums.GameType.Kills))
            {
                if (killer != null && killer.LifeComponent.kills > currentGameSettings.KillLimit) GameOver();
            }
            if (currentGameSettings.Type.Equals(Enums.GameType.Stock))
            {
                if (victim.LifeComponent.Lives <= 0) numDead++;
                if (numDead >= controllers.Count - 1) GameOver();
            }
        }

        /// <summary>
        /// Sets up the player so it can respawn.
        /// </summary>
        /// <param name="id">The ID of the player that died</param>
        public void Respawn(PlayerID id)
        {
            // Find the dead player
            Controller deadPlayer = controllers.Find(x => x.ID.Equals(id));
            if(deadPlayer != null)
            {
                // Initialize the respawn timer
                CountdownTimer t = gameObject.AddComponent<CountdownTimer>();
                t.Initialize(3f, deadPlayer.ID.ToString());
                t.TimeOut += new CountdownTimer.TimerEvent(ResawnHelper);
            }
        }

        /// <summary>
        /// Target for the respawn timer to run on timeout.
        /// </summary>
        /// <param name="t">The timer that is firing the event</param>
        private void ResawnHelper(CountdownTimer t)
        {
            // Find the dead player again
            Controller deadPlayer = controllers.Find(x => x.ID.Equals(System.Enum.Parse(typeof(PlayerID), t.ID)));
            if (deadPlayer != null)
            {
                // Find an appropriate spawning pod (set to default for now)
                deadPlayer.transform.position = Vector3.zero;
                // Let the player revive itself
                deadPlayer.LifeComponent.Respawn();
            }
        }

        /// <summary>
        /// Creates a player when signing in and puts it in the list of active players
        /// </summary>
        /// <param name="id">ID of the player to create (the ID of the controller controlling it)</param>
        public void InitializePlayer(PlayerID id)
        {
            GameObject newPlayer = Instantiate(playerPrefab);
            Controller controller = newPlayer.GetComponent<Controller>();
            controller.ID = id;
            controllers.Add(controller);
            controller.Disable();
        }

        /// <summary>
        /// Remove the player from the list of active players
        /// </summary>
        /// <param name="id">ID of the player to deactivate from the game</param>
        public void RemovePlayer(PlayerID id)
        {
            Controller removePlayer = controllers.Find(x => x.ID.Equals(id));
            controllers.Remove(removePlayer);
        }

		public Controller GetPlayer(PlayerID id) {
			Controller p = controllers.Find(x => x.ID.Equals(id));
			return p;
		}

        #region C# Properties
        /// <summary>
        /// List of all the tokens prefabs
        /// </summary>
        public List<GameObject> AllTokens
        {
            get { return allTokens; }
        }
        /// <summary>
        /// Current game settings to check rules against
        /// </summary>
        public GameSettings CurrentGameSettings
        {
            get { return currentGameSettings; }
            set { currentGameSettings = value; }
        }
        /// <summary>
        /// All the players in the current game
        /// </summary>
        public List<Controller> AllPlayers
        {
            get { return controllers; }
        }
        #endregion
    }
}
