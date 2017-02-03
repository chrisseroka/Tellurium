﻿using System;
using Tellurium.MvcPages.Utils;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.TestRunersAdapters.Providers
{
    public class TeamCityRunnerAdapter : ITestRunnerAdapter
    {
        private const string TeamcityVariableName = "TEAMCITY_PROJECT_NAME";
        private const string VisualAssertionWebPathVariableName = "Tellurium_VisualAssertions_Web";

        public bool IsPresent()
        {
            return Environment.GetEnvironmentVariable(TeamcityVariableName) != null;
        }

        public void NotifyAboutTestSuccess(string testName)
        {
            var escapedTestName = Escape(testName);
            Console.WriteLine("##teamcity[testStarted name='{0}']", escapedTestName);
            Console.WriteLine("##teamcity[testFinished name='{0}']", escapedTestName);
        }

        public void NotifyAboutTestFail(string testName, TestSession session, BrowserPattern pattern)
        {
            var escapedTestName = Escape(testName);
            var detailsMessage = GetDetailsMessage(session, pattern);
            Console.WriteLine("##teamcity[testStarted name='{0}']", escapedTestName);
            Console.WriteLine("##teamcity[testFailed name='{0}' details='{1}']", escapedTestName, detailsMessage);
            Console.WriteLine("##teamcity[testFinished name='{0}']", escapedTestName);
        }

        public void NotifyAboutError(Exception ex)
        {
            Console.WriteLine("##teamcity[message text='{0}' errorDetails='{1}' status='ERROR']",ex.GetFullExceptionMessage(), ex.StackTrace);
        }

        private string GetDetailsMessage(TestSession session, BrowserPattern pattern)
        {
            var testResultPreviewPath = GetTestResultPreviewPath(session, pattern);
            var detailsMessage = string.Format("Screenshots are different. Details at {0}", testResultPreviewPath);
            return detailsMessage;
        }

        private string GetVisualAssertionWebPath()
        {
            return Environment.GetEnvironmentVariable(VisualAssertionWebPathVariableName);
        }

        private string GetTestResultPreviewPath(TestSession session, BrowserPattern pattern)
        {
            var rootPath = GetVisualAssertionWebPath();
            if (string.IsNullOrWhiteSpace(rootPath))
            {
                return string.Empty;
            }
            return string.Format("{0}/Home/GetTestResultPreview?testSessionId={1}&patternId={2}", rootPath.TrimEnd('/'), session.Id, pattern.Id);
        }

        static string Escape(string value)
        {
            return value.Replace("|", "||")
                .Replace("'", "|'")
                .Replace("\r", "|r")
                .Replace("\n", "|n")
                .Replace("]", "|]");
        }
    }
}