using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallSaveLoad : MonoBehaviour {

    public FancyInventory Inv;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveLoadScript.Save();
            Debug.Log("Save");
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            SaveLoadScript.Load();
            if (Inv != null)
            {
                Inv.LoadFromStaticInv();
            }
            Debug.Log("Load");
        }
	}
}
