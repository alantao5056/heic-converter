---
title: "Why Won't HEIC Files Open on Windows? Causes & Fixes"
description: "HEIC photos won't open on Windows 10 or 11? The cause is almost always missing HEIF/HEVC codecs. Here's how to fix it: install the codecs, or convert the files to JPG for free."
pubDate: 2026-07-08
tldr: "HEIC files won't open on Windows because Windows lacks the required codecs by default: HEIF Image Extensions (free) and HEVC Video Extensions ($0.99 on the Microsoft Store). Install both, or skip the codec problem entirely by converting the files to JPG with a free offline tool like HEIC Batch Converter."
faq:
  - q: "Why does Windows show 'It looks like we don't support this file format' for HEIC?"
    a: "The Photos app displays this error when the HEIF or HEVC codec is missing. HEIC images are compressed with HEVC, and Windows does not license this codec by default, so it must be installed separately from the Microsoft Store."
  - q: "Is the HEVC Video Extensions purchase really necessary?"
    a: "To view HEIC files natively in Windows apps, yes: the $0.99 HEVC Video Extensions package supplies the decoder. The alternative is converting HEIC files to JPG with a third-party converter that includes its own decoder, which costs nothing."
  - q: "Can I make Windows File Explorer show HEIC thumbnails?"
    a: "Yes. Once both HEIF Image Extensions and HEVC Video Extensions are installed, File Explorer renders HEIC thumbnails and the Photos app opens the files normally. If thumbnails still don't appear, restart Explorer or reboot."
  - q: "What's the fastest fix if I don't want to install codecs?"
    a: "Convert the HEIC files to JPG. A free batch converter such as HEIC Batch Converter ships its own HEIC decoder, so it opens and converts files even on systems where Photos can't display them, and JPG opens everywhere."
  - q: "Will this problem come back with new photos?"
    a: "If your iPhone is set to High Efficiency capture, every new photo is HEIC. Either keep a converter handy for imports, or set Settings → Camera → Formats → Most Compatible on the iPhone so it captures JPG directly."
---

You copy photos from your iPhone to your PC, double-click one, and Windows
answers with *"It looks like we don't support this file format"*, or the file
opens as a blank square, or File Explorer shows no thumbnail at all. Nothing is
wrong with your photos. Here's what's actually happening and every way to fix
it.

## The cause: Windows doesn't include HEIC codecs

HEIC photos are stored in the HEIF container and compressed with the **HEVC
(H.265)** codec. HEVC is patent-encumbered, and Microsoft chose not to pay the
license fee for every Windows install. So a default Windows 10 or Windows 11
system is missing one or both of:

- **HEIF Image Extensions:** free, handles the HEIF container. Preinstalled
  on most Windows 11 systems, sometimes missing on Windows 10.
- **HEVC Video Extensions:** **$0.99**, supplies the actual HEVC decoder.
  This is the one almost everyone is missing, and without it HEIC photos
  cannot be displayed.

Both must be present for Photos, Paint, and File Explorer thumbnails to work
with HEIC.

## Fix 1: Install the two codec packages

1. Open the Microsoft Store and install
   [HEIF Image Extensions](https://apps.microsoft.com/detail/9pmmsr1cgpwg)
   (free).
2. Install
   [HEVC Video Extensions](https://apps.microsoft.com/detail/9nmzlz57r3t7)
   ($0.99).
3. Reboot (or restart File Explorer) so thumbnails refresh.

After this, HEIC files open in Photos and Paint, and thumbnails appear in
Explorer.

**When this fix isn't enough:** the codecs only let *Windows* display HEIC.
Plenty of other software (older photo editors, corporate tools, web upload
forms, smart TVs, digital photo frames) still won't accept HEIC files. If
that's your situation, converting is the more permanent answer.

## Fix 2: Convert the HEIC files to JPG (no codecs needed)

Converting sidesteps the codec problem completely, and JPG opens on
effectively every device made in the last 25 years.

The free
[HEIC Batch Converter](https://apps.microsoft.com/detail/9pmm2c5ch29k)
(disclosure: made by the author of this site) includes its own HEIC decoder,
so it works even on PCs where Photos shows the "unsupported format" error:

1. Install it from the Microsoft Store (free, Windows 10 & 11).
2. Select the folder with your HEIC files, with subfolders included if you like.
3. Pick JPG (or PNG/GIF/BMP), click **Start conversion**, and the whole folder
   converts locally on your PC. Nothing is uploaded, and there are no file
   limits or watermarks.

For a step-by-step walkthrough with screenshots of every method, see
[How to convert HEIC to JPG on Windows 11](/articles/convert-heic-to-jpg-windows-11/).

## Fix 3: Prevent future HEIC files at the source

On your iPhone: **Settings → Camera → Formats → Most Compatible**. The camera
then saves JPG instead of HEIC. Two things to know:

- JPG files are roughly **twice the size** of HEIC for the same photo, so
  your phone storage fills faster.
- This only affects *new* photos; existing HEIC files still need converting.

There's a fuller breakdown of that trade-off in
[HEIC vs JPG: quality, size, and compatibility](/articles/heic-vs-jpg/).

## Less common causes worth checking

If the codecs are installed and HEIC files *still* won't open:

- **The file isn't really a HEIC.** Some Android phones and apps mislabel
  files. Try renaming a copy to `.heif`, or open it in a converter to check.
- **The file is corrupted.** Interrupted AirDrop/USB transfers can truncate
  files. Re-copy from the phone and compare file sizes.
- **A third-party codec pack is interfering.** Old "codec packs" can override
  the Store extensions. Uninstall them, then reinstall the two Store packages.
- **Very old Windows 10 builds** (before version 1809) don't support the Store
  extensions at all; on those systems, converting is the only practical
  route.

## Bottom line

| You want | Do this |
|---|---|
| HEIC to open in Windows Photos/Explorer | Install HEIF (free) + HEVC ($0.99) extensions |
| Files that open everywhere, no purchases | Batch convert to JPG offline, free |
| No HEIC ever again | iPhone: Formats → Most Compatible |
