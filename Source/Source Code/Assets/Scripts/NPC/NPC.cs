using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
   
    [Header("Object/Code Reference")]
    public GameObject bullet;               //Gameobject use to create bullet instances
    public SteeringBehaviours behaviors;    //SteeringBehaviors code reference
    public Pathfinding pathfinding;         //Pathfinding code reference

    //auditive sense
    public AudioSource playerAudioSource;   //player audio source reference
    public Vector3 soundSource;             //sound osurce position
    public bool soundPositionReache = true; //checks if the sound source was reached

    //Player Data
    public GameObject player;               //reference to the player
    public PlayerController playerCode;     //reference to the player code
    public Vector3 playerLastPosition;             //saves player's last detected position
    public bool playerLastPositionReached = true;

    [Header("Desirebility")]
    public float healthDesire;              //stores health desirability value
    public float ammoDesire;                //stores ammo desirability value

    [Header("Health")]
    public float currentHealth = 100; //AIs current health 
    public float maxHealth = 100;     //AIs maximum health
    public Image healthBar;           //reference to AI's health bar

    [Header("Ammo")]
    public int currentAmmo=10;      //AI's current ammo
    public int maxAmmo=10;          //AI's maximum ammo

    [Header("AI Checks")]
    public bool canSee;                    //detects if the AI can see the player
    public bool soundPositionReached = true;
    public bool canHearPlayer;             //checks if the AI can hear the player

    [Header("Ammo/Health")]
    //AMMO & HEALTH
    public GameObject[] healthPacks;        //health packs array
    public GameObject[] ammoPacks;          //ammo packs array
    public GameObject closestHealth;        //closest health pack
    public GameObject closestAmmo;          //closest ammo pack

    [Header("Path")]
    //Paths
    public Path pathCode;           //used to access the pathfinding code
    public List<Transform> globalWaypoints; //used to store global waypoints
    public Vector3[] localWaypoints;    //used to store local waypoints
    public GameObject targetPos;
    
    //visual sense
    float fov = 130.0f;             //AI's field of view
    float distance = 15.0f;         //AI's view range


    //fire delay
    public float fireTime = 1.5f; //delay between shoots
    public float nextFire; //calculates when the AI is able to fire again


    private void Awake()
    {
        targetPos = this.gameObject;
        playerCode = player.GetComponent<PlayerController>();   //get playerCode script
        behaviors = GetComponent<SteeringBehaviours>();         //get SteeringBehavior script 

      
        healthPacks = GameObject.FindGameObjectsWithTag("HealthPack");  //get all health packs in the scene
        ammoPacks = GameObject.FindGameObjectsWithTag("AmmoPack");      //get all the ammo packs in the scene

    }
    private void Update()
    {
       
            behaviors.globalWaypoints = globalWaypoints;



            healthDesire = Mathf.Clamp(healthDesirability(), 0, 1); //calculate health desirability
              ammoDesire = Mathf.Clamp(ammoDesirability(), 0, 1);   //calculate ammo desirability


            Health(healthPacks);    //tests if the AI collided with health packs
            Ammo(ammoPacks);        //test if the AI collided with ammo packs

            closestHealth = closestToPlayer(healthPacks);       //gets the closest health pack to the player
            closestAmmo = closestToPlayer(ammoPacks);           //gets the closest ammo pack to the player

            healthBar.fillAmount = currentHealth / 100.0f;      //set health bar value

            canHearPlayer = checkSound();                   //checks player's sound
            canSee = playerInSight(player);                     //test if the AI can see the player

       
       
    }
    
    
    //Checks if player is in sight
    
    public bool playerInSight(GameObject player)
    {
        float inFront = Vector3.Dot(transform.position, player.transform.position); //checks if the player is in front of the player (negative result => player is behind, positive resulte => player is in front)
        float angle = Vector3.Angle(player.transform.position - transform.position, transform.forward); //calculates angle between AI an player
        RaycastHit hit;

        if (angle < fov / 2 && inFront > 0.0f) // checks if the angle between the AI and the player is in the FOV range and if the player is in the fornt of the player
        {
            if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, distance)) //Use raycast to test if there is any object between the player ant the AI
            {
                
                if (hit.collider.tag == "Player" || hit.collider.tag=="AIBullet")
                {
                    playerLastPositionReached = false;
                    playerLastPosition = player.transform.position;
                    //add last player position
                    return true;
                }
            }
        }
        return false;
    }
    
    //check noise
    public bool checkSound()
    {
        //check distance to the player. Distance is calculated using Pathfinding
        if (pathCode.GetPathLength(transform.position, soundSource) < 20)
        {
            //checks if the audio source is plating
            if (playerAudioSource.isPlaying)
            {
                soundPositionReached = false;
                soundSource = player.transform.position; //set audio source position
                return true;
            }
            else
            {
                return false;
            }

        }
        return false;
    }
    

    //This function instantianates and shoots bullets
    public void shoot()
    {
        Vector3 direction = player.transform.position - transform.position; //calculates AI to player directions

        //fire delay
        if (Time.time > nextFire)       
        {
            //delay next shoot
            nextFire = Time.time + fireTime;
            StartCoroutine(Delay());

            //create bullet instant
            GameObject Instant = Instantiate(bullet, transform.position + transform.forward, transform.rotation);
            Instant.GetComponent<Rigidbody>().velocity = Instant.transform.forward+direction * 5;
            currentAmmo--;
            //destroy bullet instant
            Destroy(Instant, 2.0f);
        }
    }

    //This function is used to get the closest ammo and health packs to the player
    public GameObject closestToPlayer(GameObject[] arrayOfObjects)
    {
        //initialise the closest point and the distance to the closest point
        GameObject closestPoint = arrayOfObjects[0];
        float distanceToCompare = Vector3.Distance(transform.position, arrayOfObjects[0].transform.position);

        //calculate closest point
        for (int i = 1; i < arrayOfObjects.Length; i++)
        {
            float newDistance = Vector3.Distance(transform.position, arrayOfObjects[i].transform.position);
            if (newDistance < distanceToCompare)
            {
                distanceToCompare = newDistance;
                closestPoint = arrayOfObjects[i];
            }
        }
        return closestPoint;
    }

    

    //check distance between player and health box
    void Health(GameObject[] aidBox)
    {
        foreach (GameObject i in aidBox)
            if (Vector3.Distance(transform.position, i.transform.position) < 2.0f)
            {
                //refill health
                currentHealth = maxHealth;
            }
    }

    //check distance between player and ammo box
    void Ammo(GameObject[] ammoBox)
    {
        foreach (GameObject i in ammoBox)
            if (Vector3.Distance(transform.position, i.transform.position) < 2.0f)
            {
                //refill ammo
                currentAmmo = maxAmmo;
            }
    }

    public bool isHit = false;
    //Damage given to the AI on collision with the Player's bullet
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerBullet")
        {
            currentHealth -= 10;
            isHit = true;
        }

    }
    //calculate health regeneration desirability
    float healthDesirability()
    {
        float value = 0;
        value = (1 - currentHealth / 100) / Vector3.Distance(transform.position,closestToPlayer(healthPacks).transform.position);
        return value*20;
    }

    //calculate ammo reload desirability
    float ammoDesirability()
    {
        float value = 0;
        value = ((currentHealth / 100) * (1 - currentAmmo / 10)) / Vector3.Distance(transform.position, closestToPlayer(ammoPacks).transform.position);
        return value*5f;
    }

    //delays the AI's fire actions
    private IEnumerator Delay()
    {
        yield return fireTime;
    }
    
}