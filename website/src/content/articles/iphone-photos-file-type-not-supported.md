---
title: "Smart TV, Fridge, or Photo Frame Says 'File Type Not Supported' for iPhone Photos"
description: "Some iPhone photos load on your smart display and others are rejected with no pattern. Even exporting them as JPEG on a Mac doesn't always help. The usual limit is 4096 pixels on the long edge, which a 24-megapixel iPhone photo already exceeds. Here's how to check, and the recipe that gets every photo accepted."
pubDate: 2026-07-26
updatedDate: 2026-07-27
tldr: "Embedded photo viewers in smart TVs, fridges, and digital frames run small decoders with hard limits, and the limit that catches most people is 4096 pixels on the long edge, the standard maximum texture size for this class of hardware. An iPhone shooting at its 24-megapixel default already writes 4284 x 5712, so it crosses that line straight out of the camera. Resolution is the first thing to check; an HDR gain map, a Display P3 color profile, or progressive JPEG encoding explain the rest, and all of them survive a 'Save as JPEG' export from macOS Preview. The fix is to produce a deliberately boring image: baseline JPEG, 8-bit, sRGB, under 4096 pixels on the long edge, metadata trimmed."
howto:
  name: "Make iPhone photos that an embedded photo viewer will accept"
  steps:
    - name: "Compare a working photo with a failing one"
      text: "On a Mac run 'sips -g all good.jpg bad.jpg' and compare pixelWidth, pixelHeight, bitsPerSample, and profile. On Windows check Properties > Details, or run 'magick identify -verbose' if ImageMagick is installed."
    - name: "Convert from the original HEIC, not from a re-export"
      text: "Convert the original .HEIC file to JPG rather than re-exporting an already-exported file. Converting from the source drops the HDR gain map and other auxiliary images instead of carrying them forward."
    - name: "Get the long edge under 4096 pixels"
      text: "Resize so the longest side is comfortably below 4096 pixels, around 3000 to 4000, and convert the color profile to sRGB. Without a terminal: open the photos in Preview on a Mac, select them all in the sidebar, choose Tools > Adjust Size, set the longest side, then File > Export Selected Images as JPEG. With a terminal: sips -s format jpeg -Z 3000 --matchTo '/System/Library/ColorSync/Profiles/sRGB Profile.icc' input.HEIC --out output.jpg"
    - name: "Stop the problem at the camera"
      text: "On an iPhone that shoots 24 or 48 megapixels, go to Settings > Camera > Formats > Photo Mode and choose 12 MP. That writes 4032 x 3024, which stays under the 4096-pixel limit for every future shot."
    - name: "Test one file before converting the library"
      text: "Copy a single converted photo to the device and confirm it displays. Only then batch-convert the rest, so you don't redo hundreds of files with the wrong settings."
faq:
  - q: "Why do some iPhone photos work on my smart display and others don't?"
    a: "Because the photos are not all the same. Photos shot at 24 or 48 megapixels, HDR photos carrying a gain map, and photos in Display P3 all differ from an ordinary 12-megapixel sRGB shot, and embedded decoders have hard limits on resolution, bit depth, and color handling. The ones that load are the ones that happen to stay inside those limits."
  - q: "Is there an exact resolution limit?"
    a: "In practice the line usually falls at 4096 pixels on the long edge, which is the standard maximum texture size for the class of GPU used in these panels. Measurements reported from a Samsung Family Hub fridge match it exactly: 4284 x 5712 and 3549 x 4732 were both rejected, while 3024 x 4032 and 2755 x 3673 both displayed. Megapixels are the wrong unit to think in, since a wide 20-megapixel panorama can pass while a taller 17-megapixel crop fails."
  - q: "My photos are only 24 megapixels, not 48. Why do they still fail?"
    a: "Because 24 megapixels on an iPhone means 4284 x 5712, and the long edge is already past 4096. The 24-megapixel mode is the default on recent Pro models, so photos can fail on an appliance without anyone having deliberately turned on a high-resolution setting."
  - q: "I exported the photo as JPEG on my Mac and it still won't load. Why?"
    a: "Preview's export changes the container, not the characteristics inside it. The exported JPEG typically keeps the original pixel dimensions and the Display P3 profile, and Apple can write an HDR gain map into JPEG as well. If a Preview export fails, that is useful information: it proves the problem is not simply that the file was HEIC."
  - q: "What settings should I convert to?"
    a: "Baseline (non-progressive) JPEG, 8-bit, sRGB color, around 3000 pixels on the long edge, with metadata trimmed. That leaves room under the 4096-pixel limit, is well within the limits of nearly every embedded viewer, and on a screen that size no one can see the difference."
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

**2. Resolution, and the number is usually 4096.** This is the single most
common reason that *some* photos work and others don't, and unlike the other
three it has a specific threshold you can check against.

The limit that keeps showing up is **4096 pixels on the long edge**. That
number is not arbitrary: 4096 is the standard maximum texture size for the
class of GPU that goes into TVs, fridge panels, and photo frames, so an image
wider or taller than that has nowhere to be uploaded and the decoder refuses
rather than degrading.

A set of measurements reported from a Samsung Family Hub lines up with it
exactly:

| Photo dimensions | Long edge | Result |
|---|---|---|
| 4284 x 5712 | 5712 | rejected |
| 3549 x 4732 | 4732 | rejected |
| 3024 x 4032 | 4032 | displayed |
| 2755 x 3673 | 3673 | displayed |

Two things follow from that. First, **megapixels are the wrong unit**: a wide
20-megapixel panorama can pass while a taller 17-megapixel crop fails, because
only the long edge matters. Second, and this is what catches people out, **you
do not need to be shooting at 48 megapixels to cross the line**. The
24-megapixel mode that recent Pro iPhones use *by default* writes
4284 x 5712, which is already over. A 48-megapixel shot at 8064 x 6048 is
simply further over.

File size in megabytes is a poor proxy for any of this. A heavily compressed
48-megapixel photo can be smaller on disk than a 12-megapixel one and still be
rejected, because the decoder allocates its buffer from the dimensions in the
header before it reads a single pixel.

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

- `pixelWidth` / `pixelHeight`: is either number above 4096 on the file that
  fails, and both below it on the file that works?
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

**On a Mac, without touching the terminal:** select the photos in Finder and
open them together in Preview, select them all in the sidebar with
`Cmd + A`, then **Tools > Adjust Size** and set the longest side to 4000 or
less. Adjust Size applies to every image selected in
the sidebar, so this is a batch operation, not a one-at-a-time one. Then
**File > Export Selected Images** and choose JPEG. This handles the limit that
causes most rejections without installing anything.

**On a Mac, one file at a time in the terminal:**

```bash
sips -s format jpeg -Z 3000 \
  --matchTo '/System/Library/ColorSync/Profiles/sRGB Profile.icc' \
  input.HEIC --out output.jpg
```

`-Z 3000` caps the long edge at 3000 pixels, comfortably under the 4096 limit,
and `--matchTo` converts the color to sRGB rather than merely relabeling it.

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

## Stop it happening to the next batch

Everything above is cleanup. The setting that prevents the most common cause
is on the phone: **Settings > Camera > Formats > Photo Mode**, set to
**12 MP**. That writes 4032 x 3024, which sits under the 4096-pixel limit
every time, and on a fridge or frame panel nobody will ever see the difference
between that and 24 megapixels.

If you would rather keep shooting at full resolution for everything else, the
alternative is to leave the camera alone and resize on the way to the
appliance, which is the recipe above.

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
| Some fail, some work | Long edge over 4096 px (24 MP is already 5712) | Resize to ~3000 px long edge, or shoot 12 MP |
| Preview's JPEG export also fails | Gain map, profile, or size survived | Convert from the original HEIC |
| Colors look wrong but it loads | Display P3 profile | Convert to sRGB |
| A `.MOV` twin sits next to it | Live Photo | Keep the HEIC, drop the MOV |

For the general case of HEIC files being refused on Windows rather than on an
appliance, see
[why HEIC files won't open on Windows](/articles/heic-files-wont-open-windows/).
