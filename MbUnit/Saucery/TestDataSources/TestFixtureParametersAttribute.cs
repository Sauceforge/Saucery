using System;
using Gallio.Common.Reflection;
using Gallio.Framework.Data;
using Gallio.Framework.Pattern;
using MbUnit.Framework;

namespace Saucery.TestDataSources {
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true)]
    public class TestFixtureParametersAttribute : DataAttribute {
        internal string Os { get; set; }
        internal string BrowserName { get; set; }
        internal string BrowserVersion { get; set; }
        internal string LongName { get; set; }
        internal string LongVersion { get; set; }
        internal string Url { get; set; }
        internal string Device { get; set; }
        internal string DeviceType { get; set; }
        internal string DeviceOrientation { get; set; }

        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement) {
            var row = new object[] {Os, BrowserName, BrowserVersion, LongName, LongVersion, Url, Device, DeviceType, DeviceOrientation};
            dataSource.AddDataSet(new ItemSequenceDataSet(new IDataItem[] {new ListDataItem<object>(row, GetMetadata(), false)}, row.Length));
        }
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 12th July 2014
 * 
 */