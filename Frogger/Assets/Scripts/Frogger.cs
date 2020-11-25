using UnityEngine;
using Active.Core;
using static Active.Raw;

public class Frogger : UGig{

    static int id = 0;
    //
    public GameObject clone;
    public int eggs = 1;
    public float traction = 10;
    public int hunger = 100;
    //
    Rigidbody body;

    override public status Step()
    => Dodge() && Feed() && Spawn();

    status Dodge(){
        var foe = GameObject.Find("NastyBall").transform;
        var u = transform.position - foe.position;
        var dist = u.magnitude;
        if(dist > 3f) return done;
        u.y = 0f;
        u.Normalize();
        body.AddForce(u * traction * 3f);
        return cont;
    }

    status Feed (){
        if(hunger == 0) return done;
        var food = GameObject.FindWithTag("Food").transform;
        if(food){
            return Reach(food) && Consume(food);
        }else{
            return fail;
        }
    }

    status Spawn(){
        if(eggs == 0) return fail;
        clone.transform.position =
            transform.position + Vector3.right * 0.1f;
        clone.SetActive(true);
        if(--eggs > 1) clone = Clone(clone);
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
        if(hunger <= 0 ) return done;
        hunger--;
        return cont;
    }

    void Start(){
        clone = Clone(gameObject);
        body = GetComponent<Rigidbody>();
    }

    GameObject Clone(GameObject original){
        clone = Instantiate(original);
        clone.name = $"Frogger #{++id}";
        clone.SetActive(false);
        return clone;
    }

}
