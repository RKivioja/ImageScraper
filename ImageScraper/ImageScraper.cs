using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Drawing;

namespace ImageScraper
{
    class ImageScraper
    {
        static void Main(string[] args)
        {
            var document = LoadPage("http://papunet.net/kuvatyokalu/api/browse/class/?type=all");
            var imagePageLinks = GetNodes(document, "a");

            foreach(var imagePageLink in imagePageLinks)
            {
                var folder = CreateFolder(imagePageLink);
                DownloadImages(imagePageLink, folder);
            }
            
            Console.ReadKey();
        }

        private static void DownloadImages(HtmlNode imagePageLink, DirectoryInfo folder)
        {
            string imagepagelink = "http://papunet.net" + imagePageLink.Attributes["href"].Value.ToString();
            var imagepage = LoadPage(imagepagelink);
            var imagelinks = GetNodes(imagepage, "img");

            foreach(var imagelink in imagelinks)
            {
                try
                {
                    SaveImage(imagelink.Attributes["src"].Value.ToString(), folder.FullName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static void SaveImage(string url, string foldername)
        {
            string filename = url.Split('/')[url.Split('/').Length - 1];

            using (WebClient client = new WebClient())
            {
                Console.WriteLine("Starting file download: " + url);
                client.DownloadFile(url, foldername + '/' + filename);
                Console.WriteLine("File downloaded.");
            }
        }

        private static DirectoryInfo CreateFolder(HtmlNode imagePageLink)
        {
            string folderName = @"C:\AAC-symbols";
            string pathString = Path.Combine(folderName, imagePageLink.InnerText);

            if (!Directory.Exists(pathString))
            {
                var directory = Directory.CreateDirectory(pathString);
                Console.WriteLine("Created folder: " + directory.FullName);
                return directory;
            }

            return null;
        }

        private static HtmlDocument LoadPage(string url)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);

            return doc;
        }

        private static List<HtmlNode> GetNodes(HtmlDocument document, string nodeType)
        {
            return document.DocumentNode.Descendants(nodeType).ToList();
        }
    }
}
