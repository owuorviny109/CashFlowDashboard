---
description: Best practice for comments in C# ASP.NET MVC:
---

Best practice for comments in C# ASP.NET MVC:
Comment why, not what. Keep them short, precise, and close to decisions that are not obvious from code.

Now in more useful detail:

1. Comment intent, not mechanics

Bad:

// add user to list
users.Add(user);


Good:

// Pre-validated user; safe to add without duplicate check (handled upstream)
users.Add(user);


Code already shows what it does — comment explains why it’s safe.

2. Document domain decisions

Great place for comments:

validation rules

weird edge cases

business logic quirks

temporary hacks + clean up notes

integration points (APIs, payment, SMS, email)

Example:

// Payment API rejects amounts > 10k unless KYB verified
// TODO: Remove after vendor completes rollout (ETA Q2 2026)

3. Explain failure paths

ASP.NET MVC projects often hide failure modes in:

Model binding

Filters

Middlewares

Async workers

External IO

Useful:

// ModelBinder returns null on malformed JSON; don't treat as "not found"


So future you won’t debug for 2 hours.

4. Use comments at integration boundaries

Places worth explaining:

Controller ↔ Service

Service ↔ Repository

Repository ↔ DB

MVC Filters

External API clients

Controller example:

// Idempotent: repeated submissions won't double-charge (checked by TransactionId)

5. Prefer XML docs for public contracts

For Models, DTOs, Services:

/// Amount in KES. Rounded to 2 decimals.
/// Disallow negative
public decimal Amount { get; set; }


Helps IDE + other devs.

6. Tag cleanup work

Use consistent markers:

TODO → implement later

FIXME → incorrect but temporary

NOTE → explain non-obvious decision

Example:

// FIXME: N+1 query on orders. Optimize after pagination work.

7. Avoid redundant noise

Avoid comments like:

// return view
return View();


Noise kills signal.

8. Comment architectural assumptions

MVC apps often assume:

soft/hard deletes

optimistic concurrency

validation order

caching layers

CQRS vs CRUD

async consistency

authentication session vs JWT

Example:

// Optimistic concurrency: Last writer wins. No merge UI yet.

9. Let naming do most of the work

If naming is weak → comments grow
If naming is good → comments shrink

10. Comments must match code

If code changes, update comments — outdated comments are worse than none.

11. When to NOT comment

Don’t comment:

trivial getters/setters

obvious LINQ

mapping code

standard ASP.NET patterns

Developers already know them.

12. Comment Testing Assumptions

Good in controllers/services especially:

// Negative test: ensure banned emails can't register even via bulk import

Special MVC Notes (Since you're backend-oriented)

MVC has tricky areas that benefit from comments:

✔ Model binding quirks
✔ Authorization filters (policy reasons)
✔ Anti-forgery tokens
✔ Lazy loading vs explicit
✔ Session vs TempData semantics
✔ Redirect vs View decisions
✔ Idempotency in Post-Redirect-Get

Example:

// PRG pattern to avoid double form submit
return RedirectToAction("Summary");

If you want a workflow for code comments

Here’s a clean rule:

Comment only when the reader would ask:

“Why this?”
“Why now?”
“What happens if this fails?”