using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(movement))]//movement kodu

public class inputSystem : MonoBehaviour
{
    movement moveScript;

    [System.Serializable]
    public class InputSettings
    {
        public string forwardInput = "Vertical";//dikey
        public string strafeInput = "Horizontal";//yatay
        public string sprintInput = "Sprint";//Sprint=s�rat ko�usu
        public string aim = "Fire2";
        public string fire = "Fire1";
    }
    [SerializeField]
    public InputSettings input;

    //karakter kameran�n dondugu y�ne ba�l� olarak on-arka-sag-sol de�ilmeli.
    [Header("Camera & Character Syncing")]
    public float lookDistance=5;
    public float lookSpeed = 5;

    [Header("Aiming Settings")]
    RaycastHit hit;
    public LayerMask aimLayers;//hedeflemek istedi�imiz katmanlar
    Ray ray;

    [Header("Spine Settings")]//hedefledi�imizi karakterin omurgas�na d�nd�rmek istiyoruz.
    public Transform spine;
    public Vector3 spineOffset;


    //karakterin hedefledi�i noktaya bakmas�n� istiyoruz.
    [Header("Head Rotation Settings")]
    public float lookAtPoint = 2.8f;

    Transform CamCenter;
    Transform mainCam;

    public bow bowScript;//art� i�aretini g�stermek i�in.
    bool isAming;

    public bool testAim;

    bool hitDetected;//isabet tespit edildi.//art� i�areti uzaklarda g�r�nm�yo.

    // Start is called before the first frame update
    void Start()
    {
        moveScript=GetComponent<movement>();
        CamCenter = Camera.main.transform.parent;//transfrom.parent = D�n���m�n ebeveyni.Ebeveynin de�i�tirilmesi ebeveyne g�re konumu, �l�e�i ve d�n��� de�i�tirir, ancak d�nya alan� konumunu, d�n���n� ve �l�e�ini ayn� tutar.
        mainCam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis(input.forwardInput) != 0 || Input.GetAxis(input.strafeInput) != 0)//bir tusa basm�yorsa //hareket etmiyorsak adam� dondurme.
            RotateToCamView();
        
        isAming = Input.GetButton(input.aim);

        if (testAim)
            isAming = true;

        if (bowScript.bowSettings.arrowCount < 1)
            isAming = false;

        moveScript.AnimateCharacter(Input.GetAxis(input.forwardInput), Input.GetAxis(input.strafeInput));
        moveScript.SprintCharacter(Input.GetButton(input.sprintInput));
        moveScript.CharacterAim(isAming);

        if (isAming)//ni�an ald�ysa
        {
            Debug.Log("bu �al���yo");
            Aim();
            bowScript.EquipBow();

            if (bowScript.bowSettings.arrowCount >0)//ok yoksa ipi de �ekememeli
                moveScript.CharacterPullString(Input.GetButton(input.fire));

            if (Input.GetButtonUp(input.fire))
            {
                moveScript.CharacterFireArrow();
                if (hitDetected)//art� i�areti varsa.
                {
                    bowScript.Fire(hit.point);
                }
                else
                {
                    bowScript.Fire(ray.GetPoint(300f));
                }
            }
               
        }
        else//ni�an alm�yorsak art� i�aretini ve oku serbest b�rak.
        {
            bowScript.UnEquipBow();
            bowScript.RemoveCrosshair();
            bowScript.DisableArrow();
            Release();
        } 
    }

    void LateUpdate()
    {
        if (isAming)
            RotateCharacterSpine();
    }

    void RotateToCamView()
    {
        Vector3 camCenterPos = CamCenter.position;
        Vector3 lookPoint = camCenterPos + (CamCenter.forward * lookDistance);
        Vector3 direction = lookPoint - transform.position;//y�n

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        lookRotation.x = 0;//karakteri bu y�nlerde d�nd�rmek istemiyoruz.
        lookRotation.z = 0;

        Quaternion finalRotation = Quaternion.Lerp(transform.rotation, lookRotation, lookSpeed * Time.deltaTime);

        transform.rotation = finalRotation;
    }

    void Aim()//ni�an al�r ve bir hedefe ���n yay�n� g�nderir
    {
        Vector3 camPosition = mainCam.position;
        Vector3 dir = mainCam.forward;//kameranin y�n�

        ray = new Ray(camPosition, dir);//parametreler = ba�lang��, y�n
        if(Physics.Raycast(ray,out hit, 500f, aimLayers))//hit = �arpt�g�m�z nsneleri depolayacak //500 seyahat etmesni istiyoruz. //hedeflemek istedi�imiz katman
        {
            hitDetected = true;
            Debug.DrawLine(ray.origin, hit.point, Color.green);//ba�lang�ctan �arp�lan yere kadar ���n ��kar.
            bowScript.ShowCrosshair(hit.point);
        }
        else//herhangi bir nesneye �arpmazsak art� i�aretini sahneden kald�rmak istiyoruz.
        {
            hitDetected = true;
            bowScript.RemoveCrosshair();
        }
        //karakterin omurgas�n� hedefe d�nd�rmek
    }
    void RotateCharacterSpine()
    {
        spine.LookAt(ray.GetPoint(50));//500 den 50. 50 birime bakacak.
        spine.Rotate(spineOffset);
    }

    public void Pull()
    {
        bowScript.PullString();
    }

    public void EnableArrow()
    {
        bowScript.PickArrow();
    }

    public void DisableArrow()
    {
        bowScript.DisableArrow();
    }

    public void Release()
    {
        bowScript.ReleaseString();
    }
}
