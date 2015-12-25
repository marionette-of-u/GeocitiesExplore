using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Windows.Forms;

namespace GeocitiesExplore
{
    class Program
    {
        static async Task<string> GetWebPageAsync(Uri uri)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(
                    "User-Agent",
                    "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko");
                client.DefaultRequestHeaders.Add("Accept-Language", "ja-JP");
                client.Timeout = TimeSpan.FromSeconds(10.0);
                try
                {
                    return await client.GetStringAsync(uri);
                }
                catch (Exception)
                {
                }
                return null;
            }
        }

        static string[] communityArray =
        {
            "Outdoors",
            "Athlete",
            "AnimalPark",
            "AnimeComic",
            "WallStreet",
            "CollegeLife",
            "SiliconValley",
            "SilkRoad",
            "SweetHome",
            "Stylish",
            "Technopolis",
            "NatureLand",
            "NeverLand",
            "HeartLand",
            "HiTeens",
            "PowderRoom",
            "Hollywood",
            "Beautycare",
            "Foodpia",
            "Bookend",
            "Playtown",
            "MusicStar",
            "MusicHall",
            "Milano",
            "Milkyway",
            "MotorCity"
        };

        [STAThreadAttribute]
        static void Main(string[] args)
        {
            if (args.Length < 3 || args.Length > 4)
            {
                string communityList = "";
                for (int i = 0; i < communityArray.Length; ++i)
                {
                    communityList += i.ToString() + "    " + communityArray[i] + "\n";
                }
                Console.Write("usage: geoexplore community n m [--nohtml]\n- community{0}\n- n, m\n1000~9999\n", communityList);
                return;
            }
            
            int communityNum = 0;
            try
            {
                bool find = false;
                for (int i = 0; i < communityArray.Length; ++i)
                {
                    if (args[0] == communityArray[i])
                    {
                        communityNum = i;
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    communityNum = int.Parse(args[0]);
                    if (communityNum < 0 || communityNum >= communityArray.Length)
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid community, {0}.", args[0]);
                return;
            }

            int addressN;
            try
            {
                addressN = int.Parse(args[1]);
                if (addressN < 1000 || addressN > 9999)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid address (n), {0}.", args[1]);
                return;
            }

            int addressM;
            try
            {
                addressM = int.Parse(args[2]);
                if (addressM < 1000 || addressM > 9999)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid address (m), {0}.", args[2]);
                return;
            }

            int optionalIndex = 3;

            bool html = true;
            if (args.Length >= 4 && args[optionalIndex] == "--nohtml")
            {
                html = false;
                ++optionalIndex;
            }

            using (StreamWriter htmlFile = html ? new StreamWriter(communityArray[communityNum] + "_" + addressN.ToString() + "-" + addressM.ToString() + ".html", true, new System.Text.ASCIIEncoding()) : null)
            {
                if (html)
                {
                    htmlFile.Write(
@"<html>
    <head>
        <script>
            var URL = ["
                    );
                }
                for (int i = addressN; i <= addressM; ++i)
                {
                    string str = "http://www.geocities.co.jp/" + communityArray[communityNum] + "/" + i.ToString() + "/";
                    Console.Write(str);
                    Task<string> result = GetWebPageAsync(new Uri(str));
                    result.Wait();
                    if (result == null || result.Result == null || result.Result.IndexOf("<frame src=\"http://info.geocities.yahoo.co.jp/attachments/404_NotFoundUser.html\">") != -1)
                    {
                        Console.WriteLine(" - 404.");
                    }
                    else
                    {
                        Console.WriteLine(" - found.");
                        if (html)
                        {
                            htmlFile.WriteLine("                \"" + str + "\"" + (i != addressM ? "," : ""));
                        }
                    }
                }
                if (html)
                {
                    htmlFile.Write(
@"            ];

            var idx = 0;

            function load(){
                var disp = document.getElementById('url_disp');
                disp.innerHTML = URL[idx];
                var f = document.getElementById('frame');
                f.src = URL[idx];
            }

            function init(){
                load();
            }

            function next(){
                ++idx;
                if(URL.lenght == idx){
                    idx = 0;
                }
                load();
            }

            function prev(){
                --idx;
                if(idx < 0){
                    idx = URL.length - 1;
                }
                load();
            }
        </script>
        <title>WallStreet 1000 - 3000</title>
    </head>
    <body onload='init();'>
        <button onclick='prev();'>prev</button> <button onclick='next();'>next</button>
        <p><span id='url_disp'></span></p>
        <iframe src='#' id='frame' width='100%' height='90%'></iframe>
    </body>
</html>"
                    );
                }
            }
            Console.WriteLine("Finish.");
        }
    }
}
