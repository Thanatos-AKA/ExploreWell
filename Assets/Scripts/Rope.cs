using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour{
    //prefab rope and list of rope
    public GameObject ropeSegmentPrefab;
    List<GameObject> ropeSegments = new List<GameObject>();

    //if the rope is incresing or decreasing
    public bool isIncreasing{get; set;}
    public bool isDecreasing{get; set;} 

    //the rigidbody which connects to the end of rope
    public Rigidbody2D connectedObject;

    //the maximum length of rope (add another pieces when it get too long)
    public float maxRopeSegmentLength = 1.0f;

    //casting speed
    public float ropeSpeed = 4.0f;

    //rope's renderer
    LineRenderer lineRenderer;

    //destroy all the rope segments and use CreateRopeSegment
    public vois ResetLength(){
        foreach (GameObject segment in ropeSegments){
            Destroy(segment);
        }

        ropeSegments = new List<GameObject>();
        isDecreasing = false;
        isIncreasing = false;
        CreateRopeSegment();
    }

    void CreateRopeSegment(){
        //create new segment
        GameObject segment = (GameObject)Instantiate(ropeSegmentPrefab, this.transform.position, Quaternion.identity);

        //make the new segment be the child of this object
        //remain it's "world position"
        segment.transform.SetParent(this.transform, true);

    }

    void Start(){
        
    }

    void Update(){
        
    }
}
