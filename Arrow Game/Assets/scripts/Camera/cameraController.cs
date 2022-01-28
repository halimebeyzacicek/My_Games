using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]//Bir komut dosyas�n�n t�m �rneklerinin D�zenleme Modunda y�r�t�lmesini sa�lar.Varsay�lan olarak, MonoBehaviours yaln�zca Oynatma Modunda y�r�t�l�r. Bu �zniteli�i ekleyerek, MonoBehaviour'un herhangi bir �rne�i, Edit�r D�zenleme Modundayken de geri �a��rma i�levlerini y�r�tecektir.


public class cameraController : MonoBehaviour
{
    [System.Serializable]
    public class CameraSettings
    {
        [Header("Camera Move Settings")]
        public float zoomSpeed = 5;
        public float moveSpeed = 5;
        public float rotationSpeed = 5;
        public float originalFieldofView = 70;//orijinal g�r�� alan� 
        public float zoomFieldofView = 20;//yak�nla�t�rma G�r�� Alan�
        public float MouseX_Sensitivity = 5;//MouseX_Hassasiyeti
        public float MouseY_Sensitivity = 5;
        public float MaxClampAngle = 90;//Maksimum Kelep�e A��s�
        public float MinClampAngle = -30;//Minimum Kelep�e A��s�

        [Header("Camera Collision")]
        public Transform camPosition;
        public LayerMask camCollisionLayers;//�arpo�ma=collision
       
    }
    [SerializeField]
    public CameraSettings cameraSettings;

    [System.Serializable]
    public class CameraInputSettings
    {
        public string mouseXAxis = "Mouse X";
        public string mouseYAxis = "Mouse Y";
        public string AimingInput = "Fire2";//Hedefleme Giri�i
    }
    [SerializeField]
    public CameraInputSettings inputSettings;

    Transform center;
    Transform target;

    Camera mainCam;
    Camera UICam;

    float cameraXrotation = 0;
    float cameraYrotation = 0;

    Vector3 initialCamPos; //initial=ilk
    RaycastHit hit;//���n yay�n vuru�u

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        UICam = mainCam.GetComponentInChildren<Camera>();
        center = transform.GetChild(0);//camera holder'in ilk �ocugu center. onu center a at�yoruz.
        FindPlayer();
        initialCamPos = mainCam.transform.localPosition;//initial=ilk
    }

    // Update is called once per frame
    void Update()
    {
        if (!target)//bir targete sahip de�ilsek.
            return;

        if (!Application.isPlaying)
            return;

        RotateCamera();
        ZoomCamera();
        HandleCamCollision();
    }

    void LateUpdate()
    {
        if (target)//karakteri takip etmek isteriz.
        {
            FollowPlayer();
        }
        else
        {
            FindPlayer();
        }
    }

    void FindPlayer()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;//hedef karakterimiz.
    }

    void FollowPlayer()
    {
        Vector3 moveVector = Vector3.Lerp(transform.position, target.transform.position, cameraSettings.moveSpeed * Time.deltaTime);
        transform.position = moveVector;
    }
    void RotateCamera()
    {
        cameraXrotation += Input.GetAxis(inputSettings.mouseYAxis) * cameraSettings.MouseY_Sensitivity;
        cameraYrotation += Input.GetAxis(inputSettings.mouseXAxis) * cameraSettings.MouseX_Sensitivity;

        cameraXrotation = Mathf.Clamp(cameraXrotation, cameraSettings.MinClampAngle, cameraSettings.MaxClampAngle);//arada bir de�er d�nd�r�yor.

        cameraYrotation = Mathf.Repeat(cameraYrotation, 360);//Hi�bir zaman uzunluktan b�y�k ve asla 0'dan k���k olmayacak �ekilde t de�erini d�ng�ler.
        //adam ekraf�nda 360 derece d�nebilecek.

        Vector3 rotationAngle = new Vector3(cameraXrotation, cameraYrotation, 0);//z s�f�r oldu=z ekseninde donme olmas�n.

        Quaternion rotation = Quaternion.Slerp(center.transform.localRotation, Quaternion.Euler(rotationAngle),cameraSettings.rotationSpeed*Time.deltaTime); //Quaternion, rotasyonlar� temsil etmek i�in kullan�l�r.//
        //Quaternion.Euler = z ekseni etraf�nda z derece, x ekseni etraf�nda x derece ve y ekseni etraf�nda y derece d�nd�ren bir d�n�� d�nd�r�r; bu s�rayla uygulan�r.

        center.transform.localRotation = rotation;
    }

    void ZoomCamera()
    {
        if (Input.GetButton(inputSettings.AimingInput))
        {
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, cameraSettings.zoomFieldofView, cameraSettings.zoomSpeed * Time.deltaTime);
            UICam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, cameraSettings.zoomFieldofView, cameraSettings.zoomSpeed * Time.deltaTime);
        }
        else
        {
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, cameraSettings.originalFieldofView, cameraSettings.zoomSpeed * Time.deltaTime);
            UICam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, cameraSettings.originalFieldofView, cameraSettings.zoomSpeed * Time.deltaTime);
        }
    }
    void HandleCamCollision()//kam �arp��mas�n� tut
    {
        if (!Application.isPlaying)
            return;
        if (Physics.Linecast(target.transform.position + target.transform.up, cameraSettings.camPosition.position, out hit, cameraSettings.camCollisionLayers)) //�izgiyle kesi�en herhangi bir �arp��t�r�c� varsa true.
        {
            Vector3 newCamPos = new Vector3(hit.point.x + hit.normal.x * .2f, hit.point.y + hit.normal.y * .8f, hit.point.z + hit.normal.z * .2f);//yans�ma vetoru ile normal vekt�r� topluyoruz.
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, newCamPos, cameraSettings.moveSpeed * Time.deltaTime);
        }
        else
        {
            mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition, initialCamPos, cameraSettings.moveSpeed * Time.deltaTime);
        }

        Debug.DrawLine(target.transform.position + target.transform.up,cameraSettings.camPosition.position,Color.blue);
    }
}
