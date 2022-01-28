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
        public string sprintInput = "Sprint";//Sprint=sürat koþusu
        public string aim = "Fire2";
        public string fire = "Fire1";
    }
    [SerializeField]
    public InputSettings input;

    //karakter kameranýn dondugu yöne baðlý olarak on-arka-sag-sol deðilmeli.
    [Header("Camera & Character Syncing")]
    public float lookDistance=5;
    public float lookSpeed = 5;

    [Header("Aiming Settings")]
    RaycastHit hit;
    public LayerMask aimLayers;//hedeflemek istediðimiz katmanlar
    Ray ray;

    [Header("Spine Settings")]//hedeflediðimizi karakterin omurgasýna döndürmek istiyoruz.
    public Transform spine;
    public Vector3 spineOffset;


    //karakterin hedeflediði noktaya bakmasýný istiyoruz.
    [Header("Head Rotation Settings")]
    public float lookAtPoint = 2.8f;

    Transform CamCenter;
    Transform mainCam;

    public bow bowScript;//artý iþaretini göstermek için.
    bool isAming;

    public bool testAim;

    bool hitDetected;//isabet tespit edildi.//artý iþareti uzaklarda görünmüyo.

    // Start is called before the first frame update
    void Start()
    {
        moveScript=GetComponent<movement>();
        CamCenter = Camera.main.transform.parent;//transfrom.parent = Dönüþümün ebeveyni.Ebeveynin deðiþtirilmesi ebeveyne göre konumu, ölçeði ve dönüþü deðiþtirir, ancak dünya alaný konumunu, dönüþünü ve ölçeðini ayný tutar.
        mainCam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis(input.forwardInput) != 0 || Input.GetAxis(input.strafeInput) != 0)//bir tusa basmýyorsa //hareket etmiyorsak adamý dondurme.
            RotateToCamView();
        
        isAming = Input.GetButton(input.aim);

        if (testAim)
            isAming = true;

        if (bowScript.bowSettings.arrowCount < 1)
            isAming = false;

        moveScript.AnimateCharacter(Input.GetAxis(input.forwardInput), Input.GetAxis(input.strafeInput));
        moveScript.SprintCharacter(Input.GetButton(input.sprintInput));
        moveScript.CharacterAim(isAming);

        if (isAming)//niþan aldýysa
        {
            Debug.Log("bu çalýþýyo");
            Aim();
            bowScript.EquipBow();

            if (bowScript.bowSettings.arrowCount >0)//ok yoksa ipi de çekememeli
                moveScript.CharacterPullString(Input.GetButton(input.fire));

            if (Input.GetButtonUp(input.fire))
            {
                moveScript.CharacterFireArrow();
                if (hitDetected)//artý iþareti varsa.
                {
                    bowScript.Fire(hit.point);
                }
                else
                {
                    bowScript.Fire(ray.GetPoint(300f));
                }
            }
               
        }
        else//niþan almýyorsak artý iþaretini ve oku serbest býrak.
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
        Vector3 direction = lookPoint - transform.position;//yön

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        lookRotation.x = 0;//karakteri bu yönlerde döndürmek istemiyoruz.
        lookRotation.z = 0;

        Quaternion finalRotation = Quaternion.Lerp(transform.rotation, lookRotation, lookSpeed * Time.deltaTime);

        transform.rotation = finalRotation;
    }

    void Aim()//niþan alýr ve bir hedefe ýþýn yayýný gönderir
    {
        Vector3 camPosition = mainCam.position;
        Vector3 dir = mainCam.forward;//kameranin yönü

        ray = new Ray(camPosition, dir);//parametreler = baþlangýç, yön
        if(Physics.Raycast(ray,out hit, 500f, aimLayers))//hit = çarptýgýmýz nsneleri depolayacak //500 seyahat etmesni istiyoruz. //hedeflemek istediðimiz katman
        {
            hitDetected = true;
            Debug.DrawLine(ray.origin, hit.point, Color.green);//baþlangýctan çarpýlan yere kadar ýþýn çýkar.
            bowScript.ShowCrosshair(hit.point);
        }
        else//herhangi bir nesneye çarpmazsak artý iþaretini sahneden kaldýrmak istiyoruz.
        {
            hitDetected = true;
            bowScript.RemoveCrosshair();
        }
        //karakterin omurgasýný hedefe döndürmek
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
