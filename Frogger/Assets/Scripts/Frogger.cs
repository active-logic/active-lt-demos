using UnityEngine;
using Active.Core;
using static Active.Raw;

public class Frogger : UGig{

    FroggerModel model;
    FroggerAp    ap;

    override public status Step()
    => Dodge() && Feed() && Spawn();

    status Dodge(){
        var u = ap.DodgeVector();
        return u == Vector3.zero ? done : -model.Propel(u * 3f);
    }

    status Feed(){
        if(!model.hungry) return done;
        var food = ap.food;
        if(food){
            return Reach(food) && -model.Feed();
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
            return -model.Impel(u * 0.02f + Vector3.up * 0.2f);
        }
        return cont;
    }

    void Start(){
        model = GetComponent<FroggerModel>();
        ap = gameObject.AddComponent<FroggerAp>();
    }

}
