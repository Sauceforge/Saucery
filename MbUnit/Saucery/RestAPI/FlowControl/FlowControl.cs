namespace Saucery.RestAPI.FlowControl {
    public class FlowControl {
        public double timestamp { get; set; }
        public Concurrency concurrency { get; set; }
    }

    public class Concurrency {
        public Jenkinsvacc jenkinsvacc { get; set; }
    }

    public class Jenkinsvacc {
        public Current current { get; set; }
        public Remaining remaining { get; set; }
    }

    public class Current {
        public int overall { get; set; }
        public int mac { get; set; }
        public int manual { get; set; }
    }

    public class Remaining {
        public int overall { get; set; }
        public int mac { get; set; }
        public int manual { get; set; }
    }
}