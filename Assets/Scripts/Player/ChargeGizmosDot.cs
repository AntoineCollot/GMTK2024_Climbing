using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeGizmosDot : MonoBehaviour
{
    int id;
    bool isVisible = true;

    SpriteRenderer spriteRend;
    Material mat;
    Transform camT;

    const float MIN_SIZE = 0.013f;
    const float MAX_SIZE = 0.04f;

    private void Update()
    {
        transform.LookAt(camT.position);
    }

    public void Init(int id)
    {
        this.id = id;
        spriteRend = GetComponent<SpriteRenderer>();
        mat = spriteRend.material;
        camT = Camera.main.transform;
    }

    public void UpdateDot(in Vector3 position, int totalDots)
    {
        transform.position = position;
        float progress = Mathf.Clamp01((float)(id + 1) / totalDots);
        spriteRend.color = Color.Lerp(Color.yellow, Color.red, progress);

        transform.localScale = Vector3.one * Curves.QuadEaseOut(MIN_SIZE, MAX_SIZE, progress);
    }

    public void SetVisible(bool value)
    {
        if (value == isVisible)
            return;

        isVisible = value;
        gameObject.SetActive(value);

        if(isVisible)
        {
            StopAllCoroutines();
            StartCoroutine(ShowAnim());
        }
    }

    public void Hide()
    {
        if (isVisible)
        {
            StopAllCoroutines();
            StartCoroutine(HideAnim());
        }
    }

    IEnumerator ShowAnim()
    {
        float t = 0;
        while (isVisible && t < 1)
        {
            t += Time.deltaTime / 0.25f;

            mat.SetFloat("_CircleRadius", Curves.Berp(0, 0.8f, Mathf.Clamp01(t)));

            yield return null;
        }

    }

    IEnumerator HideAnim()
    {
        yield return new WaitForSeconds(id * 0.025f);

        float t = 0;
        while (isVisible && t < 1)
        {
            t += Time.deltaTime / 0.1f;

            mat.SetFloat("_CircleRadius", Curves.QuadEaseIn(0.8f, 0, Mathf.Clamp01(t)));

            yield return null;
        }

        SetVisible(false);
    }
}
