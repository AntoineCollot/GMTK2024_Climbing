using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayVideoToUI : MonoBehaviour
{
    RenderTexture tempRenderTex;
    public RawImage targetUi;
    VideoPlayer player;
    const float PLAY_VIDEO_DELAY = 1;

    // Start is called before the first frame update
    void Start()
    {
       tempRenderTex = RenderTexture.GetTemporary(512, 100,0,RenderTextureFormat.ARGB32);
        player = GetComponent<VideoPlayer>();
        player.renderMode = VideoRenderMode.RenderTexture;
        player.targetTexture = tempRenderTex;
        player.url = System.IO.Path.Combine(Application.streamingAssetsPath,"Logo","LogoYorshInGame_WhiteOnBlack_1024.mp4");
        targetUi.texture = tempRenderTex;
        Invoke("PlayVideo", PLAY_VIDEO_DELAY);

    }

    public void PlayVideo()
    {
        player.Play();
    }


    private void OnDestroy()
    {
       RenderTexture.ReleaseTemporary(tempRenderTex);
    }
}
