using UnityEngine;

public class FroggerAp : MonoBehaviour{

    public Vector3   dodgeVector;
    public Transform food;

    public float Distance(Transform target)
    => (target.position - transform.position).magnitude;

    public Vector3 Direction(Transform target)
    => (target.position - transform.position).normalized;

    public Vector3 JumpVector(Transform target)
    => Direction(target) * 0.07f + Vector3.up * 0.35f;

    void Start() => InvokeRepeating("DoUpdate", 0.1f, 0.1f);

    void DoUpdate(){
        dodgeVector = DodgeVector();
        food        = Food();
    }

    Vector3 DodgeVector(){
        var foe = GameObject.Find("NastyBall").transform;
        var u = transform.position - foe.position;
        var dist = u.magnitude;
        if(dist > 3f) return Vector3.zero;
        return u.normalized;
    }

    Transform Food() => GameObject.FindWithTag("Food").transform;

}
