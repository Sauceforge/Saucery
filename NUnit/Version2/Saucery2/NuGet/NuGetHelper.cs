using System;
using System.Linq;
//using NuGet;
using Saucery2.Util;

namespace Saucery2.NuGet {
    public class NuGetHelper {
        //public static string GetInstalledVersion() {
        //    var version = GetInstalledSemanticVersion();
        //    return ToString(version);
        //}

        //public static string GetMaxVersion() {
        //    var version = GetMaxSemanticVersion();
        //    return ToString(version);
        //}

        //public static bool IsNewVersionAvailable() {
        //    var installed = GetInstalledSemanticVersion();
        //    var max = GetMaxSemanticVersion();
        //    return max.CompareTo(installed) > 0;
        //}

        //private static SemanticVersion GetMaxSemanticVersion() {
        //    var repo = PackageRepositoryFactory.Default.CreateRepository(SauceryConstants.NUGET_API);
        //    var packages = repo.FindPackagesById(Saucery2Constants.PRODUCTNAME).ToList();
        //    return packages.Any() ? packages.Max(p => p.Version) : new SemanticVersion(new Version());
        //}

        //private static SemanticVersion GetInstalledSemanticVersion() {
        //    var packageConfigFileName = FileHelper.PackageConfigFilePath;
        //    var file = new PackageReferenceFile(packageConfigFileName);
        //    foreach (var packageReference in file.GetPackageReferences()) {
        //        if (packageReference.Id.Equals(Saucery2Constants.PRODUCTNAME)) {
        //            Console.WriteLine(ToString(packageReference.Version));
        //            return packageReference.Version;
        //        }
        //    }
        //    return new SemanticVersion(new Version());
        //}

        //private static string ToString(SemanticVersion v) {
        //    return string.Format(SauceryConstants.NUGET_VERSION, v.Version.Major, v.Version.Minor, v.Version.Build);
        //}
    }
}
/*
 * Copyright Andrew Gray, Full Circle Solutions
 * Date: 24th December 2015
 * 
 */