using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class bow : MonoBehaviour
{
    public TextMeshProUGUI text;

    [System.Serializable]
    public class BowSettings
    {
        [Header("Arrow Settings")]
        public float arrowCount;
        public Rigidbody arrowPrefab;
        public Transform arrowPos;
        public Transform arrowEquipParent;
        public float arrowForce = 3;//ok gücü.atýlýrken.

        [Header("bow Equip & Unequip Settings")]
        public Transform EquipPos;
        public Transform UnEquipPos;//donatýlmamýþ kullanýcý ok atmayý hedeflemiyor.

        public Transform UnEquipParent;
        public Transform EquipParent;

        [Header("Bow String Settings")]
        public Transform bowString;
        public Transform stringInitialPos;//ipin ilk pozisyonu
        public Transform stringHandPullPos;
        public Transform stringInitialParent;

    }
    [SerializeField]
    public BowSettings bowSettings;

    [Header("Crosshair Settings")]//artý iþareti
    public GameObject crossHairPrefab;
    GameObject currentCrossHair;

    Rigidbody currentArrow;

    bool canPullString = false;//çekebilir
    bool canFireArrow = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PickArrow()
    {
        bowSettings.arrowPos.gameObject.SetActive(true);
        
    }

    public void DisableArrow()
    {
        bowSettings.arrowPos.gameObject.SetActive(false);
    }

    public void PullString()//ipi çekme 
    {
        bowSettings.bowString.transform.position = bowSettings.stringHandPullPos.position;
        bowSettings.bowString.transform.parent = bowSettings.stringHandPullPos;
    }
    public void ReleaseString()//ipi serbest býrakmak
    {
        bowSettings.bowString.transform.position = bowSettings.stringInitialPos.position;
        bowSettings.bowString.transform.parent = bowSettings.stringInitialParent;
    }

    public void EquipBow()//yay donatmak
    {
        this.transform.position = bowSettings.EquipPos.position;
        this.transform.rotation = bowSettings.EquipPos.rotation;
        this.transform.parent = bowSettings.EquipParent;

    }
    public void UnEquipBow()//yay donatmak
    {
        this.transform.position = bowSettings.UnEquipPos.position;
        this.transform.rotation = bowSettings.UnEquipPos.rotation;
        this.transform.parent = bowSettings.UnEquipParent;

    }

    public void ShowCrosshair(Vector3 crosshairPos)//artý iþareti
    {
        if (!currentCrossHair)//eðer sahnede artý iþareti yoksa
            currentCrossHair = Instantiate(crossHairPrefab) as GameObject;

        currentCrossHair.transform.position = crosshairPos;
        currentCrossHair.transform.LookAt(Camera.main.transform);//artý iþareti her zaman kamera yönüne bakacak.
    }

    public void RemoveCrosshair()
    {
        if (currentCrossHair)//eðer artý iþaretimiz varsa
            Destroy(currentCrossHair);
    }

    IEnumerator finish_bekle_bow()
    {
        yield return new WaitForSeconds(1f);
        dusman.finish.SetActive(true);
    }

    public void Fire(Vector3 hitPoint)
    {
        if (bowSettings.arrowCount < 1)//ok yok.
            return;
        Vector3 dir = hitPoint - bowSettings.arrowPos.position;//ok için yön olmuþ olacak bu parametre.
        currentArrow = Instantiate(bowSettings.arrowPrefab, bowSettings.arrowPos.position, bowSettings.arrowPos.rotation) as Rigidbody;//somutlaþtýrdýk

        currentArrow.AddForce(dir * bowSettings.arrowForce, ForceMode.Force);

        bowSettings.arrowCount -= 1;

        if (bowSettings.arrowCount < 1)
        {
            text.text = "No Arrow!";
            StartCoroutine(finish_bekle_bow());
        }
        else
            text.text = "Arrow Count: " + bowSettings.arrowCount.ToString();
    }
}
