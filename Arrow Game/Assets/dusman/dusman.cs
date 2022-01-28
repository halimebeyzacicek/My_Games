using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dusman : MonoBehaviour
{
    Animator anim;
    public GameObject spine;
    int count=0;
    public GameObject image;
    public static GameObject finish;//bow da da kullanmak için

    private void Start()
    {
        anim = GetComponent<Animator>();
        finish = image;
    }
    
    IEnumerator finish_bekle()
    {
        yield return new WaitForSeconds(1f);
        finish.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "arrow")//kadýna deyince oyun bitmeli
        {
            other.transform.parent = spine.transform;
            anim.enabled = false;
            StartCoroutine(finish_bekle());
        }
    }
}
