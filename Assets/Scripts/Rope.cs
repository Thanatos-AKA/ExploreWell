﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour{
    public Rigidbody2D connectedObject;
    public GameObject ropeSegmentPrefab;
    List<GameObject> ropeSegments = new List<GameObject>();

    public bool isDecreasing{get; set;}
    public bool isIncreasing{get; set;}

    public float maxRopeSegmentLength = 1.0f;
    public float ropeSpeed = 4.0f;

    LineRenderer lineRenderer;

    void Start(){
        lineRenderer = GetComponent<LineRenderer>();
        ResetLength();
    }

    public void ResetLength(){
        foreach(GameObject segment in ropeSegments){
            Destroy(segment);
        }

        ropeSegments = new List<GameObject>();
        isIncreasing = false;
        isDecreasing = false;

        CreateRopeSegment();
    }

    void CreateRopeSegment(){
        GameObject segment = (GameObject)Instantiate(ropeSegmentPrefab, this.transform.position, Quaternion.identity);
        segment.transform.SetParent(this.transform, true);
        Rigidbody2D segmentBody = segment.GetComponent<Rigidbody2D>();
        SpringJoint2D segmentJoint = segment.GetComponent<SpringJoint2D>();

        if(segmentBody == null || segmentJoint == null){
            Debug.LogError("Rope segment body prefab has no Rigidbody2D and/or SpringJoint2D!");
        }

        ropeSegments.Insert(0, segment);

        if(ropeSegments.Count == 1){
            SpringJoint2D connectedObjectJoint = connectedObject.GetComponent<SpringJoint2D>();
            connectedObjectJoint.connectedBody = segmentBody;
            connectedObjectJoint.distance = 0.1f;

            segmentJoint.distance = maxRopeSegmentLength;
        }
        else{
            GameObject nextSegment = ropeSegments[1];
            SpringJoint2D nextSegmentJoint = nextSegment.GetComponent<SpringJoint2D>();

            nextSegmentJoint.connectedBody = segmentBody;
            segmentJoint.distance = 0.0f;
        }

        segmentJoint.connectedBody = this.GetComponent<Rigidbody2D>();
    }

    void RemoveRopeSegment(){
        if(ropeSegments.Count < 3){
            return;
        }

        GameObject topSegment = ropeSegments[0];
        GameObject nextSegment = ropeSegments[1];

        SpringJoint2D nextSegmentJoint = nextSegment.GetComponent<SpringJoint2D>();
        nextSegmentJoint.connectedBody = this.GetComponent<Rigidbody2D>();

        ropeSegments.RemoveAt(0);
        Destroy(topSegment);
    }

    void Update(){
        GameObject topSegment = ropeSegments[0];
        SpringJoint2D topSegmentJoint = topSegment.GetComponent<SpringJoint2D>();

        if(isIncreasing){
            if(topSegmentJoint.distance >= maxRopeSegmentLength){
                CreateRopeSegment();
            }
            else{
                topSegmentJoint.distance += ropeSpeed * Time.deltaTime;
            }
        }

        if(isDecreasing){
            if(topSegmentJoint.distance <= 0.005f){
                RemoveRopeSegment();
            }
            else{
                topSegmentJoint.distance -= ropeSpeed * Time.deltaTime;
            }
        }

        if(lineRenderer != null){
            lineRenderer.positionCount = ropeSegments.Count + 2;
            lineRenderer.SetPosition(0, this.transform.position);

            for(int i = 0; i < ropeSegments.Count; i++){
                lineRenderer.SetPosition(i + 1, ropeSegments[i].transform.position);
            }

            SpringJoint2D connectedObjectJoint = connectedObject.GetComponent<SpringJoint2D>();
            lineRenderer.SetPosition(ropeSegments.Count + 1, connectedObject.transform.TransformPoint(connectedObjectJoint.anchor));
        }
    }
}
