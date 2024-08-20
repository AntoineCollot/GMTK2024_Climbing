using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roof : MonoBehaviour
{
    TowerBuilder builder;
    [SerializeField] ClimbingHold finishHold;
    [SerializeField] GameObject buildCanvas;
    [SerializeField] GameObject buildFX;
    List<TowerSegment> nextPossibilities;
    List<TowerSegment> previewSegments;
    int segmentID = 0;
    bool endHasBeenTriggered;
    bool buildHasBeenSelected;

    [Header("Camera")]
    [SerializeField] GameObject buildCamera;
    [SerializeField] Transform gameCameraTarget;
    [SerializeField] Transform buildCameraTarget;

    // Start is called before the first frame update
    void Start()
    {
        builder = FindObjectOfType<TowerBuilder>();
        GameManager.Instance.onLoopToStart.AddListener(ResetToStart);
    }

    private void ResetToStart()
    {
        endHasBeenTriggered = false;
        buildHasBeenSelected = false;
        buildCanvas.SetActive(false);
        buildCamera.SetActive(false);
        buildFX.SetActive(false);
    }

    void LateUpdate()
    {
        if (finishHold.isGrabbed)
        {
            ReachedEnd();
        }
        if (!GameManager.Instance.isInCutscene)
            transform.position = Tower.GetTowerCenter(Tower.FLOOR_HEIGHT * builder.totalFloors);
    }

    void ReachedEnd()
    {
        if (endHasBeenTriggered)
            return;
        endHasBeenTriggered = true;
        segmentID = 0;
        nextPossibilities = builder.GetSegmentPossibilities(builder.currentBiome, builder.totalFloors, builder.floorsInCurrentBiome);
        SpawnPreviewSegments(nextPossibilities);
        buildHasBeenSelected = false;
        GameManager.Instance.isInCutscene = true;

        Invoke("SpawnPreviewDelayed", 1);
    }

    void SpawnPreviewDelayed()
    {
        PreviewNextSegment();
        buildCanvas.SetActive(true);
        buildCameraTarget.position = gameCameraTarget.position + Vector3.up;
        buildCamera.SetActive(true);
        GameManager.Instance.player.GetComponent<PlayerClimbingHoldGrabber>().ReleaseHold();
        GameManager.Instance.player.gameObject.SetActive(false);
    }

    void SpawnPreviewSegments(List<TowerSegment> segments)
    {
        previewSegments = new();

        for (int i = 0; i < segments.Count; i++)
        {
            //Instantiate
            TowerSegment newSegment = Instantiate(segments[i], null);
            newSegment.transform.position = Tower.GetTowerCenter(builder.totalFloors * Tower.FLOOR_HEIGHT);
            newSegment.transform.localRotation = Quaternion.Euler(0, builder.nextFloorRotation, 0);
            previewSegments.Add(newSegment);
            newSegment.gameObject.SetActive(false);
        }
    }

    public void PreviewNextSegment()
    {
        segmentID++;
        PreviewSegment(segmentID);
    }

    public void PreviewSegment(int id)
    {
        transform.position = Tower.GetTowerCenter(Tower.FLOOR_HEIGHT * (builder.totalFloors + 1));
        id %= previewSegments.Count;
        for (int i = 0; i < previewSegments.Count; i++)
        {
            previewSegments[i].gameObject.SetActive(i == id);
        }
    }

    public void SelectCurrentPreviewed()
    {
        if (buildHasBeenSelected)
            return;
        buildHasBeenSelected = true;
        builder.SpawnSegment(nextPossibilities[segmentID % nextPossibilities.Count]);
        nextPossibilities.Clear();
        buildCanvas.SetActive(false);

        StartCoroutine(BuildAnim());
    }

    IEnumerator BuildAnim()
    {
        buildFX.SetActive(true);

        yield return new WaitForSeconds(1);

        GameManager.Instance.LoopToStart();
    }

    void DeleteAllPreviews()
    {
        for (int i = 0; i < previewSegments.Count; i++)
        {
            Destroy(previewSegments[i].gameObject);
        }
        previewSegments.Clear();
    }
}
