using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Data;
using VideoOS.Platform.Live;
using VideoOS.Platform.SDK.Platform;

namespace screnshots_prototype
{
    public partial class Form1 : Form
    {

        private static readonly Guid IntegrationId = new Guid("CD52BF80-A58B-4A35-BF30-159753159754");
        private const string IntegrationName = "ScreenShoter";
        private const string Version = "1.0";
        private const string ManufacturerName = "SGIU";

        private bool _isConnected;

        public Form1()
        {
            InitializeComponent();

            // Connect 

            ServerInfo ms_Info = new ServerInfo()
            {
                Address = "10.1.0.79",
                Domain = ".",
                UserName = "Administrator",
                Password = "Firewall1"
            };

            _isConnected = ConnectManagementServer(ms_Info);

            // Load all cameras

            if (_isConnected)
            {
                allCameras = Configuration.Instance.GetItemsBySearch(Kind.Camera.ToString(), 100, 5, out SearchResult searchResult).Where(i => i.FQID.FolderType == FolderType.No);
                listBox1.Items.AddRange(allCameras.ToArray());
            }
        }

        private bool ConnectManagementServer(ServerInfo ms_Info)
        {
            // Build credentials
            NetworkCredential nc = new NetworkCredential(ms_Info.UserName, ms_Info.Password, ms_Info.Domain);
            Uri uri = new Uri("http://" + ms_Info.Address);

            try
            {
                // Attemp to connect
                VideoOS.Platform.SDK.Environment.Initialize();
                VideoOS.Platform.SDK.Export.Environment.Initialize();
                VideoOS.Platform.SDK.Media.Environment.Initialize();

                VideoOS.Platform.SDK.Environment.AddServer(false, uri, nc, true);                    // Add the server to the environment 
                try
                {
                    VideoOS.Platform.SDK.Environment.Login(uri, IntegrationId, IntegrationName, Version, ManufacturerName);     // attempt to login 
                }

                catch (System.BadImageFormatException)
                {
                    // Ignore this error.  // 2021R1 SDK bug. 

                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (ServerNotFoundMIPException snfe)
            {
                throw new Exception("Server not found: " + snfe.Message + "." );
            }
            catch (InvalidCredentialsMIPException ice)
            {
                throw new Exception("Invalid credentials: " + ice.ToString() + "." );
            }
            catch (Exception ex)
            {
                throw new Exception("Other error connecting to " + ms_Info.Address + " : " + ex.ToString() + "." );
            }

            return true;
        }

        IEnumerable<Item> allCameras;

        private void GetRecordedSnapshot(Item item, SnapshotSettings snapshotSettings)
        {
            JPEGVideoSource src = null;
            try
            {
                if (pictureBox_snapshot.Image != null)
                {
                    pictureBox_snapshot.Image.Dispose();
                    pictureBox_snapshot.Image = null;
                    pictureBox_snapshot.Update();
                }

                item.FQID.ServerId.UserContext.SetPrivacyMaskLifted(snapshotSettings.LiftPrivacyMask);
                if (item == null)
                {
                    //WriteWarning($"Configuration not available for camera with ID {cameraId}. It might be disabled.");
                    return;
                }
                src = new JPEGVideoSource(item);
                src.SetKeepAspectRatio(snapshotSettings.KeepAspectRatio, snapshotSettings.IncludeBlackBars);
                src.Init(snapshotSettings.Width, snapshotSettings.Height);
                src.Compression = snapshotSettings.Quality;
                JPEGData snapshot = null;
                switch (snapshotSettings.Behavior.ToLower())
                {
                    case "getbegin":
                        {
                            snapshot = src.GetBegin();
                            SaveSnapshot(snapshot, src.Item);
                            break;
                        }
                    case "getend":
                        {
                            snapshot = src.GetEnd();
                            SaveSnapshot(snapshot, src.Item);
                            break;
                        }
                    case "getnearest":
                        {
                            var nextTimestamp = snapshotSettings.Timestamp;
                            snapshot = src.GetNearest(nextTimestamp) as JPEGData;
                            if (snapshot?.Bytes == null || snapshot.Bytes.Length == 0)
                            {
                                throw new InvalidDataException("Received empty snapshot");
                            }
                            SaveSnapshot(snapshot, src.Item);

                            break;
                        }
                }
            }
            catch (CommunicationMIPException)
            {
                throw new Exception($"Unable to connect to {src?.Item.Name} with ID {src?.Item.FQID.ObjectId}");
            }
            catch (Exception ex)
            {
                //WriteError(
                //    new ErrorRecord(
                //        ex,
                //        ex.Message,
                //        ErrorCategory.ConnectionError,
                //        src));
            }
            finally
            {
                src?.Close();
            }
        }

        private Image AddWaterMark(Image bitmap, string text, int x, int y)
        {
            //using (Image image = Image.FromFile(@"C:\Users\Public\Pictures\Sample Pictures\Desert.jpg"))
            //using (Image watermarkImage = Image.FromFile(@"C:\Users\Public\Pictures\Sample Pictures\watermark.png"))
            //using (Graphics imageGraphics = Graphics.FromImage(image))
            //using (TextureBrush watermarkBrush = new TextureBrush(watermarkImage))
            //{
            //    int x = (image.Width / 2 - watermarkImage.Width / 2);
            //    int y = (image.Height / 2 - watermarkImage.Height / 2);
            //    watermarkBrush.TranslateTransform(x, y);
            //    imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));
            //    image.Save(@"C:\Users\Public\Pictures\Sample Pictures\Desert_watermark.jpg");
            //}


            Font font = new Font("Arial", 10, FontStyle.Italic, GraphicsUnit.Pixel);

            Point upLeftPoint = new Point(x, y);
            Color textColor = Color.White;
            Point atpoint = new Point(x, y + bitmap.Height / 2);

            SolidBrush brush = new SolidBrush(textColor);
            Graphics graphics = Graphics.FromImage(bitmap);
            
            StringFormat sf = new StringFormat();
            Size sizeOfText = TextRenderer.MeasureText(text, font);
            Rectangle rect = new Rectangle(atpoint, sizeOfText);
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Near;
            graphics.FillRectangle(Brushes.Black, rect);
            graphics.DrawString(text, font, brush, atpoint, sf);
            graphics.Dispose();
            MemoryStream m = new MemoryStream();
            bitmap.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] convertedToBytes = m.ToArray();
            Stream stream = new MemoryStream(convertedToBytes);
            var frame = Image.FromStream(stream);
            return frame;
        }

        private void SaveSnapshot(JPEGData snapshot, Item item)
        {
            if (snapshot == null) return;

            Image frame = ConverJPEGData2Image(snapshot);
            Image frameWithWaterMark = AddWaterMark(frame, item.Name, 2, 2);
            Image frameWithWaterMark2 = AddWaterMark(frameWithWaterMark, snapshot.DateTime.ToLocalTime().ToString(), 2, 15);
            pictureBox_snapshot.Image = frameWithWaterMark;

            //string FileName = null;
            //bool UseFriendlyName = false;
            //string path2 = null;

            //var fileName = FileName ?? $"{(UseFriendlyName ? item.Name : item.FQID.ObjectId.ToString())}_{snapshot.DateTime:yyyy-MM-dd_HH-mm-ss.fff}.jpg";
            //System.IO.Path.GetInvalidFileNameChars().ToList().ForEach(c => fileName = fileName.Replace(c, '-'));
            //System.IO.Path.GetInvalidPathChars().ToList().ForEach(c => path2.Replace(c, '-'));
            //try
            //{
            //    var path = System.IO.Path.Combine(path2, fileName);
            //    File.WriteAllBytes(path, snapshot.Bytes);
            //    File.SetLastWriteTimeUtc(path, snapshot.DateTime);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(
            //            ex.Message);
            //}
        }

        private static Image ConverJPEGData2Image(JPEGData snapshot)
        {
            byte[] temp = snapshot.Bytes.ToArray();
            Stream stream = new MemoryStream(temp);
            var frame = Image.FromStream(stream);
            return frame;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var item = Configuration.Instance.GetItem(Connection.CurrentSite.FQID.ServerId, cameraId, Kind.Camera);
            var item = listBox1.SelectedItem as Item;
            bool liftPrivacyMask = false;
            item.FQID.ServerId.UserContext.SetPrivacyMaskLifted(liftPrivacyMask);
            var defaultSnapshotSettings = new SnapshotSettings();
            defaultSnapshotSettings.Timestamp = DateTime.Now;
            GetRecordedSnapshot(item, defaultSnapshotSettings);
        }
    }
}
