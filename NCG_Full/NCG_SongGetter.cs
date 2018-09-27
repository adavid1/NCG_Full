using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Data;
using System.IO;
using VideoLibrary;

namespace NCG_Full
{
    class NCG_SongGetter
    {
        public static string pathToSongs = @"C:\Users\Axel David\Documents\Code\NCG\songs\";
        public static string v_channelName;

        public static void SongGetter()
        {
            Console.WriteLine("NCG Song Getter");
            Console.WriteLine("============================");

            DataTable channels = new DataTable();
            channels.Columns.Add("id", typeof(string));
            channels.Columns.Add("name", typeof(string));
            channels.Rows.Add("UCM3sYnaN67Epz3vrZxPRRMA", "Vital EDM");
            channels.Rows.Add("UCEickjZj99-JJIU8_IJ7J-Q", "Vlog No Copyright Music");
            channels.Rows.Add("UCqawukswZ0GUo4efsX9kLGw", "Arc North");
            channels.Rows.Add("UCCeAaS4K5IOr3n6KUZvN9zA", "GANGSTER GANG");
            channels.Rows.Add("UC4AGuSvfkIxX3r7Of7HgRFg", "Trap Monkey");
            channels.Rows.Add("UCh3QapbW5p-PKwrN46RJG5w", "Simplify.");
            //channels.Rows.Add("UC65afEgL62PGFWXY7n6CUbA", "Trap City");
            channels.Rows.Add("UC3ifTl5zKiCAhHIBQYcaTeg", "Proximity");
            channels.Rows.Add("UCMOgdURr7d8pOVlc-alkfRg", "xKito Music");

            foreach (DataRow channel in channels.Rows)
            {
                Console.WriteLine("\nSearching on channel \"" + channel["name"] + "\" ...");
                v_channelName = channel["name"].ToString();

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
                    string todayDate = DateTime.Now.ToString().Substring(0, 10);

                    if ((CheckIfCopyrighted(videoUrl) == false) && /*(uploadDate == todayDate)*/ && (CheckIfAlreadyDL(video.Id.VideoId) == false))
                    {
                        Console.WriteLine("\nDownloading " + videoName);
                        AudioDownloader(videoUrl, videoName); //Download the video
                        RenameAudioFile(videoName);

                        File.AppendAllText("DownloadHistory.txt", video.Id.VideoId + Environment.NewLine);
                    }
                }
            }

            Console.WriteLine("\nExiting NCG Song Getter\n\n");
        }

        public static bool CheckIfCopyrighted(string videoUrl)
        {
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
            foreach (string line in File.ReadLines("DownloadHistory.txt"))
            {
                if (line.Contains(videoId))
                {
                    return true;
                }
            }
            return false;
        }

        public static void AudioDownloader(string link, string videoName)
        {
            var youtube = YouTube.Default;
            var vid = youtube.GetVideo(link);
            File.WriteAllBytes(pathToSongs + videoName, vid.GetBytes());

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
            File.Move(pathToSongs + videoName + ".mp3", pathToSongs + NameFormater.FormatByChannel(videoName, v_channelName) + ".mp3");
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
