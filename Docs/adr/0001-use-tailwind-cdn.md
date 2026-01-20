# Use Tailwind CSS via CDN

## Context
Participating in a hackathon requires rapid UI iteration.
Setting up a full Node.js/PostCSS build pipeline for Tailwind adds complexity and requires build steps that might slow down the "edit-refresh" cycle during rapid prototyping.
Avoiding large node_modules or generated CSS files in the repository is also desirable.

## Decision
Use Tailwind CSS via the official CDN script (`<script src="https://cdn.tailwindcss.com"></script>`) injected directly into `_Layout.cshtml`.

## Consequences
**Positive**:
- Zero configuration required.
- Instant feedback (no build step).
- Easy to use arbitrary values `w-[245px]` without config.

**Negative**:
- Does not work offline (requires internet).
- Performance is lower than a compiled CSS file (runtime parsing).
- No IDE intellisense for custom config unless explicitly configured.

**Mitigation**:
- The performance hit is accepted for development speed.
- If production deployment is required later, the CDN can be swapped for a proper PostCSS build pipeline.
