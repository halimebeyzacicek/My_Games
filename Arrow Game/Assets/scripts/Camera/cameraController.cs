using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]//Bir komut dosyasýnýn tüm örneklerinin Düzenleme Modunda yürütülmesini saðlar.Varsayýlan olarak, MonoBehaviours yalnýzca Oynatma Modunda yürütülür. Bu özniteliði ekleyerek, MonoBehaviour'un herhangi bir örneði, Editör Düzenleme Modundayken de geri çaðýrma iþlevlerini yürütecektir.


public class cameraController : MonoBehaviour
{
    [System.Serializable]
    public class CameraSettings
    {
        [Header("Camera Move Settings")]
        public float zoomSpeed = 5;
        public float moveSpeed = 5;
        public float rotationSpeed = 5;
        public float originalFieldofView = 70;//orijinal görüþ alaný 
        public float zoomFieldofView = 20;//yakýnlaþtýrma Görüþ Alaný
        public float MouseX_Sensitivity = 5;//MouseX_Hassasiyeti
        public float MouseY_Sensitivity = 5;
        public float MaxClampAngle = 90;//Maksimum Kelepçe Açýsý
        public float MinClampAngle = -30;//Minimum Kelepçe Açýsý

        [Header("Camera Collision")]
        public Transform camPosition;
        public LayerMask camCollisionLayers;//çarpoþma=collision
       
    }
    [SerializeField]
    public CameraSettings cameraSettings;

    [System.Serializable]
    public class CameraInputSettings
    {
        public string mouseXAxis = "Mouse X";
        public string mouseYAxis = "Mouse Y";
        public string AimingInput = "Fire2";//Hedefleme Giriþi
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
    RaycastHit hit;//ýþýn yayýn vuruþu

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        UICam = mainCam.GetComponentInChildren<Camera>();
        center = transform.GetChild(0);//camera holder'in ilk çocugu center. onu center a atýyoruz.
        FindPlayer();
        initialCamPos = mainCam.transform.localPosition;//initial=ilk
    }

    // Update is called once per frame
    void Update()
    {
        if (!target)//bir targete sahip deðilsek.
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

        cameraXrotation = Mathf.Clamp(cameraXrotation, cameraSettings.MinClampAngle, cameraSettings.MaxClampAngle);//arada bir deðer döndürüyor.

        cameraYrotation = Mathf.Repeat(cameraYrotation, 360);//Hiçbir zaman uzunluktan büyük ve asla 0'dan küçük olmayacak þekilde t deðerini döngüler.
        //adam ekrafýnda 360 derece dönebilecek.

        Vector3 rotationAngle = new Vector3(cameraXrotation, cameraYrotation, 0);//z sýfýr oldu=z ekseninde donme olmasýn.

        Quaternion rotation = Quaternion.Slerp(center.transform.localRotation, Quaternion.Euler(rotationAngle),cameraSettings.rotationSpeed*Time.deltaTime); //Quaternion, rotasyonlarý temsil etmek için kullanýlýr.//
        //Quaternion.Euler = z ekseni etrafýnda z derece, x ekseni etrafýnda x derece ve y ekseni etrafýnda y derece döndüren bir dönüþ döndürür; bu sýrayla uygulanýr.

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
    void HandleCamCollision()//kam çarpýþmasýný tut
    {
        if (!Application.isPlaying)
            return;
        if (Physics.Linecast(target.transform.position + target.transform.up, cameraSettings.camPosition.position, out hit, cameraSettings.camCollisionLayers)) //çizgiyle kesiþen herhangi bir çarpýþtýrýcý varsa true.
        {
            Vector3 newCamPos = new Vector3(hit.point.x + hit.normal.x * .2f, hit.point.y + hit.normal.y * .8f, hit.point.z + hit.normal.z * .2f);//yansýma vetoru ile normal vektörü topluyoruz.
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, newCamPos, cameraSettings.moveSpeed * Time.deltaTime);
        }
        else
        {
            mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition, initialCamPos, cameraSettings.moveSpeed * Time.deltaTime);
        }

        Debug.DrawLine(target.transform.position + target.transform.up,cameraSettings.camPosition.position,Color.blue);
    }
}
