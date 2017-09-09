using UnityEngine;
using System.Collections;

public class AutoDelete : MonoBehaviour {

    //This is how long until the object is deleted.
    public float duration = 1.0f;

	// Use this for initialization
	void Start ()
    {
        Invoke("Die", duration);
	}

    //Delete.
    public void Die()
    {
        Destroy(this.gameObject);
    }
}
