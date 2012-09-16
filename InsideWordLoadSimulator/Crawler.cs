using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using HtmlAgilityPack;

namespace InsideWordLoadSimulator
{
    class Crawler
    {
        public Uri RootAddress { get; set; }
        public List<string> Visited { get; set; }
        public List<double> LoadTimes { get; set; }
        public bool DoneCrawling { get; set; }

        protected string _appendableRoot;
        protected Random _randomizer  { get; set; }


        public Crawler(Uri rootAddress)
        {
            DoneCrawling = false;
            RootAddress = rootAddress;
            _randomizer = new Random((int)DateTime.UtcNow.Ticks);
            Visited = new List<string>();
            LoadTimes = new List<double>();
            _appendableRoot = RootAddress.AbsoluteUri.Substring(0, RootAddress.AbsoluteUri.Length-1);
        }

        public void Start()
        {
            string url = "";
            int choice = 0;
            List<string> visitOptions = null;

            string html = Visit(RootAddress.AbsoluteUri);
            visitOptions = FetchUnvisitedPages(html);

            while (visitOptions.Count > 0)
            {
                choice = _randomizer.Next() % visitOptions.Count;
                url = visitOptions[choice];
                html = Visit(url);
                visitOptions = FetchUnvisitedPages(html);
            }
            DoneCrawling = true;
        }

        static public double RunningAvg(double currentAvg, double newValue, ref double counter)
        {
            double newAverage = (currentAvg * counter + newValue) / (counter + 1);
            counter++;
            return newAverage;
        }

        public string Visit(string url)
        {
            string returnValue = null;
            DateTime start;
            DateTime stop;
            using (WebClient client = new WebClient())
            {
                start = DateTime.UtcNow;
                try
                {
                    returnValue = client.DownloadString(url);
                }
                catch (WebException webException)
                {
                    Console.WriteLine();
                    Console.WriteLine("\t" + url + " (" + webException.Status + ")");
                    Console.WriteLine("\t\t" + webException.Message);
                }
                stop = DateTime.UtcNow;
            }
            LoadTimes.Add( (stop - start).TotalMilliseconds );
            Visited.Add(url);
            return returnValue;
        }

        public List<string> FetchUnvisitedPages(string html)
        {
            List<string> returnList = new List<string>();
            if (!string.IsNullOrWhiteSpace(html))
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                HtmlNodeCollection anchorNodes = htmlDoc.DocumentNode.SelectNodes("//a");
                if (anchorNodes != null)
                {
                    foreach (HtmlNode anchor in anchorNodes)
                    {
                        string target = null;
                        target = anchor.GetAttributeValue("href", null);
                        if (!string.IsNullOrWhiteSpace(target) && target.StartsWith("/"))
                        {
                            target = _appendableRoot + target;
                            if (!Visited.Contains(target))
                            {
                                returnList.Add(target);
                            }
                        }
                    }
                }
            }
            return returnList;
        }

    }
}
