using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BGE.Geom;

public class individualFlockingFish : MonoBehaviour {
    /*
        远景鱼个体，远景鱼的运动由以下几种运动加权合成：
         flocking（集群运动）、seeking（向心运动）、avoiding（躲避障碍物）
         集群运动又由alignment（排列）、collection（聚集）、seperation（自我）加权合成
         */



    public enum SWIM_STATE {
        FLOCKING = 0,
        ESCAPE = 1
    }
    SWIM_STATE state = SWIM_STATE.FLOCKING;

    // 开放参数
    public FlockingFishBasicAction thisFishAction;
    public flockingFishManager managerObj;


    // 私有参数
    public Vector3 target = new Vector3(0, 0, 0);
    public float rotateSpeed = 0;
    public float rotateAngle = 0;

    public float lerpDirAngle;


    // 当前运动方向
    public Vector3 moveVector = new Vector3(0, 0, 0);


    // 临近的远景鱼链表
    public List<individualFlockingFish> neighborFishes = new List< individualFlockingFish >( );


    public float curTime = 0;
    public float thinkGap = 0;


    // Use this for initialization
    void Start ()
    {
        target.z = 0;
        managerObj = GameObject.Find("flockingFishManager").GetComponent<flockingFishManager>();
		thisFishAction = gameObject.GetComponent<FlockingFishBasicAction>();
        
        rotateAngle = Random.Range(managerObj.limitRotateAngle.x, managerObj.limitRotateAngle.y);
        thinkGap = Random.Range(managerObj.limitThinkGap.x, managerObj.limitThinkGap.y);

		moveVector = Random.insideUnitSphere;
        
        curTime = thinkGap;

        lerpDirAngle = Random.Range(managerObj.limitLerpDirAngle.x, managerObj.limitLerpDirAngle.y);
    }
    
    
    void UpdateNeighborInfo() {
        neighborFishes.Clear();
        foreach (individualFlockingFish obj in managerObj.fishes) 
        {
			if (obj.gameObject != thisFishAction.gameObject) 
            {
                float dist = Vector3.Distance( 
                    obj.transform.position, 
					thisFishAction.transform.position 
                );
                if (dist < managerObj.senseDistNeighbor) 
                {
                    neighborFishes.Add(obj);
                }
            }
        }
    }


    void borderJudge()
    {
        if (thisFishAction.transform.localPosition.x < (managerObj.left) && moveVector.x < 0)
        {
            float angleX = Random.Range(0, lerpDirAngle);
            moveVector.x = Mathf.Cos(angleX);
            
            float radius = Mathf.Sin(angleX);
            float angleO = Random.Range(0, Mathf.PI * 2);
            
            moveVector.y = Mathf.Cos(angleO) * radius;
            moveVector.z = Mathf.Sin(angleO) * radius;

        }
        else if (thisFishAction.transform.localPosition.x > (managerObj.right) && moveVector.x > 0)
        {
            float angleX = Random.Range(0, lerpDirAngle);
            moveVector.x = -Mathf.Cos(angleX);
            
            float radius = Mathf.Sin(angleX);
            
            float angleO = Random.Range(0, Mathf.PI * 2);
            
            moveVector.y = Mathf.Cos(angleO) * radius;
            moveVector.z = Mathf.Sin(angleO) * radius;

        }
        else if (thisFishAction.transform.localPosition.y < (managerObj.bottom) && moveVector.y < 0)
        {
            float angleY = Random.Range(0, lerpDirAngle);
            moveVector.y = Mathf.Cos(angleY);
            
            float radius = Mathf.Sin(angleY);
            
            float angleO = Random.Range(0, Mathf.PI * 2);
            
            moveVector.x = Mathf.Cos(angleO) * radius;
            moveVector.z = Mathf.Sin(angleO) * radius;
        }
        else if (thisFishAction.transform.localPosition.y > (managerObj.top) && moveVector.y > 0)
        {
            float angleY = Random.Range(0, lerpDirAngle);
            moveVector.y = -Mathf.Cos(angleY);
            
            float radius = Mathf.Sin(angleY);
            
            float angleO = Random.Range(0, Mathf.PI * 2);
            
            moveVector.x = Mathf.Cos(angleO) * radius;
            moveVector.z = Mathf.Sin(angleO) * radius;
        }
        else if (thisFishAction.transform.localPosition.z < (managerObj.back) && (moveVector.z < 0))
        {
            float angleZ = Random.Range(0, lerpDirAngle);
            moveVector.z = Mathf.Cos(angleZ);
            
            float radius = Mathf.Sin(angleZ);
            
            float angleO = Random.Range(0, Mathf.PI * 2);
            
            moveVector.x = Mathf.Cos(angleO) * radius;
            moveVector.y = Mathf.Sin(angleO) * radius;
        }
        else if (thisFishAction.transform.localPosition.z > (managerObj.front) && (moveVector.z > 0))
        {
            float angleZ = Random.Range(0, lerpDirAngle);
            moveVector.z = -Mathf.Cos(angleZ);
            
            float radius = Mathf.Sin(angleZ);
            float angleO = Random.Range(0, Mathf.PI * 2);
            
            moveVector.x = Mathf.Cos(angleO) * radius;
            moveVector.y = Mathf.Sin(angleO) * radius;
        }

    } 



    void ObstacleAvoid() 
    { 
        if (managerObj.obstacles.Length == 0) 
        { 
            return ;
        } 
        List<Transform> tagObjs = new List<Transform>();

        foreach (Transform obstacle in managerObj.obstacles) 
        { 
            if (obstacle == null) 
            { 
                Debug.Log("Null object");
                continue;
            } 

            Vector3 toCentre = transform.position - obstacle.transform.position;
            float dist = toCentre.magnitude - obstacle.transform.localScale.x;
            if ( dist < managerObj.minDistObstacle ) 
            {
                if ( MayCrash( transform.position, moveVector,
                    GetRadius( obstacle ), obstacle.transform.position ) && 
                    Vector3.Distance( transform.position, obstacle.transform.position ) < managerObj.minDistObstacle + GetRadius( obstacle ) 
                    ) 
                {
                    tagObjs.Add(obstacle);
                } 
            } 
        }

        if (tagObjs.Count <= 0)
        {
            state = SWIM_STATE.FLOCKING;
            return;
        }
        else
        {
            state = SWIM_STATE.ESCAPE;
        }


        int indexMaxO = 0;
        for( int i=0; i<tagObjs.Count; i++ ) 
        { 
            if (GetRadius(tagObjs[indexMaxO]) < GetRadius(tagObjs[i]) ) 
            { 
                indexMaxO = i;
            } 
        }

        float maxDist = managerObj.minDistObstacle + GetRadius(tagObjs[indexMaxO]);
        
        Vector3 nV = transform.position - tagObjs[indexMaxO].transform.position;

        float factor = Vector3.Distance( transform.position, tagObjs[indexMaxO].transform.position  ) / maxDist;

        moveVector = ( nV.normalized * (1-factor) * 100 + moveVector ).normalized;
        
        return  ; 
    }
    
    bool MayCrash( Vector3 oriPos, Vector3 dir, float radius, Vector3 sphereCenter ) 
    {
        Vector3 toObstacle = sphereCenter - oriPos;
        
        return Vector3.Angle(toObstacle, dir) * Mathf.Deg2Rad < Mathf.Asin(radius / toObstacle.magnitude) ? true : false;
    }




    public float defaultRadius = 5.0f;
    private float GetRadius( Transform obj )
    {
        Renderer r = obj.gameObject.GetComponent<Renderer>();
        if (r == null) 
        {
            return defaultRadius;
        }
        else
        {
            return r.bounds.extents.magnitude;
        }
    }


    // Update is called once per frame
    void Update () {
        UpdateNeighborInfo();
        
        ObstacleAvoid();

        if (curTime < thinkGap)
        {
            curTime += Time.deltaTime;
        }
        else
        {
            curTime = 0;
            if (state == SWIM_STATE.FLOCKING)
            {
                moveVector = ((GetFlockingVelocity().normalized * neighborFishes.Count + moveVector).normalized
                 * managerObj.集群权重
                + 向心() * managerObj.向心权重).normalized
                ;
            }
            
            target = thisFishAction.transform.position + moveVector;
			thisFishAction.HeadTo(target, managerObj.moveSpeed);
        }
    }
    
    // flocking
    Vector3 排列()
    {
        Vector3 averageDirection = Vector3.zero;

        if (neighborFishes.Count == 0)
            return averageDirection;

        foreach (var agent in neighborFishes)
            averageDirection += agent.moveVector;

        averageDirection /= neighborFishes.Count;
		return averageDirection.normalized;
    }

    Vector3 聚集()
    {
        Vector3 averagePosition = Vector3.zero;

        foreach (var agent in neighborFishes)
            averagePosition += (Vector3)agent.transform.position;

        averagePosition /= neighborFishes.Count;

		return (averagePosition - thisFishAction.transform.position).normalized;
    }

    Vector3 自我()
    {
        Vector3 moveDirection = Vector3.zero;

        foreach (var agent in neighborFishes)
			moveDirection += (thisFishAction.transform.position - agent.transform.position);

		return moveDirection.normalized;
    }


    Vector3 向心() {
        Vector3 moveDir = Vector3.zero;

        moveDir = managerObj.centerPosObj.transform.localPosition - transform.localPosition;

        return moveDir;
    }


    public Vector3 GetFlockingVelocity()
    {
		return 排列() * managerObj.排列 + 聚集() * managerObj.聚集 + 自我() * managerObj.自我;
    }

    
    void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(
        //    thisFishAction.transform.position,
        //     managerObj.senseDistNeighbor
        //    );

    }
}
