// Copyright 2024 hope1026.

public static class UnityLayerDefines
{
    public class LayerInder
    {
        public const int MONSTER = 6;
        public const int GROUND = 7;
        public const int TOWER = 8;
    }

    public class LayerMask
    {
        public const int MONSTER = 1 << LayerInder.MONSTER;
        public const int GROUND = 1 << LayerInder.GROUND;
        public const int TOWER = 1 << LayerInder.TOWER;
    }
}