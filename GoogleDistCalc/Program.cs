using System;
using System.IO;
using System.Xml;
using System.Net;
using System.Xml.Linq;
using Hounds;
using System.Linq;

namespace GoogleDistCalc
{
    class Program
    {
        /// <summary>
        /// args[0] is API Key
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) {

            Console.WriteLine("Checking if Settings exist.");
            String APIKey = "";
            CheckSettings:
            if (!File.Exists("Settings.cfg")) {
                if (!(args == null || args.Length == 0)) {
                    XDocument doc = new XDocument(new XElement("settings",
                                                    new XElement("API_Key", Encryption.Encrypt(args[0].ToString()))));
                    doc.Save("./Settings.cfg");
                    goto CheckSettings;
                }

            } else {
                if (File.Exists("Settings.cfg")) {

                    XDocument xdoc = XDocument.Load("Settings.cfg");

                    foreach (XElement element in xdoc.Descendants("settings")) {

                        APIKey = Encryption.Decrypt(element.Value.ToString());
                    }

                }
            }





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

                        Origin = ClientsRows[Col].Trim().Replace(' ', '+');
                        Destination = ClientsCol[Row].Trim().Replace(' ', '+');
                        double distance = getDistance(Origin, Destination, APIKey);
                        distance = Math.Round(distance, 1);
                        if (distance > 0) {
                            OutputArr[Row, Col] = (distance / 1000).ToString("0.0");
                        } else { distance = 0; }

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

                //Horizontal Headings
                Console.Write(",");
                output.Write(",");
                for (int Col = 0; Col < ClientsCol.Length; Col++) {
                    Console.Write(ClientsCol[Col] + ",");
                    output.Write(ClientsCol[Col] + ",");
                }
                Console.WriteLine();
                output.WriteLine();

                //Matrix it,self
                for (int Row = 0; Row < ClientsRows.Length; Row++) {

                    //Vertical Headings
                    Console.Write(ClientsRows[Row] + ",");
                    output.Write(ClientsRows[Row] + ",");

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

        public static double getDistance(string origin, string destination, string Key) {
            System.Threading.Thread.Sleep(1);
            string distance = "";
            string status = "";
            string url = "https://maps.googleapis.com/maps/api/distancematrix/xml?units=metric" + "&origins=" + origin + "&destinations=" + destination + "&key=" + Key;
            //url = https://maps.googleapis.com/maps/api/distancematrix/xml?units=metric&origins=<origin>&destinations=<Destination>&key=YOUR_API_KEY
            string requesturl = url;
            string content = fileGetContents(requesturl);

            if (content != "Error") {
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(content);
                XmlNodeList Nodes = xdoc.GetElementsByTagName("element");

                status = Nodes[0].ChildNodes[0].InnerText.ToString();
                if (status == "OK") {
                    //Console.WriteLine("DEBUG: Status OK");
                    distance = Nodes[0].ChildNodes[2].ChildNodes[0].InnerText.ToString();
                    return double.Parse(distance);
                } else {
                    //Console.WriteLine("DEBUG: Status Not OK");
                    return -1;
                }
            } else {
                return -2;
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
            } catch {
                return "error";
            }
        }


    }
}
