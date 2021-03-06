﻿using System;
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
        int currentSong;

        public MusicManager(ArrayList _songs)
        {
            MediaPlayer.IsRepeating = true;
            currentSong = -1;

            songs = _songs;
        }

        public void Play(int i)
        {
            Debug.Assert(i < songs.Count);
            currentSong = i;

            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
            }

            // so we can have music-less levels if we want
            if (i > -1)
            {
                MediaPlayer.Play((Song)songs[i]);
            }
        }

        public void Stop()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
            }
        }

        public bool isStopped
        {
            get { return MediaPlayer.State != MediaState.Playing; }
        }

        public int CurrSong
        {
            get { return currentSong; }
        }

    }
}
