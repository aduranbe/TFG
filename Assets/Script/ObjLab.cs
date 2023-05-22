using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjLab : MonoBehaviour
{
    private bool left, rigth;
    private GameObject objDetect;
    Vector3 rot;
    public float speed;
    // Update is called once per frame
    void Update()
    {
        if (rigth)
        {
            objDetect.transform.parent.parent.localRotation = Quaternion.RotateTowards(objDetect.transform.parent.parent.localRotation, Quaternion.Euler(rot), Time.deltaTime * speed);
            if (objDetect.transform.parent.parent.localRotation == Quaternion.Euler(rot))
            {
                rot = new Vector3((int)objDetect.transform.parent.parent.localRotation.eulerAngles.x, Mathf.Round(objDetect.transform.parent.parent.localRotation.eulerAngles.y), Mathf.Round(objDetect.transform.parent.parent.localRotation.eulerAngles.z));
                objDetect.transform.parent.parent.localRotation = Quaternion.Euler(rot);
                rigth = false;
            }
        }
        else if (left)
        {
            objDetect.transform.parent.parent.localRotation = Quaternion.RotateTowards(objDetect.transform.parent.parent.localRotation, Quaternion.Euler(rot), Time.deltaTime * speed);
            if (objDetect.transform.parent.parent.localRotation == Quaternion.Euler(rot))
            {
                rot = new Vector3((int)objDetect.transform.parent.parent.localRotation.eulerAngles.x, Mathf.Round(objDetect.transform.parent.parent.localRotation.eulerAngles.y), Mathf.Round(objDetect.transform.parent.parent.localRotation.eulerAngles.z));
                left = false;
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "RotDer")
        {
            objDetect = other.gameObject;
            rot = new Vector3((int)objDetect.transform.parent.parent.localRotation.eulerAngles.x, (int)objDetect.transform.parent.parent.localRotation.eulerAngles.y, Mathf.Round(objDetect.transform.parent.parent.localRotation.eulerAngles.z - 90));
            rigth = true;
            GetComponent<BoxCollider>().enabled = false;
        }
        if (other.tag == "RotIz")
        {
            objDetect = other.gameObject;
            rot = new Vector3((int)objDetect.transform.parent.parent.localRotation.eulerAngles.x, (int)objDetect.transform.parent.parent.localRotation.eulerAngles.y, Mathf.Round(objDetect.transform.parent.parent.localRotation.eulerAngles.z + 90));
            left = true;
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
