# Design System Specification: The Ethereal Utility

## 1. Overview & Creative North Star
**Creative North Star: "The Digital Curator"**

This design system moves beyond the utility of a standard desktop app to create an environment that feels curated, lightweight, and atmospheric. While traditional utility software often feels rigid and mechanical, this system leverages Windows 11 Fluent principles—specifically **Mica** and **Acrylic** textures—to create a sense of depth and focus. 

The aesthetic is built on **Tonal Layering** rather than structural containment. We reject the "boxed-in" feel of legacy enterprise software in favor of an editorial layout that breathes. By using intentional asymmetry in sidebars and overlapping surface containers, we guide the user’s eye through data density with the grace of a high-end magazine.

---

## 2. Colors: The Atmospheric Palette
We utilize a sophisticated range of neutrals and soft blues to mimic the behavior of light passing through glass.

### Surface Hierarchy & The "No-Line" Rule
**Explicit Instruction:** Do not use 1px solid borders to define sections. All boundaries must be established through color shifts or stacking.
- **Base Layer:** Use `surface` (#f9f9f9) for the primary application window background.
- **Nesting Logic:** 
    - Use `surface_container_low` (#f2f4f4) for secondary content areas like sidebars.
    - Use `surface_container_lowest` (#ffffff) for the most prominent "floating" cards or data tables.
    - Use `surface_container_high` (#e4e9ea) for interactive hover states or deeply nested utility panels.

### The "Glass & Gradient" Rule
To achieve a premium "Mica" effect, use `surface_variant` (#dde4e5) with a 60% opacity and a 30px Backdrop Blur. 
- **Signature CTA Texture:** For primary action buttons, apply a linear gradient from `primary` (#005eb1) to `primary_dim` (#00529c) at a 135-degree angle. This adds "soul" and depth that prevents the UI from feeling flat.

---

## 3. Typography: Editorial Authority
We utilize **Inter** (as a high-performance alternative to Segoe UI Variable for cross-platform precision) to create a clear informational hierarchy.

- **Display & Headlines:** Use `display-md` (2.75rem) for main dashboard headers. The large scale provides a sense of "Editorial Authority."
- **Titles:** Use `title-lg` (1.375rem) for section headers within cards.
- **Body:** `body-md` (0.875rem) is our workhorse. Ensure a line-height of 1.5 to maintain readability in data-heavy utility views.
- **Labels:** `label-md` (0.75rem) should be used in all-caps with a 0.05rem letter spacing for utility metadata to distinguish it from interactive body text.

---

## 4. Elevation & Depth: Tonal Layering
In this system, "Elevation" is a state of light, not a drop shadow.

- **The Layering Principle:** Depth is achieved by placing a `surface_container_lowest` card on a `surface_container_low` background. This creates a soft, natural lift.
- **Ambient Shadows:** For floating modals or context menus, use a 32px blur, 0px offset, and 4% opacity of the `on_surface` color. It should feel like a soft glow of shadow rather than a hard edge.
- **The "Ghost Border" Fallback:** If a high-contrast environment requires more definition, use a "Ghost Border": `outline_variant` (#adb3b4) at **15% opacity**. Never use a 100% opaque border.
- **Glassmorphism:** Apply a `surface_tint` at 5% opacity over `surface_container_lowest` for elements that need to feel "closer" to the user, creating a frosted-glass refraction.

---

## 5. Components: Precision Built

### Buttons
- **Primary:** Gradient fill (`primary` to `primary_dim`), `on_primary` text, `DEFAULT` (0.5rem/8px) corner radius.
- **Secondary:** `secondary_container` fill with `on_secondary_container` text. No border.
- **Tertiary:** No background. Use `primary` color for text. High-contrast hover state using `surface_container_high`.

### Data Tables (The "Fluid Grid")
- **Forbid Dividers:** Do not use horizontal lines between rows. Use alternating row colors (`surface` and `surface_container_low`) or simply 16px of vertical whitespace.
- **Header:** Use `label-md` in `on_surface_variant` for column headers.
- **Row Hover:** On hover, transition the background to `surface_container_highest` with an 8px corner radius.

### Sidebars & Navigation
- **Asymmetric Layout:** The sidebar should utilize `surface_container_low` and extend the full height of the app, but with a 24px inner padding to allow navigation items to "float."
- **Active State:** Use a "pill" indicator in `primary_container` with `on_primary_container` text.

### Input Fields
- **Container:** `surface_container_highest` background.
- **Focus State:** Instead of a thick border, use a 2px bottom-accent in `primary` and a subtle 4% `primary` glow/shadow around the entire input.

---

## 6. Do’s and Don’ts

### Do:
- **Do** use `lg` (1rem) corner radius for large containers and `DEFAULT` (0.5rem) for smaller buttons to create a nested "squircle" aesthetic.
- **Do** use `tertiary` colors for non-critical status indicators (e.g., "Pending" or "Draft").
- **Do** allow content to bleed into the "Mica" title bar area for a modern, integrated feel.

### Don’t:
- **Don’t** use pure black (#000000) for text. Always use `on_surface` (#2d3435) to maintain the soft, premium feel.
- **Don’t** use 1px borders to separate the sidebar from the main content; use the shift from `surface_container_low` to `surface`.
- **Don’t** stack more than three levels of surface containers (Base > Section > Card). Any more will muddy the visual hierarchy.