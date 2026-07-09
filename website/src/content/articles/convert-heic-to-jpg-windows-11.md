---
title: "How to Convert HEIC to JPG on Windows 11 (4 Free Ways)"
description: "Four free ways to convert HEIC photos to JPG on Windows 11 and Windows 10: the Photos app, Microsoft Paint, online converters, and a free batch converter for whole folders."
pubDate: 2026-07-08
tldr: "To convert a few HEIC photos to JPG on Windows 11, open each photo in the Photos app and use Save as → JPG. To convert dozens or hundreds at once, use a free batch tool such as HEIC Batch Converter from the Microsoft Store, which converts entire folders to JPG, PNG, GIF, or BMP offline."
howto:
  name: "Batch convert a folder of HEIC files to JPG on Windows"
  steps:
    - name: "Install HEIC Batch Converter"
      text: "Install the free HEIC Batch Converter app from the Microsoft Store."
    - name: "Select your source folder"
      text: "Open the app and pick the folder containing your HEIC files. Enable 'Include subfolders' to scan nested folders too."
    - name: "Choose JPG and set quality"
      text: "Select JPG as the output format and adjust the quality slider (85% is a good balance of size and clarity)."
    - name: "Start the conversion"
      text: "Click 'Start conversion'. Every HEIC file is converted to JPG, with your subfolder structure preserved in the target folder."
faq:
  - q: "Can Windows 11 convert HEIC to JPG without extra software?"
    a: "Yes, for small numbers of photos. If the HEIF and HEVC codecs are installed, the built-in Photos app can open a HEIC file and save it as JPG. However, Windows has no built-in way to batch-convert a whole folder; for that you need a third-party tool."
  - q: "Why can't my PC open HEIC files at all?"
    a: "Windows needs two codecs to display HEIC photos: HEIF Image Extensions (free) and HEVC Video Extensions ($0.99) from the Microsoft Store. If either is missing, HEIC files won't open. A converter app that ships its own decoder, such as HEIC Batch Converter, works without these codecs."
  - q: "Does converting HEIC to JPG lose quality?"
    a: "JPG is a lossy format, so there is minor recompression. At 85–100% JPG quality the difference is invisible for normal photos. If you need pixel-perfect output, convert to PNG instead, which is lossless."
  - q: "How do I stop my iPhone from taking HEIC photos in the first place?"
    a: "On the iPhone, go to Settings → Camera → Formats and choose 'Most Compatible'. New photos will then be saved as JPG. Existing HEIC photos still need to be converted."
---

If you've copied photos from an iPhone to a Windows PC, you've met the `.heic`
file extension, and probably an error message. iPhones have saved photos in
HEIC (High-Efficiency Image Container) format by default since iOS 11, and
Windows still doesn't fully support it out of the box.

The good news: converting HEIC to JPG on Windows 11 (or Windows 10) is free,
whichever way you do it. Here are the four practical methods, from quickest
single-file fix to full-library batch conversion.

## Method 1: The Photos app (best for one or two photos)

Windows 11's built-in Photos app can save a HEIC file as JPG, *if* your PC
has the required codecs installed (see [why HEIC files won't
open](/articles/heic-files-wont-open-windows/) if it doesn't).

1. Double-click the HEIC file to open it in **Photos**.
2. Click the **⋯ (See more)** menu in the toolbar.
3. Choose **Save as**.
4. In the "Save as type" dropdown, pick **JPG** and save.

This works well for a photo or two. Its limitation is obvious: you repeat all
four steps for every single file. For a camera roll with hundreds of photos,
it's not realistic.

## Method 2: Microsoft Paint (also built-in)

Paint on Windows 11 can open HEIC files (again, codecs required) and save them
in another format:

1. Right-click the HEIC file → **Open with** → **Paint**.
2. Go to **File** → **Save as** → **JPEG picture**.

Same story as the Photos app: fine for occasional use, painful at scale. Paint
also strips some metadata and won't let you control JPG quality.

## Method 3: Online converters (convenient, but read this first)

Websites like heictojpg.com or CloudConvert convert HEIC files in the browser.
They're handy when you're on a computer where you can't install anything. Be
aware of the trade-offs, though:

- **Your photos are uploaded to someone else's server.** For family photos,
  documents, or anything private, that's a real consideration.
- **Upload limits.** Most free tiers cap you at 5–50 images per batch, or
  throttle file size.
- **Speed.** Uploading a few gigabytes of photos over home internet takes far
  longer than converting them locally.

If the photos are non-sensitive and few, online tools are fine. For anything
larger or more personal, a local app is faster and safer.

## Method 4: A free batch converter (best for folders and libraries)

When you need to convert a whole folder, or your entire photo library, the
practical answer is a dedicated batch tool.
[HEIC Batch Converter](https://apps.microsoft.com/detail/9pmm2c5ch29k) is a
free Windows 10/11 app (built by the author of this site) designed for exactly
this job:

1. **Install** it free from the Microsoft Store.
2. **Pick a source folder.** It finds every HEIC file inside, including
   subfolders if you want.
3. **Choose JPG** (or PNG, GIF, BMP) and set the quality slider.
4. **Click Start conversion.** Hundreds of files convert in one run, with live
   per-file status, and your subfolder structure is recreated in the output
   folder.

A few things that make it suited to big jobs:

- **Fully offline:** nothing is uploaded anywhere; it even works without
  internet, and it doesn't depend on Windows' HEIC codecs being installed.
- **No limits or watermarks:** convert 10 files or 10,000.
- **Original file handling:** after converting, automatically keep, delete,
  or move your HEIC originals.
- **Conflict resolution:** duplicate names are renamed, replaced, or skipped
  according to your setting.

## Which method should you use?

| Situation | Best method |
|---|---|
| 1–5 photos, codecs installed | Photos app (Method 1) |
| Quick edit + convert | Paint (Method 2) |
| On a locked-down PC, non-private photos | Online converter (Method 3) |
| Whole folders, photo libraries, recurring imports | HEIC Batch Converter (Method 4) |
| HEIC files won't open at all | Method 4 (no codecs needed) |

## One more tip: stop the problem at the source

If you'd rather not deal with HEIC again, change your iPhone camera setting:
**Settings → Camera → Formats → Most Compatible**. New photos will be captured
as JPG. Keep in mind JPGs take roughly twice the storage of HEIC, and the
photos already on your PC still need converting; see the comparison in
[HEIC vs JPG](/articles/heic-vs-jpg/) before deciding.
