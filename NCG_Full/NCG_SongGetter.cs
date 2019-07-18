using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using VideoLibrary;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;

namespace NCG_Full
{
    class NCG_SongGetter
    {
        public static string pathToSongs = Program.basePath + @"songs\";
        public static string channelName;

        public static void SongGetter()
        {
            Console.WriteLine("NCG Song Getter");
            Console.WriteLine("============================");

            if (File.ReadAllText(Program.basePath + @"Memory\LastSearch.txt") != Convert.ToString(DateTime.Now.Date))
            {
                try
                {
                    GetSongs();
                }
                catch (AggregateException ex)
                {
                    foreach (var e in ex.InnerExceptions)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                }
                File.WriteAllText(Program.basePath + @"Memory\LastSearch.txt", Convert.ToString(DateTime.Now.Date));
                Console.WriteLine("\nResearch of the day done\n\n");
            }
            else
            {
                Console.WriteLine("\nResearch already done today\n\n");
            }
        }
        public static void GetSongs()
        {
            DataTable channels = new DataTable();
            channels.Columns.Add("id", typeof(string));
            channels.Columns.Add("name", typeof(string));
            channels.Rows.Add("UCM3sYnaN67Epz3vrZxPRRMA", "Vital EDM");
            channels.Rows.Add("UCqawukswZ0GUo4efsX9kLGw", "Arc North");
            channels.Rows.Add("UCCeAaS4K5IOr3n6KUZvN9zA", "GANGSTER GANG");
            channels.Rows.Add("UC4AGuSvfkIxX3r7Of7HgRFg", "Trap Monkey");
            channels.Rows.Add("UCh3QapbW5p-PKwrN46RJG5w", "Simplify.");
            channels.Rows.Add("UC65afEgL62PGFWXY7n6CUbA", "Trap City");
            channels.Rows.Add("UC3ifTl5zKiCAhHIBQYcaTeg", "Proximity");
            channels.Rows.Add("UCMOgdURr7d8pOVlc-alkfRg", "xKito Music");
            channels.Rows.Add("UC3xS7KD-nL8dpireWEUIxNA", "Indefinitely Music");
            channels.Rows.Add("UCV3IseaOx-KwjBgeiood8gg", "DJ Smile Music");
            channels.Rows.Add("UCaB_KyYOjfNHBm0f-TvBmiw", "TrapMusicHDTV");
            channels.Rows.Add("UCi2bIyFtz-JdI-ou8kaqsqg", "Trap Music Now.");
            channels.Rows.Add("UCvmUdL2NHWlj1NRiNJPI-TQ", "EDM Bot");
            channels.Rows.Add("UCj_Y-xJ2DRDGP4ilfzplCOQ", "House Nation");
            channels.Rows.Add("UCA2zt34_chJ1S0n9Ke_zh6g", "Car Music");
            channels.Rows.Add("UCjIgbUrRqLPRDAqeAdcYupw", "JacK Music");
            channels.Rows.Add("UCBefBxNTPoNCQBU_Lta6Nvg", "NEFFEX Music");


            foreach (DataRow channel in channels.Rows)
            {
                Console.WriteLine("\nSearching on channel \"" + channel["name"] + "\" ...");
                channelName = channel["name"].ToString();

                var yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyBuoiaCssYGs86HMJ0CcWnsAhxXP7R29XQ" });
                var searchListRequest = yt.Search.List("snippet");
                searchListRequest.Type = "video";
                searchListRequest.Order = SearchResource.ListRequest.OrderEnum.Date;
                searchListRequest.MaxResults = 3;
                searchListRequest.ChannelId = channel["id"].ToString();
                var searchListResult = searchListRequest.Execute();

                foreach (var video in searchListResult.Items)
                {
                    string videoUrl = "https://www.youtube.com/watch?v=" + video.Id.VideoId;
                    string videoName = CheckChar(video.Snippet.Title);
                    string uploadDate = video.Snippet.PublishedAt.ToString().Substring(0, 10);
                    string yesterdayDate = DateTime.Now.AddDays(-1).ToString().Substring(0, 10);

                    if (/*(CheckIfCopyrighted(videoUrl) == false) &&*/ (uploadDate == yesterdayDate) && (CheckIfAlreadyDL(video.Id.VideoId) == false) && !videoName.Contains("Video"))
                    {
                        Console.WriteLine("\nDownloading " + videoName);
                        var task = AudioDownloader(videoUrl, videoName); //Download the video
                        task.Wait();
                        RenameAudioFile(videoName);

                        File.AppendAllText(Program.basePath + @"Memory\DownloadHistory.txt", video.Id.VideoId + Environment.NewLine);
                    }
                }
            }

            Console.WriteLine("\nExiting NCG Song Getter\n\n");
        }

        /// <summary>
        /// NOT WORKING
        /// </summary>
        public static bool CheckIfCopyrighted(string videoUrl)
        {
            //NOT WORKING
            string webpageData;
            using (System.Net.WebClient webClient = new System.Net.WebClient())
                webpageData = webClient.DownloadString(videoUrl);

            string pagedata = webpageData.ToString();
            bool containsWord = webpageData.Contains("Nummer");
            bool containsWord2 = webpageData.Contains("Artiest");

            if (containsWord == false && containsWord == false)
            {
                return false;
            }
            else //(containsWord == true && containsWord == true)
            {
                return true;
            }
        }

        public static bool CheckIfAlreadyDL(string videoId)
        {
            foreach (string line in File.ReadLines(Program.basePath + @"Memory\DownloadHistory.txt"))
            {
                if (line.Contains(videoId))
                {
                    return true;
                }
            }
            return false;
        }

        public static async Task AudioDownloader(string link, string videoName) //YoutubeExplode
        {
            var id = YoutubeClient.ParseVideoId(link);

            var client = new YoutubeClient();
            var streamInfoSet = await client.GetVideoMediaStreamInfosAsync(id);

            var streamInfo = streamInfoSet.Muxed.WithHighestVideoQuality();
            var ext = streamInfo.Container.GetFileExtension();
            await client.DownloadMediaStreamAsync(streamInfo, $"{pathToSongs + videoName}.mp4");

            var inputFile = new MediaFile { Filename = $"{pathToSongs + videoName}.mp4" };
            var outputFile = new MediaFile { Filename = $"{pathToSongs + videoName}.mp3" };

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
                engine.Convert(inputFile, outputFile);
            }

            File.Delete($"{pathToSongs + videoName}.mp4");
            Console.WriteLine("Downloaded : " + videoName);
        }

        public static void AudioDownloader2(string link, string videoName) //VideoLibrary
        {
            var youtube = YouTube.Default;
            var vid = youtube.GetVideo(link);
            var video = vid.GetBytes();
            File.WriteAllBytes(pathToSongs + videoName, video);

            var inputFile = new MediaFile { Filename = pathToSongs + videoName };
            var outputFile = new MediaFile { Filename = $"{pathToSongs + videoName}.mp3" };

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
                engine.Convert(inputFile, outputFile);
            }

            File.Delete(pathToSongs + videoName);
            Console.WriteLine("Downloaded : " + videoName);
        }

        public static void RenameAudioFile(string videoName)
        {
            File.Move(pathToSongs + videoName + ".mp3", pathToSongs + NameFormater.FormatByChannel(videoName, channelName) + ".mp3");
        }

        public static string CheckChar(string inputString)
        {
            var str = inputString;
            var charsToRemove = new string[] { "/", "/", ":", "*", "?", "<", ">", "|", "\"" };
            foreach (var c in charsToRemove)
            {
                str = str.Replace(c, string.Empty);
            }
            return str;
        }
    }
}
