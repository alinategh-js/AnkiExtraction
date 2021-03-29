using AnkiSharp;
using System;

namespace AnkiSharpTest
{
    public static class ApkgCreation
    {
        public static void CreateApkg()
        {
            Anki testAnkiShit = new Anki("testAnkiShit");

            testAnkiShit.AddItem("Hello", "Bonjour");
            testAnkiShit.AddItem("How are you ?", "Comment ca va ?");
            testAnkiShit.AddItem("Flower", "fleur");
            testAnkiShit.AddItem("House", "Maison");

            testAnkiShit.CreateApkgFile(@"C:\Users\ASUS\Desktop\");
        }
    }
}
