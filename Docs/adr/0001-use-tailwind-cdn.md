# Use Tailwind CSS via CDN

**Status**: Accepted
**Date**: 2026-01-19

## Context
We are participating in a hackathon and need to iterate on the UI extremely fast.
Setting up a full Node.js/PostCSS build pipeline for Tailwind adds complexity and requires build steps that might slow down the "edit-refresh" cycle during rapid prototyping.
We also want to avoid committing large node_modules or generated CSS files if possible.

## Decision
We will use Tailwind CSS via the official CDN script (`<script src="https://cdn.tailwindcss.com"></script>`) injected directly into `_Layout.cshtml`.

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
- We accept the performance hit for development speed.
- If we go to production later, we will swap the CDN for a proper PostCSS build pipeline.
