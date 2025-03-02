using System.Diagnostics;
using System.Xml;
using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace Free_MS_Store_Apps
{
    public partial class Form1 : Form
    {
        private string defaultAppsDir = @"C:/Program Files/WindowsApps";

        private string AppName = "Windows Store Poc";
        private string AppCreator = "Dev Goyal & Jayant Khandelwal";
        private string AppCurrentDirctory = Directory.GetCurrentDirectory();
        private string WSAppXmlFile = "AppxManifest.xml";
        private bool Checking = true;
        private string WSAppName;
        private string WSAppPath;
        private string WSAppVersion;
        private string WSAppFileName;
        private string WSAppOutputPath;
        private string WSAppProcessorArchitecture;
        private string WSAppPublisher;
        private List<FileEntry> fileEntries;


        public Form1()
        {
            InitializeComponent();
            InitializeListView();
            FolderPermi();
        }


        private void FolderPermi()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(defaultAppsDir);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();

            directorySecurity.SetOwner(WindowsIdentity.GetCurrent().User);
            directoryInfo.SetAccessControl(directorySecurity);
        }




        async private static Task<string> RunProcess(string fileName, string args, string input = null)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = false
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                var outputBuilder = new StringBuilder();
                var hasInput = !string.IsNullOrEmpty(input);

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        outputBuilder.AppendLine(e.Data);
                    }
                };

                process.Start();

                if (hasInput)
                {
                    using (var sw = process.StandardInput)
                    {
                        sw.WriteLine(input);
                    }
                }

                process.BeginOutputReadLine();
                await process.WaitForExitAsync();

                //MessageBox.Show(outputBuilder.ToString());

                return outputBuilder.ToString();
            }
        }




        private void StartPoc(string file_name)
        {
            var file_parts = file_name.Split("_");
            var file_parts_e = file_name.Split(".");
            file_name = file_name.Replace("." + file_parts_e[file_parts_e.Length - 1],"");
            var ext = file_parts[0];
            var directories = Directory.GetDirectories(defaultAppsDir,
                   ext  +  "*",
                    SearchOption.TopDirectoryOnly);

            //MessageBox.Show(String.Join(",", directories));
            var directory = "";

            foreach (string dir in directories)
            {
                if (dir.Contains("x64"))
                {
                    directory = dir;
                    break;
                }
            }

            if (directory == "")
            {
                foreach (string dir in directories)
                {
                    if (File.Exists(dir + "/" + WSAppXmlFile))
                    {
                        directory = dir;
                        break;
                    }
                }
            }





            label3.Text = "Starting Poc";

            WSAppPath = directory;

            if (WSAppPath.Contains("\""))
            {
                WSAppPath = WSAppPath.Replace("\"", "");
                WSAppPath = "\"" + WSAppPath + "\"";
            }
            else if (File.Exists(WSAppPath + "\\" + WSAppXmlFile))
            {

                WSAppOutputPath = AppCurrentDirctory;

                if (Directory.Exists(WSAppOutputPath))
                {
                    WSAppFileName = file_name;
                    using (XmlReader xmlReader = XmlReader.Create(WSAppPath + "\\" + WSAppXmlFile))
                    {
                        while (xmlReader.Read())
                        {
                            if (xmlReader.IsStartElement() && xmlReader.Name == "Identity")
                            {
                                string str1 = xmlReader["Name"];
                                if (str1 != null)
                                    WSAppName = str1;
                                string str2 = xmlReader["Publisher"];
                                if (str2 != null)
                                    WSAppPublisher = str2;
                                string str3 = xmlReader["Version"];
                                if (str3 != null)
                                    WSAppVersion = str3;
                                string str4 = xmlReader["ProcessorArchitecture"];
                                if (str4 != null)
                                    WSAppProcessorArchitecture = str4;

                            }
                        }
                    }

                    MakeAppx();
                }
                else
                {
                    Checking = true;
                    MessageBox.Show($"Invailed Output Path, {(object)WSAppOutputPath} Directory not found!");
                
            }
        }
                else
                {
                    Checking = true;
                    MessageBox.Show($"Invailed App Path, {(object)WSAppXmlFile} file not found! {WSAppPath}");
                }
}

        async private void MakeAppx()
        {
            string str = AppCurrentDirctory + "\\utils\\makeappx.exe";
            string args = "pack -d \"" + WSAppPath + "\" -p \"" + WSAppOutputPath + "\\" + WSAppFileName + ".appx\" -l";
            if (File.Exists(str))
            {
                if (File.Exists(WSAppOutputPath + "\\" + WSAppFileName + ".appx"))
                    File.Delete(WSAppOutputPath + "\\" + WSAppFileName + ".appx");
                label3.Text = "Creating '.appx' package file.";

                string runres = await RunProcess(str, args);
                if (runres.Length != 0)
                {
                   label3.Text = $".appx created succeeded.";
                   MakeCert();
                }
                else
                {
                    Checking = false;
                    MessageBox.Show($"Package {(object)(WSAppFileName)}.appx creation failed");
                }
            }
            else
            {
                Checking = false;
                MessageBox.Show("'MakeAppx.exe' file not found!");
            }
        }

        async private void MakeCert()
        {
            string str = AppCurrentDirctory + "\\utils\\makecert.exe";
            string args = "-n \"" + WSAppPublisher + "\" -r -a sha256 -len 2048 -cy end -h 0 -eku 1.3.6.1.5.5.7.3.3 -b 01/01/2000 -sv \"" + WSAppOutputPath + "\\" + WSAppFileName + ".pvk\" \"" + WSAppOutputPath + "\\" + WSAppFileName + ".cer\"";
            if (File.Exists(str))
            {
                if (File.Exists(WSAppOutputPath + "\\" + WSAppFileName + ".pvk"))
                    File.Delete(WSAppOutputPath + "\\" + WSAppFileName + ".pvk");
                if (File.Exists(WSAppOutputPath + "\\" + WSAppFileName + ".cer"))
                    File.Delete(WSAppOutputPath + "\\" + WSAppFileName + ".cer");
                 label3.Text = "Creating certificate..";
                Console.Write("Certificate creation: ");
                string runres = await RunProcess(str, args);

                if (runres.Length != 0)
                {
                   /* while (Checking)*/
                        Pvk2Pfx();
                }
                else
                {
                    Checking = false;
                    MessageBox.Show($"Failed to create Certificate for the package");
                }
            }
            else
            {
                Checking = false;
                MessageBox.Show($"'MakeCert.exe' file not found!");
            }
        }

        async private void Pvk2Pfx()
        {
            string str = AppCurrentDirctory + "\\utils\\pvk2pfx.exe";
            string args = "-pvk \"" + WSAppOutputPath + "\\" + WSAppFileName + ".pvk\" -spc \"" + WSAppOutputPath + "\\" + WSAppFileName + ".cer\" -pfx \"" + WSAppOutputPath + "\\" + WSAppFileName + ".pfx\"";
            if (File.Exists(str))
            {
                if (File.Exists(WSAppOutputPath + "\\" + WSAppFileName + ".pfx"))
                    File.Delete(WSAppOutputPath + "\\" + WSAppFileName + ".pfx");
                Console.WriteLine("\nPlease wait.. Converting certificate to sign the package.\n");
                Console.Write("Certificate convertion: ");
                label3.Text = "convertion of certificate..";

                string runres = await RunProcess(str, args);

                if (runres.Length == 0)
                {
                    Console.Write("succeeded");
                /*    while (Checking)*/
                        SignApp();
                }
                else
                {
                    Checking = false;
                    MessageBox.Show("Can't convert certificate!");
                }
            }
            else
            {
                Checking = false;
                MessageBox.Show("'Pvk2Pfx.exe' file not found!");
            }
        }

       async private void SignApp()
        {
            string str = AppCurrentDirctory + "\\utils\\signtool.exe";
            string args = "sign -fd SHA256 -a -f \"" + WSAppOutputPath + "\\" + WSAppFileName + ".pfx\" \"" + WSAppOutputPath + "\\" + WSAppFileName + ".appx\"";
            if (File.Exists(str))
            {
                Console.WriteLine("\n\nPlease wait.. Signing the package, this may take some minutes.\n");
                string runres = await RunProcess(str, args);

                if (runres.ToLower().Contains("successfully signed"))
                {
                    Checking = false;
                    Console.WriteLine("Package signing succeeded. Please install the '.cer' file to [Local Computer\\Trusted Root Certification Authorities] before install the App Package or use 'WSAppPkgIns.exe' file to install the App Package!");
                   
                    RemovePackage(WSAppFileName);

                    X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                    store.Open(OpenFlags.ReadWrite);
                    store.Add(new X509Certificate2(X509Certificate.CreateFromCertFile(WSAppFileName + ".cer")));
                    store.Close();

                    InstallPackage(WSAppFileName + ".appx",false);
                    DeletePackageFiles(WSAppFileName);
                }
                else
                {
                    Checking = false;
                    MessageBox.Show("Can't Sign the package...");
                }
            }
            else
            {
                Checking = false;
                MessageBox.Show("Can't Sign the package, 'SignTool.exe' file not found!");

            }
        }



        async private Task<string> getPackagesResponse(string url)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.AutomaticDecompression = System.Net.DecompressionMethods.All;

            HttpClient client = new HttpClient(handler);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://store.rg-adguard.net/api/GetFiles");
            request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
            request.Content = new StringContent("type=url&url=" + url);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            progressBar1.Value = 35;
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            progressBar1.Value = 100;


            return responseBody;
        }


        private void InitializeListView()
        {
            // Set the view to Details mode
            listView1.View = View.Details;

            // Add columns to the ListView
            listView1.Columns.Add("File", 200);
            listView1.Columns.Add("Size", 100);


        }

        private void AddCustomListItem(string file, string size, Color textColor, int fontSize, int spacing)
        {
            ListViewItem item = new ListViewItem(file);
            item.SubItems.Add(size);
            item.ForeColor = textColor;
            item.Font = new Font(listView1.Font.FontFamily, fontSize);
            item.UseItemStyleForSubItems = false;

            for (int i = 1; i < item.SubItems.Count; i++)
            {
                item.SubItems[i].Text = " " + item.SubItems[i].Text;
                item.SubItems[i].Font = new Font(item.Font.FontFamily, fontSize, FontStyle.Bold);

            }

            listView1.Items.Add(item);
        }

        async private void button1_Click(object sender, EventArgs e)
        {
            //fetching packages
            var pakUrl = textBox1.Text;
            var data = await getPackagesResponse(pakUrl);
            listView1.Items.Clear();


            if (data != null)
            {
                listView1.FullRowSelect = true;
                fileEntries = FileEntityTools.ExtractFileEntries(data);

                foreach (var entry in fileEntries)
                {
                    /*listView1 = new ListView();*/
                    if (entry.File.Contains(".msixbundle"))
                    {
                        AddCustomListItem(entry.File, entry.Size, Color.DarkGreen, 10, 20);
                    }
                    AddCustomListItem(entry.File, entry.Size, Color.Black, 10, 20);
                }

                listView1.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
            }
            else
            {

            }

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        async private void listView1_DblClickChanged(object sender, EventArgs e)
        {
            var index = listView1.SelectedIndices[0];
            if (index == -1)
            {
                return;
            }

            var downloadFileUrl = fileEntries[index].DownloadUrl;
            var file_name = fileEntries[index].File;

            var destinationFilePath = Path.GetFullPath(file_name);
            label3.Text = "Downloading...";

            using (var client = new HttpClientDownloadWithProgress(downloadFileUrl, destinationFilePath))
            {
                client.ProgressChanged += async (totalFileSize, totalBytesDownloaded, progressPercentage) =>
                {
                    progressBar1.Value = (int)progressPercentage;
                    label3.Text = $"Downloading {progressPercentage}%";

                    if(progressPercentage == 100)
                    {
                        progressBar1.Value = 0;

                        /*                        setTimeout(() => { InstallPackage(file_name); }, 2);
                        */
                        await Task.Delay(TimeSpan.FromSeconds(3));

                        InstallPackage(file_name);

                    }

                    Console.WriteLine($"{progressPercentage}% ({totalBytesDownloaded}/{totalFileSize})");
                };

                await client.StartDownload();
            }

        }

        public void setTimeout(Action TheAction, int Timeout)
        {
            Thread t = new Thread(
                () =>
                {
                    Thread.Sleep(Timeout);
                    TheAction.Invoke();
                }
            );
            t.Start();
        }




        private void DeletePackageFiles(string fileName)
        {
            //delete the file

            if (File.Exists(AppCurrentDirctory+ "\\" + fileName + ".cer"))
            {
                File.Delete(AppCurrentDirctory + "\\" + fileName + ".cer");
            }
            if (File.Exists(AppCurrentDirctory + "\\" + fileName + ".pvk"))
            {
                File.Delete(AppCurrentDirctory + "\\" + fileName + ".pvk");
            }

            if (File.Exists(AppCurrentDirctory + "\\" + fileName + ".pfx"))
            {
                File.Delete(AppCurrentDirctory + "\\" + fileName + ".pfx");
            }

            if (File.Exists(AppCurrentDirctory + "\\" + fileName + ".appx"))
            {
                File.Delete(AppCurrentDirctory + "\\" + fileName + ".appx");
            }

        }

        private void RemovePackage(string fileName)
        {
            try
            {
                label3.Text = "Removing App..";
                string powershellCommand = $"Remove-AppxPackage -package '{fileName}'";

                ProcessStartInfo psi = new ProcessStartInfo()
                {
                    FileName = "powershell",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{powershellCommand}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                //psi.RedirectStandardOutput = true;

                using (Process process = new Process())
                {
                    process.StartInfo = psi;

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.WriteLine("Output: " + e.Data);
                            MessageBox.Show(e.Data + " DONE");
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            MessageBox.Show(e.Data);
                            Console.WriteLine("Error: " + e.Data);
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    Console.WriteLine("PowerShell process exited with code: " + process.ExitCode);
                    //success install
                    //StartPoc(fileName);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred: {ex.Message}");
                MessageBox.Show(ex.Message);
            }
            label3.Text = "";
        }



        private void InstallPackage(string fileName,bool fromStart= true)
        {
            try
            {
                string appFilePath = AppCurrentDirctory + "/" +  fileName; // Replace with the actual path
                label3.Text = "Installing App..";

                string powershellCommand = $"Add-AppxPackage -Path '{appFilePath}'";

                ProcessStartInfo psi = new ProcessStartInfo()
                {
                    FileName = "powershell",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{powershellCommand}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (Process process = new Process())
                {
                    process.StartInfo = psi;

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.WriteLine("Output: " + e.Data);
                            MessageBox.Show(e.Data + " DONE");
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            MessageBox.Show(e.Data);
                            Console.WriteLine("Error: " + e.Data);
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    Console.WriteLine("PowerShell process exited with code: " + process.ExitCode);
                    //success install
                    if (fromStart)
                    {
                    StartPoc(fileName);
                    }
                }
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"An error occurred: {ex.Message}");
                MessageBox.Show(ex.Message);
            }
            label3.Text = "";
       }
    }
}