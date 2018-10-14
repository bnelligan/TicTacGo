/* Brendan Nelligan
 * June 2018
 * Fox Cub Interview
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {
    const float DegToRad = Mathf.PI / 180f;
    const float RadToDeg = 180f / Mathf.PI;
    
    public float minAngle = 0f;
    public float maxAngle = 20f;
    public float minSpeed = 5f;
    public float maxSpeed = 8f;

    private Vector3 _velocity = Vector3.zero;
    private Vector3 _gravity = new Vector3(0, -8f, 0);
    private float launchAngle = 0f;
    private float launchSpeed;
    bool launched = false;
    
	
	// Update is called once per frame
	void Update () {
        if(launched)
        {
            _velocity += _gravity * Time.deltaTime;
            transform.position += _velocity * Time.deltaTime;
        }
        
	}
    public void Launch()
    {
        // Choose a random launch angle and speed
        if (launchAngle >= 180f)
            launchAngle -= Random.Range(minAngle, maxAngle);
        else
            launchAngle += Random.Range(minAngle, maxAngle);
        launchSpeed = Random.Range(minSpeed, maxSpeed);

        // Set the velocity
        _velocity.x = Mathf.Cos(launchAngle * DegToRad);
        _velocity.y = Mathf.Sin(launchAngle * DegToRad);
        _velocity *= launchSpeed;
        launched = true;
        Destroy(gameObject, 2);
    }

    public void LaunchRight()
    {
        launchAngle = 0f;
        Launch();
    }
    public void LaunchLeft()
    {
        launchAngle = 180f;
        Launch();
    }
}
