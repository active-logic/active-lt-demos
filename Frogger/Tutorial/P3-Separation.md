# Frogger tutorial P3 - Design for performance and modularity

In the second part of this tutorial we have implemented a simple, yet life-like agent. The "one big class" approach is sometimes handy but does not really scale beyond very simple cases.

The good news here is that we can leverage an approach which only helps scaling up, but also leans towards better performance.

## Moving state to a model object

In a first step we'll just grab all variables defined in `Frogger` and migrate them to a separate component.

```cs
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
```

Of course we need to adjust `Frogger` accordingly. You can see how this was done [in this diff](PENDING).

We also migrated the details of cloning Frogger because this doesn't partake the BT.
