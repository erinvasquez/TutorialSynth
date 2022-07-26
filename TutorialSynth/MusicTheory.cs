﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutorialSynth {

    /*
     * TO DO:
     * Get Comments on everything
     * Get Regions tidied up
     * 
     * Run some tests and output the results for all possible use cases
     * 
     * Fix GetETFrequencyFromPianoKey
     * 
     * 
     */

    /// <summary>
    /// A custom music class for music theory calculations
    /// used in my Oscillators and Synthesizers for Audio Programming
    /// </summary>
    class MusicTheory {

        

        #region Get/Set
        /// <summary>
        /// Move this to a music note class? It comes from it originially
        /// Get Half Steps from A4 in order to calculate Equal Temperament Frequency (piano freq to play)
        /// given the note's name and octave
        /// </summary>
        /// <param name="_noteName"></param>
        /// <param name="octave"></param>
        /// <returns></returns>
        public HalfStepsFromA4 GetHalfStepsFromA4(SharpNotes _noteName, int octave) {

            return (HalfStepsFromA4)System.Enum.Parse(typeof(HalfStepsFromA4), (_noteName.ToString().ToUpper() + octave.ToString()));
        }

        public MusicNote GetHalfStepUp(MusicNote firstNote) {
            int newNote = (int)firstNote.noteName;
            int newOct = firstNote.octave;

            // If this note is SharpNotes.GSHARP, loop back to A in the next octabe
            if (newNote == (int)SharpNotes.GSHARP) {

                newNote = (int)SharpNotes.A;
                newOct++;
            } else {
                // Just up our note and keep the octave
                newNote++;
            }

            return new MusicNote((SharpNotes)newNote, newOct);
        }

        /// <summary>
        /// Get the music note 1 whole step,
        /// or 2 Half steps (semitones) up
        /// </summary>
        /// <returns></returns>
        public MusicNote GetWholeStepUp(MusicNote firstNote) {
            int note = (int)firstNote.noteName;
            int oct = firstNote.octave;

            // If this note is SharpNotes.GSHARP, up the octave
            if (firstNote.noteName == SharpNotes.GSHARP) {
                note = (int)SharpNotes.ASHARP;
                oct++;
            } else if (firstNote.noteName == SharpNotes.G) {
                // G too, since we move two half steps
                note = (int)SharpNotes.A;
                oct++;

            } else {
                // upping this note by 2 wont affect octaves of notes under G and GSharp
                note += 2;
            }

            // *** NOTE FOR LATER ****
            // if we make a new reference, we use up more memory
            // should we just initialize all our possible notes in the beginning to save on processing later?
            // Lets try it out later
            return new MusicNote((SharpNotes)note, oct);
        }

        /// <summary>
        /// Get the MIDI note number
        /// </summary>
        /// <returns></returns>
        public double GetP(double eTFreq) {
            return 9.0 + (12.0 * Math.Log(eTFreq, 2.0) / 440.0);
        }

        #endregion

        #region Equal Temperament Frequency Controls

        /// <summary>
        /// Get Piano Equal Temperament Frequency given a note's name and octave
        /// number
        /// </summary>
        /// <param name="_noteName"></param>
        /// <param name="octave"></param>
        /// <returns></returns>
        public double GetETFrequency(SharpNotes _noteName, int octave) {
            double aForForty = 440.0;

            double a = Math.Pow((double)2, (double)(1 / 12));

            return aForForty * Math.Pow(a, (double) GetHalfStepsFromA4(_noteName, octave));

        }

        /// <summary>
        /// OK THIS NEEDS SOME DOUBLE CHECKING THAT LAST LINE SHOULDNT BE (double) key, it should be half steps from A4
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static double GetETFrequencyFromPianoKey(PianoKeys key) {
            double aForForty = 440.0;

            double a = Math.Pow((double) 2, (double) (1 / 12));

            // A4 in Piano keys is 48
            int myKey = (int) PianoKeys.A4;
            // We want to convert this into HalfStepsFromA4
            // Get how many half steps from A4 our key is
            // So Get our key in piano keys and subtract that
            // value from 49 (our PianoKeys.A4)


            return aForForty * Math.Pow(a, (double)key);

        }

        #endregion

        #region Volume Control
        // Referenced from https://www.youtube.com/watch?v=Vjm--AqG04Y&ab_channel=GDC
        // at 9:43 seconds
        /// <summary>
        /// Returns a float volume level
        /// </summary>
        /// <param name="volume"></param>
        /// <returns>A float volume level from a float dB value</returns>
        public float VolumeToDb(float volume) {

            // originally uses Math.LogF in the algorithm, we cast to float here
            // instead of receiving a native float for ""Math.Log10(volume)"
            return 10.0f * (float) Math.Log10(volume);
        }
        
        /// <summary>
        /// Returns a double volume level
        /// </summary>
        /// <param name="volume"></param>
        /// <returns>A double volume level from a double dB value</returns>
        public double VolumeToDb(double volume) {

            return 10.0 * Math.Log10(volume);
        }

        /// <summary>
        /// Returns a float dB level
        /// </summary>
        /// <param name="dB"></param>
        /// <returns>A float dB level from a float volume value</returns>
        public float DbToVolume(float dB) {

            return (float) Math.Pow(10f, 0.05f * dB);
        }

        /// <summary>
        /// Returns a double dB level
        /// </summary>
        /// <param name="dB"></param>
        /// <returns>A double volume level from our dB value</returns>
        public double DbToVolume(double dB) {

            return Math.Pow(10.0, 0.05 * dB);
        }

        #endregion

    }

    /// <summary>
    /// 12 Note chromatic scale starting
    /// from A, written in sharps
    /// </summary>
    public enum SharpNotes {
        A,
        ASHARP,
        B,
        C,
        CSHARP,
        D,
        DSHARP,
        E,
        F,
        FSHARP,
        G,
        GSHARP
    };

    /// <summary>
    /// 12 Note chromatic scale starting
    /// from A, written in sharps
    /// BUT NOW IN BITS
    /// A being the left most bit,
    /// and GSHARP the right most
    /// </summary>
    [Flags]
    public enum SharpNoteBits {
        A = 1 << 11,
        ASHARP = 1 << 10,
        B = 1 << 9,
        C = 1 << 8,
        CSHARP = 1 << 7,
        D = 1 << 6,
        DSHARP = 1 << 5,
        E = 1 << 4,
        F = 1 << 3,
        FSHARP = 1 << 2,
        G = 1 << 1,
        GSHARP = 1 << 0
    };

    /// <summary>
    /// Enum indicating how many half steps from A4 the key is
    /// </summary>
    public enum HalfStepsFromA4 {
        A0 = -48,
        ASHARP0 = -47,
        B0 = -46,
        C1 = -45,
        CSHARP1 = -44,
        D1 = -43,
        DSHARP1 = -42,
        E1 = -41,
        F1 = -40,
        FSHARP1 = -39,
        G1 = -38,
        GSHARP1 = -37,
        A1 = -36,
        ASHARP1 = -35,
        B1 = -34,
        C2 = -33,
        CSHARP2 = -32,
        D2 = -31,
        DSHARP2 = -30,
        E2 = -29,
        F2 = -28,
        FSHARP2 = -27,
        G2 = -26,
        GSHARP2 = -25,
        A2 = -24,
        ASHARP2 = -23,
        B2 = -22,
        C3 = -21,
        CSHARP3 = -20,
        D3 = -19,
        DSHARP3 = -18,
        E3 = -17,
        F3 = -16,
        FSHARP3 = -15,
        G3 = -14,
        GSHARP3 = -13,
        A3 = -12,
        ASHARP3 = -11,
        B3 = -10,
        C4 = -9,
        CSHARP4 = -8,
        D4 = -7,
        DSHARP4 = -6,
        E4 = -5,
        F4 = -4,
        FSHARP4 = -3,
        G4 = -2,
        GSHARP4 = -1,
        A4 = 0,
        ASHARP4,
        B4,
        C5,
        CSHARP5,
        D5,
        DSHARP5,
        E5,
        F5,
        FSHARP5,
        G5,
        GSHARP5,
        A5,
        ASHARP5,
        B5,
        C6,
        CSHARP6,
        D6,
        DSHARP6,
        E6,
        F6,
        FSHARP6,
        G6,
        GSHARP6,
        A6,
        ASHARP6,
        B6,
        C7,
        CSHARP7,
        D7,
        DSHARP7,
        E7,
        F7,
        FSHARP7,
        G7,
        GSHARP7,
        A7,
        ASHARP7,
        B7,
        C8,
    };

    /// <summary>
    /// 
    /// EDIT FOR JUST ALPHABET
    /// Starting at -46 with Z, based on half steps from
    /// A4 in order to make it easier to calculate
    /// Equal Temperament frequency.
    /// 
    /// Half steps from A4 as a PC Keyboard Enum
    /// </summary>
    public enum KeyboardKeysFromA4 {
        Z = HalfStepsFromA4.A0, X, C, V, B, N, M,
        A, S, D, F, G, H, J, K, L,
        Q, W, E, R, T, Y, U, I, O, P
    };

    /// <summary>
    /// An enum listing piano keys
    /// from A0 to C8,
    /// mapped from 0 to 87 (88 keys)
    /// </summary>
    public enum PianoKeys {
        A0 = 0,
        ASharp0,
        B0,
        C1,
        CSharp1,
        D1,
        DSharp1,
        E1,
        F1,
        FSharp1,
        G1,
        GSharp1,
        A1,
        ASharp1,
        B1,
        C2,
        CSharp2,
        D2,
        DSharp2,
        E2,
        F2,
        FSharp2,
        G2,
        GSharp2,
        A2,
        ASharp2,
        B2,
        C3,
        CSharp3,
        D3,
        DSharp3,
        E3,
        F3,
        FSharp3,
        G3,
        GSharp3,
        A3,
        ASharp3,
        B3,
        C4,
        CSharp4,
        D4,
        DSharp4,
        E4,
        F4,
        FSharp4,
        G4,
        GSharp4,
        A4,
        ASharp4,
        B4,
        C5,
        CSharp5,
        D5,
        DSharp5,
        E5,
        F5,
        FSharp5,
        G5,
        GSharp5,
        A5,
        ASharp5,
        B5,
        C6,
        CSharp6,
        D6,
        DSharp6,
        E6,
        F6,
        FSharp6,
        G6,
        GSharp6,
        A6,
        ASharp6,
        B6,
        C7,
        CSharp7,
        D7,
        DSharp7,
        E7,
        F7,
        FSharp7,
        G7,
        GSharp7,
        A7,
        ASharp7,
        B7,
        C8
    };

}
