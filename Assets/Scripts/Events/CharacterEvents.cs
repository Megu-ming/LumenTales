using System;
using UnityEngine;

public class CharacterEvents
{
    public static Action<GameObject, float> characterDamaged;
    public static Action<GameObject, float> characterHealed;

    public static Action infoUIRefresh;
}
