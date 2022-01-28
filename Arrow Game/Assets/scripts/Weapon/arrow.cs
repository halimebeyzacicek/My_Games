using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class arrow : MonoBehaviour
{
    public TextMeshProUGUI text_point;

    Rigidbody rb;
    BoxCollider bx;
    bool disableRotation;//dödürmeyi kapat.
    public float destroyTime=10f;//oku bir süre sonra yok etmeliyiz.

    public static int point;
    public static int red_count = 0;//yeni ok üretildiðinde 
    public static int blue_count = 0;
    public static int green_count = 0;
    public static int orange_count = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bx = GetComponent<BoxCollider>();
        text_point = GameObject.Find("point_text").GetComponent<TMPro.TextMeshProUGUI>();

        //Destroy(this.gameObject, destroyTime);
    }

    private void Update()
    {
        if (!disableRotation)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }

    private void OnCollisionEnter(Collision collision)//ok herhangi bir objeye çarptýðýnda 
    {
        if (collision.gameObject.tag != "Player")//oklar adamýn kendi collider ine çarpýyo.
        {
            disableRotation = true;
            rb.isKinematic = true;
            bx.isTrigger = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "tahta_red")
        {
            red_count =red_count+ 1;
            Debug.Log("red"+red_count);
            point_heapla();
        }
        if (other.tag == "tahta_blue")
        {
            blue_count += 1;
            Debug.Log("blue" + blue_count);
            point_heapla();
        }
        if (other.tag == "tahta_green")
        {
            green_count += 1;
            Debug.Log("green" + green_count);
            point_heapla();
        }
        if (other.tag == "tahta_orange")
        {
            orange_count += 1;
            Debug.Log("orange" + orange_count);
            point_heapla();
        }
    }
    private void point_heapla()
    {
        point = 0;
        point = (red_count * 1) + (blue_count * 2) + (green_count * 3) + (orange_count * 5);
        text_point.text = "point: " + point;
    } 
}
