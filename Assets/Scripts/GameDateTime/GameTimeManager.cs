using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

namespace GameDateTime
{
    [System.Serializable]
    public struct SkyboxTime
    {
        public Material material;
        public float time;
    }

    public class GameTimeManager : Singleton<GameTimeManager>
    {
        [Header("DateTime Configuration")]
        [SerializeField] private float timeScale = 1f;
        [SerializeField] private GameTime gameTime;
        [SerializeField] private bool pause = false;
        [SerializeField] private SkyboxTime[] skyboxTimes;

        public GameTime GameTime { get { return gameTime; } }

        [Header("Sun")]
        [SerializeField] private Transform sunTransform;
        [SerializeField] private float sunRotateSpeed = 1f;
        [SerializeField] private float indoorSunAngle = 40f;
        private Transform SunTransform
        {
            get
            {
                if(sunTransform == null)
                {
                    sunTransform = GameObject.Find("Directional Light").GetComponent<Transform>();
                }
                return sunTransform;
            }
        }
        private float _sunMoveAngle;
        private Quaternion _targetSunAngle;

        List<ITimeChecker> timeCheckerListeners = new();
        private bool _updateNewDay = false;

        private SkyboxTime currentSkybox;
        private float skyboxTransitionTime = 1f;
        private float skyboxTimer = 0f;

        public void Reset()
        {
            gameTime = new(0, Season.Spring, 1, 6, 0); // 6am
        }

        protected override void AwakeSingleton()
        {
            gameTime = new(0, Season.Spring, 1, 6, 0); // 6am

            float degreeInHour = 360 / GameTime.MAX_HOUR;
            _sunMoveAngle = degreeInHour / GameTime.MAX_MINUTE;
            if(sunTransform == null)
            {
                sunTransform = GameObject.Find("Directional Light").GetComponent<Transform>();
            }

            StartCoroutine(UpdateTime()); // Start the time update
            LoadSunTransform(); // Rotate the sun instantly
            //currentSkybox = skyboxTimes[0];
        }

        public void Load(GameTime gameTime)
        {
            this.gameTime = gameTime;
        }

        private void Update()
        {
            SunTransform.rotation = Quaternion.Slerp(SunTransform.rotation, _targetSunAngle, sunRotateSpeed * Time.deltaTime);
        }

        private IEnumerator UpdateTime()
        {
            while (true)
            {
                if(!pause)
                    Tick(); // Put before wait for second to prevent any delay

                yield return new WaitForSeconds(1 / timeScale);
            }
        }
        
        public void Tick()
        {
            gameTime.IncreaseTime();
            if(gameTime.Hour >= 5 && !_updateNewDay) // 5AM Reset, Player already sleep
            {
                _updateNewDay = true;
                // Inform listeners new day start
                foreach (ITimeChecker listener in timeCheckerListeners)
                {
                    listener.NewDay(gameTime);
                }
            }
            else if(gameTime.Hour == 0 && _updateNewDay) // Auto flag update new day, not neccessary but it can be update the new day if no sleep feature is implemented
            {
                _updateNewDay = false;
            }

            // Inform listeners about the clock time
            foreach(ITimeChecker listener in timeCheckerListeners)
            {
                if(listener != null)
                    listener.ClockUpdate(gameTime);
            }

            // Update skybox
            if(skyboxTimes != null)
            {
                foreach(SkyboxTime skyboxTime in skyboxTimes)
                {
                    if(gameTime.Hour == skyboxTime.time && skyboxTime.time != currentSkybox.time)
                    {
                        currentSkybox = skyboxTime;
                        RenderSettings.skybox = currentSkybox.material; // TODO: Make it smoothly change the texture
                        //skyboxTimer = 0;
                        break;
                    }
                }
            }

            UpdateSunTransform(); // Calculate target sun rotation
        }

        public void Sleep()
        {
            LoadSunTransform();
            _updateNewDay = false;
            gameTime.Reset(); // Reset the date time
            Tick(); // Explicitly call tick to update the date time
            LoadSunTransform();
        }

        public void LoadSunTransform()
        {
            UpdateSunTransform();
            SunTransform.rotation = _targetSunAngle; // Rotate the sun instantly
        }

        private void UpdateSunTransform()
        {
            int minutes = GameTime.HoursToMinutes(gameTime.Hour) + gameTime.Minute;
            float sunAngle = _sunMoveAngle * minutes - 90f;

            _targetSunAngle = Quaternion.Euler(sunAngle, 0f, 0f);
        }

        public void AddListener(ITimeChecker listener)
        {
            timeCheckerListeners.Add(listener);
        }
        public void RemoveListener(ITimeChecker listener)
        {
            timeCheckerListeners.Remove(listener);
        }
        public bool ExistListener(ITimeChecker listener)
        {
            return timeCheckerListeners.Exists(x => x.Equals(listener));
        }

        public void UpdateListener()
        {
            timeCheckerListeners.RemoveAll(x => x == null);
        }

        public void RemoveAllListeners()
        {
            timeCheckerListeners.Clear();
        }

        public void TogglePause()
        {
            PauseTime(!pause);
        }

        public void PauseTime(bool v)
        {
            pause = v;
        }

        public void UpdateClock()
        {
            foreach (ITimeChecker listener in timeCheckerListeners)
            {
                if (listener != null)
                    listener.ClockUpdate(gameTime);
            }
        }
    }
}
