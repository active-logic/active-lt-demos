using UnityEngine;
using Active.Core;
using static Active.Raw;

public class FroggerZero : MonoBehaviour{

    public Vector3 impulse = new Vector3(1f, 3f, 0f);
    status state;
    bool didCollide = false;

    void Update(){
        state = Jump();
        if(!state.running) enabled = false;
    }

    status Jump(){
        var body     = GetComponent<Rigidbody>();
        var speed    = body.velocity.magnitude;
        if(transform.position.y < -1f){
            body.isKinematic = true;
            return fail;
        }
        if(speed <= 1e-6f){
            body.AddForce(impulse, ForceMode.Impulse);
        }
        // Return the `done` state on collide (3)
        return didCollide ? done : cont;
    }

    void OnCollisionEnter(Collision x){
        if(x.collider.gameObject.name == "Wall"){
            didCollide = true;
        }
    }

}
