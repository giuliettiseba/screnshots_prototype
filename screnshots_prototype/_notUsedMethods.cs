using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoOS.Platform.Live;

namespace screnshots_prototype
{
    internal class _notUsedMethods
    {


        private void SendPanicEvent_Click(object sender, EventArgs e)
        {

                    private readonly object _liveContentLock = new object();
        private LiveSourceContent _liveContent;

        //int width = 640;
        //int height = 480;
        //int quality = 75;
        //bool keepAspectRatio = true;
        //bool includeBlackBars = false;
        //double liveTimeoutMS = 2000;
        //bool liftPrivacyMask = false;

        //var item = allCameras.FirstOrDefault(i => i.Name.Contains("AXIS M1065-L Network Camera (10.1.0.68) - Camera 1"));


        //var src = new JPEGLiveSource(item)
        //{
        //    Width = width,
        //    Height = height,
        //    Compression = quality,
        //    SendInitialImage = false
        //};
        //src.SetKeepAspectRatio(keepAspectRatio, includeBlackBars);
        //src.LiveContentEvent += SrcOnLiveContentEvent;
        //src.Init();
        //src.LiveModeStart = true;

        //var liveImageArrived = _liveDataSignal.Wait(TimeSpan.FromMilliseconds(liveTimeoutMS));
        //src.LiveContentEvent -= SrcOnLiveContentEvent;
        //src.LiveModeStart = false;
        //if (!liveImageArrived)
        //{
        //    //throw new Exception($"No live image available for {src.Item.Name} after -{nameof(liveTimeoutMS)} value of {liveTimeoutMS}.");
        //}
        //else
        //{
        //    var snapshot = new JPEGData { Bytes = _liveContent.Content, DateTime = _liveContent.EndTime };

        //    byte[] temp = snapshot.Bytes.ToArray();
        //    Stream stream = new MemoryStream(temp);
        //    pictureBox_snapshot.Image = Image.FromStream(stream);
        //}

        //foreach (var camera in allCameras)
        //{
        //    JPEGLiveSource source = new JPEGLiveSource(camera);
        //    source.Init();
        //    source.LiveContentEvent += Source_LiveContentEvent; ;


        //}

        //private void SrcOnLiveContentEvent(object sender, EventArgs e)
        //{
        //    lock (_liveContentLock)
        //    {
        //        if (_liveDataSignal.IsSet || !(e is LiveContentEventArgs liveContent)) return;
        //        if (liveContent.Exception != null) return;
        //        _liveContent = liveContent.LiveContent;
        //        _liveDataSignal.Set();
        //        var snapshot = new JPEGData { Bytes = _liveContent.Content, DateTime = _liveContent.EndTime };

        //        byte[] temp = snapshot.Bytes.ToArray();
        //        Stream stream = new MemoryStream(temp);
        //        pictureBox_snapshot.Image = Image.FromStream(stream);
        //    }
        //}

        private readonly ManualResetEventSlim _liveDataSignal = new ManualResetEventSlim();



    }


}
}
