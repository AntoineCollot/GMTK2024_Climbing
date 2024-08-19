using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabGizmos : MonoBehaviour
{
    [SerializeField] float animTime;
    SpriteRenderer spriteRend;
    float baseAlpha;

    public void DisplayMissedGrab(Vector3 pos, float grabRange)
    {
        if(spriteRend == null)
        {
            spriteRend = GetComponentInChildren<SpriteRenderer>();
            baseAlpha = spriteRend.color.a;
        }

        gameObject.SetActive(true);
        transform.position = pos;
        spriteRend.transform.localScale = Vector3.one * grabRange;
        StopAllCoroutines();
        StartCoroutine(Anim());
    }

    IEnumerator Anim()
    {
        float t = 0;
        Color c = spriteRend.color;
        while (t<1)
        {
            t += Time.deltaTime / animTime;

            c.a = Curves.QuadEaseIn(baseAlpha, 0, Mathf.Clamp01(t));
            spriteRend.color = c;

            yield return null;
        }
        gameObject.SetActive(false);
    }
}
