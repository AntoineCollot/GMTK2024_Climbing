using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartClimb : MonoBehaviour
{
    [SerializeField] Transform playerInitPosition;
    [SerializeField] ClimbingHold firstHold;
    PlayerClimbingHoldGrabber holdGrabber;
    PlayerAnims anims;

    // Start is called before the first frame update
    void Start()
    {
        holdGrabber = GameManager.Instance.player.GetComponent<PlayerClimbingHoldGrabber>();

        GameManager.Instance.onGameStart.AddListener(StartClimbing);
        GameManager.Instance.onLoopToStart.AddListener(StartClimbing);
    }

    public void StartClimbing()
    {
        StartCoroutine(StartClimbingAnim());
    }

    IEnumerator StartClimbingAnim()
    {
        GameManager.Instance.isInCutscene = true;
        holdGrabber.gameObject.SetActive(true);
        //Place player
        holdGrabber.transform.position = playerInitPosition.position;
        holdGrabber.transform.rotation = playerInitPosition.rotation;
        holdGrabber.GetComponent<Rigidbody>().velocity = Vector3.zero;

        yield return new WaitForSeconds(2);

        holdGrabber.GetComponentInChildren<Animator>().SetTrigger("PickUpRock");

        //Pick up rock anim
        yield return new WaitForSeconds(2);

        GameManager.Instance.isInCutscene = false;
        holdGrabber.GrabHold(firstHold);
    }
}
