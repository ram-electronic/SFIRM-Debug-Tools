# SFIRM Debug Tools

Tools to set up and test Windows crash dump collection for Star Finanz (SFirm) applications.

| File | Description |
|------|-------------|
| `gpo-sfirm.ps1` | Creates/updates a GPO (`GPO-X-SFIRM-Crash-Dumps`) that enables Windows Error Reporting LocalDumps for the SFirm executables (dumps go to `%PROGRAMDATA%\Star Finanz\CrashDumps`). Requires the GroupPolicy module. |
| `CrashGui.cs` | Small WinForms test app with a "CRASH now" button, used to verify that crash dumps are written. Includes a marker string to search for in the dump. |
| `sfirm-fake-crash-exe-gui.ps1` | Compiles a minimal crash test GUI under a given exe name (default `SFSupport.exe`) so it matches a LocalDumps key. |
| `SFSupport.exe` | Prebuilt crash test executable named to match the LocalDumps configuration. |
| `LICENSE` | License. |
