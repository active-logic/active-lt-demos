using UnityEngine;

public class FroggerAp : MonoBehaviour{

    public Vector3 DodgeVector(){
        var foe = GameObject.Find("NastyBall").transform;
        var u = transform.position - foe.position;
        var dist = u.magnitude;
        if(dist > 3f) return Vector3.zero;
        return u.normalized;
    }

    public Transform food
    => GameObject.FindWithTag("Food").transform;

}
