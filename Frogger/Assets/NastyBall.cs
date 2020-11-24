using UnityEngine;
using Active.Core;
using static Active.Raw;

public class NastyBall: UGig{

    public float traction = 5;

    override public status Step(){
        GetComponent<Rigidbody>().AddForce(
            Random.onUnitSphere * traction,
            ForceMode.Impulse
        );
        return cont;
    }

}
