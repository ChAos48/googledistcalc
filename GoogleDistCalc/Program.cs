using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GoogleDistCalc
{
    class Program
    {
        /// <summary>
        /// args[0] is input filename
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) {
            string[,] workingArray = new string[200, 2];
            string inputfilename = args[0];
            string[] Clients = File.ReadAllLines(inputfilename);
            string Origin = "";
            string Destination = "";
            string[,] OutputArray = new string[200, 3];

            for (int i = 0; i < 200; i++) {
                workingArray[i, 0] = Clients[i];
                workingArray[i, 1] = Clients[i];
            }
            
            for (int j = 0; j < 200; j++) {

                Origin = workingArray[j, 0].Replace(' ', '+');
                Destination = workingArray[j, 1].Replace(' ', '+');

                
                OutputArray[j,0] = Origin;
                OutputArray[j,1] = Destination;
                OutputArray[j, 3] = getDistance(Origin, Destination).ToString();

            }

            using (StreamWriter output = new StreamWriter("Output.csv")) {
                for(int i = 0; i < 200; i++) {
                    output.Write(OutputArray[i, 0]+",");
                    output.Write(OutputArray[i, 1] + ",");
                    output.Write(OutputArray[i, 2] + Environment.NewLine);
                }
            }
            
        }

        public static int getDistance(string origin, string destination) {
            System.Threading.Thread.Sleep(1000);
            int distance = 0;
            //string from = origin.Text;
            //string to = destination.Text;
            string url = "http://maps.googleapis.com/maps/api/directions/json?origin=" + origin + "&destination=" + destination + "&units=" + "&key=AIzaSyB4hwDqmpQ-p6eTsUXRYmC4-oc_CQjRH6Q";
            string requesturl = url;
            //string requesturl = @"http://maps.googleapis.com/maps/api/directions/json?origin=" + from + "&alternatives=false&units=imperial&destination=" + to + "&sensor=false";
            string content = fileGetContents(requesturl);
            JObject o = JObject.Parse(content);
            try {
                distance = (int)o.SelectToken("routes[0].legs[0].distance.value");
                return distance;
            } catch {
                return distance;
            }
            return distance;
            //ResultingDistance.Text = distance;
        }

        protected static string fileGetContents(string fileName) {
            string sContents = string.Empty;
            string me = string.Empty;
            try {
                if (fileName.ToLower().IndexOf("http:") > -1) {
                    System.Net.WebClient wc = new System.Net.WebClient();
                    byte[] response = wc.DownloadData(fileName);
                    sContents = System.Text.Encoding.ASCII.GetString(response);

                } else {
                    System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
                    sContents = sr.ReadToEnd();
                    sr.Close();
                }
            } catch { sContents = "unable to connect to server "; }
            return sContents;
        }
    }
}
