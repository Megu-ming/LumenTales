using System;
using UnityEngine;

public class CharacterEvents
{
    public static Action<GameObject, int> characterDamaged;
    public static Action<GameObject, int> characterHealed;
}
