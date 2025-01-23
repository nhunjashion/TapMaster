using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static UnityEditor.PlayerSettings;

namespace TapMaster
{
    public class CubeObj : MonoBehaviour 
    {
        public int ID;
        public Cube cube;
        public CubeDirection direction;
        public CubeDirection directionCounter;
        public Vector3 raycastDir;
        public Renderer rd;
        public Rigidbody rb;
        public BoxCollider cl;
        public bool isMerge;
        public CubeObj parent = null;
        [Header("QUAD")]
        public GameObject frontQuad;
        public GameObject backQuad;
        public GameObject righQuad;
        public GameObject leftQuad;
        public GameObject upQuad;
        public GameObject downQuad;


        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            cl = GetComponent<BoxCollider>();
            isMerge = false;
        }

        public void SetPosition(Vector3 pos, int id)
        {
            cube.itemID = id;
            this.ID = id; 
            cube.posX = pos.x;
            cube.posY = pos.y;
            cube.posZ = pos.z;
            this.transform.position = pos;
        }

        public void ResetCube()
        {
            this.gameObject.SetActive(true);
            this.cl.enabled = true;
            this.rb.isKinematic = true;
            this.transform.position = cube.pos;
        }

        public void SetDirection(CubeDirection dir)
        {
            direction = dir;
            switch (dir)
            {
                case CubeDirection.Up:
                    raycastDir =  Vector3.up;
                    directionCounter = CubeDirection.Down;
                    break;
                case CubeDirection.Down:
                    raycastDir = Vector3.down;
                    directionCounter = CubeDirection.Up;
                    break;
                case CubeDirection.Left:
                    raycastDir = Vector3.left;
                    directionCounter = CubeDirection.Right;
                    break;
                case CubeDirection.Right:
                    raycastDir = Vector3.right;
                    directionCounter = CubeDirection.Left;
                    break;
                case CubeDirection.Front:
                    raycastDir = Vector3.forward;
                    directionCounter = CubeDirection.Back;
                    break;
                case CubeDirection.Back:
                    raycastDir = Vector3.back;              
                    directionCounter = CubeDirection.Front;
                    break;
                default:
                    break;
            }
       
        }


        public void SetQuadValue(CubeDirection dir)
        {
            SetQuadDirection(dir);
            SetDirQuad(dir);
        }

        public void SetQuadDirection(CubeDirection dir)
        {
            switch (dir)
            {
                case CubeDirection.Up:
                    HideQuad(CubeDirection.Up);
                    break;
                case CubeDirection.Down:
                    HideQuad(CubeDirection.Up);
                    break;
                case CubeDirection.Left:
                    HideQuad(CubeDirection.Left);
                    break;
                case CubeDirection.Right:
                    HideQuad(CubeDirection.Left);
                    break;
                case CubeDirection.Front:
                    HideQuad(CubeDirection.Front);
                    break;
                case CubeDirection.Back:
                    HideQuad(CubeDirection.Front);
                    break;
                default:
                    break;
            }
        }
        public void HideQuad(CubeDirection dir)
        {
            switch (dir)
            {
                case CubeDirection.Up:
                    upQuad.SetActive(false);
                    downQuad.SetActive(false);
                    break;
                case CubeDirection.Left:
                    leftQuad.SetActive(false);
                    righQuad.SetActive(false);
                    break;
                case CubeDirection.Front:
                    frontQuad.SetActive(false);
                    backQuad.SetActive(false);
                    break;
            }
        }

        public void SetDirQuad(CubeDirection dir)
        {
            switch (dir)
            {
                case CubeDirection.Up:
                    RotQuad(180);
                    break;
                case CubeDirection.Down:
                    RotQuad(0);
                    break;
                case CubeDirection.Left:
                    RotQuad(-90);
                    break;
                case CubeDirection.Right:
                    RotQuad(90);
                    break;
                case CubeDirection.Front:
                    RotQuad2(90,180);
                    break;
                case CubeDirection.Back:
                    RotQuad2(-90,0);
                    break;
                default:
                    break;
            }
        }

        public void RotQuad(float z)
        {
            upQuad.transform.Rotate(0, 0, z);
            downQuad.transform.Rotate(0, 0, z);
            righQuad.transform.Rotate(0, 0, z);
            leftQuad.transform.Rotate(0, 0, z);
            backQuad.transform.Rotate(0, 0, z);
            frontQuad.transform.Rotate(0, 0, -z);
        }

        public void RotQuad2(float z1, float z2)
        {
            if(z2 == 0) downQuad.transform.Rotate(0, 0, 180);
            if(z2 == 180) downQuad.transform.Rotate(0, 0, 0);
            upQuad.transform.Rotate(0, 0, z2);
            righQuad.transform.Rotate(0, 0, z1);
            leftQuad.transform.Rotate(0, 0, -z1);
            backQuad.transform.Rotate(0, 0, z2);
            frontQuad.transform.Rotate(0, 0, z2);
        }

        public void OnMouseUpAsButton()
        {
            Debug.Log("Direction: " + direction);
            if(parent != null)
                CubeCtrl.Instance.cubeSelect = parent;
            else
                CubeCtrl.Instance.cubeSelect = this;
            CubeCtrl.Instance.CheckDirection();     
        }

        private void OnMouseEnter()
        {
            this.rd.material.color = Color.red;
        }
        private void OnMouseExit()
        {
            this.rd.material.color = Color.white;
        }

        private void Update()
        {
            Vector3 pivot = new(0, 0, 0);
            float distance = Vector3.Distance(pivot, this.transform.position);
            if (distance >= 10)
                this.gameObject.SetActive(false);
                    
        }
    }

    public enum CubeDirection
    {
        Up,
        Down,
        Left,
        Right,
        Front,
        Back
    }
}
