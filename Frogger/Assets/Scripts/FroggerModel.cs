using UnityEngine;

public class FroggerModel : MonoBehaviour{

    static int id = 0;
    //
    GameObject clone;
    Rigidbody  body;
    int        eggs = 1;
    float      traction = 10;
    int        hunger = 100;

    public float speed => body.velocity.magnitude;
    public bool hungry => hunger > 0;

    public void Feed(){
        hunger--;
    }

    public Transform Clone(){
        if(eggs == 0) return null;
        var c = clone;
        if(--eggs > 1) clone = DoClone(clone);
        c.SetActive(true);
        return c.transform;
    }

    public void Propel(Vector3 u)
    => body.AddForce(u * traction);

    public void Impel(Vector3 u)
    => body.AddForce(u * traction, ForceMode.Impulse);

    // =============================================================

    GameObject DoClone(GameObject original){
        var clone = Instantiate(original);
        clone.name = $"Frogger #{++id}";
        clone.SetActive(false);
        return clone;
    }

    void Start(){
        clone = DoClone(gameObject);
        body  = GetComponent<Rigidbody>();
    }

}
