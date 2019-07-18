using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.IO;

namespace NCG_Full
{
    internal class NCG_FeedGetter
    {
        public static void FeedGetter()
        {
            Console.WriteLine("NCG Feed Getter");
            Console.WriteLine("============================");

            if (File.ReadAllText(Program.basePath + @"Memory\LastFeedRefresh.txt") != Convert.ToString(DateTime.Now.Date))
            {
                try
                {
                    new NCG_FeedGetter().GetLatestUploads();
                }
                catch (AggregateException ex)
                {
                    foreach (var e in ex.InnerExceptions)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                }
                File.WriteAllText(Program.basePath + @"Memory\LastFeedRefresh.txt", Convert.ToString(DateTime.Now.Date));
                Console.WriteLine("\nFeed Updated\n\n");
            }
            else
            {
                Console.WriteLine("\nFeed already updated today\n\n");
            }
        }

        private void GetLatestUploads()
        {
            var yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyBuoiaCssYGs86HMJ0CcWnsAhxXP7R29XQ" });
            var searchListRequest = yt.Search.List("snippet");
            searchListRequest.Type = "video";
            searchListRequest.Order = SearchResource.ListRequest.OrderEnum.Date;
            searchListRequest.MaxResults = 15;
            searchListRequest.ChannelId = "UCI9enNiz3TK48az13fMGTfg";
            var searchListResult = searchListRequest.Execute();

            File.WriteAllText(Program.basePath + @"Memory\FeedLatest.txt", String.Empty); //Clear file

            foreach (var video in searchListResult.Items)
            {
                string videoUrl = "https://www.youtube.com/watch?v=" + video.Id.VideoId;
                string videoName = NCG_SongGetter.CheckChar(video.Snippet.Title);
                string uploadDate = video.Snippet.PublishedAt.ToString().Substring(0, 10);
                string yesterdayDate = DateTime.Now.AddDays(-1).ToString().Substring(0, 10);

                Console.WriteLine("{0} ({1})", videoName, video.Id.VideoId);
                File.AppendAllText(Program.basePath + @"Memory\FeedLatest.txt", videoName + Environment.NewLine + video.Id.VideoId + Environment.NewLine);
            }
        }
    }
}