using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveOnPlay : MonoBehaviour
{
    // Probably want to find a way to find all of these during the build process
    // and destroy game objects then.

    void Start()
    {
        Destroy(gameObject);
    }
}
