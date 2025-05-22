namespace CompoundV24
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Exiled.API.Features;
    using Exiled.API.Features.Toys;
    using UnityEngine;
    using Speaker = Speaker;

    internal static class SoundHelper
    {
        public static Dictionary<string, List<string>> RegisteredNoiseLookupTable { get; set; } = new();

        internal static void RegisterSounds()
        {
            foreach (KeyValuePair<string, List<string>> kvp in Plugin.Config.NameToPathForSounds)
            {
                foreach (string path in kvp.Value)
                {
                    string newPath = path;
                    if (path.Contains("{defaultpath}"))
                    {
                        string[] pathsSplit = path.Split(new string[] { "{defaultpath}" }, StringSplitOptions.RemoveEmptyEntries);
                        Log.Info($"{pathsSplit[0]}");
                        Log.Info($"{Paths.Plugins}");
                        Log.Info(Path.Combine(Paths.Plugins, pathsSplit[0]));
                        Log.Info(Path.Combine(Paths.Plugins, pathsSplit[0]));
                        newPath = Path.Combine(Paths.Plugins, pathsSplit[0]);
                    }

                    AudioClipStorage.LoadClip(newPath, newPath);
                    Log.Debug($"registered {newPath} under {kvp.Key}");
                }

                RegisteredNoiseLookupTable.Add(kvp.Key, kvp.Value);
            }
        }

        private static int AudioPlayers { get; set; } = 0;

        /// <summary>
        /// Plays a sound in the specified position.
        /// </summary>
        /// <param name="pos">The position to play the sound at.</param>
        /// <param name="sound">The name of the sound to play.</param>
        /// <param name="audioPlayer">The audioplayer playing the noise.</param>
        /// <param name="speaker">The speaker playing the noise.</param>
        /// <param name="loop">Whether to loop the noise.</param>
        /// <param name="spatial">Whether to make the sound spatial.</param>
        internal static void PlaySound(Vector3 pos, string sound, out AudioPlayer audioPlayer, out Speaker speaker, bool loop = false, bool spatial = true, float minDistance = 5, float maxDistance = 5f)
        {
            AudioPlayers++;
            audioPlayer = AudioPlayer.CreateOrGet($"icedchqi_v24sfx_{AudioPlayers}", destroyWhenAllClipsPlayed: true);
            speaker = audioPlayer.AddSpeaker("main", position: pos, isSpatial: spatial, minDistance: minDistance, maxDistance: maxDistance);
            audioPlayer.AddClip(RegisteredNoiseLookupTable[sound] is null ? sound : RegisteredNoiseLookupTable[sound].RandomItem(), loop: loop);
        }

        internal static void PlaySound(string sound, bool loop = false) => PlaySound(Vector3.zero, sound, out AudioPlayer _, out Speaker _, loop, false, 5, 50000);

        internal static void PlaySound(Vector3 pos, string sound) => PlaySound(pos, sound, out AudioPlayer _, out Speaker _);

        internal static void PlaySound(Player player, string sound, bool loop = false)
        {
            PlaySound(player.Position, sound, out AudioPlayer _, out Speaker speaker, loop: loop);
            speaker.transform.parent = player.Transform;
        }
    }
}
