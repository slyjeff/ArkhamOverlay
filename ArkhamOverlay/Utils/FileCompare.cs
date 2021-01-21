﻿using System.IO;

namespace ArkhamOverlay.Utils {
    //borrowed from here: https://docs.microsoft.com/en-us/troubleshoot/dotnet/csharp/create-file-compare
    public static class FileCompare {
        public static bool CompareFiles(string file1, string file2) {
            try {
                int file1byte;
                int file2byte;
                FileStream fs1;
                FileStream fs2;

                // Determine if the same file was referenced two times.
                if (file1 == file2) {
                    // Return true to indicate that the files are the same.
                    return true;
                }

                // Open the two files.
                fs1 = new FileStream(file1, FileMode.Open);
                fs2 = new FileStream(file2, FileMode.Open);

                // Check the file sizes. If they are not the same, the files
                // are not the same.
                if (fs1.Length != fs2.Length) {
                    // Close the file
                    fs1.Close();
                    fs2.Close();

                    // Return false to indicate files are different
                    return false;
                }

                // Read and compare a byte from each file until either a
                // non-matching set of bytes is found or until the end of
                // file1 is reached.
                do {
                    // Read one byte from each file.
                    file1byte = fs1.ReadByte();
                    file2byte = fs2.ReadByte();
                }
                while ((file1byte == file2byte) && (file1byte != -1));

                // Close the files.
                fs1.Close();
                fs2.Close();

                // Return the success of the comparison. "file1byte" is
                // equal to "file2byte" at this point only if the files are
                // the same.
                return ((file1byte - file2byte) == 0);
            } catch {
                //catch errors in case there's some issue with opening files (There might be contention with OBS)
                return false;
            }
        }
    }
}
