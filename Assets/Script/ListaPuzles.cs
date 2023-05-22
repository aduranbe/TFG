using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Nuevo Puzle", menuName = "ListaPuzles")]
public class ListaPuzles : ScriptableObject {

    public List<Puzle> puzles;
    [System.Serializable]
    public class Puzle
    {
        public string nombre;
        public int id;
        public GameObject puzle;
    }
}
