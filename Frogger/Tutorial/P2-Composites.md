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

Now that we have a working setup we'll sketch our AI 'top down' starting with an empty implementation of each high level task.
