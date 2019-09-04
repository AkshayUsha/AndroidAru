using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json.Linq;
using YoutubeExtractor;
using IOException = System.IO.IOException;

namespace WoWonder.Helpers
{
    public class VideoInfoRetriever
    {
        private static string URL_YOUTUBE_GET_VIDEO_INFO = "http://www.youtube.com/get_video_info?&video_id=";

        public static IEnumerable<YoutubeExtractor.VideoInfo> videoInfos = null;
        public static YoutubeExtractor.VideoInfo VideoSelected = null;

        public static string VideoDownloadstring = null;


        public class VideoInfo
        {
            public string quality { get; set; }
            public string Videourl { get; set; }
        }

        public static async Task<List<VideoInfo>> Get_Youtube_Video(string url)
        {
            try
            {
                List < VideoInfo > ListVideos = new List<VideoInfo>();
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("https://you-link.herokuapp.com/?url="+ url);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                  
                    JArray Videos = JArray.Parse(json);
                    if (Videos.Count >= 0)
                    {
                        foreach (var key in Videos)
                        {
                            var quality = key["quality"].ToString();
                            var Videourl = key["url"].ToString();
                            VideoInfo vid = new VideoInfo();
                            vid.Videourl = Videourl;
                            vid.quality = quality;
                            ListVideos.Add(vid);
                        }
                        
                        return ListVideos;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                return null;
            }
        }


        public static async Task<string> GetEmbededVideo(string url)
        {
            try
            {
                if (url.Contains("youtube") || url.Contains("youtu"))
                {
                    videoInfos =  await DownloadUrlResolver.GetDownloadUrlsAsync(url);
                  
                    if (videoInfos.Count() > 1)
                    {
                        YoutubeExtractor.VideoInfo video =  videoInfos.FirstOrDefault(info => info.VideoType == VideoType.Mp4 && info.Resolution == 720);

                        if (video == null)
                            video = videoInfos.FirstOrDefault(
                                info => info.VideoType == VideoType.Mp4 && info.Resolution == 480);

                        if (video == null)
                            video = videoInfos.FirstOrDefault(
                                info => info.VideoType == VideoType.Mp4 && info.Resolution == 360);

                        if (video == null)
                            video = videoInfos.FirstOrDefault(
                                info => info.VideoType == VideoType.Mp4 && info.Resolution == 240);

                        if (video == null)
                            video = videoInfos.FirstOrDefault(
                                info => info.VideoType == VideoType.Mp4 && info.Resolution == 144);

                        if (video != null)
                        {
                            if (video.RequiresDecryption)
                                DownloadUrlResolver.DecryptDownloadUrl(video);
                            VideoSelected = video;
                            VideoDownloadstring = video.DownloadUrl;

                            return VideoDownloadstring;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        var Result = await Get_Youtube_Video(url);
                        if (Result != null)
                        {
                            VideoDownloadstring = Result[0].Videourl;
                            return VideoDownloadstring;
                        }
                        else
                        {
                            return null;
                        } 
                    } 
                }

                else
                {
                    return null;
                }

            }
            catch (IOException exception)
            {
                Crashes.TrackError(exception);
                return "Invalid";
            }
        }
    }
}