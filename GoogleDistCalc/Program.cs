using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Net;

namespace GoogleDistCalc {
    class Program {
        /// <summary>
        /// args[0] is input filename
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) {

            Console.WriteLine("Reading Input.txt");

            string[] ClientsRows = File.ReadAllLines("input.txt");
            string[] ClientsCol = File.ReadAllLines("input.txt");
            string Origin = "";
            string Destination = "";
            string[,] OutputArr = new string[ClientsRows.Length, ClientsCol.Length];

            Console.WriteLine("Proccessing");

            //populate array
            for (int Row = 0; Row < ClientsRows.Length; Row++) {
                for (int Col = 0; Col < ClientsCol.Length; Col++) {

                    if (ClientsRows[Row] != null && ClientsCol != null) {

                        Origin = ClientsRows[Col].Replace(' ', '+');
                        Destination = ClientsCol[Row].Replace(' ', '+');
                        int distance = getDistance(Origin, Destination);

                        OutputArr[Row, Col] = (distance / 1000).ToString();

                        Origin = "";
                        Destination = "";

                    }
                }
            }

            if (File.Exists("Output.csv")) {
                File.Delete("Output.csv");
            }

            //output to file
            using (StreamWriter output = new StreamWriter("Output.csv")) {

                for (int Row = 0; Row < ClientsRows.Length; Row++) {
                    for (int Col = 0; Col < ClientsCol.Length; Col++) {

                        if (Col <= OutputArr.GetLength(1)) {

                            Console.Write(String.Format("{0}", OutputArr[Row, Col]));
                            Console.Write(",");

                            output.Write(String.Format("{0}", OutputArr[Row, Col]));
                            output.Write(",");

                        }
                    }
                    output.WriteLine();
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Done. please open output.csv");
            Console.WriteLine("Press Enter to Quit");
            Console.Read();

        }

        public static int getDistance(string origin, string destination) {
            System.Threading.Thread.Sleep(1);
            string distance = "";
            string url = "https://maps.googleapis.com/maps/api/distancematrix/xml?units=metric" + "&origins=" + origin + "&destinations=" + destination + "&key=AIzaSyB4hwDqmpQ-p6eTsUXRYmC4-oc_CQjRH6Q";
            //url = https://maps.googleapis.com/maps/api/distancematrix/xml?units=metric&origins=<origin>&destinations=<Destination>&key=YOUR_API_KEY
            string requesturl = url;
            string content = fileGetContents(requesturl);

            if (content != "Error") {
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(content);
                XmlNodeList Nodes = xdoc.GetElementsByTagName("distance");
                distance = Nodes[0].ChildNodes[0].InnerText.ToString();
                return int.Parse(distance);
            }
            else {
                return 0;
            }

        }

        protected static string fileGetContents(string fileName) {
            try {
                var webRequest = WebRequest.Create(fileName);
                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content)) {
                    var strContent = reader.ReadToEnd();
                    return strContent;
                }
            }
            catch {
                Console.WriteLine("Something went wrong");
                return "Error";

            }
        }
    }
}
