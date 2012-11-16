using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CurrentCostAgent
{
    public delegate void NotifyUploadHandler();
    public delegate void NewReadingHandler(SensorReading reading);

    public partial class MainApplication : Component
    {
        private string _comPort;
        private int _baudRate;
        private string _cosmKey;
        private string _cosmFeedId;
        private string _plotWattKey;

        private const int UploadFrequency = 30;

        private ConcurrentQueue<SensorReading> readings = new ConcurrentQueue<SensorReading>();
        private StatusForm _statusForm = new StatusForm();
        Thread _readerThread;
        Thread _postingThread;
        bool _exit = false;

        public MainApplication()
        {
            InitializeComponent();
        }

        public MainApplication(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
            Environment.Exit(0);
        }

        public void Start()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _comPort = ConfigurationManager.AppSettings["ComPort"];
            _baudRate = int.Parse(ConfigurationManager.AppSettings["BaudRate"]);
            _cosmKey = ConfigurationManager.AppSettings["CosmKey"];
            _cosmFeedId = ConfigurationManager.AppSettings["CosmFeedId"];
            _plotWattKey = ConfigurationManager.AppSettings["PlotWattKey"];
            _statusForm.StartPosition = FormStartPosition.Manual;
            _statusForm.Top = Screen.PrimaryScreen.WorkingArea.Bottom - _statusForm.Height;
            _statusForm.Left = Screen.PrimaryScreen.WorkingArea.Right - _statusForm.Width;
            _statusForm.Show();
            _readerThread = new Thread(new ThreadStart(SerialReader));
            _readerThread.IsBackground = false;
            _readerThread.Start();
            _postingThread = new Thread(new ThreadStart(QueueUploader));
            _postingThread.IsBackground = false;
            _postingThread.Start();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //absorb
        }

        private void QueueUploader()
        {
            DateTime nextUpload = DateTime.Now.AddSeconds(UploadFrequency);
            while (true)
            {
                try
                {
                    Thread.Sleep(1000);
                    if (_exit)
                    {
                        return;
                    }
                    if (DateTime.Now < nextUpload)
                    {
                        continue;
                    }
                    nextUpload = DateTime.Now.AddSeconds(UploadFrequency);
                    List<SensorReading> newReadings = new List<SensorReading>();
                    SensorReading newReading;
                    while (readings.TryDequeue(out newReading))
                    {
                        newReadings.Add(newReading);
                    }

                    if (newReadings.Count == 0)
                    {
                        continue;
                    }

                    PostToPlotWatt(newReadings);

                    PostToCosm(newReadings);

                    _statusForm.Invoke(new NotifyUploadHandler(_statusForm.NotifyUpload));

                }
                catch
                {
                    //catch and absorb and never die
                    Thread.Sleep(10000);
                }
            }

        }

        private void PostToCosm(List<SensorReading> newReadings)
        {
            if (string.IsNullOrWhiteSpace(_cosmKey))
            {
                return;
            }
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Dictionary<int, JArray> feeds = new Dictionary<int, JArray>();
            foreach (var reading in newReadings)
            {
                if (!feeds.ContainsKey(reading.Sensor))
                {
                    feeds.Add(reading.Sensor, new JArray());
                }
                JObject o = new JObject();
                o["at"] = epoch.AddSeconds(reading.Timestamp).ToString("o");
                o["value"] = (int)reading.Watts;
                feeds[reading.Sensor].Add(o);
            }

            WebClient cosmClient = new WebClient();
            foreach (var subFeedId in feeds.Keys)
            {
                string cosmUrl = string.Format("http://api.cosm.com/v2/feeds/{0}/datastreams/{1}/datapoints?key={2}", _cosmFeedId, subFeedId, _cosmKey);
                JObject cosm = new JObject();
                cosm["datapoints"] = feeds[subFeedId];
                string cosmData = cosm.ToString();
                cosmClient.UploadString(cosmUrl, cosmData);
            }
        }

        private void PostToPlotWatt(List<SensorReading> newReadings)
        {
            if (string.IsNullOrWhiteSpace(_plotWattKey))
            {
                return;
            }
            //first check how many meters are on the account already and create new ones if necessary
            int maxSensorReadings = newReadings.Max(sr => sr.Sensor) + 1;
            WebClient client = new WebClient();
            string authInfo = _plotWattKey + ":";
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            client.Headers["Authorization"] = "Basic " + authInfo;
            string rawMeters = client.DownloadString("http://plotwatt.com/api/v2/list_meters").Trim().Trim('[', ']');
            string[] meters = rawMeters.Split(',');
            if (string.IsNullOrWhiteSpace(rawMeters) || meters.Length < maxSensorReadings)
            {
                int numberToCreate;
                if (string.IsNullOrWhiteSpace(rawMeters))
                {
                    numberToCreate = maxSensorReadings;
                }
                else
                {
                    numberToCreate = maxSensorReadings - meters.Length;
                }
                client.UploadString("http://plotwatt.com/api/v2/new_meters", "number_of_new_meters=" + numberToCreate.ToString());
                rawMeters = client.DownloadString("http://plotwatt.com/api/v2/list_meters").Trim().Trim('[', ']');
                meters = rawMeters.Split(',');
            }

            //now build the upload and post the readings
            List<string> readingsToPost = new List<string>();
            foreach (var reading in newReadings)
            {
                readingsToPost.Add(string.Format("{0},{1},{2}", meters[reading.Sensor], reading.Watts / 1000.0, reading.Timestamp));
            }

            Debug.WriteLine("Uploading {0} readings.", readingsToPost.Count);

            string postData = string.Join(",", readingsToPost);
            client.UploadString("http://plotwatt.com/api/v2/push_readings", postData);
        }


        private void SerialReader()
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            while (true)
            {
                try
                {
                    using (SerialPort port = new SerialPort())
                    {
                        port.PortName = _comPort;
                        port.BaudRate = _baudRate;
                        port.DtrEnable = true;
                        port.ReadTimeout = 5000;
                        port.Open();
                        while (true)
                        {
                            try
                            {
                                if (_exit)
                                {
                                    return;
                                }
                                string line = port.ReadLine();
                                //Debug.WriteLine(line);
                                XDocument doc = XDocument.Parse(line, LoadOptions.None);
                                var msg = doc.Element("msg");
                                if (msg != null)
                                {
                                    SensorReading reading = new SensorReading();
                                    var ch1 = msg.Element("ch1");
                                    if (ch1 != null)
                                    {
                                        reading.Timestamp = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
                                        reading.Sensor = int.Parse(msg.Element("sensor").Value);
                                        reading.Watts = int.Parse(ch1.Element("watts").Value);
                                        var ch2 = msg.Element("ch2");
                                        if (ch2 != null)
                                        {
                                            reading.Watts += int.Parse(ch2.Element("watts").Value);
                                        }
                                        var ch3 = msg.Element("ch3");
                                        if (ch3 != null)
                                        {
                                            reading.Watts += int.Parse(ch3.Element("watts").Value);
                                        }
                                        readings.Enqueue(reading);
                                        Debug.WriteLine("time={0}, sensor={1}, watts={2}", reading.Timestamp, reading.Sensor, reading.Watts);

                                        _statusForm.Invoke(new NewReadingHandler(_statusForm.NewReading), reading);
                                    }
                                }
                            }
                            catch (TimeoutException timeout)
                            {
                                //absorb timeouts and try again
                            }
                        }

                    }
                }
                catch
                {
                    //absorb and never die, but at least pause
                    Thread.Sleep(3000);
                }
            }
        }

        private void statusMenuItem_Click(object sender, EventArgs e)
        {
            _statusForm.Show();
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _statusForm.Show();
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                _statusForm.Show();
            }
        }
    }
}
