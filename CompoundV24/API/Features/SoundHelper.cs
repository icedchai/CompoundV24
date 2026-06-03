using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace CompoundV24.API.Features;

/// <summary>
/// Manages and plays sounds in groups.
/// </summary>
public class SoundHelper
{
    /// <summary>
    /// Gets a <see cref="Dictionary{TKey, TValue}"/>, corresponding a sound group's name to the sound effects that are members of it.
    /// </summary>
    public static Dictionary<string, List<string>> RegisteredNoiseLookupTable { get; internal set; } = new();

    /// <summary>
    /// Gets a member of the soundgroup specified if the soundgroup exists, otherwise the name of the sound.
    /// </summary>
    /// <param name="sound">The soundgroup.</param>
    /// <returns>A member of the soundgroup of the name <paramref name="sound"/> if it exists, otherwise <paramref name="sound"/>.</returns>
    public static string GetSound(string sound)
    {
        return RegisteredNoiseLookupTable[sound] is null ? sound : RegisteredNoiseLookupTable[sound].RandomItem();
    }

    /// <summary>
    /// Registers a sound group.
    /// </summary>
    /// <param name="soundGroupToRegister">The sound group to register.</param>
    public static void RegisterSoundGroup(Dictionary<string, List<string>> soundGroupToRegister)
    {
        foreach (KeyValuePair<string, List<string>> kvp in soundGroupToRegister)
        {
            foreach (string path in kvp.Value)
            {
                string newPath = path;
                if (path.Contains("{labapi_plugin_folder}"))
                {
                    string[] pathsSplit = path.Split(new string[] { "{labapi_plugin_folder}" }, StringSplitOptions.RemoveEmptyEntries);
                    newPath = Path.Combine(LabApi.Loader.Features.Paths.PathManager.Plugins.FullName, pathsSplit[0]);
                }

                AudioClipStorage.LoadClip(newPath, path);
                Logger.Debug($"registered {newPath} under {kvp.Key}");
            }

            if (!RegisteredNoiseLookupTable.TryGetValue(kvp.Key, out _))
            {
                RegisteredNoiseLookupTable.Add(kvp.Key, kvp.Value);
            }
            else
            {
                Logger.Warn($"Attempted to register a sound group that already exists!");
            }
        }
    }

    /// <summary>
    /// De-registers a sound group.
    /// </summary>
    /// <param name="soundGroupToUnregister">The sound group to de-register.</param>
    /// <returns>true if the sound group was successfully unregistered, otherwise false.</returns>
    public static bool UnregisterSoundGroup(string soundGroupToUnregister)
    {
        return RegisteredNoiseLookupTable.Remove(soundGroupToUnregister);
    }

    /// <summary>
    /// Unregisters a specific noise from a sound group.
    /// </summary>
    /// <param name="soundGroup">The sound group to remove this from.</param>
    /// <param name="soundPath">The sound path to remove.</param>
    /// <returns>true if the sound group was successfully unregistered, otherwise false.</returns>
    public static bool UnregisterNoise(string soundGroup, string soundPath)
    {
        return RegisteredNoiseLookupTable.TryGetValue(soundGroup, out _) && RegisteredNoiseLookupTable[soundGroup].Remove(soundPath);
    }

    private static long AudioPlayers { get; set; } = long.MinValue;

    /// <summary>
    /// Plays a sound effect globally to all players.
    /// </summary>
    /// <param name="sound">The sound group to play.</param>
    /// <param name="audioClipPlayback">The <see cref="AudioClipPlayback"/> of this sound.</param>
    /// <param name="loop">Whether to loop this sound.</param>
    public static void PlaySoundGlobal(string sound, out AudioClipPlayback audioClipPlayback, bool loop = false)
    {
        AudioPlayer audioPlayer = AudioPlayer.CreateOrGet("icedchqi_audioplayer_global");
        Speaker speaker = audioPlayer.GetOrAddSpeaker("main", 1, false, 50000, 50000);
        audioClipPlayback = audioPlayer.AddClip(GetSound(sound), loop: loop);
    }

    /// <summary>
    /// Plays a sound in the specified position.
    /// </summary>
    /// <param name="pos">The position to play the sound at.</param>
    /// <param name="sound">The sound group to play.</param>
    /// <param name="audioPlayer">The <see cref="AudioPlayer"/> responsible for playing this sound.</param>
    /// <param name="speaker">The <see cref="Speaker"/> from which this sound is playing.</param>
    /// <param name="loop">Whether to loop this sound.</param>
    /// <param name="spatial">Whether this sound is spatial.</param>
    /// <param name="minDistance">The minimum distance for full-volume audio.</param>
    /// <param name="maxDistance">The maximum audible distance for the audio.</param>
    public static void PlaySound(Vector3 pos, string sound, out AudioPlayer audioPlayer, out Speaker speaker, bool loop = false, bool spatial = true, float minDistance = 5, float maxDistance = 15f) => PlaySound(pos, sound, out audioPlayer, out speaker, out _, loop, spatial, minDistance, maxDistance);

    /// <summary>
    /// Plays a sound in the specified position.
    /// </summary>
    /// <param name="pos">The position to play the sound at.</param>
    /// <param name="sound">The sound group to play.</param>
    /// <param name="audioPlayer">The <see cref="AudioPlayer"/> responsible for playing this sound.</param>
    /// <param name="speaker">The <see cref="Speaker"/> from which this sound is playing.</param>
    /// <param name="audioClipPlayback"></param>
    /// <param name="loop">Whether to loop this sound.</param>
    /// <param name="spatial">Whether this sound is spatial.</param>
    /// <param name="minDistance">The minimum distance for full-volume audio.</param>
    /// <param name="maxDistance">The maximum audible distance for the audio.</param>
    public static void PlaySound(Vector3 pos, string sound, out AudioPlayer audioPlayer, out Speaker speaker, out AudioClipPlayback audioClipPlayback, bool loop = false, bool spatial = true, float minDistance = 5, float maxDistance = 15f)
    {
        AudioPlayers++;
        audioPlayer = AudioPlayer.CreateOrGet($"icedchqi_audioplayer_{AudioPlayers}", destroyWhenAllClipsPlayed: true);
        speaker = audioPlayer.AddSpeaker("main", position: pos, isSpatial: spatial, minDistance: minDistance, maxDistance: maxDistance);
        audioClipPlayback = audioPlayer.AddClip(GetSound(sound), loop: loop);
    }

    /// <summary>
    /// Plays a sound at (0, 0, 0)
    /// </summary>
    /// <param name="sound">The sound group to play.</param>
    /// <param name="loop">Whether to loop this sound.</param>
    public static void PlaySound(string sound, bool loop = false) => PlaySound(Vector3.zero, sound, out AudioPlayer _, out Speaker _, loop, false, 50000, 50000);

    /// <summary>
    /// Plays a sound in the specified position.
    /// </summary>
    /// <param name="pos">The position to play the sound at.</param>
    /// <param name="sound">The sound group to play.</param>
    public static void PlaySound(Vector3 pos, string sound) => PlaySound(pos, sound, out AudioPlayer _, out Speaker _);

    /// <summary>
    /// Plays a sound in the specified position.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="sound">The sound group to play.</param>
    /// <param name="loop">Whether to loop this sound.</param>
    public static void PlaySound(Transform parent, string sound, bool loop = false) => PlaySound(parent, sound, out _, out _, loop);

    /// <summary>
    /// Plays a sound in the specified position.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="sound">The sound group to play.</param>
    /// <param name="audioPlayer">The <see cref="AudioPlayer"/> responsible for playing this sound.</param>
    /// <param name="speaker">The <see cref="Speaker"/> from which this sound is playing.</param>
    /// <param name="loop">Whether to loop this sound.</param>
    public static void PlaySound(Transform parent, string sound, out AudioPlayer audioPlayer, out Speaker speaker, bool loop = false)
    {
        PlaySound(parent.position, sound, out audioPlayer, out speaker, loop: loop);
        speaker.transform.parent = parent;
    }
}
