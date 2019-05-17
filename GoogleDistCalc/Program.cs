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

            Console.WriteLine("Proccessing");

            if (File.Exists("Output.csv")) {
                File.Delete("Output.csv");
            }
            using (StreamWriter output = new StreamWriter("Output.csv")) {
                for (int i = 0; i < ClientsRows.Length; i++) {
                    for (int j = 0; j < ClientsCol.Length; j++) {
                        if (ClientsRows[i] != null && ClientsCol != null) {
                            Origin = ClientsRows[i].Replace(' ', '+');
                            Destination = ClientsCol[j].Replace(' ', '+');

                            Console.Write("" + ClientsRows[i] + ",");
                            Console.Write("" + ClientsCol[j] + ",");
                            Console.Write(getDistance(Origin, Destination).ToString());

                            Console.Write(Environment.NewLine);

                            output.Write("" + ClientsRows[i] + ",");
                            output.Write("" + ClientsCol[j] + ",");
                            output.Write(getDistance(Origin, Destination).ToString());

                            output.Write(Environment.NewLine);

                            Origin = "";
                            Destination = "";

                        }
                    }
                }
            }

            Console.WriteLine("Done. please open output.csv");
            Console.WriteLine("Press Enter to Quit");
            Console.Read();

        }

        public static string getDistance(string origin, string destination) {
            System.Threading.Thread.Sleep(10);
            string distance = "";
            string url = "https://maps.googleapis.com/maps/api/distancematrix/xml?units=metric" + "&origins=" + origin + "&destinations=" + destination + "&key=AIzaSyB4hwDqmpQ-p6eTsUXRYmC4-oc_CQjRH6Q";
            //url = https://maps.googleapis.com/maps/api/distancematrix/xml?units=metric&origins=<origin>&destinations=<Destination>&key=YOUR_API_KEY
            //Console.WriteLine("URL: " + url);
            string requesturl = url;
            string content = fileGetContents(requesturl);

            if (content != "Error") {
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(content);
                XmlNodeList Nodes = xdoc.GetElementsByTagName("distance");
                distance = Nodes[0].ChildNodes[1].InnerText.ToString();
                return distance;
            }
            else {
                return "Error getting distance";
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
