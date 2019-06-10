using DG.Tweening;
using FantasyErrand.Entities.Interfaces;
using System.Collections;
using UnityEngine;
namespace FantasyErrand.Entities
{
    public delegate void PlayerBroadcast(float coinValue);
    public delegate void SpeedBroadcast(float multiplier);
    public delegate void GoldenCoinBroadcast(bool goldenCoinActive);
    public class Player : MonoBehaviour
    {
        public static event PlayerBroadcast coinAdded;
        [Header("Non-Game")]
        public bool enableNonGameMode;
        
        [Header("Motion Configuration"), SerializeField]
        private bool controlActive;
        public float sidestepSpeedThreshold = 1f;
        public float jumpSpeedThreshold = 1f;
        public float slideSpeedThreshold = 1f;
        public float turnThreshold = 4f;

        EmotionManager emotionManager;

        internal float speed;

        public delegate void TurnEventDelegate(int direction);
        public event System.Action<Collision> OnCollision;
        public event TurnEventDelegate OnTurn;
        public LayerMask layerMask;

        //----[Magnet Attribute]
        private float magnetRange;
        public int magnetSpeed = 8;
        private bool resetMagnet = false;
        private bool magnetStarted = false;
       
        //---[Phase Attribute]
        private bool resetPhase = false;
        private bool phaseStarted = false;

        //--[Boost Powerups Attribute]
        private bool resetBoost = false;
        private bool boostStarted = false;
        public static event SpeedBroadcast speedBroadcast;

        //--[GoldenCoinCollectibe Attribute]
        private bool resetGoldenCoin = false;
        private bool goldenCoinStarted = false;
        public static event GoldenCoinBroadcast goldenCoinBroadcast;
        /// <summary>
        /// Can the player be controlled by motion controls?
        /// </summary>
        public bool IsControlActive { get { return controlActive; } internal set { controlActive = value; } }

        //Vector2 moveDir = new Vector2(0, 1); //Move Direction: X -> X actual, Y -> Z actual

        //Motion related variables
        //Sidestep/Jump/Slide
        Vector3 accelerometerValue;
        Vector3 initialScale;
        float velocityX = 0;
        float velocityY = 0;
        bool canJump = true;
        bool canSlide = true;
        bool canSidestep = true;

        [HideInInspector]
        public float magnetTime = 0;
        [HideInInspector]
        public float goldenCoinTime = 0;
        [HideInInspector]
        public float phaseTime = 0;
        [HideInInspector]
        public float boostTime = 0;
        private bool magnetActivated = false;
        int lane = 0;

        // Use this for initialization
        void Start()
        {
            PowerUpsManager.magnetBroadcast += SetMagnetProperty;
            if (enableNonGameMode)
            {
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Obstacles"), LayerMask.NameToLayer("Player"));
                speed = 5;
            }
            else
            {
                emotionManager = FindObjectOfType<EmotionManager>();
                initialScale = transform.localScale;
            }
        }


        // Update is called once per frame
        void Update()
        {
            //StartCoroutine(ReportSensor());
            //Camera.main.transform.rotation = Input.gyro.attitude;
            //speed += rate * emotionManager.HappyRatio * Time.deltaTime;
            //speed -= rate * emotionManager.DisgustRatio * Time.deltaTime;

            transform.Translate(transform.forward * speed * Time.deltaTime);

            if (IsControlActive && !enableNonGameMode)
            {
                if (Application.platform == RuntimePlatform.WindowsEditor)
                    ProcessKeyControls();
                else ProcessControls();
            }

            if (magnetStarted)
            {
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, magnetRange, transform.forward, magnetRange, layerMask, QueryTriggerInteraction.UseGlobal);
                foreach (RaycastHit hit in hits)
                {
                    GameObject currObj = hit.transform.gameObject;
                    ICollectible collect = currObj.GetComponent<ICollectible>();
                    if (collect != null)
                    {
                        if (collect.Type == CollectibleType.Monetary)
                            currObj.GetComponent<CoinCollectible>().SetTarget(gameObject, magnetSpeed);
                    }
                }
            }
        }

        /// <summary>
        /// Process player Keyboard controls (Editor and Windows Test build only!). Not for use together with <see cref="Player.ProcessControls"/>
        /// </summary>
        void ProcessKeyControls()
        {
            //Check for Y acceleration, then find the speed by adding it by the acceleration. Multiply by 0.97 to decay it
            velocityY += accelerometerValue.y * Time.deltaTime;
            velocityY *= 0.97f;

            if (Input.GetKeyDown(KeyCode.W) && canJump) //If Y velocity reaches the high threshold, provided it can jump...
            {
                //Jump and disable player's capability to jump and slide while on air
                Jump();

            }
            else if (Input.GetKeyDown(KeyCode.S) && canSlide) //If Y velocity reaches the low threshold, provided it can slide...
            {
                //Slide and disable player's capability to jump and slide while sliding
                Slide();
            }

            //Check slide direction from velocity. Also check if the player object is on the edge of the lane
            if (Input.GetKeyDown(KeyCode.D) && lane < 2) Sidestep(false);
            else if (Input.GetKeyDown(KeyCode.A) && lane > -2) Sidestep(true);

        }

        /// <summary>
        /// Process player motion controls. Not to be used together with <see cref="Player.ProcessKeyControls"/>
        /// </summary>
        void ProcessControls()
        {
            //Check for Y acceleration, then find the speed by adding it by the acceleration. Multiply by 0.97 to decay it
            velocityY += accelerometerValue.y * Time.deltaTime;
            velocityY *= 0.97f;

            if (velocityY > jumpSpeedThreshold && canJump) //If Y velocity reaches the high threshold, provided it can jump...
            {
                //Jump and disable player's capability to jump and slide while on air
                Jump();
            }
            else if (velocityY < -jumpSpeedThreshold && canSlide) //If Y velocity reaches the low threshold, provided it can slide...
            {
                //Slide and disable player's capability to jump and slide while sliding
                Slide();
            }

            velocityX += accelerometerValue.x * Time.deltaTime;
            velocityX *= 0.97f;

            if (canSidestep)
            {
                //Check slide direction from velocity. Also check if the player object is on the edge of the lane
                if (velocityX > sidestepSpeedThreshold && lane < 2)
                {
                    Sidestep(false);
                    canSidestep = false;
                }
                else if (velocityX < -sidestepSpeedThreshold && lane > -2)
                {
                    Sidestep(true);
                    canSidestep = false;
                }
            }
            else if (velocityX < 0.05f && velocityX > -0.05f) canSidestep = true;
        }

        void Jump()
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * 7, ForceMode.Impulse);
            canJump = false;
            canSlide = false;
        }

        void Slide()
        {
            transform.DOScaleY(initialScale.y / 2, 0.3f);
            StartCoroutine(SlideTime());
        }

        void Sidestep(bool left)
        {
            if (left)
                transform.DOMoveX(1.5f * --lane, 0.5f);
            else
                transform.DOMoveX(1.5f * ++lane, 0.5f);
        }

        float GetDeviceYAngle()
        {
            Quaternion referenceRotation = Quaternion.identity;
            Quaternion deviceRotation = new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) * Input.gyro.attitude * new Quaternion(0, 0, 1, 0);
            Quaternion eliminationOfXZ = Quaternion.Inverse(
                Quaternion.FromToRotation(referenceRotation * Vector3.up,
                                          deviceRotation * Vector3.up)
            );
            Quaternion rotationY = eliminationOfXZ * deviceRotation;
            return rotationY.eulerAngles.y;
        }

        public void OnAccelerometer(float x, float y, float z)
        {
            accelerometerValue = new Vector3(x, y, z);
        }

        IEnumerator SlideTime()
        {
            canSlide = false;
            canJump = false;
            yield return new WaitForSeconds(1f);
            transform.DOScaleY(initialScale.y, 0.3f);
            canSlide = true;
            canJump = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Floor")
            {
                canSlide = true;
                canJump = true;
            }
            else OnCollision?.Invoke(collision);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(new Vector3(transform.position.x, 0.1f, transform.position.z), Vector3.down * 0.2f);
        }

        private void OnTriggerEnter(Collider other)
        {
            ICollectible collect = other.gameObject.GetComponent<ICollectible>();
            if (collect != null)
            {

                if (collect.Type == CollectibleType.Powerups)
                {
                    collect.CollectibleEffect();
                    SoundManager.Instance.PlayPowerUpsSound(PowerUpsSoundsType.Gulp);
                }
                else
                {
                    coinAdded?.Invoke((float)collect.Value);
                    other.gameObject.transform.position = new Vector3(0, 0, -9999);
                    other.gameObject.GetComponent<CoinCollectible>().SetMagnet(false);
                }
            }
        }

        private void SetMagnetProperty(bool activated, int range, int speed)
        {
            magnetActivated = activated;
            magnetRange = range;
            magnetSpeed = speed;
        }


        public void StartMagnetPowerUps(float magnetDuration,float Range)
        {
            StartCoroutine(MagnetPower(magnetDuration));
            magnetRange = Range;
        }

        public void StartPhasePowerUps(float phaseDuration)
        {
            StartCoroutine(PhasePower(phaseDuration));
        }

        public void StartBoostPowerUps(float boostDuration,float boostPhaseDuration)
        {
            StartCoroutine(BoostPower(boostDuration,boostPhaseDuration));
        }

        public void StartGoldenCoinPowerUps(float goldenCoinDuration)
        {
            StartCoroutine(GoldenCoinPower(goldenCoinDuration));
        }
        IEnumerator PhasePower(float phaseDuration)
        {
            print("Phase Worked");
            if (!phaseStarted)
            {
                phaseStarted = true;
                float duration = phaseDuration;
                float timeStamp = Time.time;
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"));
                while (Time.time < timeStamp + duration)
                {
                    phaseTime = Time.time - timeStamp;
                    if (resetPhase)
                    {
                        resetPhase = false;
                        timeStamp = Time.time;
                    }
                    yield return null;
                }
                phaseTime = 0;
                phaseStarted = false;
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"), false);
            }
            else
            {
                resetPhase = true;
            }
        }

        IEnumerator MagnetPower(float magnetDuration)
        {
            if (!magnetStarted)
            {
                magnetStarted = true;
                float duration = magnetDuration;
                float timeStamp = Time.time;
                while (Time.time < timeStamp + duration)
                {
                    magnetTime = Time.time - timeStamp;
                    if (resetMagnet)
                    {
                        resetMagnet = false;
                        timeStamp = Time.time;
                    }
                    yield return null;
                }
                magnetStarted = false;
            }
            else
            {
                resetMagnet = true;
            }
        }

        IEnumerator BoostPower(float boostDuration,float boostPhaseDuration)
        {
            print("Boost PLayer started");
            if (!boostStarted)
            {
                SoundManager.Instance.PlayPowerUpsSound(PowerUpsSoundsType.Boost);
                boostStarted = true;
                float duration = boostDuration;
                float timeStamp = Time.time;
                speedBroadcast?.Invoke(5);
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"));
                while (Time.time < timeStamp + duration)
                {
                    SoundManager.Instance.PlayPowerUpsSound(PowerUpsSoundsType.Boost);
                    boostTime = Time.time - timeStamp;
                    if (resetBoost)
                    {
                        resetBoost = false;
                        timeStamp = Time.time;
                    }
                    yield return null;
                }
                boostStarted = false;
                speedBroadcast?.Invoke(1);
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacles"), false);
                StartCoroutine(PhasePower(boostPhaseDuration));
            }
            else
            {
                resetBoost = true;
            }
        }

        IEnumerator GoldenCoinPower(float goldenCoinDuration)
        {
            print("Golden Coin Player Started");
            if (!goldenCoinStarted)
            {
                goldenCoinStarted = true;
                float duration = goldenCoinDuration;
                float timeStamp = Time.time;
                goldenCoinBroadcast?.Invoke(true);
                while (Time.time < timeStamp + duration)
                {
                    goldenCoinTime = Time.time - timeStamp;
                    if (resetGoldenCoin)
                    {
                        resetGoldenCoin = false;
                        timeStamp = Time.time;
                    }
                    yield return null;
                }
                goldenCoinBroadcast?.Invoke(false);
                goldenCoinStarted = false;
            }
            else
            {
                resetGoldenCoin = true;
            }
        }

      

    }
}