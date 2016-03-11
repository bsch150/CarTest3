using UnityEngine;
using System.Collections;

public abstract class PowerballBallBehavior {
    abstract public void inactiveAct();
    abstract public void activeAct();
    abstract public void collideWith(Collider other, Powerball p);
}
