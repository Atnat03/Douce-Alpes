using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Audio SO", menuName = "Audio SO")]
public class AudioSO : ScriptableObject
{
    public List<AudioClip> audioClips = new List<AudioClip>();
}