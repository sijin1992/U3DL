using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirEnemy : Enemy {

	void Update () {
        RotateTo();
        MoveTo();
        Fly();
	}
    public void Fly() {
        float flyspeed = 0;
        if (this.transform.position.y<1.2f)
        {
            flyspeed = 1.0f;
        }
        this.transform.Translate(0, flyspeed * Time.deltaTime, 0);
    }
}
