using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GM : MonoBehaviour
{
    public GameObject mainCamera;
    public float speedHC;
    public float speedVC;
    public float speedLab;
    public enum Direccion { iz, der, ar, ab };
    public GameObject[] Flechas;
    private float xCamera;
    private float yCamera;
    private bool activeMove;
    private bool tapaPressed = false;
    public GameObject flechaTapa;
    public GameObject flechaLaberinto;
    public GameObject objLaberinto;
    public bool movLab=false;
    private Vector3 posNextLab;
    private GameObject detectHit;
    private GameObject saveLastHit;
    private GameObject pestanaObj;
    public PestanasManager controlPestana;
    [System.Serializable]
    public class PestanasManager
    {
        public bool correcto;
        public Pestanas[] arrPestanas;
        [System.Serializable]
        public class Pestanas
        {
            public GameObject Pestana;
            public int posCorrecta;
        }
    }
    public LabManager controlLab;
    [System.Serializable]
    public class LabManager
    {
        public bool correcto;
        public Pestanas[] arrCruz;
        [System.Serializable]
        public class Pestanas
        {
            public GameObject Cruz;
            public float posCorrecta;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        #region Guardado Flechas puzle
        for (int i = 0; i < Flechas.Length; i++)
        {
            FlechasMostrar(Flechas[i]);
            for (int e = 0; e < Flechas[i].transform.GetChild(0).childCount; e++)
            {
                GameObject obj = Flechas[i].transform.GetChild(0).GetChild(e).gameObject;
                if (Flechas[i].transform.GetChild(0).GetChild(e).name == "iz")
                {
                    Flechas[i].transform.GetChild(0).GetChild(e).GetComponent<Button>().onClick.AddListener(delegate { MovimientoCuadro(Direccion.iz, obj); });
                }
                else
                if (Flechas[i].transform.GetChild(0).GetChild(e).name == "der")
                {
                    Flechas[i].transform.GetChild(0).GetChild(e).GetComponent<Button>().onClick.AddListener(delegate { MovimientoCuadro(Direccion.der, obj); });
                }
                else
                if (Flechas[i].transform.GetChild(0).GetChild(e).name == "ar")
                {
                    Flechas[i].transform.GetChild(0).GetChild(e).GetComponent<Button>().onClick.AddListener(delegate { MovimientoCuadro(Direccion.ar, obj); });
                }
                else
                if (Flechas[i].transform.GetChild(0).GetChild(e).name == "ab")
                {
                    Flechas[i].transform.GetChild(0).GetChild(e).GetComponent<Button>().onClick.AddListener(delegate { MovimientoCuadro(Direccion.ab, obj); });
                }
            }
        }
        #endregion
    }
    private void FixedUpdate()
    {
        RaycastHit hit;
        Ray ray = mainCamera.transform.GetChild(0).GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            Debug.DrawRay(mainCamera.transform.GetChild(0).transform.position, mainCamera.transform.GetChild(0).transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            detectHit = hit.collider.gameObject;
        }
        else
        {
            detectHit = null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            activeMove = true;
        }
        if (Input.GetMouseButtonDown(0) && detectHit != null)
        {
            #region PestañaButtonDown
            if (detectHit.tag == "Pestana")
            {
                pestanaObj = detectHit.gameObject;
            }
            else
            {
                if (saveLastHit != null && detectHit.tag != "Flecha")
                {
                    saveLastHit.gameObject.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                }
            }
            #endregion
            #region TapaButtonDown
            if (detectHit.gameObject.tag == "Tapa")
            {
                tapaPressed = true;
            }
            #endregion
        }
        if (Input.GetMouseButtonUp(0) && detectHit != null)
        {
            #region PestañaButtonUp
            if (detectHit.gameObject == pestanaObj)
            {
                ShowArr();
            }
            else if (saveLastHit != null && detectHit.tag != "Flecha")
            {
                saveLastHit.gameObject.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
            }
            #endregion
            #region PestañaButtonUp
            if (detectHit.gameObject.tag == "Tapa" && tapaPressed == true)
            {
                if (controlPestana.correcto)
                {
                    flechaTapa.SetActive(true);
                }
                tapaPressed = false;
            }
            else { flechaTapa.SetActive(false); tapaPressed = false; }
            #endregion
        }
        if (Input.GetMouseButtonUp(0) && activeMove)
        {
            activeMove = false;
        }
        if (activeMove)
        {
            GiroCamera();
        }
        #region Laberinto
        if(movLab)
        {
            float step = speedLab * Time.deltaTime;
            objLaberinto.transform.position = Vector3.MoveTowards(objLaberinto.transform.position, new Vector3(posNextLab.x, objLaberinto.transform.position.y, posNextLab.z), step);
            if (Vector3.Distance(objLaberinto.transform.position,posNextLab) < 0.04f)
            {
                FlechasLab();
                movLab = false;
                if(!objLaberinto.GetComponent<BoxCollider>().enabled)
                {
                    objLaberinto.GetComponent<BoxCollider>().enabled = true;
                    Invoke("CompLab",0.5f);
                    }
            }
        }
        #endregion
    }
    #region Giro Camara
    public void GiroCamera()
    {
        xCamera += speedHC * Input.GetAxis("Mouse X");
        yCamera -= speedVC * Input.GetAxis("Mouse Y");
        mainCamera.transform.eulerAngles = new Vector3(yCamera, xCamera, 0.0f);
    }
    #endregion
    #region Puzle Flechas
    public void MovimientoCuadro(Direccion Dir, GameObject obj)
    {
        GameObject ParentObj = obj.transform.parent.parent.gameObject;
        RaycastHit hit;
        if (Dir == Direccion.ab)
        {
            if (Physics.Raycast(ParentObj.transform.position, ParentObj.transform.TransformDirection(Vector3.left), out hit, Mathf.Infinity))
            {
                Debug.Log(hit.collider.name);
                Vector3 posInv = hit.transform.position;
                hit.transform.position = ParentObj.transform.position;
                ParentObj.transform.position = posInv;
            }
        }
        if (Dir == Direccion.ar)
        {
            if (Physics.Raycast(ParentObj.transform.position, ParentObj.transform.TransformDirection(Vector3.right), out hit, Mathf.Infinity))
            {
                Debug.Log(hit.collider.name);
                Vector3 posInv = hit.transform.position;
                hit.transform.position = ParentObj.transform.position;
                ParentObj.transform.position = posInv;
            }
        }
        if (Dir == Direccion.der)
        {
            if (Physics.Raycast(ParentObj.transform.position, ParentObj.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
            {
                Vector3 posInv = hit.transform.position;
                hit.transform.position = ParentObj.transform.position;
                ParentObj.transform.position = posInv;
            }
        }
        if (Dir == Direccion.iz)
        {
            if (Physics.Raycast(ParentObj.transform.position, ParentObj.transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity))
            {
                Vector3 posInv = hit.transform.position;
                hit.transform.position = ParentObj.transform.position;
                ParentObj.transform.position = posInv;
            }
        }
        StartCoroutine(ContadorFlechasMost());
    }
    IEnumerator ContadorFlechasMost()
    {
        yield return new WaitForSeconds(0.05f);
        for (int i = 0; i < Flechas.Length; i++)
        {
            FlechasMostrar(Flechas[i]);
        }
    }
    public void FlechasMostrar(GameObject obj)
    {
        for (int i = 0; i < obj.transform.GetChild(0).childCount; i++)
        {
            obj.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
        }
        RaycastHit hit;
        if (Physics.Raycast(obj.transform.position, obj.transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity) && hit.transform.tag == "CuadroInv")
        {
            obj.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            obj.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            obj.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
            obj.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);

        }
        else if (Physics.Raycast(obj.transform.position, obj.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity) && hit.transform.tag == "CuadroInv")
        {
            obj.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            obj.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            obj.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
            obj.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
        }
        else if (Physics.Raycast(obj.transform.position, obj.transform.TransformDirection(Vector3.right), out hit, Mathf.Infinity) && hit.transform.tag == "CuadroInv")
        {
            obj.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            obj.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            obj.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
            obj.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
        }
        else if (Physics.Raycast(obj.transform.position, obj.transform.TransformDirection(Vector3.left), out hit, Mathf.Infinity) && hit.transform.tag == "CuadroInv")
        {
            obj.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            obj.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            obj.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
            obj.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
        }
    }
    #endregion
    #region PuzleBarras
    public void ShowArr()
    {
        if (saveLastHit != null && saveLastHit != detectHit)
        {
            saveLastHit.gameObject.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
            detectHit.gameObject.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
            saveLastHit = detectHit;
        }
        else
        {
            saveLastHit = detectHit;
            saveLastHit.gameObject.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        }
    }
    public void MovAnim(int num)
    {
        saveLastHit.transform.GetChild(0).GetComponent<Animator>().SetInteger("Mov", num);
        CompArr();
    }
    public void CompArr()
    {
        for (int i = 0; i < controlPestana.arrPestanas.Length; i++)
        {
            Animator pestAnim = controlPestana.arrPestanas[i].Pestana.GetComponent<Animator>();
            int goodPos = controlPestana.arrPestanas[i].posCorrecta;
            if (pestAnim.GetInteger("Mov") != goodPos)
            {
                controlPestana.correcto = false;
                return;
            }
        }
        controlPestana.correcto = true;
    }
    #endregion
    #region Tapa
    public void TapaAnim(GameObject tapa)
    {
        flechaTapa.SetActive(false);
        tapa.GetComponent<Animator>().SetBool("Abrir", true);
    }
    #endregion
    #region Laberinto
    public void MovLab(int Value)
    {
        Direccion Dir = (Direccion)Value;

        RaycastHit hit;
        if (Dir == Direccion.ab)
        {
            if (Physics.Raycast(objLaberinto.transform.position, objLaberinto.transform.TransformDirection(Vector3.left), out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == "PuntoMov")
                {
                     posNextLab = hit.transform.position;
                    for (int i = 0; i < objLaberinto.transform.GetChild(0).childCount; i++)
                    {
                        objLaberinto.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
                    }
                    movLab = true;
                }
            }
        }
        if (Dir == Direccion.ar)
        {
            if (Physics.Raycast(objLaberinto.transform.position, objLaberinto.transform.TransformDirection(Vector3.right), out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == "PuntoMov")
                {
                     posNextLab = hit.transform.position;
                    for (int i = 0; i < objLaberinto.transform.GetChild(0).childCount; i++)
                    {
                        objLaberinto.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
                    }
                    movLab = true;
                }
            }
        }
        if (Dir == Direccion.der)
        {
            if (Physics.Raycast(objLaberinto.transform.position, objLaberinto.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == "PuntoMov")
                {
                     posNextLab = hit.transform.position;
                    for (int i = 0; i < objLaberinto.transform.GetChild(0).childCount; i++)
                    {
                        objLaberinto.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
                    }
                    movLab = true;
                }
            }
        }
        if (Dir == Direccion.iz)
        {
            if (Physics.Raycast(objLaberinto.transform.position, objLaberinto.transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == "PuntoMov")
                {
                     posNextLab = hit.transform.position;
                    for (int i = 0; i < objLaberinto.transform.GetChild(0).childCount; i++)
                    {
                        objLaberinto.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
                    }
                    movLab = true;
                }
            }
        }
    }
    public void FlechasLab()
    {
        RaycastHit hit;
        if (Physics.Raycast(objLaberinto.transform.position, objLaberinto.transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity) && hit.transform.tag == "PuntoMov")
        {
            objLaberinto.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        }
         if (Physics.Raycast(objLaberinto.transform.position, objLaberinto.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity) && hit.transform.tag == "PuntoMov")
        {
            objLaberinto.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        }
         if (Physics.Raycast(objLaberinto.transform.position, objLaberinto.transform.TransformDirection(Vector3.right), out hit, Mathf.Infinity) && hit.transform.tag == "PuntoMov")
        {
            objLaberinto.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        }
         if (Physics.Raycast(objLaberinto.transform.position, objLaberinto.transform.TransformDirection(Vector3.left), out hit, Mathf.Infinity) && hit.transform.tag == "PuntoMov")
        {
            objLaberinto.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
        }
    }
    public void CompLab()
    {
        for (int i = 0; i < controlLab.arrCruz.Length; i++)
        {
           Debug.Log(Mathf.Round(controlLab.arrCruz[i].Cruz.transform.localRotation.eulerAngles.y) + "   " + controlLab.arrCruz[i].posCorrecta + "  "+controlLab.arrCruz[i].Cruz.name);
            if (Mathf.Round(controlLab.arrCruz[i].Cruz.transform.localRotation.eulerAngles.y) != controlLab.arrCruz[i].posCorrecta)
            {
                return;
            }
        }
        controlLab.correcto = true;
        flechaLaberinto.SetActive(true);
    }
    public void TapaLabAnim(GameObject tapa)
    {
        flechaLaberinto.SetActive(false);
        tapa.GetComponent<Animator>().SetBool("Abrir", true);
    }
    #endregion
}
