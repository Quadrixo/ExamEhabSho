using UnityEngine;
using System.Collections;

public class DeathTimer : MonoBehaviour {

    public float timeToDie = 1f;

	// Update is called once per frame
	void Update () {
        timeToDie -= Time.deltaTime;
        if (timeToDie < 0)
            Destroy(this.gameObject);
	}
}
