using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorExplosionTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<IntroHelper>().gameObject.SetActive(false);
        GetComponent<CameraView>().RequestView();
    }
}
