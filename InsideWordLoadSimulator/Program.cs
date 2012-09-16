using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using HtmlAgilityPack;
using System.Threading;

namespace InsideWordLoadSimulator
{
    class Program
    {
        static public List<Crawler> crawlerList;
        static public List<Thread> crawlerThreadList;
        static public Uri rootAddress;
        static public int testLimit = 100;

        static void Main(string[] args)
        {
            crawlerList = new List<Crawler>();
            rootAddress = new Uri("http://www.chunkng.com/");
            for (int totalCrawlers = 1; totalCrawlers < testLimit; totalCrawlers++)
            {
                AddCrawlers(totalCrawlers);
                StartCrawlers();
                while (CrawlersRunning()) { Thread.Sleep(1500); }
                Console.WriteLine(totalCrawlers + "\t=\t" + AverageLoadTime(crawlerList));
            }
        }

        static public void AddCrawlers(int count)
        {
            crawlerList = new List<Crawler>();
            for(int currentCount = 0; currentCount < count; currentCount++)
            {
                crawlerList.Add(new Crawler(rootAddress));
            }
        }

        static public void StartCrawlers()
        {
            crawlerThreadList = new List<Thread>();
            foreach (Crawler aCrawler in crawlerList)
            {
                Thread crawlerThread = new Thread(aCrawler.Start);
                crawlerThreadList.Add(crawlerThread);
                crawlerThread.Start();
            }
        }

        static public bool CrawlersRunning()
        {
            bool returnValue = false;
            foreach(Thread crawlerThread in crawlerThreadList)
            {
                if (crawlerThread.ThreadState == ThreadState.Running)
                {
                    returnValue = true;
                    break;
                }
            }
            return returnValue;
        }

        static double AverageLoadTime(List<Crawler> crawlerList)
        {
            double sum = 0;
            double count = 0;
            foreach(Crawler aCrawler in crawlerList)
            {
                sum += aCrawler.LoadTimes.Sum();
                count += aCrawler.LoadTimes.Count();
            }
            return sum / count;
        }
    }
}
