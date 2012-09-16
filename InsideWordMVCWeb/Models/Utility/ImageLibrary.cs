using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InsideWordResource;

namespace InsideWordMVCWeb.Models.Utility
{
    public static class ImageLibrary // optional (shown left of message)
    {
        static private readonly ImageInfo _success;
        static private readonly ImageInfo _alert;
        static private readonly ImageInfo _checkEmail;
        static private readonly ImageInfo _defaultProfile;

        static ImageLibrary()
        {
            _success = new ImageInfo
            {
                Width = 64,
                Height = 64,
                Alt = "Success!",
                Src = Links.Content.img.@interface.greenCheckmark_png
            };

            _alert = new ImageInfo
            {
                Width = 64,
                Height = 56,
                Alt = "Alert!",
                Src = Links.Content.img.@interface.alert_png
            };

            _checkEmail = new ImageInfo
            {
                Width = 64,
                Height = 62,
                Alt = "Check your email",
                Src = Links.Content.img.@interface.email_png
            };

            _defaultProfile = new ImageInfo
            {
                Width = 78,
                Height = 82,
                Alt = "Default Profile Image",
                Src = Links.Content.img.@interface.professor_png
            };
        }

        static public ImageInfo Success { get { return _success; } }
        static public ImageInfo Alert { get { return _alert; } }
        static public ImageInfo CheckEmail { get { return _checkEmail; } }
        static public ImageInfo DefaultProfile { get { return _defaultProfile; } }
    }
}