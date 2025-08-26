# NCG Full

**NCG Full** is a C# automation project that streamlines the process of sourcing, editing, and publishing royalty-free music content to YouTube. It was originally built to support the [**No Copyright Gang**](https://www.youtube.com/@NoCopyrightGang) channel by automating repetitive workflows like fetching tracks, generating videos, and uploading them with minimal manual intervention.

This project was primarily created as a way to **train and improve my C# programming skills**, while also solving a practical challenge.

## Features

* 🎵 **Song Fetcher** (`NCG_SongGetter.cs`):
  Retrieves music from a curated list of YouTube channels that publish royalty-free or copyright-free tracks.

* 🎬 **Video Editor** (`NCG_VideoEditor.cs`):
  Automatically generates a video by combining downloaded audio with a randomly selected image from a local directory.

* ☁️ **Uploader** (`NCG_Uploader.cs`):
  Handles authentication and uploads the generated videos to YouTube.

* 📡 **Feed Getter** (`NCG_FeedGetter.cs`):
  Collects and updates metadata/feeds to keep track of published and pending uploads.

* 🛠 **Console Tools** (`ConsoleTools.cs`):
  Utility functions for console output and runtime handling.

* 🔄 **Main Workflow** (`Program.cs`):
  Orchestrates the full pipeline in a loop:

  1. Fetch new songs
  2. Generate videos
  3. Upload to YouTube
  4. Update feed
  5. Sleep and repeat

## Workflow Diagram

```
 ┌───────────────┐       ┌───────────────┐       ┌───────────────┐
 │   SongGetter  │──────▶│  VideoEditor  │──────▶│   Uploader     │
 │ (Download mp3)│       │(Add image + mp3)│     │ (Upload video) │
 └───────────────┘       └───────────────┘       └───────────────┘
         │                          │                     │
         ▼                          ▼                     ▼
   Free music feeds         Local image pool       YouTube Channel
   (YouTube sources)             (random)        [No Copyright Gang]
```

## Technology Stack

* **Language**: C#
* **Framework**: .NET Framework
* **APIs/Integrations**: YouTube Data API
* **Build System**: Visual Studio project (`.csproj`)

## Getting Started

> ⚠️ Note: This project was created in 2019 and may require updates to run with modern dependencies or YouTube APIs.

### Prerequisites

* Visual Studio or compatible .NET IDE
* .NET Framework (version used in 2019, likely 4.x)
* YouTube Data API credentials

### Setup

1. Clone the repository:

   ```bash
   git clone https://github.com/adavid1/NCG_Full.git
   cd NCG_Full
   ```
2. Open the solution in Visual Studio.
3. Update the `basePath` in `Program.cs` to point to your local project directory.
4. Configure API credentials in `NCG_Uploader.cs` (YouTube Data API).
5. Prepare a folder of images for video backgrounds.

### Run

Start the project from Visual Studio or with:

```bash
dotnet run
```

The pipeline will continuously fetch, edit, and upload content until terminated.

## Purpose & Learnings

This project was built both as a **practical automation tool** and as a way to **train and strengthen my C# skills**. Through it, I gained hands-on experience with:

* Consuming third-party APIs (YouTube Data API)
* Automating multimedia workflows (audio/video processing)
* Structuring modular C# applications
* Managing asynchronous and scheduled tasks

## License

This project is not licensed for reuse, as it was a personal automation tool.
