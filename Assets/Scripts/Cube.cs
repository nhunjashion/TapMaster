using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TapMaster
{
    [Serializable]
    public class Cube
    {
        public int itemID;
        public float posX;
        public float posY;
        public float posZ;

        public Vector3 pos => new Vector3(posX, posY, posZ);
        public Vector2 Up => new(posX, posZ);
        public Vector2 Left => new(posY, posZ);
        public Vector2 Front => new(posX, posY);
    }
}