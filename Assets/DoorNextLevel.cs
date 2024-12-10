using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorNextLevel : MonoBehaviour
{
    BoxCollider2D coll;

    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        coll.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Door");
        if (other.CompareTag("Player"))
        {
            Invoke(nameof(NextLevel), 1f);
        }
    }

    void NextLevel()
    {
       // GameObject gameManager = GameObject.Find("GameManager");
        //gameManager.GetComponent<SceneLoadingManager>().NextLevel();
        SceneLoadingManager.NextLevel();
    }

    public void OpenNextLevel()
    {
        coll.enabled = true;
    }
}
