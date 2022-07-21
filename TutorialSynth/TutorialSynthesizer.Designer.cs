
namespace TutorialSynth {
    partial class TutorialSynthesizer {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.oscillator1 = new TutorialSynth.Oscillator();
            this.SuspendLayout();
            // 
            // oscillator1
            // 
            this.oscillator1.Location = new System.Drawing.Point(93, 77);
            this.oscillator1.Name = "oscillator1";
            this.oscillator1.Size = new System.Drawing.Size(200, 100);
            this.oscillator1.TabIndex = 0;
            this.oscillator1.TabStop = false;
            // 
            // TutorialSynthesizer
            // 
            this.ClientSize = new System.Drawing.Size(737, 456);
            this.Controls.Add(this.oscillator1);
            this.KeyPreview = true;
            this.Name = "TutorialSynthesizer";
            this.Text = "TutorialSynthesizer";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TutorialSynthesizer_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion
        private Oscillator oscillator1;
    }
}

