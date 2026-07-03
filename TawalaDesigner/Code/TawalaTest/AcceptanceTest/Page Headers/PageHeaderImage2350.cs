// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NUnit.Framework;
using Tawala.Projects;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.PageHeaders
{
    [TestFixture]
    public class PageHeaderImage2350
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        #endregion

        private void roundTripImageThroughProjectXml(string filename, ImageFormat format, int width, int height)
        {
            string expectedBase64 = Convert.ToBase64String(File.ReadAllBytes(getPath(filename)));

            Project.Current.PageHeader.SetImage(getPath(filename));

            string projectFile = Util.SaveCurrentProject();

            Util.LoadProject(projectFile);

            using (Image image = Image.FromFile(Project.Current.PageHeader.GetImage()))
            {
                string reloadedBase64 = Convert.ToBase64String(File.ReadAllBytes(getPath(Project.Current.PageHeader.GetImage())));

                Assert.IsNotNull(image);
                Assert.AreEqual(format, image.RawFormat);
                Assert.AreEqual(width, image.Width);
                Assert.AreEqual(height, image.Height);
                Assert.AreEqual(expectedBase64, reloadedBase64);
            }
        }

        private string getPath(string testFile)
        {
            return Path.Combine("PageHeader", testFile);
        }

        [Test]
        public void RoundTripImageGifThroughProjectXml()
        {
            roundTripImageThroughProjectXml("PageHeader.gif", ImageFormat.Gif, 48, 37);
        }

        [Test]
        public void RoundTripImageJpegThroughProjectXml()
        {
            roundTripImageThroughProjectXml("PageHeader.jpg", ImageFormat.Jpeg, 50, 39);
        }

        [Test]
        public void RoundTripImagePngThroughProjectXml()
        {
            roundTripImageThroughProjectXml("PageHeader.png", ImageFormat.Png, 53, 41);
        }
    }
}