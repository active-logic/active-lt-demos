using UnityEngine;
using Active.Core;
using static Active.Raw;

public class Frogger : UGig{

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
        model.Propel(u * 3f);
        return cont;
    }

    status Feed (){
        if(!model.hungry) return done;
        var food = GameObject.FindWithTag("Food").transform;
        if(food){
            return Reach(food) && Consume(food);
        }else{
            return fail;
        }
    }

    status Spawn(){
        var clone = model.Clone();
        if(!clone) return fail;
        clone.position = transform.position + Vector3.right * 0.1f;
        return done;
    }

    status Reach(Transform target){
        var u = target.position - transform.position;
        var dist = u.magnitude;
        if(dist < 1f) return done;
        if(model.speed <= 1e-6f){
            model.Impel(u * 0.02f + Vector3.up * 0.2f);
        }
        return cont;
    }

    status Consume(Transform obj){
        model.Feed();
        return cont;
    }

    void Start(){
        model = GetComponent<FroggerModel>();
    }

}
