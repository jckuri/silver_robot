using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FireMovement : MonoBehaviour {

    //[SerializeField] public Transform transform;
    public const float fireSpeed = 0.4f; //0.2f; // 0.1f;
    [NonSerialized] private float horizontalSpeed = -fireSpeed;

    void Awake() {
        horizontalSpeed = -fireSpeed;
    }

    // Start is called before the first frame update
    void Start() {     
        horizontalSpeed = -fireSpeed;
    }

    // Update is called once per frame
    void Update() {
        if(horizontalSpeed > 0) horizontalSpeed = fireSpeed; else horizontalSpeed = -fireSpeed;
        transform.position = new Vector2(transform.position.x + horizontalSpeed * Time.timeScale, transform.position.y);
    }

    void OnTriggerEnter2D(Collider2D other) { 
        string tag = other.gameObject.tag;
        if(tag == "Player") {
            Game.instance.GameOver();
        }
        if(tag == "FireLimit") {
            horizontalSpeed *= -1;
        }
    }

}