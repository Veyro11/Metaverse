using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GotoScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("flappy") && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene("2d_Flappy");
        }
        else if (collision.CompareTag("stack") && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene("3d_stack");
        }
    }
}
