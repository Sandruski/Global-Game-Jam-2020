﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehaviour : MonoBehaviour
{
    #region PUBLIC_VARIABLES
    public bool TouchGround
    {
        get { return touchGround; }
    }
    public bool Invisible
    {
        get { return invisible; }
    }

    public uint height;

    public float spawnProbability;
    public float redProbability;

    public Mesh mesh1;
    public Mesh mesh2;
    public Mesh mesh3;
    public Material material1;
    public Material material2;
    public Material material3;

    public ScrewdriverController screwdriverController;

    [HideInInspector]
    public Vector3 size;
    #endregion

    #region PRIVATE_VARIABLES
    private bool touchGround = false;
    private bool invisible = false;
    public List<GameObject> holes;
    #endregion

    void Start()
    {
        holes = new List<GameObject>();
        RecalculateSize();
        SpawnHoles();
        AddRigidbodyToHoles();
    }

    public void RecalculateSize()
    {
        size = Vector3.Scale(transform.localScale, GetComponent<MeshFilter>().mesh.bounds.size);
    }

    public void SpawnHoles()
    {
        touchGround = false;
        invisible = false;

        bool hasSpawned = false;

        for (uint i = 0; i < height; ++i)
        {
            for (uint j = 0; j < 4; ++j)
            {
                if (Random.value <= spawnProbability
                    || (i == height - 1 && j == 3 && !hasSpawned))
                {
                    float y = 0.0f;
                    switch (i)
                    {
                        case 0:
                            y = screwdriverController.heightDistance / 2.0f;
                            break;
                        case 1:
                            y = screwdriverController.heightDistance / 2.0f + screwdriverController.heightDistance;
                            break;
                        case 2:
                            y = screwdriverController.heightDistance / 2.0f + 2.0f * screwdriverController.heightDistance;
                            break;
                    }

                    float halfWidthDistance = size.x / 2.0f;
                    float x = 0.0f;
                    float z = 0.0f;

                    Quaternion spawnRotation = Quaternion.identity;

                    switch (j)
                    {
                        case 0:
                            x = halfWidthDistance; // right
                            spawnRotation = Quaternion.AngleAxis(180.0f, Vector3.up);
                            break;
                        case 1:
                            x = -halfWidthDistance; // left
                            break;
                        case 2:
                            z = halfWidthDistance; // back
                            spawnRotation = Quaternion.AngleAxis(90.0f, Vector3.up);
                            break;
                        case 3:
                            z = -halfWidthDistance; // forward
                            break;
                    }

                    if (z == -halfWidthDistance)
                    {
                        if ((i == height - 1 && j == 3 && !hasSpawned))
                        {
                            z *= -1.0f;
                            spawnRotation = Quaternion.AngleAxis(90.0f, Vector3.up);
                        }
                        else
                        {
                            // Not forward!
                            continue;
                        }
                    }

                    hasSpawned = true;

                    Vector3 spawnPosition = transform.position - new Vector3(0.0f, size.y / 2.0f, 0.0f) + new Vector3(x, y, z);
                    GameObject hole = null;
                    if (Random.value <= redProbability)
                    {
                        hole = Instantiate(Resources.Load("RedHole") as GameObject, spawnPosition, spawnRotation, transform);
                    }
                    else
                    {
                        hole = Instantiate(Resources.Load("BlueHole") as GameObject, spawnPosition, spawnRotation, transform);
                    }

                    holes.Add(hole);
                }
            }
        }
    }

    public void RemoveHole(GameObject hole)
    {
        holes.Remove(hole);
        Destroy(hole);
    }

    public void RemoveHoles()
    {
        foreach(GameObject hole in holes)
        {
            Destroy(hole);
        }

        holes.Clear();
    }

    void AddRigidbodyToHoles()
    {
        foreach (GameObject hole in holes)
        {
            if (hole.GetComponent<Rigidbody>() == null)
            {
                hole.AddComponent<Rigidbody>();
            }
            hole.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public bool AreAllHolesRemoved()
    {
        return holes.Count == 0;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "FakeFloor")
        {
            touchGround = true;
            AddRigidbodyToHoles();
        }
    }

    void OnBecameInvisible()
    {
        invisible = true;   
    }
}
