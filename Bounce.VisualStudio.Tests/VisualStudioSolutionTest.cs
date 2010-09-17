﻿using System;
using System.IO;
using System.Linq;
using Bounce.Framework;
using ICSharpCode.SharpZipLib.Zip;
using NUnit.Framework;

namespace Bounce.VisualStudio.Tests {
    [TestFixture]
    public class VisualStudioSolutionTest {
        private const string SolutionUnzipDirectory = "TestSolution";

        [Test]
        public void LastBuiltShouldReturnLatestDateForAllOutputFiles() {
            UnzipTestSolution();

            var solution = new VisualStudioSolution { SolutionPath = new PlainValue<string>(Path.Combine(SolutionUnzipDirectory, @"TestSolution\TestSolution.sln")) };

            Assert.That(solution.LastBuilt, Is.Null);

            solution.Build();

            DateTime now = DateTime.UtcNow;
            Assert.That(solution.LastBuilt, Is.InRange(now.AddSeconds(-2), now));

            solution.Clean();

            Assert.That(solution.LastBuilt, Is.Null);
        }

        [Test]
        public void BuildsOutputFileOfFirstProject() {
            UnzipTestSolution();

            var solution = new VisualStudioSolution { SolutionPath = new PlainValue<string>(Path.Combine(SolutionUnzipDirectory, @"TestSolution\TestSolution.sln")) };

            solution.Build();

            Assert.That(File.Exists(solution.Projects["TestSolution".V()].OutputFile.Value));
        }

        [Test]
        public void CanAccessProjectsBeforeSolutionExists() {
            DeleteTestSolution();

            var solution = new VisualStudioSolution {SolutionPath = new PlainValue<string>(Path.Combine(SolutionUnzipDirectory, @"TestSolution\TestSolution.sln"))};

            var outputFiles = solution.Projects.Select(p => p.OutputFile);

            UnzipTestSolution();

            Assert.That(outputFiles.Select(o => o.Value).ToArray(), Is.EquivalentTo(new [] {@"TestSolution\TestSolution\TestSolution\bin\Debug\TestSolution.dll"}));
        }
        
        private void UnzipTestSolution() {
            DeleteTestSolution();
            new FastZip().ExtractZip("TestSolution.zip", SolutionUnzipDirectory, null);
        }

        private void DeleteTestSolution() {
            if (Directory.Exists(SolutionUnzipDirectory)) {
                Directory.Delete(SolutionUnzipDirectory, true);
            }
        }
    }
}