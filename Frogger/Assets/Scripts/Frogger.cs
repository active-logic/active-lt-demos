using UnityEngine;
using Active.Core;
using static Active.Raw;

public class Frogger : UGig{

    FroggerModel model;
    FroggerAp    ap;

    override public status Step()
    => Dodge() && Feed() && Spawn();

    status Dodge()
    => (ap.dodgeVector == Vector3.zero) ? done :
       -model.Propel(ap.dodgeVector * 3f);

    status Feed()
    => !model.hungry
    || ((ap.food!=null) && Reach(ap.food) && -model.Feed());

    status Spawn(){
        var clone = model.Clone();
        if(!clone) return fail;
        clone.position = transform.position + Vector3.right * 0.1f;
        return done;
    }

    status Reach(Transform target)
    => (ap.Distance(target) < 1f) ? done :
       (model.speed > 1e-6f)      ? cont :
       -model.Impel(ap.JumpVector(target));

    void Start(){
        model = GetComponent<FroggerModel>();
        ap = gameObject.AddComponent<FroggerAp>();
    }

}
