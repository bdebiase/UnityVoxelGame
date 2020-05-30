using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour {
    //INSTANCE VARIABLES
    public GameObject item;

    //run at start of program
    private void Start() {
        if (item != null)
            GetComponent<MenuButton>().tooltip = item.name;
    }
}
