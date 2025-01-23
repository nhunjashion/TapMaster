using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Random = System.Random;

namespace TapMaster
{
    public class CubeCtrl : MonoBehaviour
    {
        public static CubeCtrl Instance { get; private set; }

        [Header("Field inf")]
        [SerializeField] float spacing = .5f;
        [SerializeField] int countX;
        [SerializeField] int countY;
        [SerializeField] int countZ;
        [SerializeField] Transform field;
        [SerializeField] Transform camTarget;
        [SerializeField] GameObject cubeGroup;
        [SerializeField] Camera cam;

        public float rotSpeed = 100;
        public bool dragging = false;


        [Header("Cube")]
    
        [SerializeField] CubeObj cubePrefab;
        [SerializeField] List<CubeObj> listCube;

        [SerializeField] GameObject ray;
        public CubeObj cubeSelect;
        Vector3 previousPos;


        [Header("Fx")]
        [SerializeField] ParticleSystem hideFx;
        private void Start()
        {
            isSpellReady = false;
            Instance = this;
            //SpawnCube();
        }

        public List<CubeObj> listTest = new();
        #region SPAWN CUBE
        public void SpawnCube()
        {
            GameSceneManager.Instance.deleteBtn.interactable = true;
            ClearData();

            countX = LevelManager.Instance.CountX;
            countY = LevelManager.Instance.CountY;
            countZ = LevelManager.Instance.CountZ;


            int index = 0;

            float xPos = ((countX-1) * spacing - spacing) /2;
            float yPos = ((countY-1) * spacing - spacing) /2;
            float zPos = ((countZ-1) * spacing - spacing) /2;
            cubeGroup.transform.position = new(-xPos,-yPos,-zPos);

            for (int i = 0; i < countZ; i++)
            {
                for (int j = 0; j < countX; j++)
                {
                    for (int z = 0; z < countY; z++)
                    {
                        CubeObj item = Instantiate(cubePrefab, field,true);
                        item.SetPosition(new Vector3(j - j*spacing, z  - z*spacing, i  - i* spacing), index);
                        item.name = index.ToString();
                        listCube.Add(item);
                        index++;
                    }
                }
            }

            SetCubeDir();

            Vector3 newDir = (listCube[0].gameObject.transform.position + listCube[listCube.Count -1].transform.position)/2;
            camTarget.position = newDir;

            foreach (var item in listCube)
            {
                ResetDir(item);
               // CheckCubeOnFlat(item);
                //CheckingDir(item);
            }


            foreach (var item in listCube)
            {
                CheckingDir(item);
               // ResetDir(item);
            }


            foreach (CubeObj item in listCube)
            {
                //if (!item.isMerge) MergerCube(item);

                item.SetQuadValue(item.direction);
            }

          //  FindLine();




            foreach (CubeObj item in listCube)
            {
                listTest.Add(item);
            }

        }

        public void ResetMap()
        {
            for (int i = 0; i < field.childCount; i++)
            {
                CubeObj item = field.GetChild(i).GetComponent<CubeObj>();
                item.ResetCube();
            }
        }


        public void FindLine()
        {
            int count = listCube.Count;

            foreach(var item in listCube)
            {
                RaycastHit hit;
                if (Physics.Raycast(item.transform.position, item.transform.TransformDirection(item.raycastDir), out hit, Mathf.Infinity))
                {
                    continue;
                }
                else
                {
                    item.gameObject.SetActive(false);

                    item.GetComponent<Collider>().enabled = !enabled;
                }
            }
        }

        public void CheckingLine(CubeObj item)
        {
            RaycastHit hit;
            if (Physics.Raycast(item.transform.position, item.transform.TransformDirection(item.raycastDir), out hit, Mathf.Infinity))
            {
                CubeObj node = hit.collider.gameObject.GetComponent<CubeObj>();
                foreach (var obj in listCube)
                {
                    if (node.ID == obj.ID)
                    {
                        CubeDirection dir = GetEnumValue(item.direction);
                        item.SetDirection(dir);
                        break;
                    }
                }
            }
            else return;
        }
        public void CheckingDir(CubeObj cube)
        {
            switch (cube.direction)
            {
                case CubeDirection.Up:
                    CheckingCounterDir(CubeDirection.Up, cube);
                    break;
                case CubeDirection.Down:
                    CheckingCounterDir(CubeDirection.Up, cube);
                    break;
                case CubeDirection.Left:
                    CheckingCounterDir(CubeDirection.Left, cube);
                    break;
                case CubeDirection.Right:
                    CheckingCounterDir(CubeDirection.Left, cube);
                    break;
                case CubeDirection.Front:
                    CheckingCounterDir(CubeDirection.Front, cube);
                    break;
                case CubeDirection.Back:
                    CheckingCounterDir(CubeDirection.Front, cube);
                    break;
                default:
                    break;
            }
        }

        public void CheckingCounterDir(CubeDirection dir, CubeObj cube)
        {
            switch (dir)
            {
                case CubeDirection.Up:
                    foreach(var item in listCube) if (item.cube.Up == cube.cube.Up)
                    {
                            if(item.direction == cube.directionCounter)
                                cube.SetDirection(item.direction);
                        }
                    break;

                case CubeDirection.Left:
                    foreach (var item in listCube) if (item.cube.Left == cube.cube.Left)
                    {
                            if (item.direction == cube.directionCounter)
                                cube.SetDirection(item.direction);
                    }
                    break;

                case CubeDirection.Front:
                    foreach (var item in listCube) if (item.cube.Front == cube.cube.Front)
                    {
                            if (item.direction == cube.directionCounter)
                                cube.SetDirection(item.direction);
                    }
                    break;
            }
        }

    
        public void MergerCube(CubeObj itemParent)
        {
            List<CubeObj> listCubeNei = new();

            float x = itemParent.cube.posX;
            float y = itemParent.cube.posY;
            float z = itemParent.cube.posZ;

            listCubeNei.Add(GetBlockByXYZ(x, y + 1, z)); 
            listCubeNei.Add(GetBlockByXYZ(x + 1, y, z)); 
            listCubeNei.Add(GetBlockByXYZ(x, y - 1, z));
            listCubeNei.Add(GetBlockByXYZ(x - 1, y, z));
            listCubeNei.Add(GetBlockByXYZ(x, y, z + 1));
            listCubeNei.Add(GetBlockByXYZ(x, y, z - 1));
 
            Debug.Log("nei count: " + listCubeNei.Count);

            foreach(var item in listCubeNei)
            {
                if (item != null)
                {
                    if (!item.isMerge && item.direction == itemParent.direction)
                    {
                        item.transform.SetParent(itemParent.transform);
                        item.isMerge = true;
                        item.parent = itemParent;
                        item.GetComponent<BoxCollider>().enabled = !enabled;
                        itemParent.isMerge = true;
                        LevelManager.Instance.LevelTarget -= 1;
                        Debug.Log("Merge item" + item.name);
                        Debug.Log("Merge item" + itemParent.name);
                    }
                }
            }
        }


        public CubeObj GetBlockByXYZ(float x, float y, float z)
        {
            CubeObj blockNeighbor = null;
            foreach (CubeObj block in this.listCube)
            {
                if (block.cube.posX == x && block.cube.posY == y && block.cube.posZ == z) blockNeighbor = block;
                else if (x < 0 || x > countX - 1 || y < 0 || y > countY - 1 || z < 0 || z > countZ -1)
                    blockNeighbor = null;
            }

            return blockNeighbor;
        }


        public void SetCubeDir()
        {

            CubeDirection dir;
            foreach (CubeObj item in listCube)
            {
                dir = GetRandomEnumValue<CubeDirection>();
                Vector3 pos1 = item.transform.position;
                item.SetDirection(dir);
            }
        }


        public CubeDirection GetEnumValue(CubeDirection dirCompare)
        {
            CubeDirection dir = GetRandomEnumValue<CubeDirection>();

            if (dir != dirCompare)
                return dir;
            else GetEnumValue(dirCompare);

            return dir;
        }

        bool CheckNewDir(CubeObj item, Vector3 dir)
        {
            bool isSatisfy = true;
            RaycastHit[] hits;
            hits = Physics.RaycastAll(item.transform.position, item.transform.TransformDirection(dir), 40f);
            if (hits.Count() == 0) return true;
            foreach (var hit in hits)
            {
                CubeObj item2 = hit.collider.gameObject.GetComponent<CubeObj>();
                if (item2.directionCounter == item.direction)
                {
                    return false;
                }
                else
                    continue;
            }
            return isSatisfy;
        }
    
        CubeDirection SetNewDir(CubeObj item)
        {
            CubeDirection cubeDir = item.direction;
            if(CheckNewDir(item,item.raycastDir))
            {
                cubeDir = item.direction;
            }
            else
            {
                CubeDirection direction = GetEnumValue(item.direction);
                item.SetDirection(direction);
                SetNewDir(item);
            }

            return cubeDir;
        }



        public static T GetRandomEnumValue<T>()
        {
            Array values = Enum.GetValues(typeof(T));
            Random random = new();
            return (T)values.GetValue(random.Next(values.Length));
        }

        public void ClearData()
        {
            listCube.Clear();
            for (int i = 0; i < field.childCount; i++)
            {
                Destroy(field.GetChild(i).gameObject);
            }
        }

        CubeObj startCube;
        List<CubeObj> line = new();
        public void CheckLine(CubeObj startObj)
        {
            RaycastHit hit;
            if (Physics.Raycast(startObj.transform.position, startObj.transform.TransformDirection(startObj.raycastDir), out hit, Mathf.Infinity))
            {
                CubeObj node = hit.collider.gameObject.GetComponent<CubeObj>();
                line.Add(node);
                foreach(var item in line)
                {
                    if (node.ID == item.ID)
                    {
                        startObj.SetDirection(startObj.directionCounter);
                    }
                    else
                    {
                        CheckLine(node);
                    }
                }

            }
            else return;
        }



        public bool spellCount;
        public bool isSpellReady = false;

        public void OnClickDelete()
        {
            if(LevelManager.Instance.SpellAmount <= 0)
                GameSceneManager.Instance.deleteBtn.interactable = false;
            else
                isSpellReady = true;
            Debug.Log("use spell");
        }






        public void ResetDir(CubeObj cubeStart)
        {
            List<CubeObj> flat1 = new();
            List<CubeObj> flat2 = new();
            List<CubeObj> flat3 = new();

            flat1 = GetCubesOnFlat(cubeStart, Axis.x);
            flat2 = GetCubesOnFlat(cubeStart, Axis.y);
            flat3 = GetCubesOnFlat(cubeStart, Axis.z);

            foreach (var item in flat1)
            {
                CubeDirection dir = GetEnumValue(item.direction);
                item.SetDirection(dir);
                CheckingDir(item);
            }
            foreach (var item in flat2)
            {
                CubeDirection dir = GetEnumValue(item.direction);
                item.SetDirection(dir);
                CheckingDir(item);
            }
            foreach (var item in flat3)
            {
                CubeDirection dir = GetEnumValue(item.direction);
                item.SetDirection(dir);
                CheckingDir(item);
            }
        }




        public void CheckCubeOnFlat(CubeObj cubeStart)
        {

            List<CubeObj> flat1 = new();
            List<CubeObj> flat2 = new();

            CubeObj node1 = null;
            CubeObj node2 = null;



            switch (cubeStart.direction)
            {
                case CubeDirection.Up:
                    flat1 = GetCubesOnFlat(cubeStart, Axis.y);
                    flat2 = GetCubesOnFlat(cubeStart, Axis.x);

                    foreach (var item in flat1) if (item.cube.posY > cubeStart.cube.posY)
                        {
                            node1 = CubeNode(cubeStart, item, Axis.y);
                            node2 = CubeNode(item, cubeStart, Axis.y);

                            if (node1.direction == node2.directionCounter
                                && ((node1.direction == CubeDirection.Front && item.cube.posZ < cubeStart.cube.posZ)
                                    || (node1.direction == CubeDirection.Back && item.cube.posZ > cubeStart.cube.posZ)))
                            {
                                node1.SetDirection(cubeStart.direction);
                            }
                        }

                    foreach (var item in flat2) if (item.cube.posY > cubeStart.cube.posY)
                        {
                            node1 = CubeNode(cubeStart, item, Axis.x);
                            node2 = CubeNode(item, cubeStart, Axis.x);
                            ///////////
                            if (node1.direction == node2.directionCounter
                                && ((node1.direction == CubeDirection.Left && item.cube.posX > cubeStart.cube.posX)
                                    || (node1.direction == CubeDirection.Right && item.cube.posX < cubeStart.cube.posX)))
                            {
                                node1.SetDirection(cubeStart.direction);
                            }
                        }
                    break;

                case CubeDirection.Down:
                    flat1 = GetCubesOnFlat(cubeStart, Axis.y);
                    flat2 = GetCubesOnFlat(cubeStart, Axis.x);

                    foreach (var item in flat1) if (item.cube.posY < cubeStart.cube.posY)
                        {
                            node1 = CubeNode(cubeStart, item, Axis.y);
                            node2 = CubeNode(item, cubeStart, Axis.y);

                            if (node1.direction == node2.directionCounter
                                && ((node1.direction == CubeDirection.Front && item.cube.posZ > cubeStart.cube.posZ)
                                    || (node1.direction == CubeDirection.Back && item.cube.posZ < cubeStart.cube.posZ)))
                            {
                                node1.SetDirection(cubeStart.direction);
                            }
                        }
               
                    foreach (var item in flat2) if (item.cube.posY < cubeStart.cube.posY)
                        {
                            node1 = CubeNode(cubeStart, item, Axis.x);
                            node2 = CubeNode(item, cubeStart, Axis.x);
                            ///////////
                            if (node1.direction == node2.directionCounter
                                && ((node1.direction == CubeDirection.Left && item.cube.posX < cubeStart.cube.posX)
                                    || (node1.direction == CubeDirection.Right && item.cube.posX > cubeStart.cube.posX)))
                            {
                                node1.SetDirection(cubeStart.direction);
                            }
                        }
                    break;


                case CubeDirection.Left:
                    flat1 = GetCubesOnFlat(cubeStart, Axis.z);
                    flat2 = GetCubesOnFlat(cubeStart, Axis.x);

                    foreach (var item in flat1) if (item.cube.posX < cubeStart.cube.posX)
                        {
                            node1 = CubeNode(cubeStart, item, Axis.z);
                            node2 = CubeNode(item, cubeStart, Axis.z);

                            if (node1.direction == node2.directionCounter
                                && ((node1.direction == CubeDirection.Front && item.cube.posZ < cubeStart.cube.posZ)
                                    || (node1.direction == CubeDirection.Back && item.cube.posZ > cubeStart.cube.posZ)))
                            {
                                node1.SetDirection(cubeStart.direction);
                            }
                        }

                    foreach (var item in flat2) if (item.cube.posX > cubeStart.cube.posX)
                        {
                            node1 = CubeNode(cubeStart, item, Axis.x);
                            node2 = CubeNode(item, cubeStart, Axis.x);

                            if (node1.direction == node2.directionCounter
                                && ((node1.direction == CubeDirection.Down && item.cube.posY > cubeStart.cube.posY)
                                    || (node1.direction == CubeDirection.Up && item.cube.posY < cubeStart.cube.posY)))
                            {
                                node2.SetDirection(item.direction);
                            }
                        }
                    break;

                case CubeDirection.Right:
                    flat1 = GetCubesOnFlat(cubeStart, Axis.z);
                    flat2 = GetCubesOnFlat(cubeStart, Axis.x);

                    foreach (var item in flat1) if (item.cube.posX > cubeStart.cube.posX)
                        {
                            node1 = CubeNode(cubeStart, item, Axis.z);
                            node2 = CubeNode(item, cubeStart, Axis.z);

                            if (node1.direction == node2.directionCounter
                                && ((node1.direction == CubeDirection.Front && item.cube.posZ > cubeStart.cube.posZ)
                                    || (node1.direction == CubeDirection.Back && item.cube.posZ < cubeStart.cube.posZ)))
                            {
                                node1.SetDirection(cubeStart.direction);
                            }
                        }

                    foreach (var item in flat2) if (item.cube.posX < cubeStart.cube.posX)
                        {
                            node1 = CubeNode(cubeStart, item, Axis.x);
                            node2 = CubeNode(item, cubeStart, Axis.x);

                            if (node1.direction == node2.directionCounter
                                && ((node1.direction == CubeDirection.Down && item.cube.posY < cubeStart.cube.posY)
                                    || (node1.direction == CubeDirection.Up && item.cube.posY > cubeStart.cube.posY)))
                            {
                                node2.SetDirection(item.direction);
                            }
                        }
                    break;


                case CubeDirection.Front:
                    flat1 = GetCubesOnFlat(cubeStart, Axis.y);
                    flat2 = GetCubesOnFlat(cubeStart, Axis.z);

                    foreach (var item in flat1) if (item.cube.posZ > cubeStart.cube.posZ)
                    {
                        node1 = CubeNode(cubeStart, item, Axis.y);
                        node2 = CubeNode(item, cubeStart, Axis.y);
                         
                        if(node1.direction == node2.directionCounter
                            &&((node1.direction == CubeDirection.Up && item.cube.posY < cubeStart.cube.posY) 
                                ||(node1.direction == CubeDirection.Down && item.cube.posY > cubeStart.cube.posY)))
                        {
                                node1.SetDirection(cubeStart.direction);
                        }
                    }

                    foreach (var item in flat2) if (item.cube.posZ > cubeStart.cube.posZ)
                    {
                        node1 = CubeNode(cubeStart, item, Axis.z);
                        node2 = CubeNode(item, cubeStart, Axis.z);

                        if (node1.direction == node2.directionCounter
                            &&((node1.direction == CubeDirection.Right && item.cube.posX > cubeStart.cube.posX) 
                                || (node1.direction == CubeDirection.Left && item.cube.posX < cubeStart.cube.posX)))
                        {
                                node2.SetDirection(item.direction);
                        }
                    }
                    break;

                case CubeDirection.Back:
                    flat1 = GetCubesOnFlat(cubeStart, Axis.y);
                    flat2 = GetCubesOnFlat(cubeStart, Axis.z);

                    foreach (var item in flat1) if (item.cube.posZ < cubeStart.cube.posZ)
                    {
                        node1 = CubeNode(cubeStart, item, Axis.y);
                        node2 = CubeNode(item, cubeStart, Axis.y);
                         
                        if(node1.direction == node2.directionCounter
                            &&((node1.direction == CubeDirection.Up && item.cube.posY < cubeStart.cube.posY) 
                                ||(node1.direction == CubeDirection.Down && item.cube.posY > cubeStart.cube.posY)))
                        {
                                node1.SetDirection(item.direction);
                        }
                    }

                    foreach (var item in flat2) if (item.cube.posZ < cubeStart.cube.posZ)
                    {
                        node1 = CubeNode(cubeStart, item, Axis.z);
                        node2 = CubeNode(item, cubeStart, Axis.z);

                        if (node1.direction == node2.directionCounter
                            &&((node1.direction == CubeDirection.Right && item.cube.posX < cubeStart.cube.posX) 
                                || (node1.direction == CubeDirection.Left && item.cube.posX > cubeStart.cube.posX)))
                        {
                                node2.SetDirection(item.direction);
                        }
                    }
                    break;
            }

        }

        public List<CubeObj> GetCubesOnFlat(CubeObj cube, Axis axis)
        {
            List<CubeObj> cubeResult = new();

            switch (axis)
            {
                case Axis.x:
                    foreach (var item in listCube) if (item.cube.posZ == cube.cube.posZ && item.directionCounter == cube.direction)
                        {
                            cubeResult.Add(item);
                        }
                    break;
                case Axis.y:
                    foreach (var item in listCube) if (item.cube.posX == cube.cube.posX && item.cube.posZ == cube.cube.posZ && item.directionCounter == cube.direction)
                        {
                            cubeResult.Add(item);
                        }
                    break;
                case Axis.z:
                    foreach (var item in listCube) if (item.cube.posZ == cube.cube.posZ && item.cube.posY == cube.cube.posY && item.directionCounter == cube.direction)
                        {
                            cubeResult.Add(item);
                        }
                    break;
            }

            return cubeResult;
        }


        public float Distance(CubeObj cube1, CubeObj cube2)
        {
            return Vector3.Distance(cube1.transform.position, cube2.transform.position);
        }


        //x: mat phang xet tai GetCubeonFlat(Axis: x)
        public CubeObj CubeNode(CubeObj cube1, CubeObj cube2, Axis axis)
        {
            CubeObj result = null;
            switch (axis)
            {
                case Axis.x:
                    result = listCube.Find(x => x.cube.Front == cube1.cube.Front);
                    break;
                case Axis.y:
                    result = listCube.Find(x => x.cube.Left == cube1.cube.Left);
                    break;
                case Axis.z:
                    result = listCube.Find(x => x.cube.Left == cube1.cube.Left);
                    break;
            }
            return result;
        }

        #endregion


        public void CheckDirection()
        {
            switch (cubeSelect.direction)
            {
                case CubeDirection.Up:
                    DrawRayCast(Vector3.up);
                    break;
                case CubeDirection.Down:
                    DrawRayCast(Vector3.down);
                    break;
                case CubeDirection.Left:
                    DrawRayCast(Vector3.left);
                    break;
                case CubeDirection.Right:
                    DrawRayCast(Vector3.right);
                    break;
                case CubeDirection.Front:
                    DrawRayCast(Vector3.forward);
                    break;
                case CubeDirection.Back:
                    DrawRayCast(Vector3.back);
                    break;
            }
        }

        float speed = 400f;
        public void DrawRayCast(Vector3 dir)
        {
            RaycastHit hit;
            if(isSpellReady)
            {
                isSpellReady = false;
                cubeSelect.gameObject.SetActive(false);
                var eff = Instantiate(hideFx,cubeSelect.transform.position,Quaternion.identity);
                cubeSelect.GetComponent<BoxCollider>().enabled = !enabled;
                LevelManager.Instance.LevelTarget -= 1;
                if (LevelManager.Instance.SpellAmount <= 0)
                {
                    isSpellReady = false;
                    GameSceneManager.Instance.deleteBtn.interactable = false;
                }
                else
                {
                    if(LevelManager.Instance.SpellAmount -1 == 0) GameSceneManager.Instance.deleteBtn.interactable = false;
                    LevelManager.Instance.SpellAmount--;
                    LevelManager.Instance.SetTextSpellAmount();
                }
            }
            else
            {
                if (Physics.Raycast(cubeSelect.transform.position, cubeSelect.transform.TransformDirection(dir), out hit, Mathf.Infinity))
                {
                    return;
                }
                else
                {
                    cubeSelect.rb.isKinematic = false;
                    cubeSelect.GetComponent<Rigidbody>().AddForce(dir*speed);
                    cubeSelect.GetComponent<BoxCollider>().enabled = !enabled;
                    LevelManager.Instance.LevelTarget -= 1;
                    LevelManager.Instance.CheckWin();

                    Debug.Log("CAN MOVE");
                }
            }

        }
    }

    public enum Axis
    {
        x,
        y,
        z
    }
}
