using CodeMigrationValidator.DataContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeMigrationValidator.DataConnectors
{
    /// <summary>
    /// This class crawls through known build directories and script folders to build a snapshot of what is expected to be in the CMT.
    /// </summary>
    public class ExpectedCMTCrawler : ICMTCrawler
    {
        private static AppSettings _appSettings;

        public ICMTCrawler Init(AppSettings appSettings)
        {
            _appSettings = appSettings;

            return this;
        }

        public FoundationCodeMigration GetCodeMigration()
        {
            return new FoundationCodeMigration()
            {
                Contents  = GetContents(),
                WorkItems = GetWorkItems()
            };
        }

        private List<WorkItem> GetWorkItems()
        {
            return _appSettings.FeatureNumbers
                               .Select( f => new WorkItem() 
                                                 { 
                                                    Id = f 
                                                 }
                               ).ToList();
        }

        private List<CMTItem> GetContents()
        {
            var result = new List<CMTItem>();
            //Get Builds for C# Server Projects

            var serverProjects = GetRecentlyUpdatedBuildFolders(_appSettings.CodeMigrationBeginDate,
                                                                _appSettings.CodeMigrationEndDate,
                                                                _appSettings.CodeBuildBaseDirectory,
                                                                _appSettings.QAEnvironmentId,
                                                                "Server",
                                                                _appSettings.ServerBuildProjectInclusions);

            result.AddRange(Translate(Constants.CONTENT_TYPE_ID_WEB_SERVICE, serverProjects));

            //Get Builds for C# Client Projects

            var clientProjects = GetRecentlyUpdatedBuildFolders(_appSettings.CodeMigrationBeginDate,
                                                                _appSettings.CodeMigrationEndDate,
                                                                _appSettings.CodeBuildBaseDirectory,
                                                                _appSettings.QAEnvironmentId,
                                                                "Client",
                                                                _appSettings.DesktopClientBuildProjectInclusions);

            result.AddRange(Translate(Constants.CONTENT_TYPE_ID_APPLICATION, clientProjects));

            //Get SQL Scripts

            var sqlScriptDirs = GetDatabaseScriptsForFeatures(_appSettings.SQLScriptsBaseDirectory,
                                                              _appSettings.FeatureNumbers,
                                                              "sql");

            result.AddRange(Translate(Constants.CONTENT_TYPE_ID_DATABASE, sqlScriptDirs));

            //Get Mongo Scripts

            var mongoScriptDirs = GetDatabaseScriptsForFeatures(_appSettings.MongoScriptsBaseDirectory,
                                                                _appSettings.FeatureNumbers,
                                                                "js");

            result.AddRange(Translate(Constants.CONTENT_TYPE_ID_CLOUD_DATABASE, mongoScriptDirs));

            return result;
        }


        /// <summary>
        /// The Build Environment is part of the folder name of a built project 
        /// </summary>
        /// <param name="qaEnvironmentId"></param>
        /// <returns></returns>
        private string ResolveBuildEnvironment(string qaEnvironmentId)
        {
            if (Constants.QA_ENVIRONMENT_BUILD_FOLDERS.ContainsKey(qaEnvironmentId))
                return Constants.QA_ENVIRONMENT_BUILD_FOLDERS[qaEnvironmentId];

            throw new Exception($"Unable to resolve qaEnvironmentId {qaEnvironmentId}");
        }

        private List<string> GetRecentlyUpdatedBuildFolders(DateTime codeMigrationStartDate, DateTime codeMigrationEndDate, string rootPath, string qaEnvironmentId, string projectType, List<string> projectInclusions)
        {
            var result = new List<string>();

            //Resolve CMTEnvironment from QAEnvironmentId
            var cmtEnvironment = ResolveBuildEnvironment(qaEnvironmentId);

            //1. Get Subdirectories that match our file name pattern and are the same or newer than the code migration date
            var directory = new DirectoryInfo(rootPath);

            var projectBuildMainFoldersInEnvironment = directory.GetDirectories($"*{cmtEnvironment}.{projectType}*")
                                                                .Where(f => f.LastWriteTime >= codeMigrationStartDate && f.LastWriteTime <= codeMigrationEndDate);

            //2. Get Sub Directories which match the inclusion list. This gives us the list of "Projects that were modified" .
            //   We have to dig into each of these to find the latest sub folder that was modified.

            var projectBuildsRecentlyModified = new List<DirectoryInfo>();

            foreach (var projectBuildMainFolder in projectBuildMainFoldersInEnvironment)
            {
                var x = projectBuildMainFolder.LastWriteTime;
                foreach (var project in projectInclusions)
                {
                    if (projectBuildMainFolder.Name.Contains(project))
                    {
                        projectBuildsRecentlyModified.Add(projectBuildMainFolder);
                        break;
                    }
                }
            }

            foreach (var project in projectBuildsRecentlyModified)
            {
                var latestBuild = project.GetDirectories($"{project.Name}*")
                                 .OrderByDescending(f => f.LastWriteTime)
                                 .FirstOrDefault();

                if (latestBuild == null)
                {
                    result.Add($"No build found for {project.Name}");
                }
                else
                {
                    result.Add(latestBuild.FullName);
                }
            }

            return result;
        }
        private List<string> GetDatabaseScriptsForFeatures(string rootPath, List<int> featureNumbers, string fileTypeExtension)
        {
            var result = new List<string>();

            var subDirectories = Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories).ToList();

            var subDirectoriesInSprint = new List<string>();

            foreach (var sub in subDirectories)
            {
                foreach (var featureNumber in featureNumbers)
                {
                    if (sub.Contains(featureNumber.ToString()))
                    {
                        subDirectoriesInSprint.Add(sub);
                        break;
                    }
                }
            }

            //Only add directories that contain the expected file type extension inside it
            foreach (var sub in subDirectoriesInSprint)
            {
                var scriptsInDirectory = Directory.EnumerateFiles(sub, $"*.{fileTypeExtension}", SearchOption.AllDirectories);

                if (scriptsInDirectory.Count() >= 1)
                {
                    result.Add(sub);
                }
            }

            return result;
        }

        /// <summary>
        /// Translate a list of directories into CM Items so they can be compared to items from the real CM
        /// </summary>
        /// <param name="contentTypeId"></param>
        /// <param name="directories"></param>
        /// <returns></returns>
        private List<CMTItem> Translate(string contentTypeId, List<string> directories)
        {
            var result = new List<CMTItem>();

            foreach (var dir in directories)
            {
                var item = new CMTItem()
                {
                    ContentTypeId = contentTypeId,
                    SourceLocation = dir
                };

                result.Add(item);
            }

            return result;
        }
    }
}
