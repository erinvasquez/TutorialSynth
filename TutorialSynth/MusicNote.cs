using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutorialSynth {

    /// <summary>
    /// Represents a musical note with a letter name
    /// and octave number.
    /// "A set of all pitches that are a whole number of octaves apart"
    /// </summary>
    class MusicNote {

        /// <summary>
        /// Our musical note's letter name, for example
        /// "DSharp" or "F". Tracking notes using sharps will be easier
        /// than using flats. Music Theory stuff which can be further elaborated on.
        /// </summary>
        public SharpNotes noteName;

        /// <summary>
        /// Current octave for this note
        /// </summary>
        public int octave;

        /// <summary>
        /// The Equal Temperament Piano frequency represented by 
        /// this musical note with the given note name and octave
        /// </summary>
        public double equalTemperamentFrequency;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="octaveNumber"></param>
        public MusicNote(SharpNotes name, int octaveNumber) {
            noteName = name;
            octave = octaveNumber;

            equalTemperamentFrequency = GetETFrequency();
        }

        public MusicNote GetHalfStepUp() {
            int note = (int)noteName;
            int oct = octave;

            // If this note is SharpNotes.GSHARP, loop back to A in the next octabe
            if (note == (int)SharpNotes.GSHARP) {

                note = (int)SharpNotes.A;
                oct++;
            } else {
                // Just up our note and keep the octave
                note++;
            }

            return new MusicNote((SharpNotes)note, oct);
        }

        /// <summary>
        /// Get the music note 1 whole step,
        /// or 2 Half steps (semitones) up
        /// </summary>
        /// <returns></returns>
        public MusicNote GetWholeStepUp() {
            int note = (int)noteName;
            int oct = octave;

            // If this note is SharpNotes.GSHARP, up the octave
            if (noteName == SharpNotes.GSHARP) {
                note = (int)SharpNotes.ASHARP;
                oct++;
            } else if (noteName == SharpNotes.G) {
                // G too, since we move two half steps
                note = (int)SharpNotes.A;
                oct++;

            } else {
                // upping this note by 2 wont affect octaves of notes under G and GSharp
                note += 2;
            }

            return new MusicNote((SharpNotes)note, oct);
        }

        /// <summary>
        /// Gets equal temprament frequency for this pitch
        /// with an octave of 0
        /// </summary>
        /// <param name="steps">Number of half steps away from A4</param>
        /// <returns></returns>
        public double GetETFrequency() {
            double aForForty = 440.0;

            double a = Math.Pow(2.0, (1.0 / 12.0));


            return (float)aForForty * Math.Pow(a, (float)GetHalfStepsFromA4());
        }

        /// <summary>
        /// Get the MIDI note number
        /// </summary>
        /// <returns></returns>
        public double GetP() {
            return 9.0 + (12.0 * Math.Log(equalTemperamentFrequency, 2.0) / 440.0);
        }

        /// <summary>
        /// Get Half Steps from A4 in order to calculate Equal Temperament Frequency (piano freq to play)
        /// given the note's name and octave
        /// </summary>
        /// <returns></returns>
        public HalfStepsFromA4 GetHalfStepsFromA4() {

            return (HalfStepsFromA4)System.Enum.Parse(typeof(HalfStepsFromA4), (noteName.ToString().ToUpper() + octave.ToString()));
        }

        public string GetName() {
            return noteName.ToString();
        }

        public PianoKeys GetPianoKey() {
            return (PianoKeys)Enum.Parse(typeof(PianoKeys), (noteName.ToString().ToUpper() + octave.ToString()));
        }

        public bool NoteIsHigherThan(MusicNote _musicNote) {

            // if our octave is greater, the frequency is greater
            if (octave > _musicNote.octave) {
                return true;
            } else if (octave == _musicNote.octave) {
                // else if the octave is the same,
                // we're only greater if our note name is

                if (noteName > _musicNote.noteName) {
                    return true;
                }

            }

            // If our octave is less than the observed one,
            // we're definitely not greater
            return false;
        }

        public override string ToString() {
            return noteName.ToString();
        }

    }
}
