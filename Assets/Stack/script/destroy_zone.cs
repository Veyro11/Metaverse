using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroy_zone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("Rubble"))
        {
            Destroy(collision.gameObject);
        }
    }
}
