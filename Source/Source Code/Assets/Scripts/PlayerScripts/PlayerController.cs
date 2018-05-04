using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    float speed;
    float horizontal;  // horizontal input
    float vertical;     //vertical input

    public float currentHealth=100;     //player's current Health
    public float maxHealth=100;         //player's maximum Health

    public float currentAmmo=10;        //player's current ammo
    public float maxAmmo = 10;          //player's maximum ammo

    //sound
    public GameObject soundSourcePosition;  //used to store soundSource    
   public AudioSource playerSource;         //player AudoiSource
    public AudioClip whistle;               //whistle sound
    //bomb  
    public GameObject bullet;               //bullet prefab

    //throw money
    public GameObject soundObj=null;

    public GameObject healthObject;     //health UI
    public Image health;                //health UI VALUE

    public Text ammoText;               //AMMO UI
    public string ammoString;           //AMMO UI VALUE

    

    //key presed
    bool keyPressed = false;

    // Use this for initialization
    void Start ()
    {
       
	}

    // Update is called once per frame
    void Update()
    {
        if (soundObj == null)
            soundObj = this.gameObject;
        health.fillAmount = currentHealth / 100 ;
        ammoText.text = currentAmmo.ToString();
        //Movement
        horizontal = Input.GetAxisRaw("Horizontal") * Time.deltaTime * 150.0f;     
        vertical = Input.GetAxisRaw("Vertical") * Time.deltaTime * 4.0f;            

        //player movement
        transform.Rotate(0, horizontal, 0);
        transform.Translate(0, 0, vertical);

        
        //fire bullet if key F is pressed
        if (Input.GetKey(KeyCode.F) && currentAmmo>0)
        {
            if (keyPressed == false)
            {
                keyPressed = true;
                
                GameObject Instant=Instantiate(bullet, transform.position+transform.forward, transform.rotation);
                Instant.GetComponent<Rigidbody>().velocity = Instant.transform.forward * 10;
                Destroy(Instant, 10.0f);
                currentAmmo--;
            }

        }else
        //make noise if key G is pressed
        if (Input.GetKey(KeyCode.G))
        {
            if (keyPressed==false)
            {
                keyPressed = true;
               

                //instantiate an object to mark the sound source
                GameObject Instant= Instantiate(soundSourcePosition, transform.position, transform.rotation);
                soundObj = Instant;
                playerSource.PlayOneShot(whistle);
                Destroy(Instant, 10.0f);
            }
        }
       
        else
        {
            keyPressed = false;
        }
        
    }
    

    public void OnCollisionEnter(Collision collision)
    {
        //checks if the Player was hit by the AI
        if (collision.gameObject.tag=="AIBullet")
        {
            currentHealth -= 10;
        }

        //if the Player hits a Health Pack health is set to maxhealth
        if (collision.gameObject.tag=="HealthPack")
        {

            currentHealth = maxHealth;
        }

        //if the Player hits a AmmoPack, ammo is set to maxhealth
        if (collision.gameObject.tag=="AmmoPack")
        {
            currentAmmo = maxAmmo;
        }
    }


}
