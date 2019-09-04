using System;
using System.Linq;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;


namespace WoWonder.Helpers
{
    public class ImageServiceLoader
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="imageview">ImageViewAsync View</param>
        /// <param name="imageUrl">Url of the image</param>
        /// <param name="transform">For CircleTrancform and border white color = 1 / For RoundedTransformation(10) = 2 / For RoundedTransformation with border = 3 / For CircleTrancform and border MainColor = 4 / For RoundedTransformation(30) = 5 </param>
        ///  <param name="downSample">DownSample the Image size by deafault its true</param>
        public static void Load_Image(ImageViewAsync imageview, string imagePlaceholder, string imageUrl,int transform = 0, bool downSample = true, int transformRadius = 5)
        {
            try
            {
                TaskParameter imageTrancform = ImageService.Instance.LoadCompiledResource(imagePlaceholder);
                 
                if (imageUrl.Contains("d-avatar.jpg"))
                {
                    imageTrancform = ImageService.Instance.LoadCompiledResource("no_profile_image.png");
                }
                else if (imageUrl.Contains("d-cover.jpg") )
                {
                    imageTrancform = ImageService.Instance.LoadCompiledResource("Cover_image.jpg");
                } 
                else if (imageUrl.Contains("http"))
                {
                    if (!String.IsNullOrEmpty(imageUrl) && (imageUrl.Contains("http") || imageUrl.Contains("file://") || imageUrl.Split('/').Count() > 1))
                        imageTrancform = ImageService.Instance.LoadUrl(imageUrl);
                    else if (!String.IsNullOrEmpty(imageUrl))
                        imageTrancform = ImageService.Instance.LoadCompiledResource(imageUrl);
                    else
                        imageTrancform = ImageService.Instance.LoadCompiledResource(imagePlaceholder);
                } 
                else
                {
                    var file = Android.Net.Uri.FromFile(new Java.IO.File(imageUrl));
                    imageTrancform = ImageService.Instance.LoadFile(file.Path);
                }

                imageTrancform.TransformPlaceholders(true).Retry(3, 5000).Error(OnError);
                imageTrancform.DownSampleMode(InterpolationMode.Default);
                imageTrancform.BitmapOptimizations(true);
                 
                if (downSample)
                {
                    imageTrancform.Success((information, result) =>
                    {
                        if (information.OriginalHeight > 1000 || information.OriginalWidth > 1000)
                        {
                            imageTrancform.DownSample(250, 250);
                           // imageTrancform.DownSampleInDip(200, 200, false);
                        }
                        else if (information.OriginalHeight > 300 && information.OriginalWidth > 300)
                        {
                            imageTrancform.DownSampleInDip(150, 150, false);
                           // imageTrancform.DownSample(200, 200);
                        }
                        else if (information.OriginalHeight > 200 && information.OriginalWidth > 200)
                        {
                            imageTrancform.DownSample(100, 100);
                        }
                        else if (information.OriginalHeight > 100 && information.OriginalWidth > 100)
                        {
                            imageTrancform.DownSample(40, 40);
                        }
                        else
                        {
                            imageTrancform.DownSample(25, 25);
                        }

                        if (transform == 2)
                            imageTrancform.Transform(new RoundedTransformation(10));
                    });
                }
                 
                if (transform == 1)
                    imageTrancform.Transform(new CircleTransformation(transformRadius, "#ffffff"));

                if (transform == 2)
                    imageTrancform.Transform(new RoundedTransformation(10));

                if (transform == 3)
                    imageTrancform.Transform(new RoundedTransformation(10, 2, 2, 10, "#ffffff"));

                if (transform == 4)
                    imageTrancform.Transform(new CircleTransformation(5, Settings.MainColor));

                if (transform == 5)
                    imageTrancform.Transform(new RoundedTransformation(30));
                
                imageTrancform.LoadingPlaceholder(imagePlaceholder, ImageSource.CompiledResource);
                imageTrancform.ErrorPlaceholder(imagePlaceholder, ImageSource.CompiledResource);
                 
                imageTrancform.Into(imageview);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public static void Load_ImageResampled(ImageViewAsync imageview, string imagePlaceholder, string imageUrl,int transform = 0, bool downSample = true, int transformRadius = 5, bool CustomHieght = false)
        {
            try
            {
                TaskParameter imageTrancform = ImageService.Instance.LoadCompiledResource(imagePlaceholder);
                 
                if (imageUrl.Contains("http"))
                {
                    if (!String.IsNullOrEmpty(imageUrl) && (imageUrl.Contains("http") || imageUrl.Contains("file://") || imageUrl.Split('/').Count() > 1))
                        imageTrancform = ImageService.Instance.LoadUrl(imageUrl);
                    else if (!String.IsNullOrEmpty(imageUrl))
                        imageTrancform = ImageService.Instance.LoadCompiledResource(imageUrl);
                    else
                        imageTrancform = ImageService.Instance.LoadCompiledResource(imagePlaceholder);
                }
                else
                {
                    var file = Android.Net.Uri.FromFile(new Java.IO.File(imageUrl));
                    imageTrancform = ImageService.Instance.LoadFile(file.Path);
                }

                if (downSample)
                    imageTrancform.DownSampleMode(InterpolationMode.Medium);

                if (transform == 1)
                    imageTrancform.Transform(new CircleTransformation(transformRadius, "#ffffff"));

                if (transform == 2)
                    imageTrancform.Transform(new RoundedTransformation(10));

                if (transform == 3)
                    imageTrancform.Transform(new RoundedTransformation(10, 2, 2, 10, "#ffffff"));
                
                imageTrancform.LoadingPlaceholder(imagePlaceholder, ImageSource.CompiledResource);
                imageTrancform.ErrorPlaceholder(imagePlaceholder, ImageSource.CompiledResource);
                imageTrancform.TransformPlaceholders(true).Retry(3, 5000).Error(OnError);
                
                if (CustomHieght)
                    imageTrancform.Success((information, result) =>
                    {
                        if (information.OriginalHeight > information.OriginalWidth)
                        {
                            if (information.OriginalHeight > 200 && information.OriginalHeight < 400)
                            {
                                imageTrancform.DownSample(height: 320);
                                imageview.SetMinimumHeight(150);
                            }
                            else if (information.OriginalHeight > 400 && information.OriginalHeight < 500)
                            {
                                imageTrancform.DownSample(height: 320);
                                imageview.SetMinimumHeight(160);
                            }
                            else if (information.OriginalHeight > 500 && information.OriginalHeight < 1000)
                            {
                                imageTrancform.DownSample(height: 420);
                                imageview.SetMinimumHeight(170);
                            }
                            if (information.OriginalHeight > 1000 && information.OriginalHeight < 2000)
                            {
                                imageTrancform.DownSample(width: 200);
                                imageview.SetMinimumHeight(180);
                            }
                        }
                        else
                        {
                            if (information.OriginalHeight > 200 && information.OriginalWidth < 500)
                            {
                                imageTrancform.DownSample(width: 210);
                                imageTrancform.DownSample(height: 210);
                                imageview.SetMinimumHeight(110);

                            }
                            else if (information.OriginalHeight < 500 && information.OriginalWidth < 1000)
                            {
                                imageTrancform.DownSample(width: 260);
                                imageTrancform.DownSample(height: 260);
                                imageview.SetMinimumHeight(130);
                                imageview.SetMinimumWidth(180);
                            }
                            else if (information.OriginalHeight < 1000 && information.OriginalWidth < 2000)
                            {

                                imageTrancform.DownSample(width: information.OriginalWidth / 2);
                                imageTrancform.DownSample(height: information.OriginalHeight / 2);
                                imageview.SetMinimumHeight(140);
                                imageview.SetMinimumWidth(210);
                            }
                            else if (information.OriginalHeight < 2000 && information.OriginalWidth < 2500)
                            {
                                imageTrancform.DownSample(width: information.OriginalWidth / 3);
                                imageTrancform.DownSample(height: information.OriginalHeight / 3);
                                imageview.SetMinimumHeight(150);
                                imageview.SetMinimumWidth(230);
                            }
                        }
                        if (transform == 2)
                            imageTrancform.Transform(new RoundedTransformation(10));
                    });
                
                imageTrancform.Into(imageview);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        private static void OnError(Exception exception)
        {
            try
            {
                var dd = exception.Source;
                var dsd = exception.Data.Values;
                //On Image Error loading 

            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
    }
}