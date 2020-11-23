using UnityEngine;
using Active.Core;
using static Active.Raw;

public class Frogger : MonoBehaviour{

    public Vector3 impulse = new Vector3(1f, 3f, 0f);
    status state;
    bool didCollide = false;

    void Update(){
        state = Jump();
        if(state.complete) enabled = false;
    }

    status Jump(){
        var body  = GetComponent<Rigidbody>();
        var speed = body.velocity.magnitude;
        if(speed <= 1e-6f){
            body.AddForce(impulse, ForceMode.Impulse);
        }
        return didCollide ? done : cont;
    }

    void OnCollisionEnter(Collision x){
        if(x.collider.gameObject.name == "Wall"){
            didCollide = true;
        }
    }

}
