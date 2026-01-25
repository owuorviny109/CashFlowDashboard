---
description: Create a new Architecture Decision Record (ADR).
---

# Create Architecture Decision Record

Use this workflow to document significant architectural decisions.

## Steps
1.  **Determine ADR Number**: Check `Docs/adr/` for the next available number (e.g., `0003`).
2.  **Create File**: Create `Docs/adr/XXXX-title-slug.md`.
3.  **Content**: Use the following strict template:

```markdown
# [Title]

**Status**: [Proposed | Accepted | Deprecated]
**Date**: YYYY-MM-DD

## Context
What is the issue that we're seeing that is motivating this decision or change?

## Decision
What is the change that we're proposing and/or doing?

## Consequences
What becomes easier or more difficult to do and any risks introduced by this change?
```
