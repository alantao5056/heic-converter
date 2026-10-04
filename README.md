# HEIC Batch Converter

**[Get it from the Microsoft Store](https://apps.microsoft.com/detail/9pmm2c5ch29k)** | **[Website](https://heicbatchconverter.alantao.com/)**

## Project Overview
HEIC Batch Converter is a Windows desktop application designed for batch converting HEIC image files to common formats. It features a clean, flat UI design that prioritizes clarity and efficiency, allowing users to easily manage source and target folders, configure conversion settings, and track conversion progress. It is free on the [Microsoft Store](https://apps.microsoft.com/detail/9pmm2c5ch29k); guides and the FAQ are on the [website](https://heicbatchconverter.alantao.com/).

## Features
- **Batch Conversion**: Efficiently convert multiple HEIC files at once.
- **Supported Formats**: Convert to common image formats including JPG, PNG, GIF, and BMP.
- **Subfolder Support**: Optionally include subfolders when scanning the source folder. The source folder structure is recreated under the target folder.
- **Configurable Settings**: 
  - Adjust JPG quality via a real-time slider.
  - Choose conflict resolution strategies (Generate unique name, Replace, Ignore).
  - Define original file handling (Keep, Delete, Move to a specific folder).
- **Remembered Settings**: Window size and position, along with all conversion settings, are saved on exit and restored on the next launch.
- **Clear Progress Tracking**: View conversion progress, success, and failure stats directly in the application's clean workspace.

## Installation
Most users should install the app from the [Microsoft Store](https://apps.microsoft.com/detail/9pmm2c5ch29k). It is free and updates automatically. The sections below are only needed to build from source.

## Prerequisites
To build and run this application, you will need:
- Windows 10 version 1809 (build 17763) or later
- .NET 8 SDK
- Windows App SDK 1.8 workloads (if building via Visual Studio)

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

- `Services/`: the conversion pipeline (`FileService`, built on [Magick.NET](https://github.com/dlemstra/Magick.NET)) and settings persistence (`SettingsService`).
- `Models/`: enums and data types such as `OutputFormat`, `FileStatus`, `ConflictResolution`, `OriginalFileHandling`, and `FileItem`.
- `Controls/`: reusable UI controls, such as the `FileStatusBadge` shown in the file table.

The user interface design mockups, layout specifications, and interactive behaviors are documented in [docs/ui-design/README.md](docs/ui-design/README.md).

## Repository Structure
- `src/App`: the current WinUI 3 application.
- `docs/`: UI design specs, logos, posters, and Microsoft Store listing material.
- `website/`: the Astro static site for the app, published at [heicbatchconverter.alantao.com](https://heicbatchconverter.alantao.com/). See [website/README.md](website/README.md).
- `legacy/v1`: source code of the early 1.x versions, kept for reference.
