using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

class CrashForm : Form {
    CrashForm() {
        Text = "Crash Test - " + AppDomain.CurrentDomain.FriendlyName;
        ClientSize = new Size(320, 140);
        Padding = new Padding(20);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        var btn = new Button {
            Text = "CRASH now\u2122",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            BackColor = Color.Firebrick,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Dock = DockStyle.Fill
        };
        btn.Click += (s, e) => Marshal.WriteInt32(IntPtr.Zero, 42);   // access violation
        Controls.Add(btn);
    }

    [STAThread]
    static void Main() {
        // Let exceptions crash the process instead of showing the WinForms "Continue/Quit" dialog
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
        Application.EnableVisualStyles();
        Application.Run(new CrashForm());
    }
}
