# Frogger tutorial P2 - Frog life

In the first part of this tutorial we created a simple 'frog' which implemented a simple *task*: hopping til she reaches a vertical wall.

In this second part let's evolve Frogger into acceptable wildlife.

## Maslow's hierachy of needs: a tribute?

A very acceptable structure for BT agents consists in prioritizing immediate threats/needs over light-hearted, wittier activities such as saving Peach. With Frogger, we consider the following activities:

- Avoid getting trampled (self-preservation)
- Eat aka avoid starving (self-preservation)
- Grow and multiply (long-term preservation!?)

Side goal implement the above within 100 lines of code?

## A good gig

Let's ditch `MonoBehaviour`; just keep in mind AL works with `MonoBehaviour` and it will also work with your visual BT asset, if any (say, for implementing low level tasks).

Rename `Frogger.cs` to `FroggerZero.cs`. Next re-create a file named `Frogger.cs`.

```cs
using UnityEngine;
using Active.Core;
using static Active.Raw;

public class Frogger : UGig{

    Rigidbody body;
    public float traction = 10;

    override public status Step(){
        body.AddForce(Vector3.right * traction);
        return cont;
    }

    void Start(){
        body = GetComponent<Rigidbody>();
    }

}
```

In your setup scene ensure Frogger uses the new 'Frogger' script; press play and confirm nothing happening, no errors.

`UGig` is a lightweight replacement for `MonoBehaviour`. It does not define any state, but also does nothing on its own. Add an `Agent` component and retest. Frogger is now rolling to the right.

![alt text](Images/Roll.png)

## Top-down design

Now that we have a working setup we sketch our AI 'top down' starting with an empty implementation of each high level task.

```cs
public class Frogger : UGig{

    Rigidbody body;
    public float traction = 10;

    override public status Step(){
        return Dodge() || Feed() || Spawn();
    }

    status Dodge(){ print("✔ Dodge"); return done; }
    status Feed (){ print("𐄂 Feed");  return fail; }
    status Spawn(){ print("𐄂 Spawn"); return fail; }

    void Start(){
        body = GetComponent<Rigidbody>();
    }

}
```

`Step()` implements a selector, meaning that subtasks are listed in order of their decreasing priority. In BT this is also known as a *fallback strategy*

### Understanding selectors

Press play; console output indicates that `Dodge()` is called in a repeating sequence.

Now replace this:

```cs
status Dodge(){ print("✔ Dodge"); return done; }
```

With:

```cs
status Dodge(){ print("~ Dodge"); return cont; }
```

Again press play and observe that `Dodge` is printed in a repeating sequence. While the `Dodge` task has not completed it has not failed either, therefore the selector will not attempt successive tasks.

Finally replace `cont` with `fail`:

```cs
status Dodge(){ print("x Dodge"); return fail; }
```

In this case, we then observe that `Dodge`, `Feed` and `Spawn` are called in a repeating sequence. At every game frame the selector tries each task in succession. Finally `fail` is returned.

|||| ***A selector iterates, until the first succeeding task is found.***

NOTE: *Active logic provides its logging API - we will cover this in the next tutorial*

Now that we're done figuring selectors, we will simplify our skeletal implementation using C# 7 style syntax:

```cs
public class Frogger : UGig{

    // ...

    override public status Step()
    => return Dodge() || Feed() || Spawn();

    status Dodge() => fail;
    status Feed () => fail;
    status Spawn() => fail;

    // ...

}
```

When dealing with status functions the expression bodied syntax is advised for clarity and concision; it is not required - sometimes the bracketted syntax is more convenient.

With that, sub-tasks may be implemented in any order. I'll start with `Feed`

## A good snack

In the Unity Editor create a 'Food' tag.

Next here is our first (incomplete) implementation of `Feed`:

```cs
status Feed (){
    var food = GameObject.FindWithTag("Food");
    if(food) return cont;
    print("No food");
    return fail;
}
```

We now add an object to represent 'food' (I used a flattened cylinder named 'Crop'). Tag the object using the *Food* tag.

![alt text](Images/Crop.png)

We want the Frog to get near the food. For simplicity I'll assume a renewable crop so 'eating' won't actually cause the crop to be depleted or destroyed. Let's update our implementation

```cs
status Feed (){
    var food = GameObject.FindWithTag("Food").transform;
    if(food){
        return Reach(food) && Consume(food);
    }else{
        return fail;
    }
}

status Reach(Transform obj)    => fail;
status Consume(Transform obj)  => fail;
```

As previously we continue refining top-down. Before proceeding, however, notice the `Reach(food) && Consume(food)` syntax. In AL the `A && B && ...` form specifies a *sequence*.

|||| ***A sequence iterates, until the first failing task is found.***

Given how sequences are defined, Frogger will move towards a crop or other food, until within feeding range. If Frogger is disturbed either while getting close, or while snacking on the delicious crop, she may later resume approaching the food source.

We now implement the `Reach` task:

```cs
status Reach(Transform target){
    var u = target.position - transform.position;
    var dist = u.magnitude;
    if(dist < 1f) return done;
    body.AddForce(u * traction);
    return cont;
}
```

Press play. Frogger quickly homes in on the food source. However this is not good enough! We prefer Frogger to hop so we'll just borrow a little from `FroggerZero`:

```cs
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
```

And sure enough...

![alt text](Images/Reach.png)

The crop won't get depleted but we still want to track Frogger's "hunger" state; for this we may add a "hunger" variable to `Frogger.cs`; eating reduces hunger.

```cs
public class Frogger : UGig{

    // ...

    public int hunger = 100;

    // ...

    status Consume(Transform obj){
        if(hunger <= 0 ) return done;
        hunger--;
        return cont;
    }

}
```

Testing, observe that, while near the crop, Frogger's hunger decrases until it reaches zero.

Our implementation of the feeding/metabolic process is not very neat. Hunger should increase over time (functional bug). Also, adding variables to the controller is not good practice. Still, we are doing good! Let's push our luck.

## Nasty ball of dark

Every hero has an archenemy. Likewise Frogger is constantly threatened by a powerful, evil foe. Create a nasty ball of dark as illustrated. We also add "walls" around the game area (or the ball will fall off).

[SETUP]

Promptly we create a `Foe` script:

```cs
public class Foe: Gig{

    public float traction = 10;

    status Step(){
        GetComponent<Rigidbody>().AddForce(
            transform.forward, ForceMode.Impulse);
            return cont;
    }

}
```

Attach a rigid body to the nasty ball. Also add the `Foe` and a `Ticker` component. Set the ticker's interval to 5f and let's play.

[RESULT]

Similar to agents, tickers drive the behavior tree. The difference is that a ticker fires at arbitrary intervals (here, every 5 seconds).

## A run for it

We will now implement the "dodge" task.
