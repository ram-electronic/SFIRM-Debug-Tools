using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

class CrashForm : Form {
    const string WerKey = @"SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps";

    // Marker string to search for in the dump
    public string stringTiereMitS = "Stirnlappenbasilisk";

    readonly string exeName = Path.GetFileName(Application.ExecutablePath);

    CrashForm() {
        Text = "Crash Test - " + exeName;
        ClientSize = new Size(360, 260);
        Padding = new Padding(15);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));

        var crash = MakeButton("CRASH now\u2122", 16, Color.Firebrick);
        crash.Click += (s, e) => Marshal.WriteInt32(IntPtr.Zero, 42);   // access violation

        var folder = MakeButton("Open dump folder", 10, Color.SteelBlue);
        folder.Click += (s, e) => OpenDumpFolder();

        var reg = MakeButton("Open registry to see dump settings", 10, Color.SteelBlue);
        reg.Click += (s, e) => OpenRegistry();

        layout.Controls.Add(crash, 0, 0);
        layout.Controls.Add(folder, 0, 1);
        layout.Controls.Add(reg, 0, 2);
        Controls.Add(layout);
    }

    static Button MakeButton(string text, float size, Color color) {
        return new Button {
            Text = text,
            Font = new Font("Segoe UI", size, FontStyle.Bold),
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 0, 8)
        };
    }

    static RegistryKey Hklm() {
        return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
    }

    void OpenDumpFolder() {
        // Same order WER uses: per-app key, then global key, then default
        string folder = ReadDumpFolder(WerKey + "\\" + exeName)
                     ?? ReadDumpFolder(WerKey)
                     ?? @"%LOCALAPPDATA%\CrashDumps";
        folder = Environment.ExpandEnvironmentVariables(folder);

        if (Directory.Exists(folder))
            Process.Start("explorer.exe", "\"" + folder + "\"");
        else
            MessageBox.Show("Folder does not exist (yet):\n" + folder, Text);
    }

    static string ReadDumpFolder(string path) {
        using (var root = Hklm())
        using (var key = root.OpenSubKey(path)) {
            if (key == null) return null;
            return key.GetValue("DumpFolder", null, RegistryValueOptions.DoNotExpandEnvironmentNames) as string;
        }
    }

    void OpenRegistry() {
        // Regedit opens at the key stored in HKCU\...\Applets\Regedit\LastKey
        const string regeditKey = @"Software\Microsoft\Windows\CurrentVersion\Applets\Regedit";
        string target = @"HKEY_LOCAL_MACHINE\" + WerKey + "\\" + exeName;

        using (var key = Registry.CurrentUser.CreateSubKey(regeditKey)) {
            // Keep the (possibly localized) root name regedit already uses, e.g. "Computer"
            string last = key.GetValue("LastKey") as string ?? "";
            int i = last.IndexOf(@"\HKEY_", StringComparison.OrdinalIgnoreCase);
            string prefix = i > 0 ? last.Substring(0, i) : "Computer";
            key.SetValue("LastKey", prefix + "\\" + target);
        }

        try { Process.Start("regedit.exe", "-m"); }   // -m = new instance even if regedit is already open
        catch (System.ComponentModel.Win32Exception) { }   // UAC prompt cancelled
    }

    [STAThread]
    static void Main() {
        // Let exceptions crash the process instead of showing the WinForms "Continue/Quit" dialog
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
        Application.EnableVisualStyles();
        Application.Run(new CrashForm());
    }
}
