using Accord.Video.FFMPEG;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using NAudio.Wave;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Microsoft.WindowsAPICodePack.Shell;
using System.IO;
using System.Linq;

namespace NCG_Full
{
    class NCG_VideoEditor
    {
        public static void VideoEditor()
        {
            Console.WriteLine("NCG Video editor");
            Console.WriteLine("============================");

            bool songFolder = true, picFolder = true;
            string songPath = "", songName = "", backgroundPath = "";
            int songDuration = 0;

            while (songFolder && picFolder)
            {
                Console.WriteLine("\nLooking for editing...");

                songFolder = false;
                picFolder = false;

                //Get oldest song
                if (Directory.EnumerateFileSystemEntries(Program.basePath + @"\songs").Any())
                {
                    songFolder = true;
                    DirectoryInfo songsDir = new DirectoryInfo(Program.basePath + @"\songs");//Assuming Test is your Folder
                    var oldestSong = songsDir
                    .GetFiles("*.mp3")
                    .OrderBy(file => file.CreationTime)
                    .Select(file => new { file.FullName, file.Name })
                    .First();

                    songPath = oldestSong.FullName;
                    songDuration = GetSongDuration(songPath);
                    songName = Path.GetFileNameWithoutExtension(oldestSong.Name);
                }
                else
                {
                    songFolder = false;
                    Console.WriteLine("No more songs to edit");
                }

                //Get random background
                if (Directory.EnumerateFileSystemEntries(Program.basePath + @"pictures\backgrounds\").Any())
                {
                    var rand = new Random();

                    picFolder = true;
                    var pics = Directory.GetFiles(Program.basePath + @"pictures\backgrounds\");

                    backgroundPath = pics[rand.Next(pics.Length)];
                }
                else
                {
                    picFolder = false;
                    Console.WriteLine("No more backgrounds available");
                }


                if (songDuration > 300) //delete file is duration is more than 5min
                {
                    Console.WriteLine("Song too long, deleting file...");
                    File.Delete(songPath);
                    break;
                }


                if (songFolder && picFolder)
                {
                    string videoPath = Program.basePath + @"videos\" + songName + ".mp4";
                    int bitrate = 44100, samplerate = 192000, fps = 30;
                    double progress = 0;
                    byte[] audioPart1, audioPart2, audioPart3, audioPart4;

                    using (Mp3FileReader reader = new Mp3FileReader(songPath))
                    {

                        //read first quarter
                        audioPart1 = new byte[reader.Length / 4];
                        reader.Read(audioPart1, 0, audioPart1.Length);

                        //read second quarter
                        reader.Position = reader.Length / 4;
                        audioPart2 = new byte[reader.Length / 4];
                        reader.Read(audioPart2, 0, audioPart2.Length);

                        //read third quarter
                        reader.Position = (reader.Length / 4) * 2;
                        audioPart3 = new byte[reader.Length / 4];
                        reader.Read(audioPart3, 0, audioPart3.Length);

                        //read fourth quarter
                        reader.Position = (reader.Length / 4) * 3;
                        audioPart4 = new byte[reader.Length / 4];
                        reader.Read(audioPart4, 0, audioPart4.Length);

                        bitrate = reader.Mp3WaveFormat.AverageBytesPerSecond * 8;
                        samplerate = reader.Mp3WaveFormat.SampleRate;
                    }

                    Console.WriteLine("\nEditing video...");
                    Console.WriteLine("Name : " + songName);
                    Console.WriteLine("Duration : " + songDuration.ToString() + " seconds");
                    Console.WriteLine(" ");

                    using (var videoWriter = new VideoFileWriter())
                    using (Bitmap background = Bitmap.FromFile(backgroundPath) as Bitmap)
                    {
                        double totalFrame = songDuration * fps;
                        int x = 0, playCounter = 0;
                        Bitmap logo = null, image = null;

                        videoWriter.Open(videoPath, 1920, 1080, fps, VideoCodec.MPEG4, 50000000, AudioCodec.MP3, bitrate, samplerate, 2);

                        for (int seconds = 0; seconds < songDuration; seconds++) //Per second
                        {
                            for (int compt = 0; compt < fps; compt++) //30 times loop
                            {
                                if (logo != null)
                                {
                                    logo.Dispose();
                                }

                                if (x < 330)
                                {
                                    logo = Bitmap.FromFile(Program.basePath + @"pictures\overlay\intro\intro_" + x.ToString("D6") + ".png") as Bitmap;
                                }
                                else
                                {
                                    logo = Bitmap.FromFile(Program.basePath + @"pictures\overlay\play\play_" + playCounter.ToString("D6") + ".png") as Bitmap;
                                    if (playCounter < 89)
                                    {
                                        playCounter++;
                                    }
                                    else
                                    {
                                        playCounter = 0;
                                    }
                                }

                                image = CombineBitmap(background, logo);
                                videoWriter.WriteVideoFrame(image);
                                image.Dispose();
                                x++;
                            }
                            progress = ((double)x / totalFrame) * 100;
                            Console.SetCursorPosition(0, Console.CursorTop - 1);
                            ConsoleTools.ClearCurrentConsoleLine();
                            Console.WriteLine("Progress : " + (int)progress + "%");
                        }
                        videoWriter.WriteAudioFrame(audioPart1);
                        videoWriter.WriteAudioFrame(audioPart2);
                        videoWriter.WriteAudioFrame(audioPart3);
                        videoWriter.WriteAudioFrame(audioPart4);
                        videoWriter.Close();
                    }

                    Console.WriteLine("Video edited");
                    ClearFiles(videoPath, backgroundPath, songPath);

                    Console.WriteLine("\nExiting NCG Video Editor\n\n");
                    break;
                }
                else
                {
                    Console.WriteLine("\nExiting NCG Video Editor\n\n");
                }
            }
        }

        public static Bitmap CombineBitmap(Bitmap lowerLayer, Bitmap upperLayer)
        {
            var output = new Bitmap(1920/*lowerLayer.Width*/, 1080/*lowerLayer.Height*/, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(output);
            graphics.CompositingMode = CompositingMode.SourceOver;

            graphics.DrawImage(lowerLayer, 0, 0, 1920, 1080);
            graphics.DrawImage(upperLayer, 0, 0, 1920, 1080);
            graphics.Dispose();

            return output;
        }

        public static void ClearFiles(string videoPath, string backgroundPath, string songPath)
        {
            Console.WriteLine("Archiving background...");
            File.Copy(backgroundPath, Program.basePath + @"pictures\used_backgrounds\" + Path.GetFileName(backgroundPath));

            Console.WriteLine("Removing files...");
            if (File.Exists(videoPath))
            {
                File.Delete(backgroundPath);
                File.Delete(songPath);
            }
            if (!File.Exists(backgroundPath))
            {
                Console.WriteLine("Background removed");
            }
            if (!File.Exists(songPath))
            {
                Console.WriteLine("Song removed");
            }
        }

        public static int GetSongDuration(string songPath)
        {
            using (var shell = ShellObject.FromParsingName(songPath)) //get audio duration
            {
                IShellProperty durationProp = shell.Properties.System.Media.Duration;
                var t = (ulong)durationProp.ValueAsObject;
                return Convert.ToInt32(TimeSpan.FromTicks((long)t).TotalSeconds) + 1;
            }
        }
    }
}