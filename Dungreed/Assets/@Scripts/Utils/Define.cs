using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define 
{
    public enum Scene
    {
        Unknown,
        DevScene,
        GameScene,
    }

    public enum Sound
    {
        Bgm,
        Effect,
    }

    public enum ObjectType
    {
        Player,
        Monster,
        Projectile,
        Env
    }

    public enum SKillType
    {
        None,
        Melee,
        Projectile,
        Etc
    }

    public const int PLAYER_DATA_ID = 1;
    public const string EXP_GEM_PREFAB = "EXPGem.prefab";
}
