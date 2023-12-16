namespace FossPDF.PlatformUtils;

public class PlatformUtils
{
    public static string GetFileExplorerForPlatform()
    {
        var fileExplorer = "explorer";
        // if we're on Linux, we use xdg-open
        if (Environment.OSVersion.Platform == PlatformID.Unix)
            fileExplorer = "xdg-open";
        // and if we're on macOS, we use open
        else if (Environment.OSVersion.Platform == PlatformID.MacOSX)
            fileExplorer = "open";

        return fileExplorer;
    }
}
