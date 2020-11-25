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

Of course we need to adjust `Frogger` accordingly. You can see how this was done [in this diff](https://github.com/active-logic/active-lt-demos/commit/2eb8fad58d6f526c0cedf126ba540008ed8b5f51#diff-bfe87ccd636cdb1785402203578e6f0fcc69ad43614e06e26b728dce72d398f8).

We also migrated the details of cloning Frogger because this does not partake the BT.

Next we hide model variables for better encapsulation (see [diff](https://github.com/active-logic/active-lt-demos/commit/4ca5cac531747677000a16d25a6e8b6ab51dd462#diff-bfe87ccd636cdb1785402203578e6f0fcc69ad43614e06e26b728dce72d398f8)).

### Cleaner BT using actions.

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

Active Logic has other 'restricted statuses' (aka [certainties](https://github.com/active-logic/activelogic-cs/blob/master/Doc/Reference/Certainties.md)). Here the `loop` type (return `forever`) would avoid the `-` operator. Still, `action` is a straightforward choice, useful for interfacing BTs with external APIs.

## Separating apperception from control

From a performance perspective, one of costlier aspects in game AI is apperception - the process of converting raw sensor sensor input to a simple representation that control can handle.

In the Frogger demo apperception is simplified. We assume *one* obstacle to avoid (the nasty ball); still, code related to detecting objects of interest and measuring distances is not directly relevant to control. As such it only contributes clutter.

Another issue is, if we want to improve performance, running apperception at a lower frame rate is a good choice. But we still want to run the control layer at refresh rate.

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

For good measure here is the final version of `FroggerAp.cs`, illustrating how we may run apperception at a lower frame rate:

```cs
using UnityEngine;

public class FroggerAp : MonoBehaviour{

    public Vector3   dodgeVector;
    public Transform food;

    public float Distance(Transform target)
    => (target.position - transform.position).magnitude;

    public Vector3 Direction(Transform target)
    => (target.position - transform.position).normalized;

    public Vector3 JumpVector(Transform target)
    => Direction(target) * 0.02f + Vector3.up * 0.2f;

    // --------------------

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
```

Subsequently, tasks in `Frogger.cs` may be simplified:

```cs
public class Frogger : UGig{

    // ...

    status Dodge()
    => (ap.dodgeVector == Vector3.zero) ? done :
       -model.Propel(ap.dodgeVector * 3f);

    status Feed()
    => !model.hungry
    || ((ap.food!=null) && Reach(ap.food) && -model.Feed());

    // ...

    status Reach(Transform target)
    => (ap.Distance(target) < 1f) ? done :
       (model.speed > 1e-6f)      ? cont :
       -model.Impel(ap.JumpVector(target));

    // ...

}
```

Once we have removed apperception and state related clutter from the BT, only case logic remains.

Tip: *As much as we love concision, readability is also important: write status expressions that you (and your team) will understand 3 months down the line.*

## What we learned

Separation of concerns makes our game AIs readable and easier to maintain; separating and encapsulating model state is advised.

For good performance, separate control from apperception.

A clean control layer does not involve intermediate calculations, or preempt the details of how low level actions are performed. This approach makes our game logic faster, and easier to maintain.

With practice you may feel compelled to write very concise logic; do consider how complex functions may be harder to read and test.

## Next up

In the fourth and final part of this tutorial, we will take a look at the logging API.
