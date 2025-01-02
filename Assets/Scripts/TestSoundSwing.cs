using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TestSoundSwing : MonoBehaviour
{
    Chain Chain;
    float timer;
    [SerializeField] float z = 1;
    float factor = 0.08f;
    void Start()
    {
        Chain = GetComponent<Chain>();
        z = 1;
        PlayMusic.StartMusic();
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (Chain.rotationalVelocity != 0)
        {
            z += Math.Abs(Chain.rotationalVelocity / Chain.currentChainLength * factor * Time.deltaTime);
        }
        timer += Time.deltaTime;

        if (z > 10 && timer > 0.7)
        {
            z = 0;
            timer = 0;
            AudioManager.Play("ChainSwing");
        }
    }
}
