using DVJUCSVConverterService;
using PDFToDJVU;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace AutoDJVUConverter
{

    partial class DJVUCreationService : ServiceBase
    {
        CancellationTokenSource source;
        Config config;
        List<string> processedFiles;
        public DJVUCreationService()
        {
            InitializeComponent();
            source = new CancellationTokenSource();
            EventLog.Source = "AutoDJVUConverter";
            EventLog.Log = "AutoDJVUConverter";
            ((ISupportInitialize)(EventLog)).BeginInit();
            if (!EventLog.SourceExists(EventLog.Source))
            {
                EventLog.CreateEventSource(EventLog.Source, EventLog.Log);
            }
            ((ISupportInitialize)(EventLog)).EndInit();
            EventLogger.InitLogger(EventLog);
        }

        protected override void OnStart(string[] args)
        {
            string configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), "config.config").Replace("file:\\", "");
            try
            {
                config = Serializer.DeserializeItem<Config>(configPath);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Message);
            }
            if (config == null)
            {
                EventLogger.LogMessage($"Config is not initialized. Please rewrite the config in {configPath}", EventLogEntryType.Warning);
                config = new Config();
                Serializer.SerializeItem(config, configPath);
                return;
            }
            try
            {
                processedFiles = new List<string>();
                List<string> filesToProcess = new List<string>();
                if (File.Exists(config.ProcessedCSVs))
                    processedFiles.AddRange(Serializer.DeserializeItem<List<string>>(config.ProcessedCSVs));

                ParallelOptions options = new ParallelOptions() { CancellationToken = source.Token, MaxDegreeOfParallelism = 3 };
                SHA256 sha = SHA256.Create();
                while (!source.IsCancellationRequested)
                {
                    var ExistingFiles = Directory.GetFiles(config.Hotfolder);
                    List<string> set = new List<string>(ExistingFiles.Select(s => BitConverter.ToString(sha.ComputeHash(Encoding.Default.GetBytes(s)))));
                    foreach (var exFile in ExistingFiles)
                    {
                        var hash = BitConverter.ToString(sha.ComputeHash(Encoding.Default.GetBytes(exFile)));
                        if (!set.Contains(hash))
                            filesToProcess.Add(exFile);
                    }
                    Parallel.ForEach(filesToProcess, options, file =>
                    {
                        XmlDocument document = new XmlDocument();
                        document.Load(file);
                        if (true)
                        {
                            processedFiles.Add(file);
                            filesToProcess.Remove(file);
                        }
                    });
                    Serializer.SerializeItem(processedFiles, config.ProcessedCSVs);
                    Thread.Sleep(1000);
                    if (filesToProcess.Count == 0)
                        for (int i = 0; i < 6; i++)
                        {
                            if (!source.IsCancellationRequested)
                                Thread.Sleep(10000);
                            else break;
                        }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Message + "\r\n" + ex.StackTrace, EventLogEntryType.Error);
            }
        }

        protected override void OnStop()
        {
            source.Cancel();
            Serializer.SerializeItem(processedFiles, config.ProcessedCSVs);
        }
        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        public override EventLog EventLog => base.EventLog;

        public void Start()
        {
            OnStart(null);
        }

    }
}
