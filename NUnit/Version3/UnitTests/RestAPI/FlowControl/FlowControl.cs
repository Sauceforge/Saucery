namespace UnitTests.RestAPI.FlowControl {
    public class FlowControl {
        public Remaining remaining { get; set; }
    }

    public class Remaining {
        public int overall { get; set; }
        public int mac { get; set; }
        public int manual { get; set; }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 18th September 2014
 * 
 */