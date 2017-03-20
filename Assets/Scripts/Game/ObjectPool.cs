using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

    public static ObjectPool pool;
    public int pooledAmount = 100;
    public bool willGrow = false;

    public GameObject pooledObjectExplosion;
    public GameObject pooledObjectText;
    public GameObject pooledObjectScrap;
    public GameObject pooledObjectMedMeteor;
    //public GameObject pooledObjectBigMeteor;

    public List<GameObject> pooledObjects1;
    public List<GameObject> pooledObjects2;

    void Awake()
    {
        pool = this;
    }

	void Start ()
    {
        pooledObjects1 = new List<GameObject>();

        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj;

            obj = Instantiate(pooledObjectExplosion);
            obj.SetActive(false);
            pooledObjects1.Add(obj);

            obj = Instantiate(pooledObjectText);
            obj.SetActive(false);
            pooledObjects1.Add(obj);

            /*obj = Instantiate(pooledObjectScrap);
            obj.SetActive(false);
            pooledObjects2.Add(obj);

            obj = Instantiate(pooledObjectMedMeteor);
            obj.SetActive(false);
            pooledObjects2.Add(obj);*/
            /*
            obj = Instantiate(pooledObjectBigMeteor);
            obj.SetActive(false);
            pooledObjects.Add(obj);
            */
        }
    }

    public GameObject GetPooledObject(GameObject gObject, int listNumber)
    {
        List<GameObject> list;

        if (listNumber == 1)
            list = pooledObjects1;

        else if (listNumber == 2)
            list = pooledObjects2;

        else return null;

        for (int i = 0; i < pooledObjects1.Count; i++)
        {
            if (!list[i].activeInHierarchy && gObject.CompareTag(list[i].tag))
            {
                return list[i];
            }
        }

        if (willGrow)
        {
            GameObject obj = Instantiate(gObject) as GameObject;
            obj.SetActive(false);
            list.Add(obj);
            return obj;
        }

        return null;
    }
	
}
