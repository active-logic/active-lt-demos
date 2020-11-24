using UnityEngine;
using Active.Core;
using static Active.Raw;

public class Frogger : UGig{

    Rigidbody body;
    public float traction = 10;
    public int hunger = 100;
    public float toFoe;

    override public status Step(){
        return -Dodge() || Feed() || Spawn();
    }

    status Dodge(){
        var foe = GameObject.Find("NastyBall").transform;
        var u = transform.position - foe.position;
        var dist = u.magnitude;
        toFoe = dist;
        if(dist > 3f) return fail;
        u.y = 0f;
        u.Normalize();
        body.AddForce(u * traction * 3f);
        return cont;
    }

    status Feed (){
        var food = GameObject.FindWithTag("Food").transform;
        if(food){
            return Reach(food) && Consume(food);
        }else{
            return fail;
        }
    }

    status Spawn() => fail;

    status Reach(Transform target){
        var u = target.position - transform.position;
        var dist = u.magnitude;
        if(dist < 1f) return done;
        if(body.velocity.magnitude <= 1e-6f){
            body.AddForce(u * 0.2f + Vector3.up * 2,
                          ForceMode.Impulse);
        }
        return cont;
    }

    status Consume(Transform obj){
        if(hunger <= 0 ) return done;
        hunger--;
        return cont;
    }

    void Start(){
        body = GetComponent<Rigidbody>();
    }

}
