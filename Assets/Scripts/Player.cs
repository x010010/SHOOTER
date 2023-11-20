using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject explosionPrefab;
    public float playerSpeed;
    private float horizontalScreenLimit = 10f;
    private float verticalScreenLimit = 4f;
    public int lives;
    public GameObject gM;
    public AudioClip coinSound;
    public AudioClip healthSound;
    public AudioClip powerupSound;
    public AudioClip powerdownSound;
    private bool betterWeapon;
    public GameObject thruster;

    // Start is called before the first frame update
    void Start()
    {
        playerSpeed = 6f;
        betterWeapon = false;
        lives = 3;
        gM = GameObject.Find("GameManager");
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Shooting();
    }

    void Movement()
    {
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * Time.deltaTime * playerSpeed);
        if (transform.position.x > horizontalScreenLimit || transform.position.x <= -horizontalScreenLimit)
        {
            transform.position = new Vector3(transform.position.x * -1f, transform.position.y, 0);
        }
        if (transform.position.y < -verticalScreenLimit)
        {
            transform.position = new Vector3(transform.position.x, -verticalScreenLimit, 0);
        } else if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
    }

    void Shooting()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !betterWeapon)
        {
            Instantiate(bulletPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        } else if(Input.GetKeyDown(KeyCode.Space) && betterWeapon)
        {
            Instantiate(bulletPrefab, transform.position + new Vector3(0.5f, 1, 0), Quaternion.Euler(0, 0, -45f)); ;
            Instantiate(bulletPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            Instantiate(bulletPrefab, transform.position + new Vector3(-0.5f, 1, 0), Quaternion.Euler(0, 0, 45f));
        }
    }

    public void LoseLife()
    {
        lives--;
        //lives -= 1;
        //lives = lives - 1;
        gM.GetComponent<GameManager>().LivesChange(lives);
        if (lives <= 0) 
        {
            //Game Over
            gM.GetComponent<GameManager>().GameOver();
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch(collision.name)
        {
            case "Coin(Clone)":
                //I picked a coin!
                AudioSource.PlayClipAtPoint(coinSound, transform.position);
                gM.GetComponent<GameManager>().EarnScore(1);
                Destroy(collision.gameObject);
                break;
            case "Health(Clone)":
                //I picked a health!
                AudioSource.PlayClipAtPoint(healthSound, transform.position);
                if (lives >= 3)
                {
                    gM.GetComponent<GameManager>().EarnScore(1);
                } else if (lives < 3)
                {
                    lives++;
                    gM.GetComponent<GameManager>().LivesChange(lives);
                }
                Destroy(collision.gameObject);
                break;
            case "Powerup(Clone)":
                //I picked a powerup!
                AudioSource.PlayClipAtPoint(powerupSound, transform.position);
                Destroy(collision.gameObject);
                int tempInt;
                tempInt = Random.Range(1, 4);
                if (tempInt == 1)
                {
                    playerSpeed = 10f;
                    StartCoroutine("SpeedPowerDown");
                    gM.GetComponent<GameManager>().PowerupChange("Speed");
                    thruster.SetActive(true);
                } else if (tempInt == 2)
                {
                    betterWeapon = true;
                    StartCoroutine("WeaponPowerDown");
                    gM.GetComponent<GameManager>().PowerupChange("Weapon");
                } else if (tempInt == 3)
                {
                    //Shield Powerup
                    gM.GetComponent<GameManager>().PowerupChange("Shield");
                }
                break;
        }
    }

    IEnumerator SpeedPowerDown ()
    {
        yield return new WaitForSeconds(4f);
        AudioSource.PlayClipAtPoint(powerdownSound, transform.position);
        playerSpeed = 6f;
        thruster.SetActive(false);
        gM.GetComponent<GameManager>().PowerupChange("No Powerup");
    }

    IEnumerator WeaponPowerDown()
    {
        yield return new WaitForSeconds(4f);
        AudioSource.PlayClipAtPoint(powerdownSound, transform.position);
        betterWeapon = false;
        gM.GetComponent<GameManager>().PowerupChange("No Powerup");
    }

}
