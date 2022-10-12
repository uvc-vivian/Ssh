// See https://aka.ms/new-console-template for more information
using System;
using System.Text.RegularExpressions;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using Renci.SshNet.Common;

namespace SshNet
{
    public class Program
    {
        static void Main()
        {
            //Doosan config
            string doosanAddress = "10.20.193.62";
            int doosanPort = 4052;
            string doosanUser = "advantech";
            string doosanPw = "opcua5497";
            string doosanPath = "/home/advantech/run.sh";
            AccessSsh(doosanAddress, doosanPort, doosanUser, doosanPw, doosanPath);

            //KUKA config
            string kukaAddress = "10.20.193.101";
            int kukaPort = 22;
            string kukaUser = "hyundai";
            string kukaPw = "opcua5497";
            string kukaPath = "/home/hyundai/Hyundai-KUKA/run.sh";
            AccessSsh(kukaAddress, kukaPort, kukaUser, kukaPw, kukaPath);

        }

        //Access SSH & Run SmartConnector (run.sh) 
        static void AccessSsh(string address, int port, string user, string password, string path)
        {
            try
            {
                //create new client & session
                SshClient client = new SshClient(address, port, user, password);
                client.Connect();

                //create terminal -> used by ShellStream
                //create a dictionary of terminal modes & add terminal mode
                IDictionary<TerminalModes, uint> termkvp = new Dictionary<TerminalModes, uint>();
                termkvp.Add(TerminalModes.ECHO, 53);

                //execute start.sh script
                ShellStream shellStream = client.CreateShellStream("xterm", 80, 24, 800, 600, 1024, termkvp);
                var output = shellStream.Expect(new Regex(@"[$>]"));
                shellStream.WriteLine($"sudo sh {path}");
                output = shellStream.Expect(new Regex(@"([$#>:])"));
                shellStream.WriteLine(password);

                string line;
                while((line = shellStream.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
        }
        /*
        static void AccessKUKASsh(string address, int port, string user, string password)
        {
            try
            {
                //create new client & session
                SshClient client = new SshClient(address, port, user, password);
                client.Connect();

                //create terminal -> used by ShellStream
                //create a dictionary of terminal modes & add terminal mode
                IDictionary<TerminalModes, uint> termkvp = new Dictionary<TerminalModes, uint>();
                termkvp.Add(TerminalModes.ECHO, 53);

                //execute start.sh script
                ShellStream shellStream = client.CreateShellStream("xterm", 80, 24, 800, 600, 1024, termkvp);
                var output = shellStream.Expect(new Regex(@"[$>]"));
                //shellStream.WriteLine("cd /home/edge/Huyundai/Doosan");
                //shellStream.WriteLine("ls");
                shellStream.WriteLine("sudo sh /home/hyundai/Hyundai-KUKA/run.sh");
                output = shellStream.Expect(new Regex(@"([$#>:])"));
                shellStream.WriteLine(password);

                string line;
                while ((line = shellStream.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
        } */
    }
}
