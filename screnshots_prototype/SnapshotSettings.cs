using System;

namespace screnshots_prototype
{
    class SnapshotSettings
    {
        public DateTime Timestamp { get; set; }
        public int Width { get; set; } = 640;
        public int Height { get; set; } = 480;
        public int Quality { get; set; } = 75;
        public bool KeepAspectRatio { get; set; } = true;
        public bool IncludeBlackBars { get; set; } = false;
        public double LiveTimeoutMS { get; set; } = 2000;
        public bool LiftPrivacyMask { get; set; } = false;
        public double Interval { get; set; } = 0;
        public string Behavior { get; set; } = "getnearest";
    }
}
