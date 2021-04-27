using System;
using System.Web.UI;
using System.Threading;
using Timer = System.Web.UI.Timer;
using System.Net.Sockets;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System.Net;
using Newtonsoft.Json;
using System.Timers;
using System.Diagnostics;

namespace VesselFinder
{
    public partial class _Default : Page
    {

        public static Table vfTable;
        public static CancellationTokenSource source = new CancellationTokenSource();
        public static CancellationToken token = source.Token;
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        private static string tableName = "vf-table";
        private static bool operationSucceeded, operationFailed;
        private const int COUNTER_MAX = 10000;
        private const int OUTPUT_FREQUENCY = 1000;
        static int counter = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            vfData();
            
        }

        private void vfData()
        {
            try
            {
                // Iterate counter.
                counter++;

                // Output counter value every so often.
                if (counter % OUTPUT_FREQUENCY == 0)
                {
                    Debug.WriteLine($"Current counter: {counter}.");
                }

                // Check if counter has reached maximum value; if not, allow recursion.
                if (counter <= COUNTER_MAX)
                {
                    // Recursively call self method.
                    WebClient webClient = new WebClient();
                    string myJSON = webClient.DownloadString("");

                    dynamic jsonObj = JsonConvert.DeserializeObject(myJSON);
                    for (int i = 0; i < jsonObj.Count; i++)
                    {
                        Document newItem = new Document();

                        newItem["ID"] = DateTime.Now.ToShortDateString().ToString() + "-" + jsonObj[i]["AIS"]["IMO"].ToString() + "-" + DateTime.Now.ToLongTimeString().ToString() + "-" + jsonObj[i]["AIS"]["MMSI"].ToString();
                        newItem["MMSI"] = jsonObj[i]["AIS"]["MMSI"].ToString();
                        newItem["TIMESTAMP"] = jsonObj[i]["AIS"]["TIMESTAMP"].ToString();
                        newItem["LATITUDE"] = jsonObj[i]["AIS"]["LATITUDE"].ToString();
                        newItem["LONGITUDE"] = jsonObj[i]["AIS"]["LONGITUDE"].ToString();
                        newItem["COURSE"] = jsonObj[i]["AIS"]["COURSE"].ToString();
                        newItem["SPEED"] = jsonObj[i]["AIS"]["SPEED"].ToString();
                        newItem["HEADING"] = jsonObj[i]["AIS"]["HEADING"].ToString();
                        newItem["NAVSTAT"] = jsonObj[i]["AIS"]["NAVSTAT"].ToString();
                        newItem["IMO"] = jsonObj[i]["AIS"]["IMO"].ToString();
                        newItem["NAME"] = jsonObj[i]["AIS"]["NAME"].ToString();
                        newItem["CALLSIGN"] = jsonObj[i]["AIS"]["CALLSIGN"].ToString();
                        newItem["TYPE"] = jsonObj[i]["AIS"]["TYPE"].ToString();
                        newItem["A"] = jsonObj[i]["AIS"]["A"].ToString();
                        newItem["B"] = jsonObj[i]["AIS"]["B"].ToString();
                        newItem["C"] = jsonObj[i]["AIS"]["C"].ToString();
                        newItem["D"] = jsonObj[i]["AIS"]["D"].ToString();
                        newItem["DRAUGHT"] = jsonObj[i]["AIS"]["DRAUGHT"].ToString();
                        newItem["DESTINATION"] = jsonObj[i]["AIS"]["DESTINATION"].ToString();
                        newItem["ETA_AIS"] = jsonObj[i]["AIS"]["ETA_AIS"].ToString();
                        newItem["ETA"] = jsonObj[i]["AIS"]["ETA"].ToString();
                        newItem["SRC"] = jsonObj[i]["AIS"]["SRC"].ToString();
                        newItem["ZONE"] = jsonObj[i]["AIS"]["ZONE"].ToString();
                        newItem["ECA"] = jsonObj[i]["AIS"]["ECA"].ToString();

                        Table vfDataa = Table.LoadTable(client, tableName);

                        vfDataAWS.PutItem(newItem);
                        Debug.WriteLine("Item " + i + " : Created!");
                    }

                    lblMsg.Text = "Data Created Successfully! Next creation: " + DateTime.Now.AddHours(1).ToShortTimeString();
                    Thread.Sleep(3600000); //1 hour
                    vfData();
                }
                else
                {
                    Debug.WriteLine("Recursion halted.");
                }
            }
            catch (StackOverflowException exception)
            {
                Debug.WriteLine(exception);
            }


            
        }


        /*--------------------------------------------------------------------------
    *          createClient
    *--------------------------------------------------------------------------*/
        public static bool createClient(bool useDynamoDBLocal)
        {

            if (useDynamoDBLocal)
            {
                operationSucceeded = false;
                operationFailed = false;

                // First, check to see whether anyone is listening on the DynamoDB local port
                // (by default, this is port 8000, so if you are using a different port, modify this accordingly)
                bool localFound = false;
                try
                {
                    using (var tcp_client = new TcpClient())
                    {
                        var result = tcp_client.BeginConnect("localhost", 8000, null, null);
                        localFound = result.AsyncWaitHandle.WaitOne(3000); // Wait 3 seconds
                        tcp_client.EndConnect(result);
                    }
                }
                catch
                {
                    localFound = false;
                }
                if (!localFound)
                {
                    Debug.WriteLine("\n      ERROR: DynamoDB Local does not appear to have been started..." +
                                      "\n        (checked port 8000)");
                    operationFailed = true;
                    return (false);
                }

                // If DynamoDB-Local does seem to be running, so create a client
                Console.WriteLine("  -- Setting up a DynamoDB-Local client (DynamoDB Local seems to be running)");
                AmazonDynamoDBConfig ddbConfig = new AmazonDynamoDBConfig();
                ddbConfig.ServiceURL = "http://localhost:8000";
                try { client = new AmazonDynamoDBClient(ddbConfig); }
                catch (Exception ex)
                {
                    Debug.WriteLine("     FAILED to create a DynamoDBLocal client; " + ex.Message);
                    operationFailed = true;
                    return false;
                }
            }

            else
            {
                try { client = new AmazonDynamoDBClient(); }
                catch (Exception ex)
                {
                    Debug.WriteLine("     FAILED to create a DynamoDB client; " + ex.Message);
                    operationFailed = true;
                }
            }
            operationSucceeded = true;
            return true;
        }

    }

     
}