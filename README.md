# HEIC Batch Converter

## Project Overview
HEIC Batch Converter is a Windows desktop application designed for batch converting HEIC image files to common formats. It features a clean, flat UI design that prioritizes clarity and efficiency, allowing users to easily manage source and target folders, configure conversion settings, and track conversion progress.

## Features
- **Batch Conversion**: Efficiently convert multiple HEIC files at once.
- **Supported Formats**: Convert to common image formats including JPG, PNG, GIF, and BMP.
- **Configurable Settings**: 
  - Adjust JPG quality via a real-time slider.
  - Choose conflict resolution strategies (Generate unique name, Replace, Ignore).
  - Define original file handling (Keep, Delete, Move to a specific folder).
- **Clear Progress Tracking**: View conversion progress, success, and failure stats directly in the application's clean workspace.

## Prerequisites
To build and run this application, you will need:
- .NET 8 SDK
- Windows App SDK workloads (if building via Visual Studio)

## Building and Running
This is a standard .NET WinUI 3 project. You can build and run it using the .NET CLI or Visual Studio.

**Using .NET CLI:**
*   **Build:** 
    ```bash
    dotnet build src/App/App.csproj -p:Platform=x64
    ```
*   **Run:** 
    ```bash
    dotnet run --project src/App/App.csproj -p:Platform=x64
    ```

**Using Visual Studio:**
Open the `HeicConverter.slnx` solution file in Visual Studio and use the standard Build and Run (F5) commands.

## Architecture & Design
The application is structured as a WinUI 3 single-project application located in `src/App`. The UI is constructed using XAML (`MainWindow.xaml`, `App.xaml`) with C# code-behind.

The user interface design mockups, layout specifications, and interactive behaviors can be found in the [docs/ui-design/README.md](docs/ui-design/README.md) directory.
