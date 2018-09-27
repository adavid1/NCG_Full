using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace NCG_Full
{
    class NCG_Uploader
    {
        const string basePath = @"C:\Users\Axel David\Documents\Code\NCG\";

        public static void Uploader()
        {
            Console.WriteLine("NCG Uploader");
            Console.WriteLine("============================");

            bool videoFolder = true, exit = false;

            while (videoFolder && !exit)
            {
                int hourNow = DateTime.Now.Hour;

                if ((hourNow > 12 && hourNow < 16) && File.ReadAllText(basePath + @"Memory\LastUpload.txt") != Convert.ToString(DateTime.Now.Date))
                {

                    Console.WriteLine("\nLooking for uploading...");

                    string videoPath = "", videoName = "";
                    videoFolder = false;

                    //Get oldest song
                    if (Directory.EnumerateFileSystemEntries(basePath + @"\videos").Any())
                    {
                        videoFolder = true;
                        DirectoryInfo songsDir = new DirectoryInfo(basePath + @"\videos");
                        var oldestVideo = songsDir
                        .GetFiles("*.mp4")
                        .OrderBy(file => file.CreationTime)
                        .Select(file => new { file.FullName, file.Name, file.Length })
                        .First();

                        videoPath = oldestVideo.FullName;
                        videoName = Path.GetFileNameWithoutExtension(oldestVideo.Name);
                    }
                    else
                    {
                        videoFolder = false;
                        Console.WriteLine("\nNo more video to upload");
                    }

                    if (videoFolder)
                    {
                        Console.WriteLine("Uploading video...\n");
                        try
                        {
                            new NCG_Uploader().UploadVideo(videoName, videoPath).Wait();
                            ClearVideoFile(videoPath);
                        }
                        catch (AggregateException ex)
                        {
                            foreach (var e in ex.InnerExceptions)
                            {
                                Console.WriteLine("Error: " + e.Message);
                            }
                        }
                    }

                    File.WriteAllText(basePath + @"Memory\LastUpload.txt", Convert.ToString(DateTime.Now.Date));
                }
                else
                {
                    Console.WriteLine("\nCannot upload now");
                    exit = true;
                }
            }

            Console.WriteLine("\nExiting NCG Uploader\n\n");
        }

        private async Task UploadVideo(string videoTitle, string videoPath)
        {
            UserCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { YouTubeService.Scope.YoutubeUpload },
                    "user",
                    CancellationToken.None
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = videoTitle.Replace('∖', '\\');
            //video.Snippet.Description = "Default Video Description";
            //video.Snippet.Tags = new string[] { "tag1", "tag2" };
            //video.Snippet.CategoryId = "22"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "public"; // or "private" or "public"
            //video.Status.PublishAt =

            using (var fileStream = new FileStream(videoPath, FileMode.Open))
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += VideosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += VideosInsertRequest_ResponseReceived;

                await videosInsertRequest.UploadAsync();
            }
        }

        void VideosInsertRequest_ProgressChanged(IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    ClearCurrentConsoleLine();
                    Console.WriteLine("{0} bytes sent", progress.BytesSent);
                    break;

                case UploadStatus.Failed:
                    Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
            }
        }

        void VideosInsertRequest_ResponseReceived(Video video)
        {
            Console.WriteLine("Video '{0}' was successfully uploaded", video.Snippet.Title);
        }

        public static void ClearVideoFile(string videoPath)
        {
            Console.WriteLine("Removing file...");
            File.Delete(videoPath);
            if (!File.Exists(videoPath))
            {
                Console.WriteLine("Video file removed");
            }
        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}