using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;
using System.Collections;
using System.Diagnostics;

namespace WindowsGame4
{
    class MusicManager
    {

        ArrayList songs;

        public MusicManager(ArrayList _songs)
        {
            MediaPlayer.IsRepeating = true;

            songs = _songs;
        }

        public void Play(int i)
        {
            Debug.Assert(i < songs.Count);

            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
            }

            MediaPlayer.Play((Song)songs[i]);
        }

    }
}
