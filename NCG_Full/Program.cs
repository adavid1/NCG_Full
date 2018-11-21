using System;
using System.Threading;

namespace NCG_Full
{
    class Program
    {
        public const string basePath = @"C:\Users\Axel David\Documents\Code\NCG\";

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

                NCG_FeedGetter.FeedGetter();


                Console.Write("Sleeping... \n");

                for (int minutes = 9; minutes >= 0; minutes--) // 30 min
                {
                    for (int seconds = 60; seconds >= 1; seconds--)
                    {
                        ConsoleTools.ClearCurrentConsoleLine();
                        if (seconds == 60)
                        {
                            Console.Write("Before restart : {0}:00", (minutes+1).ToString("00"));
                        }
                        else
                        {
                            Console.Write("Before restart : {0}:{1}", minutes.ToString("00"), seconds.ToString("00"));
                        }
                        Thread.Sleep(1000);
                    }
                }

                Console.Write("\n\n\n");
            }
        }
    }
}