using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace Thron.Test
{

    [TestClass()]
    public class RealGameTest
    {
        string rememberMeCookie = "1510662d387f857308cc7bac141570d682125d8";
        int userID = 1510662;

        static HttpClient client = new HttpClient(new LoggingHandler(new HttpClientHandler()));


        [TestMethod()]
        public void DebugGame()
        {
            var gameInputs = parseGameInput(readGameInput(File.ReadAllText("GameResult.json")));

            Console.SetIn(new StringReader(string.Join("\n", gameInputs)));
            Program.Main(null);
        }

        [TestMethod()]
        public void DebugGameRemote()
        {
            string startLocations = "(25,12)(2,7)\n";
            var gameInputs = parseGameInput(readGameInput(GameJsonFromServer(startLocations)));

            Console.SetIn(new StringReader(string.Join("\n", gameInputs)));
            Program.Main(null);
        }


        private bool GameIsWon(string gameJson)
        {
            dynamic data = JObject.Parse(gameJson);
            int first = data.ranks[0];
            return first == 0;
        }

        private List<string> readGameInput(string gameResultJson)
        {
            List<string> result = new List<string>();
            dynamic data = JObject.Parse(gameResultJson);
            foreach (var frame in data.frames)
            {
                string agent = frame.agentId;
                if (agent == "0")
                {
                    string error = frame.stderr;
                    result.Add(error);
                }
            }

            return result;
        }


        private List<string> parseGameInput(List<string> errorOutputs)
        {
            List<string> result = new List<string>();
            foreach (var errorOutput in errorOutputs)
            {
                string[] errorLines = errorOutput.Split('\n');
                foreach (var line in errorLines)
                {
                    MatchCollection matches = Regex.Matches(line, @"debug:\^(.*)debug:\$");
                    foreach (Match match in matches)
                    {
                        result.Add(match.Groups[1].Value);
                    }
                }
            }
            return result;
        }

        public string GameJsonFromServer(string startLocations)
        {
            client.DefaultRequestHeaders.Add("cookie", $"rememberMe = {rememberMeCookie};");
            string code = File.ReadAllText("Program.cs");
            dynamic multi = new JObject();
            multi.agentsIds = JToken.FromObject(new int[] { -1, -2 });
            multi.gameOptions = startLocations;
            dynamic json = new JObject();
            json.code = code;
            json.programmingLanguageId = "C#";
            json.multi = multi;
            string payload = $"[\"653412561b2d3ea04ec25f89dc9262d1eb4c16e\",{json} ]";


            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = client.PostAsync("https://www.codingame.com/services/TestSession/play", content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

    }
}