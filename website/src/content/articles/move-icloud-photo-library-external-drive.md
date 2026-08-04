---
title: "Move a 1 TB iCloud Photo Library to an External Drive (Before You Cancel iCloud)"
description: "A 1 TB photo library and a 256 GB Mac. The step everyone does first, turning on Download Originals, is the one step that cannot work. Here is the order that does, and why you move the library instead of exporting the photos."
pubDate: 2026-08-04
tldr: "Do not turn on Download Originals to this Mac first, because the originals land on the startup disk. Copy the Photos Library bundle to an external APFS drive, hold Option while opening Photos and pick the library on that drive, set it as the System Photo Library, and only then turn on Download Originals so the full size files download onto the external drive. Move the library rather than exporting photos: edits, the video half of every Live Photo, RAW and JPEG pairs, HEIC originals, albums, faces, dates, and locations all live inside the bundle, and exporting flattens them. Cancel iCloud+ only after the download has finished and you have a second copy of the drive."
howto:
  name: "Move an iCloud photo library to an external drive"
  steps:
    - name: "Format the external drive as APFS"
      text: "In Disk Utility, erase the drive as APFS, or Mac OS Extended (Journaled) for an older Mac. A Photos library cannot live on exFAT, FAT32, or a network share."
    - name: "Quit Photos and copy the library across"
      text: "Find Photos Library.photoslibrary in your Pictures folder and drag it to the external drive. Copy, do not move, until the new copy is confirmed working. A 1 TB library takes hours."
    - name: "Open the copy with the Option key held"
      text: "Hold Option and launch Photos. In the library chooser, select the library on the external drive and open it."
    - name: "Make it the System Photo Library"
      text: "Photos > Settings > General > Use as System Photo Library. Only the System Photo Library can use iCloud Photos, so this has to happen before the download."
    - name: "Now turn on Download Originals to this Mac"
      text: "Photos > Settings > iCloud > Download Originals to this Mac. Full size files now flow into the library on the external drive, not onto the startup disk."
    - name: "Wait for the download to finish and verify"
      text: "Watch the status at the bottom of the Library view until it reports everything is up to date, then compare the library size in Finder against your iCloud storage usage. Keep the Mac awake and the drive connected throughout."
    - name: "Make a second copy, then cancel iCloud+"
      text: "Back the drive up to a second disk or Time Machine. Only once two copies exist should you downgrade or cancel your iCloud+ plan."
faq:
  - q: "Why can't I just turn on Download Originals first?"
    a: "Because Download Originals downloads into whichever library is currently in use, and that is the one on your startup disk. On a 256 GB Mac with a 1 TB library, the disk fills, the download stalls, and macOS starts warning about free space. Moving the library first is what redirects the download onto the external drive."
  - q: "Can the photo library live on an exFAT drive?"
    a: "No. Photos requires APFS or Mac OS Extended (Journaled). exFAT and FAT32 do not support the permissions and hard links inside the library bundle, and network volumes are not supported either. Erase the drive as APFS in Disk Utility before you copy anything to it."
  - q: "Should I export my photos instead of moving the library?"
    a: "No, exporting is the lossy path. Edits, the video half of every Live Photo, RAW and JPEG pairs, albums, faces, keywords, and often the capture date and location are properties of the library bundle rather than of the exported files. Moving the whole bundle keeps all of it, and you can still export individual photos later."
  - q: "What happens to Live Photos?"
    a: "Inside the library each Live Photo is a still image plus a short QuickTime movie, kept together by the library database. Move the bundle and they stay paired. Export them and you get either a still with no motion, or a still and a separate .mov file that nothing outside Photos knows are related."
  - q: "How do I know the download actually finished?"
    a: "Photos shows the sync status at the bottom of the Library view, and it should say the library is up to date with no pending downloads. As a second check, the library size in Finder should be roughly your iCloud Photos usage. A library that is still mostly thumbnails will be dramatically smaller than the iCloud figure."
  - q: "What happens to my photos when I cancel iCloud+?"
    a: "iCloud Photos stops syncing, and content above your new storage limit is eventually removed from iCloud. Apple gives a grace period, commonly cited as 30 days, but there is no reason to lean on it. Finish the download, verify it, make a second copy, and only then downgrade."
  - q: "Will my photos still be HEIC after this?"
    a: "Yes. Moving a library does not change formats, so iPhone photos stay HEIC and RAW files stay RAW. That is fine on a Mac, which reads both. It becomes a problem the day you hand the drive to a Windows PC, a TV, or a photo frame, and the answer then is to convert copies rather than the library."
---

The setup is common enough that it comes up on Apple support forums every
week. A photo library that has grown past 1 TB, a MacBook Air with a 256 GB
SSD, an iCloud+ bill you would like to stop paying, and an external drive
bought for the purpose. Every photo is safely in iCloud. All that is left is
to get them onto the drive.

Then the first step everyone tries, turning on "Download Originals to this
Mac", fails in a way that makes the whole plan look impossible.

## Why that first step cannot work

`Download Originals to this Mac` does exactly what it says: it replaces the
space-saving thumbnails in your **current** library with full size files. The
current library is the one in your `Pictures` folder, on the startup disk.
Asking a 256 GB disk to hold 1 TB of originals ends with a full disk, a
stalled download, and macOS complaining it is out of space.

Nothing about that failure means the photos cannot be moved. It means the
library has to be somewhere else *before* the originals are requested. The
order is the entire trick.

## The order that works

1. **Format the drive as APFS.** Disk Utility, erase, APFS. Mac OS Extended
   (Journaled) is fine on older machines. Not exFAT, not FAT32, not a network
   share, none of which can hold a Photos library.
2. **Quit Photos, then copy `Photos Library.photoslibrary` from `Pictures`
   to the drive.** Copy it, do not move it, until the copy is proven to work.
   Expect this to take hours at 1 TB, and expect the Mac to need to stay awake
   for it.
3. **Hold Option while launching Photos.** The library chooser appears.
   Select the library on the external drive.
4. **Photos > Settings > General > Use as System Photo Library.** Only the
   System Photo Library is allowed to use iCloud Photos, so skipping this
   leaves the download pointed at the old library.
5. **Now** turn on **Photos > Settings > iCloud > Download Originals to this
   Mac.** Full size files start landing in the library on the external drive.
6. **Let it finish.** Keep the drive connected and the Mac awake. This is the
   long part.
7. **Make a second copy, then cancel iCloud+.**

Steps 4 and 5 are the ones people reverse, and reversing them is what fills
the startup disk.

## Move the library, do not export the photos

The other common instinct is to select everything in Photos and export it to
a folder on the drive. It feels tidier, and it quietly throws away most of
what you were trying to keep.

A `.photoslibrary` bundle is a database plus originals. Inside it:

- **Edits are non-destructive.** The library holds the untouched original
  plus a list of adjustments. Export gives you a flattened file and the
  ability to revert is gone.
- **A Live Photo is two files**, a still and a short QuickTime movie, tied
  together by the database. Exported, they become either a plain still or a
  still plus an unrelated-looking `.mov`.
- **RAW and JPEG pairs collapse.** A photo shot as RAW+JPEG is one item with
  two files behind it. Export hands you whichever one the export options
  picked.
- **Albums, faces, keywords, memories, and favorites are database rows.**
  They do not exist in an exported folder at all.
- **Capture date and location survive only as EXIF**, which some export
  settings strip, and which nothing restores afterwards.

Moving the bundle keeps every one of those, and you can still export
individual photos any time later. Exporting first is the one decision here
that cannot be undone.

## Verifying before you cancel anything

Two checks, both quick:

- The bottom of the **Library** view in Photos reports sync status. It should
  say the library is up to date, with no "Downloading" counter left.
- In Finder, **Get Info** on the library. Its size should be in the same
  neighbourhood as your iCloud Photos usage in System Settings. A library
  still full of thumbnails is smaller by an order of magnitude, and that
  difference is easy to spot.

Only after both look right should the iCloud+ plan change. When you downgrade,
iCloud Photos stops syncing and content over the new limit is removed from
iCloud after a grace period. That grace period is not a backup.

## One drive is not a backup

When the download finishes and iCloud is cancelled, that external drive holds
the only copy of every photo you have. It is a single mechanical or flash
device, usually carried around, usually unplugged and replugged daily.

Time Machine onto a second drive, a clone of the first drive, or a cheaper
cloud tier used purely as an archive all work. The specific choice matters
much less than there being two of something before iCloud stops holding a
copy for you.

## The format question, later

Moving a library changes no formats. iPhone photos stay HEIC, RAW stays RAW,
and on the Mac that is invisible: Preview, Photos, and Quick Look read all of
it.

It stops being invisible the first time the drive meets something that is not
a Mac. Windows needs codec extensions before File Explorer will even show a
HEIC thumbnail, and TVs, digital photo frames, and car head units frequently
refuse HEIC outright. The fix at that point is to convert **copies**, not the
library:

- For a handful of photos, select them in Photos and use **File > Export >
  Export Unmodified Original**, or export as JPEG directly.
- For a whole folder that has already been copied to a Windows machine, this
  site's app,
  [HEIC Batch Converter](https://apps.microsoft.com/detail/9pmm2c5ch29k)
  (disclosure: written by the author of this site), converts folders and
  subfolders to JPG in one pass and ships its own decoder, so the Windows
  codec extensions are not needed. It is Windows only. On the Mac side,
  Photos' own export or Preview's batch export does the same job.

Either way, keep the library as the master and treat JPGs as disposable
copies for whatever device needed them.

## Bottom line

| Question | Answer |
|---|---|
| Turn on Download Originals first? | No. That is the step that fills the startup disk |
| Correct order | Copy library to drive, Option-launch, set as System Photo Library, then Download Originals |
| Drive format | APFS or Mac OS Extended (Journaled). Not exFAT, not a network share |
| Export instead of moving? | No. Exporting drops edits, Live Photo videos, RAW pairs, albums, and sometimes dates |
| When to cancel iCloud+ | After the download completes, is verified, and a second copy exists |
| Photos stay HEIC? | Yes. Convert copies later if a non-Apple device needs JPG |
