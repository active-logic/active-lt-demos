using UnityEngine;
using Active.Core;
using static Active.Raw;

public class Frogger : UGig{

    Rigidbody body;
    FroggerModel model;

    override public status Step()
    => Dodge() && Feed() && Spawn();

    status Dodge(){
        var foe = GameObject.Find("NastyBall").transform;
        var u = transform.position - foe.position;
        var dist = u.magnitude;
        if(dist > 3f) return done;
        u.y = 0f;
        u.Normalize();
        body.AddForce(u * model.traction * 3f);
        return cont;
    }

    status Feed (){
        if(model.hunger == 0) return done;
        var food = GameObject.FindWithTag("Food").transform;
        if(food){
            return Reach(food) && Consume(food);
        }else{
            return fail;
        }
    }

    status Spawn(){
        if(model.eggs == 0) return fail;
        var clone = model.Clone();
        clone.transform.position =
            transform.position + Vector3.right * 0.1f;
        clone.SetActive(true);
        return done;
    }

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
        if(model.hunger <= 0 ) return done;
        model.hunger--;
        return cont;
    }

    void Start(){
        model = GetComponent<FroggerModel>();
        body = GetComponent<Rigidbody>();
    }

}
