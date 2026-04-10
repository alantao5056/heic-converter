# HEIC Batch Converter — UI Design

A clean, flat Windows desktop application mockup for batch converting HEIC image files to common formats. Built as a single-file HTML prototype.

---

## Overview

This prototype presents a two-panel layout: a fixed-width settings sidebar on the left and a file management workspace on the right. The design prioritizes clarity and efficiency — all controls are immediately visible, and conversion progress is always in view.

---

## Layout

### Sidebar (232px fixed)

The left panel is divided into three zones:

**Header** — App name and version, separated from the settings area by a horizontal divider.

**Settings** — Six labeled control groups, each with a blue accent section label (`#185FA5`, weight 600):

| Section | Control | Notes |
|---|---|---|
| Search scope | Checkbox | Toggle subfolder scanning |
| Output format | Button grid (2×2) | JPG / PNG / GIF / BMP |
| JPG quality | Slider | Visible only when JPG is selected |
| Conflict resolution | Dropdown | Generate unique name / Replace / Ignore |
| Original file handling | Dropdown | Keep / Delete / Move to… |
| *(Move to… path)* | Path input + folder icon | Shown conditionally |

**Stats row** — Pinned to the bottom of the sidebar, same height as the footer (44px), separated by a border that visually aligns with the footer divider on the right panel. Displays two equal-width chips: `✓ N done` (green) and `✕ N failed` (red).

---

### Main panel

**Top bar** — Source folder and target folder path inputs, each with a folder-picker icon button.

**Action bar** — Right-aligned "Start conversion" button in `#2E86C1` with a subtle box-shadow for prominence.

**File table** — Four columns with fixed widths tuned to content:

| Column | Width | Notes |
|---|---|---|
| Path | Flexible (fills remaining) | Long paths truncated with ellipsis; full path in tooltip |
| Original name | 148px | |
| Convert name | 136px | Rendered as a blue link |
| Status | 136px | Colored badge |

**Footer** — Progress bar (flexible width) + file count + percentage, all vertically centered at 44px height.

---

## Visual Design

### Color palette

| Role | Value |
|---|---|
| Accent / links | `#185FA5` |
| Primary button | `#2E86C1` |
| Sidebar background | `#f3f3f0` |
| Main background | `#ffffff` |
| Borders | `rgba(0,0,0,0.12)` |
| Section labels | `#185FA5` |

### Status badges

| State | Background | Text |
|---|---|---|
| Completed | `#EAF3DE` | `#3B6D11` |
| Converting | `#E6F1FB` | `#185FA5` |
| Pending | `#FAEEDA` | `#854F0B` |
| Done chip | `#EAF3DE` | `#3B6D11` |
| Failed chip | `#FCEBEB` | `#A32D2D` |

### Typography

- Font stack: `-apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif`
- App title: 16px / weight 500
- Section labels: 11px / weight 600 / accent blue
- Body / controls: 12–13px / weight 400
- Table headers: 10px / weight 500 / uppercase / letter-spacing 0.06em

### Border radius

All interactive elements (inputs, buttons, dropdowns, badges, chips) use a uniform `4px` border radius throughout the interface.

---

## Interactive Behaviors

- **Output format buttons** — Selecting a format highlights it with a blue border and light blue fill. Selecting any format other than JPG hides the JPG quality slider.
- **JPG quality slider** — Dragging updates both the percentage label and the visual track/thumb in real time.
- **Original file handling** — Selecting "Move to…" reveals an inline path input with a folder-picker icon directly below the dropdown.
- **Table rows** — Hover state applies a subtle background tint (`#f3f3f0`).
- **Start conversion button** — Hover darkens the button to `#1a6fa0`.

---

## File

The prototype is delivered as a single self-contained HTML file (`heic_batch_converter.html`) with all styles and scripts inlined. No external dependencies or build steps required — open directly in any modern browser.
