using System;

namespace NCG_Full
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("========================================================");
            Console.WriteLine("                       NCG Full");
            Console.WriteLine("========================================================\n\n");

            while(true)
            {
                NCG_SongGetter.SongGetter();

                NCG_VideoEditor.VideoEditor();

                NCG_Uploader.Uploader();
            }
        }
    }
}