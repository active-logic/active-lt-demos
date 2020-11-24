using UnityEngine;
using Active.Core;
using static Active.Raw;

public class Frogger : UGig{

    Rigidbody body;
    public float traction = 10;

    override public status Step(){
        body.AddForce(Vector3.right * traction);
        return cont;
    }

    void Start(){
        body = GetComponent<Rigidbody>();
    }

}
