# Frogger tutorial P3 - Design for performance and modularity

In the second part of this tutorial we have implemented a simple, yet life-like agent. The "one big class" approach is sometimes handy but does not really scale beyond very simple cases.

The good news here is that we can leverage an approach which only helps scaling up, but also leans towards better performance.

## Separating control from state

In a first step we'll just grab all variables defined in `Frogger` and migrate them to a separate component.

```cs
using UnityEngine;

public class FroggerModel : MonoBehaviour{

    static int id = 0;
    //
    GameObject clone;
    public int eggs = 1;
    public float traction = 10;
    public int hunger = 100;

    void Start() => clone = DoClone(gameObject);

    public Transform Clone(){
        if(eggs == 0) return null;
        var c = clone;
        if(--eggs > 1) clone = DoClone(clone);
        c.SetActive(true);
        return c.transform;
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

We also migrated the details of cloning Frogger because this does not partake the BT.

Next we hide model variables for better encapsulation (see (diff)[PENDING]).

## Cleaner BT using actions.

In AL, an `action` is equivalent to the `done` status. We can use this to clean the BT and simplify our logic.

Let's take an example to illustrate. `FroggerModel.Propel` returns `void`. We'll replace `void` with `action`:

```cs
public action Propel(Vector3 u){
    body.AddForce(u * traction);
    return @void;
}
```

We now simplify the `Dodge` task - before:

```cs
status Dodge(){
    // ...
    model.Propel(u * 3f);
    return cont;
}
```

After:

```cs
status Dodge(){
    // ...
    return -model.Propel(u * 3f);
}
```

The `-` operator demotes a status value so that `done` becomes `cont` and `cont` becomes `fail`.

## Separating apperception from control

From a performance perspective, one of costlier aspects in game AI is apperception - the process of converting raw sensor sensor input to a simple representation that control can handle.

In the Frogger demo apperception is much simplified. For example we just assume that there is only *one* obstacle to avoid (the nasty ball). Still, code related to detecting objects of interest and measuring distances is not directly relevant to control. As such it only adds clutter to the BT. Another issue is, if we want to improve performance, running apperception at a slower frame rate is a good choice. But we still want to run the control layer at the normal frame rate.

Let's factor perception/apperception related code into a separate class:

```cs
// FroggerAp.cs
using UnityEngine;

public class FroggerAp : MonoBehaviour{

    public Vector3 DodgeVector(){
        var foe = GameObject.Find("NastyBall").transform;
        var u = transform.position - foe.position;
        var dist = u.magnitude;
        if(dist > 3f) return Vector3.zero;
        return u.normalized;
    }

    public Transform food
    => GameObject.FindWithTag("Food").transform;

}
```

Then, the refactor of matching tasks:

```cs
public class Frogger : UGig{

    // ...

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

    // ...

}
```

For good measure we'll now illustrate how apperception can run at a lower frame rate.
