using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    //单例
    public static AudioManager Instance;
    //背景音乐播放器
    private AudioSource bgmPlayer;
    //音效播放器
    private AudioSource sePlayer;

    private void Awake()
    {
        //单例
        Instance = this;
        //添加一个音乐播放器组件
        bgmPlayer = gameObject.AddComponent<AudioSource>();
        //设置音乐循环播放
        bgmPlayer.loop = true;
        //添加一个音效播放器组件
        sePlayer = gameObject.AddComponent<AudioSource>();
    }


    //播放音乐，将参数填写为“项目”面板的Resources文件夹中的音乐文件路径
    public void PlayBgm(string path)
    {
        //如果当前没有音乐播放
        if(bgmPlayer.isPlaying == false)
        {
            //从Resources文件夹中读取一个音频文件
            AudioClip clip = Resources.Load<AudioClip>(path);
            //设置播放器的音频片段
            bgmPlayer.clip= clip;
            //播放
            bgmPlayer.Play();
        }
    }

    //停止播放音乐
    public void StopBgm(string path)
    {
        //如果音乐正在播放
        if(bgmPlayer.isPlaying == true)
        {
            //停止播放
            bgmPlayer.Stop();
        }
    }

    //播放音效
    public void PlaySe(string path)
    {
        //从Resources文件夹中读取一个音频文件
        AudioClip clip = Resources.Load<AudioClip>(path);
        //播放音效
        sePlayer.PlayOneShot(clip);
    }

}
