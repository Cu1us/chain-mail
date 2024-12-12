using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TestSoundSwing : MonoBehaviour
{
    Chain Chain;
    Transform player1;
    Transform player2;
    float timer;
    float x;
    float y;
    [SerializeField] float z = 1;
    [SerializeField] float factor;
    void Start()
    {
        Chain = GameObject.Find("Chain and Players").GetComponent<Chain>();
        player1 = GameObject.Find("Player1").GetComponent<Transform>();
        player2 = GameObject.Find("Rock").GetComponent<Transform>();
        z = 1;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(Chain.rotationalVelocity != 0)
        {
             z +=Math.Abs( Chain.rotationalVelocity / Chain.currentChainLength * factor * Time.deltaTime);
        }
       
       //Debug.Log(Chain.rotationalVelocity / Chain.currentChainLength);
        timer += Time.deltaTime;

        if(Chain.rotationalVelocity != 0 && z > 10 && timer > 0.7)
        {
            z = 0;
            timer = 0;
            AudioManager.Play("ChainSwing");
        }
    }
}
