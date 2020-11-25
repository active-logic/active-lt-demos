using UnityEngine;

public class FroggerModel : MonoBehaviour{

    public static int id = 0;
    //
    public GameObject clone;
    public int eggs = 1;
    public float traction = 10;
    public int hunger = 100;

    void Start() => clone = DoClone(gameObject);

    public GameObject Clone(){
        if(eggs == 0) return null;
        var c = clone;
        if(--eggs > 1) clone = DoClone(clone);
        return c;
    }

    GameObject DoClone(GameObject original){
        var clone = Instantiate(original);
        clone.name = $"Frogger #{++id}";
        clone.SetActive(false);
        return clone;
    }

}
