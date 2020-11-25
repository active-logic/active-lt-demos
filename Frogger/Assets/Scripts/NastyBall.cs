using UnityEngine;
using Active.Core;
using static Active.Raw;

public class NastyBall: UGig{

    public float traction = 20;

    override public status Step(){
        var u = Random.onUnitSphere; u.y *= 0.1f;
        GetComponent<Rigidbody>().AddForce(
            u * traction,
            ForceMode.Impulse
        );
        return cont;
    }

}
