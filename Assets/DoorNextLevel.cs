using UnityEngine;

public class DoorNextLevel : MonoBehaviour
{
    BoxCollider2D coll;

    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        coll.enabled = false;
    }

    public void OpenNextLevel()
    {
        coll.enabled = true;
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
        SceneLoadingManager.NextLevel();
    }


}
