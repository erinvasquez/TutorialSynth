using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TutorialSynth {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TutorialSynthesizer());



            //initialize all piano keys?
            for (int a = 0; a < 88; a++) {
                Console.WriteLine(a + "");
            }

        }

    }
}
