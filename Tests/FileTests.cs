using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Nfu.Models;
using System.IO;

namespace Tests
{
    [TestClass]
    public class FileTests
    {
        [TestMethod]
        public void AfterUploadTest()
        {
            var files = new List<UploadFile> {
                new UploadFile(FileState.Temporary),
                new UploadFile(FileState.Temporary)
            };

            // Set the state of the second file to normal
            // There's one temporary and one normal file now
            files[1].State = FileState.Normal;

            foreach (var file in files)
            {
                file.AfterUpload();

                if (file.State == FileState.Normal && !File.Exists(file.Path))
                {
                    Assert.Fail("Normal file has been deleted.");
                }
                if (file.State == FileState.Temporary && File.Exists(file.Path))
                {
                    Assert.Fail("Temporary fail has not been deleted.");
                }
            }
        }
    }
}
