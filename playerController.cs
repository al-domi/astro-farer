using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    // rotate variables
    public float tiltAngle = 90.0f;
    float amountRotate = 0.0f;
    public Vector2 awayObject;

    // movement variables
    public float baseSpeed = 0.03f;
    public float maxSpeed = 1f;
    float prevSpeedX = 0.0f;
    float prevSpeedY = 0.0f;

    // gravity variables
    public float mass = 1.0f;
    public float gravationalCon = 1.0f;
    public bool inField = false;
    public float massOfPlanet = 10.0f;

    public bool explode = false;
    int clock = 0;

    public ParticleSystem mainExhaustParticles;
    public ParticleSystem rightExhaustParticles;
    public ParticleSystem leftExhaustParticles;
    public ParticleSystem topExhaustParticles;
    public ParticleSystem fireParticles;
    
    IEnumerator waiter() {
        Vector2 position = transform.position;
        
        fireParticles.Play();
        yield return new WaitForSeconds(3);
        fireParticles.Stop();
        
        position.x = -15.0f;
        position.y= -5.0f;
        transform.position = position;
        prevSpeedX = 0.00f;
        prevSpeedY = 0.00f;

    }

    // Start is called before the first frame update   
    void Start()
    {
        // find the particle systems
        mainExhaustParticles = GameObject.Find("mainExhaust").GetComponent<ParticleSystem>();
        rightExhaustParticles = GameObject.Find("rightExhaust").GetComponent<ParticleSystem>();
        leftExhaustParticles = GameObject.Find("leftExhaust").GetComponent<ParticleSystem>();
        topExhaustParticles = GameObject.Find("topExhaust").GetComponent<ParticleSystem>();

        fireParticles = GameObject.Find("fire").GetComponent<ParticleSystem>();
        fireParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {   
        // position vector
        Vector2 position = transform.position;
        float gravitySpeedX = 0.0f;
        float gravitySpeedY = 0.0f;
        float gravityDifX = 0.0f;
        float gravityDifY = 0.0f;
        float rotatePlayer = 0.0f;
        float gravityRotate = 0.0f;
        float radius = 0;
        
        // respawn
        if (explode == true) {
            
            StartCoroutine(waiter());
            explode = false;
        }

        // extra values
        if (inField == true) {
            gravityDifX = awayObject.x - position.x;
            gravityDifY = awayObject.y - position.y;
            radius = Mathf.Sqrt(gravityDifX * gravityDifX + gravityDifY * gravityDifY);
        }
            

        // keyboard input
        float keyPressTurn = Input.GetAxis("Horizontal") * tiltAngle;
        float keyPressSpeed = Input.GetAxis("Vertical") * baseSpeed;
        float keyPressRotate = keyPressTurn * Time.deltaTime * (-1);

        // reset location
        //if (Input.GetAxis("Fire1") > 0) {
            //position.x = position.x + 0.1f;
            //position.y = 0.0f;
            //transform.position = position;
        //}
        if (inField == true) {
            // get angle of object
            float gravityRad = Mathf.Atan(gravityDifY/gravityDifX);
            float gravityPrinceAngle = gravityRad * 180 / Mathf.PI;
            float gravityAngle = 0.0f;

            // divide from quadrants into the good angles
            if (gravityDifX > 0 && gravityDifY > 0) {
                // Q1
                gravityAngle = -90 + gravityPrinceAngle;
            } else if (gravityDifX > 0 && gravityDifY < 0) {
                // Q4
                gravityAngle = -90 + gravityPrinceAngle;
            } else if (gravityDifX < 90 && gravityDifY > 0) {
                // Q2
                gravityAngle = 90 + gravityPrinceAngle;
            } else if (gravityDifX < 0 && gravityDifY < 0) {
                // Q3
                gravityAngle = 90 + gravityPrinceAngle;
            } else if (gravityDifX == 0 && gravityDifY > 0) {
                gravityAngle = 0;
            } else if (gravityDifX == 0 && gravityDifY < 0) {
                gravityAngle = 180;
            }
            /*
            Debug.Log( "angle ----------------------------------" + gravityAngle);
            Debug.Log(gravityDifX + " x ");
            Debug.Log(gravityDifY + " y ");
            Debug.Log(gravityPrinceAngle + " pric ");
            */
            // rotate player
            float gravityDif = gravityAngle - amountRotate;
            gravityRotate = gravityDif * Time.deltaTime;
        }
            
        // add together
        rotatePlayer = keyPressRotate + gravityRotate;

        // rotate
        transform.Rotate(0, 0, rotatePlayer , Space.Self);
        amountRotate = amountRotate + rotatePlayer;
        
        // amount rotated
        if (amountRotate >= 360.0f) {
            amountRotate = amountRotate - 360.0f;
        } else if (amountRotate <= -360.0f) {
            amountRotate = amountRotate + 360.0f;
        }
        
        // get the componets for the directional speed
        float amountRad = -1 * (amountRotate * Mathf.PI / 180);
        float directionalSpeedX = Mathf.Sin(amountRad) * keyPressSpeed; 
        float directionalSpeedY = Mathf.Cos(amountRad) * keyPressSpeed; 

        // gravity speed
        if (inField == true) {
            float gravitySpeed = (gravationalCon * (mass + massOfPlanet)) / (mass * radius );
            float gravityRatio = Mathf.Sqrt(gravitySpeed / (gravitySpeed / (gravityDifX * gravityDifX + gravityDifY * gravityDifY)));
            gravitySpeedX = gravityRatio * gravityDifX;
            gravitySpeedY = gravityRatio * gravityDifY;
        }
        

        // final speed
        float finalSpeedX = directionalSpeedX  + prevSpeedX;
        float finalSpeedY = directionalSpeedY + prevSpeedY;
        /*
        Debug.Log(finalSpeedX + " final " + finalSpeedY);
        Debug.Log(directionalSpeedX + " direction " + directionalSpeedY);
        Debug.Log(gravitySpeedX + " gravity " + gravitySpeedY);
        Debug.Log(prevSpeedX + " prev " + prevSpeedY);
        */
        
        // make sure it doesn't go over cap
        if (Mathf.Abs(finalSpeedX) > maxSpeed) {
            if (finalSpeedX > 0) {
                finalSpeedX = maxSpeed;
            } else {
                finalSpeedX = -maxSpeed;
            }
        }
        if (Mathf.Abs(finalSpeedY) > maxSpeed) {
            if (finalSpeedY > 0) {
                finalSpeedY = maxSpeed;
            } else {
                finalSpeedY = -maxSpeed;
            }
        }
        position.x = position.x + finalSpeedX * Time.deltaTime;
        position.y = position.y + finalSpeedY * Time.deltaTime;
        

        // set next speed
        prevSpeedX = finalSpeedX;
        prevSpeedY = finalSpeedY;

        // if off screen
        if (position.y > 18) {
            position.y = -18;
        } else if (position.y < -18) {
            position.y = 18;
        } else if (position.x > 35) {
            position.x = -35;
        } else if (position.x < -35) {
            position.x = 35;
        } 

        // move

        transform.position = position;

        // turn on/off exhaust
        if (Input.GetKey("w")) {
            mainExhaustParticles.Play();
        } else {
            mainExhaustParticles.Stop();
        }

        if (Input.GetKey("a")) {
            leftExhaustParticles.Play();
        } else {
            leftExhaustParticles.Stop();
        }

        if (Input.GetKey("d")) {
            rightExhaustParticles.Play();
        } else {
            rightExhaustParticles.Stop();
        }

        if (Input.GetKey("s")) {
            topExhaustParticles.Play();
        } else {
            topExhaustParticles.Stop();
        }
        Debug.Log(inField);
        // clock for inField
        if (clock == 30) {
            inField = false;
            clock = 0;
        } else {
            clock = clock + 1;
        }
    }
}
