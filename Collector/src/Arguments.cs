using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Collector
{
    public class Arguments
    {
        private const string BaseHelpMessage = "Collector Version {0}\n\nUsage: {1} [Options]... [Files]...\n\nThe Collector tool gathers forensic artifacts from hosts with NTFS file systems quickly, securely and minimizes impact to the host.\n\nThe available options are:";
        private static readonly Dictionary<string, string> HelpTopics = new Dictionary<string, string>
        {
            {
                "-od",
                "Defines the directory that the zip archive will be created in. Defaults to current working directory.\nUsage: -od <directory path>"
            },
            {
                "-of",
                "Defines the name of the zip archive will be created. Defaults to host machine's name.\nUsage: -of <archive name>"
            },
            {
                "-c",
                "Optional argument to provide custom list of artifact files and directories (one entry per line). NOTE: Please see CUSTOM_PATH_TEMPLATE.txt for sample.\nUsage: -c <path to config file>"
            },
            {
                "-d",
                "Same as '-c' but will collect default paths included in Collector in addition to those specified in the provided config file.\nUsage: -d <path to config file>"
            },
            {
                "-u",
                "SFTP username"
            },
            {
                "-p",
                "SFTP password"
            },
            {
                "-s",
                "SFTP Server resolvable hostname or IP address and port. If no port is given then 22 is used by default.  Format is <server name>:<port>\n Usage: -s <ip>:<port>"
            },
            {
                "--s3-bucket",
                "The S3 Bucket URL to write output to."
            },
            {
                "--s3-accesskey",
                "The AWS IAM Access Key which has access to upload to the Bucket URL spcified in --s3-bucket"
            },
            {
                "--s3-secret",
                "The AWS IAM Secrey key associated with the access key listed in --s3-accesskey"
            },
            {
                "--s3-region",
                "The region (ie. us-east-1) the S3 Bucket is located in"
            },
            {
                "-os",
                "Defines the output directory on the SFTP server, as it may be a different location than the ZIP generate on disk. Can be full or relative path.\n Usage: -os <directory path>"
            },
            {
                "--no-uploadcleanup",
                "Disables the removal of the .zip file used for collection after uploading. Only applies if SFTP or S3 option is enabled."
            },
            {
                "--dry-run",
                "Collect artifacts to a virtual zip archive, but does not send or write to disk."
            },
            {
                "--force-native",
                "Uses the native file system instead of a raw NTFS read. Unix-like environments always use this option."
            },
            {
                "-zp",
                "Uses a password to encrypt the archive file"
            },
            {
                "-zl",
                "Uses a number between 1-9 to change the compression level of the archive file"
            },
            {
                "--usnjrnl",
                "Enables collecting $UsnJrnl"
            },
            {
                "-l",
                "Sets the file path to write log messages to. Defaults to ./Collector.log\n Usage: -l Collector_run.log"
            },
            {
                "-q",
                "Disables logging to the console and file.\n Usage: -q"
            },
            {
                "-v",
                "Increases verbosity of the console log. By default the console only shows information or greater events and the file log shows all entries. Disabled when `-q` is used.\n Usage: -v"
            }
        };

        public readonly bool HelpRequested;

        public readonly string HelpTopic;

        public readonly string CollectionFilePath = ".";
        public readonly bool CollectDefaults = false;
        public readonly List<string> CollectionFiles = null;
        public readonly string OutputPath = ".";
        public readonly string OutputFileName = $"{Environment.MachineName}.zip";

        public readonly bool UseSftp;
        public readonly string UserName = string.Empty;
        public readonly string UserPassword = string.Empty;
        public readonly string SFTPServer = string.Empty;
        public readonly string SFTPOutputPath = ".";
        public readonly bool UploadCleanUp = true;

        public readonly bool UseS3;
        public readonly string S3Bucket = string.Empty;
        public readonly string S3AccessKey = string.Empty;
        public readonly string S3Secret = string.Empty;
        public readonly string S3Region = string.Empty;

        public readonly bool DryRun;
        public readonly bool ForceNative;
        public readonly string ZipPassword;
        public readonly string ZipLevel;
        public readonly bool Usnjrnl = false;
        public readonly string LogFilePath = "Collector.log";
        public readonly bool EnableVerboseConsole = false;
        public readonly bool DisableLogging = false;

        public Arguments(IEnumerable<string> args)
        {
            ForceNative = !Platform.SupportsRawAccess(); //default this to whether or not the platform supports raw access
            var argEnum = args.GetEnumerator();
            while (!HelpRequested && argEnum.MoveNext())
            {
                switch (argEnum.Current)
                {
                    case "--help":
                    case "-h":
                    case "/?":
                    case "--version":
                        HelpRequested = true;
                        argEnum.GetArgumentParameter(ref HelpTopic);
                        break;

                    case "-od":
                        OutputPath = argEnum.GetArgumentParameter();
                        break;

                    case "-of":
                        OutputFileName = argEnum.GetArgumentParameter();
                        break;

                    case "-u":
                        UserName = argEnum.GetArgumentParameter();
                        break;

                    case "-p":
                        UserPassword = argEnum.GetArgumentParameter();
                        break;

                    case "-s":
                        SFTPServer = argEnum.GetArgumentParameter();
                        break;

                    case "-os":
                        SFTPOutputPath = argEnum.GetArgumentParameter();
                        break;

                    case "--no-uploadcleanup":
                        UploadCleanUp = false;
                        break;

                    case "--s3-bucket":
                        S3Bucket = argEnum.GetArgumentParameter();
                        break;

                    case "--s3-accesskey":
                        S3AccessKey = argEnum.GetArgumentParameter();
                        break;

                    case "--s3-secret":
                        S3Secret = argEnum.GetArgumentParameter();
                        break;

                    case "--s3-region":
                        S3Region = argEnum.GetArgumentParameter();
                        break;

                    case "-c":
                        CollectionFilePath = argEnum.GetArgumentParameter();
                        break;

                    case "-d":
                        CollectionFilePath = argEnum.GetArgumentParameter();
                        CollectDefaults = true;
                        break;

                    case "-zp":
                        ZipPassword = argEnum.GetArgumentParameter();
                        break;

                    case "-zl":
                        ZipLevel = argEnum.GetArgumentParameter();
                        break;

                    case "-l":
                        LogFilePath = argEnum.GetArgumentParameter();
                        break;

                    case "-q":
                        DisableLogging = true;
                        break;

                    case "-v":
                        EnableVerboseConsole = true;
                        break;

                    case "--usnjrnl":
                        Usnjrnl = true;
                        break;

                    case "--force-native":
                        if (ForceNative)
                        {
                            Console.WriteLine("Warning: This platform only supports native reads, --force-native has no effect.");
                        }
                        ForceNative = true;
                        break;

                    case "--dry-run":
                        DryRun = true;
                        break;

                    case "-o":
                        throw new ArgumentException("-o is no longer supported, please use -od instead.");

                    default:
                        CollectionFiles = CollectionFiles ?? new List<string>();
                        CollectionFiles.Add(argEnum.Current);
                        break;
                }
            }

            if (!HelpRequested)
            {
                var sftpArgs = new[] { UserName, UserPassword, SFTPServer };
                UseSftp = sftpArgs.Any(arg => !string.IsNullOrEmpty(arg));
                if (UseSftp && sftpArgs.Any(string.IsNullOrEmpty))
                {
                    throw new ArgumentException("The flags -u, -p, and -s must all have values to continue.  Please try again.");
                }

                if (DryRun)
                {
                    //Disable SFTP in a dry run.
                    UseSftp = false;
                }
            }

            if (!HelpRequested)
            {
                var s3Args = new[] { S3Bucket, S3AccessKey, S3Secret, S3Region };
                UseS3 = s3Args.Any(arg => !string.IsNullOrEmpty(arg));
                if (UseS3 && s3Args.Any(string.IsNullOrEmpty))
                {
                    throw new ArgumentException("The flags --s3-bucket, --s3-region, --s3-accesskey, and --s3-secret must all have values to continue.  Please try again.");
                }

                if (DryRun)
                {
                    //Disable S3 in a dry run.
                    UseS3 = false;
                }
            }
        }

        public string GetHelp(string topic)
        {
            string help;
            if (string.IsNullOrEmpty(topic))
            {
                var helpText = new StringBuilder(string.Format(BaseHelpMessage, Version.GetVersion(), AppDomain.CurrentDomain.FriendlyName)).AppendLine();
                foreach (var command in HelpTopics)
                {
                    helpText.AppendLine(command.Key).AppendLine("\t" + command.Value).AppendLine();
                }
                help = helpText.ToString();
            }
            else if (!HelpTopics.TryGetValue(topic, out help))
            {
                help = $@"{topic} is not a valid argument.";
            }
            return help;
        }
    }

    internal static class ArgumentExtentions
    {
        public static string GetArgumentParameter(this IEnumerator<string> arguments)
        {
            var currentArg = arguments.Current;
            var hasArg = arguments.MoveNext();
            if (!hasArg)
            {
                throw new ArgumentException(
                    $"Argument '{currentArg}' had no parameters. Use '--help {currentArg}' for usage details.");
            }

            return arguments.Current;
        }

        public static bool GetArgumentParameter(this IEnumerator<string> arguments, ref string parameter)
        {
            var hasArg = arguments.MoveNext();

            if (hasArg)
            {
                parameter = arguments.Current;
            }

            return hasArg;
        }
    }
}