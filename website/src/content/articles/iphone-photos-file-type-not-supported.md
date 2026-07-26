---
title: "Smart TV, Fridge, or Photo Frame Says 'File Type Not Supported' for iPhone Photos"
description: "Some iPhone photos load on your smart display and others are rejected with no pattern. Even exporting them as JPEG on a Mac doesn't always help. Here's what embedded photo viewers actually choke on, and the one recipe that gets every photo accepted."
pubDate: 2026-07-26
tldr: "Embedded photo viewers in smart TVs, fridges, and digital frames run small decoders with hard limits. If some iPhone photos load and others fail, the container is rarely the whole story: the usual culprits are 48-megapixel resolution, an HDR gain map, a Display P3 color profile, or progressive JPEG encoding, all of which survive a 'Save as JPEG' export from macOS Preview. The fix is to produce a deliberately boring image: baseline JPEG, 8-bit, sRGB, roughly 3000 pixels on the long edge, metadata trimmed."
howto:
  name: "Make iPhone photos that an embedded photo viewer will accept"
  steps:
    - name: "Compare a working photo with a failing one"
      text: "On a Mac run 'sips -g all good.jpg bad.jpg' and compare pixelWidth, pixelHeight, bitsPerSample, and profile. On Windows check Properties > Details, or run 'magick identify -verbose' if ImageMagick is installed."
    - name: "Convert from the original HEIC, not from a re-export"
      text: "Convert the original .HEIC file to JPG rather than re-exporting an already-exported file. Converting from the source drops the HDR gain map and other auxiliary images instead of carrying them forward."
    - name: "Force sRGB and a sane resolution"
      text: "Resize to about 3000 pixels on the long edge and convert the color profile to sRGB. On a Mac: sips -s format jpeg -Z 3000 --matchTo '/System/Library/ColorSync/Profiles/sRGB Profile.icc' input.HEIC --out output.jpg"
    - name: "Test one file before converting the library"
      text: "Copy a single converted photo to the device and confirm it displays. Only then batch-convert the rest, so you don't redo hundreds of files with the wrong settings."
faq:
  - q: "Why do some iPhone photos work on my smart display and others don't?"
    a: "Because the photos are not all the same. Photos shot at 48 megapixels, HDR photos carrying a gain map, and photos in Display P3 all differ from an ordinary 12-megapixel sRGB shot, and embedded decoders have hard limits on resolution, bit depth, and color handling. The ones that load are the ones that happen to stay inside those limits."
  - q: "I exported the photo as JPEG on my Mac and it still won't load. Why?"
    a: "Preview's export changes the container, not the characteristics inside it. The exported JPEG typically keeps the original pixel dimensions and the Display P3 profile, and Apple can write an HDR gain map into JPEG as well. If a Preview export fails, that is useful information: it proves the problem is not simply that the file was HEIC."
  - q: "What settings should I convert to?"
    a: "Baseline (non-progressive) JPEG, 8-bit, sRGB color, around 3000 pixels on the long edge, with metadata trimmed. This is well within the limits of nearly every embedded viewer, and on a screen that size no one can see the difference."
  - q: "Is it the file size that's too big?"
    a: "Sometimes, but pixel dimensions cause more failures than megabytes. A decoder that allocates a buffer per image can refuse a 48-megapixel photo that is only 5 MB on disk. Resizing fixes both at once."
  - q: "Could it be the filename or the USB drive instead?"
    a: "It can. Some appliances reject long names, non-ASCII characters, or deeply nested folders, and many read FAT32 or exFAT only. If every photo fails rather than some, suspect the drive or the folder layout before the photos."
---

You put a folder of iPhone photos on a USB stick or push them to a smart
fridge, a TV, or a digital photo frame. Some appear. Others come back as
*"File type not supported"*. They came off the same phone, on the same day,
in the same folder. Nothing about which ones fail looks like a pattern.

There is a pattern. It just isn't the file extension.

## What an embedded photo viewer actually is

The gallery on an appliance is not Windows Photos. It's a small decoder
inside firmware that was specified years before the photo was taken, sized
for a fixed memory budget, and tested against ordinary JPEGs. It typically
supports baseline JPEG and PNG, allocates a buffer based on the image's pixel
dimensions, and has no ICC color management at all.

Modern iPhone photos violate several of those assumptions at once, and which
assumptions get violated depends on how each individual photo was shot. That
is why the failures look random.

## The four things that actually trip it, in order

**1. It's still HEIC.** The obvious one. Most appliance galleries never
implemented HEIF, and the fix is to convert. If *every* photo fails, start
here.

**2. Resolution.** A 48-megapixel photo from a Pro iPhone is 8064 x 6048.
Many embedded decoders cap out well below that, and a decoder that allocates
its buffer up front simply refuses rather than degrading. This is the single
most common reason that *some* photos work and others don't, because the
48-megapixel setting only applies to some shots. Note that file size in
megabytes is a poor proxy: a heavily compressed 48-megapixel photo can be
smaller on disk than a 12-megapixel one and still be rejected.

**3. HDR gain map.** Recent iPhones store HDR photos as a normal image plus
an auxiliary gain map that brightens highlights on displays that understand
it. A decoder that walks into an unexpected auxiliary image or an unfamiliar
marker segment can bail out instead of ignoring it. Crucially, this survives
a JPEG export, because Apple writes gain maps into JPEG too.

**4. Display P3 color and progressive encoding.** A non-sRGB profile more
often produces *wrong colors* than an outright refusal, so treat this as the
explanation for washed-out or oversaturated photos rather than for rejections.
Progressive JPEG encoding, on the other hand, does cause hard refusals on
decoders that only implement baseline.

## Why "Export as JPEG" on a Mac often doesn't help

This is the step where most people conclude the device is broken. Preview
exports a real JPEG, the device still refuses it, and the obvious conclusion
is that format was never the issue.

What actually happened is that Preview changed the container and kept
everything else: the same pixel dimensions, the same Display P3 profile, and
potentially the gain map. Points 2 through 4 above all passed straight
through.

So a failed Preview export is not a dead end, it's a diagnosis. It rules out
"it was HEIC" and points at resolution, HDR, or color.

## Compare a good photo with a bad one

Before converting anything, find out which limit you're hitting. On a Mac:

```bash
sips -g all good.jpg bad.jpg
```

Look at four lines for each file:

- `pixelWidth` / `pixelHeight`: is the failing one dramatically larger?
- `bitsPerSample`: 8 is safe, more is not.
- `profile`: `sRGB IEC61966-2.1` is safe, `Display P3` is not.
- `hasAlpha`: transparency in a JPEG-expecting gallery is a problem.

On Windows, right-click > **Properties > Details** shows dimensions and bit
depth. If you have ImageMagick, `magick identify -verbose photo.jpg` shows
the profile and whether the file is progressive ("Interlace: JPEG" means
progressive).

Usually one of these differs cleanly between the file that works and the one
that doesn't, and that's your answer.

## The recipe: make a deliberately boring photo

The target is a file that would have been unremarkable in 2010: baseline
JPEG, 8-bit, sRGB, moderate resolution, no auxiliary images.

**On a Mac, one file at a time:**

```bash
sips -s format jpeg -Z 3000 \
  --matchTo '/System/Library/ColorSync/Profiles/sRGB Profile.icc' \
  input.HEIC --out output.jpg
```

`-Z 3000` caps the long edge at 3000 pixels, and `--matchTo` converts the
color to sRGB rather than merely relabeling it.

**On Windows, for a whole library:** convert the original `.HEIC` files, not
files you already exported. Converting from the source means only the primary
image is read, so the gain map and other auxiliary images are dropped rather
than carried forward, and the output is a plain baseline 8-bit JPEG. The free
[HEIC Batch Converter](https://apps.microsoft.com/detail/9pmm2c5ch29k)
(disclosure: made by the author of this site) does this for entire folders
offline, with a quality slider and your subfolder structure preserved.

To be straight about the limits: converting handles the container, the bit
depth, the gain map, and progressive encoding, but it does not resize your
photos, and a color profile present in the original is carried into the JPG.
If your appliance is refusing on resolution or on Display P3 specifically,
you need a resize and a profile conversion too, which is what the `sips`
command above does and what tools like XnConvert or ImageMagick do in bulk on
Windows.

**Test one file on the device before converting hundreds.** It costs thirty
seconds and saves redoing the whole library at the wrong settings.

## When it isn't the photos at all

If *every* photo fails, including plain screenshots, stop looking at image
internals:

- **The drive.** Many appliances read FAT32 or exFAT only, and ignore NTFS
  entirely.
- **The folder layout.** Some galleries scan one level deep, or cap the
  number of files per folder.
- **The filenames.** Long names, emoji, and non-ASCII characters get rejected
  by firmware that assumed 8.3-era naming.
- **Live Photos.** These arrive as a `.HEIC` plus a `.MOV` with the same
  base name. The video half is not a photo, and some galleries report the
  pair as unsupported rather than showing the still.

## Bottom line

| Symptom | Most likely cause | Fix |
|---|---|---|
| Every photo fails | Format, drive, or folder layout | Convert to JPG; check FAT32/exFAT |
| Some fail, some work | Resolution (48 MP shots) | Resize to ~3000 px long edge |
| Preview's JPEG export also fails | Gain map, profile, or size survived | Convert from the original HEIC |
| Colors look wrong but it loads | Display P3 profile | Convert to sRGB |
| A `.MOV` twin sits next to it | Live Photo | Keep the HEIC, drop the MOV |

For the general case of HEIC files being refused on Windows rather than on an
appliance, see
[why HEIC files won't open on Windows](/articles/heic-files-wont-open-windows/).
