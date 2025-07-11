# Fonts Installer

A simple Windows Forms application for bulk installing fonts from a selected folder (including subfolders). Supports `.ttf`, `.otf`, and `.zip` archives containing fonts.

## Features

- Select a root folder to search for fonts.
- Installs all `.ttf` and `.otf` font files found in the folder and its subfolders.
- Automatically extracts and installs fonts from `.zip` archives.
- Displays log messages and installation status in the UI.
- Requires administrator privileges to install fonts.

## Usage

1. **Run the application as Administrator.**
2. Click the **"Pick Fonts Folder"** button.
3. Select the folder containing your fonts (including subfolders or zip archives).
4. The application will extract, copy, and install all found fonts.
5. View the log and status in the main window.

## Requirements

- Windows 7 or later
- .NET Framework 4.7.2 or later

## Building

Open [FontsInstaller.sln](FontsInstaller.sln) in Visual Studio and build the solution.

## Project Structure

- [Form1.cs](Form1.cs): Main form logic and font installation code.
- [Form1.Designer.cs](Form1.Designer.cs): UI layout.
- [Program.cs](Program.cs): Application entry point and privilege elevation.
- [Properties/](Properties/): Assembly info, resources, and settings.

## License

Copyright © 2024

---

**Note:** You must run this application as an administrator to install fonts system
