using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace TutorialSynth {
    class Oscillator : GroupBox {

        public Oscillator() {

            this.Controls.Add(new Button() {
                Name = "Sine",
                Location = new System.Drawing.Point(10, 15),
                Text = "Sine",
                BackColor = Color.Yellow
            });

            this.Controls.Add(new Button() {
                Name = "Square",
                Location = new System.Drawing.Point(65, 15),
                Text = "Square"
            });

            this.Controls.Add(new Button() {
                Name = "Saw",
                Location = new System.Drawing.Point(120, 15),
                Text = "Saw"
            });

            this.Controls.Add(new Button() {
                Name = "Triangle",
                Location = new System.Drawing.Point(10, 50),
                Text = "Triangle"
            });

            this.Controls.Add(new Button() {
                Name = "Noise",
                Location = new System.Drawing.Point(65, 50),
                Text = "Noise"
            });

            foreach(Control control in this.Controls) {
                control.Size = new Size(50, 30);
                control.Font = new Font("Microsoft Sans Serif", 6.75f);
                control.Click += WaveButton_Click;
            }

        }

        public WaveForm Waveform { get; private set; }

        /// <summary>
        /// Event handler for click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WaveButton_Click(object sender, EventArgs e) {

            Button button = (Button)sender; // "cast here instead of the rest of the function"
            this.Waveform = (WaveForm)Enum.Parse(typeof(WaveForm), button.Text);



            //MessageBox.Show($"The button you pressed was {this.Waveform}");


            foreach (Button otherButtons in this.Controls.OfType<Button>()) {
                otherButtons.UseVisualStyleBackColor = true;
            }

            button.BackColor = Color.Yellow;



        }

    }

}
